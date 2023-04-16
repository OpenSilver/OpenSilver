
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
using System.Collections.Generic;
using System.Reflection;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Defines a segment of time.
    /// </summary>
    public abstract partial class Timeline : DependencyObject
    {
        internal INameResolver NameResolver { get; set; }

        internal DependencyProperty GetProperty(DependencyObject target, PropertyPath propertyPath)
        {
            if (propertyPath.DependencyProperty != null)
            {
                return propertyPath.DependencyProperty;
            }

            return INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(
                target.GetType(),
                propertyPath.SVI[propertyPath.SVI.Length - 1].propertyName
            );
        }

        // Returns:
        //     The timeline's simple duration: the amount of time this timeline takes to
        //     complete a single forward iteration. The default value is a Duration that
        //     evaluates as Automatic.
        /// <summary>
        /// Gets or sets the length of time for which this timeline plays, not counting
        /// repetitions.
        /// </summary>
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        /// <summary>
        /// Identifies the Duration dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration), typeof(Timeline), new PropertyMetadata(Duration.Automatic));



        // Returns:
        //     An iteration Count that specifies the number of times the timeline should
        //     play, a TimeSpan value that specifies the total length of this timeline's
        //     active period, or the special value Forever, which specifies that the timeline
        //     should repeat indefinitely. The default value is a RepeatBehavior with a
        //     Count value of 1, which indicates that the timeline plays once.
        /// <summary>
        /// Gets or sets the repeating behavior of this timeline.
        /// </summary>
        public RepeatBehavior RepeatBehavior
        {
            get { return (RepeatBehavior)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }
        //Note: In WPF, repeatBehavior seems to do nothing if not set on the Storyboard (so basically, setting it on a DoubleAnimation for example is useless).
        //      I don't know why this is in Timeline instead of Storyboard then.

        /// <summary>
        /// Identifies the RepeatBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatBehaviorProperty =
            DependencyProperty.Register("RepeatBehavior", typeof(RepeatBehavior), typeof(Timeline), new PropertyMetadata(new RepeatBehavior(1)));


        // Returns:
        //     The time at which this System.Windows.Media.Animation.Timeline should begin,
        //     relative to its parent's System.Windows.Media.Animation.Timeline.BeginTime.
        //     If this timeline is a root timeline, the time is relative to its interactive
        //     begin time (the moment at which the timeline was triggered). This value may
        //     be positive, negative, or null; a null value means the timeline never plays.
        //     The default value is zero.
        /// <summary>
        /// Gets or sets the time at which this System.Windows.Media.Animation.Timeline
        /// should begin.
        /// </summary>
        public TimeSpan? BeginTime
        {
            get { return (TimeSpan?)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }
        /// <summary>
        /// Identifies the BeginTime dependency property.
        /// </summary>
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan?), typeof(Timeline), new PropertyMetadata(new TimeSpan()));

        /// <summary>
        /// Occurs when this timeline has completely finished playing: it will no longer
        /// enter its active period.
        /// </summary>
        public event EventHandler Completed;
        internal void INTERNAL_RaiseCompletedEvent()
        {
            if (Completed != null)
                Completed(this, new EventArgs());
        }

        internal HashSet<Guid> CompletedGuids = new HashSet<Guid>();
        internal void INTERNAL_RaiseCompletedEvent(Guid guid)
        {
            if (CompletedGuids == null)
            {
                CompletedGuids = new HashSet<Guid>();
            }
            if (!CompletedGuids.Contains(guid))
            {
                CompletedGuids.Add(guid);
            }
            if (Completed != null)
                Completed(this, new EventArgs());
        }

        internal virtual void Stop(IterationParameters parameters, bool revertToFormerValue = false)
        {
            _animationTimer.Stop();
            if (_beginTimeTimer != null)
            {
                _beginTimeTimer.Stop();
            }
        }

        /// <summary>
        /// Removes the value set for the VisualState on the specified frameworkElement.
        /// </summary>
        /// <param name="frameworkElement"></param>
        internal void UnApply(FrameworkElement frameworkElement)
        {
            if (frameworkElement is Control)
            {
                GetTargetElementAndPropertyInfo(_parameters, out DependencyObject target, out PropertyPath propertyPath);
                if (target != null && propertyPath != null)
                {
                    AnimationHelpers.ApplyValue(target, propertyPath, DependencyProperty.UnsetValue);
                }
            }
        }

        internal void GetTargetElementAndPropertyInfo(
            IterationParameters parameters,
            out DependencyObject target,
            out PropertyPath propertyPath)
        {
            propertyPath = null;
            target = null;

            if (parameters != null && parameters.TimelineMappings.TryGetValue(this, out Tuple<DependencyObject, PropertyPath> info))
            {
                propertyPath = info.Item2;
                target = propertyPath.GetFinalItem(info.Item1);
            }
        }

        internal void GetPropertyPathAndTargetBeforePath(
            IterationParameters parameters,
            out DependencyObject targetBeforePath,
            out PropertyPath propertyPath)
        {
            Tuple<DependencyObject, PropertyPath> info = parameters.TimelineMappings[this];

            targetBeforePath = info.Item1;
            propertyPath = info.Item2;
        }

        //Stuff added for loops purposes:

        //How loops work:
        //  The Timeline calls IterateOnce the first time, which starts the timer that handles the duration of the animation if any, and starts the animation.
        //  When the animation is over or the timer ticks, we call OnIterationCompleted which checks if there should be one more loop.
        //      In any case, we stop the current animation and if we loop, we call IterateOnce, otherwise, we raise the completed event.
        //  If the Timeline is a Storyboard, IterateOnce also calls InitializeIteration which sets (initializes) the amount of loops we should do on itself and every Timeline in its children.

        ///
        //Note: the Guid below is used to make a different name for each animation in velocity. That way, a storyboard with looping animations will be able to
        //      stop the animations separately when they finish their loop (otherwise, the first animation to finish will stop all the animations in velocity and the other
        //      animations won't be able to loop since velocity will never call the animation completed for them).
        internal readonly string animationInstanceSpecificName = Guid.NewGuid().ToString(); //this will allow us to stop a specific animation from whithin a Storyboard with multiple animations

        //Note on BeginTime:
        //  It delays the first loop so we added a method (StartFirstIteration) before calling IterateOnce where it was fit.
        //  Ultimately, we might start the timers for the whole tree of animations at once and add each animation's delay to their parents' delays, which will allow us to handle negative delays.
        DispatcherTimer _beginTimeTimer;
        /// <summary>
        /// Starts the first iteration of this timeline, while managing the BeginTime property.
        /// </summary>
        /// <param name="parameters">the parameters required for the iteration</param>
        /// <param name="isLastLoop">A boolean that says if it is the last loop</param>
        /// <param name="parentDelay">The Delay due to the BeginTime of the parent Timeline</param>
        internal void StartFirstIteration(IterationParameters parameters,  bool isLastLoop, TimeSpan? parentDelay)
        {
            if (BeginTime != null)
            {
                if (BeginTime.Value.TotalMilliseconds > 0)
                {
                    _beginTimeTimer = new DispatcherTimer(); //this line is to avoid having more than one callback on the tick (we cannot use "_beginTimeTimer.Tick -= XXX" since we use a anonymous method).
                    _beginTimeTimer.Interval = BeginTime.Value;

                    //Note: anonymous method since it allows us to use simply parameters and isLastLoop.
                    _beginTimeTimer.Tick += (sender, args) =>
                    {
                        _beginTimeTimer.Stop();
                        IterateOnce(parameters, isLastLoop);
                    };
                    _beginTimeTimer.Start();
                }
                else
                {
                    IterateOnce(parameters, isLastLoop);
                }
            }
        }


        internal virtual void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            _parameters = parameters;
            Duration duration = ResolveDuration();
            if (duration.HasTimeSpan && duration.TimeSpan.TotalMilliseconds > 0)
            {
                _animationTimer.Interval = duration.TimeSpan;
                _animationTimer.Tick -= _animationTimer_Tick;
                _animationTimer.Tick += _animationTimer_Tick;
                _isAnimationDurationReached = false;
                _animationTimer.Start();
                return;
            } 
            
            _isAnimationDurationReached = true;
        }


        double remainingIterations = double.NaN;

        internal void InitializeIteration()
        {
            if (RepeatBehavior == null)
            {
                remainingIterations = 1;
            }
            else if (RepeatBehavior != RepeatBehavior.Forever)
            {
                remainingIterations = RepeatBehavior.Count;
            }
            else
            {
                remainingIterations = double.PositiveInfinity;
            }
        }

        DispatcherTimer _animationTimer = new DispatcherTimer();
        internal void OnIterationCompleted(IterationParameters parameters)
        {
            if (_isAnimationDurationReached) //the default duration is Automatic, which currently has a TimeSpan of 0 ms (which is considered here to be no timespan).
            {
                --remainingIterations;
                if (remainingIterations <= 0)
                {
                    Stop(parameters, revertToFormerValue: false);
                    INTERNAL_RaiseCompletedEvent();
                }
                else
                {
                    if (this is Storyboard storyboard)
                    {
                        storyboard.Stop();
                    }
                    else
                    {
                        Stop(parameters, revertToFormerValue: true);
                    }

                    IterateOnce(parameters, isLastLoop: remainingIterations == 1);
                }
            }
        }

        bool _isAnimationDurationReached = false;
        internal IterationParameters _parameters;
        void _animationTimer_Tick(object sender, object e)
        {
            _animationTimer.Stop();
            _isAnimationDurationReached = true;
            OnIterationCompleted(_parameters);
        }

        /// <summary>
        /// Implemented by the class author to provide a custom natural Duration
        /// in the case that the Duration property is set to Automatic.  If the author
        /// cannot determine the Duration, this method should return Automatic.
        /// </summary>
        /// <returns>
        /// A Duration quantity representing the natural duration.
        /// </returns>
        protected virtual Duration GetNaturalDurationCore()
        {
            return Duration.Automatic;
        }

        internal Duration ResolveDuration()
        {
            Duration duration = Duration;
            if (duration == Duration.Automatic)
            {
                return GetNaturalDurationCore();
            }

            return duration;
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register("SpeedRatio", typeof(double), typeof(Timeline), new PropertyMetadata(1d));

        [OpenSilver.NotImplemented]
        public double SpeedRatio
        {
            get { return (double)this.GetValue(Timeline.SpeedRatioProperty); }
            set { this.SetValue(Timeline.SpeedRatioProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AutoReverseProperty = DependencyProperty.Register("AutoReverse", typeof(bool), typeof(Timeline), null);
        [OpenSilver.NotImplemented]
        public bool AutoReverse
        {
            get { return (bool)this.GetValue(AutoReverseProperty); }
            set { this.SetValue(AutoReverseProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FillBehaviorProperty = DependencyProperty.Register("FillBehavior", typeof(FillBehavior), typeof(Timeline), null);
        [OpenSilver.NotImplemented]
        public FillBehavior FillBehavior
        {
            get { return (FillBehavior)this.GetValue(FillBehaviorProperty); }
            set { this.SetValue(FillBehaviorProperty, value); }
        }
    }
}