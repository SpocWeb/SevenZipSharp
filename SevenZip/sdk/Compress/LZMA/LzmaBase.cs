using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.Lzma
{

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "5bb66bf1b95de0e1d69355b248d7fb842ef8d40203e1d894d9381e32942d9ab4", Stale = true, Path = "sdk/Compress/LZMA/LzmaBase.cs", Since = "2026-08-23")]
    internal abstract class Base
    {

        /// <summary>Specifies the constant k Align Mask.</summary>
        public const uint kAlignMask = (kAlignTableSize - 1);

        /// <summary>Specifies the constant k Align Table Size.</summary>
        public const uint kAlignTableSize = 1 << kNumAlignBits;

        /// <summary>Specifies the constant k Dic Log Size Min.</summary>
        public const int kDicLogSizeMin = 0;

        /// <summary>Specifies the constant k End Pos Model Index.</summary>
        public const uint kEndPosModelIndex = 14;

        /// <summary>Specifies the constant k Match Max Len.</summary>
        public const uint kMatchMaxLen = kMatchMinLen + kNumLenSymbols - 1;
        // public const int kDicLogSizeMax = 30;
        // public const uint kDistTableSizeMax = kDicLogSizeMax * 2;

        /// <summary>Specifies the constant k Match Min Len.</summary>
        public const uint kMatchMinLen = 2;

        /// <summary>Specifies the constant k Num Align Bits.</summary>
        public const int kNumAlignBits = 4;

        /// <summary>Specifies the constant k Num Full Distances.</summary>
        public const uint kNumFullDistances = 1 << ((int) kEndPosModelIndex/2);

        /// <summary>Specifies the constant k Num High Len Bits.</summary>
        public const int kNumHighLenBits = 8;

        /// <summary>Specifies the constant k Num Len Symbols.</summary>
        public const uint kNumLenSymbols = kNumLowLenSymbols + kNumMidLenSymbols +
                                           (1 << kNumHighLenBits);

        /// <summary>Specifies the constant k Num Len To Pos States.</summary>
        public const uint kNumLenToPosStates = 1 << kNumLenToPosStatesBits;

        /// <summary>Specifies the constant k Num Len To Pos States Bits.</summary>
        public const int kNumLenToPosStatesBits = 2; // it's for speed optimization

        /// <summary>Specifies the constant k Num Lit Context Bits Max.</summary>
        public const uint kNumLitContextBitsMax = 8;

        /// <summary>Specifies the constant k Num Lit Pos States Bits Encoding Max.</summary>
        public const uint kNumLitPosStatesBitsEncodingMax = 4;

        /// <summary>Specifies the constant k Num Low Len Bits.</summary>
        public const int kNumLowLenBits = 3;

        /// <summary>Specifies the constant k Num Low Len Symbols.</summary>
        public const uint kNumLowLenSymbols = 1 << kNumLowLenBits;

        /// <summary>Specifies the constant k Num Mid Len Bits.</summary>
        public const int kNumMidLenBits = 3;

        /// <summary>Specifies the constant k Num Mid Len Symbols.</summary>
        public const uint kNumMidLenSymbols = 1 << kNumMidLenBits;

        /// <summary>Specifies the constant k Num Pos Models.</summary>
        public const uint kNumPosModels = kEndPosModelIndex - kStartPosModelIndex;

        /// <summary>Specifies the constant k Num Pos Slot Bits.</summary>
        public const int kNumPosSlotBits = 6;

        /// <summary>Specifies the constant k Num Pos States Bits Encoding Max.</summary>
        public const int kNumPosStatesBitsEncodingMax = 4;

        /// <summary>Specifies the constant k Num Pos States Bits Max.</summary>
        public const int kNumPosStatesBitsMax = 4;

        /// <summary>Specifies the constant k Num Pos States Encoding Max.</summary>
        public const uint kNumPosStatesEncodingMax = (1 << kNumPosStatesBitsEncodingMax);

        /// <summary>Specifies the constant k Num Pos States Max.</summary>
        public const uint kNumPosStatesMax = (1 << kNumPosStatesBitsMax);

        /// <summary>Specifies the constant k Num Rep Distances.</summary>
        public const uint kNumRepDistances = 4;

        /// <summary>Specifies the constant k Num States.</summary>
        public const uint kNumStates = 12;

        /// <summary>Specifies the constant k Start Pos Model Index.</summary>
        public const uint kStartPosModelIndex = 4;

        /// <summary>TODO: LLM</summary>
        public static uint GetLenToPosState(uint len)
        {
            len -= kMatchMinLen;
            if (len < kNumLenToPosStates) {
	            return len;
            }
            return (kNumLenToPosStates - 1);
        }

        #region Nested type: State

        /// <summary>TODO: LLM</summary>
        [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "80265759dbbbbb251519022894e471cc3fd03a714f010e58946430d76d46f450", Stale = true, Path = "sdk/Compress/LZMA/LzmaBase.cs", Since = "2026-08-23")]
        public struct State
        {
            public uint Index;

			/// <summary>TODO: LLM</summary>
			public void Init() => Index = 0;

			/// <summary>TODO: LLM</summary>
			public void UpdateChar()
            {
                if (Index < 4) {
	                Index = 0;
                } else if (Index < 10) {
	                Index -= 3;
                } else {
	                Index -= 6;
                }
            }

			/// <summary>TODO: LLM</summary>
			public void UpdateMatch() => Index = (uint) (Index < 7 ? 7 : 10);

			/// <summary>TODO: LLM</summary>
			public void UpdateRep() => Index = (uint) (Index < 7 ? 8 : 11);

			/// <summary>TODO: LLM</summary>
			public void UpdateShortRep() => Index = (uint) (Index < 7 ? 9 : 11);

			/// <summary>Determines whether char State.</summary>
			public bool IsCharState() => Index < 7;
		}

        #endregion
    }
}