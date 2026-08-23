using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Buffer
{
    using System.IO;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:44Z", Digest = "8fa9cde83bd95224abc0f25627b2003a80cf707e18040674af6803c42fedfe68", Stale = true, Path = "sdk/Common/OutBuffer.cs", Since = "2026-08-23")]
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
        [System.ComponentModel.Description("Initializes a new instance of the OutBuffer class")]
        public OutBuffer(uint bufferSize)
        {
            m_Buffer = new byte[bufferSize];
            m_BufferSize = bufferSize;
        }

		/// <summary>TODO: LLM</summary>
		public void SetStream(Stream stream) => m_Stream = stream;

		/// <summary>TODO: LLM</summary>
		public void FlushStream() => m_Stream.Flush();

		/// <summary>TODO: LLM</summary>
		public void CloseStream() => m_Stream.Close();

		/// <summary>TODO: LLM</summary>
		public void ReleaseStream() => m_Stream = null;

		/// <summary>TODO: LLM</summary>
		public void Init()
        {
            m_ProcessedSize = 0;
            m_Pos = 0;
        }

        /// <summary>TODO: LLM</summary>
        public void WriteByte(byte b)
        {
            m_Buffer[m_Pos++] = b;
            if (m_Pos >= m_BufferSize) {
	            FlushData();
            }
        }

        /// <summary>TODO: LLM</summary>
        public void FlushData()
        {
            if (m_Pos == 0) {
	            return;
            }
            m_Stream.Write(m_Buffer, 0, (int) m_Pos);
            m_Pos = 0;
        }

		/// <summary>TODO: LLM</summary>
		public ulong GetProcessedSize() => m_ProcessedSize + m_Pos;
	}
}