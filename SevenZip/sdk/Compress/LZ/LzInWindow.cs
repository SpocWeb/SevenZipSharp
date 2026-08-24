using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>
    /// Manages a sliding input buffer for LZ compression, tracking stream position <br/>and available bytes for pattern matching operations.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 83 | <see cref="MoveBlock"/> | Compacts buffer data by discarding bytes before the keep-before boundary,  adjusting position tracking accordingly. |
    /// | 100 | <see cref="ReadBlock"/> | Reads data from the stream into the buffer until the stream ends or  sufficient data is available after the current position. |
    /// | 134 | <see cref="Create"/> | Allocates and initializes the buffer with the specified keep-before,  keep-after, and reserve sizes. |
    /// | 149 | <see cref="SetStream"/> | Associates the stream to read data from. |
    /// | 152 | <see cref="ReleaseStream"/> | Disassociates the stream. |
    /// | 155 | <see cref="Init"/> | Initializes the window state and reads the first data block from the stream. |
    /// | 165 | <see cref="MovePos"/> | Advances the current position by one byte, triggering buffer  compaction and stream read when necessary. |
    /// | 179 | <see cref="GetIndexByte"/> | Returns the byte at the specified index offset from the current position. |
    /// | 188 | <see cref="GetMatchLen"/> | index + limit have not to exceed _keepSizeAfter |
    /// | 206 | <see cref="GetNumAvailableBytes"/> | Returns the count of unread bytes from the current position to the stream position. |
    /// | 209 | <see cref="ReduceOffsets"/> | Subtracts a value from all position-tracking fields. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Stream"/> | Source of input data read into the buffer. |
    /// | <see cref="Byte"/> | Element type of the data buffer. |
    /// | <see cref="UInt32"/> | Position and size tracking throughout the buffer and stream. |
    /// </remarks>
    /// <seealso cref="Stream">Stream: provides input data to the window.</seealso>
    /// <seealso cref="Byte">Byte: element type of the managed buffer.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T13:57:39Z", Digest = "e7fc3a08bc7803a01d3aa44f68ca209a25a8ba9e3eed59c05fa62f06e824f3fc", Stale = false, Path = "sdk/Compress/LZ/LzInWindow.cs", Since = "2026-08-23")]
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

        /// <summary>Compacts buffer data by discarding bytes before the keep-before boundary, <br/>adjusting position tracking accordingly.</summary>
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

        /// <summary>Reads data from the stream into the buffer until the stream ends or <br/>sufficient data is available after the current position.</summary>
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

		/// <summary>Releases the allocated buffer.</summary>
		private void Free() => _bufferBase = null;

		/// <summary>Allocates and initializes the buffer with the specified keep-before, <br/>keep-after, and reserve sizes.</summary>
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

		/// <summary>Associates the stream to read data from.</summary>
		public void SetStream(Stream stream) => _stream = stream;

		/// <summary>Disassociates the stream.</summary>
		public void ReleaseStream() => _stream = null;

		/// <summary>Initializes the window state and reads the first data block from the stream.</summary>
		public void Init()
        {
            _bufferOffset = 0;
            _pos = 0;
            _streamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        /// <summary>Advances the current position by one byte, triggering buffer <br/>compaction and stream read when necessary.</summary>
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

		/// <summary>Returns the byte at the specified index offset from the current position.</summary>
		public Byte GetIndexByte(Int32 index) => _bufferBase[_bufferOffset + _pos + index];

		/// <summary>
		/// index + limit have not to exceed _keepSizeAfter
		/// </summary>
		/// <param name="index"></param>
		/// <param name="distance"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		[System.ComponentModel.Description("index + limit have not to exceed _keepSizeAfter")]
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

		/// <summary>Returns the count of unread bytes from the current position to the stream position.</summary>
		public UInt32 GetNumAvailableBytes() => _streamPos - _pos;

		/// <summary>Subtracts a value from all position-tracking fields.</summary>
		public void ReduceOffsets(Int32 subValue)
        {
            _bufferOffset += (UInt32) subValue;
            _posLimit -= (UInt32) subValue;
            _pos -= (UInt32) subValue;
            _streamPos -= (UInt32) subValue;
        }
    }
}