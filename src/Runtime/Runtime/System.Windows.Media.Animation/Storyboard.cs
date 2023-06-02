﻿
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
using System.Diagnostics;
using System.Windows.Markup;
using OpenSilver.Internal;

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
    public sealed partial class Storyboard : Timeline
    {
        private TimelineCollection _children;
        private Dictionary<Tuple<string, string>, Timeline> INTERNAL_propertiesChanged; //todo: change this into a Hashset.

        private bool _isUnApplied = false; // Note: we set this variable because the animation start is done inside a Dispatcher, so if the user Starts then Stops the animation immediately (in the same thread), we want to cancel the start of the animation.
        internal bool IsUnApplied
        {
            get { return _isUnApplied; }
            set
            {
                _isUnApplied = value;
                foreach (Timeline tl in Children)
                {
                    if (tl is AnimationTimeline)
                    {
                        ((AnimationTimeline)tl)._isUnapplied = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the collection of child Timeline objects.
        /// </summary>
        public TimelineCollection Children
            => _children ?? (_children = new TimelineCollection(this));

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
                foreach (Timeline timeLine in Children)
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
                        PropertyPath targetProperty = Storyboard.GetTargetProperty(timeLine);
                        string targetName = Storyboard.GetTargetName(timeLine);

                        Tuple<string, string> key = new Tuple<string, string>(targetName, targetProperty?.Path);
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
            BeginCommon(this);
        }

        internal void Begin(FrameworkElement target)
        {
            BeginCommon(target);
        }

        private void BeginCommon(DependencyObject containingObject)
        {
            this.IsUnApplied = false; // Note: we set this variable because the animation start is done inside a Dispatcher, so if the user synchronously Starts then Stops then Starts an animation, we want it to be in the started state.

            try
            {
                if (!this._isUnApplied) // Note: we use this variable because the animation start is done inside a Dispatcher, so if the user Starts then Stops the animation immediately (in the same thread), we want to cancel the start of the animation.
                {
                    InitializeIteration();

                    var timelineMappings = new Dictionary<Timeline, Tuple<DependencyObject, PropertyPath>>();
                    TimelineTreeWalkRecursive(this,
                        containingObject,
                        null,
                        null,
                        null,
                        timelineMappings);

                    IterationParameters parameters = new IterationParameters(timelineMappings);

                    bool isThisSingleLoop = RepeatBehavior.HasCount && RepeatBehavior.Count == 1;

                    StartFirstIteration(parameters, isThisSingleLoop, new TimeSpan()); //todo: use a parameter instead of just a new TimeSpan since we can have a Storyboard inside a Storyboard.
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void timeLine_Completed(object sender, EventArgs e)
        {
            NotifyStoryboardOfTimelineEnd((Timeline)sender);
        }

        private Dictionary<Guid, int> _expectedAmountOfTimelineEndsDict = new Dictionary<Guid, int>();
        object thisLock = new object();
        internal void NotifyStoryboardOfTimelineEnd(Timeline timeline)
        {
            if (timeline != null && timeline._parameters != null)
            {
                Guid guid = timeline._parameters.Guid;
                bool raiseEvent = false;
                lock (thisLock)
                {
                    if (_expectedAmountOfTimelineEndsDict.ContainsKey(guid))
                    {
                        int expectedAmount = _expectedAmountOfTimelineEndsDict[guid];
                        --expectedAmount;
                        _expectedAmountOfTimelineEndsDict[guid] = expectedAmount;
                        if (expectedAmount <= 0)
                        {
                            raiseEvent = true;
                        }
                    }
                }
                if (raiseEvent)
                {
                    if (_guidToIterationParametersDict.ContainsKey(guid))
                    {
                        _guidToIterationParametersDict.Remove(guid);

                        if (_guidToIterationParametersDict.Count == 0)
                        {
                            OnIterationCompleted(this._parameters);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves the storyboard to the specified animation position immediately(synchronously).
        /// </summary>
        /// <param name="offset">
        /// A positive or negative time value that describes the amount by which the timeline should move
        /// forward or backward from the beginning of the animation. By using the TimeSpan Parse behavior,
        /// a TimeSpan can be specified as a string in the following format (in this syntax, the [] characters
        /// denote optional components of the string, but the quotes, colons, and periods are all a literal part of the syntax):
        ///"[days.]hours:minutes:seconds[.fractionalSeconds]"
        ///- or -
        ///"days"
        /// </param>
        [OpenSilver.NotImplemented]
        public void SeekAlignedToLastTick(TimeSpan offset) { }

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

        /// <summary>
        /// Stops the storyboard.
        /// </summary>
        public void Stop(FrameworkElement frameworkElement)
        {
            Stop();
        }

        public void Stop()
        {
            Stop(null, revertToFormerValue: true);
            foreach (Timeline timeLine in Children)
            {
                timeLine.Stop(null, revertToFormerValue: true);
            }
        }

        //todo: make a Stop method with no arguments that actually stops the Storyboard (at least when the storyboard was started programatically).
        //Note: in WPF, Stop(FrameworkElement parentElement) does NOT stop the Storyboard (at least when it was started through Storyboard.Begin())

        ///
        //Stuff added for loops purposes:


        private Dictionary<Guid, IterationParameters> _guidToIterationParametersDict = new Dictionary<Guid, IterationParameters>(); //the purpose of this Dictionary is to be able to retreive the parameters at the next iteration of the Storyboard.

        /// <summary>
        /// Recursively walks the timeline tree and determine the target object
        /// and property for each timeline in the tree.
        /// </summary>
        /// <remarks>
        /// The currently active object and property path are passed in as parameters,
        /// they will be used unless a target/property specification exists on
        /// the Timeline object corresponding to the current timeline.  (So that the
        /// leaf-most reference wins.)
        ///
        /// The active object and property parameters may be null if they have
        /// never been specified.  If we reach a leaf node timeline and a needed attribute
        /// is still null, it is an error condition.  Otherwise we keep hoping they'll be found.
        /// </remarks>
        private void TimelineTreeWalkRecursive(
            Timeline currentTimeline,
            DependencyObject containingObject,
            DependencyObject parentObject,
            string parentObjectName,
            PropertyPath parentPropertyPath,
            Dictionary<Timeline, Tuple<DependencyObject, PropertyPath>> timelineMappings)
        {
            DependencyObject targetObject = parentObject;
            string currentObjectName = parentObjectName;
            PropertyPath currentPropertyPath = parentPropertyPath;

            // If we have target object/property information, use it instead of the
            //  parent's information.
            string nameString = (string)currentTimeline.GetValue(TargetNameProperty);
            if (nameString != null)
            {
                currentObjectName = nameString;
            }

            // The TargetProperty trumps the TargetName property.
            DependencyObject localTargetObject = (DependencyObject)currentTimeline.GetValue(TargetProperty);
            if (localTargetObject != null)
            {
                targetObject = localTargetObject;
                currentObjectName = null;
            }

            PropertyPath propertyPath = (PropertyPath)currentTimeline.GetValue(TargetPropertyProperty);
            if (propertyPath != null)
            {
                currentPropertyPath = propertyPath;
            }

            if (!(currentTimeline is Storyboard))
            {
                if (targetObject == null)
                {
                    // Resolve the target object name.  If no name specified, use the
                    //  containing object.
                    if (currentObjectName != null)
                    {
                        targetObject = ResolveTargetName(
                            currentObjectName,
                            FrameworkElement.FindMentor(containingObject),
                            currentTimeline.NameResolver);
                    }
                    else
                    {
                        // The containing object must be either an FE.
                        targetObject = containingObject as FrameworkElement;

                        if (targetObject == null)
                        {
                            // The containing object is not an FE.
                            throw new InvalidOperationException(string.Format("No target was specified for '{0}'.", currentTimeline.GetType().ToString()));
                        }
                    }
                }

                // See if we have a property name to use.
                if (currentPropertyPath == null)
                {
                    throw new InvalidOperationException(string.Format("Must specify TargetProperty for '{0}'.", currentTimeline.GetType().ToString()));
                }

                timelineMappings.Add(currentTimeline, Tuple.Create(targetObject, currentPropertyPath));
            }
            else
            {
                Storyboard storyboard = (Storyboard)currentTimeline;
                TimelineCollection childrenTimelines = storyboard.Children;

                for (int i = 0; i < childrenTimelines.Count; i++)
                {
                    TimelineTreeWalkRecursive(
                        childrenTimelines[i],
                        containingObject,
                        targetObject,
                        currentObjectName,
                        currentPropertyPath,
                        timelineMappings);
                }
            }
        }

        private static DependencyObject ResolveTargetName(string targetName, IInternalFrameworkElement fe, INameResolver nameResolver)
        {
            object namedObject;
            DependencyObject targetObject;

            if (nameResolver != null)
            {
                namedObject = nameResolver.Resolve(targetName);
            }
            else if (fe != null)
            {
                namedObject = fe.FindName(targetName);
            }
            else
            {
                throw new InvalidOperationException(string.Format("No applicable name scope exists to resolve the name '{0}'.", targetName));
            }

            if (namedObject == null)
            {
                throw new InvalidOperationException(
                    string.Format("'{0}' name cannot be found in the name scope of '{1}'.",
                        targetName,
                        fe.GetType().ToString()));
            }

            targetObject = namedObject as DependencyObject;
            if (targetObject == null)
            {
                throw new InvalidOperationException(string.Format("'{0}' target object Name found but the object is not a valid target type.", targetName));
            }

            return targetObject;
        }

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            base.IterateOnce(parameters, isLastLoop);
            //we (re)set the children's remaining iterations:
            foreach (Timeline timeline in Children)
            {
                timeline.InitializeIteration();
            }

            GetPropertiesChanged(); //we make sure INTERNAL_propertiesChanged is filled.

            if (!_expectedAmountOfTimelineEndsDict.ContainsKey(parameters.Guid))
            {
                _expectedAmountOfTimelineEndsDict.Add(parameters.Guid, Children.Count);
                _guidToIterationParametersDict.Add(parameters.Guid, parameters);
            }
            else
            {
                //I'm not sure this is useful but we never know.
                _expectedAmountOfTimelineEndsDict[parameters.Guid] = Children.Count;
                _guidToIterationParametersDict[parameters.Guid] = parameters;
            }

            foreach (Timeline timeLine in Children)
            {
                timeLine.Completed -= timeLine_Completed;
                timeLine.Completed += timeLine_Completed;
                bool isTimelineSingleLoop = timeLine.RepeatBehavior.HasCount && timeLine.RepeatBehavior.Count == 1;
                timeLine.StartFirstIteration(parameters, isTimelineSingleLoop, BeginTime);
            }
        }

        [OpenSilver.NotImplemented]
        public ClockState GetCurrentState()
        {
            return ClockState.Active;
        }

        [OpenSilver.NotImplemented]
        public void SkipToFill()
        {

        }

        [OpenSilver.NotImplemented]
        public void Seek(TimeSpan offset)
        {

        }

        [OpenSilver.NotImplemented]
        public void Pause()
        {

        }

        /// <summary>
        /// Resumes the animation clock, or run-time state, associated with the storyboard.
        /// </summary>
        [OpenSilver.NotImplemented]
        public void Resume()
        {

        }
    }

    /// <summary>
    /// this class has been added to make passing the parameters through the iterations easier.
    /// </summary>
    internal sealed class IterationParameters
    {
        public IterationParameters(IReadOnlyDictionary<Timeline, Tuple<DependencyObject, PropertyPath>> mappings)
        {
            TimelineMappings = mappings;
            Guid = Guid.NewGuid();
        }

        internal readonly IReadOnlyDictionary<Timeline, Tuple<DependencyObject, PropertyPath>> TimelineMappings;
        internal readonly Guid Guid;
    }
}
