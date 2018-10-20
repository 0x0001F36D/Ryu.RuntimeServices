// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.RuntimeServices.Callable.Internal
{
    using System;
    using System.Diagnostics;

    internal class PerformanceCalculator : IDisposable
    {
        #region Constructors

        static PerformanceCalculator()
        {
            using (var v = new PerformanceCalculator(null))
                v.Start();
        }

        public PerformanceCalculator(EventHandler<TimeSpan> onStop)
        {
            this._stopwatch = new Stopwatch();
            this._onStop += onStop;
        }

        #endregion Constructors

        #region Fields

        private readonly Stopwatch _stopwatch;

        #endregion Fields

        #region Events

        private event EventHandler<TimeSpan> _onStop;

        #endregion Events

        #region Properties

        public static PerformanceCalculator Default
        {
            get
            {
                var p = new PerformanceCalculator((sender, e) => Console.WriteLine(e));
                return p;
            }
        }

        public static PerformanceCalculator StartNew
        {
            get
            {
                var p = new PerformanceCalculator((sender, e) => Console.WriteLine(e));
                p.Start();
                return p;
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose() => this.Stop();

        public void ForLoop(Action action, int count)
        {
            this.Stop();

            if (action == null || count < 0)
                return;

            for (; count >= 0; count--)
            {
                this.Start();
                action();
                this._stopwatch.Stop();
                this._onStop?.Invoke(this, this._stopwatch.Elapsed);
            }
        }

        public void Signal(bool reset)
        {
            this.Stop();
            if (reset)
                this._stopwatch.Restart();
            else
                this._stopwatch.Start();
        }

        public void Start()
                                    => this._stopwatch.Restart();

        private void Stop()
        {
            if (this._stopwatch.IsRunning)
            {
                this._stopwatch.Stop();
                this._onStop?.Invoke(this, this._stopwatch.Elapsed);
            }
        }

        #endregion Methods
    }
}