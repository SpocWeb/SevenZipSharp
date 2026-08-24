using System.ComponentModel;
using org.SpocWeb.root.Attributes;
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
    /// | 35 | <see cref="DefaultMessage"/> | Exception dafault message which is displayed if no extra information is specified |
    /// | 40 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
    /// | 47 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
    /// | 55 | <see cref="SevenZipSfxValidationException"/> | Initializes a new instance of the SevenZipSfxValidationException class |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="SerializationInfo"/> | Passed as a parameter. |
    /// | <see cref="StreamingContext"/> | Passed as a parameter. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "f5b7081dd8ff92fdc4e070f72c021ddf797a582ca574e19ecabbe8731154074a", Stale = false, Path = "Exceptions/SevenZipSfxValidationException.cs", Since = "2026-08-23")]
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
        [System.ComponentModel.Description("Initializes a new instance of the SevenZipSfxValidationException class")]
        public SevenZipSfxValidationException() : base(DefaultMessage) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        [System.ComponentModel.Description("Initializes a new instance of the SevenZipSfxValidationException class")]
        public SevenZipSfxValidationException(string message) : base(DefaultMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="message">Additional detailed message</param>
        /// <param name="inner">Inner exception occured</param>
        [System.ComponentModel.Description("Initializes a new instance of the SevenZipSfxValidationException class")]
        public SevenZipSfxValidationException(string message, Exception inner) : base(DefaultMessage, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SevenZipSfxValidationException class
        /// </summary>
        /// <param name="info">All data needed for serialization or deserialization</param>
        /// <param name="context">Serialized stream descriptor</param>
        [System.ComponentModel.Description("Initializes a new instance of the SevenZipSfxValidationException class")]
        protected SevenZipSfxValidationException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
