// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Microsoft.PowerToys.PreviewHandler.Folder.Utilities
{
    /// <summary>
    /// Helper class to get a folder icon
    /// Code courtesy of <see href="https://stackoverflow.com/a/59129804">Stack Overflow</see>
    /// Author: <see href="https://stackoverflow.com/users/574531/herohtar">Herohtar</see>
    /// </summary>
    public static class FolderIconHelper
    {
        private static Icon folderIcon;

        /// <summary>
        /// Gets default windows folder icon
        /// </summary>
        public static Icon FolderLarge => folderIcon ?? (folderIcon = GetStockIcon(SHSIIDFOLDER, SHGSILARGEICON));

        private static Icon GetStockIcon(uint type, uint size)
        {
            var info = default(SHSTOCKICONINFO);
            info.cbSize = (uint)Marshal.SizeOf(info);

            _ = SHGetStockIconInfo(type, SHGSIICON | size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
        }

        /// <summary>
        /// Stock icon information struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
#pragma warning disable SA1307 // Accessible fields must begin with upper case letter
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
#pragma warning restore SA1307 // Accessible fields must begin with upper case letter
        }

        /// <summary>
        /// Retrieves information about system-defined shell icons.
        /// </summary>
        /// <param name="siid">Stock icon ID</param>
        /// <param name="uFlags">Flags</param>
        /// <param name="psii">Returned stock icon info</param>
        /// <returns>S_OK if successful else HRESULT error code</returns>
        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="handle">Handle icon to destroy</param>
        /// <returns>If the function succeeds the return value is non zero</returns>
        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        private const uint SHSIIDFOLDER = 0x3;
        private const uint SHGSIICON = 0x100;
        private const uint SHGSILARGEICON = 0x0;
    }
}
