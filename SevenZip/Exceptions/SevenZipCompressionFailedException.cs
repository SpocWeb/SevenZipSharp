#if UNMANAGED

namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for fail to create an archive in SevenZipCompressor.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="DEFAULT_MESSAGE"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 40 | <see cref="SevenZipCompressionFailedException"/> | Initializes a new instance of the SevenZipCompressionFailedException class |
    /// | 46 | <see cref="SevenZipCompressionFailedException"/> | Initializes a new instance of the SevenZipCompressionFailedException class |
    /// | 53 | <see cref="SevenZipCompressionFailedException"/> | Initializes a new instance of the SevenZipCompressionFailedException class |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 3cb5be017223e55d07dbdf815ff66aa8587c8bb6fb58bda8d954a576982d3ef7
    /// </code>
    /// </example>
    [Serializable]
    public class SevenZipCompressionFailedException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public const string DEFAULT_MESSAGE = "The compression has failed for an unknown reason with code ";

        /// <summary>
        /// Initializes a new instance of the SevenZipCompressionFailedException class
        /// </summary>
        public SevenZipCompressionFailedException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipCompressionFailedException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public SevenZipCompressionFailedException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipCompressionFailedException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public SevenZipCompressionFailedException(string message, Exception inner)
            : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipCompressionFailedException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected SevenZipCompressionFailedException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

#endif
