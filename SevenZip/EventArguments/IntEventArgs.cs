#if UNMANAGED

namespace SevenZip
{
    using System;

    /// <summary>
    /// Stores an int number
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 34 | <see cref="IntEventArgs"/> | Initializes a new instance of the IntEventArgs class |
    /// | 42 | <see cref="Value"/> | Gets the value of the IntEventArgs class |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 3d6f1a8169bfc317f174ae1359240cb6eb9d118ece68f35f95d9d5db7bbae2b7
    /// </code>
    /// </example>
    public sealed class IntEventArgs : EventArgs
    {
        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the IntEventArgs class
        /// </summary>
        /// <param name="value">Useful data carried by the IntEventArgs class</param>
        public IntEventArgs(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of the IntEventArgs class
        /// </summary>
        public int Value => _value;
    }
}

#endif
