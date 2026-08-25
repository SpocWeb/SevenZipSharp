using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    /// <summary>Tests for seven Zip Extractor Asynchronous.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 33 | <see cref="AsynchronousExtractArchiveEventsTest"/> | Asynchronous Extract Archive Events Test. |
    /// | 92 | <see cref="AsynchronousExtractFileEventsTest"/> | Asynchronous Extract File Events Test. |
    /// | 125 | <see cref="AsynchronousExtractFilesEventsTest"/> | Asynchronous Extract Files Events Test. |
    /// | 156 | <see cref="ExtractArchiveAsync"/> | Extract Archive Async. |
    /// | 168 | <see cref="ExtractFileAsync_ByIndex"/> | Extract File Async — by Index. |
    /// | 184 | <see cref="ExtractFileAsync_ByFileName"/> | Extract File Async — by File Name. |
    /// | 200 | <see cref="ExtractFilesAsync_ByCallback"/> | Extract Files Async — by Callback. |
    /// | 212 | <see cref="ExtractFilesAsync_ByIndex"/> | Extract Files Async — by Index. |
    /// | 224 | <see cref="ExtractFilesAsync_ByFileName"/> | Extract Files Async — by File Name. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-25T02:10:14Z", Digest = "fff1830b02b33e3b563ad797e03ec0353d38ee52623aaf8392011933bc9024ff", Stale = false, Path = "SevenZipExtractorAsynchronousTests.cs", Since = "2026-08-23")]
    [TestFixture, Ignore("Flaky tests, need to be re-written to run consistently in AppVeyor.")]
    public class SevenZipExtractorAsynchronousTests : TestBase
    {

        /// <summary>Asynchronous Extract Archive Events Test.</summary>
        [Test]
        public void AsynchronousExtractArchiveEventsTest()
        {
            var extractingInvoked = 0;
            var extractionFinishedInvoked = 0;
            var fileExistsInvoked = 0;
            var fileExtractionStartedInvoked = 0;
            var fileExtractionFinishedInvoked = 0;

            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                extractor.EventSynchronization = EventSynchronizationStrategy.AlwaysSynchronous;

                extractor.Extracting += (o, e) => extractingInvoked++;
                extractor.ExtractionFinished += (o, e) => extractionFinishedInvoked++;
                extractor.FileExists += (o, e) => fileExistsInvoked++;
                extractor.FileExtractionStarted += (o, e) => fileExtractionStartedInvoked++;
                extractor.FileExtractionFinished += (o, e) => fileExtractionFinishedInvoked++;

                extractor.BeginExtractArchive(OutputDirectory);

                var timeToWait = 1000;
                while (extractionFinishedInvoked == 0)
                {
                    if (timeToWait <= 0)
                    {
                        break;
                    }

                    Thread.Sleep(25);
                    timeToWait -= 25;
                }

                ClassicAssert.AreEqual(3, extractingInvoked);
                ClassicAssert.AreEqual(1, extractionFinishedInvoked);
                ClassicAssert.AreEqual(0, fileExistsInvoked);
                ClassicAssert.AreEqual(3, fileExtractionStartedInvoked);
                ClassicAssert.AreEqual(3, fileExtractionFinishedInvoked);

                extractionFinishedInvoked = 0;
                extractor.BeginExtractArchive(OutputDirectory);

                timeToWait = 1000;
                while (extractionFinishedInvoked == 0)
                {
                    if (timeToWait <= 0)
                    {
                        break;
                    }

                    Thread.Sleep(25);
                    timeToWait -= 25;
                }

                ClassicAssert.AreEqual(3, fileExistsInvoked);
            }
        }

        /// <summary>Asynchronous Extract File Events Test.</summary>
        [Test]
        public void AsynchronousExtractFileEventsTest()
        {
            var extractionFinishedInvoked = false;

            using (var fileStream = File.Create(TemporaryFile))
            {
                using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
                {
                    extractor.EventSynchronization = EventSynchronizationStrategy.AlwaysSynchronous;
                    extractor.ExtractionFinished += (o, e) => extractionFinishedInvoked = true;
                    extractor.BeginExtractFile(0, fileStream);

                    var maximumTimeToWait = 500;

                    while (!extractionFinishedInvoked)
                    {
                        if (maximumTimeToWait <= 0)
                        {
                            break;
                        }

                        Thread.Sleep(25);
                        maximumTimeToWait -= 25;
                    }
                }
            }

            ClassicAssert.IsTrue(extractionFinishedInvoked);
            ClassicAssert.AreEqual("file1", File.ReadAllText(TemporaryFile));
        }

        /// <summary>Asynchronous Extract Files Events Test.</summary>
        [Test]
        public void AsynchronousExtractFilesEventsTest()
        {
            var extractionFinishedInvoked = false;

            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                extractor.EventSynchronization = EventSynchronizationStrategy.AlwaysSynchronous;

                extractor.ExtractionFinished += (o, e) => extractionFinishedInvoked = true;

                extractor.BeginExtractFiles(OutputDirectory, 0, 2);

                var timeToWait = 250;
                while (!extractionFinishedInvoked)
                {
                    if (timeToWait <= 0)
                    {
                        break;
                    }

                    Thread.Sleep(25);
                    timeToWait -= 25;
                }
            }

            ClassicAssert.IsTrue(extractionFinishedInvoked);
            ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
        }

        /// <summary>Extract Archive Async.</summary>
        [Test]
        public async Task ExtractArchiveAsync()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractArchiveAsync(OutputDirectory);
            }

            ClassicAssert.AreEqual(3, Directory.GetFiles(OutputDirectory).Length);
        }

        /// <summary>Extract File Async — by Index.</summary>
        [Test]
        public async Task ExtractFileAsync_ByIndex()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                using (var fileStream = File.Create(TemporaryFile))
                {
                    await extractor.ExtractFileAsync(0, fileStream);
                }
            }

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.AreEqual("file1", File.ReadAllText(TemporaryFile));
        }

        /// <summary>Extract File Async — by File Name.</summary>
        [Test]
        public async Task ExtractFileAsync_ByFileName()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                using (var fileStream = File.Create(TemporaryFile))
                {
                    await extractor.ExtractFileAsync("file1.txt", fileStream);
                }
            }

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.AreEqual("file1", File.ReadAllText(TemporaryFile));
        }

        /// <summary>Extract Files Async — by Callback.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByCallback()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\zip.zip"))
            {
                await extractor.ExtractFilesAsync(args => { args.ExtractToFile = TemporaryFile; });
            }

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
        }

        /// <summary>Extract Files Async — by Index.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByIndex()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, 0, 2);
            }

            ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
        }

        /// <summary>Extract Files Async — by File Name.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByFileName()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, "file1.txt", "file3.txt");
            }

            ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
        }
    }
}
