using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Tests that custom compression parameters are correctly applied<br/>
    /// across different archive formats and compression methods.
    /// </summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Class | Relationship |
    /// |---|---|
    /// | <see cref="SevenZipCompressor"/> | Subject under test. |
    /// | <see cref="TestBase"/> | Base class providing test infrastructure. |
    ///
    /// ## Invariants
    /// Each test validates parameter acceptance for a specific compression<br/>
    /// method, verifying both successful application and error handling of<br/>
    /// invalid parameters.
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 47 | <see cref="CompressWithCustomParameters_OnlyWorksWithCorrectMethod"/> | Compress With Custom Parameters — only Works With Correct Method. |
    /// | 66 | <see cref="InvalidCustomParameters_Throws"/> | Invalid Custom Parameters — throws. |
    /// | 83 | <see cref="Zip_Deflate_WithCustomParameters"/> | Zip — deflate — with Custom Parameters. |
    /// | 104 | <see cref="Zip_Deflate64_WithCustomParameters"/> | Zip — deflate64 — with Custom Parameters. |
    /// | 125 | <see cref="Zip_PPMd_WithCustomParameters"/> | Zip — pp Md — with Custom Parameters. |
    /// | 141 | <see cref="Zip_BZip2_WithCustomParameters"/> | Zip — b Zip2 — with Custom Parameters. |
    /// | 159 | <see cref="SevenZip_Default_WithCustomParameters"/> | Seven Zip — default — with Custom Parameters. |
    /// | 185 | <see cref="SevenZip_Lzma_WithCustomParameters"/> | Seven Zip — lzma — with Custom Parameters. |
    /// | 207 | <see cref="SevenZip_Lzma2_WithCustomParameters"/> | Seven Zip — lzma2 — with Custom Parameters. |
    ///
    /// See https://sevenzip.osdn.jp/chm/cmdline/switches/method.htm for parameter details.
    /// </remarks>
    /// <seealso cref="SevenZipCompressor">SevenZipCompressor: provides compression functionality with custom parameter support.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T14:17:17Z", Digest = "c01abb6091bf59ec49d3b4eb9ca8d438d4f6730537ebb83cb12d6ef5b5540f5b", Stale = false, Path = "SevenZipCompressorCustomParameterTests.cs", Since = "2026-08-23")]
    [TestFixture]
    public class SevenZipCompressorCustomParameterTests : TestBase
    {

        /// <summary>Compress With Custom Parameters — only Works With Correct Method.</summary>
        [Test]
        public void CompressWithCustomParameters_OnlyWorksWithCorrectMethod()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMethod = CompressionMethod.Lzma
            };

            // Check parameters for PPMd compression.
            compressor.CustomParameters.Add("mem", "25");
            Assert.Throws<CompressionFailedException>(() => compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip"));
            compressor.CustomParameters.Remove("mem");
            compressor.CustomParameters.Add("o", "10");
            Assert.Throws<CompressionFailedException>(() => compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip"));
            compressor.CustomParameters.Remove("o");
        }

        /// <summary>Invalid Custom Parameters — throws.</summary>
        [Test]
        public void InvalidCustomParameters_Throws()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip
            };

            compressor.CustomParameters.Add("x", "3");
            Assert.Throws<CompressionFailedException>(() => compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip"));
            compressor.CustomParameters.Remove("x");

            compressor.CustomParameters.Add("em", "AES128");
            Assert.Throws<CompressionFailedException>(() => compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip"));
        }

        /// <summary>Zip — deflate — with Custom Parameters.</summary>
        [Test]
        public void Zip_Deflate_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMethod = CompressionMethod.Deflate
            };

            compressor.CustomParameters.Add("fb", "4");
            compressor.CustomParameters.Add("pass", "4");
            compressor.CustomParameters.Add("mt", "off");
            compressor.CustomParameters.Add("tc", "off");
            compressor.CustomParameters.Add("cl", "on");
            compressor.CustomParameters.Add("cu", "on");
            compressor.CustomParameters.Add("cp", "866");

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Zip — deflate64 — with Custom Parameters.</summary>
        [Test]
        public void Zip_Deflate64_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMethod = CompressionMethod.Deflate64
            };

            compressor.CustomParameters.Add("fb", "4");
            compressor.CustomParameters.Add("pass", "4");
            compressor.CustomParameters.Add("mt", "off");
            compressor.CustomParameters.Add("tc", "off");
            compressor.CustomParameters.Add("cl", "on");
            compressor.CustomParameters.Add("cu", "on");
            compressor.CustomParameters.Add("cp", "866");

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Zip — pp Md — with Custom Parameters.</summary>
        [Test]
        public void Zip_PPMd_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMethod = CompressionMethod.Ppmd
            };

            compressor.CustomParameters.Add("mem", "128m");
            compressor.CustomParameters.Add("o", "9");

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Zip — b Zip2 — with Custom Parameters.</summary>
        [Test]
        public void Zip_BZip2_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMethod = CompressionMethod.BZip2
            };

            compressor.CustomParameters.Add("d", "1048576b");
            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");

            compressor.CustomParameters.Remove("d");
            compressor.CustomParameters.Add("d", "16");
            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Seven Zip — default — with Custom Parameters.</summary>
        [Test]
        public void SevenZip_Default_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                CompressionMethod = CompressionMethod.Default
            };

            compressor.CustomParameters.Add("yx", "7");
            compressor.CustomParameters.Add("s", "off");
            compressor.CustomParameters.Add("qs", "on");
            compressor.CustomParameters.Add("f", "off");
            compressor.CustomParameters.Add("hc", "off");
            compressor.CustomParameters.Add("he", "on");
            compressor.CustomParameters.Add("mt", "off");
            compressor.CustomParameters.Add("mtf", "off");
            compressor.CustomParameters.Add("tm", "off");
            compressor.CustomParameters.Add("tc", "on");
            compressor.CustomParameters.Add("ta", "on");
            compressor.CustomParameters.Add("tr", "off");

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Seven Zip — lzma — with Custom Parameters.</summary>
        [Test]
        public void SevenZip_Lzma_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                CompressionMethod = CompressionMethod.Lzma
            };

            compressor.CustomParameters.Add("a", "0");
            compressor.CustomParameters.Add("d", "25");
            compressor.CustomParameters.Add("mf", "bt3");
            compressor.CustomParameters.Add("fb", "24");
            compressor.CustomParameters.Add("mc", "24");
            compressor.CustomParameters.Add("lc", "4");
            compressor.CustomParameters.Add("lp", "1");
            compressor.CustomParameters.Add("pb", "3");
            
            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }

        /// <summary>Seven Zip — lzma2 — with Custom Parameters.</summary>
        [Test]
        public void SevenZip_Lzma2_WithCustomParameters()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                CompressionMethod = CompressionMethod.Lzma2
            };

            compressor.CustomParameters.Add("c", "512m");

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");
        }
    }
}
