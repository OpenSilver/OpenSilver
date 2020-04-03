

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
#if MIGRATION
        internal static readonly string[] defaultTypesPaths = {
                                                                 "", //keep an empty one to test the type directly.
                                                                 "System.Windows.Xaml.",
                                                                 "System.Windows.Controls.",
                                                                 "System.Windows.Controls.Primitives.",
                                                                 "System.Windows.Media.",
                                                                 "System.Windows.Media.Animation.",
                                                                 "System.Windows.Media.Imaging.",
                                                                 "System.Windows.Shapes.",
                                                              };
#else
        internal static readonly string[] defaultTypesPaths = {
                                                                 "", //keep an empty one to test the type directly.
                                                                 "Windows.UI.Xaml.",
                                                                 "Windows.UI.Xaml.Controls.",
                                                                 "Windows.UI.Xaml.Controls.Primitives.",
                                                                 "Windows.UI.Xaml.Media.",
                                                                 "Windows.UI.Xaml.Media.Animation.",
                                                                 "Windows.UI.Xaml.Media.Imaging.",
                                                                 "Windows.UI.Xaml.Shapes.",
                                                             };
#endif

        internal DependencyProperty GetProperty(DependencyObject target, PropertyPath propertyPath)
        {
            if (propertyPath.INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                return propertyPath.INTERNAL_DependencyProperty;
            }
            Type lastElementType = target.GetType();
            Type dependencyPropertyContainerType = null;
            PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);
            if (propertyInfo != null)
            {
                dependencyPropertyContainerType = propertyInfo.DeclaringType;
            }
            else
            {
                string[] splittedPath = propertyPath.Path.Split('('); //example: "(Canvas.Left)" --> ["", "Canvas.Left)"]
                string end = splittedPath[splittedPath.Length - 1];
                string[] splittedEnd = end.Split('.'); //with the example above: --> ["Canvas", "Left)"]
                string parentTypeString = end.Substring(0, end.Length - splittedEnd[splittedEnd.Length - 1].Length - 1); // -1 to remove the dot that wasn't counted due to the split.
                parentTypeString = parentTypeString.StartsWith("global::") ? parentTypeString.Substring(8) : parentTypeString;
                int i = 0;
                //todo: find a way to handle user defined types
                //todo: find a better way to do the following
                while (i < defaultTypesPaths.Length && dependencyPropertyContainerType == null)
                {
                    dependencyPropertyContainerType = Type.GetType(defaultTypesPaths[i] + parentTypeString);
                    ++i;
                }
                if (dependencyPropertyContainerType == null)
                {
                    throw new InvalidOperationException("Type \"" + parentTypeString + "\"could not be found.");
                }
            }
            FieldInfo dependencyPropertyField = dependencyPropertyContainerType.GetField(propertyPath.INTERNAL_DependencyPropertyName + "Property");

            // - Get the DependencyProperty (since Bridge, the GetValue parameter must be null because DependencyProperties are always static)
            DependencyProperty dp = (DependencyProperty)dependencyPropertyField.GetValue(null);
            return dp;
        }

        ///// <summary>
        ///// Provides base class initialization behavior for Timeline-derived classes.
        ///// </summary>
        //protected Timeline()
        //{

        //}

        //// The default is false.
        ///// <summary>
        ///// Gets or sets a value that determines whether the timeline should be permitted
        ///// to run on properties where the animation is considered a dependent animation.
        ///// </summary>
        //public static bool AllowDependentAnimations { get; set; }

        //// The default value is false.
        ///// <summary>
        ///// Gets or sets a value that indicates whether the timeline plays in reverse
        ///// after it completes a forward iteration.
        ///// </summary>
        //public bool AutoReverse { get; set; }

        ///// <summary>
        ///// Identifies the AutoReverse dependency property.
        ///// </summary>
        //public static DependencyProperty AutoReverseProperty { get; }

        //// Returns:
        ////     The start time of the time line. The default value is zero. If you are programming
        ////     using C# or Visual Basic, the parameter type of this parameter is projected
        ////     as System.TimeSpan? (a nullable System.TimeSpan).
        ///// <summary>
        ///// Gets or sets the time at which this Timeline should begin.
        ///// </summary>
        //public TimeSpan? BeginTime { get; set; }

        ///// <summary>
        ///// Identifies the BeginTime dependency property.
        ///// </summary>
        //public static DependencyProperty BeginTimeProperty { get; }

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
            DependencyProperty.Register("Duration", typeof(Duration), typeof(Timeline), new PropertyMetadata(Duration.Automatic)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



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
            DependencyProperty.Register("RepeatBehavior", typeof(RepeatBehavior), typeof(Timeline), new PropertyMetadata(new RepeatBehavior(1))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


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
            DependencyProperty.Register("BeginTime", typeof(TimeSpan?), typeof(Timeline), new PropertyMetadata(new TimeSpan())
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });





        //// Returns:
        ////     A value that specifies how the timeline behaves after it reaches the end
        ////     of its active period but its parent is inside its active or fill period.
        ////     The default value is HoldEnd.
        ///// <summary>
        ///// Gets or sets a value that specifies how the animation behaves after it reaches
        ///// the end of its active period.
        ///// </summary>
        //public FillBehavior FillBehavior { get; set; }
        ///// <summary>
        ///// Identifies the FillBehavior dependency property.
        ///// </summary>
        //public static DependencyProperty FillBehaviorProperty { get; }

        //// Returns:
        ////     A finite value greater than 0 that specifies the rate at which time progresses
        ////     for this timeline, relative to the speed of the timeline's parent. If this
        ////     timeline is a root timeline, specifies the default timeline speed. The value
        ////     is expressed as a factor where 1 represents normal speed, 2 is double speed,
        ////     0.5 is half speed, and so on. The default value is 1.
        ///// <summary>
        ///// Gets or sets the rate, relative to its parent, at which time progresses for
        ///// this Timeline.
        ///// </summary>
        //public double SpeedRatio { get; set; }

        ///// <summary>
        ///// Identifies for the SpeedRatio dependency property.
        ///// </summary>
        //public static DependencyProperty SpeedRatioProperty { get; }

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

        internal HashSet2<Guid> CompletedGuids = new HashSet2<Guid>();
        internal void INTERNAL_RaiseCompletedEvent(Guid guid)
        {
            if (CompletedGuids == null)
            {
                CompletedGuids = new HashSet2<Guid>();
            }
            if (!CompletedGuids.Contains(guid))
            {
                CompletedGuids.Add(guid);
            }
            if (Completed != null)
                Completed(this, new EventArgs());
        }

        internal virtual void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false)
        {
            _animationTimer.Stop();
            if (_beginTimeTimer != null)
            {
                _beginTimeTimer.Stop();
            }
        }

        internal void Stop(FrameworkElement frameworkElement, bool revertToFormerValue)
        {
            Stop(frameworkElement, "visualStateGroupName", revertToFormerValue: revertToFormerValue); //visualStateGroupName is the default name for a programatically started storyboard.
        }

        /// <summary>
        /// Removes the value set for the VisualState on the specified frameworkElement.
        /// </summary>
        /// <param name="frameworkElement"></param>
        internal void UnApply(FrameworkElement frameworkElement)
        {
            DependencyObject target = default(DependencyObject);
            PropertyPath propertyPath = default(PropertyPath);
            Control frameworkElementAsControl = frameworkElement as Control;
            if (frameworkElementAsControl != null)
            {
                GetTargetElementAndPropertyInfo(frameworkElement, out target, out propertyPath);
                //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
                propertyPath.INTERNAL_PropertySetVisualState(target, CSHTML5.Internal.INTERNAL_NoValue.NoValue);
            }
        }


        internal void GetTargetElementAndPropertyInfo(FrameworkElement targetParent, out DependencyObject target, out PropertyPath propertyPath, bool isTargetParentTheTarget = false)
        {
            propertyPath = Storyboard.GetTargetProperty(this);

            DependencyObject targetBeforePath;
            if (!isTargetParentTheTarget) // "isTargetParentTheTarget" is used when running the Storyboard programmatically via Begin(), because in that case the target is defined at the storyboard level.
            {
                // First, see if the target element is already known:
                targetBeforePath = Storyboard.GetTarget(this);

                // If not, look for it based on its name:
                if (targetBeforePath == null)
                {
                    string targetName = Storyboard.GetTargetName(this);
                    if (targetParent is Control)
                    {
                        targetBeforePath = ((Control)targetParent).GetTemplateChild(targetName);
                    }
                    else
                    {
                        targetBeforePath = (DependencyObject)targetParent.FindName(targetName);
                    }
                }
            }
            else
            {
                targetBeforePath = targetParent;
            }

            target = targetBeforePath;
            foreach (Tuple<DependencyObject, DependencyProperty, int?> element in propertyPath.INTERNAL_AccessPropertyContainer(targetBeforePath))
            {
                target = element.Item1;
            }
        }

        internal void GetPropertyPathAndTargetBeforePath(FrameworkElement targetParent, out DependencyObject targetBeforePath, out PropertyPath propertyPath, bool isTargetParentTheTarget = false)
        {
            string targetName = Storyboard.GetTargetName(this);
            propertyPath = Storyboard.GetTargetProperty(this);
            if (!isTargetParentTheTarget && targetName != null) //If the targetName is null, the target has to be the targetParent.
            {
                if (targetParent is Control)
                {
                    targetBeforePath = ((Control)targetParent).GetTemplateChild(targetName);
                    if (targetBeforePath == null && targetParent.Name == targetName) //if we didn't find the target, it might be that targetParent IS the target.
                    {
                        targetBeforePath = targetParent;
                    }
                }
                else
                {
                    targetBeforePath = (DependencyObject)targetParent.FindName(targetName);
                }
            }
            else
            {
                targetBeforePath = targetParent;
            }
        }

        internal IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>> GoThroughElementsToAccessProperty(PropertyPath propertyPath, DependencyObject targetBeforePath)
        {
            foreach (Tuple<DependencyObject, DependencyProperty, int?> element in propertyPath.INTERNAL_AccessPropertyContainer(targetBeforePath))
            {
                yield return element;
            }
            yield break;
        }

        internal void GetCSSEquivalents(FrameworkElement frameworkElement, out CSSEquivalent cssEquivalent, out List<CSSEquivalent> cssEquivalents)
        {
            DependencyObject target;
            PropertyPath propertyPath;
            cssEquivalent = null;
            cssEquivalents = null;
            Control frameworkElementAsControl = frameworkElement as Control;
            //todo: check if the following comment is true and relevant or maybe remove the test
            if (frameworkElementAsControl != null) //if frameworkElement is not a control, there can't be a Storyboard?
            {
                GetTargetElementAndPropertyInfo(frameworkElement, out target, out propertyPath);
                //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
                Type lastElementType = target.GetType();
                PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);



                Type dependencyPropertyContainerType = propertyInfo.DeclaringType;
                FieldInfo dependencyPropertyField = dependencyPropertyContainerType.GetField(propertyPath.INTERNAL_DependencyPropertyName + "Property");
                // - Get the DependencyProperty
                DependencyProperty dp = (DependencyProperty)dependencyPropertyField.GetValue(null);
                // - Get the propertyMetadata from the property
                PropertyMetadata propertyMetadata = dp.GetTypeMetaData(target.GetType());
                // - Get the cssPropertyName from the PropertyMetadata

                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    cssEquivalent = propertyMetadata.GetCSSEquivalent(target);
                }
                //todo: use GetCSSEquivalent instead (?)
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    cssEquivalents = propertyMetadata.GetCSSEquivalents(target);
                }
            }
        }

        //todo: move this to a helper class and remove the implementation of this method from another class (don't remember which one but it probably has something to do with the animations)
        internal static object DynamicCast(object source, Type destType)
        {
            if (source == null)
                return null;

            //We get the non-nullable destination type because a nullable enum is not considered as an enum (at least bu JSIL)
            Type nonNullableDestType = destType; //Note: we will use this one everywhere because we have already checked whether the source was null or not.
            if (destType.FullName.StartsWith("System.Nullable`1"))
            {
                nonNullableDestType = Nullable.GetUnderlyingType(destType);
            }

            Type srcType = source.GetType();
            if (srcType == nonNullableDestType || nonNullableDestType.IsAssignableFrom(srcType) || (nonNullableDestType == typeof(double) && srcType == typeof(int))) return source;

            var paramTypes = new Type[] { srcType };
            MethodInfo cast = nonNullableDestType.GetMethod("op_Implicit", paramTypes);

            if (cast == null)
            {
                cast = nonNullableDestType.GetMethod("op_Explicit", paramTypes);

                if (cast == null)
                {
                    cast = srcType.GetMethod("op_Implicit", paramTypes);

                    if (cast == null)
                    {
                        cast = srcType.GetMethod("op_Explicit", paramTypes);
                    }
                }
            }

            if (cast != null) return cast.Invoke(null, new object[] { source });

            //BRIDGETODO : implemente Enum.ToObject
#if !BRIDGE
            if (nonNullableDestType.IsEnum) return Enum.ToObject(nonNullableDestType, source);
#else
            throw new NotImplementedException("Enum.ToObject not implemented in Bridge");
#endif
            throw new InvalidCastException();

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
        internal Guid animationInstanceSpecificName = Guid.NewGuid(); //this will allow us to stop a specific animation from whithin a Storyboard with multiple animations

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
        internal void StartFirstIteration(IterationParameters parameters, bool isLastLoop, TimeSpan? parentDelay)
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
            //ComputeDuration();
            if (Duration.HasTimeSpan)
            {
                if (Duration.TimeSpan.TotalMilliseconds > 0)
                {
                    _parameters = parameters;
                    _animationTimer.Interval = Duration.TimeSpan;
                    _animationTimer.Tick -= _animationTimer_Tick;
                    _animationTimer.Tick += _animationTimer_Tick;
                    _isAnimationDurationReached = false;
                    _animationTimer.Start();
                }
                else
                {
                    _isAnimationDurationReached = true;
                }
            }
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
                    if (this is Storyboard)
                    {
                        ((Storyboard)this).Stop(parameters.Target);
                    }
                    else
                    {
                        Stop(parameters.Target, revertToFormerValue: false);
                    }

                    INTERNAL_RaiseCompletedEvent();
                }
                else
                {
                    if (this is Storyboard)
                    {
                        ((Storyboard)this).Stop(parameters.Target);
                    }
                    else
                    {
                        Stop(parameters.Target, revertToFormerValue: true);
                    }

                    IterateOnce(parameters, isLastLoop: remainingIterations == 1);
                }
            }
        }

        bool _isAnimationDurationReached = false;
        IterationParameters _parameters;
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
        internal void ComputeDuration()
        {
            if (Duration == Duration.Automatic)
            {
                Duration = GetNaturalDurationCore();
            }
        }

        protected virtual Duration GetNaturalDurationCore()
        {
            return Duration.Automatic;
        }
#if WORKINPROGRESS

        public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register("SpeedRatio", typeof(double), typeof(Timeline), new PropertyMetadata(1d)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        public double SpeedRatio
        {
            get { return (double)this.GetValue(Timeline.SpeedRatioProperty); }
            set { this.SetValue(Timeline.SpeedRatioProperty, value); }
        }

        public static readonly DependencyProperty AutoReverseProperty = DependencyProperty.Register("AutoReverse", typeof(bool), typeof(Timeline), null);
        public bool AutoReverse
        {
            get { return (bool)this.GetValue(AutoReverseProperty); }
            set { this.SetValue(AutoReverseProperty, value); }
        }

        public static readonly DependencyProperty FillBehaviorProperty = DependencyProperty.Register("FillBehavior", typeof(FillBehavior), typeof(Timeline), null);
        public FillBehavior FillBehavior
        {
            get { return (FillBehavior)this.GetValue(FillBehaviorProperty); }
            set { this.SetValue(FillBehaviorProperty, value); }
        }
#endif
    }
}