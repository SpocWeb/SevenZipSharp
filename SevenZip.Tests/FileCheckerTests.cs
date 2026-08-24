using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System.Collections.Generic;
    using System.IO;

    using SevenZip;

    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    /// <summary>
    /// Test data to use for CheckFileSignatureTest.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 37 | <see cref="FileCheckerTestData"/> | Initializes a new instance of FileCheckerTestData with the specified testDataFilePath and expectedFormat. |
    /// | 46 | <see cref="ExpectedFormat"/> | Format this test expects to find. |
    /// | 52 | <see cref="TestDataFilePath"/> | Path to archive file to test against. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="InArchiveFormat"/> | Passed as a parameter. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:53:00Z", Digest = "fa7a89f3b2d9944b3333bb79cdad1f85e5348c2ae2a018b5c685126d679bc06d", Stale = false, Path = "FileCheckerTests.cs", Since = "2026-08-23")]
    public struct FileCheckerTestData
    {

        /// <summary>Initializes a new instance of <see cref="FileCheckerTestData"/> with the specified <paramref name="testDataFilePath"/> and <paramref name="expectedFormat"/>.</summary>
        public FileCheckerTestData(string testDataFilePath, InArchiveFormat expectedFormat)
        {
            TestDataFilePath = testDataFilePath;
            ExpectedFormat = expectedFormat;
        }

        /// <summary>
        /// Format this test expects to find.
        /// </summary>
        [System.ComponentModel.Description("Format this test expects to find.")]
        public InArchiveFormat ExpectedFormat { get; }

        /// <summary>
        /// Path to archive file to test against.
        /// </summary>
        [System.ComponentModel.Description("Path to archive file to test against.")]
        public string TestDataFilePath { get; }

		/// <inheritdoc />
		public override string ToString() =>
			// Used to get useful test results.
			ExpectedFormat.ToString();
	}

    /// <summary>Tests for file Checker.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 122 | <see cref="SetUp"/> | Ensures the working directory is set to the test directory for accessing test data files. |
    /// | 128 | <see cref="CheckFileSignatureTest"/> | Check File Signature Test. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="FileCheckerTestData"/> | Used as a field. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T13:52:27Z", Digest = "96a24fc88d6ee4960a6b32c2aa678c6c00cc0d7029a64f76b4b91198fbcac0d7", Stale = false, Path = "FileCheckerTests.cs", Since = "2026-08-23")]
    [TestFixture]
    public class FileCheckerTests
    {
        /// <summary>
        /// Test data for CheckFileSignature test.
        /// </summary>
        public static List<FileCheckerTestData> TestData = new List<FileCheckerTestData>
        {
            new FileCheckerTestData(@"TestData\arj.arj", InArchiveFormat.Arj),
            new FileCheckerTestData(@"TestData\bzip2.bz2", InArchiveFormat.BZip2),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Cab),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Chm),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Compound),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Cpio),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Deb),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Dmg),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Elf),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Flv),
            new FileCheckerTestData(@"TestData\gzip.gz", InArchiveFormat.GZip),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Hfs),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Iso),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Lzh),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Lzma),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Lzw),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Msi),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Mslz),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Mub),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Nsis),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.PE),
            new FileCheckerTestData(@"TestData\rar5.rar", InArchiveFormat.Rar),
            new FileCheckerTestData(@"TestData\rar4.rar", InArchiveFormat.Rar4),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Rpm),
            new FileCheckerTestData(@"TestData\7z_LZMA2.7z", InArchiveFormat.SevenZip),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Split),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Swf),
            new FileCheckerTestData(@"TestData\tar.tar", InArchiveFormat.Tar),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Udf),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Vhd),
            new FileCheckerTestData(@"TestData\wim.wim", InArchiveFormat.Wim),
            new FileCheckerTestData(@"TestData\xz.xz", InArchiveFormat.XZ),
            new FileCheckerTestData(@"TestData\", InArchiveFormat.Xar),
            new FileCheckerTestData(@"TestData\zip.zip", InArchiveFormat.Zip)
        };

		/// <summary>Ensures the working directory is set to the test directory for accessing test data files.</summary>
		[SetUp]
		public void SetUp() =>
			// Ensures we're in the correct working directory (for test data files).
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		/// <summary>Check File Signature Test.</summary>
		[TestCaseSource(nameof(TestData))]
        public void CheckFileSignatureTest(FileCheckerTestData data)
        {
            if (!File.Exists(data.TestDataFilePath))
            {
                Assert.Ignore("No test data found for this format.");
            }
            else
            {
                int ignored;
                bool ignored2;
                ClassicAssert.AreEqual(data.ExpectedFormat, FileChecker.CheckSignature(data.TestDataFilePath, out ignored, out ignored2));
            }
        }
    }
}
