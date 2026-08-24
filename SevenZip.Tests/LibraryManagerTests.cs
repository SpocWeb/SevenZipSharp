using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    /// <summary>Tests for library Manager.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="SetNonExistant7zDllLocationTest"/> | Set Non Existant7z Dll Location Test. |
    /// | 27 | <see cref="CurrentLibraryFeaturesTest"/> | Current Library Features Test. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-23T10:39:12Z", Digest = "102293e94063feb14a2ee31d677bda21ef595637df16cf2c7f64ba35bfafce03", Stale = false, Path = "LibraryManagerTests.cs", Since = "2026-08-23")]
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

            ClassicAssert.IsTrue(features.HasFlag(LibraryFeature.ExtractAll));
            ClassicAssert.IsTrue(features.HasFlag(LibraryFeature.CompressAll));
            ClassicAssert.IsTrue(features.HasFlag(LibraryFeature.Modify));
        }
    }
}
