using System;
using System.Windows.Threading;

namespace OpenSilver.Simulator
{
    public class OSDispatcherTimer
    {
        private DispatcherTimer _DispatcherTimer;
        public bool IsEnabled { get { return _DispatcherTimer.IsEnabled; } set { _DispatcherTimer.IsEnabled = value; } }

        public OSDispatcherTimer(Action tickAction, TimeSpan interval)
        {
            _DispatcherTimer = new DispatcherTimer();
            _DispatcherTimer.Interval = interval;
            _DispatcherTimer.Tick += (s, e) => tickAction();
        }

        public void Start()
        {
            _DispatcherTimer.Start();
        }

        public void Stop()
        {
            _DispatcherTimer.Stop();
        }
    }
}
