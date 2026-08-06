namespace SevenZip
{
    /// <summary>
    /// The definition of the interface which supports the cancellation of a process.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 19 | <see cref="Cancel"/> | Gets or sets whether to stop the current archive operation. |
    /// | 24 | <see cref="Skip"/> | Gets or sets whether to skip the current file. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: 21d6c03f693b8217e6803e9bcca91b49a5aa00d99f12f2fc67548b2731d6d120
    /// </code>
    /// </example>
    public interface ICancellable
    {
        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether to skip the current file.
        /// </summary>
        bool Skip { get; set; }
    }
}
