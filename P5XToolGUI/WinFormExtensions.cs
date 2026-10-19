using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace P5XToolGUI
{
    public enum ScrollBarDirection
    {
        Horizontal,
        Vertical
    }
    public static partial class WinFormExtensions
    {
        [LibraryImport("user32.dll")]
        private static partial int GetScrollPos(IntPtr hWnd, int nBar);

        [LibraryImport("user32.dll")]
        private static partial int SetScrollPos(IntPtr hWnd, int nBar, int nPos, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

        extension (Control control)
        {
            public int GetScrollPos(ScrollBarDirection direction)
            {
                int nBar = (direction == ScrollBarDirection.Horizontal) ? 0 : 1;
                return GetScrollPos(control.Handle, nBar);
            }
            public int SetScrollPos(ScrollBarDirection direction, int position, bool redraw)
            {
                int nBar = (direction == ScrollBarDirection.Horizontal) ? 0 : 1;
                return SetScrollPos(control.Handle, nBar, position, redraw);
            }
        }
    }
}
