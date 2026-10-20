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
    public partial class RawClipboard : IDisposable
    {
        bool _disposed = false;
        public const uint CF_DIB = 8;
        private RawClipboard() {
            OpenClipboard(IntPtr.Zero);
        }
        [LibraryImport(
            "user32.dll",
            EntryPoint = "RegisterClipboardFormatW",
            StringMarshalling = StringMarshalling.Utf16,
            SetLastError = true)]
        static private partial uint RegisterClipboardFormat(string name);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static private partial bool OpenClipboard(IntPtr hWndNewOwner);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static private partial bool EmptyClipboard();

        [LibraryImport("user32.dll", SetLastError = true)]
        static private partial IntPtr SetClipboardData(uint format, IntPtr hMem);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static private partial bool CloseClipboard();

        [LibraryImport("kernel32.dll", SetLastError = true)]
        static private partial IntPtr GlobalAlloc(uint flags, IntPtr size);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        static private partial IntPtr GlobalLock(IntPtr hMem);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static private partial bool GlobalUnlock(IntPtr hMem);

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
