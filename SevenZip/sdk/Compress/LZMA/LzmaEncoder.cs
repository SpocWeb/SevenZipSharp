using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.Lzma
{
    using System;
    using System.Globalization;
    using System.IO;

    using SevenZip.Sdk.Compression.LZ;
    using SevenZip.Sdk.Compression.RangeCoder;

    /// <summary>
    /// The LZMA encoder class
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 150 | <see cref="Encoder"/> | Initializes a new instance of the Encoder class |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt32"/> | Used as a field. |
    /// | <see cref="Byte"/> | Used as a field. |
    /// | <see cref="BitEncoder"/> | Used as a field. |
    /// | <see cref="LenPriceTableEncoder"/> | Used as a field. |
    /// | <see cref="LiteralEncoder"/> | Used as a field. |
    /// | <see cref="Optimal"/> | Used as a field. |
    /// | <see cref="BitTreeEncoder"/> | Used as a field. |
    /// | <see cref="Encoder"/> | Used as a field. |
    /// | <see cref="IMatchFinder"/> | Used as a field. |
    /// | <see cref="EMatchFinderType"/> | Used as a field. |
    /// | <see cref="State"/> | Used as a field. |
    /// | <see cref="ICodeProgress"/> | Passed as a parameter. |
    /// | <see cref="CoderPropId"/> | Passed as a parameter. |
    /// | <see cref="LenEncoder"/> | Nested type. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:20:41Z", Digest = "b14608aa30bfb7ec09c323e12c00431012f7cde990cd08413ea904adab4145e4", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
    public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {

        /// <summary>Specifies the constant k Default Dictionary Log Size.</summary>
        private const int kDefaultDictionaryLogSize = 22;

        /// <summary>Specifies the constant k Ifinity Price.</summary>
        private const UInt32 kIfinityPrice = 0xFFFFFFF;

        /// <summary>Specifies the constant k Num Fast Bytes Default.</summary>
        private const UInt32 kNumFastBytesDefault = 0x20;

        /// <summary>Specifies the constant k Num Len Spec Symbols.</summary>
        private const UInt32 kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

        /// <summary>Specifies the constant k Num Opts.</summary>
        private const UInt32 kNumOpts = 1 << 12;

        /// <summary>Specifies the constant k Prop Size.</summary>
        private const int kPropSize = 5;

        /// <summary>Gets the g Fast Pos.</summary>
        private static readonly Byte[] g_FastPos = new Byte[1 << 11];

        /// <summary>Gets the k Match Finder I Ds.</summary>
        private static readonly string[] kMatchFinderIDs =
            {
                "BT2",
                "BT4",
            };

        private readonly UInt32[] _alignPrices = new UInt32[Base.kAlignTableSize];
        private readonly UInt32[] _distancesPrices = new UInt32[Base.kNumFullDistances << Base.kNumLenToPosStatesBits];

        private readonly BitEncoder[] _isMatch = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
        private readonly BitEncoder[] _isRep = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRep0Long = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
        private readonly BitEncoder[] _isRepG0 = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRepG1 = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRepG2 = new BitEncoder[Base.kNumStates];

        private readonly LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();

        private readonly LiteralEncoder _literalEncoder = new LiteralEncoder();

        private readonly UInt32[] _matchDistances = new UInt32[Base.kMatchMaxLen*2 + 2];
        private readonly Optimal[] _optimum = new Optimal[kNumOpts];
        private readonly BitEncoder[] _posEncoders = new BitEncoder[Base.kNumFullDistances - Base.kEndPosModelIndex];
        private readonly BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[Base.kNumLenToPosStates];

        private readonly UInt32[] _posSlotPrices = new UInt32[1 << (Base.kNumPosSlotBits + Base.kNumLenToPosStatesBits)];
        private readonly RangeCoder.Encoder _rangeEncoder = new RangeCoder.Encoder();
        private readonly UInt32[] _repDistances = new UInt32[Base.kNumRepDistances];
        private readonly LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();
        private readonly Byte[] properties = new Byte[kPropSize];
        private readonly UInt32[] repLens = new UInt32[Base.kNumRepDistances];
        private readonly UInt32[] reps = new UInt32[Base.kNumRepDistances];
        private readonly UInt32[] tempPrices = new UInt32[Base.kNumFullDistances];
        private UInt32 _additionalOffset;
        private UInt32 _alignPriceCount;

        private UInt32 _dictionarySize = (1 << kDefaultDictionaryLogSize);
        private UInt32 _dictionarySizePrev = 0xFFFFFFFF;
        private UInt32 _distTableSize = (kDefaultDictionaryLogSize*2);
        private bool _finished;
        private Stream _inStream;
        private UInt32 _longestMatchLength;
        private bool _longestMatchWasFound;
        private IMatchFinder _matchFinder;

        private EMatchFinderType _matchFinderType = EMatchFinderType.BT4;
        private UInt32 _matchPriceCount;

        private bool _needReleaseMFStream;
        private UInt32 _numDistancePairs;
        private UInt32 _numFastBytes = kNumFastBytesDefault;
        private UInt32 _numFastBytesPrev = 0xFFFFFFFF;
        private int _numLiteralContextBits = 3;
        private int _numLiteralPosStateBits;
        private UInt32 _optimumCurrentIndex;
        private UInt32 _optimumEndIndex;
        private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(Base.kNumAlignBits);
        private int _posStateBits = 2;
        private UInt32 _posStateMask = (4 - 1);
        private Byte _previousByte;
        private Base.State _state;
        private uint _trainSize;
        private bool _writeEndMark;
        private Int64 nowPos64;

        /// <summary>Initializes a new instance of <see cref="Encoder"/>.</summary>
        static Encoder()
        {
            const Byte kFastSlots = 22;
            int c = 2;
            g_FastPos[0] = 0;
            g_FastPos[1] = 1;
            for (Byte slotFast = 2; slotFast < kFastSlots; slotFast++)
            {
                UInt32 k = ((UInt32) 1 << ((slotFast >> 1) - 1));
                for (UInt32 j = 0; j < k; j++, c++)
                    g_FastPos[c] = slotFast;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Encoder class
        /// </summary>
        [System.ComponentModel.Description("Initializes a new instance of the Encoder class")]
        public Encoder()
        {
            for (int i = 0; i < kNumOpts; i++)
                _optimum[i] = new Optimal();
            for (int i = 0; i < Base.kNumLenToPosStates; i++)
                _posSlotEncoder[i] = new BitTreeEncoder(Base.kNumPosSlotBits);
        }

        #region ICoder Members

        /// <summary>
        /// Codes the specified stream
        /// </summary>
        /// <param name="inStream">The input stream</param>
        /// <param name="inSize">The input size</param>
        /// <param name="outSize">The output size</param>
        /// <param name="outStream">The output stream</param>
        /// <param name="progress">The progress callback</param>
        [System.ComponentModel.Description("Codes the specified stream")]
        public void Code(Stream inStream, Stream outStream,
                         Int64 inSize, Int64 outSize, ICodeProgress progress)
        {
            _needReleaseMFStream = false;
            try
            {
                SetStreams(inStream, outStream /*, inSize, outSize*/);
                while (true)
                {
                    Int64 processedInSize;
                    Int64 processedOutSize;
                    bool finished;
                    CodeOneBlock(out processedInSize, out processedOutSize, out finished);
                    if (finished) {
	                    return;
                    }
                    if (progress != null)
                    {
                        progress.SetProgress(processedInSize, processedOutSize);
                    }
                }
            }
            finally
            {
                ReleaseStreams();
            }
        }

        #endregion

        #region ISetCoderProperties Members

        /// <summary>
        /// Sets the coder properties
        /// </summary>
        /// <param name="propIDs">The property identificators</param>
        /// <param name="properties">The array of properties</param>
        [System.ComponentModel.Description("Sets the coder properties")]
        public void SetCoderProperties(CoderPropId[] propIDs, object[] properties)
        {
            for (UInt32 i = 0; i < properties.Length; i++)
            {
                object prop = properties[i];
                switch (propIDs[i])
                {
                    case CoderPropId.NumFastBytes:
                    {
                        if (!(prop is Int32)) {
	                        throw new InvalidParamException();
                        }
                        var numFastBytes = (Int32) prop;
                        if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen) {
	                        throw new InvalidParamException();
                        }
                        _numFastBytes = (UInt32) numFastBytes;
                        break;
                    }
                    case CoderPropId.Algorithm:
                    {
                        /*
						if (!(prop is Int32))
							throw new InvalidParamException();
						Int32 maximize = (Int32)prop;
						_fastMode = (maximize == 0);
						_maxMode = (maximize >= 2);
						*/
                        break;
                    }
                    case CoderPropId.MatchFinder:
                    {
                        if (!(prop is String)) {
	                        throw new InvalidParamException();
                        }
                        EMatchFinderType matchFinderIndexPrev = _matchFinderType;
                        int m = FindMatchFinder(((string) prop).ToUpper(CultureInfo.CurrentCulture));
                        if (m < 0) {
	                        throw new InvalidParamException();
                        }
                        _matchFinderType = (EMatchFinderType) m;
                        if (_matchFinder != null && matchFinderIndexPrev != _matchFinderType)
                        {
                            _dictionarySizePrev = 0xFFFFFFFF;
                            _matchFinder = null;
                        }
                        break;
                    }
                    case CoderPropId.DictionarySize:
                    {
                        const int kDicLogSizeMaxCompress = 30;
                        if (!(prop is Int32)) {
	                        throw new InvalidParamException();
                        }
                        ;
                        var dictionarySize = (Int32) prop;
                        if (dictionarySize < (UInt32) (1 << Base.kDicLogSizeMin) ||
                            dictionarySize > (UInt32) (1 << kDicLogSizeMaxCompress)) {
	                        throw new InvalidParamException();
                        }
                        _dictionarySize = (UInt32) dictionarySize;
                        int dicLogSize;
                        for (dicLogSize = 0; dicLogSize < (UInt32) kDicLogSizeMaxCompress; dicLogSize++)
                            if (dictionarySize <= ((UInt32) (1) << dicLogSize)) {
	                            break;
                            }
                        _distTableSize = (UInt32) dicLogSize*2;
                        break;
                    }
                    case CoderPropId.PosStateBits:
                    {
                        if (!(prop is Int32)) {
	                        throw new InvalidParamException();
                        }
                        var v = (Int32) prop;
                        if (v < 0 || v > (UInt32) Base.kNumPosStatesBitsEncodingMax) {
	                        throw new InvalidParamException();
                        }
                        _posStateBits = v;
                        _posStateMask = (((UInt32) 1) << _posStateBits) - 1;
                        break;
                    }
                    case CoderPropId.LitPosBits:
                    {
                        if (!(prop is Int32)) {
	                        throw new InvalidParamException();
                        }
                        var v = (Int32) prop;
                        if (v < 0 || v > Base.kNumLitPosStatesBitsEncodingMax) {
	                        throw new InvalidParamException();
                        }
                        _numLiteralPosStateBits = v;
                        break;
                    }
                    case CoderPropId.LitContextBits:
                    {
                        if (!(prop is Int32)) {
	                        throw new InvalidParamException();
                        }
                        var v = (Int32) prop;
                        if (v < 0 || v > Base.kNumLitContextBitsMax) {
	                        throw new InvalidParamException();
                        }
                        ;
                        _numLiteralContextBits = v;
                        break;
                    }
                    case CoderPropId.EndMarker:
                    {
                        if (!(prop is Boolean)) {
	                        throw new InvalidParamException();
                        }
                        SetWriteEndMarkerMode((Boolean) prop);
                        break;
                    }
                    default:
                        throw new InvalidParamException();
                }
            }
        }

        #endregion

        #region IWriteCoderProperties Members

        /// <summary>
        /// Writes the coder properties
        /// </summary>
        /// <param name="outStream">The output stream to write the properties to.</param>
        [System.ComponentModel.Description("Writes the coder properties")]
        public void WriteCoderProperties(Stream outStream)
        {
            properties[0] = (Byte) ((_posStateBits*5 + _numLiteralPosStateBits)*9 + _numLiteralContextBits);
            for (int i = 0; i < 4; i++)
                properties[1 + i] = (Byte) (_dictionarySize >> (8*i));
            outStream.Write(properties, 0, kPropSize);
        }

        #endregion

        /// <summary>
        /// Retrieves the position slot for a given position value<br/>
        /// using a fast lookup table with logarithmic scaling.
        /// </summary>
        /// <returns>The position slot corresponding to <paramref name="pos"/>.</returns>
        private static UInt32 GetPosSlot(UInt32 pos)
        {
            if (pos < (1 << 11)) {
	            return g_FastPos[pos];
            }
            if (pos < (1 << 21)) {
	            return (UInt32) (g_FastPos[pos >> 10] + 20);
            }
            return (UInt32) (g_FastPos[pos >> 20] + 40);
        }

        /// <summary>
        /// Retrieves the position slot for a distance using<br/>
        /// high-range position-slot lookup with coarser granularity.
        /// </summary>
        /// <returns>The position slot for large distances.</returns>
        private static UInt32 GetPosSlot2(UInt32 pos)
        {
            if (pos < (1 << 17)) {
	            return (UInt32) (g_FastPos[pos >> 6] + 12);
            }
            if (pos < (1 << 27)) {
	            return (UInt32) (g_FastPos[pos >> 16] + 32);
            }
            return (UInt32) (g_FastPos[pos >> 26] + 52);
        }

        /// <summary>
        /// Initializes the encoder state, previous byte, and<br/>
        /// repetition distances to their default values.
        /// </summary>
        private void BaseInit()
        {
            _state.Init();
            _previousByte = 0;
            for (UInt32 i = 0; i < Base.kNumRepDistances; i++)
                _repDistances[i] = 0;
        }

        /// <summary>
        /// Initializes the match finder and literal encoder<br/>
        /// with the configured dictionary size and fast bytes.
        /// </summary>
        private void Create()
        {
            if (_matchFinder == null)
            {
                var bt = new BinTree();
                int numHashBytes = 4;
                if (_matchFinderType == EMatchFinderType.BT2) {
	                numHashBytes = 2;
                }
                bt.SetType(numHashBytes);
                _matchFinder = bt;
            }
            _literalEncoder.Create(_numLiteralPosStateBits, _numLiteralContextBits);

            if (_dictionarySize == _dictionarySizePrev && _numFastBytesPrev == _numFastBytes) {
	            return;
            }
            _matchFinder.Create(_dictionarySize, kNumOpts, _numFastBytes, Base.kMatchMaxLen + 1);
            _dictionarySizePrev = _dictionarySize;
            _numFastBytesPrev = _numFastBytes;
        }

		/// <summary>Sets whether to write an end-of-stream marker.</summary>
		private void SetWriteEndMarkerMode(bool writeEndMarker) => _writeEndMark = writeEndMarker;

		/// <summary>
		/// Initializes all range and bit encoders, position models,<br/>
		/// and optimization state for a new encoding sequence.
		/// </summary>
		private void Init()
        {
            BaseInit();
            _rangeEncoder.Init();

            uint i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (uint j = 0; j <= _posStateMask; j++)
                {
                    uint complexState = (i << Base.kNumPosStatesBitsMax) + j;
                    _isMatch[complexState].Init();
                    _isRep0Long[complexState].Init();
                }
                _isRep[i].Init();
                _isRepG0[i].Init();
                _isRepG1[i].Init();
                _isRepG2[i].Init();
            }
            _literalEncoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
                _posSlotEncoder[i].Init();
            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
                _posEncoders[i].Init();

            _lenEncoder.Init((UInt32) 1 << _posStateBits);
            _repMatchLenEncoder.Init((UInt32) 1 << _posStateBits);

            _posAlignEncoder.Init();

            _longestMatchWasFound = false;
            _optimumEndIndex = 0;
            _optimumCurrentIndex = 0;
            _additionalOffset = 0;
        }

        /// <summary>
        /// Retrieves the longest match and all match distances<br/>
        /// at the current position from the match finder.
        /// </summary>
        private void ReadMatchDistances(out UInt32 lenRes, out UInt32 numDistancePairs)
        {
            lenRes = 0;
            numDistancePairs = _matchFinder.GetMatches(_matchDistances);
            if (numDistancePairs > 0)
            {
                lenRes = _matchDistances[numDistancePairs - 2];
                if (lenRes == _numFastBytes) {
	                lenRes += _matchFinder.GetMatchLen((int) lenRes - 1, _matchDistances[numDistancePairs - 1],
		                Base.kMatchMaxLen - lenRes);
                }
            }
            _additionalOffset++;
        }


        /// <summary>
        /// Advances the match finder position by the<br/>
        /// specified number of bytes.
        /// </summary>
        private void MovePos(UInt32 num)
        {
            if (num > 0)
            {
                _matchFinder.Skip(num);
                _additionalOffset += num;
            }
        }

		/// <summary>
		/// Computes the price of encoding a single-byte<br/>
		/// repetition match using the rep0 and rep0long encoders.
		/// </summary>
		/// <returns>The cumulative encoder price for this condition.</returns>
		private UInt32 GetRepLen1Price(Base.State state, UInt32 posState) => _isRepG0[state.Index].GetPrice0() +
				   _isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0();

		/// <summary>
		/// Computes the encoder price for selecting a<br/>
		/// specific repetition distance register.
		/// </summary>
		/// <returns>The price of switching to <paramref name="repIndex"/>.</returns>
		private UInt32 GetPureRepPrice(UInt32 repIndex, Base.State state, UInt32 posState)
        {
            UInt32 price;
            if (repIndex == 0)
            {
                price = _isRepG0[state.Index].GetPrice0();
                price += _isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            }
            else
            {
                price = _isRepG0[state.Index].GetPrice1();
                if (repIndex == 1) {
	                price += _isRepG1[state.Index].GetPrice0();
                } else
                {
                    price += _isRepG1[state.Index].GetPrice1();
                    price += _isRepG2[state.Index].GetPrice(repIndex - 2);
                }
            }
            return price;
        }

        /// <summary>
        /// Computes the total encoder price for a repetition<br/>
        /// match of specified length and distance register.
        /// </summary>
        /// <returns>The combined price of the repetition match.</returns>
        private UInt32 GetRepPrice(UInt32 repIndex, UInt32 len, Base.State state, UInt32 posState)
        {
            UInt32 price = _repMatchLenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
            return price + GetPureRepPrice(repIndex, state, posState);
        }

        /// <summary>
        /// Computes the total encoder price for an absolute<br/>
        /// distance match of specified length and position.
        /// </summary>
        /// <returns>The combined price of the distance match.</returns>
        private UInt32 GetPosLenPrice(UInt32 pos, UInt32 len, UInt32 posState)
        {
            UInt32 price;
            UInt32 lenToPosState = Base.GetLenToPosState(len);
            if (pos < Base.kNumFullDistances) {
	            price = _distancesPrices[(lenToPosState*Base.kNumFullDistances) + pos];
            } else {
	            price = _posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + GetPosSlot2(pos)] +
	                    _alignPrices[pos & Base.kAlignMask];
            }
            return price + _lenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
        }

        /// <summary>
        /// Traces backward through the optimization array<br/>
        /// to extract the optimal encoding sequence.
        /// </summary>
        /// <returns>The number of bytes in the optimal sequence.</returns>
        private UInt32 Backward(out UInt32 backRes, UInt32 cur)
        {
            _optimumEndIndex = cur;
            UInt32 posMem = _optimum[cur].PosPrev;
            UInt32 backMem = _optimum[cur].BackPrev;
            do
            {
                if (_optimum[cur].Prev1IsChar)
                {
                    _optimum[posMem].MakeAsChar();
                    _optimum[posMem].PosPrev = posMem - 1;
                    if (_optimum[cur].Prev2)
                    {
                        _optimum[posMem - 1].Prev1IsChar = false;
                        _optimum[posMem - 1].PosPrev = _optimum[cur].PosPrev2;
                        _optimum[posMem - 1].BackPrev = _optimum[cur].BackPrev2;
                    }
                }
                UInt32 posPrev = posMem;
                UInt32 backCur = backMem;

                backMem = _optimum[posPrev].BackPrev;
                posMem = _optimum[posPrev].PosPrev;

                _optimum[posPrev].BackPrev = backCur;
                _optimum[posPrev].PosPrev = cur;
                cur = posPrev;
            } while (cur > 0);
            backRes = _optimum[0].BackPrev;
            _optimumCurrentIndex = _optimum[0].PosPrev;
            return _optimumCurrentIndex;
        }


        /// <summary>
        /// Computes the optimal match or literal choice at<br/>
        /// the current position using dynamic programming.
        /// </summary>
        /// <returns>The length of the optimal match or literal sequence.</returns>
        private UInt32 GetOptimum(UInt32 position, out UInt32 backRes)
        {
            if (_optimumEndIndex != _optimumCurrentIndex)
            {
                UInt32 lenRes = _optimum[_optimumCurrentIndex].PosPrev - _optimumCurrentIndex;
                backRes = _optimum[_optimumCurrentIndex].BackPrev;
                _optimumCurrentIndex = _optimum[_optimumCurrentIndex].PosPrev;
                return lenRes;
            }
            _optimumCurrentIndex = _optimumEndIndex = 0;

            UInt32 lenMain, numDistancePairs;
            if (!_longestMatchWasFound)
            {
                ReadMatchDistances(out lenMain, out numDistancePairs);
            }
            else
            {
                lenMain = _longestMatchLength;
                numDistancePairs = _numDistancePairs;
                _longestMatchWasFound = false;
            }

            UInt32 numAvailableBytes = _matchFinder.GetNumAvailableBytes() + 1;
            if (numAvailableBytes < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }
            if (numAvailableBytes > Base.kMatchMaxLen) {
	            numAvailableBytes = Base.kMatchMaxLen;
            }

            UInt32 repMaxIndex = 0;

            for (UInt32 i = 0; i < Base.kNumRepDistances; i++)
            {
                reps[i] = _repDistances[i];
                repLens[i] = _matchFinder.GetMatchLen(0 - 1, reps[i], Base.kMatchMaxLen);
                if (repLens[i] > repLens[repMaxIndex]) {
	                repMaxIndex = i;
                }
            }
            if (repLens[repMaxIndex] >= _numFastBytes)
            {
                backRes = repMaxIndex;
                UInt32 lenRes = repLens[repMaxIndex];
                MovePos(lenRes - 1);
                return lenRes;
            }

            if (lenMain >= _numFastBytes)
            {
                backRes = _matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
                MovePos(lenMain - 1);
                return lenMain;
            }

            Byte currentByte = _matchFinder.GetIndexByte(0 - 1);
            Byte matchByte = _matchFinder.GetIndexByte((Int32) (0 - _repDistances[0] - 1 - 1));

            if (lenMain < 2 && currentByte != matchByte && repLens[repMaxIndex] < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            _optimum[0].State = _state;

            UInt32 posState = (position & _posStateMask);

            _optimum[1].Price = _isMatch[(_state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                                _literalEncoder.GetSubCoder(position, _previousByte).GetPrice(!_state.IsCharState(),
                                                                                              matchByte, currentByte);
            _optimum[1].MakeAsChar();

            UInt32 matchPrice = _isMatch[(_state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            UInt32 repMatchPrice = matchPrice + _isRep[_state.Index].GetPrice1();

            if (matchByte == currentByte)
            {
                UInt32 shortRepPrice = repMatchPrice + GetRepLen1Price(_state, posState);
                if (shortRepPrice < _optimum[1].Price)
                {
                    _optimum[1].Price = shortRepPrice;
                    _optimum[1].MakeAsShortRep();
                }
            }

            UInt32 lenEnd = ((lenMain >= repLens[repMaxIndex]) ? lenMain : repLens[repMaxIndex]);

            if (lenEnd < 2)
            {
                backRes = _optimum[1].BackPrev;
                return 1;
            }

            _optimum[1].PosPrev = 0;

            _optimum[0].Backs0 = reps[0];
            _optimum[0].Backs1 = reps[1];
            _optimum[0].Backs2 = reps[2];
            _optimum[0].Backs3 = reps[3];

            UInt32 len = lenEnd;
            do
                _optimum[len--].Price = kIfinityPrice; while (len >= 2);

            for (UInt32 i = 0; i < Base.kNumRepDistances; i++)
            {
                UInt32 repLen = repLens[i];
                if (repLen < 2) {
	                continue;
                }
                UInt32 price = repMatchPrice + GetPureRepPrice(i, _state, posState);
                do
                {
                    UInt32 curAndLenPrice = price + _repMatchLenEncoder.GetPrice(repLen - 2, posState);
                    Optimal optimum = _optimum[repLen];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = i;
                        optimum.Prev1IsChar = false;
                    }
                } while (--repLen >= 2);
            }

            UInt32 normalMatchPrice = matchPrice + _isRep[_state.Index].GetPrice0();

            len = ((repLens[0] >= 2) ? repLens[0] + 1 : 2);
            if (len <= lenMain)
            {
                UInt32 offs = 0;
                while (len > _matchDistances[offs])
                    offs += 2;
                for (;; len++)
                {
                    UInt32 distance = _matchDistances[offs + 1];
                    UInt32 curAndLenPrice = normalMatchPrice + GetPosLenPrice(distance, len, posState);
                    Optimal optimum = _optimum[len];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = distance + Base.kNumRepDistances;
                        optimum.Prev1IsChar = false;
                    }
                    if (len == _matchDistances[offs])
                    {
                        offs += 2;
                        if (offs == numDistancePairs) {
	                        break;
                        }
                    }
                }
            }

            UInt32 cur = 0;

            while (true)
            {
                cur++;
                if (cur == lenEnd) {
	                return Backward(out backRes, cur);
                }
                UInt32 newLen;
                ReadMatchDistances(out newLen, out numDistancePairs);
                if (newLen >= _numFastBytes)
                {
                    _numDistancePairs = numDistancePairs;
                    _longestMatchLength = newLen;
                    _longestMatchWasFound = true;
                    return Backward(out backRes, cur);
                }
                position++;
                UInt32 posPrev = _optimum[cur].PosPrev;
                Base.State state;
                if (_optimum[cur].Prev1IsChar)
                {
                    posPrev--;
                    if (_optimum[cur].Prev2)
                    {
                        state = _optimum[_optimum[cur].PosPrev2].State;
                        if (_optimum[cur].BackPrev2 < Base.kNumRepDistances) {
	                        state.UpdateRep();
                        } else {
	                        state.UpdateMatch();
                        }
                    }
                    else {
	                    state = _optimum[posPrev].State;
                    }
                    state.UpdateChar();
                }
                else {
	                state = _optimum[posPrev].State;
                }
                if (posPrev == cur - 1)
                {
                    if (_optimum[cur].IsShortRep()) {
	                    state.UpdateShortRep();
                    } else {
	                    state.UpdateChar();
                    }
                }
                else
                {
                    UInt32 pos;
                    if (_optimum[cur].Prev1IsChar && _optimum[cur].Prev2)
                    {
                        posPrev = _optimum[cur].PosPrev2;
                        pos = _optimum[cur].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        pos = _optimum[cur].BackPrev;
                        if (pos < Base.kNumRepDistances) {
	                        state.UpdateRep();
                        } else {
	                        state.UpdateMatch();
                        }
                    }
                    Optimal opt = _optimum[posPrev];
                    if (pos < Base.kNumRepDistances)
                    {
                        if (pos == 0)
                        {
                            reps[0] = opt.Backs0;
                            reps[1] = opt.Backs1;
                            reps[2] = opt.Backs2;
                            reps[3] = opt.Backs3;
                        }
                        else if (pos == 1)
                        {
                            reps[0] = opt.Backs1;
                            reps[1] = opt.Backs0;
                            reps[2] = opt.Backs2;
                            reps[3] = opt.Backs3;
                        }
                        else if (pos == 2)
                        {
                            reps[0] = opt.Backs2;
                            reps[1] = opt.Backs0;
                            reps[2] = opt.Backs1;
                            reps[3] = opt.Backs3;
                        }
                        else
                        {
                            reps[0] = opt.Backs3;
                            reps[1] = opt.Backs0;
                            reps[2] = opt.Backs1;
                            reps[3] = opt.Backs2;
                        }
                    }
                    else
                    {
                        reps[0] = (pos - Base.kNumRepDistances);
                        reps[1] = opt.Backs0;
                        reps[2] = opt.Backs1;
                        reps[3] = opt.Backs2;
                    }
                }
                _optimum[cur].State = state;
                _optimum[cur].Backs0 = reps[0];
                _optimum[cur].Backs1 = reps[1];
                _optimum[cur].Backs2 = reps[2];
                _optimum[cur].Backs3 = reps[3];
                UInt32 curPrice = _optimum[cur].Price;

                currentByte = _matchFinder.GetIndexByte(0 - 1);
                matchByte = _matchFinder.GetIndexByte((Int32) (0 - reps[0] - 1 - 1));

                posState = (position & _posStateMask);

                UInt32 curAnd1Price = curPrice +
                                      _isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                                      _literalEncoder.GetSubCoder(position, _matchFinder.GetIndexByte(0 - 2)).
                                          GetPrice(!state.IsCharState(), matchByte, currentByte);

                Optimal nextOptimum = _optimum[cur + 1];

                bool nextIsChar = false;
                if (curAnd1Price < nextOptimum.Price)
                {
                    nextOptimum.Price = curAnd1Price;
                    nextOptimum.PosPrev = cur;
                    nextOptimum.MakeAsChar();
                    nextIsChar = true;
                }

                matchPrice = curPrice + _isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
                repMatchPrice = matchPrice + _isRep[state.Index].GetPrice1();

                if (matchByte == currentByte &&
                    !(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
                {
                    UInt32 shortRepPrice = repMatchPrice + GetRepLen1Price(state, posState);
                    if (shortRepPrice <= nextOptimum.Price)
                    {
                        nextOptimum.Price = shortRepPrice;
                        nextOptimum.PosPrev = cur;
                        nextOptimum.MakeAsShortRep();
                        nextIsChar = true;
                    }
                }

                UInt32 numAvailableBytesFull = _matchFinder.GetNumAvailableBytes() + 1;
                numAvailableBytesFull = Math.Min(((int) kNumOpts) - 1 - cur, numAvailableBytesFull);
                numAvailableBytes = numAvailableBytesFull;

                if (numAvailableBytes < 2) {
	                continue;
                }
                if (numAvailableBytes > _numFastBytes) {
	                numAvailableBytes = _numFastBytes;
                }
                if (!nextIsChar && matchByte != currentByte)
                {
                    // try Literal + rep0
                    UInt32 t = Math.Min(numAvailableBytesFull - 1, _numFastBytes);
                    UInt32 lenTest2 = _matchFinder.GetMatchLen(0, reps[0], t);
                    if (lenTest2 >= 2)
                    {
                        Base.State state2 = state;
                        state2.UpdateChar();
                        UInt32 posStateNext = (position + 1) & _posStateMask;
                        UInt32 nextRepMatchPrice = curAnd1Price +
                                                   _isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].
                                                       GetPrice1() +
                                                   _isRep[state2.Index].GetPrice1();
                        {
                            UInt32 offset = cur + 1 + lenTest2;
                            while (lenEnd < offset)
                                _optimum[++lenEnd].Price = kIfinityPrice;
                            UInt32 curAndLenPrice = nextRepMatchPrice + GetRepPrice(
                                                                            0, lenTest2, state2, posStateNext);
                            Optimal optimum = _optimum[offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = false;
                            }
                        }
                    }
                }

                UInt32 startLen = 2; // speed optimization 

                for (UInt32 repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++)
                {
                    UInt32 lenTest = _matchFinder.GetMatchLen(0 - 1, reps[repIndex], numAvailableBytes);
                    if (lenTest < 2) {
	                    continue;
                    }
                    UInt32 lenTestTemp = lenTest;
                    do
                    {
                        while (lenEnd < cur + lenTest)
                            _optimum[++lenEnd].Price = kIfinityPrice;
                        UInt32 curAndLenPrice = repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState);
                        Optimal optimum = _optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = repIndex;
                            optimum.Prev1IsChar = false;
                        }
                    } while (--lenTest >= 2);
                    lenTest = lenTestTemp;

                    if (repIndex == 0) {
	                    startLen = lenTest + 1;
                    }

                    // if (_maxMode)
                    if (lenTest < numAvailableBytesFull)
                    {
                        UInt32 t = Math.Min(numAvailableBytesFull - 1 - lenTest, _numFastBytes);
                        UInt32 lenTest2 = _matchFinder.GetMatchLen((Int32) lenTest, reps[repIndex], t);
                        if (lenTest2 >= 2)
                        {
                            Base.State state2 = state;
                            state2.UpdateRep();
                            UInt32 posStateNext = (position + lenTest) & _posStateMask;
                            UInt32 curAndLenCharPrice =
                                repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState) +
                                _isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() +
                                _literalEncoder.GetSubCoder(position + lenTest,
                                                            _matchFinder.GetIndexByte((Int32) lenTest - 1 - 1)).GetPrice
                                    (true,
                                     _matchFinder.GetIndexByte(((Int32) lenTest - 1 - (Int32) (reps[repIndex] + 1))),
                                     _matchFinder.GetIndexByte((Int32) lenTest - 1));
                            state2.UpdateChar();
                            posStateNext = (position + lenTest + 1) & _posStateMask;
                            UInt32 nextMatchPrice = curAndLenCharPrice +
                                                    _isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext]
                                                        .GetPrice1();
                            UInt32 nextRepMatchPrice = nextMatchPrice + _isRep[state2.Index].GetPrice1();

                            // for(; lenTest2 >= 2; lenTest2--)
                            {
                                UInt32 offset = lenTest + 1 + lenTest2;
                                while (lenEnd < cur + offset)
                                    _optimum[++lenEnd].Price = kIfinityPrice;
                                UInt32 curAndLenPrice = nextRepMatchPrice +
                                                        GetRepPrice(0, lenTest2, state2, posStateNext);
                                Optimal optimum = _optimum[cur + offset];
                                if (curAndLenPrice < optimum.Price)
                                {
                                    optimum.Price = curAndLenPrice;
                                    optimum.PosPrev = cur + lenTest + 1;
                                    optimum.BackPrev = 0;
                                    optimum.Prev1IsChar = true;
                                    optimum.Prev2 = true;
                                    optimum.PosPrev2 = cur;
                                    optimum.BackPrev2 = repIndex;
                                }
                            }
                        }
                    }
                }

                if (newLen > numAvailableBytes)
                {
                    newLen = numAvailableBytes;
                    for (numDistancePairs = 0; newLen > _matchDistances[numDistancePairs]; numDistancePairs += 2) ;
                    _matchDistances[numDistancePairs] = newLen;
                    numDistancePairs += 2;
                }
                if (newLen >= startLen)
                {
                    normalMatchPrice = matchPrice + _isRep[state.Index].GetPrice0();
                    while (lenEnd < cur + newLen)
                        _optimum[++lenEnd].Price = kIfinityPrice;

                    UInt32 offs = 0;
                    while (startLen > _matchDistances[offs])
                        offs += 2;

                    for (UInt32 lenTest = startLen;; lenTest++)
                    {
                        UInt32 curBack = _matchDistances[offs + 1];
                        UInt32 curAndLenPrice = normalMatchPrice + GetPosLenPrice(curBack, lenTest, posState);
                        Optimal optimum = _optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = curBack + Base.kNumRepDistances;
                            optimum.Prev1IsChar = false;
                        }

                        if (lenTest == _matchDistances[offs])
                        {
                            if (lenTest < numAvailableBytesFull)
                            {
                                UInt32 t = Math.Min(numAvailableBytesFull - 1 - lenTest, _numFastBytes);
                                UInt32 lenTest2 = _matchFinder.GetMatchLen((Int32) lenTest, curBack, t);
                                if (lenTest2 >= 2)
                                {
                                    Base.State state2 = state;
                                    state2.UpdateMatch();
                                    UInt32 posStateNext = (position + lenTest) & _posStateMask;
                                    UInt32 curAndLenCharPrice = curAndLenPrice +
                                                                _isMatch[
                                                                    (state2.Index << Base.kNumPosStatesBitsMax) +
                                                                    posStateNext].GetPrice0() +
                                                                _literalEncoder.GetSubCoder(position + lenTest,
                                                                                            _matchFinder.GetIndexByte(
                                                                                                (Int32) lenTest - 1 - 1))
                                                                    .
                                                                    GetPrice(true,
                                                                             _matchFinder.GetIndexByte((Int32) lenTest -
                                                                                                       (Int32)
                                                                                                       (curBack + 1) - 1),
                                                                             _matchFinder.GetIndexByte((Int32) lenTest -
                                                                                                       1));
                                    state2.UpdateChar();
                                    posStateNext = (position + lenTest + 1) & _posStateMask;
                                    UInt32 nextMatchPrice = curAndLenCharPrice +
                                                            _isMatch[
                                                                (state2.Index << Base.kNumPosStatesBitsMax) +
                                                                posStateNext].GetPrice1();
                                    UInt32 nextRepMatchPrice = nextMatchPrice + _isRep[state2.Index].GetPrice1();

                                    UInt32 offset = lenTest + 1 + lenTest2;
                                    while (lenEnd < cur + offset)
                                        _optimum[++lenEnd].Price = kIfinityPrice;
                                    curAndLenPrice = nextRepMatchPrice + GetRepPrice(0, lenTest2, state2, posStateNext);
                                    optimum = _optimum[cur + offset];
                                    if (curAndLenPrice < optimum.Price)
                                    {
                                        optimum.Price = curAndLenPrice;
                                        optimum.PosPrev = cur + lenTest + 1;
                                        optimum.BackPrev = 0;
                                        optimum.Prev1IsChar = true;
                                        optimum.Prev2 = true;
                                        optimum.PosPrev2 = cur;
                                        optimum.BackPrev2 = curBack + Base.kNumRepDistances;
                                    }
                                }
                            }
                            offs += 2;
                            if (offs == numDistancePairs) {
	                            break;
                            }
                        }
                    }
                }
            }
        }

        /*static bool ChangePair(UInt32 smallDist, UInt32 bigDist)
		{
			const int kDif = 7;
			return (smallDist < ((UInt32)(1) << (32 - kDif)) && bigDist >= (smallDist << kDif));
		}*/

        /// <summary>
        /// Encodes an end-of-stream marker using a<br/>
        /// maximum-distance match sentinel value.
        /// </summary>
        private void WriteEndMarker(UInt32 posState)
        {
            if (!_writeEndMark) {
	            return;
            }

            _isMatch[(_state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(_rangeEncoder, 1);
            _isRep[_state.Index].Encode(_rangeEncoder, 0);
            _state.UpdateMatch();
            UInt32 len = Base.kMatchMinLen;
            _lenEncoder.Encode(_rangeEncoder, len - Base.kMatchMinLen, posState);
            UInt32 posSlot = (1 << Base.kNumPosSlotBits) - 1;
            UInt32 lenToPosState = Base.GetLenToPosState(len);
            _posSlotEncoder[lenToPosState].Encode(_rangeEncoder, posSlot);
            int footerBits = 30;
            UInt32 posReduced = (((UInt32) 1) << footerBits) - 1;
            _rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
            _posAlignEncoder.ReverseEncode(_rangeEncoder, posReduced & Base.kAlignMask);
        }

        /// <summary>
        /// Writes the end-of-stream marker and flushes<br/>
        /// all remaining data to the output stream.
        /// </summary>
        private void Flush(UInt32 nowPos)
        {
            ReleaseMFStream();
            WriteEndMarker(nowPos & _posStateMask);
            _rangeEncoder.FlushData();
            _rangeEncoder.FlushStream();
        }

        /// <summary>
        /// Encodes a block of input using optimal matching and<br/>
        /// returns the sizes of data processed and produced.
        /// </summary>
        internal void CodeOneBlock(out Int64 inSize, out Int64 outSize, out bool finished)
        {
            inSize = 0;
            outSize = 0;
            finished = true;

            if (_inStream != null)
            {
                _matchFinder.SetStream(_inStream);
                _matchFinder.Init();
                _needReleaseMFStream = true;
                _inStream = null;
                if (_trainSize > 0) {
	                _matchFinder.Skip(_trainSize);
                }
            }

            if (_finished) {
	            return;
            }
            _finished = true;


            Int64 progressPosValuePrev = nowPos64;
            if (nowPos64 == 0)
            {
                if (_matchFinder.GetNumAvailableBytes() == 0)
                {
                    Flush((UInt32) nowPos64);
                    return;
                }
                UInt32 len, numDistancePairs; // it's not used
                ReadMatchDistances(out len, out numDistancePairs);
                UInt32 posState = (UInt32) (nowPos64) & _posStateMask;
                _isMatch[(_state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(_rangeEncoder, 0);
                _state.UpdateChar();
                Byte curByte = _matchFinder.GetIndexByte((Int32) (0 - _additionalOffset));
                _literalEncoder.GetSubCoder((UInt32) (nowPos64), _previousByte).Encode(_rangeEncoder, curByte);
                _previousByte = curByte;
                _additionalOffset--;
                nowPos64++;
            }
            if (_matchFinder.GetNumAvailableBytes() == 0)
            {
                Flush((UInt32) nowPos64);
                return;
            }
            while (true)
            {
                UInt32 pos;
                UInt32 len = GetOptimum((UInt32) nowPos64, out pos);

                UInt32 posState = ((UInt32) nowPos64) & _posStateMask;
                UInt32 complexState = (_state.Index << Base.kNumPosStatesBitsMax) + posState;
                if (len == 1 && pos == 0xFFFFFFFF)
                {
                    _isMatch[complexState].Encode(_rangeEncoder, 0);
                    Byte curByte = _matchFinder.GetIndexByte((Int32) (0 - _additionalOffset));
                    LiteralEncoder.Encoder2 subCoder = _literalEncoder.GetSubCoder((UInt32) nowPos64, _previousByte);
                    if (!_state.IsCharState())
                    {
                        Byte matchByte =
                            _matchFinder.GetIndexByte((Int32) (0 - _repDistances[0] - 1 - _additionalOffset));
                        subCoder.EncodeMatched(_rangeEncoder, matchByte, curByte);
                    }
                    else {
	                    subCoder.Encode(_rangeEncoder, curByte);
                    }
                    _previousByte = curByte;
                    _state.UpdateChar();
                }
                else
                {
                    _isMatch[complexState].Encode(_rangeEncoder, 1);
                    if (pos < Base.kNumRepDistances)
                    {
                        _isRep[_state.Index].Encode(_rangeEncoder, 1);
                        if (pos == 0)
                        {
                            _isRepG0[_state.Index].Encode(_rangeEncoder, 0);
                            if (len == 1) {
	                            _isRep0Long[complexState].Encode(_rangeEncoder, 0);
                            } else {
	                            _isRep0Long[complexState].Encode(_rangeEncoder, 1);
                            }
                        }
                        else
                        {
                            _isRepG0[_state.Index].Encode(_rangeEncoder, 1);
                            if (pos == 1) {
	                            _isRepG1[_state.Index].Encode(_rangeEncoder, 0);
                            } else
                            {
                                _isRepG1[_state.Index].Encode(_rangeEncoder, 1);
                                _isRepG2[_state.Index].Encode(_rangeEncoder, pos - 2);
                            }
                        }
                        if (len == 1) {
	                        _state.UpdateShortRep();
                        } else
                        {
                            _repMatchLenEncoder.Encode(_rangeEncoder, len - Base.kMatchMinLen, posState);
                            _state.UpdateRep();
                        }
                        UInt32 distance = _repDistances[pos];
                        if (pos != 0)
                        {
                            for (UInt32 i = pos; i >= 1; i--)
                                _repDistances[i] = _repDistances[i - 1];
                            _repDistances[0] = distance;
                        }
                    }
                    else
                    {
                        _isRep[_state.Index].Encode(_rangeEncoder, 0);
                        _state.UpdateMatch();
                        _lenEncoder.Encode(_rangeEncoder, len - Base.kMatchMinLen, posState);
                        pos -= Base.kNumRepDistances;
                        UInt32 posSlot = GetPosSlot(pos);
                        UInt32 lenToPosState = Base.GetLenToPosState(len);
                        _posSlotEncoder[lenToPosState].Encode(_rangeEncoder, posSlot);

                        if (posSlot >= Base.kStartPosModelIndex)
                        {
                            var footerBits = (int) ((posSlot >> 1) - 1);
                            UInt32 baseVal = ((2 | (posSlot & 1)) << footerBits);
                            UInt32 posReduced = pos - baseVal;

                            if (posSlot < Base.kEndPosModelIndex) {
	                            BitTreeEncoder.ReverseEncode(_posEncoders,
		                            baseVal - posSlot - 1, _rangeEncoder, footerBits,
		                            posReduced);
                            } else
                            {
                                _rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits,
                                                               footerBits - Base.kNumAlignBits);
                                _posAlignEncoder.ReverseEncode(_rangeEncoder, posReduced & Base.kAlignMask);
                                _alignPriceCount++;
                            }
                        }
                        UInt32 distance = pos;
                        for (Int32 i = ((int) Base.kNumRepDistances) - 1; i >= 1; i--)
                            _repDistances[i] = _repDistances[i - 1];
                        _repDistances[0] = distance;
                        _matchPriceCount++;
                    }
                    _previousByte = _matchFinder.GetIndexByte((Int32) (len - 1 - _additionalOffset));
                }
                _additionalOffset -= len;
                nowPos64 += len;
                if (_additionalOffset == 0)
                {
                    // if (!_fastMode)
                    if (_matchPriceCount >= (1 << 7)) {
	                    FillDistancesPrices();
                    }
                    if (_alignPriceCount >= Base.kAlignTableSize) {
	                    FillAlignPrices();
                    }
                    inSize = nowPos64;
                    outSize = _rangeEncoder.GetProcessedSizeAdd();
                    if (_matchFinder.GetNumAvailableBytes() == 0)
                    {
                        Flush((UInt32) nowPos64);
                        return;
                    }

                    if (nowPos64 - progressPosValuePrev >= (1 << 12))
                    {
                        _finished = false;
                        finished = false;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Releases the match finder stream if it<br/>
        /// was previously acquired by the encoder.
        /// </summary>
        private void ReleaseMFStream()
        {
            if (_matchFinder != null && _needReleaseMFStream)
            {
                _matchFinder.ReleaseStream();
                _needReleaseMFStream = false;
            }
        }

		/// <summary>Sets the output stream for the range encoder.</summary>
		private void SetOutStream(Stream outStream) => _rangeEncoder.SetStream(outStream);

		/// <summary>Releases the output stream from the range encoder.</summary>
		private void ReleaseOutStream() => _rangeEncoder.ReleaseStream();

		/// <summary>
		/// Releases both the match finder and output<br/>
		/// streams from the encoder.
		/// </summary>
		private void ReleaseStreams()
        {
            ReleaseMFStream();
            ReleaseOutStream();
        }

        /// <summary>
        /// Initializes the encoder with input and output streams,<br/>
        /// and prepares all price tables for encoding.
        /// </summary>
        private void SetStreams(Stream inStream, Stream outStream /*,
				Int64 inSize, Int64 outSize*/)
        {
            _inStream = inStream;
            _finished = false;
            Create();
            SetOutStream(outStream);
            Init();

            // if (!_fastMode)
            {
                FillDistancesPrices();
                FillAlignPrices();
            }

            _lenEncoder.SetTableSize(_numFastBytes + 1 - Base.kMatchMinLen);
            _lenEncoder.UpdateTables((UInt32) 1 << _posStateBits);
            _repMatchLenEncoder.SetTableSize(_numFastBytes + 1 - Base.kMatchMinLen);
            _repMatchLenEncoder.UpdateTables((UInt32) 1 << _posStateBits);

            nowPos64 = 0;
        }

        /// <summary>
        /// Computes and caches the encoder prices for all<br/>
        /// distance encodings across all position states.
        /// </summary>
        private void FillDistancesPrices()
        {
            for (UInt32 i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++)
            {
                UInt32 posSlot = GetPosSlot(i);
                var footerBits = (int) ((posSlot >> 1) - 1);
                UInt32 baseVal = ((2 | (posSlot & 1)) << footerBits);
                tempPrices[i] = BitTreeEncoder.ReverseGetPrice(_posEncoders,
                                                               baseVal - posSlot - 1, footerBits, i - baseVal);
            }

            for (UInt32 lenToPosState = 0; lenToPosState < Base.kNumLenToPosStates; lenToPosState++)
            {
                UInt32 posSlot;
                BitTreeEncoder encoder = _posSlotEncoder[lenToPosState];

                UInt32 st = (lenToPosState << Base.kNumPosSlotBits);
                for (posSlot = 0; posSlot < _distTableSize; posSlot++)
                    _posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
                for (posSlot = Base.kEndPosModelIndex; posSlot < _distTableSize; posSlot++)
                    _posSlotPrices[st + posSlot] += ((((posSlot >> 1) - 1) - Base.kNumAlignBits) <<
                                                     BitEncoder.kNumBitPriceShiftBits);

                UInt32 st2 = lenToPosState*Base.kNumFullDistances;
                UInt32 i;
                for (i = 0; i < Base.kStartPosModelIndex; i++)
                    _distancesPrices[st2 + i] = _posSlotPrices[st + i];
                for (; i < Base.kNumFullDistances; i++)
                    _distancesPrices[st2 + i] = _posSlotPrices[st + GetPosSlot(i)] + tempPrices[i];
            }
            _matchPriceCount = 0;
        }

        /// <summary>
        /// Computes and caches the encoder prices for<br/>
        /// distance alignment bit encoding.
        /// </summary>
        private void FillAlignPrices()
        {
            for (UInt32 i = 0; i < Base.kAlignTableSize; i++)
                _alignPrices[i] = _posAlignEncoder.ReverseGetPrice(i);
            _alignPriceCount = 0;
        }


        /// <summary>
        /// Searches for the match finder ID in the supported<br/>
        /// match finder types list.
        /// </summary>
        /// <returns>The zero-based index if found; otherwise, -1.</returns>
        private static int FindMatchFinder(string s)
        {
            for (int m = 0; m < kMatchFinderIDs.Length; m++)
                if (s == kMatchFinderIDs[m]) {
	                return m;
                }
            return -1;
        }

        #region Nested type: EMatchFinderType

        /// <summary>
        /// Specifies the binary tree match finder algorithm
        /// variant used for string matching during compression.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 1481 | <see cref="BT2"/> | Binary tree match finder with 2-byte hash. |
        /// | 1483 | <see cref="BT4"/> | Binary tree match finder with 4-byte hash. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T15:43:23Z", Digest = "17f8d9d24ad32b9db1a51e9c5b292b182208d0e98026c4cd3637dac495aef2f9", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
        private enum EMatchFinderType
        {
            /// <summary>Binary tree match finder with 2-byte hash.</summary>
            BT2,
            /// <summary>Binary tree match finder with 4-byte hash.</summary>
            BT4,
        } ;

        #endregion

        #region Nested type: LenEncoder

        /// <summary>
        /// Encodes match lengths using a tiered bit tree<br/>
        /// approach that scales from low to high values.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 1521 | <see cref="LenEncoder"/> | Initializes a new instance of LenEncoder. |
        /// | 1534 | <see cref="Init"/> | Initializes all length encoders for the  specified number of position states. |
        /// | 1550 | <see cref="Encode"/> | Encodes a match length symbol using low,  mid, or high encoder based on the value. |
        /// | 1578 | <see cref="SetPrices"/> | Computes and caches encoder prices for all  length symbols up to the specified limit. |
        ///
        /// ## Collaborators
        ///
        /// | Type | Role |
        /// |---|---|
        /// | <see cref="BitTreeEncoder"/> | Stores low, mid, and high-range bit tree encoders. |
        /// | <see cref="BitEncoder"/> | Choice selectors for range branching. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:21:35Z", Digest = "e6174d61a4dd8d28a1cdb1bab053d2a31961330d316673b52ed13bda3ed55a06", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
        private class LenEncoder
        {
            private readonly BitTreeEncoder[] _lowCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private readonly BitTreeEncoder[] _midCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private BitEncoder _choice;
            private BitEncoder _choice2;
            private BitTreeEncoder _highCoder = new BitTreeEncoder(Base.kNumHighLenBits);

            /// <summary>Initializes a new instance of <see cref="LenEncoder"/>.</summary>
            public LenEncoder()
            {
                for (UInt32 posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++)
                {
                    _lowCoder[posState] = new BitTreeEncoder(Base.kNumLowLenBits);
                    _midCoder[posState] = new BitTreeEncoder(Base.kNumMidLenBits);
                }
            }

            /// <summary>
            /// Initializes all length encoders for the<br/>
            /// specified number of position states.
            /// </summary>
            public void Init(UInt32 numPosStates)
            {
                _choice.Init();
                _choice2.Init();
                for (UInt32 posState = 0; posState < numPosStates; posState++)
                {
                    _lowCoder[posState].Init();
                    _midCoder[posState].Init();
                }
                _highCoder.Init();
            }

            /// <summary>
            /// Encodes a match length symbol using low,<br/>
            /// mid, or high encoder based on the value.
            /// </summary>
            public void Encode(RangeCoder.Encoder rangeEncoder, UInt32 symbol, UInt32 posState)
            {
                if (symbol < Base.kNumLowLenSymbols)
                {
                    _choice.Encode(rangeEncoder, 0);
                    _lowCoder[posState].Encode(rangeEncoder, symbol);
                }
                else
                {
                    symbol -= Base.kNumLowLenSymbols;
                    _choice.Encode(rangeEncoder, 1);
                    if (symbol < Base.kNumMidLenSymbols)
                    {
                        _choice2.Encode(rangeEncoder, 0);
                        _midCoder[posState].Encode(rangeEncoder, symbol);
                    }
                    else
                    {
                        _choice2.Encode(rangeEncoder, 1);
                        _highCoder.Encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
                    }
                }
            }

            /// <summary>
            /// Computes and caches encoder prices for all<br/>
            /// length symbols up to the specified limit.
            /// </summary>
            public void SetPrices(UInt32 posState, UInt32 numSymbols, UInt32[] prices, UInt32 st)
            {
                UInt32 a0 = _choice.GetPrice0();
                UInt32 a1 = _choice.GetPrice1();
                UInt32 b0 = a1 + _choice2.GetPrice0();
                UInt32 b1 = a1 + _choice2.GetPrice1();
                UInt32 i = 0;
                for (i = 0; i < Base.kNumLowLenSymbols; i++)
                {
                    if (i >= numSymbols) {
	                    return;
                    }
                    prices[st + i] = a0 + _lowCoder[posState].GetPrice(i);
                }
                for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++)
                {
                    if (i >= numSymbols) {
	                    return;
                    }
                    prices[st + i] = b0 + _midCoder[posState].GetPrice(i - Base.kNumLowLenSymbols);
                }
                for (; i < numSymbols; i++)
                    prices[st + i] = b1 + _highCoder.GetPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
            }
        } ;

        #endregion

        #region Nested type: LenPriceTableEncoder

        /// <summary>
        /// Caches precomputed encoder prices for length<br/>
        /// symbols to accelerate repeated price lookups.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 1636 | <see cref="SetTableSize"/> | Sets the maximum length symbol size for price computation. |
        /// | 1643 | <see cref="GetPrice"/> | Retrieves the cached encoder price for  a length symbol at the specified position state. |
        /// | 1659 | <see cref="UpdateTables"/> | Recomputes the cached price tables for  all specified position states. |
        /// | 1669 | <see cref="Encode"/> | Encodes a length symbol and updates the  cached prices when the counter expires. |
        ///
        /// ## Collaborators
        ///
        /// | Type | Role |
        /// |---|---|
        /// | <see cref="LenEncoder"/> | Base class providing length encoding logic. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:21:57Z", Digest = "8c1080afb1921087312aded327262da66fe65330f7e43ef48cf616c89e63b377", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
        private class LenPriceTableEncoder : LenEncoder
        {
            private readonly UInt32[] _counters = new UInt32[Base.kNumPosStatesEncodingMax];
            private readonly UInt32[] _prices = new UInt32[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];
            private UInt32 _tableSize;

			/// <summary>Sets the maximum length symbol size for price computation.</summary>
			public void SetTableSize(UInt32 tableSize) => _tableSize = tableSize;

			/// <summary>
			/// Retrieves the cached encoder price for<br/>
			/// a length symbol at the specified position state.
			/// </summary>
			/// <returns>The cached price value.</returns>
			public UInt32 GetPrice(UInt32 symbol, UInt32 posState) => _prices[posState * Base.kNumLenSymbols + symbol];

			/// <summary>
			/// Recomputes the cached prices for the<br/>
			/// specified position state.
			/// </summary>
			private void UpdateTable(UInt32 posState)
            {
                SetPrices(posState, _tableSize, _prices, posState*Base.kNumLenSymbols);
                _counters[posState] = _tableSize;
            }

            /// <summary>
            /// Recomputes the cached price tables for<br/>
            /// all specified position states.
            /// </summary>
            public void UpdateTables(UInt32 numPosStates)
            {
                for (UInt32 posState = 0; posState < numPosStates; posState++)
                    UpdateTable(posState);
            }

            /// <summary>
            /// Encodes a length symbol and updates the<br/>
            /// cached prices when the counter expires.
            /// </summary>
            public new void Encode(RangeCoder.Encoder rangeEncoder, UInt32 symbol, UInt32 posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--_counters[posState] == 0) {
	                UpdateTable(posState);
                }
            }
        }

        #endregion

        #region Nested type: LiteralEncoder

        /// <summary>
        /// Encodes literal bytes using context-dependent<br/>
        /// bit tree encoders based on position and previous byte.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 1683 | <see cref="Create"/> | Initializes the literal encoder with the specified position and previous byte context bits. |
        /// | 1701 | <see cref="Init"/> | Initializes all literal encoder contexts for a new encoding sequence. |
        /// | 1710 | <see cref="GetSubCoder"/> | Retrieves the context-specific literal encoder for the given position and previous byte. |
        ///
        /// ## Collaborators
        ///
        /// | Type | Role |
        /// |---|---|
        /// | <see cref="Encoder2"/> | Context-specific literal encoders. |
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:21:57Z", Digest = "32dfcd907dfed8934ec4c099526f4ca8d4ca0aac22bf5b21cbec5c8b564c6296", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
        private class LiteralEncoder
        {
            private Encoder2[] m_Coders;
            private int m_NumPosBits;
            private int m_NumPrevBits;
            private uint m_PosMask;

            /// <summary>
            /// Initializes the literal encoder with the<br/>
            /// specified position and previous byte context bits.
            /// </summary>
            internal void Create(int numPosBits, int numPrevBits)
            {
                if (m_Coders != null && m_NumPrevBits == numPrevBits && m_NumPosBits == numPosBits) {
	                return;
                }
                m_NumPosBits = numPosBits;
                m_PosMask = ((uint) 1 << numPosBits) - 1;
                m_NumPrevBits = numPrevBits;
                uint numStates = (uint) 1 << (m_NumPrevBits + m_NumPosBits);
                m_Coders = new Encoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Create();
            }

            /// <summary>
            /// Initializes all literal encoder contexts<br/>
            /// for a new encoding sequence.
            /// </summary>
            internal void Init()
            {
                uint numStates = (uint) 1 << (m_NumPrevBits + m_NumPosBits);
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Init();
            }

			/// <summary>
			/// Retrieves the context-specific literal<br/>
			/// encoder for the given position and previous byte.
			/// </summary>
			/// <returns>The Encoder2 context for the position and byte.</returns>
			internal Encoder2 GetSubCoder(UInt32 pos, Byte prevByte) => m_Coders[((pos & m_PosMask) << m_NumPrevBits) + (uint) (prevByte >> (8 - m_NumPrevBits))];

			#region Nested type: Encoder2

			/// <summary>
			/// Encodes a single literal byte using binary<br/>
			/// tree range encoding with adaptive contexts.
			/// </summary>
			/// <remarks>
			/// ## Public Methods
			///
			/// | Line | Method | Description |
			/// |--:|---|---|
			/// | 1774 | <see cref="Create"/> | Allocates the bit encoder array for literal encoding. |
			/// | 1780 | <see cref="Init"/> | Initializes all bit encoders for  a new literal encoding sequence. |
			/// | 1789 | <see cref="Encode"/> | Encodes a literal byte using bit-by-bit  range encoding through a binary tree. |
			/// | 1804 | <see cref="EncodeMatched"/> | Encodes a literal byte relative to a match  byte from a repeated distance using adaptive contexts. |
			/// | 1828 | <see cref="GetPrice"/> | Computes the encoder price for encoding a literal  byte optionally relative to a match byte. |
			///
			/// ## Collaborators
			///
			/// | Type | Role |
			/// |---|---|
			/// | <see cref="BitEncoder"/> | 768 bit encoders for binary tree contexts. |
			/// </remarks>
			[DocState(Pass = 2, MTime = "2026-08-24T14:22:17Z", Digest = "ae91a8cdca004807af2931d961e583ba60e6509a049b4446dcb44e94ec737e3c", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
			public struct Encoder2
            {
                private BitEncoder[] m_Encoders;

				/// <summary>Allocates the bit encoder array for literal encoding.</summary>
				public void Create() => m_Encoders = new BitEncoder[0x300];

				/// <summary>
				/// Initializes all bit encoders for<br/>
				/// a new literal encoding sequence.
				/// </summary>
				public void Init()
                {
                    for (int i = 0; i < 0x300; i++) m_Encoders[i].Init();
                }

                /// <summary>
                /// Encodes a literal byte using bit-by-bit<br/>
                /// range encoding through a binary tree.
                /// </summary>
                public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
                {
                    uint context = 1;
                    for (int i = 7; i >= 0; i--)
                    {
                        var bit = (uint) ((symbol >> i) & 1);
                        m_Encoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                /// <summary>
                /// Encodes a literal byte relative to a match<br/>
                /// byte from a repeated distance using adaptive contexts.
                /// </summary>
                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint context = 1;
                    bool same = true;
                    for (int i = 7; i >= 0; i--)
                    {
                        var bit = (uint) ((symbol >> i) & 1);
                        uint state = context;
                        if (same)
                        {
                            var matchBit = (uint) ((matchByte >> i) & 1);
                            state += ((1 + matchBit) << 8);
                            same = (matchBit == bit);
                        }
                        m_Encoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                /// <summary>
                /// Computes the encoder price for encoding a literal<br/>
                /// byte optionally relative to a match byte.
                /// </summary>
                /// <returns>The total price in encoder units.</returns>
                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint price = 0;
                    uint context = 1;
                    int i = 7;
                    if (matchMode)
                    {
                        for (; i >= 0; i--)
                        {
                            uint matchBit = (uint) (matchByte >> i) & 1;
                            uint bit = (uint) (symbol >> i) & 1;
                            price += m_Encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }
                    }
                    for (; i >= 0; i--)
                    {
                        uint bit = (uint) (symbol >> i) & 1;
                        price += m_Encoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }
                    return price;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: Optimal

        /// <summary>
        /// Tracks the optimal parsing state and cost for<br/>
        /// each position in dynamic programming optimization.
        /// </summary>
        /// <remarks>
        /// ## Public Methods
        ///
        /// | Line | Method | Description |
        /// |--:|---|---|
        /// | 1905 | <see cref="MakeAsChar"/> | Marks this optimal step as a literal  (non-match) encoding. |
        /// | 1915 | <see cref="MakeAsShortRep"/> | Marks this optimal step as a single-byte  repetition match. |
        /// | 1923 | <see cref="IsShortRep"/> | Determines whether short Rep. |
        ///
        /// ## Invariants
        ///
        /// Repetition distances are stored in Backs0-Backs3<br/>
        /// following the LZMA state machine; BackPrev<br/>
        /// distance codes are resolved after backtracking.
        /// </remarks>
        [DocState(Pass = 2, MTime = "2026-08-24T14:22:17Z", Digest = "93a0deb52772a4b399b4e675c5bb25f7a5e93f3ead5ca106603ae02ac923d2d1", Stale = false, Path = "sdk/Compress/LZMA/LzmaEncoder.cs", Since = "2026-08-23")]
        private class Optimal
        {
            public UInt32 BackPrev;
            public UInt32 BackPrev2;

            public UInt32 Backs0;
            public UInt32 Backs1;
            public UInt32 Backs2;
            public UInt32 Backs3;
            public UInt32 PosPrev;
            public UInt32 PosPrev2;
            public bool Prev1IsChar;
            public bool Prev2;
            public UInt32 Price;
            public Base.State State;

            /// <summary>
            /// Marks this optimal step as a literal<br/>
            /// (non-match) encoding.
            /// </summary>
            public void MakeAsChar()
            {
                BackPrev = 0xFFFFFFFF;
                Prev1IsChar = false;
            }

            /// <summary>
            /// Marks this optimal step as a single-byte<br/>
            /// repetition match.
            /// </summary>
            public void MakeAsShortRep()
            {
                BackPrev = 0;
                ;
                Prev1IsChar = false;
            }

			/// <summary>Determines whether short Rep.</summary>
			public bool IsShortRep() => (BackPrev == 0);
		} ;

		#endregion

		/// <summary>
		/// Sets the size of training data to skip at the<br/>
		/// beginning of the input stream.
		/// </summary>
		internal void SetTrainSize(uint trainSize) => _trainSize = trainSize;
	}
}