using System.Diagnostics.Tracing;
using Microsoft.PowerToys.Telemetry;
using Microsoft.PowerToys.Telemetry.Events;

namespace Microsoft.PowerToys.PreviewHandler.Folder.Telemetry.Events
{
    /// <summary>
    /// A telemetry event to be raised when an error has occurred in the preview pane.
    /// </summary>
    [EventData]
    public class FolderFilePreviewError : EventBase, IEvent
    {
        /// <summary>
        /// Gets or sets the error message to log as part of the telemetry event.
        /// </summary>
        public string Message { get; set; }

        /// <inheritdoc/>
        public PartA_PrivTags PartA_PrivTags => PartA_PrivTags.ProductAndServicePerformance;
    }
}