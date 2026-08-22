using System.ComponentModel;
namespace SevenZip
{
    /// <summary>
    /// The definition of the interface which supports the cancellation of a process.
    /// </summary>
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
