namespace SevenZip.Sdk.Buffer
{
    using System.IO;

    /// <summary>Buffers bytes written by the LZMA encoder and flushes them in blocks to an underlying <see cref="Stream"/>.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 18 | <see cref="OutBuffer"/> | Initializes a new instance of the OutBuffer class |
    /// | 25 | <see cref="SetStream"/> | Attaches stream as the destination that buffered bytes are flushed to. |
    /// | 28 | <see cref="FlushStream"/> | Flushes the underlying stream's own buffers (does not flush m_Buffer itself). |
    /// | 31 | <see cref="CloseStream"/> | Closes the underlying stream. |
    /// | 34 | <see cref="ReleaseStream"/> | Detaches the underlying stream without closing it, so this buffer no longer references it. |
    /// | 37 | <see cref="Init"/> | Resets the processed-size counter and write position, discarding any buffered bytes. |
    /// | 44 | <see cref="WriteByte"/> | Appends b to the buffer, flushing to the stream first if the buffer is full. |
    /// | 53 | <see cref="FlushData"/> | Writes any buffered bytes to the underlying stream and resets the write position. |
    /// | 64 | <see cref="GetProcessedSize"/> | Gets the total number of bytes processed so far, including any still buffered. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 306d8854442334d29b238c814c84c9a529d004e702f6b8126275271957f555a0
    /// </code>
    /// </example>
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
        public OutBuffer(uint bufferSize)
        {
            m_Buffer = new byte[bufferSize];
            m_BufferSize = bufferSize;
        }

		/// <summary>Attaches <paramref name="stream"/> as the destination that buffered bytes are flushed to.</summary>
		public void SetStream(Stream stream) => m_Stream = stream;

		/// <summary>Flushes the underlying stream's own buffers (does not flush <see cref="m_Buffer"/> itself).</summary>
		public void FlushStream() => m_Stream.Flush();

		/// <summary>Closes the underlying stream.</summary>
		public void CloseStream() => m_Stream.Close();

		/// <summary>Detaches the underlying stream without closing it, so this buffer no longer references it.</summary>
		public void ReleaseStream() => m_Stream = null;

		/// <summary>Resets the processed-size counter and write position, discarding any buffered bytes.</summary>
		public void Init()
        {
            m_ProcessedSize = 0;
            m_Pos = 0;
        }

        /// <summary>Appends <paramref name="b"/> to the buffer, flushing to the stream first if the buffer is full.</summary>
        public void WriteByte(byte b)
        {
            m_Buffer[m_Pos++] = b;
            if (m_Pos >= m_BufferSize) {
	            FlushData();
            }
        }

        /// <summary>Writes any buffered bytes to the underlying stream and resets the write position.</summary>
        public void FlushData()
        {
            if (m_Pos == 0) {
	            return;
            }
            m_Stream.Write(m_Buffer, 0, (int) m_Pos);
            m_Pos = 0;
        }

		/// <summary>Gets the total number of bytes processed so far, including any still buffered.</summary>
		/// <returns>The sum of previously flushed bytes and the current buffered position.</returns>
		public ulong GetProcessedSize() => m_ProcessedSize + m_Pos;
	}
}