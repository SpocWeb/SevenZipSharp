using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>
    /// Encodes symbols using a hierarchical bit-tree model with range encoding.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="BitTreeEncoder"/> | Initializes a new instance of BitTreeEncoder with the specified numBitLevels. |
    /// | 42 | <see cref="Init"/> | Initializes all encoder models in the bit tree. |
    /// | 49 | <see cref="Encode"/> | Encodes a symbol using the bit tree hierarchy through range encoding. |
    /// | 62 | <see cref="ReverseEncode"/> | Encodes a symbol in reverse bit order through range encoding. |
    /// | 76 | <see cref="GetPrice"/> | Calculates the bit-tree encoding price for a symbol. |
    /// | 92 | <see cref="ReverseGetPrice"/> | Calculates the bit-tree encoding price for a symbol in reverse bit order. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="BitEncoder"/> | Underlying model for each bit level in the tree. |
    /// | <see cref="Encoder"/> | Provided by the caller to encode individual bits. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:19:18Z", Digest = "3e6665e51e681e932d8f782eb90806585bd13db45b8f4b1c802a0992d66e6287", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoderBitTree.cs", Since = "2026-08-23")]
    internal struct BitTreeEncoder
    {
        private readonly BitEncoder[] Models;
        private readonly int NumBitLevels;

        /// <summary>Initializes a new instance of <see cref="BitTreeEncoder"/> with the specified <paramref name="numBitLevels"/>.</summary>
        public BitTreeEncoder(int numBitLevels)
        {
            NumBitLevels = numBitLevels;
            Models = new BitEncoder[1 << numBitLevels];
        }

        /// <summary>Initializes all encoder models in the bit tree.</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>Encodes a symbol using the bit tree hierarchy through range encoding.</summary>
        public void Encode(Encoder rangeEncoder, UInt32 symbol)
        {
            UInt32 m = 1;
            for (int bitIndex = NumBitLevels; bitIndex > 0;)
            {
                bitIndex--;
                UInt32 bit = (symbol >> bitIndex) & 1;
                Models[m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
            }
        }

        /// <summary>Encodes a symbol in reverse bit order through range encoding.</summary>
        public void ReverseEncode(Encoder rangeEncoder, UInt32 symbol)
        {
            UInt32 m = 1;
            for (UInt32 i = 0; i < NumBitLevels; i++)
            {
                UInt32 bit = symbol & 1;
                Models[m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
                symbol >>= 1;
            }
        }

        /// <summary>Calculates the bit-tree encoding price for a symbol.</summary>
        /// <returns>The cumulative price of encoding the symbol through all bit levels.</returns>
        public UInt32 GetPrice(UInt32 symbol)
        {
            UInt32 price = 0;
            UInt32 m = 1;
            for (int bitIndex = NumBitLevels; bitIndex > 0;)
            {
                bitIndex--;
                UInt32 bit = (symbol >> bitIndex) & 1;
                price += Models[m].GetPrice(bit);
                m = (m << 1) + bit;
            }
            return price;
        }

        /// <summary>Calculates the bit-tree encoding price for a symbol in reverse bit order.</summary>
        /// <returns>The cumulative price of encoding the symbol bits in reverse through all levels.</returns>
        public UInt32 ReverseGetPrice(UInt32 symbol)
        {
            UInt32 price = 0;
            UInt32 m = 1;
            for (int i = NumBitLevels; i > 0; i--)
            {
                UInt32 bit = symbol & 1;
                symbol >>= 1;
                price += Models[m].GetPrice(bit);
                m = (m << 1) | bit;
            }
            return price;
        }

        /// <inheritdoc cref="ReverseGetPrice(UInt32)"/>
        public static UInt32 ReverseGetPrice(BitEncoder[] Models, UInt32 startIndex,
                                             int NumBitLevels, UInt32 symbol)
        {
            UInt32 price = 0;
            UInt32 m = 1;
            for (int i = NumBitLevels; i > 0; i--)
            {
                UInt32 bit = symbol & 1;
                symbol >>= 1;
                price += Models[startIndex + m].GetPrice(bit);
                m = (m << 1) | bit;
            }
            return price;
        }

        /// <inheritdoc cref="ReverseEncode(Encoder, UInt32)"/>
        public static void ReverseEncode(BitEncoder[] Models, UInt32 startIndex,
                                         Encoder rangeEncoder, int NumBitLevels, UInt32 symbol)
        {
            UInt32 m = 1;
            for (int i = 0; i < NumBitLevels; i++)
            {
                UInt32 bit = symbol & 1;
                Models[startIndex + m].Encode(rangeEncoder, bit);
                m = (m << 1) | bit;
                symbol >>= 1;
            }
        }
    }

    /// <summary>
    /// Decodes symbols using a hierarchical bit-tree model with range decoding.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 164 | <see cref="BitTreeDecoder"/> | Initializes a new instance of BitTreeDecoder with the specified numBitLevels. |
    /// | 171 | <see cref="Init"/> | Initializes all decoder models in the bit tree. |
    /// | 179 | <see cref="Decode"/> | Decodes a symbol from the range decoder using the bit tree hierarchy. |
    /// | 189 | <see cref="ReverseDecode"/> | Decodes a symbol from the range decoder in reverse bit order. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="BitDecoder"/> | Underlying model for each bit level in the tree. |
    /// | <see cref="Decoder"/> | Provided by the caller to decode individual bits. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:19:18Z", Digest = "24d8748304d1fc3c16868bce8b838883cfc9078913dcc7136e4d3b8dce160069", Stale = false, Path = "sdk/Compress/RangeCoder/RangeCoderBitTree.cs", Since = "2026-08-23")]
    internal struct BitTreeDecoder
    {
        private readonly BitDecoder[] Models;
        private readonly int NumBitLevels;

        /// <summary>Initializes a new instance of <see cref="BitTreeDecoder"/> with the specified <paramref name="numBitLevels"/>.</summary>
        public BitTreeDecoder(int numBitLevels)
        {
            NumBitLevels = numBitLevels;
            Models = new BitDecoder[1 << numBitLevels];
        }

        /// <summary>Initializes all decoder models in the bit tree.</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>Decodes a symbol from the range decoder using the bit tree hierarchy.</summary>
        /// <returns>The decoded symbol.</returns>
        public uint Decode(Decoder rangeDecoder)
        {
            uint m = 1;
            for (int bitIndex = NumBitLevels; bitIndex > 0; bitIndex--)
                m = (m << 1) + Models[m].Decode(rangeDecoder);
            return m - ((uint) 1 << NumBitLevels);
        }

        /// <summary>Decodes a symbol from the range decoder in reverse bit order.</summary>
        /// <returns>The decoded symbol with bits assembled in reverse order.</returns>
        public uint ReverseDecode(Decoder rangeDecoder)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < NumBitLevels; bitIndex++)
            {
                uint bit = Models[m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= (bit << bitIndex);
            }
            return symbol;
        }

        /// <inheritdoc cref="ReverseDecode(Decoder)"/>
        public static uint ReverseDecode(BitDecoder[] Models, UInt32 startIndex,
                                         Decoder rangeDecoder, int NumBitLevels)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < NumBitLevels; bitIndex++)
            {
                uint bit = Models[startIndex + m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= (bit << bitIndex);
            }
            return symbol;
        }
    }
}