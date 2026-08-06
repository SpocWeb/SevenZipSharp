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

    /// <summary>Tests for seven Zip Extractor.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 21 | <see cref="TestFiles"/> | Gets every file in the TestData directory, excluding multi-volume and long-path archives. |
    /// | 42 | <see cref="ExtractFilesTest"/> | Extract Files Test. |
    /// | 57 | <see cref="ExtractSpecificFilesTest"/> | Extract Specific Files Test. |
    /// | 72 | <see cref="ExtractArchiveMultiVolumesTest"/> | Extract Archive Multi Volumes Test. |
    /// | 85 | <see cref="ExtractionWithCancellationTest"/> | Extraction With Cancellation Test. |
    /// | 105 | <see cref="ExtractionWithSkipTest"/> | Extraction With Skip Test. |
    /// | 125 | <see cref="ExtractionFromStreamTest"/> | Extraction From Stream Test. |
    /// | 138 | <see cref="ExtractionToStreamTest"/> | Extraction To Stream Test. |
    /// | 157 | <see cref="DetectMultiVolumeIndexTest"/> | Detect Multi Volume Index Test. |
    /// | 174 | <see cref="ThreadedExtractionTest"/> | Threaded Extraction Test. |
    /// | 207 | <see cref="ExtractArchiveWithLongPath"/> | Extract Archive With Long Path. |
    /// | 217 | <see cref="ReadArchivedFileNames"/> | Read Archived File Names. |
    /// | 232 | <see cref="ReadArchivedFileData"/> | Read Archived File Data. |
    /// | 247 | <see cref="ExtractDifferentFormatsTest"/> | Extract Different Formats Test. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="TestFile"/> | Used as a property. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 8ecf22186c0ca90d5c2283ab27d5d863c9793d29fa43b0d4066a7fb47f5497ac
    /// </code>
    /// </example>
    [TestFixture]
    public class SevenZipExtractorTests : TestBase
    {

        /// <summary>Gets every file in the TestData directory, excluding multi-volume and long-path archives.</summary>
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

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
            }
        }

        /// <summary>Extract Specific Files Test.</summary>
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

        /// <summary>Extract Archive Multi Volumes Test.</summary>
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

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
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

                Directory.GetFiles(OutputDirectory).Length.ShouldBe(2);
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
                Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
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

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(1);

            var extractedFile = Directory.GetFiles(OutputDirectory)[0];

            File.ReadAllText(extractedFile).ShouldBe("file2");
        }

        /// <summary>Detect Multi Volume Index Test.</summary>
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

			Directory.Exists(destination1).ShouldBe();
	        Directory.Exists(destination2).ShouldBe();
			Directory.GetFiles(destination1).Length.ShouldBe(3);
	        Directory.GetFiles(destination2).Length.ShouldBe(3);
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
                fileNames.Count.ShouldBe(3);

                fileNames[0].ShouldBe("file1.txt");
                fileNames[1].ShouldBe("file2.txt");
                fileNames[2].ShouldBe("file3.txt");
            }
        }
        
        /// <summary>Read Archived File Data.</summary>
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

        /// <summary>Extract Different Formats Test.</summary>
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
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 273 | <see cref="FilePath"/> | Path of the extracted or referenced test archive file. |
    /// | 276 | <see cref="TestFile"/> | Initializes a new instance of TestFile wrapping filePath. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2025-05-02T17:50:09Z
    /// digest: 6b8b55b97e299d81c17c4dba2c079c6e91ea59f37bd466c65fc677f86efc8917
    /// </code>
    /// </example>
    public class TestFile
    {

        /// <summary>Path of the extracted or referenced test archive file.</summary>
        public string FilePath { get; }

        /// <summary>Initializes a new instance of <see cref="TestFile"/> wrapping <paramref name="filePath"/>.</summary>
        public TestFile(string filePath)
        {
            FilePath = filePath;
        }

		public override string ToString() => Path.GetFileName(FilePath);
	}
}
