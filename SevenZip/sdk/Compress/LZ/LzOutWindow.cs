using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.LZ
{
    using System.IO;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "df75b33a8399ce273334a9916da0281a9ba8e7ca246a4a0e5c0d12d794e838f3", Stale = true, Path = "sdk/Compress/LZ/LzOutWindow.cs", Since = "2026-08-23")]
    internal class OutWindow
    {
        private byte[] _buffer;
        private uint _pos;
        private Stream _stream;
        private uint _streamPos;
        private uint _windowSize;
        public uint TrainSize;

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
        public void ReleaseStream()
        {
            Flush();
            _stream = null;
        }

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
        public void PutByte(byte b)
        {
            _buffer[_pos++] = b;
            if (_pos >= _windowSize) {
	            Flush();
            }
        }

        /// <summary>TODO: LLM</summary>
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