using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace P5XTool
{
    public static partial class ConsoleMgr
    {
        [LibraryImport("kernel32.dll")]
        private static partial IntPtr GetConsoleWindow();
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AllocConsole();
        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static bool consoleVisible = false;

        public static void Init()
        {
            AllocConsole();
            ShowWindow(GetConsoleWindow(), 0);
            var stdout = Console.OpenStandardOutput();
            var stderr = Console.OpenStandardError();
            var stdin = Console.OpenStandardInput();

            Console.SetOut(new System.IO.StreamWriter(stdout)
            {
                AutoFlush = true
            });

            Console.SetError(new System.IO.StreamWriter(stderr)
            {
                AutoFlush = true
            });

            Console.SetIn(new System.IO.StreamReader(stdin));
        }

        public static void SetVisible(bool visible)
        {
            if (consoleVisible != visible)
            {
                consoleVisible = visible;
                ShowWindow(GetConsoleWindow(), visible ? 5 : 0);
            }
        }
    }
}
