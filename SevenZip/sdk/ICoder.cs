namespace SevenZip.Sdk
{
    using System;
    using System.IO;

    /// <summary>
    /// The exception that is thrown when an error in input stream occurs during decoding.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 22 | <see cref="DataErrorException"/> | Initializes a new instance of DataErrorException. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 2a54acf601042205aacae5aee63187feb5a00b4bcfccaffaf6aec4c5679c39b4
    /// </code>
    /// </example>
    [Serializable]
    internal class DataErrorException : ApplicationException
    {

        /// <summary>Initializes a new instance of <see cref="DataErrorException"/>.</summary>
        public DataErrorException() : base("Data Error") {}
    }

    /// <summary>
    /// The exception that is thrown when the value of an argument is outside the allowable range.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 41 | <see cref="InvalidParamException"/> | Initializes a new instance of InvalidParamException. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: f2d9b2b9b0610d7243ec731a37c174a011116806f0d45f9d64c783653c069468
    /// </code>
    /// </example>
    [Serializable]
    internal class InvalidParamException : ApplicationException
    {

        /// <summary>Initializes a new instance of <see cref="InvalidParamException"/>.</summary>
        public InvalidParamException() : base("Invalid Parameter") {}
    }

    /// <summary>
    /// Callback progress interface.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 66 | <see cref="SetProgress"/> | Callback progress. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: fbb6c8dad66643c6593060e6ff6413d90de67a7ed73d42d1bc7eb0389f24bfa1
    /// </code>
    /// </example>
    public interface ICodeProgress
    {
        /// <summary>
        /// Callback progress.
        /// </summary>
        /// <param name="inSize">
        /// Processed input size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// Processed output size. -1 if unknown.
        /// </param>
        void SetProgress(Int64 inSize, Int64 outSize);
    } ;

    /// <summary>
    /// Stream coder interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 103 | <see cref="Code"/> | Codes streams. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="ICodeProgress"/> | Passed as a parameter. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: cb5cbf12d71d7b0f3bd00d2075455a9fa66fcd7a9cd3b8b7cfbb40a8f23e85a7
    /// </code>
    /// </example>
    public interface ICoder
    {
        /// <summary>
        /// Codes streams.
        /// </summary>
        /// <param name="inStream">
        /// input Stream.
        /// </param>
        /// <param name="outStream">
        /// output Stream.
        /// </param>
        /// <param name="inSize">
        /// input Size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// output Size. -1 if unknown.
        /// </param>
        /// <param name="progress">
        /// callback progress reference.
        /// </param>
        /// <exception cref="SevenZip.Sdk.DataErrorException">
        /// if input stream is not valid
        /// </exception>
        void Code(Stream inStream, Stream outStream,
                  Int64 inSize, Int64 outSize, ICodeProgress progress);
    } ;

    /*
	public interface ICoder2
	{
		 void Code(ISequentialInStream []inStreams,
				const UInt64 []inSizes, 
				ISequentialOutStream []outStreams, 
				UInt64 []outSizes,
				ICodeProgress progress);
	};
  */

    /// <summary>
    /// Provides the fields that represent properties idenitifiers for compressing.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 134 | <see cref="DefaultProp"/> | Specifies default property. |
    /// | 138 | <see cref="DictionarySize"/> | Specifies size of dictionary. |
    /// | 142 | <see cref="UsedMemorySize"/> | Specifies size of memory for PPM*. |
    /// | 146 | <see cref="Order"/> | Specifies order for PPM methods. |
    /// | 150 | <see cref="BlockSize"/> | Specifies Block Size. |
    /// | 154 | <see cref="PosStateBits"/> | Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4). |
    /// | 158 | <see cref="LitContextBits"/> | Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8). |
    /// | 162 | <see cref="LitPosBits"/> | Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4). |
    /// | 166 | <see cref="NumFastBytes"/> | Specifies number of fast bytes for LZ*. |
    /// | 170 | <see cref="MatchFinder"/> | Specifies match finder. |
    /// | 174 | <see cref="MatchFinderCycles"/> | Specifies the number of match finder cyckes. |
    /// | 178 | <see cref="NumPasses"/> | Specifies number of passes. |
    /// | 182 | <see cref="Algorithm"/> | Specifies number of algorithm. |
    /// | 186 | <see cref="NumThreads"/> | Specifies the number of threads. |
    /// | 190 | <see cref="EndMarker"/> | Specifies mode with end marker. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: b3bddf342dfda561777005f4c5504a23222268a333432d73e128252de6cc2eef
    /// </code>
    /// </example>
    public enum CoderPropId
    {
        /// <summary>
        /// Specifies default property.
        /// </summary>
        DefaultProp = 0,
        /// <summary>
        /// Specifies size of dictionary.
        /// </summary>
        DictionarySize,
        /// <summary>
        /// Specifies size of memory for PPM*.
        /// </summary>
        UsedMemorySize,
        /// <summary>
        /// Specifies order for PPM methods.
        /// </summary>
        Order,
        /// <summary>
        /// Specifies Block Size.
        /// </summary>
        BlockSize,
        /// <summary>
        /// Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        PosStateBits,
        /// <summary>
        /// Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8).
        /// </summary>
        LitContextBits,
        /// <summary>
        /// Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        LitPosBits,
        /// <summary>
        /// Specifies number of fast bytes for LZ*.
        /// </summary>
        NumFastBytes,
        /// <summary>
        /// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
        /// </summary>
        MatchFinder,
        /// <summary>
        /// Specifies the number of match finder cyckes.
        /// </summary>
        MatchFinderCycles,
        /// <summary>
        /// Specifies number of passes.
        /// </summary>
        NumPasses,
        /// <summary>
        /// Specifies number of algorithm.
        /// </summary>
        Algorithm,
        /// <summary>
        /// Specifies the number of threads.
        /// </summary>
        NumThreads,
        /// <summary>
        /// Specifies mode with end marker.
        /// </summary>
        EndMarker = 0x490
    } ;

    /// <summary>
    /// The ISetCoderProperties interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 208 | <see cref="SetCoderProperties"/> | Applies the given properties, matched by propIDs, to the coder. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="CoderPropId"/> | Passed as a parameter. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 1d62cb94a5f66efd6afeb80b87e6af4ab9bcff2eceed3fed088eb20902899356
    /// </code>
    /// </example>
    internal interface ISetCoderProperties
    {

        /// <summary>Applies the given <paramref name="properties"/>, matched by <paramref name="propIDs"/>, to the coder.</summary>
        void SetCoderProperties(CoderPropId[] propIDs, object[] properties);
    } ;

    /// <summary>
    /// The IWriteCoderProperties interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 226 | <see cref="WriteCoderProperties"/> | Writes the coder's current properties to outStream for later decoding. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 68dff711fc830bcf85ebf34f7584311772d730b0b73bd97a9efcd8aaeb3bb30e
    /// </code>
    /// </example>
    internal interface IWriteCoderProperties
    {

        /// <summary>Writes the coder's current properties to <paramref name="outStream"/> for later decoding.</summary>
        void WriteCoderProperties(Stream outStream);
    }

    /// <summary>
    /// The ISetDecoderPropertiesinterface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 246 | <see cref="SetDecoderProperties"/> | Sets decoder properties |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 4cf33d8bdde3edb8feb51cfba66727968c5cb3ff777473d5f744eaa0c0503999
    /// </code>
    /// </example>
    internal interface ISetDecoderProperties
    {
        /// <summary>
        /// Sets decoder properties
        /// </summary>
        /// <param name="properties">Array of byte properties</param>
        void SetDecoderProperties(byte[] properties);
    }
}