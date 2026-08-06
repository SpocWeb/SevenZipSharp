namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>Abstraction over a sliding input buffer that gives byte-level lookback/lookahead <br/>
    /// access to the data being compressed or decompressed.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 12 | <see cref="SetStream"/> | Attaches   as the source the window reads its data from. |
    /// | 15 | <see cref="Init"/> | Resets the window to the start of the attached stream and preloads the first block. |
    /// | 18 | <see cref="ReleaseStream"/> | Detaches the current input stream without disposing it. |
    /// | 21 | <see cref="GetIndexByte"/> | Returns the byte located   positions from the current position. |
    /// | 26 | <see cref="GetMatchLen"/> | Returns how many bytes starting at   match the bytes   found   positions earlier, up to   bytes. |
    /// | 29 | <see cref="GetNumAvailableBytes"/> | Returns the number of bytes still available for reading ahead of the current position. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Byte"/> | Returned by a method. |
    /// | <see cref="UInt32"/> | Returned by a method. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: f541f389fdafe883a2b5f26d6b7cd3469749b7d07ec7dfeae87951e832515b5c
    /// </code>
    /// </example>
    internal interface IInWindowStream
    {

        /// <summary>Attaches <paramref name='inStream'/> as the source the window reads its data from.</summary>
        void SetStream(Stream inStream);

        /// <summary>Resets the window to the start of the attached stream and preloads the first block.</summary>
        void Init();

        /// <summary>Detaches the current input stream without disposing it.</summary>
        void ReleaseStream();

        /// <summary>Returns the byte located <paramref name='index'/> positions from the current position.</summary>
        Byte GetIndexByte(Int32 index);

        /// <summary>Returns how many bytes starting at <paramref name='index'/> match the bytes <br/>
        /// found <paramref name='distance'/> positions earlier, up to <paramref name='limit'/> bytes.</summary>
        /// <returns>The length of the matching run, which may be 0 if no bytes match.</returns>
        UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);

        /// <summary>Returns the number of bytes still available for reading ahead of the current position.</summary>
        UInt32 GetNumAvailableBytes();
    }

    /// <summary>Finds previous occurrences of the byte sequence at the current position within the <br/>
    /// sliding window, so the encoder can emit LZ77-style back-references.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 40 | <see cref="Create"/> | Allocates the match finder's internal buffers sized to hold     bytes of history plus the requested look-behind and look-ahead margins. |
    /// | 46 | <see cref="GetMatches"/> | Locates matches for the byte sequence at the current position and appends each   match's (length, distance) pair to  , then advances the position by one byte. |
    /// | 50 | <see cref="Skip"/> | Advances the current position by   bytes, updating internal match   structures without returning any matches. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="UInt32"/> | Passed as a parameter. |
    /// </remarks>
    /// <inheritdoc cref="IInWindowStream"/>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 23a189417cb4a2ec4844c241f2e13441f836db3f59dcedabd15aadde035829ea
    /// </code>
    /// </example>
    internal interface IMatchFinder : IInWindowStream
    {

        /// <summary>Allocates the match finder's internal buffers sized to hold <paramref name='historySize'/> <br/>
        /// bytes of history plus the requested look-behind and look-ahead margins.</summary>
        void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                    UInt32 matchMaxLen, UInt32 keepAddBufferAfter);

        /// <summary>Locates matches for the byte sequence at the current position and appends each <br/>
        /// match's (length, distance) pair to <paramref name='distances'/>, then advances the position by one byte.</summary>
        /// <returns>The number of <see cref="UInt32"/> values written into <paramref name='distances'/>.</returns>
        UInt32 GetMatches(UInt32[] distances);

        /// <summary>Advances the current position by <paramref name='num'/> bytes, updating internal match <br/>
        /// structures without returning any matches.</summary>
        void Skip(UInt32 num);
    }
}