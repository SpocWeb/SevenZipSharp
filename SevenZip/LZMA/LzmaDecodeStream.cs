using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;
    using System.IO;

    using SevenZip.Sdk.Compression.Lzma;

    /// <summary>
    /// The stream which decompresses data with LZMA on the fly.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 43 | <see cref="LzmaDecodeStream"/> | Initializes a new instance of the LzmaDecodeStream class. |
    /// | 56 | <see cref="ChunkSize"/> | Gets the chunk size. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="MemoryStream"/> | Used as a field. |
    /// | <see cref="Decoder"/> | Used as a field. |
    /// | <see cref="SeekOrigin"/> | Passed as a parameter. |
    /// </remarks>
    [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
    [Tags("code/decompressionlzmastream_reading")]
    [System.ComponentModel.Description("The stream which decompresses data with LZMA on the fly.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:51Z", Digest = "d5fbf109c374f62ad56ae32d1dd3bcd983b50571975397ff1e92d6f0abbc41e7", Stale = false, Path = "LZMA/LzmaDecodeStream.cs", Since = "2026-08-23")]
    [Concept("stream_adapterwrapper_pattern")]
    public class LzmaDecodeStream : Stream
    {
        private readonly MemoryStream _buffer = new MemoryStream();
        private readonly Decoder _decoder = new Decoder();
        private readonly Stream _input;
        private byte[] _commonProperties;
        private bool _error;
        private bool _firstChunkRead;

        /// <summary>
        /// Initializes a new instance of the LzmaDecodeStream class.
        /// </summary>
        /// <param name="encodedStream">A compressed stream.</param>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Initializes a new instance of the LzmaDecodeStream class.")]
        [Concept("stream_adapterwrapper_pattern")]
        public LzmaDecodeStream(Stream encodedStream)
        {
            if (!encodedStream.CanRead)
            {
                throw new ArgumentException("The specified stream can not read.", "encodedStream");
            }
            _input = encodedStream;
        }

        /// <summary>
        /// Gets the chunk size.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets the chunk size.")]
        [Concept("stream_adapterwrapper_pattern")]
        public int ChunkSize => (int) _buffer.Length;

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets a value indicating whether the current stream supports reading.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override bool CanRead => true;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets a value indicating whether the current stream supports seeking.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override bool CanSeek => false;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets a value indicating whether the current stream supports writing.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the length in bytes of the output stream.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets the length in bytes of the output stream.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override long Length
        {
            get
            {
                if (_input.CanSeek)
                {
                    return _input.Length;
                }

                return _buffer.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position within the output stream.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Gets or sets the position within the output stream.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override long Position
        {
            get
            {
                if (_input.CanSeek)
                {
                    return _input.Position;
                }
                return _buffer.Position;
            }
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Reads and decodes the next chunk from the input stream, validating LZMA
        /// <br/>
        /// properties and updating the internal buffer.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Reads and decodes the next chunk from the input stream, validating LZMA properties and updating the internal buffer.")]
        [Concept("stream_adapterwrapper_pattern")]
        private void ReadChunk()
        {
            long size;
            byte[] properties;
            try
            {
                properties = SevenZipExtractor.GetLzmaProperties(_input, out size);
            }
            catch (LzmaException)
            {
                _error = true;
                return;
            }
            if (!_firstChunkRead)
            {
                _commonProperties = properties;
            }
            if (_commonProperties[0] != properties[0] ||
                _commonProperties[1] != properties[1] ||
                _commonProperties[2] != properties[2] ||
                _commonProperties[3] != properties[3] ||
                _commonProperties[4] != properties[4])
            {
                _error = true;
                return;
            }
            if (_buffer.Capacity < (int) size)
            {
                _buffer.Capacity = (int) size;
            }
            _buffer.SetLength(size);
            _decoder.SetDecoderProperties(properties);
            _buffer.Position = 0;
            _decoder.Code(
                _input, _buffer, 0, size, null);
            _buffer.Position = 0;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Does nothing.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override void Flush() {}

        /// <summary>
        /// Reads a sequence of bytes from the current stream and decompresses data if necessary.
        /// </summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>        
        [Facets(Layer = "transform", Status = "stable", Complexity = 3)]
        [Tags("code/decompressionlzmastream_reading")]
        [System.ComponentModel.Description("Reads a sequence of bytes from the current stream and decompresses data if necessary.")]
        [Concept("stream_adapterwrapper_pattern")]
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_error)
            {
                return 0;
            }

            if (!_firstChunkRead)
            {
                ReadChunk();
                _firstChunkRead = true;
            }
            int readCount = 0;
            while (count > _buffer.Length - _buffer.Position && !_error)
            {
                var buf = new byte[_buffer.Length - _buffer.Position];
                _buffer.Read(buf, 0, buf.Length);
                buf.CopyTo(buffer, offset);
                offset += buf.Length;
                count -= buf.Length;
                readCount += buf.Length;
                ReadChunk();
            }
            if (!_error)
            {
                _buffer.Read(buffer, offset, count);
                readCount += count;
            }
            return readCount;
        }

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the origin parameter.</param>
		/// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>       
		[Facets(Layer = "transform", Status = "stable", Complexity = 3)]
		[Tags("code/decompressionlzmastream_reading")]
		[System.ComponentModel.Description("Sets the position within the current stream.")]
		[Concept("stream_adapterwrapper_pattern")]
		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		[Facets(Layer = "transform", Status = "stable", Complexity = 3)]
		[Tags("code/decompressionlzmastream_reading")]
		[System.ComponentModel.Description("Sets the length of the current stream.")]
		[Concept("stream_adapterwrapper_pattern")]
		public override void SetLength(long value) => throw new NotSupportedException();

		/// <summary>
		/// Writes a sequence of bytes to the current stream.
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		[Facets(Layer = "transform", Status = "stable", Complexity = 3)]
		[Tags("code/decompressionlzmastream_reading")]
		[System.ComponentModel.Description("Writes a sequence of bytes to the current stream.")]
		[Concept("stream_adapterwrapper_pattern")]
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}
}
