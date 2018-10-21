// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#if TASKBAR
namespace Ryuko.Windows.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Ryuko.Windows.Shell.Enums;
    using Ryuko.Windows.Shell.Internal;

    public static partial class Taskbar
    {

        private static IntPtr StartMenuWnd = IntPtr.Zero;

        /// <summary>
        /// Sets the visibility of the taskbar.
        /// </summary>
        public static bool IsVisible
        {
            get { return GetVisibility(); }

            set { SetVisibility(value); }
        }

        /// <summary>
        /// Hide the taskbar.
        /// </summary>
        public static void Hide()
            => SetVisibility(false);

        /// <summary>
        /// Show the taskbar.
        /// </summary>
        public static void Show()
            => SetVisibility(true);

        [DllImport(Constants.USER32_DLL, CharSet = CharSet.Auto)]
        private static extern bool EnumThreadWindows(int threadId, Func<IntPtr, IntPtr, bool> pfnEnum, IntPtr lParam);

        [DllImport(Constants.USER32_DLL, SetLastError = true)]
        private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(Constants.USER32_DLL, SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport(Constants.USER32_DLL)]
        private static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);

        private static IntPtr GetStartMenuWnd(IntPtr taskBarWnd)
        {
            GetWindowThreadProcessId(taskBarWnd, out var procId);
            if (Process.GetProcessById(procId) is Process p)
                foreach (ProcessThread t in p.Threads)
                    EnumThreadWindows(t.Id, (hWnd, lParam) =>
                    {
                        var buffer = new StringBuilder(256);
                        if (GetWindowText(hWnd, buffer, buffer.Capacity) > 0 && (buffer.ToString() == Constants.START_MENU_CAPTION))
                        {
                            StartMenuWnd = hWnd;
                            return false;
                        }
                        return true;
                    }, IntPtr.Zero);
            return StartMenuWnd;
        }

        private static bool GetVisibility()
        {
            var taskBarWnd = FindWindow(Constants.TRAY_WND, null);
            var startWnd = FindWindowEx(taskBarWnd, IntPtr.Zero, Constants.START_MENU_BUTTON_CAPTION, Constants.START_MENU_CAPTION);

            if (startWnd == IntPtr.Zero)
                startWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, Constants.START_MENU_CAPTION);

            if (startWnd == IntPtr.Zero)
            {
                startWnd = FindWindow(Constants.START_MENU_BUTTON_CAPTION, null);
                if (startWnd == IntPtr.Zero)
                    startWnd = GetStartMenuWnd(taskBarWnd);
            }

            return IsWindowVisible(startWnd);
        }

        [DllImport(Constants.USER32_DLL)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport(Constants.USER32_DLL)]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);

        [DllImport(Constants.USER32_DLL)]
        private static extern bool IsWindowVisible(IntPtr hwnd);

        private static void SetVisibility(bool show)
        {
            var taskBarWnd = FindWindow(Constants.TRAY_WND, null);
            var startWnd = FindWindowEx(taskBarWnd, IntPtr.Zero, Constants.START_MENU_BUTTON_CAPTION, Constants.START_MENU_CAPTION);

            if (startWnd == IntPtr.Zero)
                startWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, Constants.START_MENU_CAPTION);

            if (startWnd == IntPtr.Zero)
            {
                startWnd = FindWindow(Constants.START_MENU_BUTTON_CAPTION, null);
                if (startWnd == IntPtr.Zero)
                    startWnd = GetStartMenuWnd(taskBarWnd);
            }

            var s = show ? SetWindows.Show : SetWindows.Hide;
            ShowWindow(taskBarWnd, s);
            ShowWindow(startWnd, s);
        }

        [DllImport(Constants.USER32_DLL)]
        private static extern int ShowWindow(IntPtr hwnd, SetWindows nCmdShow);

    }
}
#endif