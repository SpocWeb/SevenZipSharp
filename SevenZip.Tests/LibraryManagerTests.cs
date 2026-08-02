namespace SevenZip.Tests
{
    using NUnit.Framework;

    using NUnit.Framework.Legacy;

    [TestFixture]
    public class LibraryManagerTests : TestBase
    {
		[Test]
		public void SetNonExistant7zDllLocationTest() => Assert.Throws<SevenZipLibraryException>(() => SevenZipLibraryManager.SetLibraryPath("null"));

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
