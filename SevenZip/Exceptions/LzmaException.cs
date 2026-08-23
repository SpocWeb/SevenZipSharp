using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for LZMA operations.
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "a3c323420e890497054cb6eaa7722a0fcfb96306c7a1dd0c1d866057fd00f94d", Stale = false, Path = "Exceptions/LzmaException.cs", Since = "2026-08-23")]
    [Serializable]
    public class LzmaException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public const string DEFAULT_MESSAGE = "Specified stream is not a valid LZMA compressed stream!";

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        [System.ComponentModel.Description("Initializes a new instance of the LzmaException class")]
        public LzmaException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        [System.ComponentModel.Description("Initializes a new instance of the LzmaException class")]
        public LzmaException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        [System.ComponentModel.Description("Initializes a new instance of the LzmaException class")]
        public LzmaException(string message, Exception inner) : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        [System.ComponentModel.Description("Initializes a new instance of the LzmaException class")]
        protected LzmaException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
