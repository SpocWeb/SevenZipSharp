using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.Lzma
{
    using System;
    using System.IO;

    using SevenZip.Sdk.Compression.LZ;
    using SevenZip.Sdk.Compression.RangeCoder;

    /// <summary>
    /// The LZMA decoder class
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 65 | <see cref="Decoder"/> | Initializes the Lzma Decoder class. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="BitDecoder"/> | Used as a field. |
    /// | <see cref="LenDecoder"/> | Used as a field. |
    /// | <see cref="LiteralDecoder"/> | Used as a field. |
    /// | <see cref="OutWindow"/> | Used as a field. |
    /// | <see cref="BitTreeDecoder"/> | Used as a field. |
    /// | <see cref="Decoder"/> | Used as a field. |
    /// | <see cref="ICodeProgress"/> | Passed as a parameter. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:18:07Z", Digest = "5ee0c376cd6864e7ac51a06a62f92526bd811cf3face7e116b66ef8c588a63c3", Stale = false, Path = "sdk/Compress/LZMA/LzmaDecoder.cs", Since = "2026-08-23")]
    public class Decoder : ICoder, ISetDecoderProperties // ,System.IO.Stream
    {
        private readonly BitDecoder[] m_IsMatchDecoders = new BitDecoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        private readonly BitDecoder[] m_IsRep0LongDecoders =
            new BitDecoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        private readonly BitDecoder[] m_IsRepDecoders = new BitDecoder[Base.kNumStates];
        private readonly BitDecoder[] m_IsRepG0Decoders = new BitDecoder[Base.kNumStates];
        private readonly BitDecoder[] m_IsRepG1Decoders = new BitDecoder[Base.kNumStates];
        private readonly BitDecoder[] m_IsRepG2Decoders = new BitDecoder[Base.kNumStates];

        private readonly LenDecoder m_LenDecoder = new LenDecoder();

        private readonly LiteralDecoder m_LiteralDecoder = new LiteralDecoder();
        private readonly OutWindow m_OutWindow = new OutWindow();
        private readonly BitDecoder[] m_PosDecoders = new BitDecoder[Base.kNumFullDistances - Base.kEndPosModelIndex];
        private readonly BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[Base.kNumLenToPosStates];
        private readonly RangeCoder.Decoder m_RangeDecoder = new RangeCoder.Decoder();
        private readonly LenDecoder m_RepLenDecoder = new LenDecoder();
        private bool _solid;

        private uint m_DictionarySize;
        private uint m_DictionarySizeCheck;
        private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(Base.kNumAlignBits);

        private uint m_PosStateMask;

        /// <summary>
        /// Initializes the Lzma Decoder class.
        /// </summary>
        [System.ComponentModel.Description("Initializes the Lzma Decoder class.")]
        public Decoder()
        {
            m_DictionarySize = 0xFFFFFFFF;
            for (int i = 0; i < Base.kNumLenToPosStates; i++)
                m_PosSlotDecoder[i] = new BitTreeDecoder(Base.kNumPosSlotBits);
        }

        #region ICoder Members

        /// <summary>
        /// Codes a stream with LZMA algorithm to an output stream
        /// </summary>
        /// <param name="inStream">The input stream</param>
        /// <param name="inSize">The input size</param>
        /// <param name="outSize">The output size</param>
        /// <param name="outStream">The output stream</param>
        /// <param name="progress">Progress interface</param>
        [System.ComponentModel.Description("Codes a stream with LZMA algorithm to an output stream")]
        public void Code(Stream inStream, Stream outStream,
                         Int64 inSize, Int64 outSize, ICodeProgress progress)
        {
            Init(inStream, outStream);

            var state = new Base.State();
            state.Init();
            uint rep0 = 0, rep1 = 0, rep2 = 0, rep3 = 0;

            UInt64 nowPos64 = 0;
            var outSize64 = (UInt64) outSize;
            if (nowPos64 < outSize64)
            {
                if (m_IsMatchDecoders[state.Index << Base.kNumPosStatesBitsMax].Decode(m_RangeDecoder) != 0) {
	                throw new DataErrorException();
                }
                state.UpdateChar();
                byte b = m_LiteralDecoder.DecodeNormal(m_RangeDecoder, 0, 0);
                m_OutWindow.PutByte(b);
                nowPos64++;
            }
            while (nowPos64 < outSize64)
            {
                // UInt64 next = Math.Min(nowPos64 + (1 << 18), outSize64);
                // while(nowPos64 < next)
                {
                    uint posState = (uint) nowPos64 & m_PosStateMask;
                    if (
                        m_IsMatchDecoders[(state.Index << Base.kNumPosStatesBitsMax) + posState].Decode(m_RangeDecoder) ==
                        0)
                    {
                        byte b;
                        byte prevByte = m_OutWindow.GetByte(0);
                        if (!state.IsCharState()) {
	                        b = m_LiteralDecoder.DecodeWithMatchByte(m_RangeDecoder,
		                        (uint) nowPos64, prevByte,
		                        m_OutWindow.GetByte(rep0));
                        } else {
	                        b = m_LiteralDecoder.DecodeNormal(m_RangeDecoder, (uint) nowPos64, prevByte);
                        }
                        m_OutWindow.PutByte(b);
                        state.UpdateChar();
                        nowPos64++;
                    }
                    else
                    {
                        uint len;
                        if (m_IsRepDecoders[state.Index].Decode(m_RangeDecoder) == 1)
                        {
                            if (m_IsRepG0Decoders[state.Index].Decode(m_RangeDecoder) == 0)
                            {
                                if (
                                    m_IsRep0LongDecoders[(state.Index << Base.kNumPosStatesBitsMax) + posState].Decode(
                                        m_RangeDecoder) == 0)
                                {
                                    state.UpdateShortRep();
                                    m_OutWindow.PutByte(m_OutWindow.GetByte(rep0));
                                    nowPos64++;
                                    continue;
                                }
                            }
                            else
                            {
                                UInt32 distance;
                                if (m_IsRepG1Decoders[state.Index].Decode(m_RangeDecoder) == 0)
                                {
                                    distance = rep1;
                                }
                                else
                                {
                                    if (m_IsRepG2Decoders[state.Index].Decode(m_RangeDecoder) == 0) {
	                                    distance = rep2;
                                    } else
                                    {
                                        distance = rep3;
                                        rep3 = rep2;
                                    }
                                    rep2 = rep1;
                                }
                                rep1 = rep0;
                                rep0 = distance;
                            }
                            len = m_RepLenDecoder.Decode(m_RangeDecoder, posState) + Base.kMatchMinLen;
                            state.UpdateRep();
                        }
                        else
                        {
                            rep3 = rep2;
                            rep2 = rep1;
                            rep1 = rep0;
                            len = Base.kMatchMinLen + m_LenDecoder.Decode(m_RangeDecoder, posState);
                            state.UpdateMatch();
                            uint posSlot = m_PosSlotDecoder[Base.GetLenToPosState(len)].Decode(m_RangeDecoder);
                            if (posSlot >= Base.kStartPosModelIndex)
                            {
                                var numDirectBits = (int) ((posSlot >> 1) - 1);
                                rep0 = ((2 | (posSlot & 1)) << numDirectBits);
                                if (posSlot < Base.kEndPosModelIndex) {
	                                rep0 += BitTreeDecoder.ReverseDecode(m_PosDecoders,
		                                rep0 - posSlot - 1, m_RangeDecoder,
		                                numDirectBits);
                                } else
                                {
                                    rep0 += (m_RangeDecoder.DecodeDirectBits(
                                                 numDirectBits - Base.kNumAlignBits) << Base.kNumAlignBits);
                                    rep0 += m_PosAlignDecoder.ReverseDecode(m_RangeDecoder);
                                }
                            }
                            else {
	                            rep0 = posSlot;
                            }
                        }
                        if (rep0 >= m_OutWindow.TrainSize + nowPos64 || rep0 >= m_DictionarySizeCheck)
                        {
                            if (rep0 == 0xFFFFFFFF) {
	                            break;
                            }
                            throw new DataErrorException();
                        }
                        m_OutWindow.CopyBlock(rep0, len);
                        nowPos64 += len;
                    }
                }
            }
            m_OutWindow.Flush();
            m_OutWindow.ReleaseStream();
            m_RangeDecoder.ReleaseStream();
        }

        #endregion

        #region ISetDecoderProperties Members

        /// <summary>
        /// Sets decoder properties
        /// </summary>
        /// <param name="properties">Array of byte properties</param>
        [System.ComponentModel.Description("Sets decoder properties")]
        public void SetDecoderProperties(byte[] properties)
        {
            if (properties.Length < 5) {
	            throw new InvalidParamException();
            }
            int lc = properties[0]%9;
            int remainder = properties[0]/9;
            int lp = remainder%5;
            int pb = remainder/5;
            if (pb > Base.kNumPosStatesBitsMax) {
	            throw new InvalidParamException();
            }
            UInt32 dictionarySize = 0;
            for (int i = 0; i < 4; i++)
                dictionarySize += ((UInt32) (properties[1 + i])) << (i*8);
            SetDictionarySize(dictionarySize);
            SetLiteralProperties(lp, lc);
            SetPosBitsProperties(pb);
        }

        #endregion

        /// <summary>
        /// Sets the dictionary size for the decoder, reallocating the output window<br/>
        /// if needed.
        /// </summary>
        private void SetDictionarySize(uint dictionarySize)
        {
            if (m_DictionarySize != dictionarySize)
            {
                m_DictionarySize = dictionarySize;
                m_DictionarySizeCheck = Math.Max(m_DictionarySize, 1);
                uint blockSize = Math.Max(m_DictionarySizeCheck, (1 << 12));
                m_OutWindow.Create(blockSize);
            }
        }

        /// <summary>
        /// Sets literal properties for literal symbol decoding after validation.
        /// </summary>
        private void SetLiteralProperties(int lp, int lc)
        {
            if (lp > 8) {
	            throw new InvalidParamException();
            }
            if (lc > 8) {
	            throw new InvalidParamException();
            }
            m_LiteralDecoder.Create(lp, lc);
        }

        /// <summary>
        /// Sets position bit properties, initializing length decoders for the<br/>
        /// specified position states.
        /// </summary>
        private void SetPosBitsProperties(int pb)
        {
            if (pb > Base.kNumPosStatesBitsMax) {
	            throw new InvalidParamException();
            }
            uint numPosStates = (uint) 1 << pb;
            m_LenDecoder.Create(numPosStates);
            m_RepLenDecoder.Create(numPosStates);
            m_PosStateMask = numPosStates - 1;
        }

        /// <summary>
        /// Initializes the decoder with input and output streams and resets all<br/>
        /// internal decoders to their initial state.
        /// </summary>
        private void Init(Stream inStream, Stream outStream)
        {
            m_RangeDecoder.Init(inStream);
            m_OutWindow.Init(outStream, _solid);

            uint i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (uint j = 0; j <= m_PosStateMask; j++)
                {
                    uint index = (i << Base.kNumPosStatesBitsMax) + j;
                    m_IsMatchDecoders[index].Init();
                    m_IsRep0LongDecoders[index].Init();
                }
                m_IsRepDecoders[i].Init();
                m_IsRepG0Decoders[i].Init();
                m_IsRepG1Decoders[i].Init();
                m_IsRepG2Decoders[i].Init();
            }

            m_LiteralDecoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
                m_PosSlotDecoder[i].Init();
            // m_PosSpecDecoder.Init();
            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
                m_PosDecoders[i].Init();

            m_LenDecoder.Init();
            m_RepLenDecoder.Init();
            m_PosAlignDecoder.Init();
        }

        /// <summary>
        /// Trains a stream
        /// </summary>
        /// <param name="stream">The stream to train.</param>
        /// <returns>true if Ok; otherwise, false.</returns>
        [System.ComponentModel.Description("Trains a stream")]
        public bool Train(Stream stream)
        {
            _solid = true;
            return m_OutWindow.Train(stream);
        }

        #region Nested type: LenDecoder

        /// <summary>
        /// Decodes length values for literal and match sequences in LZMA<br/>
        /// decompression.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 401 | <see cref="Decode"/> | Decodes the stream |
        ///
        /// ## Collaborators
        ///
        /// | Type | Role |
        /// |---|---|
        /// | <see cref="BitTreeDecoder"/> | Used as a field. |
        /// | <see cref="BitDecoder"/> | Used as a field. |
        /// | <see cref="Decoder"/> | Passed as a parameter. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:18:07Z", Digest = "f5929c9a3ccea2f31422980049487ef26affe7f2e533797a967d1804e7849c2d", Stale = false, Path = "sdk/Compress/LZMA/LzmaDecoder.cs", Since = "2026-08-23")]
        private class LenDecoder
        {
            private readonly BitTreeDecoder[] m_LowCoder = new BitTreeDecoder[Base.kNumPosStatesMax];
            private readonly BitTreeDecoder[] m_MidCoder = new BitTreeDecoder[Base.kNumPosStatesMax];
            private BitDecoder m_Choice;
            private BitDecoder m_Choice2;
            private BitTreeDecoder m_HighCoder = new BitTreeDecoder(Base.kNumHighLenBits);
            private uint m_NumPosStates;

            /// <summary>
            /// Creates bit tree decoders for the specified number of position states.
            /// </summary>
            internal void Create(uint numPosStates)
            {
                for (uint posState = m_NumPosStates; posState < numPosStates; posState++)
                {
                    m_LowCoder[posState] = new BitTreeDecoder(Base.kNumLowLenBits);
                    m_MidCoder[posState] = new BitTreeDecoder(Base.kNumMidLenBits);
                }
                m_NumPosStates = numPosStates;
            }

            /// <summary>
            /// Initializes all bit tree and choice decoders for length decoding.
            /// </summary>
            internal void Init()
            {
                m_Choice.Init();
                for (uint posState = 0; posState < m_NumPosStates; posState++)
                {
                    m_LowCoder[posState].Init();
                    m_MidCoder[posState].Init();
                }
                m_Choice2.Init();
                m_HighCoder.Init();
            }

            /// <summary>
            /// Decodes the stream
            /// </summary>
            /// <param name="rangeDecoder">The specified RangeCoder</param>
            /// <param name="posState">The position state</param>
            /// <returns></returns>
            [System.ComponentModel.Description("Decodes the stream")]
            public uint Decode(RangeCoder.Decoder rangeDecoder, uint posState)
            {
                if (m_Choice.Decode(rangeDecoder) == 0) {
	                return m_LowCoder[posState].Decode(rangeDecoder);
                } else
                {
                    uint symbol = Base.kNumLowLenSymbols;
                    if (m_Choice2.Decode(rangeDecoder) == 0) {
	                    symbol += m_MidCoder[posState].Decode(rangeDecoder);
                    } else
                    {
                        symbol += Base.kNumMidLenSymbols;
                        symbol += m_HighCoder.Decode(rangeDecoder);
                    }
                    return symbol;
                }
            }
        }

        #endregion

        #region Nested type: LiteralDecoder

        /// <summary>
        /// Decodes literal bytes using context-dependent Huffman-like decoders<br/>
        /// based on match history and position.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 458 | <see cref="Create"/> | Creates Decoder2 instances for all context states based on position and  previous-byte bit counts. |
        /// | 476 | <see cref="Init"/> | Initializes all context-dependent literal decoders. |
        /// | 495 | <see cref="DecodeNormal"/> | Decodes a literal byte using the normal decoder for the specified  position and previous-byte context. |
        /// | 502 | <see cref="DecodeWithMatchByte"/> | Decodes a literal byte using match-byte context for improved compression  in repetitive data. |
        ///
        /// ## Collaborators
        ///
        /// | Type | Role |
        /// |---|---|
        /// | <see cref="Decoder2"/> | Used as a field. |
        /// | <see cref="Decoder"/> | Passed as a parameter. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:18:07Z", Digest = "385ae594f02437adc6faeaf9cc57b0098214c2e671609d91e10f94306ed56958", Stale = false, Path = "sdk/Compress/LZMA/LzmaDecoder.cs", Since = "2026-08-23")]
        private class LiteralDecoder
        {
            private Decoder2[] m_Coders;
            private int m_NumPosBits;
            private int m_NumPrevBits;
            private uint m_PosMask;

            /// <summary>
            /// Creates Decoder2 instances for all context states based on position and<br/>
            /// previous-byte bit counts.
            /// </summary>
            public void Create(int numPosBits, int numPrevBits)
            {
                if (m_Coders != null && m_NumPrevBits == numPrevBits &&
                    m_NumPosBits == numPosBits) {
	                return;
                }
                m_NumPosBits = numPosBits;
                m_PosMask = ((uint) 1 << numPosBits) - 1;
                m_NumPrevBits = numPrevBits;
                uint numStates = (uint) 1 << (m_NumPrevBits + m_NumPosBits);
                m_Coders = new Decoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Create();
            }

            /// <summary>
            /// Initializes all context-dependent literal decoders.
            /// </summary>
            public void Init()
            {
                uint numStates = (uint) 1 << (m_NumPrevBits + m_NumPosBits);
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Init();
            }

			/// <summary>
			/// Returns the decoder state index for the specified position and previous<br/>
			/// byte context.
			/// </summary>
			/// <returns>The decoder state index.</returns>
			private uint GetState(uint pos, byte prevByte) => ((pos & m_PosMask) << m_NumPrevBits) + (uint) (prevByte >> (8 - m_NumPrevBits));

			/// <summary>
			/// Decodes a literal byte using the normal decoder for the specified<br/>
			/// position and previous-byte context.
			/// </summary>
			/// <returns>The decoded literal byte.</returns>
			public byte DecodeNormal(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte) => m_Coders[GetState(pos, prevByte)].DecodeNormal(rangeDecoder);

			/// <summary>
			/// Decodes a literal byte using match-byte context for improved compression<br/>
			/// in repetitive data.
			/// </summary>
			/// <returns>The decoded literal byte.</returns>
			public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte) => m_Coders[GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);

			#region Nested type: Decoder2

			/// <summary>
			/// Decodes a single literal byte using binary tree arithmetic decoding.
			/// </summary>
			/// <remarks>
			/// ## Public Methods
			///
			/// | Line | Method | Description |
			/// |--:|---|---|
			/// | 534 | <see cref="Create"/> | Allocates the bit decoder array for this decoder instance. |
			/// | 539 | <see cref="Init"/> | Initializes all bit decoders in the decoder array. |
			/// | 548 | <see cref="DecodeNormal"/> | Decodes a byte from the range decoder using a binary tree algorithm. |
			/// | 561 | <see cref="DecodeWithMatchByte"/> | Decodes a byte using match-byte-guided binary tree decoding for faster  convergence. |
			///
			/// ## Collaborators
			///
			/// | Type | Role |
			/// |---|---|
			/// | <see cref="BitDecoder"/> | Used as a field. |
			/// | <see cref="Decoder"/> | Passed as a parameter. |
			/// </remarks>
			[DocState(Pass = 2, MTime = "2026-08-24T14:18:07Z", Digest = "8bce2f7c1986f69dda8433b87b954f3373bbe647f9abf87851cc7bdbcf43f5fe", Stale = false, Path = "sdk/Compress/LZMA/LzmaDecoder.cs", Since = "2026-08-23")]
			private struct Decoder2
            {
                private BitDecoder[] m_Decoders;

				/// <summary>
				/// Allocates the bit decoder array for this decoder instance.
				/// </summary>
				public void Create() => m_Decoders = new BitDecoder[0x300];

				/// <summary>
				/// Initializes all bit decoders in the decoder array.
				/// </summary>
				public void Init()
                {
                    for (int i = 0; i < 0x300; i++) m_Decoders[i].Init();
                }

                /// <summary>
                /// Decodes a byte from the range decoder using a binary tree algorithm.
                /// </summary>
                /// <returns>The decoded byte.</returns>
                public byte DecodeNormal(RangeCoder.Decoder rangeDecoder)
                {
                    uint symbol = 1;
                    do
                        symbol = (symbol << 1) | m_Decoders[symbol].Decode(rangeDecoder); while (symbol < 0x100);
                    return (byte) symbol;
                }

                /// <summary>
                /// Decodes a byte using match-byte-guided binary tree decoding for faster<br/>
                /// convergence.
                /// </summary>
                /// <returns>The decoded byte.</returns>
                public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, byte matchByte)
                {
                    uint symbol = 1;
                    do
                    {
                        uint matchBit = (uint) (matchByte >> 7) & 1;
                        matchByte <<= 1;
                        uint bit = m_Decoders[((1 + matchBit) << 8) + symbol].Decode(rangeDecoder);
                        symbol = (symbol << 1) | bit;
                        if (matchBit != bit)
                        {
                            while (symbol < 0x100)
                                symbol = (symbol << 1) | m_Decoders[symbol].Decode(rangeDecoder);
                            break;
                        }
                    } while (symbol < 0x100);
                    return (byte) symbol;
                }
            }

            #endregion
        } ;

        #endregion

        /*
		public override bool CanRead { get { return true; }}
		public override bool CanWrite { get { return true; }}
		public override bool CanSeek { get { return true; }}
		public override long Length { get { return 0; }}
		public override long Position
		{
			get { return 0;	}
			set { }
		}
		public override void Flush() { }
		public override int Read(byte[] buffer, int offset, int count) 
		{
			return 0;
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
		}
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			return 0;
		}
		public override void SetLength(long value) {}
		*/
    }
}