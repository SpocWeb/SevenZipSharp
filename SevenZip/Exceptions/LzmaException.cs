namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for LZMA operations.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="DEFAULT_MESSAGE"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 28 | <see cref="LzmaException"/> | Initializes a new instance of the LzmaException class |
    /// | 34 | <see cref="LzmaException"/> | Initializes a new instance of the LzmaException class |
    /// | 41 | <see cref="LzmaException"/> | Initializes a new instance of the LzmaException class |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="SerializationInfo"/> | Passed as a parameter. |
    /// | <see cref="StreamingContext"/> | Passed as a parameter. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: a3c323420e890497054cb6eaa7722a0fcfb96306c7a1dd0c1d866057fd00f94d
    /// </code>
    /// </example>
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
        public LzmaException() : base(DEFAULT_MESSAGE) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public LzmaException(string message) : base(DEFAULT_MESSAGE, message) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public LzmaException(string message, Exception inner) : base(DEFAULT_MESSAGE, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the LzmaException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected LzmaException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
