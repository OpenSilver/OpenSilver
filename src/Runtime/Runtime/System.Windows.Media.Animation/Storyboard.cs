
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Markup;
using OpenSilver.Internal;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Controls animations with a timeline, and provides object and property targeting
/// information for its child animations.
/// </summary>
[ContentProperty(nameof(Children))]
public sealed class Storyboard : Timeline
{
    private TimelineCollection _children;
    private TimelineClock _activeClock;

    /// <summary>
    /// Initializes a new instance of the <see cref="Storyboard"/> class.
    /// </summary>
    public Storyboard() { }

    /// <summary>
    /// Gets the collection of child <see cref="Timeline"/> objects.
    /// </summary>
    /// <returns>
    /// The collection of child <see cref="Timeline"/> objects. The
    /// default is an empty collection.
    /// </returns>
    public TimelineCollection Children => _children ??= new TimelineCollection(this);

    /// <summary>
    /// Identifies the Storyboard.TargetName attached property.
    /// </summary>
    public static readonly DependencyProperty TargetNameProperty =
        DependencyProperty.RegisterAttached(
            "TargetName",
            typeof(string),
            typeof(Storyboard),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets the Storyboard.TargetName of the specified <see cref="Timeline"/> object.
    /// </summary>
    /// <param name="element">
    /// The <see cref="Timeline"/> object to get the target name from.
    /// </param>
    /// <returns>
    /// The string name of the target object.
    /// </returns>
    public static string GetTargetName(Timeline element) => (string)element.GetValue(TargetNameProperty);

    /// <summary>
    /// Causes the specified <see cref="Timeline"/> to target the object
    /// with the specified name.
    /// </summary>
    /// <param name="element">
    /// The timeline that targets the specified dependency object.
    /// </param>
    /// <param name="name">
    /// The name of the object to target.
    /// </param>
    public static void SetTargetName(Timeline element, string name) => element.SetValueInternal(TargetNameProperty, name);

    /// <summary>
    /// Identifies the Storyboard.TargetProperty attached property.
    /// </summary>
    public static readonly DependencyProperty TargetPropertyProperty =
        DependencyProperty.RegisterAttached(
            "TargetProperty",
            typeof(PropertyPath),
            typeof(Storyboard),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets the Storyboard.TargetProperty of the specified <see cref="Timeline"/> object.
    /// </summary>
    /// <param name="element">
    /// The <see cref="Timeline"/> object to get the target property from.
    /// </param>
    /// <returns>
    /// The property path information for the animated property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static PropertyPath GetTargetProperty(Timeline element) => (PropertyPath)element.GetValue(TargetPropertyProperty);

    /// <summary>
    /// Causes the specified <see cref="Timeline"/> to target the specified
    /// dependency property.
    /// </summary>
    /// <param name="element">
    /// The timeline with which to associate the specified dependency property.
    /// </param>
    /// <param name="value">
    /// A path that describe the dependency property to be animated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One or more of the parameters is null.
    /// </exception>
    public static void SetTargetProperty(Timeline element, PropertyPath value) => element.SetValueInternal(TargetPropertyProperty, value);

    /// <summary>
    /// Identifies the Storyboard.TargetName attached property.
    /// </summary>
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.RegisterAttached(
            "Target",
            typeof(DependencyObject),
            typeof(Storyboard),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets the value of the Storyboard.Target attached property from a
    /// target element.
    /// </summary>
    /// <param name="element">
    /// The target element from which to get the value.
    /// </param>
    /// <returns>
    /// The Storyboard.Target value of the target element.
    /// </returns>
    public static DependencyObject GetTarget(Timeline element) => (DependencyObject)element.GetValue(TargetProperty);

    /// <summary>
    /// Causes the specified <see cref="Timeline"/> to target the specified object.
    /// </summary>
    /// <param name="element">
    /// The timeline that targets the specified dependency object.
    /// </param>
    /// <param name="target">
    /// The actual instance of the object to target.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One or more of the parameters is null.
    /// </exception>
    public static void SetTarget(Timeline element, DependencyObject target) => element.SetValueInternal(TargetProperty, target);

    /// <summary>
    /// Initiates the set of animations associated with the storyboard.
    /// </summary>
    public void Begin() => BeginCommon(this, false);

    internal void Begin(DependencyObject containingObject, bool alignedToLastTick) =>
        BeginCommon(containingObject, alignedToLastTick);

    private void BeginCommon(DependencyObject containingObject, bool alignedToLastTick)
    {
        _activeClock?.Pause();

        _activeClock = CreateClock(true);
        ClockTreeWalkRecursive(_activeClock,
            containingObject,
            null,
            null,
            null,
            null);

        _activeClock.Begin(alignedToLastTick);
    }

    /// <summary>
    /// Gets the clock state of the storyboard.
    /// </summary>
    /// <returns>
    /// One of the enumeration values: <see cref="ClockState.Active"/>,
    /// <see cref="ClockState.Filling"/>, or <see cref="ClockState.Stopped"/>.
    /// </returns>
    public ClockState GetCurrentState() => _activeClock?.CurrentState ?? ClockState.Stopped;

    /// <summary>
    /// Gets the current time of the storyboard.
    /// </summary>
    /// <returns>
    /// The current time of the storyboard, or null if the storyboard's clock is <see cref="ClockState.Stopped"/>.
    /// </returns>
    public TimeSpan GetCurrentTime() => _activeClock?.CurrentTime ?? TimeSpan.Zero;

    /// <summary>
    /// Pauses the animation clock associated with the storyboard.
    /// </summary>
    public void Pause() => _activeClock?.Pause();

    /// <summary>
    /// Resumes the animation clock, or run-time state, associated with the storyboard.
    /// </summary>
    public void Resume() => _activeClock?.Resume();

    /// <summary>
    /// Moves the storyboard to the specified animation position. The storyboard performs
    /// the requested seek when the next clock tick occurs.
    /// </summary>
    /// <param name="offset">
    /// A positive or negative time value that describes the amount by which the timeline
    /// should move forward or backward from the beginning of the animation. By using
    /// the <see cref="TimeSpan"/> Parse behavior, a <see cref="TimeSpan"/> can be specified 
    /// as a string in the following format (in this syntax, the [] characters denote optional 
    /// components of the string, but the quotes, colons, and periods are all a literal part of
    /// the syntax):"[days.]hours:minutes:seconds[.fractionalSeconds]"- or -"days"
    /// </param>
    public void Seek(TimeSpan offset) => _activeClock?.Seek(offset);

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
    public void SeekAlignedToLastTick(TimeSpan offset) => _activeClock?.SeekAlignedToLastTick(offset);

    /// <summary>
    /// Advances the current time of the storyboard's clock to the end of its active period.
    /// </summary>
    public void SkipToFill() => _activeClock?.SkipToFill();

    /// <summary>
    /// Stops the storyboard.
    /// </summary>
    public void Stop()
    {
        if (_activeClock is not null)
        {
            _activeClock.Stop();
            _activeClock = null;
        }
    }

    internal override TimelineClock CreateClock(bool isRoot) => new StoryboardClock(this, isRoot);

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
    private static void ClockTreeWalkRecursive(
        TimelineClock currentClock,
        DependencyObject containingObject,
        TimelineClock parentClock,
        DependencyObject parentObject,
        string parentObjectName,
        PropertyPath parentPropertyPath)
    {
        Timeline currentTimeline = currentClock.Timeline;

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

        if (currentTimeline is not Storyboard storyboard)
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
                    targetObject = containingObject;

                    if (targetObject is not IFrameworkElement)
                    {
                        // The containing object is not an FE.
                        throw new InvalidOperationException(
                            $"No target was specified for '{currentTimeline.GetType()}'.");
                    }
                }
            }

            // See if we have a property name to use.
            if (currentPropertyPath == null)
            {
                throw new InvalidOperationException(
                    $"Must specify TargetProperty for '{currentTimeline.GetType()}'.");
            }

            currentClock.SetParent(parentClock);
            currentClock.SetContext(targetObject, currentPropertyPath);
        }
        else
        {
            var storyboardClock = (StoryboardClock)currentClock;
            storyboardClock.SetParent(parentClock);

            TimelineCollection childrenTimelines = storyboard.Children;

            for (int i = 0; i < childrenTimelines.Count; i++)
            {
                TimelineClock childClock = childrenTimelines[i].CreateClock(false);
                if (childClock == null)
                {
                    continue;
                }

                storyboardClock.AddClock(childClock);

                ClockTreeWalkRecursive(
                    childClock,
                    containingObject,
                    currentClock,
                    targetObject,
                    currentObjectName,
                    currentPropertyPath);
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
            throw new InvalidOperationException(
                $"No applicable name scope exists to resolve the name '{targetName}'.");
        }

        if (namedObject == null)
        {
            throw new InvalidOperationException(
                $"'{targetName}' name cannot be found in the name scope of '{fe.GetType()}'.");
        }

        targetObject = namedObject as DependencyObject;
        if (targetObject == null)
        {
            throw new InvalidOperationException(
                $"'{targetName}' target object Name found but the object is not a valid target type.");
        }

        return targetObject;
    }

    private sealed class StoryboardClock : TimelineClock
    {
        private readonly List<TimelineClock> _children = new();

        public StoryboardClock(Storyboard owner, bool isRoot)
            : base(owner, isRoot)
        {
        }

        public override IEnumerable<TimelineClock> Children => _children;

        public void AddClock(TimelineClock clock)
        {
            Debug.Assert(clock is not null);
            Debug.Assert(!clock.IsRoot);

            _children.Add(clock);
        }

        public override void SetContext(DependencyObject target, PropertyPath targetProperty) =>
            throw new NotImplementedException();

        protected override void OnFrameCore()
        {
            foreach (TimelineClock clock in _children)
            {
                clock.OnFrame(CurrentTime);
            }
        }

        protected override void OnStopCore()
        {
            foreach (TimelineClock clock in _children)
            {
                clock.OnStop();
            }
        }

        public override Duration IterationDuration
        {
            get
            {
                Duration iterationDuration = Timeline.Duration;
                if (iterationDuration.HasTimeSpan)
                {
                    return iterationDuration;
                }

                TimeSpan maxDuration = TimeSpan.Zero;

                foreach (TimelineClock clock in _children)
                {
                    Duration duration = clock.EffectiveDuration;
                    
                    if (duration == Duration.Forever)
                    {
                        return Duration.Forever;
                    }

                    if (duration.HasTimeSpan)
                    {
                        TimeSpan timespan = duration.TimeSpan + clock.BeginTime;
                        if (timespan > maxDuration)
                        {
                            maxDuration = timespan;
                        }
                    }
                }

                return new Duration(maxDuration);
            }
        }
    }
}
