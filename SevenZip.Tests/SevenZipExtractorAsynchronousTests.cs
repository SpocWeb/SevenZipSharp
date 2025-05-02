using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture, Ignore("Flaky tests, need to be re-written to run consistently in AppVeyor.")]
    public class SevenZipExtractorAsynchronousTests : TestBase
    {
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

        [Test]
        public async Task ExtractArchiveAsync()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractArchiveAsync(OutputDirectory);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
        }

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

        [Test]
        public async Task ExtractFilesAsync_ByCallback()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\zip.zip"))
            {
                await extractor.ExtractFilesAsync(args => { args.ExtractToFile = TemporaryFile; });
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
        }

        [Test]
        public async Task ExtractFilesAsync_ByIndex()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, 0, 2);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
        }

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
