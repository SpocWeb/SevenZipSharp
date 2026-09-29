using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.LZ
{
    using System.IO;

    /// <summary>Manages a circular buffer window for LZ compression data, buffering and flushing data
    /// <br/>to an output stream.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 33 | <see cref="Create"/> | Creates or resizes the output window buffer to the specified size. |
    /// | 49 | <see cref="Init"/> | Initializes the output window with the target stream. |
    /// | 65 | <see cref="Train"/> | Loads data from the stream into the window for initial training. |
    /// | 93 | <see cref="ReleaseStream"/> | Flushes any pending data and releases the output stream. |
    /// | 100 | <see cref="Flush"/> | Writes buffered data to the output stream and updates the window position. |
    /// | 119 | <see cref="CopyBlock"/> | Copies data from the specified distance-back within the window, repeating for  the given length. |
    /// | 139 | <see cref="PutByte"/> | Writes a byte to the output window, flushing automatically if the window is full. |
    /// | 150 | <see cref="GetByte"/> | Retrieves the byte at the specified distance-back from the current position. |
    /// </remarks>
    [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
    [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
    [System.ComponentModel.Description("Manages a circular buffer window for LZ compression data, buffering and flushing data to an output stream.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:51Z", Digest = "d07b9a965a5b1e5e651084ed25fc9f752894566cc9123f589c3072f9f00417e1", Stale = false, Path = "sdk/Compress/LZ/LzOutWindow.cs", Since = "2026-08-23")]
    [Concept("lz_compression")]
    [Concept("sliding_window")]
    [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
    internal class OutWindow
    {
        private byte[] _buffer;
        private uint _pos;
        private Stream _stream;
        private uint _streamPos;
        private uint _windowSize;
        public uint TrainSize;

        /// <summary>Creates or resizes the output window buffer to the specified size.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Creates or resizes the output window buffer to the specified size.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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

        /// <summary>Initializes the output window with the target stream.</summary>
        /// <param name="stream">The output stream to write compressed data to.</param>
        /// <param name="solid">If <see langword="false"/>, resets window state; if <see langword="true"/>,
        /// <br/>preserves existing window position and data for continuous compression.</param>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Initializes the output window with the target stream.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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

        /// <summary>Loads data from the stream into the window for initial training.</summary>
        /// <param name="stream">The input stream to read training data from.</param>
        /// <returns><see langword="true"/> if training completed successfully;
        /// <br/><see langword="false"/> if the stream ended prematurely.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Loads data from the stream into the window for initial training.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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

        /// <summary>Flushes any pending data and releases the output stream.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Flushes any pending data and releases the output stream.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
        public void ReleaseStream()
        {
            Flush();
            _stream = null;
        }

        /// <summary>Writes buffered data to the output stream and updates the window position.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Writes buffered data to the output stream and updates the window position.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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

        /// <summary>Copies data from the specified distance-back within the window, repeating for
        /// <br/>the given length.</summary>
        /// <param name="distance">The distance back from the current position to copy from.</param>
        /// <param name="len">The number of bytes to copy.</param>
        /// <remarks>Used for LZ match copying in compression; wraps around the circular window
        /// <br/>and flushes automatically when the window is full.</remarks>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Copies data from the specified distance-back within the window, repeating for the given length.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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

        /// <summary>Writes a byte to the output window, flushing automatically if the window is full.</summary>
        /// <param name="b">The byte to write.</param>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Writes a byte to the output window, flushing automatically if the window is full.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
        public void PutByte(byte b)
        {
            _buffer[_pos++] = b;
            if (_pos >= _windowSize) {
	            Flush();
            }
        }

        /// <summary>Retrieves the byte at the specified distance-back from the current position.</summary>
        /// <param name="distance">The distance back from the current position.</param>
        /// <returns>The byte at the specified distance within the window.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/buffer_management", "code/buffering", "code/circular_buffer")]
        [System.ComponentModel.Description("Retrieves the byte at the specified distance-back from the current position.")]
        [Concept("lz_compression")]
        [Concept("sliding_window")]
        [Concept("Technology\\IT\\IT-Algorithm\\Data_Compression.md")]
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