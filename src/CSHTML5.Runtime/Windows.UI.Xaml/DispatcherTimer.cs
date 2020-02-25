
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
        global::System.Threading.Timer _timer;
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
            get { return _timer != null; }
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
                _timer = StartTimer(OnTick, intervalInMilliseconds);
            }
        }

        /// <summary>
        /// Stops the DispatcherTimer.
        /// </summary>
        public void Stop()
        {
            if (_timer != null)
            {
                StopTimer(_timer);
                _timer = null;
            }
        }

#if !BRIDGE
        [JSReplacement("setInterval($action,$intervalInMilliseconds)")]
#else
        [Template("setInterval({action},{intervalInMilliseconds})")]
#endif
        static
#if CSHTML5NETSTANDARD
        global::System.Threading.Timer
#else
        object
#endif
        StartTimer(Action action, long intervalInMilliseconds)
        {
#if !CSHTML5NETSTANDARD
            object dispatcherTimer = INTERNAL_Simulator.SimulatorProxy.StartDispatcherTimer(action, intervalInMilliseconds);
            return dispatcherTimer;
#else
            global::System.Threading.Timer timer = new global::System.Threading.Timer(
                delegate (object state)
                { 
                    action(); 
                },
                null,
                intervalInMilliseconds,
                intervalInMilliseconds);
            return timer;
#endif
            //            global::System.Threading.Timer timer = new global::System.Threading.Timer(
            //                delegate(object state)
            //                {
            //#if !CSHTML5NETSTANDARD
            //                    INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
            //                    {
            //#endif
            //                        action();
            //#if !CSHTML5NETSTANDARD
            //                    }));
            //#endif
            //                },
            //                null,
            //                intervalInMilliseconds,
            //                intervalInMilliseconds);
            //            return timer;
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("clearInterval($timer)")]
#else
        [Template("clearInterval({timer})")]

#endif
        static void StopTimer(object timer) //Note: "timer" is of type "object" because otherwise JSIL is unable to do the "JSReplacement" of the "StopTimer" method.
        {
#if !CSHTML5NETSTANDARD
            INTERNAL_Simulator.SimulatorProxy.StopDispatcherTimer(timer);
#else
            ((global::System.Threading.Timer)timer).Dispose();
#endif
        }
    }
}