using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    /// <summary>
    /// The definition of the interface which supports the cancellation of a process.
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-22T17:32:56Z", Digest = "21d6c03f693b8217e6803e9bcca91b49a5aa00d99f12f2fc67548b2731d6d120", Stale = false, Path = "EventArguments/ICancellable.cs", Since = "2026-08-23")]
    public interface ICancellable
    {
        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets whether to stop the current archive operation.")]
        bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether to skip the current file.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets whether to skip the current file.")]
        bool Skip { get; set; }
    }
}
