namespace SevenZip.Sdk.Compression.Lzma
{

    /// <summary>Shared LZMA constants and the literal/match coder state machine <br/>
    /// used by both <see cref="Encoder"/> and <see cref="Decoder"/>.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 10 | <see cref="kAlignMask"/> | Specifies the constant k Align Mask. |
    /// | 13 | <see cref="kAlignTableSize"/> | Specifies the constant k Align Table Size. |
    /// | 16 | <see cref="kDicLogSizeMin"/> | Specifies the constant k Dic Log Size Min. |
    /// | 19 | <see cref="kEndPosModelIndex"/> | Specifies the constant k End Pos Model Index. |
    /// | 22 | <see cref="kMatchMaxLen"/> | Specifies the constant k Match Max Len. |
    /// | 27 | <see cref="kMatchMinLen"/> | Specifies the constant k Match Min Len. |
    /// | 30 | <see cref="kNumAlignBits"/> | Specifies the constant k Num Align Bits. |
    /// | 33 | <see cref="kNumFullDistances"/> | Specifies the constant k Num Full Distances. |
    /// | 36 | <see cref="kNumHighLenBits"/> | Specifies the constant k Num High Len Bits. |
    /// | 39 | <see cref="kNumLenSymbols"/> | Specifies the constant k Num Len Symbols. |
    /// | 43 | <see cref="kNumLenToPosStates"/> | Specifies the constant k Num Len To Pos States. |
    /// | 46 | <see cref="kNumLenToPosStatesBits"/> | Specifies the constant k Num Len To Pos States Bits. |
    /// | 49 | <see cref="kNumLitContextBitsMax"/> | Specifies the constant k Num Lit Context Bits Max. |
    /// | 52 | <see cref="kNumLitPosStatesBitsEncodingMax"/> | Specifies the constant k Num Lit Pos States Bits Encoding Max. |
    /// | 55 | <see cref="kNumLowLenBits"/> | Specifies the constant k Num Low Len Bits. |
    /// | 58 | <see cref="kNumLowLenSymbols"/> | Specifies the constant k Num Low Len Symbols. |
    /// | 61 | <see cref="kNumMidLenBits"/> | Specifies the constant k Num Mid Len Bits. |
    /// | 64 | <see cref="kNumMidLenSymbols"/> | Specifies the constant k Num Mid Len Symbols. |
    /// | 67 | <see cref="kNumPosModels"/> | Specifies the constant k Num Pos Models. |
    /// | 70 | <see cref="kNumPosSlotBits"/> | Specifies the constant k Num Pos Slot Bits. |
    /// | 73 | <see cref="kNumPosStatesBitsEncodingMax"/> | Specifies the constant k Num Pos States Bits Encoding Max. |
    /// | 76 | <see cref="kNumPosStatesBitsMax"/> | Specifies the constant k Num Pos States Bits Max. |
    /// | 79 | <see cref="kNumPosStatesEncodingMax"/> | Specifies the constant k Num Pos States Encoding Max. |
    /// | 82 | <see cref="kNumPosStatesMax"/> | Specifies the constant k Num Pos States Max. |
    /// | 85 | <see cref="kNumRepDistances"/> | Specifies the constant k Num Rep Distances. |
    /// | 88 | <see cref="kNumStates"/> | Specifies the constant k Num States. |
    /// | 91 | <see cref="kStartPosModelIndex"/> | Specifies the constant k Start Pos Model Index. |
    /// | 96 | <see cref="GetLenToPosState"/> | Maps a match   to the length-to-distance-state   index used to select the distance-slot probability model. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="State"/> | Nested type. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 4c7e2d603cf58dea7207ff58ef9f9bce5284c25c04d4e2c176c01146e998d2e0
    /// </code>
    /// </example>
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

        /// <summary>Maps a match <paramref name='len'/> to the length-to-distance-state <br/>
        /// index used to select the distance-slot probability model.</summary>
        /// <returns>An index in the range [0, <see cref="kNumLenToPosStates"/>).</returns>
        public static uint GetLenToPosState(uint len)
        {
            len -= kMatchMinLen;
            if (len < kNumLenToPosStates) {
	            return len;
            }
            return (kNumLenToPosStates - 1);
        }

        #region Nested type: State

        /// <summary>Tracks the current position in the 12-state LZMA state machine <br/>
        /// that models the recent history of literals, matches, and repeats.</summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 114 | <see cref="Init"/> | Resets the state to its initial value, as at the start of a stream. |
        /// | 117 | <see cref="UpdateChar"/> | Advances the state after a literal byte was coded. |
        /// | 129 | <see cref="UpdateMatch"/> | Advances the state after a full (distance + length) match was coded. |
        /// | 132 | <see cref="UpdateRep"/> | Advances the state after a repeated-distance match was coded. |
        /// | 135 | <see cref="UpdateShortRep"/> | Advances the state after a single-byte short-repeat match was coded. |
        /// | 138 | <see cref="IsCharState"/> | Determines whether char State. |
        /// </remarks>
        /// <example>
        /// <code language="yaml">
        /// pass: 2
        /// mtime: 2026-08-06T06:59:29Z
        /// digest: 11f418b4718f1ee318e93d3c4c090bcd3d4545fab6c89db64b2bb710291d02ef
        /// </code>
        /// </example>
        public struct State
        {
            public uint Index;

			/// <summary>Resets the state to its initial value, as at the start of a stream.</summary>
			public void Init() => Index = 0;

			/// <summary>Advances the state after a literal byte was coded.</summary>
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

			/// <summary>Advances the state after a full (distance + length) match was coded.</summary>
			public void UpdateMatch() => Index = (uint) (Index < 7 ? 7 : 10);

			/// <summary>Advances the state after a repeated-distance match was coded.</summary>
			public void UpdateRep() => Index = (uint) (Index < 7 ? 8 : 11);

			/// <summary>Advances the state after a single-byte short-repeat match was coded.</summary>
			public void UpdateShortRep() => Index = (uint) (Index < 7 ? 9 : 11);

			/// <summary>Determines whether char State.</summary>
			public bool IsCharState() => Index < 7;
		}

        #endregion
    }
}