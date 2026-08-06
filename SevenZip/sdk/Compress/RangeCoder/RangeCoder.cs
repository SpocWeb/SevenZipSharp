namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;
    using System.IO;

    /// <summary>Arithmetic range coder that serializes a probability-weighted bit <br/>
    /// stream into a byte stream, underlying all LZMA symbol encoders.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 12 | <see cref="kTopValue"/> | Specifies the constant k Top Value. |
    /// | 24 | <see cref="SetStream"/> | Binds the encoder to the   that will receive   the encoded byte output. |
    /// | 27 | <see cref="ReleaseStream"/> | Releases the encoder's reference to its output stream. |
    /// | 31 | <see cref="Init"/> | Resets the range, low value, and byte cache to their initial state   at the start of a new encoded stream. |
    /// | 43 | <see cref="FlushData"/> | Flushes the remaining buffered low-value bytes so the final range   state is fully committed to the output stream. |
    /// | 50 | <see cref="FlushStream"/> | Flushes the underlying output stream's own buffers. |
    /// | 70 | <see cref="ShiftLow"/> | Emits the top byte of Low once it is fully determined,   carrying pending 0xFF cache bytes when a carry propagates. |
    /// | 88 | <see cref="EncodeDirectBits"/> | Encodes the low   bits of     as equiprobable (non-adaptive) bits, halving the range each step. |
    /// | 123 | <see cref="GetProcessedSizeAdd"/> | Returns the number of bytes written to the output stream so far,   including the bytes still pending in the low-value cache. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt64"/> | Used as a field. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 9e90f098330dd0f3251427c7dd49a40fba7853e19b7c700d24a5a7787de5656c
    /// </code>
    /// </example>
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

		/// <summary>Binds the encoder to the <paramref name='stream'/> that will receive <br/>
		/// the encoded byte output.</summary>
		public void SetStream(Stream stream) => Stream = stream;

		/// <summary>Releases the encoder's reference to its output stream.</summary>
		public void ReleaseStream() => Stream = null;

		/// <summary>Resets the range, low value, and byte cache to their initial state <br/>
		/// at the start of a new encoded stream.</summary>
		public void Init()
        {
            StartPosition = Stream.Position;

            Low = 0;
            Range = 0xFFFFFFFF;
            _cacheSize = 1;
            _cache = 0;
        }

        /// <summary>Flushes the remaining buffered low-value bytes so the final range <br/>
        /// state is fully committed to the output stream.</summary>
        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
                ShiftLow();
        }

		/// <summary>Flushes the underlying output stream's own buffers.</summary>
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

		/// <summary>Emits the top byte of <see cref="Low"/> once it is fully determined, <br/>
		/// carrying pending 0xFF cache bytes when a carry propagates.</summary>
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

        /// <summary>Encodes the low <paramref name='numTotalBits'/> bits of <paramref name='v'/> <br/>
        /// as equiprobable (non-adaptive) bits, halving the range each step.</summary>
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

		/// <summary>Returns the number of bytes written to the output stream so far, <br/>
		/// including the bytes still pending in the low-value cache.</summary>
		public long GetProcessedSizeAdd() => _cacheSize +
				   Stream.Position - StartPosition + 4;// (long)Stream.GetProcessedSize();
	}

    /// <summary>Arithmetic range coder that reconstructs a probability-weighted bit <br/>
    /// stream from a byte stream, underlying all LZMA symbol decoders.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 133 | <see cref="kTopValue"/> | Specifies the constant k Top Value. |
    /// | 141 | <see cref="Init"/> | Binds the decoder to   and primes the code   register by reading the initial 5 bytes. |
    /// | 153 | <see cref="ReleaseStream"/> | Releases the decoder's reference to its input stream. |
    /// | 195 | <see cref="DecodeDirectBits"/> | Decodes   equiprobable (non-adaptive) bits,   the counterpart to EncodeDirectBits. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 59888211669e95438a888c326dd6d7c5692170dfe4bb4570ee3ceb8b458f9a14
    /// </code>
    /// </example>
    internal class Decoder
    {

        /// <summary>Specifies the constant k Top Value.</summary>
        public const uint kTopValue = (1 << 24);
        public uint Code;
        public uint Range;
        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        public Stream Stream;

        /// <summary>Binds the decoder to <paramref name='stream'/> and primes the code <br/>
        /// register by reading the initial 5 bytes.</summary>
        public void Init(Stream stream)
        {
            // Stream.Init(stream);
            Stream = stream;

            Code = 0;
            Range = 0xFFFFFFFF;
            for (int i = 0; i < 5; i++)
                Code = (Code << 8) | (byte) Stream.ReadByte();
        }

		/// <summary>Releases the decoder's reference to its input stream.</summary>
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

		/// <summary>Decodes <paramref name='numTotalBits'/> equiprobable (non-adaptive) bits, <br/>
		/// the counterpart to <see cref="Encoder.EncodeDirectBits"/>.</summary>
		/// <returns>The decoded bits, most significant bit first.</returns>
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