using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "8b9d321f2461c57997fc1a51e2e05396f03d9f0560d2ecd9028f3dfa6e0949c5", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoderBit.cs", Since = "2026-08-23")]
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

		/// <summary>TODO: LLM</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/*public void UpdateModel(uint symbol)
		{
			if (symbol == 0)
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
			else
				Prob -= (Prob) >> kNumMoveBits;
		}*/

		/// <summary>TODO: LLM</summary>
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

		/// <summary>TODO: LLM</summary>
		public uint GetPrice(uint symbol) => ProbPrices[(((Prob - symbol) ^ ((-(int) symbol))) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];

		/// <summary>TODO: LLM</summary>
		public uint GetPrice0() => ProbPrices[Prob >> kNumMoveReducingBits];

		/// <summary>TODO: LLM</summary>
		public uint GetPrice1() => ProbPrices[(kBitModelTotal - Prob) >> kNumMoveReducingBits];
	}

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "b155ca3ac3c69555b11d9028d7b107488b4368082ef2e66c54a63efe8a6fe129", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoderBit.cs", Since = "2026-08-23")]
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

		/// <summary>TODO: LLM</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/// <summary>TODO: LLM</summary>
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