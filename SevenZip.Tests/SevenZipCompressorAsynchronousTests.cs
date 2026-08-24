using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    /// <summary>Tests for seven Zip Compressor Asynchronous.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 33 | <see cref="AsynchronousCompressDirectoryAndEventsTest"/> | Asynchronous Compress Directory And Events Test. |
    /// | 76 | <see cref="AsynchronousCompressFilesTest"/> | Asynchronous Compress Files Test. |
    /// | 109 | <see cref="AsynchronousCompressStreamTest"/> | Asynchronous Compress Stream Test. |
    /// | 146 | <see cref="AsynchronousModifyArchiveTest"/> | Asynchronous Modify Archive Test. |
    /// | 180 | <see cref="AsynchronousCompressFilesEncryptedTest"/> | Asynchronous Compress Files Encrypted Test. |
    /// | 215 | <see cref="CompressFilesAsync"/> | Compress Files Async. |
    /// | 232 | <see cref="CompressDirectoryAsync"/> | Compress Directory Async. |
    /// | 249 | <see cref="CompressFilesEncryptedAsync"/> | Compress Files Encrypted Async. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:39:12Z", Digest = "443dd3b5eaeb0c19dbda88700012ef2cbd96e07d18c2601caf18a74cc340046c", Stale = false, Path = "SevenZipCompressorAsynchronousTests.cs", Since = "2026-08-23")]
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

            ClassicAssert.AreEqual(1, filesFoundInvoked);
            ClassicAssert.AreEqual(numberOfTestDataFiles, fileCompressionStartedInvoked);
            ClassicAssert.AreEqual(numberOfTestDataFiles, fileCompressionFinishedInvoked);
            ClassicAssert.AreEqual(numberOfTestDataFiles, compressingInvoked);
            ClassicAssert.AreEqual(1, compressionFinishedInvoked);

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(2, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("zip.zip"));
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("tar.tar"));
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual("tartar", extractor.ArchiveFileNames[0]);
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(2, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("zip.zip"));
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("tar.tar"));

                Assert.Throws<ExtractionFailedException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }

        /// <summary>Compress Files Async.</summary>
        [Test]
        public async Task CompressFilesAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressFilesAsync(TemporaryFile, @"TestData\zip.zip", @"TestData\tar.tar");

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(2, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("zip.zip"));
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("tar.tar"));
            }
        }

        /// <summary>Compress Directory Async.</summary>
        [Test]
        public async Task CompressDirectoryAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressDirectoryAsync("TestData", TemporaryFile);

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(Directory.GetFiles("TestData").Length, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("zip.zip"));
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("tar.tar"));
            }
        }

        /// <summary>Compress Files Encrypted Async.</summary>
        [Test]
        public async Task CompressFilesEncryptedAsync()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };
            await compressor.CompressFilesEncryptedAsync(TemporaryFile, "secure", @"TestData\zip.zip", @"TestData\tar.tar");

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile, "insecure"))
            {
                ClassicAssert.AreEqual(2, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("zip.zip"));
                ClassicAssert.IsTrue(extractor.ArchiveFileNames.Contains("tar.tar"));

                Assert.Throws<ExtractionFailedException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }
    }
}
