using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using SevenZip;

    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    /// <summary>Tests for seven Zip Extractor.</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T10:53:00Z", Digest = "dde28f0e288e410981650752ea40bd740da8ebcf478f6fd83adba1e228529739", Stale = false, Path = "SevenZipExtractorTests.cs", Since = "2026-08-23")]
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

        /// <summary>Extract Files Test.</summary>
        [Test]
        public void ExtractFilesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                for (var i = 0; i < extractor.ArchiveFileData.Count; i++)
                {
                    extractor.ExtractFiles(OutputDirectory, extractor.ArchiveFileData[i].Index);
                }

                ClassicAssert.AreEqual(3, Directory.GetFiles(OutputDirectory).Length);
            }
        }

        /// <summary>Extract Specific Files Test.</summary>
        [Test]
        public void ExtractSpecificFilesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                extractor.ExtractFiles(OutputDirectory, 0, 2);
                ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
            }

            ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.Contains(Path.Combine(OutputDirectory, "file1.txt"), Directory.GetFiles(OutputDirectory));
            ClassicAssert.Contains(Path.Combine(OutputDirectory, "file3.txt"), Directory.GetFiles(OutputDirectory));
        }

        /// <summary>Extract Archive Multi Volumes Test.</summary>
        [Test]
        public void ExtractArchiveMultiVolumesTest()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multivolume.part0001.rar"))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.IsTrue(File.ReadAllText(Directory.GetFiles(OutputDirectory)[0]).StartsWith("Lorem ipsum dolor sit amet"));
        }

        /// <summary>Extraction With Cancellation Test.</summary>
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

                ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
            }
        }

        /// <summary>Extraction With Skip Test.</summary>
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

                ClassicAssert.AreEqual(2, Directory.GetFiles(OutputDirectory).Length);
            }
        }

        /// <summary>Extraction From Stream Test.</summary>
        [Test]
        public void ExtractionFromStreamTest()
        {
            // TODO: Rewrite this to test against more/all TestData archives.

            using (var tmp = new SevenZipExtractor(File.OpenRead(@"TestData\multiple_files.7z")))
            {
                tmp.ExtractArchive(OutputDirectory);
                ClassicAssert.AreEqual(3, Directory.GetFiles(OutputDirectory).Length);
            }
        }

        /// <summary>Extraction To Stream Test.</summary>
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

            ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);

            var extractedFile = Directory.GetFiles(OutputDirectory)[0];

            ClassicAssert.AreEqual("file2", File.ReadAllText(extractedFile));
        }

        /// <summary>Detect Multi Volume Index Test.</summary>
        [Test]
        public void DetectMultiVolumeIndexTest()
        {
            using (var tmp = new SevenZipExtractor(@"TestData\multivolume.part0001.rar"))
            {
                ClassicAssert.IsTrue(tmp.ArchiveProperties.Any(x => x.Name.Equals("IsVolume") && x.Value != null && x.Value.Equals(true)));
                ClassicAssert.IsTrue(tmp.ArchiveProperties.Any(x => x.Name.Equals("VolumeIndex") && x.Value != null && Convert.ToInt32(x.Value) == 0));
            }

            using (var tmp = new SevenZipExtractor(@"TestData\multivolume.part0002.rar"))
            {
                ClassicAssert.IsTrue(tmp.ArchiveProperties.Any(x => x.Name.Equals("IsVolume") && x.Value != null && x.Value.Equals(true)));
                ClassicAssert.IsFalse(tmp.ArchiveProperties.Any(x => x.Name.Equals("VolumeIndex") && x.Value != null && Convert.ToInt32(x.Value) == 0));
            }
        }

        /// <summary>Threaded Extraction Test.</summary>
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

			ClassicAssert.IsTrue(Directory.Exists(destination1));
	        ClassicAssert.IsTrue(Directory.Exists(destination2));
			ClassicAssert.AreEqual(3, Directory.GetFiles(destination1).Length);
	        ClassicAssert.AreEqual(3, Directory.GetFiles(destination2).Length);
		}

        /// <summary>Extract Archive With Long Path.</summary>
        [Test]
        public void ExtractArchiveWithLongPath()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\long_path.7z"))
            {
                Assert.Throws<PathTooLongException>(() => extractor.ExtractArchive(OutputDirectory));
            }
        }

        /// <summary>Read Archived File Names.</summary>
        [Test]
        public void ReadArchivedFileNames()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                var fileNames = extractor.ArchiveFileNames;
                ClassicAssert.AreEqual(3, fileNames.Count);

                ClassicAssert.AreEqual("file1.txt", fileNames[0]);
                ClassicAssert.AreEqual("file2.txt", fileNames[1]);
                ClassicAssert.AreEqual("file3.txt", fileNames[2]);
            }
        }
        
        /// <summary>Read Archived File Data.</summary>
        [Test]
        public void ReadArchivedFileData()
        {
            using (var extractor = new SevenZipExtractor(@"TestData\multiple_files.7z"))
            {
                var fileData = extractor.ArchiveFileData;
                ClassicAssert.AreEqual(3, fileData.Count);

                ClassicAssert.AreEqual("file1.txt", fileData[0].FileName);
                ClassicAssert.IsFalse(fileData[0].Encrypted);
                ClassicAssert.IsFalse(fileData[0].IsDirectory);
            }
        }

        /// <summary>Extract Different Formats Test.</summary>
        [Test, TestCaseSource(nameof(TestFiles))]
        public void ExtractDifferentFormatsTest(TestFile file)
        {
            using (var extractor = new SevenZipExtractor(file.FilePath))
            {
                extractor.ExtractArchive(OutputDirectory);
                ClassicAssert.AreEqual(1, Directory.GetFiles(OutputDirectory).Length);
            }
        }
    }

    /// <summary>
    /// Simple wrapper to get better names for ExtractDifferentFormatsTest results.
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-23T10:53:00Z", Digest = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", Stale = false, Path = "SevenZipExtractorTests.cs", Since = "2026-08-23")]
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
