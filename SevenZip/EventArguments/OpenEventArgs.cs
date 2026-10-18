#if UNMANAGED

namespace SevenZip
{
    using System;

    /// <summary>
    /// EventArgs used to report the size of unpacked archive data
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 34 | <see cref="OpenEventArgs"/> | Initializes a new instance of the OpenEventArgs class |
    /// | 43 | <see cref="TotalSize"/> | Gets the size of unpacked archive data |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 3e1c058e45a275bfad8003133a02447f0d5c4fcd20e103b320312d2336a862f8
    /// </code>
    /// </example>
    public sealed class OpenEventArgs : EventArgs
    {
        private readonly ulong _totalSize;

        /// <summary>
        /// Initializes a new instance of the OpenEventArgs class
        /// </summary>
        /// <param name="totalSize">Size of unpacked archive data</param>
        [CLSCompliant(false)]
        public OpenEventArgs(ulong totalSize)
        {
            _totalSize = totalSize;
        }

        /// <summary>
        /// Gets the size of unpacked archive data
        /// </summary>
        [CLSCompliant(false)]
        public ulong TotalSize => _totalSize;
    }
}

#endif
