

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Threading
#else
namespace Windows.UI.Xaml
#endif
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
    public partial class DispatcherTimer
    {
#if CSHTML5NETSTANDARD
        dynamic _timer;
#else
        object _timer; //Note: "timer" is of type "object" because otherwise JSIL is unable to do the "JSReplacement" of the "StopTimer" method.
#endif

        /// <summary>
        /// Initializes a new instance of the DispatcherTimer class.
        /// </summary>
        public DispatcherTimer() { }

        TimeSpan _interval = new TimeSpan();
        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;

                // Restart the timer if the Interval has been modified while the timer was running:
                if (_timer != null)
                {
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the timer is running.
        /// </summary>
        public bool IsEnabled
        {
            get { return _timer == null ? false : _timer.IsEnabled; }
        }

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
#if MIGRATION
        public event EventHandler Tick;
#else
        public event EventHandler<object> Tick;
#endif
        /// <summary>
        /// Raises the Tick event.
        /// </summary>
        protected void OnTick()
        {
            if (Tick != null)
            {
                Tick(this, new EventArgs());
            }
        }

        /// <summary>
        /// Starts the DispatcherTimer.
        /// </summary>
        public void Start()
        {
            if (_timer == null)
            {
                long intervalInMilliseconds = (long)_interval.TotalMilliseconds; //(long)(_interval.TotalSeconds * 1000);
                if (intervalInMilliseconds == 0)
                    intervalInMilliseconds = 1; // Note: this appears to be the default bahavior of other XAML platforms.

                if (OpenSilver.Interop.IsRunningInTheSimulator)
                    _timer = INTERNAL_Simulator.SimulatorProxy.CreateOSDispatcherTimer((Action)OnTick, TimeSpan.FromMilliseconds(intervalInMilliseconds));
                else
                    _timer = new global::System.Threading.Timer((e) => OnTick(), null, intervalInMilliseconds, intervalInMilliseconds);
            }

            _timer.Start();
        }

        /// <summary>
        /// Stops the DispatcherTimer.
        /// </summary>
        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }
    }
}