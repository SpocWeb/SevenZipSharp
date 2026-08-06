namespace SevenZip.Sdk.Compression.LZ
{
    using System.IO;

    /// <summary>Sliding output window that buffers decompressed bytes in memory so LZ77 back-<br/>
    /// references can copy from recent output, flushing completed data to the destination stream.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 18 | <see cref="Create"/> | Allocates the output buffer to hold   bytes and resets   the write and flush positions to the start. |
    /// | 32 | <see cref="Init"/> | Attaches   as the destination for flushed output, resetting   the buffer position unless   keeps it continuing from a prior block. |
    /// | 48 | <see cref="Train"/> | Preloads the output buffer with up to _windowSize trailing bytes from    , so subsequent back-references can reach into that history. |
    /// | 76 | <see cref="ReleaseStream"/> | Flushes any pending buffered bytes and detaches the output stream. |
    /// | 84 | <see cref="Flush"/> | Writes all bytes produced since the last flush to the output stream, wrapping   the write position back to the start once the buffer is full. |
    /// | 100 | <see cref="CopyBlock"/> | Applies an LZ77 back-reference by copying   bytes from     bytes behind the current position to the current position,   flushing the buffer to the stream whenever it fills up. |
    /// | 120 | <see cref="PutByte"/> | Appends a single decoded byte   to the output buffer, flushing   to the stream once the buffer fills up. |
    /// | 130 | <see cref="GetByte"/> | Returns the byte located   positions behind the current   write position, wrapping around the buffer if needed. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: e1f966b02d28e62312733a1190c05e7955e6ec8bd8f4cc2ed90f245a5c7a7de6
    /// </code>
    /// </example>
    internal class OutWindow
    {
        private byte[] _buffer;
        private uint _pos;
        private Stream _stream;
        private uint _streamPos;
        private uint _windowSize;
        public uint TrainSize;

        /// <summary>Allocates the output buffer to hold <paramref name='windowSize'/> bytes and resets <br/>
        /// the write and flush positions to the start.</summary>
        public void Create(uint windowSize)
        {
            if (_windowSize != windowSize)
            {
                // System.GC.Collect();
                _buffer = new byte[windowSize];
            }
            _windowSize = windowSize;
            _pos = 0;
            _streamPos = 0;
        }

        /// <summary>Attaches <paramref name='stream'/> as the destination for flushed output, resetting <br/>
        /// the buffer position unless <paramref name='solid'/> keeps it continuing from a prior block.</summary>
        public void Init(Stream stream, bool solid)
        {
            ReleaseStream();
            _stream = stream;
            if (!solid)
            {
                _streamPos = 0;
                _pos = 0;
                TrainSize = 0;
            }
        }

        /// <summary>Preloads the output buffer with up to <see cref="_windowSize"/> trailing bytes from <br/>
        /// <paramref name='stream'/>, so subsequent back-references can reach into that history.</summary>
        /// <returns><see langword='true'/> if the training data was read in full; <see langword='false'/> <br/>
        /// if <paramref name='stream'/> ended prematurely.</returns>
        public bool Train(Stream stream)
        {
            long len = stream.Length;
            uint size = (len < _windowSize) ? (uint) len : _windowSize;
            TrainSize = size;
            stream.Position = len - size;
            _streamPos = _pos = 0;
            while (size > 0)
            {
                uint curSize = _windowSize - _pos;
                if (size < curSize) {
	                curSize = size;
                }
                int numReadBytes = stream.Read(_buffer, (int) _pos, (int) curSize);
                if (numReadBytes == 0) {
	                return false;
                }
                size -= (uint) numReadBytes;
                _pos += (uint) numReadBytes;
                _streamPos += (uint) numReadBytes;
                if (_pos == _windowSize) {
	                _streamPos = _pos = 0;
                }
            }
            return true;
        }

        /// <summary>Flushes any pending buffered bytes and detaches the output stream.</summary>
        public void ReleaseStream()
        {
            Flush();
            _stream = null;
        }

        /// <summary>Writes all bytes produced since the last flush to the output stream, wrapping <br/>
        /// the write position back to the start once the buffer is full.</summary>
        public void Flush()
        {
            uint size = _pos - _streamPos;
            if (size == 0) {
	            return;
            }
            _stream.Write(_buffer, (int) _streamPos, (int) size);
            if (_pos >= _windowSize) {
	            _pos = 0;
            }
            _streamPos = _pos;
        }

        /// <summary>Applies an LZ77 back-reference by copying <paramref name='len'/> bytes from <br/>
        /// <paramref name='distance'/> bytes behind the current position to the current position, <br/>
        /// flushing the buffer to the stream whenever it fills up.</summary>
        public void CopyBlock(uint distance, uint len)
        {
            uint pos = _pos - distance - 1;
            if (pos >= _windowSize) {
	            pos += _windowSize;
            }
            for (; len > 0; len--)
            {
                if (pos >= _windowSize) {
	                pos = 0;
                }
                _buffer[_pos++] = _buffer[pos++];
                if (_pos >= _windowSize) {
	                Flush();
                }
            }
        }

        /// <summary>Appends a single decoded byte <paramref name='b'/> to the output buffer, flushing <br/>
        /// to the stream once the buffer fills up.</summary>
        public void PutByte(byte b)
        {
            _buffer[_pos++] = b;
            if (_pos >= _windowSize) {
	            Flush();
            }
        }

        /// <summary>Returns the byte located <paramref name='distance'/> positions behind the current <br/>
        /// write position, wrapping around the buffer if needed.</summary>
        public byte GetByte(uint distance)
        {
            uint pos = _pos - distance - 1;
            if (pos >= _windowSize) {
	            pos += _windowSize;
            }
            return _buffer[pos];
        }
    }
}