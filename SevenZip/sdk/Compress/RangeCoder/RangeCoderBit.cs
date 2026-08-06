namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>Encodes a single adaptive binary symbol into a range-coder <see cref="Encoder"/>, <br/>
    /// tracking and updating that bit's probability of being zero.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 11 | <see cref="kBitModelTotal"/> | Specifies the constant k Bit Model Total. |
    /// | 14 | <see cref="kNumBitModelTotalBits"/> | Specifies the constant k Num Bit Model Total Bits. |
    /// | 17 | <see cref="kNumBitPriceShiftBits"/> | Specifies the constant k Num Bit Price Shift Bits. |
    /// | 45 | <see cref="Init"/> | Resets this bit's probability to the unbiased midpoint (50/50). |
    /// | 57 | <see cref="Encode"/> | Encodes   (0 or 1) using this bit's current   probability, then adapts that probability toward the encoded value. |
    /// | 82 | <see cref="GetPrice"/> | Estimates the bit cost of encoding   (0 or 1)   under this bit's current probability, without side effects. |
    /// | 85 | <see cref="GetPrice0"/> | Estimates the bit cost of encoding a 0 under this bit's current probability. |
    /// | 88 | <see cref="GetPrice1"/> | Estimates the bit cost of encoding a 1 under this bit's current probability. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt32"/> | Used as a field. |
    /// | <see cref="Encoder"/> | Passed as a parameter. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 1dfbcf0e81559531aafef0e918719857bb6f1f7ba4717a6e0364846770bf026d
    /// </code>
    /// </example>
    internal struct BitEncoder
    {

        /// <summary>Specifies the constant k Bit Model Total.</summary>
        public const uint kBitModelTotal = (1 << kNumBitModelTotalBits);

        /// <summary>Specifies the constant k Num Bit Model Total Bits.</summary>
        public const int kNumBitModelTotalBits = 11;

        /// <summary>Specifies the constant k Num Bit Price Shift Bits.</summary>
        public const int kNumBitPriceShiftBits = 6;

        /// <summary>Specifies the constant k Num Move Bits.</summary>
        private const int kNumMoveBits = 5;

        /// <summary>Specifies the constant k Num Move Reducing Bits.</summary>
        private const int kNumMoveReducingBits = 2;

        /// <summary>Gets the prob Prices.</summary>
        private static readonly UInt32[] ProbPrices = new UInt32[kBitModelTotal >> kNumMoveReducingBits];

        private uint Prob;

        /// <summary>Initializes a new instance of <see cref="BitEncoder"/>.</summary>
        static BitEncoder()
        {
            const int kNumBits = (kNumBitModelTotalBits - kNumMoveReducingBits);
            for (int i = kNumBits - 1; i >= 0; i--)
            {
                UInt32 start = (UInt32) 1 << (kNumBits - i - 1);
                UInt32 end = (UInt32) 1 << (kNumBits - i);
                for (UInt32 j = start; j < end; j++)
                    ProbPrices[j] = ((UInt32) i << kNumBitPriceShiftBits) +
                                    (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
            }
        }

		/// <summary>Resets this bit's probability to the unbiased midpoint (50/50).</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/*public void UpdateModel(uint symbol)
		{
			if (symbol == 0)
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
			else
				Prob -= (Prob) >> kNumMoveBits;
		}*/

		/// <summary>Encodes <paramref name='symbol'/> (0 or 1) using this bit's current <br/>
		/// probability, then adapts that probability toward the encoded value.</summary>
		public void Encode(Encoder encoder, uint symbol)
        {
            // encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
            // UpdateModel(symbol);
            uint newBound = (encoder.Range >> kNumBitModelTotalBits)*Prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                Prob -= (Prob) >> kNumMoveBits;
            }
            if (encoder.Range < Encoder.kTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

		/// <summary>Estimates the bit cost of encoding <paramref name='symbol'/> (0 or 1) <br/>
		/// under this bit's current probability, without side effects.</summary>
		public uint GetPrice(uint symbol) => ProbPrices[(((Prob - symbol) ^ ((-(int) symbol))) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];

		/// <summary>Estimates the bit cost of encoding a 0 under this bit's current probability.</summary>
		public uint GetPrice0() => ProbPrices[Prob >> kNumMoveReducingBits];

		/// <summary>Estimates the bit cost of encoding a 1 under this bit's current probability.</summary>
		public uint GetPrice1() => ProbPrices[(kBitModelTotal - Prob) >> kNumMoveReducingBits];
	}

    /// <summary>Decodes a single adaptive binary symbol from a range-coder <see cref="Decoder"/>, <br/>
    /// tracking and updating that bit's probability of being zero.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 97 | <see cref="kBitModelTotal"/> | Specifies the constant k Bit Model Total. |
    /// | 100 | <see cref="kNumBitModelTotalBits"/> | Specifies the constant k Num Bit Model Total Bits. |
    /// | 116 | <see cref="Init"/> | Resets this bit's probability to the unbiased midpoint (50/50). |
    /// | 121 | <see cref="Decode"/> | Decodes one bit using this bit's current probability, adapting it   toward the decoded value and renormalizing the range. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Decoder"/> | Passed as a parameter. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 48f2e8105e9eca4d58352b42f75775223e0491ffaeffcdbcb70d6f7a3affe58b
    /// </code>
    /// </example>
    internal struct BitDecoder
    {

        /// <summary>Specifies the constant k Bit Model Total.</summary>
        public const uint kBitModelTotal = (1 << kNumBitModelTotalBits);

        /// <summary>Specifies the constant k Num Bit Model Total Bits.</summary>
        public const int kNumBitModelTotalBits = 11;

        /// <summary>Specifies the constant k Num Move Bits.</summary>
        private const int kNumMoveBits = 5;

        private uint Prob;

		/*public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0)
				Prob += (kBitModelTotal - Prob) >> numMoveBits;
			else
				Prob -= (Prob) >> numMoveBits;
		}*/

		/// <summary>Resets this bit's probability to the unbiased midpoint (50/50).</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/// <summary>Decodes one bit using this bit's current probability, adapting it <br/>
		/// toward the decoded value and renormalizing the range.</summary>
		/// <returns>The decoded symbol, 0 or 1.</returns>
		public uint Decode(Decoder rangeDecoder)
        {
            uint newBound = (rangeDecoder.Range >> kNumBitModelTotalBits)*Prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 0;
            }
            else
            {
                rangeDecoder.Range -= newBound;
                rangeDecoder.Code -= newBound;
                Prob -= (Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 1;
            }
        }
    }
}