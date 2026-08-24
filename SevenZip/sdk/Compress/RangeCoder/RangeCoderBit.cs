using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>
    /// Encodes binary symbols for range compression using adaptive<br/>
    /// probability estimates.
    /// </summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Relationship |
    /// |---|---|
    /// | <see cref="UInt32"/> | Stores probability estimates and probability lookup table values. |
    /// | <see cref="Encoder"/> | Range encoder parameter receiving encoded symbols. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 38 | <see cref="kBitModelTotal"/> | Specifies the constant k Bit Model Total. |
    /// | 41 | <see cref="kNumBitModelTotalBits"/> | Specifies the constant k Num Bit Model Total Bits. |
    /// | 44 | <see cref="kNumBitPriceShiftBits"/> | Specifies the constant k Num Bit Price Shift Bits. |
    /// | 76 | <see cref="Init"/> | Initializes the probability estimate to a neutral state. |
    /// | 90 | <see cref="Encode"/> | Encodes a binary symbol and updates the probability model based on  the encoded value. |
    /// | 114 | <see cref="GetPrice"/> | Returns the encoding price for a specified symbol. |
    /// | 117 | <see cref="GetPrice0"/> | Returns the encoding price for symbol 0. |
    /// | 120 | <see cref="GetPrice1"/> | Returns the encoding price for symbol 1. |
    /// </remarks>
    /// <seealso cref="Encoder">Encoder: manages the range-coded bitstream being written.</seealso>
    /// <!-- <example> block is managed by check-stale -->
    [DocState(Pass = 2, MTime = "2026-08-24T14:20:49Z", Digest = "f5ffaa18b597d65c4dc1ac09d0039eac0a83ddcf45b1ada907389129b48c4878", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoderBit.cs", Since = "2026-08-23")]
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

        /// <summary>
        /// Holds the current probability estimate for symbol 0 in the range<br/>
        /// coding model.
        /// </summary>
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

		/// <summary>Initializes the probability estimate to a neutral state.</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/*public void UpdateModel(uint symbol)
		{
			if (symbol == 0)
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
			else
				Prob -= (Prob) >> kNumMoveBits;
		}*/

		/// <summary>
		/// Encodes a binary symbol and updates the probability model based on<br/>
		/// the encoded value.
		/// </summary>
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

		/// <summary>Returns the encoding price for a specified symbol.</summary>
		public uint GetPrice(uint symbol) => ProbPrices[(((Prob - symbol) ^ ((-(int) symbol))) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];

		/// <summary>Returns the encoding price for symbol 0.</summary>
		public uint GetPrice0() => ProbPrices[Prob >> kNumMoveReducingBits];

		/// <summary>Returns the encoding price for symbol 1.</summary>
		public uint GetPrice1() => ProbPrices[(kBitModelTotal - Prob) >> kNumMoveReducingBits];
	}

    /// <summary>
    /// Decodes binary symbols from range-compressed data using adaptive<br/>
    /// probability estimates.
    /// </summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Relationship |
    /// |---|---|
    /// | <see cref="Decoder"/> | Range decoder parameter providing coded bits to decode. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 150 | <see cref="kBitModelTotal"/> | Specifies the constant k Bit Model Total. |
    /// | 153 | <see cref="kNumBitModelTotalBits"/> | Specifies the constant k Num Bit Model Total Bits. |
    /// | 173 | <see cref="Init"/> | Initializes the probability estimate to a neutral state. |
    /// | 179 | <see cref="Decode"/> | Decodes and returns a binary symbol from the decoder, updating the  probability model. |
    /// </remarks>
    /// <seealso cref="Decoder">Decoder: manages the range-coded bitstream being read.</seealso>
    /// <!-- <example> block is managed by check-stale -->
    [DocState(Pass = 2, MTime = "2026-08-24T14:20:49Z", Digest = "d1dcb4c6e1f652d32fb314fc49a5f12cb15f536ac814155fda9cbee04575ceea", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoderBit.cs", Since = "2026-08-23")]
    internal struct BitDecoder
    {

        /// <summary>Specifies the constant k Bit Model Total.</summary>
        public const uint kBitModelTotal = (1 << kNumBitModelTotalBits);

        /// <summary>Specifies the constant k Num Bit Model Total Bits.</summary>
        public const int kNumBitModelTotalBits = 11;

        /// <summary>Specifies the constant k Num Move Bits.</summary>
        private const int kNumMoveBits = 5;

        /// <summary>
        /// Holds the current probability estimate for symbol 0 in the range<br/>
        /// decoding model.
        /// </summary>
        private uint Prob;

		/*public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0)
				Prob += (kBitModelTotal - Prob) >> numMoveBits;
			else
				Prob -= (Prob) >> numMoveBits;
		}*/

		/// <summary>Initializes the probability estimate to a neutral state.</summary>
		public void Init() => Prob = kBitModelTotal >> 1;

		/// <summary>
		/// Decodes and returns a binary symbol from the decoder, updating the<br/>
		/// probability model.
		/// </summary>
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