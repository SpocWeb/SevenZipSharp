using System.ComponentModel;
namespace SevenZip
{
    using System;

    /// <summary>
    /// EventArgs for storing PercentDone property.
    /// </summary>
    public class PercentDoneEventArgs : EventArgs
    {
        private readonly byte _percentDone;

        /// <summary>
        /// Initializes a new instance of the PercentDoneEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        [System.ComponentModel.Description("Initializes a new instance of the PercentDoneEventArgs class.")]
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
        [System.ComponentModel.Description("Gets the percent of finished work.")]
        public byte PercentDone => _percentDone;

		/// <summary>
		/// Converts a [0, 1] rate to its percent equivalent.
		/// </summary>
		/// <param name="doneRate">The rate of the done work.</param>
		/// <returns>Percent integer equivalent.</returns>
		/// <exception cref="System.ArgumentException"/>
		[System.ComponentModel.Description("Converts a [0, 1] rate to its percent equivalent.")]
		internal static byte ProducePercentDone(float doneRate) => (byte) Math.Round(Math.Min(100 * doneRate, 100), MidpointRounding.AwayFromZero);
	}
}
