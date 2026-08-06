using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using System.Collections.Generic;
    using System.IO;

    using SevenZip;

    using NUnit.Framework;

    /// <summary>
    /// Test data to use for CheckFileSignatureTest.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 27 | <see cref="FileCheckerTestData"/> | Initializes a new instance of FileCheckerTestData with the specified testDataFilePath and expectedFormat. |
    /// | 36 | <see cref="ExpectedFormat"/> | Format this test expects to find. |
    /// | 47 | <see cref="TestDataFilePath"/> | Path to archive file to test against. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="InArchiveFormat"/> | Passed as a parameter. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2025-05-02T17:50:09Z
    /// digest: fa7a89f3b2d9944b3333bb79cdad1f85e5348c2ae2a018b5c685126d679bc06d
    /// </code>
    /// </example>
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
        public InArchiveFormat ExpectedFormat { get; }

		/// <summary> Path to archive file to test against. </summary>
		///
		/// <example>
		/// <code language="yaml">
		/// pass: 2
		/// mtime: 2025-05-02T17:50:09Z
		/// digest: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
		/// </code>
		/// </example>
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
    /// | 101 | <see cref="SetUp"/> | Sets the current directory to the test directory so relative TestData paths resolve. |
    /// | 107 | <see cref="CheckFileSignatureTest"/> | Check File Signature Test. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="FileCheckerTestData"/> | Used as a field. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 00cdec4095cd6faa29684326c9c1ddda13fcff07c3fad574c6e5e32f89b818c5
    /// </code>
    /// </example>
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

		/// <summary>Sets the current directory to the test directory so relative TestData paths resolve.</summary>
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
                FileChecker.CheckSignature(data.TestDataFilePath, out ignored, out ignored2).ShouldBe(data.ExpectedFormat);
            }
        }
    }
}
