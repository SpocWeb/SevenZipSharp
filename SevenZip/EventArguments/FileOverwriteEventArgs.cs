#if UNMANAGED

namespace SevenZip
{
    using System;

    /// <summary>
    /// EventArgs for FileExists event, stores the file name and asks whether to overwrite it in case it already exists.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 33 | <see cref="FileOverwriteEventArgs"/> | Initializes a new instance of the FileOverwriteEventArgs class |
    /// | 41 | <see cref="Cancel"/> | Gets or sets the value indicating whether to cancel the extraction. |
    /// | 46 | <see cref="FileName"/> | Gets or sets the file name to extract to. Null means skip. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 2973ff5ab42578525e6a8073d6e34abc3764f03b74219b26b65c21e28ab29df8
    /// </code>
    /// </example>
    public sealed class FileOverwriteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the FileOverwriteEventArgs class
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public FileOverwriteEventArgs(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Gets or sets the value indicating whether to cancel the extraction.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the file name to extract to. Null means skip.
        /// </summary>
        public string FileName { get; set; }
    }
}

#endif