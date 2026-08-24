using System.ComponentModel;
using org.SpocWeb.root.Attributes;
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
    /// | 26 | <see cref="ProgressEventArgs"/> | Initializes a new instance of the ProgressEventArgs class. |
    /// | 36 | <see cref="PercentDelta"/> | Gets the change in done work percentage. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "c241f573236c3c082b26e9c4c7445c861e714b933c652bc94ea2fb4dcf893bcc", Stale = false, Path = "EventArguments/ProgressEventArgs.cs", Since = "2026-08-23")]
    public sealed class ProgressEventArgs : PercentDoneEventArgs
    {
        private readonly byte _delta;

        /// <summary>
        /// Initializes a new instance of the ProgressEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <param name="percentDelta">The percent of work done after the previous event.</param>
        [System.ComponentModel.Description("Initializes a new instance of the ProgressEventArgs class.")]
        public ProgressEventArgs(byte percentDone, byte percentDelta)
            : base(percentDone)
        {
            _delta = percentDelta;
        }

        /// <summary>
        /// Gets the change in done work percentage.
        /// </summary>
        [System.ComponentModel.Description("Gets the change in done work percentage.")]
        public byte PercentDelta => _delta;
    }
}
