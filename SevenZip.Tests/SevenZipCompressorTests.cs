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

    /// <summary>Tests for the SevenZip Compressor.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 54 | <see cref="CompressionMethods"/> | TestCaseSource for CompressDifferentFormatsTest |
    /// | 70 | <see cref="CompressDirectory_WithSfnPath"/> | Compress Directory — with Sfn Path. |
    /// | 90 | <see cref="CompressDirectory_NonExistentDirectory"/> | Compress Directory — non Existent Directory. |
    /// | 100 | <see cref="CompressFile_WithSfnPath"/> | Compress File — with Sfn Path. |
    /// | 119 | <see cref="CompressFileTest"/> | Compress File Test. |
    /// | 140 | <see cref="CompressDirectoryTest"/> | Compress Directory Test. |
    /// | 163 | <see cref="CompressWithAppendModeTest"/> | Compress With Append Mode Test. |
    /// | 191 | <see cref="ModifyProtectedArchiveTest"/> | Modify Protected Archive Test. |
    /// | 220 | <see cref="ModifyNonArchiveTest"/> | Modify Non Archive Test. |
    /// | 236 | <see cref="CompressWithModifyModeRenameTest"/> | Compress With Modify Mode Rename Test. |
    /// | 261 | <see cref="CompressWithModifyModeDeleteTest"/> | Compress With Modify Mode Delete Test. |
    /// | 285 | <see cref="MultiVolumeCompressionTest"/> | Multi Volume Compression Test. |
    /// | 302 | <see cref="CompressToStreamTest"/> | Compress To Stream Test. |
    /// | 322 | <see cref="CompressFromStreamTest"/> | Compress From Stream Test. |
    /// | 349 | <see cref="CompressFileDictionaryTest"/> | Compress File Dictionary Test. |
    /// | 371 | <see cref="ThreadedCompressionTest"/> | Threaded Compression Test. |
    /// | 399 | <see cref="CompressDifferentFormatsTest"/> | Compress Different Formats Test. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="SevenZipCompressor"/> | Main type under test; instantiated in every test method. |
    /// | <see cref="SevenZipExtractor"/> | Used to verify compressed archive contents. |
    /// | <see cref="CompressionMethod"/> | Enumeration providing compression method test variations. |
    /// </remarks>
    [Facets(Layer = "test", Status = "stable", Complexity = 4)]
    [Tags("code/compression")]
    [System.ComponentModel.Description("Tests for the SevenZip Compressor.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:52Z", Digest = "2881180b57f40b5e2ac27f69d9df434dc7669e7c6ac3be67ce9ad067775130c2", Stale = false, Path = "SevenZipCompressorTests.cs", Since = "2026-08-23")]
    [TestFixture]
    [Concept("test_fixture_multi_format")]
    public class SevenZipCompressorTests : TestBase
    {
        /// <summary>
        /// TestCaseSource for CompressDifferentFormatsTest
        /// </summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("TestCaseSource for CompressDifferentFormatsTest")]
        [Concept("test_fixture_multi_format")]
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
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress Directory — with Sfn Path.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressDirectory_WithSfnPath()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                PreserveDirectoryRoot = true
            };

            compressor.CompressDirectory("TESTDA~1", TemporaryFile);
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.IsTrue(extractor.ArchiveFileNames[0].StartsWith("TestData_LongerDirectoryName", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>Compress Directory — non Existent Directory.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress Directory — non Existent Directory.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressDirectory_NonExistentDirectory()
        {
            var compressor = new SevenZipCompressor();

            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("nonexistent", TemporaryFile));
            Assert.Throws<ArgumentException>(() => compressor.CompressDirectory("", TemporaryFile));
        }

        /// <summary>Compress File — with Sfn Path.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress File — with Sfn Path.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressFile_WithSfnPath()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip
            };

            compressor.CompressFiles(TemporaryFile, @"TESTDA~1\emptyfile.txt");
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual("emptyfile.txt", extractor.ArchiveFileNames[0]);
            }
        }

        /// <summary>Compress File Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress File Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressFileTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };
            
            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            ClassicAssert.IsTrue(File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")));
        }

        /// <summary>Compress Directory Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress Directory Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressDirectoryTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressDirectory("TestData", TemporaryFile);
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                extractor.ExtractArchive(OutputDirectory);
            }

            File.Delete(TemporaryFile);

            ClassicAssert.AreEqual(Directory.GetFiles("TestData").Select(Path.GetFileName).ToArray(), Directory.GetFiles(OutputDirectory).Select(Path.GetFileName).ToArray());
        }

        /// <summary>Compress With Append Mode Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress With Append Mode Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressWithAppendModeTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
            }

            compressor.CompressionMode = CompressionMode.Append;

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(2, extractor.FilesCount);
            }
        }

        /// <summary>Modify Protected Archive Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Modify Protected Archive Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile, "password"))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual("changed.zap", extractor.ArchiveFileNames[0]);
            }
        }

        /// <summary>Modify Non Archive Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Modify Non Archive Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
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
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress With Modify Mode Rename Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressWithModifyModeRenameTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            compressor.ModifyArchive(TemporaryFile, new Dictionary<int, string> { { 0, "renamed.7z" }});

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                extractor.ExtractArchive(OutputDirectory);
            }

            ClassicAssert.IsTrue(File.Exists(Path.Combine(OutputDirectory, "renamed.7z")));
            ClassicAssert.IsFalse(File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")));
        }

        /// <summary>Compress With Modify Mode Delete Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress With Modify Mode Delete Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressWithModifyModeDeleteTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            compressor.ModifyArchive(TemporaryFile, new Dictionary<int, string> { { 0, null } });

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(0, extractor.FilesCount);
                extractor.ExtractArchive(OutputDirectory);
            }

            ClassicAssert.IsFalse(File.Exists(Path.Combine(OutputDirectory, "7z_LZMA2.7z")));
        }

        /// <summary>Multi Volume Compression Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Multi Volume Compression Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void MultiVolumeCompressionTest()
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = false,
                VolumeSize = 100
            };

            compressor.CompressFiles(TemporaryFile, @"Testdata\7z_LZMA2.7z");

            ClassicAssert.AreEqual(3, Directory.GetFiles(OutputDirectory).Length);
            ClassicAssert.IsTrue(File.Exists($"{TemporaryFile}.003"));
        }

        /// <summary>Compress To Stream Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress To Stream Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressToStreamTest()
        {
            var compressor = new SevenZipCompressor {DirectoryStructure = false};

            using (var stream = File.Create(TemporaryFile))
            {
                compressor.CompressFiles(stream, @"TestData\zip.zip");
            }
            
            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual("zip.zip", extractor.ArchiveFileNames[0]);
            }
        }

        /// <summary>Compress From Stream Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress From Stream Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
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

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual(new FileInfo(@"TestData\zip.zip").Length, extractor.ArchiveFileData[0].Size);
            }
        }

        /// <summary>Compress File Dictionary Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress File Dictionary Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
        public void CompressFileDictionaryTest()
        {
            var compressor = new SevenZipCompressor { DirectoryStructure = false };

            var fileDict = new Dictionary<string, string>
            {
                {"zip.zip", @"TestData\zip.zip"}
            };

            compressor.CompressFileDictionary(fileDict, TemporaryFile);

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));

            using (var extractor = new SevenZipExtractor(TemporaryFile))
            {
                ClassicAssert.AreEqual(1, extractor.FilesCount);
                ClassicAssert.AreEqual("zip.zip", extractor.ArchiveFileNames[0]);
            }
        }

        /// <summary>Threaded Compression Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Threaded Compression Test.")]
        [Test]
        [Concept("test_fixture_multi_format")]
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

			ClassicAssert.IsTrue(File.Exists(tempFile1));
			ClassicAssert.IsTrue(File.Exists(tempFile2));
		}

        /// <summary>Compress Different Formats Test.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 4)]
        [Tags("code/compression")]
        [System.ComponentModel.Description("Compress Different Formats Test.")]
        [Test, TestCaseSource(nameof(CompressionMethods))]
        [Concept("test_fixture_multi_format")]
        public void CompressDifferentFormatsTest(CompressionMethod method)
        {
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                CompressionMethod = method
            };

            compressor.CompressFiles(TemporaryFile, @"TestData\zip.zip");

            ClassicAssert.IsTrue(File.Exists(TemporaryFile));
        }
    }
}
