using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;

    using NUnit.Framework;

    /// <summary>base used in tests.</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T10:53:01Z", Digest = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", Stale = false, Path = "TestBase.cs", Since = "2026-08-23")]
    public abstract class TestBase
    {

        protected const string OutputDirectory = "output";
        protected readonly string TemporaryFile = Path.Combine(OutputDirectory, "tmp.7z");

        [SetUp]
        public void SetUp()
        {
            // Ensures we're in the correct working directory (for test data files).
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            Directory.CreateDirectory(OutputDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            // Sometimes the Sfx test locks the .exe file for a few milliseconds.
            for (var n = 0; n < 10; n++)
            {
                try
                {
                    Directory.Delete(OutputDirectory, true);
                    break;
                }
                catch
                {
                    Thread.Sleep(20);
                }
            }
        }
    }
}
