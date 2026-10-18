#if UNMANAGED

namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for ArchiveUpdateCallback.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="DEFAULT_MESSAGE"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 40 | <see cref="CompressionFailedException"/> | Initializes a new instance of the CompressionFailedException class |
    /// | 46 | <see cref="CompressionFailedException"/> | Initializes a new instance of the CompressionFailedException class |
    /// | 53 | <see cref="CompressionFailedException"/> | Initializes a new instance of the CompressionFailedException class |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: e50c215a0f05d7134caff960051f2151863bae854c972d7cf89f20b6be1e9377
    /// </code>
    /// </example>
    [Serializable]
    public class CompressionFailedException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public const string DEFAULT_MESSAGE = "Could not pack files!";

        /// <summary>
        /// Initializes a new instance of the CompressionFailedException class
        /// </summary>
        public CompressionFailedException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the CompressionFailedException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public CompressionFailedException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the CompressionFailedException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public CompressionFailedException(string message, Exception inner) : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the CompressionFailedException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected CompressionFailedException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}

#endif