// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.Windows.Shell
{
    using Ryuko.Windows.Shell.Enums;

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class TaskbarProgressBar : INotifyPropertyChanged, IDisposable
    {
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
            void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

            [PreserveSig]
            void SetProgressState(IntPtr hwnd, ProgressStatus state);
        }

        [ComImport]
        [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class Taskbar
        {
        }

        private readonly static ITaskbar s_taskbar;

        private readonly bool _autoReset;

        private readonly PropertyChangedEventArgs _statusChangedEventArgs;

        private readonly PropertyChangedEventArgs _valueChangedEventArgs;

        private ulong _maxValue;

        private IntPtr _ownerHandle;

        private ProgressStatus _status;

        private ulong _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public IntPtr Handle
        {
            get
            {
                if (this._ownerHandle == IntPtr.Zero)
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    if (currentProcess == null || currentProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        throw new InvalidOperationException();
                    }
                    this._ownerHandle = currentProcess.MainWindowHandle;
                }
                return this._ownerHandle;
            }
        }

        public ulong MaxValue
        {
            get => this._maxValue;
            set
            {
                if (value == 0u)
                    value++;
                this._maxValue = value;
            }
        }

        public ProgressStatus Status
        {
            get => this._status;
            set
            {
                this._status = value;
                s_taskbar.SetProgressState(this.Handle, value);
                this.PropertyChanged?.Invoke(this, this._statusChangedEventArgs);
            }
        }

        public ulong Value
        {
            get => this._value;
            set
            {
                if (value > this.MaxValue)
                    value = this.MaxValue;

                if (value == this.MaxValue && this._autoReset)
                    this.Init();
                else
                {
                    this._value = value;
                    s_taskbar.SetProgressValue(this.Handle, value, this.MaxValue);
                }

                this.PropertyChanged?.Invoke(this, this._valueChangedEventArgs);
            }
        }

        static TaskbarProgressBar()
        {
            s_taskbar = (ITaskbar)new Taskbar();
        }

        public TaskbarProgressBar(ulong maximum, bool autoReset = false)
        {
            var supported = Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0;
            if (!supported)
                throw new PlatformNotSupportedException();

            this._valueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
            this._statusChangedEventArgs = new PropertyChangedEventArgs(nameof(Status));

            this.MaxValue = maximum;
            this._autoReset = autoReset;
            this.Init();
        }

        void IDisposable.Dispose()
        {
            this.Init();
        }

        public void Init()
        {
            this.Status = ProgressStatus.NoProgress;
            s_taskbar.SetProgressValue(this.Handle, 0, this.MaxValue);
        }
    }
}