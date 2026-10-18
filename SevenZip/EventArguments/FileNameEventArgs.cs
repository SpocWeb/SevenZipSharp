#if UNMANAGED

namespace SevenZip
{
    using System;

    /// <summary>
    /// EventArgs class which stores the file name.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 35 | <see cref="FileNameEventArgs"/> | Initializes a new instance of the FileNameEventArgs class. |
    /// | 58 | <see cref="FileName"/> | Gets the file name. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 0bc68123074526de9ceceb6218b611008b145f19abe53ee25723c91946a585d7
    /// </code>
    /// </example>
    public sealed class FileNameEventArgs : PercentDoneEventArgs, ICancellable
    {
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the FileNameEventArgs class.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="percentDone">The percent of finished work</param>
        public FileNameEventArgs(string fileName, byte percentDone) :
            base(percentDone)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        public bool Skip
        {
            get => false;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName => _fileName;
    }
}

#endif