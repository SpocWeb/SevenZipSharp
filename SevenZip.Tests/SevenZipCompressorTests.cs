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

    /// <summary>Tests for seven Zip Compressor.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 22 | <see cref="CompressionMethods"/> | TestCaseSource for CompressDifferentFormatsTest |
    /// | 37 | <see cref="CompressDirectory_WithSfnPath"/> | Compress Directory — with Sfn Path. |
    /// | 57 | <see cref="CompressDirectory_NonExistentDirectory"/> | Compress Directory — non Existent Directory. |
    /// | 67 | <see cref="CompressFile_WithSfnPath"/> | Compress File — with Sfn Path. |
    /// | 86 | <see cref="CompressFileTest"/> | Compress File Test. |
    /// | 107 | <see cref="CompressDirectoryTest"/> | Compress Directory Test. |
    /// | 131 | <see cref="CompressWithAppendModeTest"/> | Compress With Append Mode Test. |
    /// | 159 | <see cref="ModifyProtectedArchiveTest"/> | Modify Protected Archive Test. |
    /// | 188 | <see cref="ModifyNonArchiveTest"/> | Modify Non Archive Test. |
    /// | 204 | <see cref="CompressWithModifyModeRenameTest"/> | Compress With Modify Mode Rename Test. |
    /// | 229 | <see cref="CompressWithModifyModeDeleteTest"/> | Compress With Modify Mode Delete Test. |
    /// | 253 | <see cref="MultiVolumeCompressionTest"/> | Multi Volume Compression Test. |
    /// | 270 | <see cref="CompressToStreamTest"/> | Compress To Stream Test. |
    /// | 290 | <see cref="CompressFromStreamTest"/> | Compress From Stream Test. |
    /// | 317 | <see cref="CompressFileDictionaryTest"/> | Compress File Dictionary Test. |
    /// | 339 | <see cref="ThreadedCompressionTest"/> | Threaded Compression Test. |
    /// | 367 | <see cref="CompressDifferentFormatsTest"/> | Compress Different Formats Test. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="CompressionMethod"/> | Used as a property. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 2881180b57f40b5e2ac27f69d9df434dc7669e7c6ac3be67ce9ad067775130c2
    /// </code>
    /// </example>
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

        /// <summary>Compress Directory — with Sfn Path.</summary>
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

        /// <summary>Compress Directory — non Existent Directory.</summary>
        [Test]
        public void CompressDirectory_NonExistentDirectory()
        {
            var compressor = new SevenZipCompressor();

            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("nonexistent", TemporaryFile));
            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("", TemporaryFile));
        }

        /// <summary>Compress File — with Sfn Path.</summary>
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

        /// <summary>Compress File Test.</summary>
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

        /// <summary>Compress Directory Test.</summary>
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

        /// <summary>Compress With Append Mode Test.</summary>
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

        /// <summary>Modify Protected Archive Test.</summary>
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

        /// <summary>Modify Non Archive Test.</summary>
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

        /// <summary>Compress With Modify Mode Rename Test.</summary>
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

        /// <summary>Compress With Modify Mode Delete Test.</summary>
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

        /// <summary>Multi Volume Compression Test.</summary>
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

        /// <summary>Compress To Stream Test.</summary>
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

        /// <summary>Compress From Stream Test.</summary>
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

        /// <summary>Compress File Dictionary Test.</summary>
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

        /// <summary>Threaded Compression Test.</summary>
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

        /// <summary>Compress Different Formats Test.</summary>
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
