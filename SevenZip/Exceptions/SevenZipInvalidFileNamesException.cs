#if UNMANAGED

namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for empty common root if file name array in SevenZipCompressor.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="DEFAULT_MESSAGE"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 40 | <see cref="SevenZipInvalidFileNamesException"/> | Initializes a new instance of the SevenZipInvalidFileNamesException class |
    /// | 46 | <see cref="SevenZipInvalidFileNamesException"/> | Initializes a new instance of the SevenZipInvalidFileNamesException class |
    /// | 53 | <see cref="SevenZipInvalidFileNamesException"/> | Initializes a new instance of the SevenZipInvalidFileNamesException class |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: a744ee3c411d0a9afd9ee960584d1c7b29c48dc42c9e3b4122dd2277144f1336
    /// </code>
    /// </example>
    [Serializable]
    public class SevenZipInvalidFileNamesException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public const string DEFAULT_MESSAGE = "Invalid file names have been specified: ";

        /// <summary>
        /// Initializes a new instance of the SevenZipInvalidFileNamesException class
        /// </summary>
        public SevenZipInvalidFileNamesException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipInvalidFileNamesException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public SevenZipInvalidFileNamesException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipInvalidFileNamesException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public SevenZipInvalidFileNamesException(string message, Exception inner) : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipInvalidFileNamesException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected SevenZipInvalidFileNamesException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

#endif
