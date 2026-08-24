using System.ComponentModel;
using org.SpocWeb.root.Attributes;
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
    /// | 24 | <see cref="DataErrorException"/> | Initializes a new instance of DataErrorException. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "2a54acf601042205aacae5aee63187feb5a00b4bcfccaffaf6aec4c5679c39b4", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
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
    /// | 43 | <see cref="InvalidParamException"/> | Initializes a new instance of InvalidParamException. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "f2d9b2b9b0610d7243ec731a37c174a011116806f0d45f9d64c783653c069468", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
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
    /// | 68 | <see cref="SetProgress"/> | Callback progress. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "fbb6c8dad66643c6593060e6ff6413d90de67a7ed73d42d1bc7eb0389f24bfa1", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
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
        [System.ComponentModel.Description("Callback progress.")]
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
    /// | 112 | <see cref="Code"/> | Codes streams. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="ICodeProgress"/> | Passed as a parameter. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "cb5cbf12d71d7b0f3bd00d2075455a9fa66fcd7a9cd3b8b7cfbb40a8f23e85a7", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
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
        [System.ComponentModel.Description("Codes streams.")]
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
    /// | 158 | <see cref="DefaultProp"/> | Specifies default property. |
    /// | 162 | <see cref="DictionarySize"/> | Specifies size of dictionary. |
    /// | 166 | <see cref="UsedMemorySize"/> | Specifies size of memory for PPM*. |
    /// | 170 | <see cref="Order"/> | Specifies order for PPM methods. |
    /// | 174 | <see cref="BlockSize"/> | Specifies Block Size. |
    /// | 178 | <see cref="PosStateBits"/> | Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4). |
    /// | 182 | <see cref="LitContextBits"/> | Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8). |
    /// | 186 | <see cref="LitPosBits"/> | Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4). |
    /// | 190 | <see cref="NumFastBytes"/> | Specifies number of fast bytes for LZ*. |
    /// | 194 | <see cref="MatchFinder"/> | Specifies match finder. |
    /// | 198 | <see cref="MatchFinderCycles"/> | Specifies the number of match finder cyckes. |
    /// | 202 | <see cref="NumPasses"/> | Specifies number of passes. |
    /// | 206 | <see cref="Algorithm"/> | Specifies number of algorithm. |
    /// | 210 | <see cref="NumThreads"/> | Specifies the number of threads. |
    /// | 214 | <see cref="EndMarker"/> | Specifies mode with end marker. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "b3bddf342dfda561777005f4c5504a23222268a333432d73e128252de6cc2eef", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
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
    /// Defines the interface for setting encoder/decoder properties<br/>
    /// by identifier and value.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 242 | <see cref="SetCoderProperties"/> | Sets encoder/decoder properties specified by    to the corresponding values in  . |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="CoderPropId"/> | Property identifier enum; paired with property values. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:21:06Z", Digest = "2ebf83591102c7fb8b181c205ab7ffba831a651fccf87824b34642ec8e22d9a8", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
    internal interface ISetCoderProperties
    {

        /// <summary>
        /// Sets encoder/decoder properties specified by <paramref name='propIDs'/><br/>
        /// to the corresponding values in <paramref name='properties'/>.
        /// </summary>
        void SetCoderProperties(CoderPropId[] propIDs, object[] properties);
    } ;

    /// <summary>
    /// Defines the interface for writing encoder/decoder properties<br/>
    /// to a stream.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 261 | <see cref="WriteCoderProperties"/> | Writes the encoder/decoder properties to  . |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T14:21:06Z", Digest = "37fe8658628aaac173fbac71a3a99b25be2805443e76bb13664bc44922a19b1e", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
    internal interface IWriteCoderProperties
    {

        /// <summary>Writes the encoder/decoder properties to <paramref name='outStream'/>.</summary>
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
    /// | 281 | <see cref="SetDecoderProperties"/> | Sets decoder properties |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:51:46Z", Digest = "4cf33d8bdde3edb8feb51cfba66727968c5cb3ff777473d5f744eaa0c0503999", Stale = false, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
    internal interface ISetDecoderProperties
    {
        /// <summary>
        /// Sets decoder properties
        /// </summary>
        /// <param name="properties">Array of byte properties</param>
        [System.ComponentModel.Description("Sets decoder properties")]
        void SetDecoderProperties(byte[] properties);
    }
}