
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
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal abstract class TimelineClock
{
    private WeakReference<TimelineClock> _weakReference;
    private TimelineClock _parent;
    private ClockFlags _flags = 0;

    protected TimelineClock(Timeline owner, bool isRoot)
    {
        Debug.Assert(owner is not null);
        Timeline = owner;
        IsRoot = isRoot;
        Clock = new ControllableStopwatch();
        CurrentState = ClockState.Stopped;
    }

    public ControllableStopwatch Clock { get; }

    public Timeline Timeline { get; }

    public virtual IEnumerable<TimelineClock> Children => Enumerable.Empty<TimelineClock>();

    public TimeSpan CurrentTime { get; private set; }

    public double CurrentProgress
    {
        get
        {
            Duration duration = IterationDuration;

            if (duration == Duration.Forever)
            {
                return 0.0;
            }

            Debug.Assert(duration.HasTimeSpan);

            if (duration.TimeSpan == TimeSpan.Zero)
            {
                return 1.0;
            }

            return Math.Min((double)CurrentTime.Ticks / duration.TimeSpan.Ticks, 1.0);
        }
    }

    public int CurrentIteration { get; private set; }

    public ClockState CurrentState { get; private set; }

    public abstract Duration IterationDuration { get; }

    public Duration EffectiveDuration
    {
        get
        {
            Duration iterationDuration = IterationDuration;
            RepeatBehavior repeatBehavior = Timeline.RepeatBehavior;

            if (iterationDuration.HasTimeSpan && iterationDuration.TimeSpan == TimeSpan.Zero)
            {
                // Zero-duration case ignores any repeat behavior
                return TimeSpan.Zero;
            }
            else if (repeatBehavior.HasCount)
            {
                // This clause avoids multiplying an infinite duration by zero
                if (repeatBehavior.Count == 0)
                {
                    return TimeSpan.Zero;
                }
                else if (iterationDuration == Duration.Forever)
                {
                    return Duration.Forever;
                }
                else
                {
                    return MultiplyTimeSpan(iterationDuration.TimeSpan, repeatBehavior.Count);
                }
            }
            else if (repeatBehavior.HasDuration)
            {
                return repeatBehavior.Duration;
            }
            else
            {
                return Duration.Forever;
            }
        }
    }

    public void Begin(bool alignedToLastTick)
    {
        EnsureRootClock();

        if (!IsActive)
        {
            IsActive = true;
            Clock.Start();
            RequestNextFrames(true);
            if (alignedToLastTick)
            {
                OnFrame(TimeSpan.Zero);
            }
        }
    }

    public void Pause()
    {
        EnsureRootClock();

        if (!IsActive) return;

        if (!IsInteractivelyPaused)
        {
            IsInteractivelyPaused = true;
            Clock.Stop();
            RequestNextFrames(false);
        }
    }

    public void Resume()
    {
        EnsureRootClock();

        if (!IsActive) return;

        if (IsInteractivelyPaused)
        {
            IsInteractivelyPaused = false;
            Clock.Start();
            RequestNextFrames(true);
        }
    }

    public void Seek(TimeSpan offset)
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalSeek(offset, false);
    }

    public void SeekAlignedToLastTick(TimeSpan offset)
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalSeek(offset, true);
    }

    public void SkipToFill()
    {
        EnsureRootClock();

        if (!IsActive) return;

        Duration effectiveDuration = EffectiveDuration;
        if (effectiveDuration == Duration.Forever)
        {
            throw new InvalidOperationException(Strings.Timing_SkipToFillDestinationIndefinite);
        }

        InternalSeek(effectiveDuration.TimeSpan + BeginTime, true);
    }

    public void Stop()
    {
        EnsureRootClock();

        if (!IsActive) return;

        if (!IsInteractivelyStopped)
        {
            IsInteractivelyStopped = true;
            RequestNextFrames(false);
            OnStop();
        }
    }

    public void SetParent(TimelineClock parent) => _parent = parent;

    public void OnFrame(TimeSpan frameTime)
    {
        if (frameTime < BeginTime)
        {
            ResetCachedStateToStopped();
            return;
        }

        UpdateLocalState(frameTime);

        OnFrameCore();

        if (IsCompleted)
        {
            RaiseCompletedForRoot();
        }
    }

    public void OnStop()
    {
        CurrentState = ClockState.Stopped;
        OnStopCore();
    }

    public abstract void SetContext(DependencyObject target, PropertyPath targetProperty);

    protected abstract void OnFrameCore();

    protected abstract void OnStopCore();

    internal WeakReference<TimelineClock> WeakReference => _weakReference ??= new(this);

    internal TimeSpan BeginTime => Timeline.BeginTime ?? TimeSpan.Zero;

    internal bool IsRoot
    {
        get => ReadFlag(ClockFlags.IsRoot);
        private set => SetFlag(ClockFlags.IsRoot, value);
    }

    private bool IsActive
    {
        get => ReadFlag(ClockFlags.IsActive);
        set => SetFlag(ClockFlags.IsActive, value);
    }

    private bool IsInteractivelyPaused
    {
        get => ReadFlag(ClockFlags.IsInteractivelyPaused);
        set => SetFlag(ClockFlags.IsInteractivelyPaused, value);
    }

    private bool IsInteractivelyStopped
    {
        get => ReadFlag(ClockFlags.IsInteractivelyStopped);
        set => SetFlag(ClockFlags.IsInteractivelyStopped, value);
    }

    private bool NextFrameRequested
    {
        get => ReadFlag(ClockFlags.NextFrameRequested);
        set => SetFlag(ClockFlags.NextFrameRequested, value);
    }

    private bool CompletedEventRaised
    {
        get => ReadFlag(ClockFlags.CompletedEventRaised);
        set => SetFlag(ClockFlags.CompletedEventRaised, value);
    }

    private bool IsCompleted
    {
        get => ReadFlag(ClockFlags.IsCompleted);
        set => SetFlag(ClockFlags.IsCompleted, value);
    }

    private void InternalSeek(TimeSpan offset, bool align)
    {
        IsCompleted = false;
        Clock.Seek(offset);
        RequestNextFrames(true);
        if (align)
        {
            OnFrame(offset);
        }
    }

    private void RequestNextFrames(bool value)
    {
        Debug.Assert(IsRoot);

        if (NextFrameRequested != value)
        {
            NextFrameRequested = value;
            AnimationManager.Current.RequestNextTicks(this, value);
        }
    }

    private TimeSpan GetAdjustedTime(TimeSpan currentTime) => currentTime - BeginTime;

    private void UpdateLocalState(TimeSpan frameTime)
    {
        TimeSpan localTime = GetAdjustedTime(frameTime);

        Duration effectiveDuration = EffectiveDuration;
        if (effectiveDuration.HasTimeSpan && localTime >= effectiveDuration.TimeSpan)
        {
            SetCompletedForRoot();

            CurrentState = ClockState.Filling;

            localTime = effectiveDuration.TimeSpan;
        }
        else
        {
            CurrentState = ClockState.Active;
        }

        Duration iterationDuration = IterationDuration;
        if (iterationDuration == Duration.Forever)
        {
            CurrentIteration = 1;
            CurrentTime = localTime;
            return;
        }

        Debug.Assert(iterationDuration.HasTimeSpan);

        if (iterationDuration.TimeSpan == TimeSpan.Zero || localTime == TimeSpan.Zero)
        {
            CurrentIteration = 1; // Arbitrary value
            CurrentTime = TimeSpan.Zero;
            return;
        }

        int nbIterations = (int)Math.DivRem(localTime.Ticks, iterationDuration.TimeSpan.Ticks, out long currentTimeTicks);
        if (CurrentState == ClockState.Filling && currentTimeTicks == 0)
        {
            CurrentIteration = nbIterations;
            CurrentTime = iterationDuration.TimeSpan;
        }
        else
        {
            CurrentIteration = nbIterations + 1;
            CurrentTime = TimeSpan.FromTicks(currentTimeTicks);
        }
    }

    private void ResetCachedStateToStopped()
    {
        CurrentIteration = 1;
        CurrentTime = TimeSpan.Zero;
        CurrentState = ClockState.Stopped;
    }

    private void SetCompletedForRoot()
    {
        if (IsRoot)
        {
            IsCompleted = true;
            RequestNextFrames(false);
        }
    }

    private void RaiseCompletedForRoot()
    {
        Debug.Assert(IsRoot);
        RaiseCompletedRecursively(this);

        static void RaiseCompletedRecursively(TimelineClock rootClock)
        {
            rootClock.RaiseCompleted();

            foreach (TimelineClock clock in rootClock.Children)
            {
                RaiseCompletedRecursively(clock);
            }
        }
    }

    private void RaiseCompleted()
    {
        if (!CompletedEventRaised)
        {
            CompletedEventRaised = true;
            Timeline.RaiseCompleted();
        }
    }

    /// <summary>
    /// Helper for more elegant code multiplying a TimeSpan by a double
    /// </summary>
    private static TimeSpan MultiplyTimeSpan(TimeSpan timeSpan, double factor) =>
        TimeSpan.FromTicks((long)(factor * timeSpan.Ticks + 0.5));

    private bool ReadFlag(ClockFlags flag) => (_flags & flag) != 0;

    private void SetFlag(ClockFlags flag, bool value)
    {
        if (value)
        {
            _flags |= flag;
        }
        else
        {
            _flags &= ~flag;
        }
    }

    private void EnsureRootClock()
    {
        if (!IsRoot)
        {
            throw new InvalidOperationException(Strings.Timing_MustBeRoot);
        }
    }

    [Flags]
    private enum ClockFlags
    {
        IsRoot = 1 << 0,
        IsActive = 1 << 1,
        IsInteractivelyPaused = 1 << 2,
        IsInteractivelyStopped = 1 << 3,
        NextFrameRequested = 1 << 4,
        CompletedEventRaised = 1 << 5,
        IsCompleted = 1 << 6,
    }
}
