using org.SpocWeb.root.logging;

namespace SevenZip.Tests
{
    using NUnit.Framework;

    /// <summary>Tests for library Manager.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 13 | <see cref="SetNonExistant7zDllLocationTest"/> | Set Non Existant7z Dll Location Test. |
    /// | 17 | <see cref="CurrentLibraryFeaturesTest"/> | Current Library Features Test. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 102293e94063feb14a2ee31d677bda21ef595637df16cf2c7f64ba35bfafce03
    /// </code>
    /// </example>
    [TestFixture]
    public class LibraryManagerTests : TestBase
    {

		/// <summary>Set Non Existant7z Dll Location Test.</summary>
		[Test]
		public void SetNonExistant7zDllLocationTest() => Assert.Throws<SevenZipLibraryException>(() => SevenZipLibraryManager.SetLibraryPath("null"));

		/// <summary>Current Library Features Test.</summary>
		[Test]
        public void CurrentLibraryFeaturesTest()
        {
            var features = SevenZipBase.CurrentLibraryFeatures;

            // Exercising more code paths...
            features = SevenZipLibraryManager.CurrentLibraryFeatures;

            features.HasFlag(LibraryFeature.ExtractAll).ShouldBe();
            features.HasFlag(LibraryFeature.CompressAll).ShouldBe();
            features.HasFlag(LibraryFeature.Modify).ShouldBe();
        }
    }
}
