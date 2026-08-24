using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.Lzma
{

    /// <summary>Provides LZMA compression algorithm constants and state management for encoder/decoder <br/>
    /// implementations.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 12 | <see cref="kAlignMask"/> | Specifies the constant k Align Mask. |
    /// | 15 | <see cref="kAlignTableSize"/> | Specifies the constant k Align Table Size. |
    /// | 18 | <see cref="kDicLogSizeMin"/> | Specifies the constant k Dic Log Size Min. |
    /// | 21 | <see cref="kEndPosModelIndex"/> | Specifies the constant k End Pos Model Index. |
    /// | 24 | <see cref="kMatchMaxLen"/> | Specifies the constant k Match Max Len. |
    /// | 29 | <see cref="kMatchMinLen"/> | Specifies the constant k Match Min Len. |
    /// | 32 | <see cref="kNumAlignBits"/> | Specifies the constant k Num Align Bits. |
    /// | 35 | <see cref="kNumFullDistances"/> | Specifies the constant k Num Full Distances. |
    /// | 38 | <see cref="kNumHighLenBits"/> | Specifies the constant k Num High Len Bits. |
    /// | 41 | <see cref="kNumLenSymbols"/> | Specifies the constant k Num Len Symbols. |
    /// | 45 | <see cref="kNumLenToPosStates"/> | Specifies the constant k Num Len To Pos States. |
    /// | 48 | <see cref="kNumLenToPosStatesBits"/> | Specifies the constant k Num Len To Pos States Bits. |
    /// | 51 | <see cref="kNumLitContextBitsMax"/> | Specifies the constant k Num Lit Context Bits Max. |
    /// | 54 | <see cref="kNumLitPosStatesBitsEncodingMax"/> | Specifies the constant k Num Lit Pos States Bits Encoding Max. |
    /// | 57 | <see cref="kNumLowLenBits"/> | Specifies the constant k Num Low Len Bits. |
    /// | 60 | <see cref="kNumLowLenSymbols"/> | Specifies the constant k Num Low Len Symbols. |
    /// | 63 | <see cref="kNumMidLenBits"/> | Specifies the constant k Num Mid Len Bits. |
    /// | 66 | <see cref="kNumMidLenSymbols"/> | Specifies the constant k Num Mid Len Symbols. |
    /// | 69 | <see cref="kNumPosModels"/> | Specifies the constant k Num Pos Models. |
    /// | 72 | <see cref="kNumPosSlotBits"/> | Specifies the constant k Num Pos Slot Bits. |
    /// | 75 | <see cref="kNumPosStatesBitsEncodingMax"/> | Specifies the constant k Num Pos States Bits Encoding Max. |
    /// | 78 | <see cref="kNumPosStatesBitsMax"/> | Specifies the constant k Num Pos States Bits Max. |
    /// | 81 | <see cref="kNumPosStatesEncodingMax"/> | Specifies the constant k Num Pos States Encoding Max. |
    /// | 84 | <see cref="kNumPosStatesMax"/> | Specifies the constant k Num Pos States Max. |
    /// | 87 | <see cref="kNumRepDistances"/> | Specifies the constant k Num Rep Distances. |
    /// | 90 | <see cref="kNumStates"/> | Specifies the constant k Num States. |
    /// | 93 | <see cref="kStartPosModelIndex"/> | Specifies the constant k Start Pos Model Index. |
    /// | 98 | <see cref="GetLenToPosState"/> | Computes the position state for the given match length, clamped to   the maximum valid state. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="State"/> | Nested type. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T13:58:58Z", Digest = "6c890c503f6c0ceda0888617f2317cf0a075bdb020e505a03898c812769c46bb", Stale = false, Path = "sdk/Compress/LZMA/LzmaBase.cs", Since = "2026-08-23")]
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

        /// <summary>Computes the position state for the given match length, clamped to <br/>
        /// the maximum valid state.</summary>
        /// <returns>The computed position state value.</returns>
        public static uint GetLenToPosState(uint len)
        {
            len -= kMatchMinLen;
            if (len < kNumLenToPosStates) {
	            return len;
            }
            return (kNumLenToPosStates - 1);
        }

        #region Nested type: State

        /// <summary>Encapsulates the current LZMA decoder state and provides transitions <br/>
        /// for character matches, string matches, and repeats.</summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 117 | <see cref="Init"/> | Initializes the state index to zero. |
        /// | 120 | <see cref="UpdateChar"/> | Transitions the state after matching a literal character. |
        /// | 132 | <see cref="UpdateMatch"/> | Transitions the state after matching a string sequence. |
        /// | 135 | <see cref="UpdateRep"/> | Transitions the state after repeating a previously matched sequence. |
        /// | 138 | <see cref="UpdateShortRep"/> | Transitions the state after a short repeat match. |
        /// | 141 | <see cref="IsCharState"/> | Determines whether char State. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T13:58:58Z", Digest = "59737f0501ed960b4514767c70dcc0e5871dd31ae9141ea42d67237cf1cfcb88", Stale = false, Path = "sdk/Compress/LZMA/LzmaBase.cs", Since = "2026-08-23")]
        public struct State
        {
            public uint Index;

			/// <summary>Initializes the state index to zero.</summary>
			public void Init() => Index = 0;

			/// <summary>Transitions the state after matching a literal character.</summary>
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

			/// <summary>Transitions the state after matching a string sequence.</summary>
			public void UpdateMatch() => Index = (uint) (Index < 7 ? 7 : 10);

			/// <summary>Transitions the state after repeating a previously matched sequence.</summary>
			public void UpdateRep() => Index = (uint) (Index < 7 ? 8 : 11);

			/// <summary>Transitions the state after a short repeat match.</summary>
			public void UpdateShortRep() => Index = (uint) (Index < 7 ? 9 : 11);

			/// <summary>Determines whether char State.</summary>
			public bool IsCharState() => Index < 7;
		}

        #endregion
    }
}