using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using SevenZip;

    using NUnit.Framework;

    [TestFixture]
    public class SevenZipExtractorTests : TestBase
    {
        public static List<TestFile> TestFiles
        {
            get
            {
                var result = new List<TestFile>();

                foreach (var file in Directory.GetFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData")))
                {
                    if (file.Contains("multi") || file.Contains("long_path"))
                    {
                        continue;
                    }

                    result.Add(new TestFile(file));
                }

                return result;
            }
        }

        [Test]
        public void ExtractFilesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                for (var i = 0; i < extractor.ArchiveFileData.Count; i++)
                {
                    extractor.ExtractFiles(OutputDirectory, extractor.ArchiveFileData[i].Index);
                }

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
            }
        }

        [Test]
        public void ExtractSpecificFilesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                extractor.ExtractFiles(OutputDirectory, 0, 2);
                Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
            Directory.GetFiles(OutputDirectory).ShouldContain(Path.Combine(OutputDirectory, "file1.txt"));
            Directory.GetFiles(OutputDirectory).ShouldContain(Path.Combine(OutputDirectory, "file3.txt"));
        }

        [Test]
        public void ExtractArchiveMultiVolumesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multivolume.part0001.rar"))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
            File.ReadAllText(Directory.GetFiles(OutputDirectory)[0]).StartsWith("Lorem ipsum dolor sit amet").ShouldBe();
        }

        [Test]
        public void ExtractionWithCancellationTest()
        {
            using (var tmp = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                tmp.FileExtractionStarted += (s, e) =>
                {
                    if (e.FileInfo.Index == 2)
                    {
                        e.Cancel = true;
                    }
                };
               
                tmp.ExtractArchive(OutputDirectory);

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
            }
        }

        [Test]
        public void ExtractionWithSkipTest()
        {
            using (var tmp = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                tmp.FileExtractionStarted += (s, e) =>
                                             {
                                                 if (e.FileInfo.Index == 1)
                                                 {
                                                     e.Skip = true;
                                                 }
                                             };

                tmp.ExtractArchive(OutputDirectory);

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
            }
        }

        [Test]
        public void ExtractionFromStreamTest()
        {
            // TODO: Rewrite this to test against more/all TestData archives.

            using (var tmp = new SevenZipExtractor(File.OpenRead(@"TestData\multiple_files.7z")))
            {
                tmp.ExtractArchive(OutputDirectory);
                Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
            }
        }

        [Test]
        public void ExtractionToStreamTest()
        {
            using (var tmp = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                using (var fileStream = new FileStream(Path.Combine(OutputDirectory, "streamed_file.txt"), FileMode.Create))
                {
                    tmp.ExtractFile(1, fileStream);
                }
            }

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);

            var extractedFile = Directory.GetFiles(OutputDirectory)[0];

            File.ReadAllText(extractedFile).ShouldBe("file2");
        }

        [Test]
        public void DetectMultiVolumeIndexTest()
        {
            using (var tmp = new SevenZipExtractor(@"TestData\multivolume.part0001.rar"))
            {
                tmp.ArchiveProperties.Any(x => x.Name.Equals("IsVolume") && x.Value != null && x.Value.Equals(true)).ShouldBe();
                tmp.ArchiveProperties.Any(x => x.Name.Equals("VolumeIndex") && x.Value != null && Convert.ToInt32(x.Value) == 0).ShouldBe();
            }

            using (var tmp = new SevenZipExtractor(@"TestData\multivolume.part0002.rar"))
            {
                tmp.ArchiveProperties.Any(x => x.Name.Equals("IsVolume") && x.Value != null && x.Value.Equals(true)).ShouldBe();
                tmp.ArchiveProperties.Any(x => x.Name.Equals("VolumeIndex") && x.Value != null && Convert.ToInt32(x.Value) == 0).ShouldNotBe();
            }
        }

        [Test]
        public void ThreadedExtractionTest()
        {
	        var destination1 = Path.Combine(OutputDirectory, "t1");
	        var destination2 = Path.Combine(OutputDirectory, "t2");

			var t1 = new Thread(() =>
            {
                using (var tmp = new SevenZipExtractor(@"TestData\multiple_files.7z"))
                {
                    tmp.ExtractArchive(destination1);
                }
            });
            var t2 = new Thread(() =>
            {
				using (var tmp = new SevenZipExtractor(@"TestData\multiple_files.7z"))
				{
                    tmp.ExtractArchive(destination2);
                }
            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

			Directory.Exists(destination1).ShouldBe();
	        Directory.Exists(destination2).ShouldBe();
			Directory.GetFiles(destination1).Length.ShouldBe(3);
	        Directory.GetFiles(destination2).Length.ShouldBe(3);
		}

        [Test]
        public void ExtractArchiveWithLongPath()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\long_path.7z"))
            {
                Assert.Throws<PathTooLongException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }

        [Test]
        public void ReadArchivedFileNames()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                var fileNames = extractor.ArchiveFileNames;
                fileNames.Count.ShouldBe(3);

                fileNames[0].ShouldBe("file1.txt");
                fileNames[1].ShouldBe("file2.txt");
                fileNames[2].ShouldBe("file3.txt");
            }
        }
        
        [Test]
        public void ReadArchivedFileData()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                var fileData = extractor.ArchiveFileData;
                fileData.Count.ShouldBe(3);

                fileData[0].FileName.ShouldBe("file1.txt");
                fileData[0].Encrypted.ShouldNotBe();
                fileData[0].IsDirectory.ShouldNotBe();
            }
        }

        [Test, TestCaseSource(nameof(TestFiles))]
        public void ExtractDifferentFormatsTest(TestFile file)
        {
            using (var extractor = new SevenZipExtractor(file.FilePath))
            {
                extractor.ExtractArchive(OutputDirectory);
                Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);
            }
        }
    }

    /// <summary>
    /// Simple wrapper to get better names for ExtractDifferentFormatsTest results.
    /// </summary>
    public class TestFile
    {
        public string FilePath { get; }

        public TestFile(string filePath)
        {
            FilePath = filePath;
        }

		public override string ToString() => Path.GetFileName(FilePath);
	}
}
