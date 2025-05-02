using org.SpocWeb.root.Logging;

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
    public class SevenZipCompressorTests : TestBase
    {
        /// <summary>
        /// TestCaseSource for CompressDifferentFormatsTest
        /// </summary>
        public static List<CompressionMethod> CompressionMethods
        {
            get
            {
                var result = new List<CompressionMethod>();
                foreach(CompressionMethod format in Enum.GetValues(typeof(CompressionMethod)))
                {
                    result.Add(format);
                }

                return result;
            }
        }

        [Test]
        public void CompressDirectory_WithSfnPath()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                PreserveDirectoryRoot = true
            };

            compressor.CompressDirectory("TESTDA~1", TemporaryFile);
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldStartWith("TestData_LongerDirectoryName", StringComparison.OrdinalIgnoreCase);
            }
        }

        [Test]
        public void CompressDirectory_NonExistentDirectory()
        {
            var compressor = new SevenZipCompressor();

            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("nonexistent", TemporaryFile));
            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("", TemporaryFile));
        }

        [Test]
        public void CompressFile_WithSfnPath()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip
            };

            compressor.CompressFiles(TemporaryFile, @"TESTDA~1\emptyfile.txt");
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldBe("emptyfile.txt");
            }
        }

        [Test]
        public void CompressFileTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };
            
            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")).ShouldBe();
        }

        [Test]
        public void CompressDirectoryTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressDirectory("TestData", TemporaryFile);
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            File.Delete(TemporaryFile);

            Directory.GetFiles("TestData").Select(Path.GetFileName).ToArray()
                .ShouldBeSequence(Directory.GetFiles(OutputDirectory).Select(Path.GetFileName));
        }

        [Test]
        public void CompressWithAppendModeTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
            }

            compressor.CompressionMode = CompressionMode.Append;

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(2u);
            }
        }

        [Test]
        public void ModifyProtectedArchiveTest()
        {
            var compressor = new SevenZipCompressor
            {
                DirectoryStructure = false,
                EncryptHeaders = true
            };

            compressor.CompressFilesEncrypted(TemporaryFile, "password", @"TestData\7z_LZMA2.7z", @"TestData\zip.zip");

            var modificationList = new Dictionary<int, string>
            {
                {0, "changed.zap"},
                {1, null }
            };

            compressor.ModifyArchive(TemporaryFile, modificationList, "password");

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile, "password"))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldBe("changed.zap");
            }
        }

        [Test]
        public void ModifyNonArchiveTest()
        {
            var compressor = new SevenZipCompressor
            {
                DirectoryStructure = false
            };

            File.WriteAllText(TemporaryFile, "I'm not an archive.");

            var modificationList = new Dictionary<int, string> {{0, ""}};

            Assert.Throws<SevenZipArchiveException>(() => compressor.ModifyArchive(TemporaryFile, modificationList));
        }

        [Test]
        public void CompressWithModifyModeRenameTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            File.Exists(TemporaryFile).ShouldBe();

            compressor.ModifyArchive(TemporaryFile, new Dictionary<int, string> { { 0, "renamed.7z" }});

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ExtractArchive(OutputDirectory);
            }

            File.Exists(Path.Combine(OutputDirectory, "renamed.7z")).ShouldBe();
            File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")).ShouldNotBe();
        }

        [Test]
        public void CompressWithModifyModeDeleteTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            File.Exists(TemporaryFile).ShouldBe();

            compressor.ModifyArchive(TemporaryFile, new Dictionary<int, string> { { 0, null } });

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(0u);
                extractor.ExtractArchive(OutputDirectory);
            }

            File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")).ShouldNotBe();
        }

        [Test]
        public void MultiVolumeCompressionTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false,
                VolumeSize = 100
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");

            Directory.GetFiles(OutputDirectory).Length.ShouldBe(3);
            File.Exists($"{TemporaryFile}.003").ShouldBe();
        }

        [Test]
        public void CompressToStreamTest()
        {
            var compressor = new SevenZipCompressor {DirectoryStructure = false};

            using (var stream = File.Create(TemporaryFile))
            {
                compressor.CompressFiles(stream, @"TestData\zip.zip");
            }
            
            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldBe("zip.zip");
            }
        }

        [Test]
        public void CompressFromStreamTest()
        {
            using (var input = File.OpenRead(@"TestData\zip.zip"))
            {
                using (var output = File.Create(TemporaryFile))
                {
                    var compressor = new SevenZipCompressor
                    {
                        DirectoryStructure = false
                    };

                    compressor.CompressStream(input, output);
                }
                    
            }

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileData[0].Size.ShouldBe((ulong)new FileInfo(@"TestData\zip.zip").Length);
            }
        }

        [Test]
        public void CompressFileDictionaryTest()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };

            var fileDict = new Dictionary<string, string>
            {
                {"zip.zip", @"TestData\zip.zip"}
            };

            compressor.CompressFileDictionary(fileDict, TemporaryFile);

            File.Exists(TemporaryFile).ShouldBe();

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.FilesCount.ShouldBe(1u);
                extractor.ArchiveFileNames[0].ShouldBe("zip.zip");
            }
        }

        [Test]
        public void ThreadedCompressionTest()
        {
			var tempFile1 = Path.Combine(OutputDirectory, "t1.7z");
			var tempFile2 = Path.Combine(OutputDirectory, "t2.7z");

			var t1 = new Thread(() =>
            {
                var tmp = new SevenZipCompressor();
				tmp.CompressDirectory("TestData", tempFile1);
			});

            var t2 = new Thread(() =>
            {
                var tmp = new SevenZipCompressor();                
                tmp.CompressDirectory("TestData", tempFile2);
            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

			File.Exists(tempFile1).ShouldBe();
			File.Exists(tempFile2).ShouldBe();
		}

        [Test, TestCaseSource(nameof(CompressionMethods))]
        public void CompressDifferentFormatsTest(CompressionMethod method)
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                CompressionMethod = method
            };

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");

            File.Exists(TemporaryFile).ShouldBe();
        }
    }
}
