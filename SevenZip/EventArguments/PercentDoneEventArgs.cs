namespace SevenZip
{
    using System;

    /// <summary>
    /// EventArgs for storing PercentDone property.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 25 | <see cref="PercentDoneEventArgs"/> | Initializes a new instance of the PercentDoneEventArgs class. |
    /// | 38 | <see cref="PercentDone"/> | Gets the percent of finished work. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2023-02-21T22:10:02Z
    /// digest: fcc9dfbeac1504d9102c3708677713d7c201d569b8af5f88940bc1efc32df227
    /// </code>
    /// </example>
    public class PercentDoneEventArgs : EventArgs
    {
        private readonly byte _percentDone;

        /// <summary>
        /// Initializes a new instance of the PercentDoneEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public PercentDoneEventArgs(byte percentDone)
        {
            if (percentDone > 100 || percentDone < 0)
            {
                throw new ArgumentOutOfRangeException("percentDone",
                    "The percent of finished work must be between 0 and 100.");
            }
            _percentDone = percentDone;
        }

        /// <summary>
        /// Gets the percent of finished work.
        /// </summary>
        public byte PercentDone => _percentDone;

		/// <summary>
		/// Converts a [0, 1] rate to its percent equivalent.
		/// </summary>
		/// <param name="doneRate">The rate of the done work.</param>
		/// <returns>Percent integer equivalent.</returns>
		/// <exception cref="System.ArgumentException"/>
		internal static byte ProducePercentDone(float doneRate) => (byte) Math.Round(Math.Min(100 * doneRate, 100), MidpointRounding.AwayFromZero);
	}
}
