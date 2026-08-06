namespace SevenZip
{
    /// <summary>
    /// EventArgs used to report the file information which is going to be packed.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="FileInfoEventArgs"/> | Initializes a new instance of the FileInfoEventArgs class. |
    /// | 32 | <see cref="Cancel"/> | Gets or sets whether to stop the current archive operation. |
    /// | 37 | <see cref="Skip"/> | Gets or sets whether to skip the current file. |
    /// | 42 | <see cref="FileInfo"/> | Gets the corresponding FileInfo to the event. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="ArchiveFileInfo"/> | Used as a field. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 097e5d4153c9d338237ae08235d531fed7d16a2a904e70c20e95de09caf7b56c
    /// </code>
    /// </example>
    public sealed class FileInfoEventArgs : PercentDoneEventArgs, ICancellable
    {
        private readonly ArchiveFileInfo _fileInfo;

        /// <summary>
        /// Initializes a new instance of the FileInfoEventArgs class.
        /// </summary>
        /// <param name="fileInfo">The current ArchiveFileInfo.</param>
        /// <param name="percentDone">The percent of finished work.</param>
        public FileInfoEventArgs(ArchiveFileInfo fileInfo, byte percentDone)
            : base(percentDone)
        {
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether to skip the current file.
        /// </summary>
        public bool Skip { get; set; }

        /// <summary>
        /// Gets the corresponding FileInfo to the event.
        /// </summary>
        public ArchiveFileInfo FileInfo => _fileInfo;
    }
}
