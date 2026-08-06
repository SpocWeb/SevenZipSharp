namespace SevenZip
{
    using System;

    /// <summary>
    /// The set of features supported by the library.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="None"/> | Default feature. |
    /// | 27 | <see cref="Extract7z"/> | The library can extract 7zip archives compressed with LZMA method. |
    /// | 31 | <see cref="Extract7zLZMA2"/> | The library can extract 7zip archives compressed with LZMA2 method. |
    /// | 35 | <see cref="Extract7zAll"/> | The library can extract 7z archives compressed with all known methods. |
    /// | 39 | <see cref="ExtractZip"/> | The library can extract zip archives. |
    /// | 43 | <see cref="ExtractRar"/> | The library can extract rar archives. |
    /// | 47 | <see cref="ExtractGzip"/> | The library can extract gzip archives. |
    /// | 51 | <see cref="ExtractBzip2"/> | The library can extract bzip2 archives. |
    /// | 55 | <see cref="ExtractTar"/> | The library can extract tar archives. |
    /// | 59 | <see cref="ExtractXz"/> | The library can extract xz archives. |
    /// | 63 | <see cref="ExtractAll"/> | The library can extract all types of archives supported. |
    /// | 67 | <see cref="Compress7z"/> | The library can compress data to 7zip archives with LZMA method. |
    /// | 71 | <see cref="Compress7zLZMA2"/> | The library can compress data to 7zip archives with LZMA2 method. |
    /// | 75 | <see cref="Compress7zAll"/> | The library can compress data to 7zip archives with all methods known. |
    /// | 79 | <see cref="CompressTar"/> | The library can compress data to tar archives. |
    /// | 83 | <see cref="CompressGzip"/> | The library can compress data to gzip archives. |
    /// | 87 | <see cref="CompressBzip2"/> | The library can compress data to bzip2 archives. |
    /// | 91 | <see cref="CompressXz"/> | The library can compress data to xz archives. |
    /// | 95 | <see cref="CompressZip"/> | The library can compress data to zip archives. |
    /// | 99 | <see cref="CompressAll"/> | The library can compress data to all types of archives supported. |
    /// | 103 | <see cref="Modify"/> | The library can modify archives. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2022-05-25T21:39:37Z
    /// digest: 5589d311440652e581fbcb84a7cce5df4f554c81ab94106f8a42bb611fe69bf0
    /// </code>
    /// </example>
    [Flags]
    [CLSCompliant(false)]
    public enum LibraryFeature : uint
    {
        /// <summary>
        /// Default feature.
        /// </summary>
        None = 0,
        /// <summary>
        /// The library can extract 7zip archives compressed with LZMA method.
        /// </summary>
        Extract7z = 0x1,
        /// <summary>
        /// The library can extract 7zip archives compressed with LZMA2 method.
        /// </summary>
        Extract7zLZMA2 = 0x2,
        /// <summary>
        /// The library can extract 7z archives compressed with all known methods.
        /// </summary>
        Extract7zAll = Extract7z|Extract7zLZMA2|0x4,
        /// <summary>
        /// The library can extract zip archives.
        /// </summary>
        ExtractZip = 0x8,
        /// <summary>
        /// The library can extract rar archives.
        /// </summary>
        ExtractRar = 0x10,
        /// <summary>
        /// The library can extract gzip archives.
        /// </summary>
        ExtractGzip = 0x20,
        /// <summary>
        /// The library can extract bzip2 archives.
        /// </summary>
        ExtractBzip2 = 0x40,
        /// <summary>
        /// The library can extract tar archives.
        /// </summary>
        ExtractTar = 0x80,
        /// <summary>
        /// The library can extract xz archives.
        /// </summary>
        ExtractXz = 0x100,
        /// <summary>
        /// The library can extract all types of archives supported.
        /// </summary>
        ExtractAll = Extract7zAll|ExtractZip|ExtractRar|ExtractGzip|ExtractBzip2|ExtractTar|ExtractXz,
        /// <summary>
        /// The library can compress data to 7zip archives with LZMA method.
        /// </summary>
        Compress7z = 0x200,
        /// <summary>
        /// The library can compress data to 7zip archives with LZMA2 method.
        /// </summary>
        Compress7zLZMA2 = 0x400,
        /// <summary>
        /// The library can compress data to 7zip archives with all methods known.
        /// </summary>
        Compress7zAll = Compress7z|Compress7zLZMA2|0x800,
        /// <summary>
        /// The library can compress data to tar archives.
        /// </summary>
        CompressTar = 0x1000,
        /// <summary>
        /// The library can compress data to gzip archives.
        /// </summary>
        CompressGzip = 0x2000,
        /// <summary>
        /// The library can compress data to bzip2 archives.
        /// </summary>
        CompressBzip2 = 0x4000,
        /// <summary>
        /// The library can compress data to xz archives.
        /// </summary>
        CompressXz = 0x8000,
        /// <summary>
        /// The library can compress data to zip archives.
        /// </summary>
        CompressZip = 0x10000,
        /// <summary>
        /// The library can compress data to all types of archives supported.
        /// </summary>
        CompressAll = Compress7zAll|CompressTar|CompressGzip|CompressBzip2|CompressXz|CompressZip,
        /// <summary>
        /// The library can modify archives.
        /// </summary>
        Modify = 0x20000
    }
}
