using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    /// <summary>Tests for seven Zip Extractor Asynchronous.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 16 | <see cref="AsynchronousExtractArchiveEventsTest"/> | Asynchronous Extract Archive Events Test. |
    /// | 75 | <see cref="AsynchronousExtractFileEventsTest"/> | Asynchronous Extract File Events Test. |
    /// | 108 | <see cref="AsynchronousExtractFilesEventsTest"/> | Asynchronous Extract Files Events Test. |
    /// | 139 | <see cref="ExtractArchiveAsync"/> | Extract Archive Async. |
    /// | 151 | <see cref="ExtractFileAsync_ByIndex"/> | Extract File Async — by Index. |
    /// | 167 | <see cref="ExtractFileAsync_ByFileName"/> | Extract File Async — by File Name. |
    /// | 183 | <see cref="ExtractFilesAsync_ByCallback"/> | Extract Files Async — by Callback. |
    /// | 195 | <see cref="ExtractFilesAsync_ByIndex"/> | Extract Files Async — by Index. |
    /// | 207 | <see cref="ExtractFilesAsync_ByFileName"/> | Extract Files Async — by File Name. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: fff1830b02b33e3b563ad797e03ec0353d38ee52623aaf8392011933bc9024ff
    /// </code>
    /// </example>
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

                extractingInvoked.ShouldBe(3);
                extractionFinishedInvoked.ShouldBe(1);
                fileExistsInvoked.ShouldBe(0);
                fileExtractionStartedInvoked.ShouldBe(3);
                fileExtractionFinishedInvoked.ShouldBe(3);

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

                fileExistsInvoked.ShouldBe(3);
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

            extractionFinishedInvoked.ShouldBe();
            File.ReadAllText(TemporaryFile).ShouldBe("file1");
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

            extractionFinishedInvoked.ShouldBe();
            Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
        }

        /// <summary>Extract Archive Async.</summary>
        [Test]
        public async Task ExtractArchiveAsync()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractArchiveAsync(OutputDirectory);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
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

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
            File.ReadAllText(TemporaryFile).ShouldBe("file1");
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

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
            File.ReadAllText(TemporaryFile).ShouldBe("file1");
        }

        /// <summary>Extract Files Async — by Callback.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByCallback()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\zip.zip"))
            {
                await extractor.ExtractFilesAsync(args => { args.ExtractToFile = TemporaryFile; });
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
        }

        /// <summary>Extract Files Async — by Index.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByIndex()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, 0, 2);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
        }

        /// <summary>Extract Files Async — by File Name.</summary>
        [Test]
        public async Task ExtractFilesAsync_ByFileName()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, "file1.txt", "file3.txt");
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
        }
    }
}
