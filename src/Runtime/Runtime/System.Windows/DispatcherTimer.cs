
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
using OpenSilver;

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
        /// Initializes a new instance of the <see cref="DispatcherTimer"/> class
        /// which processes timer events at the specified priority.
        /// </summary>
        /// <param name="priority">The priority at which to invoke the timer.</param>
        [NotImplemented]
        public DispatcherTimer(DispatcherPriority priority)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherTimer"/> class
        /// which runs on the specified <see cref="Dispatcher"/> at the specified priority.
        /// </summary>
        /// <param name="priority">The priority at which to invoke the timer.</param>
        /// <param name="dispatcher">The dispatcher the timer is associated with.</param>
        [NotImplemented]
        public DispatcherTimer(DispatcherPriority priority, Dispatcher dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherTimer"/> class
        /// which uses the specified time interval, priority, event handler, and <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="interval">The period of time between ticks.</param>
        /// <param name="priority">The priority at which to invoke the timer.</param>
        /// <param name="callback">The event handler to call when the <see cref="Tick"/> event occurs.</param>
        /// <param name="dispatcher">The dispatcher the timer is associated with.</param>
        [NotImplemented]
        public DispatcherTimer(TimeSpan interval, DispatcherPriority priority, EventHandler callback, Dispatcher dispatcher)
        {
            Interval = interval;

            Tick += callback;
            Start();
        }

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
        /// Gets or sets a value that indicates whether the timer is running.
        /// </summary>
        /// <returns>
        /// true if the timer is enabled; otherwise, false. The default is false.
        /// </returns>
        public bool IsEnabled
        {
            get => _timer is not null;
            set
            {
                if (_timer is not null && !value)
                {
                    Stop();
                }
                else if (_timer is null && value)
                {
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets a user-defined data object.
        /// </summary>
        /// <returns>
        /// The user-defined data. The default is null.
        /// </returns>
        public object Tag { get; set; }

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
                        timer.FireTick();
                    }
                });
            }
            else
            {
                timer.FireTick();
            }
        }

        private void FireTick()
        {
            if (OpenSilverCompatibilityPreferences.HandleDispatcherTimerExceptions)
            {
                var oldSynchronizationContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(Dispatcher.CurrentDispatcher.DefaultSynchronizationContext);

                try
                {
                    OnTick();
                }
                catch (Exception ex)
                {
                    bool handled = Application.CallHandleException(ex);

                    if (!handled)
                    {
                        throw;
                    }
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
                }
            }
            else
            {
                OnTick();
            }
        }
    }
}
