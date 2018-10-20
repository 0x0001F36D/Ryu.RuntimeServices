// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Windows.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Ryuko.Windows.Utilities.Enums;
    using Ryuko.Windows.Utilities.Internal;

    public static partial class Taskbar
    {
        #region Fields

        private static IntPtr StartMenuWnd = IntPtr.Zero;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Sets the visibility of the taskbar.
        /// </summary>
        public static bool IsVisible
        {
            get { return GetVisibility(); }

            set { SetVisibility(value); }
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }

    static partial class Taskbar
    {
        #region Classes

        public class IconProgressBar
        {
            #region Interfaces

            [ComImport]
            [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface ITaskbar
            {
                // ITaskbarList
                [PreserveSig]
                void HrInit();

                [PreserveSig]
                void AddTab(IntPtr hwnd);

                [PreserveSig]
                void DeleteTab(IntPtr hwnd);

                [PreserveSig]
                void ActivateTab(IntPtr hwnd);

                [PreserveSig]
                void SetActiveAlt(IntPtr hwnd);

                // ITaskbarList2
                [PreserveSig]
                void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

                // ITaskbarList3
                [PreserveSig]
                void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);

                [PreserveSig]
                void SetProgressState(IntPtr hwnd, ProgressStatus state);
            }

            #endregion Interfaces

            #region Constructors

            static IconProgressBar()
            {
                s_taskbar = (ITaskbar)new Taskbar();
                IsSupported = Environment.OSVersion.Version >= new Version(6, 1);
            }

            public IconProgressBar(IntPtr hWnd, ulong maximun)
            {
                if (!IsSupported)
                    throw new NotSupportedException();
                if (hWnd == IntPtr.Zero)
                    throw new ArgumentException("", "hWnd");
                if (maximun == 0ul)
                    throw new ArgumentOutOfRangeException("", "maximum");

                this.Handle = hWnd;
                this.Maximun = maximun;
            }

            #endregion Constructors

            #region Classes

            [ComImport]
            [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
            [ClassInterface(ClassInterfaceType.None)]
            private class Taskbar
            {
            }

            #endregion Classes

            #region Fields

            private const ulong MAXIMUM = 1ul;
            private readonly static ITaskbar s_taskbar;

            #endregion Fields

            #region Properties

            public static bool IsSupported { get; }

            public IntPtr Handle { get; }

            public double Maximun { get; set; } = MAXIMUM;

            #endregion Properties

            #region Methods

            public void Reset()
            {
                s_taskbar.SetProgressState(this.Handle, ProgressStatus.NoProgress);
                s_taskbar.SetProgressValue(this.Handle, 0ul, (ulong)this.Maximun);
            }

            public void SetState(ProgressStatus status)
            {
                s_taskbar.SetProgressState(this.Handle, status);
            }

            public void SetValue(double progressValue)
            {
                this.SetValue((ulong)progressValue);
            }

            public void SetValue(ulong progressValue)
            {
                if (this.Maximun < progressValue || 0ul > progressValue)
                    throw new ArgumentOutOfRangeException(nameof(progressValue));

                s_taskbar.SetProgressValue(this.Handle, progressValue, (ulong)this.Maximun);
            }

            #endregion Methods
        }

        public class IconProgressBarEnumerator<T>
        {
            #region Constructors

            public IconProgressBarEnumerator(IntPtr hWnd, IEnumerable<T> collection, Action<T> onExecute, Action<Exception> onError, Action onCompleted)
            {
                this._list = collection?.ToList() ?? throw new ArgumentNullException();
                this._bar = new IconProgressBar(hWnd, (ulong)this._list.Count);
                this.OnExecute = onExecute;
                this.OnError = onError;
                this.OnCompleted = onCompleted;
            }

            #endregion Constructors

            #region Fields

            private readonly IconProgressBar _bar;
            private readonly IList<T> _list;

            #endregion Fields

            #region Properties

            public Action OnCompleted { get; }

            public Action<Exception> OnError { get; }

            public Action<T> OnExecute { get; }

            #endregion Properties

            #region Methods

            public async Task RunAsync(CancellationToken cancellationToken = default(CancellationToken))
            {
                await Task.Run(() =>
                {
                    this._bar.SetState(ProgressStatus.NoProgress);
                    try
                    {
                        this._bar.SetState(ProgressStatus.Normal);
                        var count = (ulong)this._list.Count;
                        for (var index = 0ul; index < count; index++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                this._bar.SetState(ProgressStatus.Indeterminate);
                                break;
                            }
                            this._bar.SetValue(index);
                            this.OnExecute?.Invoke(this._list[(int)index]);
                        }
                    }
                    catch (Exception e)
                    {
                        this._bar.SetState(ProgressStatus.Error);
                        this.OnError?.Invoke(e);
                    }

                    this.OnCompleted?.Invoke();
                    this._bar.Reset();
                }, cancellationToken);
            }

            #endregion Methods
        }

        #endregion Classes
    }
}