using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Buffer
{
    using System.IO;

    /// <summary>Buffers bytes for efficient writing to an underlying output stream.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 36 | <see cref="OutBuffer"/> | Initializes a new instance of the OutBuffer class |
    /// | 44 | <see cref="SetStream"/> | Designates the output stream for buffered writes. |
    /// | 47 | <see cref="FlushStream"/> | Flushes the underlying output stream. |
    /// | 50 | <see cref="CloseStream"/> | Closes the underlying output stream. |
    /// | 53 | <see cref="ReleaseStream"/> | Releases the reference to the output stream. |
    /// | 56 | <see cref="Init"/> | Resets the buffer position and processed size counter. |
    /// | 63 | <see cref="WriteByte"/> | Writes a byte to the buffer, flushing if the buffer reaches capacity. |
    /// | 72 | <see cref="FlushData"/> | Writes buffered data to the stream and resets the buffer position. |
    /// | 83 | <see cref="GetProcessedSize"/> | Returns the total number of bytes written to the stream. |
    /// </remarks>
    [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
    [Tags("code/data_container", "code/buffer_snapshot")]
    [System.ComponentModel.Description("Buffers bytes for efficient writing to an underlying output stream.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:51Z", Digest = "dc587e4a8cb5c15ee0fdedbb93a554a85895a196b5872eabc963341ba99fb4a6", Stale = false, Path = "sdk/Common/OutBuffer.cs", Since = "2026-08-23")]
    [Concept("code/batch_processing")]
    [Concept("code/file_io")]
    internal class OutBuffer
    {
        private readonly byte[] m_Buffer;
        private readonly uint m_BufferSize;
        private uint m_Pos;
        private ulong m_ProcessedSize;
        private Stream m_Stream;

        /// <summary>
        /// Initializes a new instance of the OutBuffer class
        /// </summary>
        /// <param name="bufferSize"></param>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/data_container", "code/buffer_snapshot")]
        [System.ComponentModel.Description("Initializes a new instance of the OutBuffer class")]
        [Concept("code/batch_processing")]
        [Concept("code/file_io")]
        public OutBuffer(uint bufferSize)
        {
            m_Buffer = new byte[bufferSize];
            m_BufferSize = bufferSize;
        }

		/// <summary>Designates the output stream for buffered writes.</summary>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Designates the output stream for buffered writes.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public void SetStream(Stream stream) => m_Stream = stream;

		/// <summary>Flushes the underlying output stream.</summary>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Flushes the underlying output stream.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public void FlushStream() => m_Stream.Flush();

		/// <summary>Closes the underlying output stream.</summary>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Closes the underlying output stream.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public void CloseStream() => m_Stream.Close();

		/// <summary>Releases the reference to the output stream.</summary>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Releases the reference to the output stream.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public void ReleaseStream() => m_Stream = null;

		/// <summary>Resets the buffer position and processed size counter.</summary>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Resets the buffer position and processed size counter.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public void Init()
        {
            m_ProcessedSize = 0;
            m_Pos = 0;
        }

        /// <summary>Writes a byte to the buffer, flushing if the buffer reaches capacity.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/data_container", "code/buffer_snapshot")]
        [System.ComponentModel.Description("Writes a byte to the buffer, flushing if the buffer reaches capacity.")]
        [Concept("code/batch_processing")]
        [Concept("code/file_io")]
        public void WriteByte(byte b)
        {
            m_Buffer[m_Pos++] = b;
            if (m_Pos >= m_BufferSize) {
	            FlushData();
            }
        }

        /// <summary>Writes buffered data to the stream and resets the buffer position.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 2)]
        [Tags("code/data_container", "code/buffer_snapshot")]
        [System.ComponentModel.Description("Writes buffered data to the stream and resets the buffer position.")]
        [Concept("code/batch_processing")]
        [Concept("code/file_io")]
        public void FlushData()
        {
            if (m_Pos == 0) {
	            return;
            }
            m_Stream.Write(m_Buffer, 0, (int) m_Pos);
            m_Pos = 0;
        }

		/// <summary>Returns the total number of bytes written to the stream.</summary>
		/// <returns>Total bytes written (processed size plus current buffer position).</returns>
		[Facets(Layer = "transform", Status = "stable", Complexity = 2)]
		[Tags("code/data_container", "code/buffer_snapshot")]
		[System.ComponentModel.Description("Returns the total number of bytes written to the stream.")]
		[Concept("code/batch_processing")]
		[Concept("code/file_io")]
		public ulong GetProcessedSize() => m_ProcessedSize + m_Pos;
	}
}