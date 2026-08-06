using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    /// <summary>Tests for seven Zip Compressor Asynchronous.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 17 | <see cref="AsynchronousCompressDirectoryAndEventsTest"/> | Asynchronous Compress Directory And Events Test. |
    /// | 60 | <see cref="AsynchronousCompressFilesTest"/> | Asynchronous Compress Files Test. |
    /// | 93 | <see cref="AsynchronousCompressStreamTest"/> | Asynchronous Compress Stream Test. |
    /// | 130 | <see cref="AsynchronousModifyArchiveTest"/> | Asynchronous Modify Archive Test. |
    /// | 164 | <see cref="AsynchronousCompressFilesEncryptedTest"/> | Asynchronous Compress Files Encrypted Test. |
    /// | 199 | <see cref="CompressFilesAsync"/> | Compress Files Async. |
    /// | 216 | <see cref="CompressDirectoryAsync"/> | Compress Directory Async. |
    /// | 233 | <see cref="CompressFilesEncryptedAsync"/> | Compress Files Encrypted Async. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 443dd3b5eaeb0c19dbda88700012ef2cbd96e07d18c2601caf18a74cc340046c
    /// </code>
    /// </example>
    [TestFixture]
    public class SevenZipCompressorAsynchronousTests : TestBase
    {

        /// <summary>Asynchronous Compress Directory And Events Test.</summary>
        [Test]
        public void AsynchronousCompressDirectoryAndEventsTest()
        {
            var filesFoundInvoked = 0;
            var fileCompressionStartedInvoked = 0;
            var fileCompressionFinishedInvoked = 0;
            var compressingInvoked = 0;
            var compressionFinishedInvoked = 0;

            var compressor = new SevenZipCompressor();

            compressor.FilesFound += (o, e) => filesFoundInvoked++;
            compressor.FileCompressionStarted += (o, e) => fileCompressionStartedInvoked++;
            compressor.FileCompressionFinished += (o, e) => fileCompressionFinishedInvoked++;
            compressor.Compressing += (o, e) => compressingInvoked++;
            compressor.CompressionFinished += (o, e) => compressionFinishedInvoked++;

            compressor.BeginCompressDirectory(@"TestData", TemporaryFile);

            var timeToWait = 1000;
            while (compressionFinishedInvoked == 0)
            {
                if (timeToWait <= 0)
                {
                    break;
                }

                Thread.Sleep(25);
                timeToWait -= 25;
            }

            var numberOfTestDataFiles = Directory.GetFiles("TestData").Length;

            filesFoundInvoked.ShouldBe(1);
            fileCompressionStartedInvoked.ShouldBe(numberOfTestDataFiles);
            fileCompressionFinishedInvoked.ShouldBe(numberOfTestDataFiles);
            compressingInvoked.ShouldBe(numberOfTestDataFiles);
            compressionFinishedInvoked.ShouldBe(1);

            File.Exists(TemporaryFile).ShouldBe();
        }

        /// <summary>Asynchronous Compress Files Test.</summary>
        [Test]
        public void AsynchronousCompressFilesTest()
        {
            var compressionFinishedInvoked = false;

            var compressor = new SevenZipCompressor {DirectoryStructure = false};
            compressor.CompressionFinished += (o, e) => compressionFinishedInvoked = true;

            compressor.BeginCompressFiles(TemporaryFile, @"TestData\zip.zip", @"TestData\tar.tar");

            var timeToWait = 1000;
            while (!compressionFinishedInvoked)
            {
                if (timeToWait <= 0)
                {
                    break;
                }

                Thread.Sleep(25);
                timeToWait -= 25;
            }

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(2u);
                extractor.ArchiveFileNames.ShouldContain("zip.zip");
                extractor.ArchiveFileNames.ShouldContain("tar.tar");
            }
        }

        /// <summary>Asynchronous Compress Stream Test.</summary>
        [Test]
        public void AsynchronousCompressStreamTest()
        {
            var compressionFinishedInvoked = false;

            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            compressor.CompressionFinished += (o, e) => compressionFinishedInvoked = true;

            using (var inputStream = File.OpenRead(@"TestData\zip.zip"))
            {
                using (var outputStream = new FileStream(TemporaryFile, FileMode.Create))
                {
                    compressor.BeginCompressStream(inputStream, outputStream);

                    var timeToWait = 1000;
                    while (!compressionFinishedInvoked)
                    {
                        if (timeToWait <= 0)
                        {
                            break;
                        }

                        Thread.Sleep(25);
                        timeToWait -= 25;
                    }
                }
            }

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
            }
        }

        /// <summary>Asynchronous Modify Archive Test.</summary>
        [Test]
        public void AsynchronousModifyArchiveTest()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };

            compressor.CompressFiles(TemporaryFile, @"TestData\tar.tar");

            var compressionFinishedInvoked = false;
            compressor.CompressionFinished += (o, e) => compressionFinishedInvoked = true;

            compressor.BeginModifyArchive(TemporaryFile, new Dictionary<int, string>{{0, @"tartar"}});

            var timeToWait = 1000;
            while (!compressionFinishedInvoked)
            {
                if (timeToWait <= 0)
                {
                    break;
                }

                Thread.Sleep(25);
                timeToWait -= 25;
            }

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldBe("tartar");
            }
        }

        /// <summary>Asynchronous Compress Files Encrypted Test.</summary>
        [Test]
        public void AsynchronousCompressFilesEncryptedTest()
        {
            var compressionFinishedInvoked = false;

            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            compressor.CompressionFinished += (o, e) => compressionFinishedInvoked = true;

            compressor.BeginCompressFilesEncrypted(TemporaryFile, "secure", @"TestData\zip.zip", @"TestData\tar.tar");

            var timeToWait = 1000;
            while (!compressionFinishedInvoked)
            {
                if (timeToWait <= 0)
                {
                    break;
                }

                Thread.Sleep(25);
                timeToWait -= 25;
            }

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(2u);
                extractor.ArchiveFileNames.ShouldContain("zip.zip");
                extractor.ArchiveFileNames.ShouldContain("tar.tar");

                Assert.Throws<ExtractionFailedException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }

        /// <summary>Compress Files Async.</summary>
        [Test]
        public async Task CompressFilesAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressFilesAsync(TemporaryFile, @"TestData\zip.zip", @"TestData\tar.tar");

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(2u);
                extractor.ArchiveFileNames.Contains("zip.zip").ShouldBe();
                extractor.ArchiveFileNames.Contains("tar.tar").ShouldBe();
            }
        }

        /// <summary>Compress Directory Async.</summary>
        [Test]
        public async Task CompressDirectoryAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressDirectoryAsync("TestData", TemporaryFile);

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe((uint)Directory.GetFiles("TestData").Length);
                extractor.ArchiveFileNames.Contains("zip.zip").ShouldBe();
                extractor.ArchiveFileNames.ShouldContain("tar.tar");
            }
        }

        /// <summary>Compress Files Encrypted Async.</summary>
        [Test]
        public async Task CompressFilesEncryptedAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressFilesEncryptedAsync(TemporaryFile, "secure", @"TestData\zip.zip", @"TestData\tar.tar");

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile, "insecure"))
            {
                extractor.FilesCount.ShouldBe(2u);
                extractor.ArchiveFileNames.ShouldContain("zip.zip");
                extractor.ArchiveFileNames.ShouldContain("tar.tar");

                Assert.Throws<ExtractionFailedException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }
    }
}
