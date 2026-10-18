#if UNMANAGED

namespace SevenZip
{
    /// <summary>
    /// The way of the event synchronization.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 30 | <see cref="Default"/> | Events are called synchronously if user can do some action; that is, cancel the execution process for example. |
    /// | 34 | <see cref="AlwaysAsynchronous"/> | Always call events asynchronously. |
    /// | 38 | <see cref="AlwaysSynchronous"/> | Always call events synchronously. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 20c7a6e2d1a5d300544095620ddfa8fcd1e1b64ec54f21e280217ba35e2a858f
    /// </code>
    /// </example>
    public enum EventSynchronizationStrategy
    {
        /// <summary>
        /// Events are called synchronously if user can do some action; that is, cancel the execution process for example.
        /// </summary>
        Default,
        /// <summary>
        /// Always call events asynchronously.
        /// </summary>
        AlwaysAsynchronous,
        /// <summary>
        /// Always call events synchronously.
        /// </summary>
        AlwaysSynchronous
    }
}

#endif
