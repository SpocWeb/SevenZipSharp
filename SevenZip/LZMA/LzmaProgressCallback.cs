using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;

    using SevenZip.Sdk;

    /// <summary>
    /// Callback to implement the ICodeProgress interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 37 | <see cref="LzmaProgressCallback"/> | Initializes a new instance of the LzmaProgressCallback class |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="EventHandler"/> | Passed as a parameter. |
    /// | <see cref="ProgressEventArgs"/> | Passed as a parameter. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "d52e5afa51eaa5b99c848bfc9f78ba7985d6635d2aebd99ada8c7d42e65ae155", Stale = false, Path = "LZMA/LzmaProgressCallback.cs", Since = "2026-08-23")]
    internal sealed class LzmaProgressCallback : ICodeProgress
    {
        private readonly long _inSize;
        private float _oldPercentDone;

        /// <summary>
        /// Initializes a new instance of the LzmaProgressCallback class
        /// </summary>
        /// <param name="inSize">The input size</param>
        /// <param name="working">Progress event handler</param>
        [System.ComponentModel.Description("Initializes a new instance of the LzmaProgressCallback class")]
        public LzmaProgressCallback(long inSize, EventHandler<ProgressEventArgs> working)
        {
            _inSize = inSize;
            Working += working;
        }

        #region ICodeProgress Members

        /// <summary>
        /// Sets the progress
        /// </summary>
        /// <param name="inSize">The processed input size</param>
        /// <param name="outSize">The processed output size</param>
        [System.ComponentModel.Description("Sets the progress")]
        public void SetProgress(long inSize, long outSize)
        {
            if (Working != null)
            {
                float newPercentDone = (inSize + 0.0f) / _inSize;
                float delta = newPercentDone - _oldPercentDone;
                if (delta * 100 < 1.0)
                {
                    delta = 0;
                }
                else
                {
                    _oldPercentDone = newPercentDone;
                }
                Working(this, new ProgressEventArgs(
                                  PercentDoneEventArgs.ProducePercentDone(newPercentDone),
                                  delta > 0 ? PercentDoneEventArgs.ProducePercentDone(delta) : (byte)0));
            }
        }

        #endregion

        public event EventHandler<ProgressEventArgs> Working;
    }
}
