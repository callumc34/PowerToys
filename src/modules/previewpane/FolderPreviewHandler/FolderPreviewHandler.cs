using System;
using System.Runtime.InteropServices;
using Common;
using Microsoft.PowerToys.Telemetry;

namespace Microsoft.PowerToys.PreviewHandler.Folder
{
    /// <summary>
    /// Implementation of preview handler for directories
    /// </summary>
    [Guid("eaf31d5b-002e-4c02-97ec-52aa008dfb77")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class FolderPreviewHandler : FileBasedPreviewHandler, IDisposable
    {
        private FolderPreviewHandlerControl _folderPreviewControl;
        private bool disposedValue;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPreviewHandler"/> class.
        /// </summary>
        public FolderPreviewHandler()
        {
            Initialize();
        }

        /// <inheritdoc />
        public override void DoPreview()
        {
            _folderPreviewControl.DoPreview(FilePath);
        }

        /// <inheritdoc/>
        protected override IPreviewHandlerControl CreatePreviewHandlerControl()
        {
            PowerToysTelemetry.Log.WriteEvent(new Telemetry.Events.FolderPreviewHandlerLoaded());
            _folderPreviewControl = new FolderPreviewHandlerControl();
            return _folderPreviewControl;
        }

        /// <summary>
        /// Disposes objects
        /// </summary>
        /// <param name="disposing">Is Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _folderPreviewControl.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}