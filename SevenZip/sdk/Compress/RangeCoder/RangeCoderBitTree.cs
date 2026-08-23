using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "e97eec3d47c313f5adabb97ee7ae0f47660c9ad8372be2f5effdf76cc471e5db", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoderBitTree.cs", Since = "2026-08-23")]
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

        /// <summary>TODO: LLM</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

        /// <summary>TODO: LLM</summary>
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

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "1b6a9ed139c44c4551aab7ab28b5fa0c82afaa093a77dfcbb2e3de4d987325db", Stale = true, Path = "sdk/Compress/RangeCoder/RangeCoderBitTree.cs", Since = "2026-08-23")]
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

        /// <summary>TODO: LLM</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>TODO: LLM</summary>
        public uint Decode(Decoder rangeDecoder)
        {
            uint m = 1;
            for (int bitIndex = NumBitLevels; bitIndex > 0; bitIndex--)
                m = (m << 1) + Models[m].Decode(rangeDecoder);
            return m - ((uint) 1 << NumBitLevels);
        }

        /// <summary>TODO: LLM</summary>
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