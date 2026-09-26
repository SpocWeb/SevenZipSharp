using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>Defines a windowed stream reader for compression algorithms to access specific bytes and match lengths.</summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Byte"/> | Individual bytes accessed from the buffered stream. |
    /// | <see cref="UInt32"/> | Distance values and match-length counts. |
    /// | <see cref="Stream"/> | The underlying input stream being buffered and examined. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 34 | <see cref="SetStream"/> | Sets the input stream to read from for pattern matching. |
    /// | 37 | <see cref="Init"/> | Initializes the stream reader. |
    /// | 40 | <see cref="ReleaseStream"/> | Releases the input stream. |
    /// | 44 | <see cref="GetIndexByte"/> | Gets the byte at the specified position in the input stream. |
    /// | 48 | <see cref="GetMatchLen"/> | Gets the length of the match starting at the specified position with the given distance constraint. |
    /// | 52 | <see cref="GetNumAvailableBytes"/> | Gets the number of bytes available to read from the current position. |
    /// </remarks>
    /// <seealso cref="Stream">Stream: the underlying input stream being buffered and examined.</seealso>
    [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
    [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
    [System.ComponentModel.Description("Defines a windowed stream reader for compression algorithms to access specific bytes and match lengths.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:51Z", Digest = "ef4f11648adcbf03af14b7db402f310dbb179418da0a8e762ae7d9f10e552aac", Stale = false, Path = "sdk/Compress/LZ/IMatchFinder.cs", Since = "2026-08-23")]
    [Concept("buffered")]
    [Concept("state_managed")]
    [Concept("position_tracking")]
    internal interface IInWindowStream
    {

        /// <summary>Sets the input stream to read from for pattern matching.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Sets the input stream to read from for pattern matching.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        void SetStream(Stream inStream);

        /// <summary>Initializes the stream reader.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Initializes the stream reader.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        void Init();

        /// <summary>Releases the input stream.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Releases the input stream.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        void ReleaseStream();

        /// <summary>Gets the byte at the specified position in the input stream.</summary>
        /// <returns>The byte value at the specified index.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Gets the byte at the specified position in the input stream.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        Byte GetIndexByte(Int32 index);

        /// <summary>Gets the length of the match starting at the specified position with the given distance constraint.</summary>
        /// <returns>The length of the match found.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Gets the length of the match starting at the specified position with the given distance constraint.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);

        /// <summary>Gets the number of bytes available to read from the current position.</summary>
        /// <returns>The count of available bytes.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/buffer_window", "code/lz_codec")]
        [System.ComponentModel.Description("Gets the number of bytes available to read from the current position.")]
        [Concept("buffered")]
        [Concept("state_managed")]
        [Concept("position_tracking")]
        UInt32 GetNumAvailableBytes();
    }

    /// <summary>Extends the windowed stream to find and return matching patterns for compression.</summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="IInWindowStream"/> | Base interface providing stream positioning and byte access. |
    /// | <see cref="UInt32"/> | Configuration and output parameters for pattern matching. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 78 | <see cref="Create"/> | Creates a match finder with the specified history size and buffer constraints. |
    /// | 83 | <see cref="GetMatches"/> | Finds and returns all matching positions, storing distances in the provided array. |
    /// | 86 | <see cref="Skip"/> | Advances the current position by the specified number of bytes. |
    /// </remarks>
    /// <seealso cref="IInWindowStream">IInWindowStream: base interface providing stream positioning and byte access.</seealso>
    [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
    [Tags("code/compression", "code/pattern_matching", "code/lz_codec", "code/match_finding")]
    [System.ComponentModel.Description("Extends the windowed stream to find and return matching patterns for compression.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:51Z", Digest = "3136ed5752cb7a4a1b12cecf5a41dfca607c0038a8c571d4e6655a8bdf2723ae", Stale = false, Path = "sdk/Compress/LZ/IMatchFinder.cs", Since = "2026-08-23")]
    [Concept("stateful")]
    [Concept("incremental")]
    [Concept("performance_critical")]
    internal interface IMatchFinder : IInWindowStream
    {

        /// <summary>Creates a match finder with the specified history size and buffer constraints.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/lz_codec", "code/match_finding")]
        [System.ComponentModel.Description("Creates a match finder with the specified history size and buffer constraints.")]
        [Concept("stateful")]
        [Concept("incremental")]
        [Concept("performance_critical")]
        void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                    UInt32 matchMaxLen, UInt32 keepAddBufferAfter);

        /// <summary>Finds and returns all matching positions, storing distances in the provided array.</summary>
        /// <returns>The number of matches found.</returns>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/lz_codec", "code/match_finding")]
        [System.ComponentModel.Description("Finds and returns all matching positions, storing distances in the provided array.")]
        [Concept("stateful")]
        [Concept("incremental")]
        [Concept("performance_critical")]
        UInt32 GetMatches(UInt32[] distances);

        /// <summary>Advances the current position by the specified number of bytes.</summary>
        [Facets(Layer = "transform", Status = "stable", Complexity = 1)]
        [Tags("code/compression", "code/pattern_matching", "code/lz_codec", "code/match_finding")]
        [System.ComponentModel.Description("Advances the current position by the specified number of bytes.")]
        [Concept("stateful")]
        [Concept("incremental")]
        [Concept("performance_critical")]
        void Skip(UInt32 num);
    }
}