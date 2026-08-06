namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>
    /// Input window class
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 62 | <see cref="MoveBlock"/> | Shifts the buffered data down to the start of the backing array, discarding   bytes older than _keepSizeBefore, to make room for further reads. |
    /// | 80 | <see cref="ReadBlock"/> | Fills the remaining free space in the buffer from the attached stream, updating   the read limit, and marks the stream as exhausted once no more bytes are available. |
    /// | 116 | <see cref="Create"/> | Allocates the backing buffer large enough to hold     bytes of history,   bytes of lookahead, and     extra bytes reserved for a read block. |
    /// | 131 | <see cref="SetStream"/> | Attaches   as the source the window reads its data from. |
    /// | 134 | <see cref="ReleaseStream"/> | Detaches the current input stream without disposing it. |
    /// | 137 | <see cref="Init"/> | Resets the window to the start of the stream and preloads the first data block. |
    /// | 148 | <see cref="MovePos"/> | Advances the current position by one byte, refilling or shifting the buffer   from the stream when the position reaches the safe-read limit. |
    /// | 162 | <see cref="GetIndexByte"/> | Returns the byte located   positions from the current position. |
    /// | 171 | <see cref="GetMatchLen"/> | index + limit have not to exceed _keepSizeAfter |
    /// | 188 | <see cref="GetNumAvailableBytes"/> | Returns the number of bytes still available for reading ahead of the current position. |
    /// | 192 | <see cref="ReduceOffsets"/> | Rebases the buffer offset, position, and stream limits by subtracting    , keeping their absolute magnitude from growing unbounded. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt32"/> | Used as a field. |
    /// | <see cref="Byte"/> | Used as a field. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 69b3261a8ab707ba88efcf40bef53b843c2f737aa003eaaa86ff446275843136
    /// </code>
    /// </example>
    internal class InWindow
    {
        /// <summary>
        /// Size of Allocated memory block
        /// </summary>
        public UInt32 _blockSize;

        /// <summary>
        /// The pointer to buffer with data
        /// </summary>
        public Byte[] _bufferBase;

        /// <summary>
        /// Buffer offset value
        /// </summary>
        public UInt32 _bufferOffset;

        /// <summary>
        /// How many BYTEs must be kept buffer after _pos
        /// </summary>
        private UInt32 _keepSizeAfter;

        /// <summary>
        /// How many BYTEs must be kept in buffer before _pos
        /// </summary>
        private UInt32 _keepSizeBefore;

        private UInt32 _pointerToLastSafePosition;

        /// <summary>
        /// Offset (from _buffer) of curent byte
        /// </summary>
        public UInt32 _pos;

        private UInt32 _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        private Stream _stream;
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream

        /// <summary>
        /// Offset (from _buffer) of first not read byte from Stream
        /// </summary>
        public UInt32 _streamPos;

        /// <summary>Shifts the buffered data down to the start of the backing array, discarding <br/>
        /// bytes older than <see cref="_keepSizeBefore"/>, to make room for further reads.</summary>
        public void MoveBlock()
        {
            UInt32 offset = (_bufferOffset) + _pos - _keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0) {
	            offset--;
            }

            UInt32 numBytes = (_bufferOffset) + _streamPos - offset;

            // check negative offset ????
            for (UInt32 i = 0; i < numBytes; i++)
                _bufferBase[i] = _bufferBase[offset + i];
            _bufferOffset -= offset;
        }

        /// <summary>Fills the remaining free space in the buffer from the attached stream, updating <br/>
        /// the read limit, and marks the stream as exhausted once no more bytes are available.</summary>
        public virtual void ReadBlock()
        {
            if (_streamEndWasReached) {
	            return;
            }
            while (true)
            {
                var size = (int) ((0 - _bufferOffset) + _blockSize - _streamPos);
                if (size == 0) {
	                return;
                }
                int numReadBytes = _stream.Read(_bufferBase, (int) (_bufferOffset + _streamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = _streamPos;
                    UInt32 pointerToPostion = _bufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition) {
	                    _posLimit = (_pointerToLastSafePosition - _bufferOffset);
                    }

                    _streamEndWasReached = true;
                    return;
                }
                _streamPos += (UInt32) numReadBytes;
                if (_streamPos >= _pos + _keepSizeAfter) {
	                _posLimit = _streamPos - _keepSizeAfter;
                }
            }
        }

		/// <summary>Releases the backing byte array so it can be reallocated with a different size.</summary>
		private void Free() => _bufferBase = null;

		/// <summary>Allocates the backing buffer large enough to hold <paramref name='keepSizeBefore'/> <br/>
		/// bytes of history, <paramref name='keepSizeAfter'/> bytes of lookahead, and <br/>
		/// <paramref name='keepSizeReserv'/> extra bytes reserved for a read block.</summary>
		public void Create(UInt32 keepSizeBefore, UInt32 keepSizeAfter, UInt32 keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            UInt32 blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (_bufferBase == null || _blockSize != blockSize)
            {
                Free();
                _blockSize = blockSize;
                _bufferBase = new Byte[_blockSize];
            }
            _pointerToLastSafePosition = _blockSize - keepSizeAfter;
        }

		/// <summary>Attaches <paramref name='stream'/> as the source the window reads its data from.</summary>
		public void SetStream(Stream stream) => _stream = stream;

		/// <summary>Detaches the current input stream without disposing it.</summary>
		public void ReleaseStream() => _stream = null;

		/// <summary>Resets the window to the start of the stream and preloads the first data block.</summary>
		public void Init()
        {
            _bufferOffset = 0;
            _pos = 0;
            _streamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        /// <summary>Advances the current position by one byte, refilling or shifting the buffer <br/>
        /// from the stream when the position reaches the safe-read limit.</summary>
        public void MovePos()
        {
            _pos++;
            if (_pos > _posLimit)
            {
                UInt32 pointerToPostion = _bufferOffset + _pos;
                if (pointerToPostion > _pointerToLastSafePosition) {
	                MoveBlock();
                }
                ReadBlock();
            }
        }

		/// <summary>Returns the byte located <paramref name='index'/> positions from the current position.</summary>
		public Byte GetIndexByte(Int32 index) => _bufferBase[_bufferOffset + _pos + index];

		/// <summary>
		/// index + limit have not to exceed _keepSizeAfter
		/// </summary>
		/// <param name="index"></param>
		/// <param name="distance"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		public UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit)
        {
            if (_streamEndWasReached) {
	            if ((_pos + index) + limit > _streamPos) {
		            limit = _streamPos - (UInt32) (_pos + index);
	            }
            }
            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            UInt32 pby = _bufferOffset + _pos + (UInt32) index;

            UInt32 i;
            for (i = 0; i < limit && _bufferBase[pby + i] == _bufferBase[pby + i - distance]; i++) ;
            return i;
        }

		/// <summary>Returns the number of bytes still available for reading ahead of the current position.</summary>
		public UInt32 GetNumAvailableBytes() => _streamPos - _pos;

		/// <summary>Rebases the buffer offset, position, and stream limits by subtracting <br/>
		/// <paramref name='subValue'/>, keeping their absolute magnitude from growing unbounded.</summary>
		public void ReduceOffsets(Int32 subValue)
        {
            _bufferOffset += (UInt32) subValue;
            _posLimit -= (UInt32) subValue;
            _pos -= (UInt32) subValue;
            _streamPos -= (UInt32) subValue;
        }
    }
}