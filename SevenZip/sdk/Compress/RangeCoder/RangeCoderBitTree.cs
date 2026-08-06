namespace SevenZip.Sdk.Compression.RangeCoder
{
    using System;

    /// <summary>Encodes a fixed-width symbol as a sequence of adaptive bits walked <br/>
    /// down a binary probability tree, most significant bit first.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 13 | <see cref="BitTreeEncoder"/> | Initializes a new instance of BitTreeEncoder with the specified numBitLevels. |
    /// | 20 | <see cref="Init"/> | Resets every level's bit probability to the unbiased midpoint. |
    /// | 28 | <see cref="Encode"/> | Encodes   bit by bit from most to least   significant, walking the probability tree one level per bit. |
    /// | 42 | <see cref="ReverseEncode"/> | Encodes   bit by bit from least to most   significant, used for the LZMA distance-alignment and footer bits. |
    /// | 56 | <see cref="GetPrice"/> | Estimates the bit cost of encoding   most   significant bit first, without side effects. |
    /// | 72 | <see cref="ReverseGetPrice"/> | Estimates the bit cost of encoding   least   significant bit first, without side effects. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="BitEncoder"/> | Used as a field. |
    /// | <see cref="Encoder"/> | Passed as a parameter. |
    /// | <see cref="UInt32"/> | Passed as a parameter. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: da7df38a1e8e5d304ef30affa16488a69e771695a9a390865b4f502391ae3906
    /// </code>
    /// </example>
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

        /// <summary>Resets every level's bit probability to the unbiased midpoint.</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>Encodes <paramref name='symbol'/> bit by bit from most to least <br/>
        /// significant, walking the probability tree one level per bit.</summary>
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

        /// <summary>Encodes <paramref name='symbol'/> bit by bit from least to most <br/>
        /// significant, used for the LZMA distance-alignment and footer bits.</summary>
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

        /// <summary>Estimates the bit cost of encoding <paramref name='symbol'/> most <br/>
        /// significant bit first, without side effects.</summary>
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

        /// <summary>Estimates the bit cost of encoding <paramref name='symbol'/> least <br/>
        /// significant bit first, without side effects.</summary>
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

    /// <summary>Decodes a fixed-width symbol from a sequence of adaptive bits walked <br/>
    /// down a binary probability tree, most significant bit first.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 125 | <see cref="BitTreeDecoder"/> | Initializes a new instance of BitTreeDecoder with the specified numBitLevels. |
    /// | 132 | <see cref="Init"/> | Resets every level's bit probability to the unbiased midpoint. |
    /// | 141 | <see cref="Decode"/> | Decodes a symbol bit by bit from most to least significant,   walking the probability tree one level per bit. |
    /// | 152 | <see cref="ReverseDecode"/> | Decodes a symbol bit by bit from least to most significant, the   counterpart to ReverseEncode(Encoder, UInt32). |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="BitDecoder"/> | Used as a field. |
    /// | <see cref="Decoder"/> | Passed as a parameter. |
    /// | <see cref="UInt32"/> | Passed as a parameter. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: a1b6fa8e50092521efa4da1fa5b0275a87a866dbf0a71923c3b4a02d57db534c
    /// </code>
    /// </example>
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

        /// <summary>Resets every level's bit probability to the unbiased midpoint.</summary>
        public void Init()
        {
            for (uint i = 1; i < (1 << NumBitLevels); i++)
                Models[i].Init();
        }

        /// <summary>Decodes a symbol bit by bit from most to least significant, <br/>
        /// walking the probability tree one level per bit.</summary>
        /// <returns>The decoded fixed-width symbol.</returns>
        public uint Decode(Decoder rangeDecoder)
        {
            uint m = 1;
            for (int bitIndex = NumBitLevels; bitIndex > 0; bitIndex--)
                m = (m << 1) + Models[m].Decode(rangeDecoder);
            return m - ((uint) 1 << NumBitLevels);
        }

        /// <summary>Decodes a symbol bit by bit from least to most significant, the <br/>
        /// counterpart to <see cref="BitTreeEncoder.ReverseEncode(Encoder, UInt32)"/>.</summary>
        /// <returns>The decoded fixed-width symbol.</returns>
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