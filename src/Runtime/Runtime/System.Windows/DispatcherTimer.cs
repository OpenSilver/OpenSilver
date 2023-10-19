

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

using System;
using DotNetForHtml5.Core;
using System.Threading;

namespace System.Windows.Threading
{
    /// <summary>
    /// Provides a timer that is integrated into the Dispatcher queue, which is processed
    /// at a specified interval of time and at a specified priority.  One scenario
    /// for this is to run code on the UI thread.
    /// </summary>
    /// <example>
    /// Here is how you can create and use a DispatcherTimer:
    /// <code lang="C#">
    /// DispatcherTimer _dispatcherTimer;
    /// </code>
    /// <code lang="C#">
    /// //We create a new instance of DispatcherTimer:
    /// _dispatcherTimer = new Dispatchertimer();
    /// //we set the time between each tick of the DispatcherTimer to 100 milliseconds:
    /// timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
    /// _dispatcherTimer.Tick += DispatcherTimer_Tick;
    /// _dispatcherTimer.Start();
    /// </code>
    /// <code lang="C#">
    /// void DispatcherTimer_Tick(object sender, object e)
    /// {
    ///     //Some code to execute at each Tick of the DispatcherTimer.
    /// }
    /// </code>
    /// When you want to stop the DispatcherTimer, you can use the following code:
    /// <code lang="C#">
    ///     _dispatcherTimer.Stop();
    /// </code>
    /// </example>
    public class DispatcherTimer
    {
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the DispatcherTimer class.
        /// </summary>
        public DispatcherTimer() { }

        private TimeSpan _interval;
        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                _interval = value;

                // Restart the timer if the Interval has been modified while the timer was running:
                if (_timer == null) return;
                Stop();
                Start();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the timer is running.
        /// </summary>
        public bool IsEnabled => _timer != null;

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler Tick;
        
        /// <summary>
        /// Raises the Tick event.
        /// </summary>
        protected void OnTick()
        {
            if (_timer == null) { return; }

            Tick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Starts the DispatcherTimer.
        /// </summary>
        public void Start()
        {
            if (_timer != null) return;
            var intervalInMilliseconds = (long)_interval.TotalMilliseconds;
            if (intervalInMilliseconds == 0)
                intervalInMilliseconds = 1; // Note: this appears to be the default behavior of other XAML platforms.

            _timer = new Timer(
                delegate
                {
                    if (INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround)
                    {
                        INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(() =>
                        {
                            //It is important to do this check on the UI thread
                            if (_timer == null)
                            {
                                return;
                            }
                            OnTick();
                        });
                        return;
                    }

                    OnTick();
                },
                null,
                intervalInMilliseconds,
                intervalInMilliseconds);
        }

        /// <summary>
        /// Stops the DispatcherTimer.
        /// </summary>
        public void Stop()
        {
            if (_timer == null) return;
            _timer.Dispose();
            _timer = null;
        }
    }
}