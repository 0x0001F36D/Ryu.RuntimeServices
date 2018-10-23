// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Windows.Shell
{
    using Ryuko.Windows.Shell.Enums;
    using Ryuko.Windows.Shell.Internal;

    using System;
    using System.Runtime.InteropServices;

    static partial class Desktop
    {
        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            private int _Left;
            private int _Top;
            private int _Right;
            private int _Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(bool? filler) : this()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
            }
        }

        #endregion Structs

        #region Methods

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetForegroundWindow();

        private static IntPtr GetHandle()
        {
            IntPtr hDesktopWin = GetDesktopWindow();
            IntPtr hProgman = FindWindow(Constants.PROGRAM, Constants.PEOGRAM_MANAGER);
            IntPtr hWorkerW = IntPtr.Zero;

            IntPtr hShellViewWin = FindWindowEx(hProgman, IntPtr.Zero, Constants.SHELLDLL_DEFVIEW, Constants.EMPTY);
            if (hShellViewWin == IntPtr.Zero)
            {
                do
                {
                    hWorkerW = FindWindowEx(hDesktopWin, hWorkerW, Constants.WORKER_W, Constants.EMPTY);
                    hShellViewWin = FindWindowEx(hWorkerW, IntPtr.Zero, Constants.SHELLDLL_DEFVIEW, Constants.EMPTY);
                }
                while (hShellViewWin == IntPtr.Zero && hWorkerW != null);
            }
            return hShellViewWin;
        }

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Unicode)]
        private static extern bool ShowWindowAsync(IntPtr hWnd, SetWindows nCmdShow);

        #endregion Methods
    }
}