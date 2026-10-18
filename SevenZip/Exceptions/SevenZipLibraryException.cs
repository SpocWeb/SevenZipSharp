#if UNMANAGED

namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for 7-zip library operations.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="DEFAULT_MESSAGE"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 40 | <see cref="SevenZipLibraryException"/> | Initializes a new instance of the SevenZipLibraryException class |
    /// | 46 | <see cref="SevenZipLibraryException"/> | Initializes a new instance of the SevenZipLibraryException class |
    /// | 53 | <see cref="SevenZipLibraryException"/> | Initializes a new instance of the SevenZipLibraryException class |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 1c375fc1ba824bec3f9824718cc016bcca1c798f7276ed2f1e2c534e4ef00981
    /// </code>
    /// </example>
    [Serializable]
    public class SevenZipLibraryException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public const string DEFAULT_MESSAGE = "Can not load 7-zip library or internal COM error!";

        /// <summary>
        /// Initializes a new instance of the SevenZipLibraryException class
        /// </summary>
        public SevenZipLibraryException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipLibraryException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public SevenZipLibraryException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipLibraryException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public SevenZipLibraryException(string message, Exception inner) : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipLibraryException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected SevenZipLibraryException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

#endif
