// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using Common;
using Microsoft.PowerToys.Telemetry;

namespace Microsoft.PowerToys.PreviewHandler.Folder
{
    /// <summary>
    ///  Implementation of Control for Folder Preview Handler.
    /// </summary>
    public class FolderPreviewHandlerControl : FormHandlerControl
    {
        /// <summary>
        /// Start the preview on the Control.
        /// </summary>
        /// <param name="dataSource">Stream reference to access source file.</param>
        public override void DoPreview<T>(T dataSource)
        {
            try
            {
                WebBrowser browser = new WebBrowser();

                browser.DocumentText = "Test";
                browser.Dock = DockStyle.Fill;
                browser.IsWebBrowserContextMenuEnabled = true;
                Controls.Add(browser);
                PowerToysTelemetry.Log.WriteEvent(new Telemetry.Events.FolderPreviewed());
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                PowerToysTelemetry.Log.WriteEvent(new Telemetry.Events.FolderPreviewError { Message = ex.Message });
            }
            finally
            {
                base.DoPreview(dataSource);
            }
        }
    }
}
