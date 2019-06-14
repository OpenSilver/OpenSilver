
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Controls animations with a timeline, and provides object and property targeting
    /// information for its child animations.
    /// </summary>
    [ContentProperty("Children")]
    public sealed class Storyboard : Timeline
    {
        private Dictionary<Tuple<string, string>, Timeline> INTERNAL_propertiesChanged; //todo: change this into a Hashset.



        private TimelineCollection _children = new TimelineCollection();
        /// <summary>
        /// Gets the collection of child Timeline objects.
        /// </summary>
        public TimelineCollection Children { get { return _children; } }

        /// <summary>
        /// Gets the value of the Storyboard.TargetName XAML attached property from a
        /// target element.
        /// </summary>
        /// <param name="element">The target element from which to get the value.</param>
        /// <returns>The Storyboard.TargetName value of the target element.</returns>
        public static string GetTargetName(Timeline element)
        {
            return (string)element.GetValue(TargetNameProperty);
        }
        /// <summary>
        /// Sets the value of the Storyboard.TargetName XAML attached property for a
        /// target element.
        /// </summary>
        /// <param name="element">The target element to set the value for.</param>
        /// <param name="name">
        /// The Storyboard.TargetName value of the target element to set. This should
        /// correspond to an existing Name or x:Name value on the element that the animation
        /// targets.
        /// </param>
        public static void SetTargetName(Timeline element, string name)
        {
            element.SetValue(TargetNameProperty, name);
        }
        /// <summary>
        /// Identifies the Storyboard.TargetName XAML attached property.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.RegisterAttached("TargetName", typeof(string), typeof(Storyboard), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of the Storyboard.TargetProperty XAML attached property from
        /// a target element.
        /// </summary>
        /// <param name="element">The target element from which to get the value.</param>
        /// <returns>The Storyboard.TargetProperty value of the target element.</returns>
        public static PropertyPath GetTargetProperty(Timeline element)
        {
            return (PropertyPath)element.GetValue(TargetPropertyProperty);
        }

        ///// <summary>
        ///// Sets the value of the Storyboard.TargetProperty XAML attached property for
        ///// a target element.
        ///// </summary>
        ///// <param name="element">The target element for which to set the value.</param>
        ///// <param name="path">
        ///// The Storyboard.TargetProperty value of the target element to set. This specifies
        ///// a qualification path that is used by an internal type conversion in order
        ///// to target the dependency property where the animation applies. See Remarks.
        ///// </param>
        //public static void SetTargetProperty(Timeline element, string value)
        //{
        //    element.SetValue(TargetPropertyProperty, value);
        //}

        /// <summary>
        /// Sets the value of the Storyboard.TargetProperty XAML attached property for
        /// a target element.
        /// </summary>
        /// <param name="element">The target element for which to set the value.</param>
        /// <param name="value"></param>
        public static void SetTargetProperty(Timeline element, PropertyPath value)
        {
            element.SetValue(TargetPropertyProperty, value);
        }

        /// <summary>
        /// Identifies the Storyboard.TargetProperty XAML attached property.
        /// </summary>
        public static readonly DependencyProperty TargetPropertyProperty =
            DependencyProperty.RegisterAttached("TargetProperty", typeof(PropertyPath), typeof(Storyboard), new PropertyMetadata(null));



        internal Dictionary<Tuple<string, string>, Timeline> GetPropertiesChanged()
        {
            if (INTERNAL_propertiesChanged == null)
            {
                INTERNAL_propertiesChanged = new Dictionary<Tuple<string, string>, Timeline>();
                foreach (Timeline timeLine in _children)
                {
                    if (timeLine is Storyboard)
                    {
                        Storyboard timelineAsStoryboard = timeLine as Storyboard;
                        Dictionary<Tuple<string, string>, Timeline> innerStoryboardPropertiesChanged = timelineAsStoryboard.GetPropertiesChanged();
                        foreach (Tuple<string, string> key in innerStoryboardPropertiesChanged.Keys)
                        {
                            if (!INTERNAL_propertiesChanged.ContainsKey(key))
                            {
                                INTERNAL_propertiesChanged.Add(key, innerStoryboardPropertiesChanged[key]);
                            }
                        }
                    }
                    else
                    {
                        Tuple<string, string> key = new Tuple<string, string>(Storyboard.GetTargetName(timeLine), Storyboard.GetTargetProperty(timeLine).Path);
                        if (!INTERNAL_propertiesChanged.ContainsKey(key))
                        {
                            INTERNAL_propertiesChanged.Add(key, timeLine); //todo: the key is no longer sufficient because since we have BeginTime in Timeline, we can have multiple animations on a same property, that happen one after the other.
                        }
                    }
                }
            }
            return INTERNAL_propertiesChanged;
        }

        /// <summary>
        /// Initiates the set of animations associated with the storyboard.
        /// </summary>
        public void Begin()
        {
            FrameworkElement target = (FrameworkElement)Storyboard.GetTarget(this);
            Begin(target, true, "DirectlyStoryboard", isVisualStateChange: false);
        }

        internal void Begin(FrameworkElement target, bool useTransitions, string visualStateGroupName, bool isVisualStateChange)
        {
            Dispatcher.INTERNAL_GetCurrentDispatcher().BeginInvoke(() =>
            {
                Guid guid = Guid.NewGuid();
                IterationParameters parameters = new IterationParameters()
                {
                    Target = target,
                    Guid = guid,
                    UseTransitions = useTransitions,
                    VisualStateGroupName = visualStateGroupName,
                    IsVisualStateChange = isVisualStateChange
                };

                InitializeIteration();

                bool isThisSingleLoop = RepeatBehavior.Type == RepeatBehaviorType.Count && RepeatBehavior.Count == 1;

                StartFirstIteration(parameters, isThisSingleLoop, new TimeSpan()); //todo: use a parameter instead of just a new TimeSpan since we can have a Storyboard inside a Storyboard.
            });
        }



        void timeLine_Completed(object sender, EventArgs e)
        {
            NotifyStoryboardOfTimelineEnd((Timeline)sender);
        }

        private int _expectedAmountOfTimelineEnds = 0;
        private Dictionary<Guid, int> _expectedAmountOfTimelineEndsDict = new Dictionary<Guid, int>();
        object thisLock = new object();
        internal void NotifyStoryboardOfTimelineEnd(Timeline timeline)
        {
            HashSet2<Guid> completedguids = timeline.CompletedGuids;
            List<Guid> guidsDealtWith = new List<Guid>();
            foreach (Guid guid in completedguids)
            {
                bool raiseEvent = false;
                lock (thisLock)
                {
                    if (_expectedAmountOfTimelineEndsDict.ContainsKey(guid))
                    {
                        int expectedAmount = _expectedAmountOfTimelineEndsDict[guid];
                        --_expectedAmountOfTimelineEnds;
                        --expectedAmount;
                        _expectedAmountOfTimelineEndsDict[guid] = expectedAmount;
                        guidsDealtWith.Add(guid);
                        if (expectedAmount <= 0)
                        {
                            raiseEvent = true;
                        }
                    }
                }
                if (raiseEvent)
                {
                    OnIterationCompleted(_guidToIterationParametersDict[guid]);
                    _guidToIterationParametersDict.Remove(guid);
                }
            }
            foreach (Guid guid in guidsDealtWith)
            {
                timeline.CompletedGuids.Remove(guid);
            }
        }

        ///// <summary>
        ///// Gets the clock state of the Storyboard.
        ///// </summary>
        ///// <returns>One of the enumeration values. Can be: Active, Filling, or Stopped.</returns>
        //public ClockState GetCurrentState();

        ///// <summary>
        ///// Gets the current animation clock time of the Storyboard.
        ///// </summary>
        ///// <returns>
        ///// The current animation time of the Storyboard per the running animation clock,
        ///// or null if the animation clock is Stopped.
        ///// </returns>
        //public TimeSpan GetCurrentTime();





        ///// <summary>
        ///// Pauses the animation clock associated with the storyboard.
        ///// </summary>
        //public void Pause();

        ///// <summary>
        ///// Resumes the animation clock, or run-time state, associated with the storyboard.
        ///// </summary>
        //public void Resume();
        //public void Seek(TimeSpan offset);
        //public void SeekAlignedToLastTick(TimeSpan offset);







        /// <summary>
        /// Gets the value of the Storyboard.Target XAML attached property from a
        /// target element.
        /// </summary>
        /// <param name="element">The target element from which to get the value.</param>
        /// <returns>The Storyboard.Target value of the target element.</returns>
        public static DependencyObject GetTarget(Timeline element)
        {
            return (DependencyObject)element.GetValue(TargetProperty);
        }
        /// <summary>
        /// Causes the specified Timeline to target the specified object.
        /// </summary>
        /// <param name="element">The timeline that targets the specified dependency object.</param>
        /// <param name="target">The actual instance of the object to target.</param>
        public static void SetTarget(Timeline element, DependencyObject target)
        {
            element.SetValue(TargetProperty, target);
        }
        /// <summary>
        /// Identifies the Storyboard.TargetName XAML attached property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached("Target", typeof(DependencyObject), typeof(Storyboard), new PropertyMetadata(null));


        //public FrameworkElement Target
        //{
        //    get { return (FrameworkElement)GetValue(TargetProperty); }
        //    set { SetValue(TargetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TargetProperty =
        //    DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(Storyboard), new PropertyMetadata(null));



        ///// <summary>
        ///// Causes the specified Timeline to target the specified object.
        ///// </summary>
        ///// <param name="timeline">The timeline that targets the specified dependency object.</param>
        ///// <param name="target">The actual instance of the object to target.</param>
        //public static void SetTarget(Timeline timeline, DependencyObject target)
        //{
        //    timeline.SetValue(TargetProperty, target);
        //}





        ///// <summary>
        ///// Advances the current time of the storyboard's clock to the end of its active
        ///// period.
        ///// </summary>
        //public void SkipToFill();

        /// <summary>
        /// Stops the storyboard.
        /// </summary>
        internal void Stop(FrameworkElement frameworkElement, string groupName)
        {
            foreach (Timeline timeLine in _children)
            {
                timeLine.Stop(frameworkElement, groupName);
            }
        }

        /// <summary>
        /// Stops the storyboard.
        /// </summary>
        public void Stop(FrameworkElement frameworkElement)
        {
            ((Timeline)this).Stop(frameworkElement, revertToFormerValue: true);
            foreach (Timeline timeLine in _children)
            {
                timeLine.Stop(frameworkElement, revertToFormerValue: true);
            }
        }

        //todo: make a Stop method with no arguments that actually stops the Storyboard (at least when the storyboard was started programatically).
        //Note: in WPF, Stop(FrameworkElement parentElement) does NOT stop the Storyboard (at least when it was started through Storyboard.Begin())

        ///
        //Stuff added for loops purposes:


        private Dictionary<Guid, IterationParameters> _guidToIterationParametersDict = new Dictionary<Guid, IterationParameters>(); //the purpose of this Dictionary is to be able to retreive the parameters at the next iteration of the Storyboard.


        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            base.IterateOnce(parameters, isLastLoop);
            //we (re)set the children's remaining iterations:
            foreach (Timeline timeline in Children)
            {
                timeline.InitializeIteration();
            }

            GetPropertiesChanged(); //we make sure INTERNAL_propertiesChanged is filled.

            _expectedAmountOfTimelineEnds = _children.Count;
            if (!_expectedAmountOfTimelineEndsDict.ContainsKey(parameters.Guid))
            {
                _expectedAmountOfTimelineEndsDict.Add(parameters.Guid, _children.Count);
                _guidToIterationParametersDict.Add(parameters.Guid, parameters);
            }
            else
            {
                //I'm not sure this is useful but we never know.
                _expectedAmountOfTimelineEndsDict[parameters.Guid] = _children.Count;
                _guidToIterationParametersDict[parameters.Guid] = parameters;
            }
            if (parameters.Target != null)
            {
                foreach (Timeline timeLine in _children)
                {
                    IterationParameters currentParameters = parameters.Clone(); //note: we make a clone for each timeline because the value of IsTargetParentTheTarget can change from one timeline to another.
                    timeLine.Completed -= timeLine_Completed;
                    timeLine.Completed += timeLine_Completed;
                    currentParameters.IsTargetParentTheTarget = true;
                    if (currentParameters.IsVisualStateChange && Storyboard.GetTargetName(timeLine) != null)
                    {
                        currentParameters.IsTargetParentTheTarget = false;
                    }
                    bool isTimelineSingleLoop = timeLine.RepeatBehavior.Type == RepeatBehaviorType.Count && timeLine.RepeatBehavior.Count == 1;
                    timeLine.StartFirstIteration(currentParameters, isTimelineSingleLoop, BeginTime);
                }
            }
            else
            {
                foreach (Timeline timeLine in _children)
                {
                    DependencyObject target = Storyboard.GetTarget(timeLine);
                    if (target is FrameworkElement)
                    {
                        parameters.Target = (FrameworkElement)target;
                        timeLine.Completed -= timeLine_Completed;
                        timeLine.Completed += timeLine_Completed;
                        parameters.VisualStateGroupName = "visualStateGroupName";
                        parameters.IsTargetParentTheTarget = false;
                        bool isTimelineSingleLoop = timeLine.RepeatBehavior.Type == RepeatBehaviorType.Count && timeLine.RepeatBehavior.Count == 1;
                        timeLine.StartFirstIteration(parameters, isTimelineSingleLoop, BeginTime);
                    }
                }
            }
        }


    }

    /// <summary>
    /// this class has been added to make passing the parameters through the iterations easier.
    /// </summary>
    internal class IterationParameters
    {
        internal FrameworkElement Target;
        internal Guid Guid;
        internal bool UseTransitions;
        internal string VisualStateGroupName;
        internal bool IsTargetParentTheTarget;
        internal bool IsVisualStateChange;

        internal IterationParameters Clone()
        {
            return new IterationParameters()
            {
                Target = this.Target,
                Guid = this.Guid,
                UseTransitions = this.UseTransitions,
                VisualStateGroupName = this.VisualStateGroupName,
                IsTargetParentTheTarget = this.IsTargetParentTheTarget,
                IsVisualStateChange = this.IsVisualStateChange
            };
        }
    }
}
