// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Common;
using Microsoft.PowerToys.PreviewHandler.Folder.Properties;
using Microsoft.PowerToys.PreviewHandler.Folder.Utilities;
using Microsoft.PowerToys.Telemetry;
using Windows.UI.ViewManagement;

namespace Microsoft.PowerToys.PreviewHandler.Folder
{
    /// <summary>
    ///  Implementation of Control for Folder Preview Handler.
    /// </summary>
    public class FolderPreviewHandlerControl : FormHandlerControl
    {
        private const int SHOWCAP = 20;

        /// <summary>
        /// Use UISettings to get system colors and scroll bar size.
        /// </summary>
        private static UISettings _uISettings = new UISettings();

        private FlowLayoutPanel _layout;

        /// <summary>
        /// Start the preview on the Control.
        /// </summary>
        /// <param name="dataSource">Stream reference to access source file.</param>
        public override void DoPreview<T>(T dataSource)
        {
            SuspendLayout();
            try
            {
                if (!(dataSource is string filePath))
                {
                    throw new ArgumentException($"{nameof(dataSource)} for {nameof(FolderPreviewHandler)} must be a string but was a '{typeof(T)}'");
                }

                InvokeOnControlThread(() =>
                {
                    BackColor = GetThemeColor(UIColorType.Background);
                    Resize += FormResized;

                    _layout = new FlowLayoutPanel
                    {
                        AutoScroll = true,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        FlowDirection = FlowDirection.TopDown,
                        WrapContents = false,
                    };
                    _layout.Width = Width;
                    _layout.Height = Height;

                    string[] dirList = Directory.GetFileSystemEntries(filePath);

                    int current = 0;
                    foreach (string item in dirList)
                    {
                        if (current >= SHOWCAP)
                        {
                            string excessFiles = string.Format(CultureInfo.InvariantCulture, Resources.FolderExtraFilesHidden, dirList.Length - SHOWCAP);
                            _layout.Controls.Add(new ExplorerFile(GetIcon(filePath), excessFiles));
                            break;
                        }

                        string[] splitPath = item.Split("\\");
                        _layout.Controls.Add(new ExplorerFile(GetIcon(item), splitPath[splitPath.Length - 1]));
                        current++;
                    }

                    Controls.Add(_layout);

                    // Setup initial textbox sizes.
                    FormResized(null, null);
                });
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

            ResumeLayout(false);
            PerformLayout();
        }

        private void FormResized(object sender, EventArgs e)
        {
            SuspendLayout();
            _layout.SuspendLayout();
            _layout.Width = Width;
            foreach (ExplorerFile panel in _layout.Controls.Find("File", false))
            {
                panel.SuspendLayout();
                panel.Width = Width;
                panel.UpdateTextBox();
                panel.ResumeLayout(false);
            }

            _layout.ResumeLayout(false);
            ResumeLayout(false);
        }

        /// <summary>
        /// Get the system theme color, based on the selected theme.
        /// </summary>
        /// <returns>An object of type <see cref="Color"/>.</returns>
        private static Color GetThemeColor(UIColorType type)
        {
            var sysColor = _uISettings.GetColorValue(type);

            // Windows dark theme is set to RGB(0,0,0) which does not match properly
            if (type == UIColorType.Background && sysColor.R == 0)
            {
                return Color.FromArgb(255, 32, 32, 32);
            }

            return Color.FromArgb(sysColor.A, sysColor.R, sysColor.G, sysColor.B);
        }

        private static Icon GetIcon(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return FolderIconHelper.FolderLarge;
            }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Icon fileIcon = Icon.ExtractAssociatedIcon(path);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (fileIcon == null)
            {
                return SystemIcons.WinLogo;
            }

            return fileIcon;
        }

        private class ExplorerFile : FlowLayoutPanel
        {
            private PictureBox _pictureBox;
            private TextBox _textBox;

            private Icon _icon;
            private string _file;

            public ExplorerFile(Icon icon, string file)
            {
                _icon = icon;
                _file = file;

                Name = "File";
                Dock = DockStyle.Fill;
                AutoSize = true;
                FlowDirection = FlowDirection.LeftToRight;

                _pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = _icon.ToBitmap(),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    AutoSize = true,
                    Padding = Padding.Empty,
                };

                _textBox = new TextBox
                {
                    Text = _file,
                    BackColor = FolderPreviewHandlerControl.GetThemeColor(UIColorType.Background),
                    ForeColor = FolderPreviewHandlerControl.GetThemeColor(UIColorType.Foreground),
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                };

                // Apply margin after text is added
                _textBox.Margin = new Padding(0, (_pictureBox.Height - _textBox.Height) / 2, 0, 0);

                Controls.Add(_pictureBox);
                Controls.Add(_textBox);
            }

            public void UpdateTextBox()
            {
                _textBox.Width = (int)(0.6 * Width);
            }
        }
    }
}
