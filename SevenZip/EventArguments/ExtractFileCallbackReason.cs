#if UNMANAGED

namespace SevenZip
{
    /// <summary>
    /// The reason for calling <see cref="ExtractFileCallback"/>.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 30 | <see cref="Start"/> | ExtractFileCallback is called the first time for a file. |
    /// | 35 | <see cref="Done"/> | All data has been written to the target without any exceptions. |
    /// | 40 | <see cref="Failure"/> | An exception occured during extraction of the file. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 45a7f39417d934dfe74dd60c5f8d0de65cac9a0662f874399f0ebf575e526dc3
    /// </code>
    /// </example>
    public enum ExtractFileCallbackReason
    {
        /// <summary>
        /// <see cref="ExtractFileCallback"/> is called the first time for a file.
        /// </summary>
        Start,

        /// <summary>
        /// All data has been written to the target without any exceptions.
        /// </summary>
        Done,

        /// <summary>
        /// An exception occured during extraction of the file.
        /// </summary>
        Failure
    }
}

#endif
