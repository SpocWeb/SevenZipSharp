namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    using NUnit.Framework.Legacy;

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

        [Test]
        public async Task ExtractArchiveAsync()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractArchiveAsync(OutputDirectory);
            }

            ClassicAssert.AreEqual(3, Directory.GetFiles(OutputDirectory).Length);
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

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.AreEqual("file1", File.ReadAllText(TemporaryFile));
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

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.AreEqual("file1", File.ReadAllText(TemporaryFile));
        }

        [Test]
        public async Task ExtractFilesAsync_ByCallback()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\zip.zip"))
            {
                await extractor.ExtractFilesAsync(args => { args.ExtractToFile = TemporaryFile; });
            }

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
        }

        [Test]
        public async Task ExtractFilesAsync_ByIndex()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                await extractor.ExtractFilesAsync(OutputDirectory, 0, 2);
            }

            ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
        }

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
