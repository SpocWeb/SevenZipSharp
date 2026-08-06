namespace SevenZip
{
    /// <summary>
    /// The EventArgs class for accurate progress handling.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="ProgressEventArgs"/> | Initializes a new instance of the ProgressEventArgs class. |
    /// | 32 | <see cref="PercentDelta"/> | Gets the change in done work percentage. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: c241f573236c3c082b26e9c4c7445c861e714b933c652bc94ea2fb4dcf893bcc
    /// </code>
    /// </example>
    public sealed class ProgressEventArgs : PercentDoneEventArgs
    {
        private readonly byte _delta;

        /// <summary>
        /// Initializes a new instance of the ProgressEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <param name="percentDelta">The percent of work done after the previous event.</param>
        public ProgressEventArgs(byte percentDone, byte percentDelta)
            : base(percentDone)
        {
            _delta = percentDelta;
        }

        /// <summary>
        /// Gets the change in done work percentage.
        /// </summary>
        public byte PercentDelta => _delta;
    }
}
