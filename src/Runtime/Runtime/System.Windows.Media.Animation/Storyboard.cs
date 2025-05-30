
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
    /// Applies the animations associated with this <see cref="Storyboard"/> to their targets and initiates them.
    /// </summary>
    public void Begin() => BeginCommon(this, true, false);

    /// <summary>
    /// Applies the animations associated with this <see cref="Storyboard"/> to their targets and initiates them.
    /// </summary>
    /// <param name="containingObject">
    /// An object contained within the same name scope as the targets of this storyboard's animations. Animations without a
    /// Storyboard.TargetName are applied to containingObject.
    /// </param>
    public void Begin(FrameworkElement containingObject) => BeginCommon(containingObject, true, false);

    /// <summary>
    /// Applies the animations associated with this <see cref="Storyboard"/> to their targets and initiates them.
    /// </summary>
    /// <param name="containingObject">
    /// An object contained within the same name scope as the targets of this storyboard's animations. Animations 
    /// without a Storyboard.TargetName are applied to containingObject.
    /// </param>
    /// <param name="isControllable">
    /// true if the storyboard should be interactively controllable; otherwise, false.
    /// </param>
    public void Begin(FrameworkElement containingObject, bool isControllable) => BeginCommon(containingObject, isControllable, false);

    // This method should only be used by VisualStateManager for Silverlight compatibility. In WPF, Begin is asynchronous. In 
    // Silverlight the VSM fires a frame immediately, but other storyboards don't.
    internal void BeginVSM(FrameworkElement containingObject) => BeginCommon(containingObject, true, true);

    private void BeginCommon(DependencyObject containingObject, bool isControllable, bool alignedToLastTick)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock currentClock)
        {
            currentClock.Controller.Pause();
        }

        TimelineClock storyboardClockTree = CreateClock();
        storyboardClockTree.IsRoot = true;
        storyboardClockTree.HasControllableRoot = isControllable;

        ClockTreeWalkRecursive(storyboardClockTree,
            isControllable,
            containingObject,
            null,
            null,
            null,
            null);

        if (isControllable)
        {
            SetStoryboardClock(containingObject, storyboardClockTree);
        }

        storyboardClockTree.InternalBegin(alignedToLastTick);
    }

    /// <summary>
    /// Retrieves the current global speed of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <returns>
    /// The current global speed, or 0 if the clock is stopped.
    /// </returns>
    public double GetCurrentGlobalSpeed() => GetCurrentGlobalSpeedImpl(this) ?? 0;

    /// <summary>
    /// Retrieves the current global speed of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// The current global speed, or null if the clock is stopped.
    /// </returns>
    public double? GetCurrentGlobalSpeed(FrameworkElement containingObject) => GetCurrentGlobalSpeedImpl(containingObject);

    private double? GetCurrentGlobalSpeedImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.CurrentGlobalSpeed;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the current iteration of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <returns>
    /// This clock's current iteration within its current active period, or null if this clock is stopped.
    /// </returns>
    public int GetCurrentIteration() => GetCurrentIterationImpl(this) ?? 0;

    /// <summary>
    /// Retrieves the current iteration of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// This clock's current iteration within its current active period, or null if this clock is stopped.
    /// </returns>
    public int? GetCurrentIteration(FrameworkElement containingObject) => GetCurrentIterationImpl(containingObject);

    private int? GetCurrentIterationImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.CurrentIteration;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the current progress of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <returns>
    /// null if this clock is <see cref="ClockState.Stopped"/>, or 0.0 if this clock is active and its timeline
    /// has a <see cref="Timeline.Duration"/> of <see cref="Duration.Forever"/>; otherwise, a value between 0.0 
    /// and 1.0 that indicates the current progress of this clock within its current iteration. A value of 0.0 
    /// indicates no progress, and a value of 1.0 indicates that the clock is at the end of its current iteration.
    /// </returns>
    public double GetCurrentProgress() => GetCurrentProgressImpl(this) ?? 0;

    /// <summary>
    /// Retrieves the current progress of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// null if this clock is <see cref="ClockState.Stopped"/>, or 0.0 if this clock is active and its timeline
    /// has a <see cref="Timeline.Duration"/> of <see cref="Duration.Forever"/>; otherwise, a value between 0.0 
    /// and 1.0 that indicates the current progress of this clock within its current iteration. A value of 0.0 
    /// indicates no progress, and a value of 1.0 indicates that the clock is at the end of its current iteration.
    /// </returns>
    public double? GetCurrentProgress(FrameworkElement containingObject) => GetCurrentProgressImpl(containingObject);

    private double? GetCurrentProgressImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.CurrentProgress;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the current state of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <returns>
    /// The current state of the clock created for this storyboard: <see cref="ClockState.Active"/>,
    /// <see cref="ClockState.Filling"/>, or <see cref="ClockState.Stopped"/>.
    /// </returns>
    public ClockState GetCurrentState()
    {
        // Silverlight allows calling this method even when the Storyboard was not started yet.
        // That's why we do not throw if the clock is not found.
        if (GetStoryboardClock(this, false) is TimelineClock clock)
        {
            return clock.CurrentState;
        }
        return ClockState.Stopped;
    }

    /// <summary>
    /// Retrieves the current state of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement)"/> method was called. This 
    /// object contains the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// The current state of the clock created for this storyboard: <see cref="ClockState.Active"/>,
    /// <see cref="ClockState.Filling"/>, or <see cref="ClockState.Stopped"/>.
    /// </returns>
    public ClockState GetCurrentState(FrameworkElement containingObject) => GetCurrentStateImpl(containingObject);

    private ClockState GetCurrentStateImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.CurrentState;
        }
        return ClockState.Stopped;
    }

    /// <summary>
    /// Retrieves the current time of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <returns>
    /// <see cref="TimeSpan.Zero"/> if this storyboard's clock is <see cref="ClockState.Stopped"/>; otherwise, the 
    /// current time of the storyboard's clock.
    /// </returns>
    public TimeSpan GetCurrentTime()
    {
        // Silverlight allows calling this method even when the Storyboard was not started yet.
        // That's why we do not throw if the clock is not found.
        if (GetStoryboardClock(this, false) is TimelineClock clock)
        {
            return clock.CurrentTime ?? TimeSpan.Zero;
        }
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Retrieves the current time of the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// null if this storyboard's clock is <see cref="ClockState.Stopped"/>; otherwise, the current time of the 
    /// storyboard's clock.
    /// </returns>
    public TimeSpan? GetCurrentTime(FrameworkElement containingObject) => GetCurrentTimeImpl(containingObject);

    private TimeSpan? GetCurrentTimeImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.CurrentTime;
        }
        return null;
    }

    /// <summary>
    /// Retrieves a value that indicates whether the clock that was created for this <see cref="Storyboard"/> is paused.
    /// </summary>
    /// <returns>
    /// true if the clock created for this <see cref="Storyboard"/> is paused; otherwise, false.
    /// </returns>
    public bool GetIsPaused() => GetIsPausedImpl(this);

    /// <summary>
    /// Retrieves a value that indicates whether the clock that was created for this <see cref="Storyboard"/> is paused.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <returns>
    /// true if the clock created for this <see cref="Storyboard"/> is paused; otherwise, false.
    /// </returns>
    public bool GetIsPaused(FrameworkElement containingObject) => GetIsPausedImpl(containingObject);

    private bool GetIsPausedImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, true) is TimelineClock clock)
        {
            return clock.IsPaused;
        }

        // A clock that has been disposed is not in a paused state.
        return false;
    }

    /// <summary>
    /// Pauses the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    public void Pause() => PauseImpl(this);

    /// <summary>
    /// Pauses the clock of the specified <see cref="FrameworkElement"/> associated with this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    public void Pause(FrameworkElement containingObject) => PauseImpl(containingObject);

    private void PauseImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.Pause();
        }
    }

    /// <summary>
    /// Resumes the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    public void Resume() => ResumeImpl(this);

    /// <summary>
    /// Resumes the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    public void Resume(FrameworkElement containingObject) => ResumeImpl(containingObject);

    private void ResumeImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.Resume();
        }
    }

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to the specified position. The <see cref="Storyboard"/> performs the 
    /// requested seek when the next clock tick occurs.
    /// </summary>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward.
    /// </param>
    public void Seek(TimeSpan offset) => SeekImpl(this, offset, TimeSeekOrigin.BeginTime);

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to the specified position. The <see cref="Storyboard"/> performs the requested 
    /// seek when the next clock tick occurs.
    /// </summary>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward from 
    /// the specified origin.
    /// </param>
    /// <param name="origin">
    /// The position from which offset is applied.
    /// </param>
    public void Seek(TimeSpan offset, TimeSeekOrigin origin) => SeekImpl(this, offset, origin);

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to the specified position. The <see cref="Storyboard"/> performs the 
    /// requested seek when the next clock tick occurs.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward 
    /// from the specified origin.
    /// </param>
    public void Seek(FrameworkElement containingObject, TimeSpan offset) => SeekImpl(containingObject, offset, TimeSeekOrigin.BeginTime);

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to the specified position. The <see cref="Storyboard"/> performs the requested 
    /// seek when the next clock tick occurs.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward from 
    /// the specified origin.
    /// </param>
    /// <param name="origin">
    /// The position from which offset is applied.
    /// </param>
    public void Seek(FrameworkElement containingObject, TimeSpan offset, TimeSeekOrigin origin) => SeekImpl(containingObject, offset, origin);

    private void SeekImpl(DependencyObject containingObject, TimeSpan offset, TimeSeekOrigin origin)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.Seek(offset, origin);
        }
    }

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to a new position immediately (synchronously).
    /// </summary>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward.
    /// </param>
    public void SeekAlignedToLastTick(TimeSpan offset) => SeekAlignedToLastTickImpl(this, offset, TimeSeekOrigin.BeginTime);

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to a new position immediately (synchronously).
    /// </summary>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward 
    /// from the specified origin.
    /// </param>
    /// <param name="origin">
    /// The position from which offset is applied.
    /// </param>
    public void SeekAlignedToLastTick(TimeSpan offset, TimeSeekOrigin origin) => SeekAlignedToLastTickImpl(this, offset, origin);

    /// <summary>
    /// Seeks this <see cref="Storyboard"/> to a new position immediately (synchronously).
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    /// <param name="offset">
    /// A positive or negative value that describes the amount by which the timeline should move forward or backward from 
    /// the specified origin.
    /// </param>
    /// <param name="origin">
    /// The position from which offset is applied.
    /// </param>
    public void SeekAlignedToLastTick(FrameworkElement containingObject, TimeSpan offset, TimeSeekOrigin origin) =>
        SeekAlignedToLastTickImpl(containingObject, offset, origin);

    private void SeekAlignedToLastTickImpl(DependencyObject containingObject, TimeSpan offset, TimeSeekOrigin origin)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.SeekAlignedToLastTick(offset, origin);
        }
    }

    /// <summary>
    /// Advances the current time of this storyboard's clock to the end of its active period.
    /// </summary>
    public void SkipToFill() => SkipToFillImpl(this);

    /// <summary>
    /// Advances the current time of this storyboard's clock to the end of its active period.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    public void SkipToFill(FrameworkElement containingObject) => SkipToFillImpl(containingObject);

    private void SkipToFillImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.SkipToFill();
        }
    }

    /// <summary>
    /// Stops the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    public void Stop() => StopImpl(this);

    /// <summary>
    /// Stops the clock that was created for this <see cref="Storyboard"/>.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    public void Stop(FrameworkElement containingObject) => StopImpl(containingObject);

    private void StopImpl(DependencyObject containingObject)
    {
        if (GetStoryboardClock(containingObject, false) is TimelineClock clock)
        {
            clock.Controller.Stop();
        }
    }

    /// <summary>
    /// Removes the clock objects that were created for this <see cref="Storyboard"/>. Animations that belong 
    /// to this <see cref="Storyboard"/> no longer affect the properties they once animated, regardless of their 
    /// <see cref="Timeline.FillBehavior"/> setting.
    /// </summary>
    public void Remove() => RemoveImpl(this, true);

    /// <summary>
    /// Removes the clock objects that were created for this <see cref="Storyboard"/>. Animations that belong 
    /// to this <see cref="Storyboard"/> no longer affect the properties they once animated, regardless of their 
    /// <see cref="Timeline.FillBehavior"/> setting.
    /// </summary>
    /// <param name="containingObject">
    /// The object specified when the <see cref="Begin(FrameworkElement, bool)"/> method was called. This object contains 
    /// the clock objects that were created for this storyboard and its children.
    /// </param>
    public void Remove(FrameworkElement containingObject) => RemoveImpl(containingObject, true);

    // This method should only be used by VisualStateManager for Silverlight compatibility. In WPF, Remove fires the Completed
    // event if it was not already fired, but the event is not fired in Silverlight when the storyboard is removed by the VSM.
    internal void RemoveVSM(FrameworkElement containingObject) => RemoveImpl(containingObject, false);

    private void RemoveImpl(DependencyObject containingObject, bool raiseCompletedEvent)
    {
        var clocks = StoryboardClockTreesField.GetValue(containingObject);

        if (clocks is not null && clocks.TryGetValue(this, out WeakReference<TimelineClock> clockReference))
        {
            if (clockReference.TryGetTarget(out TimelineClock clock))
            {
                if (raiseCompletedEvent)
                {
                    clock.Controller.Remove();
                }
                else
                {
                    clock.Controller.Stop();
                }
            }

            clocks.Remove(this);
        }
    }

    internal override TimelineClock CreateClock() => new StoryboardClock(this);

    private static readonly UncommonField<Dictionary<Storyboard, WeakReference<TimelineClock>>> StoryboardClockTreesField = new();

    private TimelineClock GetStoryboardClock(DependencyObject o, bool throwIfNull)
    {
        Debug.Assert(o is not null);

        WeakReference<TimelineClock> weakClock = null;

        var clocks = StoryboardClockTreesField.GetValue(o);

        if (clocks is null || !clocks.TryGetValue(this, out weakClock))
        {
            if (throwIfNull)
            {
                throw new InvalidOperationException(Strings.Storyboard_NeverApplied);
            }
        }

        if (weakClock is not null && weakClock.TryGetTarget(out TimelineClock clock))
        {
            return clock;
        }

        return null;
    }

    private void SetStoryboardClock(DependencyObject o, TimelineClock clock)
    {
        Debug.Assert(o is not null);

        var clocks = StoryboardClockTreesField.GetValue(o);

        if (clocks is null)
        {
            clocks = [];
            StoryboardClockTreesField.SetValue(o, clocks);
        }

        clocks[this] = clock.WeakReference;
    }

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
        bool hasControllableRoot,
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
        if (currentTimeline.GetValue(TargetNameProperty) is string nameString)
        {
            currentObjectName = nameString;
        }

        // The TargetProperty trumps the TargetName property.
        if (currentTimeline.GetValue(TargetProperty) is DependencyObject localTargetObject)
        {
            targetObject = localTargetObject;
            currentObjectName = null;
        }

        if (currentTimeline.GetValue(TargetPropertyProperty) is PropertyPath propertyPath)
        {
            currentPropertyPath = propertyPath;
        }

        if (currentClock is AnimationClock animationClock)
        {
            if (targetObject is null)
            {
                // Resolve the target object name.  If no name specified, use the
                //  containing object.
                if (currentObjectName is not null)
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
                        throw new InvalidOperationException(string.Format(Strings.Storyboard_NoTarget, currentTimeline.GetType()));
                    }
                }
            }

            // See if we have a property name to use.
            if (currentPropertyPath is null)
            {
                throw new InvalidOperationException(string.Format(Strings.Storyboard_TargetPropertyRequired, currentTimeline.GetType()));
            }

            animationClock.SetParent(parentClock);

            (DependencyObject animatedTarget, DependencyProperty animatedProperty) =
                StoryboardPathResolver.Resolve(targetObject, currentPropertyPath);

            animationClock.SetContext(animatedTarget, animatedProperty);
        }
        else
        {
            if (currentClock is not StoryboardClock storyboardClock)
            {
                return;
            }

            storyboardClock.SetParent(parentClock);

            var storyboard = (Storyboard)currentTimeline;
            List<Timeline> childrenTimelines = storyboard.Children.InternalItems;

            for (int i = 0; i < childrenTimelines.Count; i++)
            {
                if (childrenTimelines[i].CreateClock() is not TimelineClock childClock)
                {
                    continue;
                }

                childClock.HasControllableRoot = hasControllableRoot;
                storyboardClock.AddClock(childClock);

                ClockTreeWalkRecursive(
                    childClock,
                    hasControllableRoot,
                    containingObject,
                    storyboardClock,
                    targetObject,
                    currentObjectName,
                    currentPropertyPath);
            }
        }
    }

    private static DependencyObject ResolveTargetName(string targetName, IInternalFrameworkElement fe, INameResolver nameResolver)
    {
        object namedObject;

        if (nameResolver is not null)
        {
            namedObject = nameResolver.Resolve(targetName);
        }
        else if (fe is not null)
        {
            namedObject = fe.FindName(targetName);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_NoNameScope, targetName));
        }

        if (namedObject is null)
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_NameNotFound, targetName, fe.GetType()));
        }

        if (namedObject is not DependencyObject targetObject)
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_TargetNameNotDependencyObject, targetName));
        }

        return targetObject;
    }

    private sealed class StoryboardClock : TimelineClock
    {
        private readonly List<TimelineClock> _children = new();

        public StoryboardClock(Storyboard owner)
            : base(owner)
        {
        }

        public override IEnumerable<TimelineClock> Children => _children;

        public void AddClock(TimelineClock clock)
        {
            Debug.Assert(clock is not null);
            Debug.Assert(!clock.IsRoot);

            _children.Add(clock);
        }

        protected override void OnFrameCore()
        {
            foreach (TimelineClock clock in _children)
            {
                clock.OnFrame(CurrentTime ?? TimeSpan.Zero);
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
