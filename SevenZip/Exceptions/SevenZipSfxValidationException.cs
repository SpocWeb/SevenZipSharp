namespace SevenZip
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception class for 7-zip sfx settings validation.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="DefaultMessage"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 28 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
    /// | 34 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
    /// | 41 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
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
    /// digest: f5b7081dd8ff92fdc4e070f72c021ddf797a582ca574e19ecabbe8731154074a
    /// </code>
    /// </example>
    [Serializable]
    public class SevenZipSfxValidationException : SevenZipException
    {
        /// <summary>
        /// Exception dafault message which is displayed if no extra information is specified
        /// </summary>
        public static readonly string DefaultMessage = "Sfx settings validation failed.";

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        public SevenZipSfxValidationException() : base(DefaultMessage) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        public SevenZipSfxValidationException(string message) : base(DefaultMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        public SevenZipSfxValidationException(string message, Exception inner) : base(DefaultMessage, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        protected SevenZipSfxValidationException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
