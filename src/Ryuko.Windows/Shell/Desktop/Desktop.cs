// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Windows.Shell
{
    using Ryuko.Windows.Shell.Enums;
    using Ryuko.Windows.Shell.Internal;

    using System;
    using System.Runtime.InteropServices;

    public static partial class Desktop
    {
        #region Properties

        /// <summary>
        /// 取得叫用端是否為作用中(Focus)的視窗
        /// </summary>
        public static bool IsActiveWindow
        {
            get
            {
                return GetWindow(GetForegroundWindow(), 5)
                    == GetWindow(GetHandle(), 3);
            }
        }

        /// <summary>
        /// 取得桌面圖示及項目選單的顯示狀態
        /// </summary>
        public static bool IsDesktopControlsVisible
        {
            get
            {
                return IsWindowVisible(GetHandle());
            }
        }

        /// <summary>
        /// 取得桌面圖示的顯示狀態
        /// </summary>
        public static bool IsDesktopIconsVisible
        {
            get
            {
                var hWnd = GetWindow(GetHandle(), 5);
                var info = new WINDOWINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);
                GetWindowInfo(hWnd, ref info);
                return (info.dwStyle & 0x10000000) == 0x10000000;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 切換為鎖定畫面
        /// </summary>
        [DllImport(Constants.USER32_DLL, EntryPoint = "LockWorkStation", CharSet = CharSet.Unicode)]
        public static extern void LockScreen();

        /// <summary>
        /// 切換桌面圖示及項目選單的顯示狀態
        /// </summary>
        public static void ToggleControls()
            => ShowWindowAsync(GetHandle(), IsDesktopControlsVisible ? SetWindows.Hide : SetWindows.Normal);

        /// <summary>
        /// 切換桌面圖示的顯示狀態
        /// </summary>
        public static void ToggleIcons()
        {
            SendMessage(GetHandle(), 0x0111, (IntPtr)0x7402, (IntPtr)0);
        }

        #endregion Methods
    }
}