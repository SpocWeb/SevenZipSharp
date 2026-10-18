#if UNMANAGED

namespace SevenZip
{
    /// <summary>
    /// Archive compression mode.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 29 | <see cref="Create"/> | Create a new archive; overwrite the existing one. |
    /// | 33 | <see cref="Append"/> | Add data to the archive. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 30888c5c262a58d154b055682e0a16adabe4fb2f231f75835c8ddb87cf32c0e2
    /// </code>
    /// </example>
    public enum CompressionMode
    {
        /// <summary>
        /// Create a new archive; overwrite the existing one.
        /// </summary>
        Create,
        /// <summary>
        /// Add data to the archive.
        /// </summary>
        Append,
    }
}

#endif
