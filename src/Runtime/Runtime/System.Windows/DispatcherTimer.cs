
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Diagnostics;
using System.Threading;
using DotNetForHtml5.Core;

namespace System.Windows.Threading
{
    /// <summary>
    /// A timer that is integrated into the <see cref="Dispatcher"/> queue, which is 
    /// processed at a specified interval of time and at a specified priority.
    /// </summary>
    public class DispatcherTimer
    {
        private Timer _timer;
        private TimeSpan _interval;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherTimer"/> class.
        /// </summary>
        public DispatcherTimer() { }

        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        /// <returns>
        /// The amount of time between ticks. The default is <see cref="TimeSpan.Zero"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified value when setting this property represents a negative time interval.
        /// </exception>
        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                if (value.TotalMilliseconds < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _interval = value;

                if (_timer is not null)
                {
                    UpdateTimer();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the timer is running.
        /// </summary>
        public bool IsEnabled => _timer is not null;

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// Raises the Tick event.
        /// </summary>
        protected void OnTick() => Tick?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Starts the <see cref="DispatcherTimer"/>.
        /// </summary>
        public void Start() => _timer ??= new Timer(OnTimerTick, this, _interval, _interval);

        /// <summary>
        /// Stops the <see cref="DispatcherTimer"/>.
        /// </summary>
        public void Stop()
        {
            if (_timer is not null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void UpdateTimer()
        {
            Debug.Assert(_timer is not null);
            _timer.Change(_interval, _interval);
        }

        private static void OnTimerTick(object state)
        {
            var timer = (DispatcherTimer)state;
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                var internalTimer = timer._timer;
                INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(() =>
                {
                    if (internalTimer == timer._timer)
                    {
                        timer.OnTick();
                    }
                });
            }
            else
            {
                timer.OnTick();
            }
        }
    }
}
