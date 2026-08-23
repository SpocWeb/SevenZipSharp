using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    /// <summary>
    /// EventArgs used to report the file information which is going to be packed.
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "097e5d4153c9d338237ae08235d531fed7d16a2a904e70c20e95de09caf7b56c", Stale = false, Path = "EventArguments/FileInfoEventArgs.cs", Since = "2026-08-23")]
    public sealed class FileInfoEventArgs : PercentDoneEventArgs, ICancellable
    {
        private readonly ArchiveFileInfo _fileInfo;

        /// <summary>
        /// Initializes a new instance of the FileInfoEventArgs class.
        /// </summary>
        /// <param name="fileInfo">The current ArchiveFileInfo.</param>
        /// <param name="percentDone">The percent of finished work.</param>
        [System.ComponentModel.Description("Initializes a new instance of the FileInfoEventArgs class.")]
        public FileInfoEventArgs(ArchiveFileInfo fileInfo, byte percentDone)
            : base(percentDone)
        {
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets whether to stop the current archive operation.")]
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether to skip the current file.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets whether to skip the current file.")]
        public bool Skip { get; set; }

        /// <summary>
        /// Gets the corresponding FileInfo to the event.
        /// </summary>
        [System.ComponentModel.Description("Gets the corresponding FileInfo to the event.")]
        public ArchiveFileInfo FileInfo => _fileInfo;
    }
}
