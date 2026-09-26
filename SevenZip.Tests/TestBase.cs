using org.SpocWeb.root.Attributes;
namespace SevenZip.Tests
{
    using System.IO;
    using System.Threading;

    using NUnit.Framework;

    /// <summary>base used in tests.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 26 | <see cref="SetUp"/> | Sets the current working directory to the test directory and creates the output directory. |
    /// | 35 | <see cref="TearDown"/> | Deletes the output directory with retry logic to handle transient file locks. |
    /// </remarks>
    [Facets(Layer = "test", Status = "stable", Complexity = 2)]
    [Tags("code/test_infrastructure")]
    [System.ComponentModel.Description("base used in tests.")]
    [DocState(Pass = 2, MTime = "2026-09-26T18:28:52Z", Digest = "c263d14bb22485fb7f3066d9eb2bf8da79b05bd6aa3dce2da238ff74ec2d5400", Stale = false, Path = "TestBase.cs", Since = "2026-08-23")]
    [Concept("setup_teardown")]
    public abstract class TestBase
    {

        protected const string OutputDirectory = "output";
        protected readonly string TemporaryFile = Path.Combine(OutputDirectory, "tmp.7z");

        /// <summary>Sets the current working directory to the test directory and creates the output directory.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 2)]
        [Tags("code/test_infrastructure")]
        [System.ComponentModel.Description("Sets the current working directory to the test directory and creates the output directory.")]
        [SetUp]
        [Concept("setup_teardown")]
        public void SetUp()
        {
            // Ensures we're in the correct working directory (for test data files).
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            Directory.CreateDirectory(OutputDirectory);
        }

        /// <summary>Deletes the output directory with retry logic to handle transient file locks.</summary>
        [Facets(Layer = "test", Status = "stable", Complexity = 2)]
        [Tags("code/test_infrastructure")]
        [System.ComponentModel.Description("Deletes the output directory with retry logic to handle transient file locks.")]
        [TearDown]
        [Concept("setup_teardown")]
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
