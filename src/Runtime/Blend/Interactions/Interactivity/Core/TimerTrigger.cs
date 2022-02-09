// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity.Core
{
    using System;
    using System.Windows.Threading;

#if MIGRATION
    using System.Windows;
#else
    using Windows.UI.Xaml;
#endif

    /// <summary>
    /// A trigger that is triggered by a specified event occurring on its source and fires after a delay when that event is fired.
    /// </summary>
    public class TimerTrigger : System.Windows.Interactivity.EventTrigger
    {
        public static readonly DependencyProperty MillisecondsPerTickProperty = DependencyProperty.Register("MillisecondsPerTick",
                                                                                                    typeof(double),
                                                                                                    typeof(TimerTrigger),
#if __WPF__
																									new FrameworkPropertyMetadata(1000.0)
#else
                                                                                                    new PropertyMetadata(1000.0)
#endif
                                                                                                    );

        public static readonly DependencyProperty TotalTicksProperty = DependencyProperty.Register("TotalTicks",
                                                                                                    typeof(int),
                                                                                                    typeof(TimerTrigger),
#if __WPF__
																									new FrameworkPropertyMetadata(-1)
#else
                                                                                                    new PropertyMetadata(-1)
#endif
                                                                                                    );

        private ITickTimer timer;
#if MIGRATION
        private EventArgs eventArgs;
#else
        private RoutedEventArgs eventArgs;
#endif
        private int tickCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTrigger"/> class.
        /// </summary>
        public TimerTrigger() :
            this(new DispatcherTickTimer())
        {
        }

        internal TimerTrigger(ITickTimer timer)
        {
            this.timer = timer;
        }


        /// <summary>
        /// Gets or sets the number of milliseconds to wait between ticks. This is a dependency property.
        /// </summary>
        public double MillisecondsPerTick
        {
            get { return (double)this.GetValue(MillisecondsPerTickProperty); }
            set { this.SetValue(MillisecondsPerTickProperty, value); }
        }

        /// <summary>
        /// Gets or sets the total number of ticks to be fired before the trigger is finished.  This is a dependency property.
        /// </summary>
        public int TotalTicks
        {
            get { return (int)this.GetValue(TotalTicksProperty); }
            set { this.SetValue(TotalTicksProperty, value); }
        }

#if MIGRATION
        protected override void OnEvent(EventArgs eventArgs)
#else
        protected override void OnEvent(RoutedEventArgs eventArgs)
#endif
        {
            this.StopTimer();

            this.eventArgs = eventArgs;
            this.tickCount = 0;

            this.StartTimer();
        }

        protected override void OnDetaching()
        {
            this.StopTimer();

            base.OnDetaching();
        }

        internal void StartTimer()
        {
            if (this.timer != null)
            {
                this.timer.Interval = TimeSpan.FromMilliseconds(this.MillisecondsPerTick);
                this.timer.Tick += this.OnTimerTick;
                this.timer.Start();
            }
        }

        internal void StopTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Tick -= this.OnTimerTick;
            }
        }

#if MIGRATION
        private void OnTimerTick(object sender, EventArgs e)
#else
        private void OnTimerTick(object sender, object e)
#endif
        {
            if (this.TotalTicks > 0 && ++this.tickCount >= this.TotalTicks)
            {
                this.StopTimer();
            }

            this.InvokeActions(this.eventArgs);
        }

        internal class DispatcherTickTimer : ITickTimer
        {
            private DispatcherTimer dispatcherTimer;

            public DispatcherTickTimer()
            {
                this.dispatcherTimer = new DispatcherTimer();
            }

#if MIGRATION
            public event EventHandler Tick
#else
            public event EventHandler<object> Tick
#endif
            {
                add { this.dispatcherTimer.Tick += value; }
                remove { this.dispatcherTimer.Tick -= value; }
            }

            public TimeSpan Interval
            {
                get { return this.dispatcherTimer.Interval; }
                set { this.dispatcherTimer.Interval = value; }
            }

            public void Start()
            {
                this.dispatcherTimer.Start();
            }

            public void Stop()
            {
                this.dispatcherTimer.Stop();
            }
        }
    }
}
