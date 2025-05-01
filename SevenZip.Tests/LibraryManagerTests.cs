namespace SevenZip.Tests
{
    using NUnit.Framework;

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

            Should.ShouldBe(features.HasFlag(LibraryFeature.ExtractAll));
            Should.ShouldBe(features.HasFlag(LibraryFeature.CompressAll));
            Should.ShouldBe(features.HasFlag(LibraryFeature.Modify));
        }
    }
}
