using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;
    using System.IO;

    /// <summary>Encodes bit sequences using arithmetic range coding,<br/>
    /// maintaining a probability range and flushing encoded output to a stream.</summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Stream"/> | Output stream for encoded bytes. |
    /// | <see cref="UInt64"/> | Maintains low bound of the range interval. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 37 | <see cref="kTopValue"/> | Specifies the constant k Top Value. |
    /// | 48 | <see cref="SetStream"/> | Sets the underlying stream for encoded output. |
    /// | 51 | <see cref="ReleaseStream"/> | Clears the stream reference by releasing the stream. |
    /// | 55 | <see cref="Init"/> | Initializes encoder state from the current stream position,  resetting all range-coding state and output cache. |
    /// | 67 | <see cref="FlushData"/> | Flushes all remaining encoded data to the stream by  calling ShiftLow five times. |
    /// | 74 | <see cref="FlushStream"/> | Flushes the underlying stream. |
    /// | 94 | <see cref="ShiftLow"/> | Shifts the low-order 8 bits to the stream and advances the  low value, managing cached byte output across range-code boundaries. |
    /// | 112 | <see cref="EncodeDirectBits"/> | Encodes numTotalBits consecutive bits of  v by narrowing the range according to each bit value. |
    /// | 148 | <see cref="GetProcessedSizeAdd"/> | Returns the number of bytes written to the stream since  the last Init call. |
    /// </remarks>
    /// <seealso cref="Stream">Stream: output stream for encoded bytes.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T14:19:13Z", Digest = "11d2d3ae5e45757bb98c8d0887b0ace587744bd6542e1de00f2f48c066c8f507", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoder.cs", Since = "2026-08-23")]
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

		/// <summary>Sets the underlying stream for encoded output.</summary>
		public void SetStream(Stream stream) => Stream = stream;

		/// <summary>Clears the stream reference by releasing the stream.</summary>
		public void ReleaseStream() => Stream = null;

		/// <summary>Initializes encoder state from the current stream position,<br/>
		/// resetting all range-coding state and output cache.</summary>
		public void Init()
        {
            StartPosition = Stream.Position;

            Low = 0;
            Range = 0xFFFFFFFF;
            _cacheSize = 1;
            _cache = 0;
        }

        /// <summary>Flushes all remaining encoded data to the stream by<br/>
        /// calling <see cref="ShiftLow"/> five times.</summary>
        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
                ShiftLow();
        }

		/// <summary>Flushes the underlying stream.</summary>
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

		/// <summary>Shifts the low-order 8 bits to the stream and advances the<br/>
		/// low value, managing cached byte output across range-code boundaries.</summary>
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

        /// <summary>Encodes <paramref name="numTotalBits"/> consecutive bits of<br/>
        /// <paramref name="v"/> by narrowing the range according to each bit value.</summary>
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

		/// <summary>Returns the number of bytes written to the stream since<br/>
		/// the last <see cref="Init"/> call.</summary>
		/// <returns>The cumulative byte count including cache and stream position.</returns>
		public long GetProcessedSizeAdd() => _cacheSize +
				   Stream.Position - StartPosition + 4;// (long)Stream.GetProcessedSize();
	}

    /// <summary>Decodes bit sequences from a stream using arithmetic range coding<br/>
    /// by tracking a code value and probability range.</summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Stream"/> | Input stream for encoded bytes. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 176 | <see cref="kTopValue"/> | Specifies the constant k Top Value. |
    /// | 184 | <see cref="Init"/> | Initializes decoder state from the given stream by reading 5  seed bytes and resetting range-code state. |
    /// | 196 | <see cref="ReleaseStream"/> | Clears the stream reference by releasing the stream. |
    /// | 238 | <see cref="DecodeDirectBits"/> | Decodes and returns numTotalBits consecutive  bits from the stream by narrowing the range for each bit. |
    /// </remarks>
    /// <seealso cref="Stream">Stream: input stream for encoded bytes.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T14:19:13Z", Digest = "1aa247d30f8d0a7c095c1d022b86f7f49f76003c73cc14278b47f765f8763f2e", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoder.cs", Since = "2026-08-23")]
    internal class Decoder
    {

        /// <summary>Specifies the constant k Top Value.</summary>
        public const uint kTopValue = (1 << 24);
        public uint Code;
        public uint Range;
        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        public Stream Stream;

        /// <summary>Initializes decoder state from the given stream by reading 5<br/>
        /// seed bytes and resetting range-code state.</summary>
        public void Init(Stream stream)
        {
            // Stream.Init(stream);
            Stream = stream;

            Code = 0;
            Range = 0xFFFFFFFF;
            for (int i = 0; i < 5; i++)
                Code = (Code << 8) | (byte) Stream.ReadByte();
        }

		/// <summary>Clears the stream reference by releasing the stream.</summary>
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

		/// <summary>Decodes and returns <paramref name="numTotalBits"/> consecutive<br/>
		/// bits from the stream by narrowing the range for each bit.</summary>
		/// <returns>The decoded bit sequence as an unsigned integer.</returns>
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