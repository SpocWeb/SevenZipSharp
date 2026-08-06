namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>Binary-tree match finder that locates the longest previous occurrence of the<br/>
    /// current byte sequence within the sliding <see cref="InWindow"/>, for LZ77-style<br/>
    /// back-reference selection during LZMA encoding.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 52 | <see cref="SetStream"/> | Attaches   as the input source, delegating to the base window. |
    /// | 55 | <see cref="ReleaseStream"/> | Detaches the current input stream, delegating to the base window. |
    /// | 59 | <see cref="Init"/> | Resets the base window and clears the hash table and cyclic buffer so the   binary tree starts matching from an empty history. |
    /// | 69 | <see cref="GetIndexByte"/> | Returns the byte located   positions from the current position. |
    /// | 73 | <see cref="GetMatchLen"/> | Returns how many bytes starting at   match the bytes found     positions earlier, up to   bytes. |
    /// | 76 | <see cref="GetNumAvailableBytes"/> | Returns the number of bytes still available for reading ahead of the current position. |
    /// | 383 | <see cref="MovePos"/> | Advances the current position by one byte, wrapping the cyclic tree buffer   position and normalizing all stored offsets once the position counter nears overflow. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt32"/> | Used as a field. |
    /// | <see cref="Byte"/> | Returned by a method. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 9f21df72c73e33c8e093e9180d23fbb202e7884fd2f3f5db287250c3d09d363a
    /// </code>
    /// </example>
    internal class BinTree : InWindow, IMatchFinder
    {

        /// <summary>Specifies the constant k B T2 Hash Size.</summary>
        private const UInt32 kBT2HashSize = 1 << 16;

        /// <summary>Specifies the constant k Empty Hash Value.</summary>
        private const UInt32 kEmptyHashValue = 0;

        /// <summary>Specifies the constant k Hash2 Size.</summary>
        private const UInt32 kHash2Size = 1 << 10;

        /// <summary>Specifies the constant k Hash3 Offset.</summary>
        private const UInt32 kHash3Offset = kHash2Size;

        /// <summary>Specifies the constant k Hash3 Size.</summary>
        private const UInt32 kHash3Size = 1 << 16;

        /// <summary>Specifies the constant k Max Val For Normalize.</summary>
        private const UInt32 kMaxValForNormalize = ((UInt32) 1 << 31) - 1;

        /// <summary>Specifies the constant k Start Max Len.</summary>
        private const UInt32 kStartMaxLen = 1;
        private UInt32 _cutValue = 0xFF;
        private UInt32 _cyclicBufferPos;
        private UInt32 _cyclicBufferSize;
        private UInt32[] _hash;

        private UInt32 _hashMask;
        private UInt32 _hashSizeSum;
        private UInt32 _matchMaxLen;

        private UInt32[] _son;

        private bool HASH_ARRAY = true;

        private UInt32 kFixHashSize = kHash2Size + kHash3Size;
        private UInt32 kMinMatchCheck = 4;
        private UInt32 kNumHashDirectBytes;

		#region IMatchFinder Members

		/// <summary>Attaches <paramref name='stream'/> as the input source, delegating to the base window.</summary>
		public new void SetStream(Stream stream) => base.SetStream(stream);

		/// <summary>Detaches the current input stream, delegating to the base window.</summary>
		public new void ReleaseStream() => base.ReleaseStream();

		/// <summary>Resets the base window and clears the hash table and cyclic buffer so the <br/>
		/// binary tree starts matching from an empty history.</summary>
		public new void Init()
        {
            base.Init();
            for (UInt32 i = 0; i < _hashSizeSum; i++)
                _hash[i] = kEmptyHashValue;
            _cyclicBufferPos = 0;
            ReduceOffsets(-1);
        }

		/// <summary>Returns the byte located <paramref name='index'/> positions from the current position.</summary>
		public new Byte GetIndexByte(Int32 index) => base.GetIndexByte(index);

		/// <summary>Returns how many bytes starting at <paramref name='index'/> match the bytes found <br/>
		/// <paramref name='distance'/> positions earlier, up to <paramref name='limit'/> bytes.</summary>
		public new UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit) => base.GetMatchLen(index, distance, limit);

		/// <summary>Returns the number of bytes still available for reading ahead of the current position.</summary>
		public new UInt32 GetNumAvailableBytes() => base.GetNumAvailableBytes();

		/// <inheritdoc />
		public void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                           UInt32 matchMaxLen, UInt32 keepAddBufferAfter)
        {
            if (historySize + 256 > kMaxValForNormalize)
            {
                throw new ArgumentException("historySize + 256 > kMaxValForNormalize", "historySize");
            }
            _cutValue = 16 + (matchMaxLen >> 1);

            UInt32 windowReservSize = (historySize + keepAddBufferBefore +
                                       matchMaxLen + keepAddBufferAfter)/2 + 256;

            base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

            _matchMaxLen = matchMaxLen;

            UInt32 cyclicBufferSize = historySize + 1;
            if (_cyclicBufferSize != cyclicBufferSize) {
	            _son = new UInt32[(_cyclicBufferSize = cyclicBufferSize)*2];
            }

            UInt32 hs = kBT2HashSize;

            if (HASH_ARRAY)
            {
                hs = historySize - 1;
                hs |= (hs >> 1);
                hs |= (hs >> 2);
                hs |= (hs >> 4);
                hs |= (hs >> 8);
                hs >>= 1;
                hs |= 0xFFFF;
                if (hs > (1 << 24)) {
	                hs >>= 1;
                }
                _hashMask = hs;
                hs++;
                hs += kFixHashSize;
            }
            if (hs != _hashSizeSum) {
	            _hash = new UInt32[_hashSizeSum = hs];
            }
        }

        /// <inheritdoc />
        public UInt32 GetMatches(UInt32[] distances)
        {
            UInt32 lenLimit;
            if (_pos + _matchMaxLen <= _streamPos) {
	            lenLimit = _matchMaxLen;
            } else
            {
                lenLimit = _streamPos - _pos;
                if (lenLimit < kMinMatchCheck)
                {
                    MovePos();
                    return 0;
                }
            }

            UInt32 offset = 0;
            UInt32 matchMinPos = (_pos > _cyclicBufferSize) ? (_pos - _cyclicBufferSize) : 0;
            UInt32 cur = _bufferOffset + _pos;
            UInt32 maxLen = kStartMaxLen; // to avoid items for len < hashSize;
            UInt32 hashValue, hash2Value = 0, hash3Value = 0;

            if (HASH_ARRAY)
            {
                UInt32 temp = CRC.Table[_bufferBase[cur]] ^ _bufferBase[cur + 1];
                hash2Value = (temp & (((int) kHash2Size) - 1));
                temp ^= (uint) ((_bufferBase[cur + 2]) << 8);
                hash3Value = (temp & (((int) kHash3Size) - 1));
                hashValue = (temp ^ (CRC.Table[_bufferBase[cur + 3]] << 5)) & _hashMask;
            }
            else {
	            hashValue = _bufferBase[cur] ^ ((UInt32) (_bufferBase[cur + 1]) << 8);
            }

            UInt32 curMatch = _hash[kFixHashSize + hashValue];
            if (HASH_ARRAY)
            {
                UInt32 curMatch2 = _hash[hash2Value];
                UInt32 curMatch3 = _hash[kHash3Offset + hash3Value];
                _hash[hash2Value] = _pos;
                _hash[kHash3Offset + hash3Value] = _pos;
                if (curMatch2 > matchMinPos) {
	                if (_bufferBase[_bufferOffset + curMatch2] == _bufferBase[cur])
	                {
		                distances[offset++] = maxLen = 2;
		                distances[offset++] = _pos - curMatch2 - 1;
	                }
                }
                if (curMatch3 > matchMinPos) {
	                if (_bufferBase[_bufferOffset + curMatch3] == _bufferBase[cur])
	                {
		                if (curMatch3 == curMatch2) {
			                offset -= 2;
		                }
		                distances[offset++] = maxLen = 3;
		                distances[offset++] = _pos - curMatch3 - 1;
		                curMatch2 = curMatch3;
	                }
                }
                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = kStartMaxLen;
                }
            }

            _hash[kFixHashSize + hashValue] = _pos;

            UInt32 ptr0 = (_cyclicBufferPos << 1) + 1;
            UInt32 ptr1 = (_cyclicBufferPos << 1);

            UInt32 len0, len1;
            len0 = len1 = kNumHashDirectBytes;

            if (kNumHashDirectBytes != 0)
            {
                if (curMatch > matchMinPos)
                {
                    if (_bufferBase[_bufferOffset + curMatch + kNumHashDirectBytes] !=
                        _bufferBase[cur + kNumHashDirectBytes])
                    {
                        distances[offset++] = maxLen = kNumHashDirectBytes;
                        distances[offset++] = _pos - curMatch - 1;
                    }
                }
            }

            UInt32 count = _cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                    break;
                }
                UInt32 delta = _pos - curMatch;
                UInt32 cyclicPos = ((delta <= _cyclicBufferPos)
                                        ?
                                            (_cyclicBufferPos - delta)
                                        :
                                            (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                UInt32 pby1 = _bufferOffset + curMatch;
                UInt32 len = Math.Min(len0, len1);
                if (_bufferBase[pby1 + len] == _bufferBase[cur + len])
                {
                    while (++len != lenLimit)
                        if (_bufferBase[pby1 + len] != _bufferBase[cur + len]) {
	                        break;
                        }
                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                }
                if (_bufferBase[pby1 + len] < _bufferBase[cur + len])
                {
                    _son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = _son[ptr1];
                    len1 = len;
                }
                else
                {
                    _son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = _son[ptr0];
                    len0 = len;
                }
            }
            MovePos();
            return offset;
        }

        /// <inheritdoc />
        public void Skip(UInt32 num)
        {
            do
            {
                UInt32 lenLimit;
                if (_pos + _matchMaxLen <= _streamPos) {
	                lenLimit = _matchMaxLen;
                } else
                {
                    lenLimit = _streamPos - _pos;
                    if (lenLimit < kMinMatchCheck)
                    {
                        MovePos();
                        continue;
                    }
                }

                UInt32 matchMinPos = (_pos > _cyclicBufferSize) ? (_pos - _cyclicBufferSize) : 0;
                UInt32 cur = _bufferOffset + _pos;

                UInt32 hashValue;

                if (HASH_ARRAY)
                {
                    UInt32 temp = CRC.Table[_bufferBase[cur]] ^ _bufferBase[cur + 1];
                    UInt32 hash2Value = (temp & (((int) kHash2Size) - 1));
                    _hash[hash2Value] = _pos;
                    temp ^= ((UInt32) (_bufferBase[cur + 2]) << 8);
                    UInt32 hash3Value = (temp & (((int) kHash3Size) - 1));
                    _hash[kHash3Offset + hash3Value] = _pos;
                    hashValue = (temp ^ (CRC.Table[_bufferBase[cur + 3]] << 5)) & _hashMask;
                }
                else {
	                hashValue = _bufferBase[cur] ^ ((UInt32) (_bufferBase[cur + 1]) << 8);
                }

                UInt32 curMatch = _hash[kFixHashSize + hashValue];
                _hash[kFixHashSize + hashValue] = _pos;

                UInt32 ptr0 = (_cyclicBufferPos << 1) + 1;
                UInt32 ptr1 = (_cyclicBufferPos << 1);

                UInt32 len0, len1;
                len0 = len1 = kNumHashDirectBytes;

                UInt32 count = _cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                        break;
                    }

                    UInt32 delta = _pos - curMatch;
                    UInt32 cyclicPos = ((delta <= _cyclicBufferPos)
                                            ?
                                                (_cyclicBufferPos - delta)
                                            :
                                                (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                    UInt32 pby1 = _bufferOffset + curMatch;
                    UInt32 len = Math.Min(len0, len1);
                    if (_bufferBase[pby1 + len] == _bufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                            if (_bufferBase[pby1 + len] != _bufferBase[cur + len]) {
	                            break;
                            }
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                    if (_bufferBase[pby1 + len] < _bufferBase[cur + len])
                    {
                        _son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = _son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        _son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = _son[ptr0];
                        len0 = len;
                    }
                }
                MovePos();
            } while (--num != 0);
        }

        #endregion

        /// <inheritdoc />
        public void SetType(int numHashBytes)
        {
            HASH_ARRAY = (numHashBytes > 2);
            if (HASH_ARRAY)
            {
                kNumHashDirectBytes = 0;
                kMinMatchCheck = 4;
                kFixHashSize = kHash2Size + kHash3Size;
            }
            else
            {
                kNumHashDirectBytes = 2;
                kMinMatchCheck = 2 + 1;
                kFixHashSize = 0;
            }
        }

        /// <summary>Advances the current position by one byte, wrapping the cyclic tree buffer <br/>
        /// position and normalizing all stored offsets once the position counter nears overflow.</summary>
        public new void MovePos()
        {
            if (++_cyclicBufferPos >= _cyclicBufferSize) {
	            _cyclicBufferPos = 0;
            }
            base.MovePos();
            if (_pos == kMaxValForNormalize) {
	            Normalize();
            }
        }

        /// <summary>Rebases every entry in <paramref name='items'/> by subtracting <paramref name='subValue'/>, <br/>
        /// clamping stale entries (at or below <paramref name='subValue'/>) to <see cref="kEmptyHashValue"/>.</summary>
        private static void NormalizeLinks(UInt32[] items, UInt32 numItems, UInt32 subValue)
        {
            for (UInt32 i = 0; i < numItems; i++)
            {
                UInt32 value = items[i];
                if (value <= subValue) {
	                value = kEmptyHashValue;
                } else {
	                value -= subValue;
                }
                items[i] = value;
            }
        }

        /// <summary>Rebases the tree links and hash table to keep their offsets within range once <br/>
        /// the absolute position approaches <see cref="kMaxValForNormalize"/>.</summary>
        private void Normalize()
        {
            UInt32 subValue = _pos - _cyclicBufferSize;
            NormalizeLinks(_son, _cyclicBufferSize*2, subValue);
            NormalizeLinks(_hash, _hashSizeSum, subValue);
            ReduceOffsets((Int32) subValue);
        }

        //public void SetCutValue(UInt32 cutValue) { _cutValue = cutValue; }
    }
}