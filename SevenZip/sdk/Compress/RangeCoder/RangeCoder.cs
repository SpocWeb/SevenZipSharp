using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;
    using System.IO;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "0192f06a43b15370f75c9923b20f0e5e4d5efd6fab23cd2cbeabb2d0f6e23904", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoder.cs", Since = "2026-08-23")]
    internal class Encoder
    {

        /// <summary>Specifies the constant k Top Value.</summary>
        public const uint kTopValue = (1 << 24);
        private byte _cache;
        private uint _cacheSize;

        public UInt64 Low;
        public uint Range;

        private long StartPosition;
        private Stream Stream;

		/// <summary>TODO: LLM</summary>
		public void SetStream(Stream stream) => Stream = stream;

		/// <summary>TODO: LLM</summary>
		public void ReleaseStream() => Stream = null;

		/// <summary>TODO: LLM</summary>
		public void Init()
        {
            StartPosition = Stream.Position;

            Low = 0;
            Range = 0xFFFFFFFF;
            _cacheSize = 1;
            _cache = 0;
        }

        /// <summary>TODO: LLM</summary>
        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
                ShiftLow();
        }

		/// <summary>TODO: LLM</summary>
		public void FlushStream() => Stream.Flush();

		/*public void CloseStream()
		{
			Stream.Close();
		}*/

		/*public void Encode(uint start, uint size, uint total)
		{
			Low += start * (Range /= total);
			Range *= size;
			while (Range < kTopValue)
			{
				Range <<= 8;
				ShiftLow();
			}
		}*/

		/// <summary>TODO: LLM</summary>
		public void ShiftLow()
        {
            if ((uint) Low < 0xFF000000 || (uint) (Low >> 32) == 1)
            {
                byte temp = _cache;
                do
                {
                    Stream.WriteByte((byte) (temp + (Low >> 32)));
                    temp = 0xFF;
                } while (--_cacheSize != 0);
                _cache = (byte) (((uint) Low) >> 24);
            }
            _cacheSize++;
            Low = ((uint) Low) << 8;
        }

        /// <summary>TODO: LLM</summary>
        public void EncodeDirectBits(uint v, int numTotalBits)
        {
            for (int i = numTotalBits - 1; i >= 0; i--)
            {
                Range >>= 1;
                if (((v >> i) & 1) == 1) {
	                Low += Range;
                }
                if (Range < kTopValue)
                {
                    Range <<= 8;
                    ShiftLow();
                }
            }
        }

		/*public void EncodeBit(uint size0, int numTotalBits, uint symbol)
		{
			uint newBound = (Range >> numTotalBits) * size0;
			if (symbol == 0)
				Range = newBound;
			else
			{
				Low += newBound;
				Range -= newBound;
			}
			while (Range < kTopValue)
			{
				Range <<= 8;
				ShiftLow();
			}
		}*/

		/// <summary>TODO: LLM</summary>
		public long GetProcessedSizeAdd() => _cacheSize +
				   Stream.Position - StartPosition + 4;// (long)Stream.GetProcessedSize();
	}

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "44a865a4a0cc0dc8e020fb1ca33fe946485b9ba56a41931a9a2124b679922079", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoder.cs", Since = "2026-08-23")]
    internal class Decoder
    {

        /// <summary>Specifies the constant k Top Value.</summary>
        public const uint kTopValue = (1 << 24);
        public uint Code;
        public uint Range;
        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        public Stream Stream;

        /// <summary>TODO: LLM</summary>
        public void Init(Stream stream)
        {
            // Stream.Init(stream);
            Stream = stream;

            Code = 0;
            Range = 0xFFFFFFFF;
            for (int i = 0; i < 5; i++)
                Code = (Code << 8) | (byte) Stream.ReadByte();
        }

		/// <summary>TODO: LLM</summary>
		public void ReleaseStream() =>
			// Stream.ReleaseStream();
			Stream = null;

		/*public void CloseStream()
		{
			Stream.Close();
		}*/

		/*public void Normalize()
		{
			while (Range < kTopValue)
			{
				Code = (Code << 8) | (byte)Stream.ReadByte();
				Range <<= 8;
			}
		}*/

		/*public void Normalize2()
		{
			if (Range < kTopValue)
			{
				Code = (Code << 8) | (byte)Stream.ReadByte();
				Range <<= 8;
			}
		}*/

		/*public uint GetThreshold(uint total)
		{
			return Code / (Range /= total);
		}*/

		/*public void Decode(uint start, uint size, uint total)
		{
			Code -= start * Range;
			Range *= size;
			Normalize();
		}*/

		/// <summary>TODO: LLM</summary>
		public uint DecodeDirectBits(int numTotalBits)
        {
            uint range = Range;
            uint code = Code;
            uint result = 0;
            for (int i = numTotalBits; i > 0; i--)
            {
                range >>= 1;
                /*
				result <<= 1;
				if (code >= range)
				{
					code -= range;
					result |= 1;
				}
				*/
                uint t = (code - range) >> 31;
                code -= range & (t - 1);
                result = (result << 1) | (1 - t);

                if (range < kTopValue)
                {
                    code = (code << 8) | (byte) Stream.ReadByte();
                    range <<= 8;
                }
            }
            Range = range;
            Code = code;
            return result;
        }

        /*public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint newBound = (Range >> numTotalBits) * size0;
			uint symbol;
			if (Code < newBound)
			{
				symbol = 0;
				Range = newBound;
			}
			else
			{
				symbol = 1;
				Code -= newBound;
				Range -= newBound;
			}
			Normalize();
			return symbol;
		}*/

        // ulong GetProcessedSize() {return Stream.GetProcessedSize(); }
    }
}