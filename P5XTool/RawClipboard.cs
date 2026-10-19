using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace P5XTool
{
    // I HATE THE WINDOWS CLIPBOARD
    public class RawClipboard : IDisposable
    {
        bool _disposed = false;
        public const uint CF_DIB = 8;
        private RawClipboard() {
            OpenClipboard(IntPtr.Zero);
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint RegisterClipboardFormat(string name);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint format, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalAlloc(uint flags, IntPtr size);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GlobalUnlock(IntPtr hMem);

        const uint GMEM_MOVEABLE = 2;
        public static RawClipboard Open()
        {
            return new();
        }
        public void Close()
        {
            if (_disposed) return;
            _disposed = true;
            CloseClipboard();
        }
        public void Dispose()
        {
            Close();
        }
        public void Empty()
        {
            EmptyClipboard();
        }
        public void SetBytes(uint fmt, byte[] bytes)
        {
            IntPtr h = GlobalAlloc(GMEM_MOVEABLE, (IntPtr)bytes.Length);
            IntPtr p = GlobalLock(h);

            Marshal.Copy(bytes, 0, p, bytes.Length);

            GlobalUnlock(h);

            SetClipboardData(fmt, h);
        }
        public void SetBytes(string format, byte[] bytes)
        {
            uint fmt = RegisterClipboardFormat(format);
            SetBytes(fmt, bytes);
        }
    }
}
