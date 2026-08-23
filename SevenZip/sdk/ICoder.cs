using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk
{
    using System;
    using System.IO;

    /// <summary>
    /// The exception that is thrown when an error in input stream occurs during decoding.
    /// </summary>
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
    /// The ISetCoderProperties interface
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:36Z", Digest = "d20518cf5542f1185a0d481d2c8a2f9eaa88e3d91da31a3343ab2d08adf13926", Stale = true, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
    internal interface ISetCoderProperties
    {

        /// <summary>TODO: LLM</summary>
        void SetCoderProperties(CoderPropId[] propIDs, object[] properties);
    } ;

    /// <summary>
    /// The IWriteCoderProperties interface
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:36Z", Digest = "d20518cf5542f1185a0d481d2c8a2f9eaa88e3d91da31a3343ab2d08adf13926", Stale = true, Path = "sdk/ICoder.cs", Since = "2026-08-23")]
    internal interface IWriteCoderProperties
    {

        /// <summary>TODO: LLM</summary>
        void WriteCoderProperties(Stream outStream);
    }

    /// <summary>
    /// The ISetDecoderPropertiesinterface
    /// </summary>
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