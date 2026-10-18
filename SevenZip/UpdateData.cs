#if UNMANAGED

namespace SevenZip
{
    using System.Collections.Generic;

    /// <summary>
    /// Archive update data for UpdateCallback.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="FileNamesToModify"/> | Gets or sets the file Names To Modify. |
    /// | 25 | <see cref="ArchiveFileData"/> | Gets or sets the archive File Data. |
    /// </remarks>
    internal struct UpdateData
    {
        public uint FilesCount;
        public InternalCompressionMode Mode;

        public IDictionary<int, string> FileNamesToModify { get; set; }

        public List<ArchiveFileInfo> ArchiveFileData { get; set; }
    }
}

#endif
