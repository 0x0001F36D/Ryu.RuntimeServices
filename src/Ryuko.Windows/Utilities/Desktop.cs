// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Windows.Utilities
{
    using Ryuko.Windows.Utilities.Enums;
    using Ryuko.Windows.Utilities.Internal;

    using System;
    using System.Runtime.InteropServices;

    public static class Desktop
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

        #region Properties

        public static bool IsActiveWindow
        {
            get
            {
                return GetWindow(GetForegroundWindow(), 5)
                    == GetWindow(GetHandle(), 3);
            }
        }

        public static bool IsDesktopControlsVisible
        {
            get
            {
                return IsWindowVisible(GetHandle());
            }
        }

        public static bool IsDesktopIconsVisible
        {
            get
            {
                IntPtr hWnd = GetWindow(GetHandle(), 5);
                WINDOWINFO info = new WINDOWINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);
                GetWindowInfo(hWnd, ref info);
                return (info.dwStyle & 0x10000000) == 0x10000000;
            }
        }

        #endregion Properties

        #region Methods

        [DllImport(Constants.USER32_DLL, EntryPoint = "LockWorkStation", CharSet = CharSet.Unicode)]
        public static extern void LockScreen();

        public static void ToggleDesktopControls()
            => ShowWindowAsync(GetHandle(), IsDesktopControlsVisible ? SetWindows.Hide : SetWindows.Normal);

        public static void ToggleDesktopIcons()
        {
            SendMessage(GetHandle(), 0x0111, (IntPtr)0x7402, (IntPtr)0);
        }

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