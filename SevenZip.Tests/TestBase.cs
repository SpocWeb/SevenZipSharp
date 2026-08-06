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
    /// | 16 | <see cref="SetUp"/> | Resets the working directory and (re)creates OutputDirectory before each test. |
    /// | 25 | <see cref="TearDown"/> | Deletes OutputDirectory after each test, retrying briefly if a file is still locked. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 862eec8aede190b60ea50a4c3a2f037e48ce25a98d0f89f545c6f087143fb9fc
    /// </code>
    /// </example>
    public abstract class TestBase
    {

        protected const string OutputDirectory = "output";
        protected readonly string TemporaryFile = Path.Combine(OutputDirectory, "tmp.7z");

        /// <summary>Resets the working directory and (re)creates <see cref="OutputDirectory"/> before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            // Ensures we're in the correct working directory (for test data files).
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            Directory.CreateDirectory(OutputDirectory);
        }

        /// <summary>Deletes <see cref="OutputDirectory"/> after each test, retrying briefly if a file is still locked.</summary>
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
