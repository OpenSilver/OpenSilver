
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

    protected TimelineClock(Timeline owner)
    {
        Debug.Assert(owner is not null);
        Timeline = owner;
        Clock = new ControllableStopwatch();
        CurrentState = ClockState.Stopped;
    }

    public ControllableStopwatch Clock { get; }

    public Timeline Timeline { get; }

    public virtual IEnumerable<TimelineClock> Children => Enumerable.Empty<TimelineClock>();

    public TimeSpan CurrentTime { get; private set; }

    public bool IsPaused => IsInteractivelyPaused;

    public double CurrentProgress { get; private set; }

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
                    double scalingFactor = repeatBehavior.Count;
                    if (Timeline.AutoReverse)
                    {
                        scalingFactor *= 2;
                    }

                    return MultiplyTimeSpan(iterationDuration.TimeSpan, scalingFactor);
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

    internal void InternalBegin(bool alignedToLastTick)
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

    internal void InternalPause()
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

    internal void InternalResume()
    {
        EnsureRootClock();

        if (!IsActive) return;

        if (IsInteractivelyPaused)
        {
            IsInteractivelyPaused = false;
            Clock.Start();

            if (CurrentState == ClockState.Active)
            {
                RequestNextFrames(true);
            }
        }
    }

    internal void InternalSeek(TimeSpan offset)
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalSeek(offset + BeginTime, false);
    }

    internal void InternalSeekAlignedToLastTick(TimeSpan offset)
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalSeek(offset + BeginTime, true);
    }

    internal void InternalSkipToFill()
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

    internal void InternalStop()
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalStop(false);
    }

    internal void InternalRemove()
    {
        EnsureRootClock();

        if (!IsActive) return;

        InternalStop(true);
    }

    internal void SetParent(TimelineClock parent) => _parent = parent;

    public void OnFrame(TimeSpan frameTime)
    {
        if (_parent is not null && _parent.CurrentState == ClockState.Stopped)
        {
            if (CurrentState != ClockState.Stopped)
            {
                ResetCachedStateToStopped();
                OnFrameCore();
            }
            return;
        }

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
        ResetCachedStateToStopped();
        OnStopCore();
    }

    protected abstract void OnFrameCore();

    protected abstract void OnStopCore();

    internal ClockController Controller
    {
        get
        {
            if (IsRoot && HasControllableRoot)
            {
                return new ClockController(this);
            }

            return default;
        }
    }

    internal WeakReference<TimelineClock> WeakReference => _weakReference ??= new(this);

    internal TimeSpan BeginTime => Timeline.BeginTime ?? TimeSpan.Zero;

    internal bool IsRoot
    {
        get => ReadFlag(ClockFlags.IsRoot);
        set => SetFlag(ClockFlags.IsRoot, value);
    }

    internal bool HasControllableRoot
    {
        get => ReadFlag(ClockFlags.HasControllableRoot);
        set => SetFlag(ClockFlags.HasControllableRoot, value);
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

    private void InternalStop(bool raiseCompleted)
    {
        if (!IsInteractivelyStopped)
        {
            IsInteractivelyStopped = true;
            RequestNextFrames(false);
            OnStop();

            if (raiseCompleted)
            {
                RaiseCompletedForRoot();
            }
        }
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

            if (Timeline.FillBehavior == FillBehavior.Stop)
            {
                ResetCachedStateToStopped();
                return;
            }

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
            CurrentProgress = 0;
            return;
        }

        Debug.Assert(iterationDuration.HasTimeSpan);

        if (iterationDuration.TimeSpan == TimeSpan.Zero)
        {
            CurrentIteration = 1; // Arbitrary value
            CurrentTime = TimeSpan.Zero;
            CurrentProgress = 1;
            return;
        }

        if (localTime == TimeSpan.Zero)
        {
            CurrentIteration = 1;
            CurrentTime = TimeSpan.Zero;
            CurrentProgress = 0;
            return;
        }

        int iteration = (int)Math.DivRem(localTime.Ticks, iterationDuration.TimeSpan.Ticks, out long ticks);
        if (CurrentState == ClockState.Filling && ticks == 0)
        {
            TimeSpan time = iterationDuration.TimeSpan;
            double progress = 1;

            if (Timeline.AutoReverse)
            {
                if ((iteration & 1) == 0) // We are on a reversing segment
                {
                    progress = 0;
                    time = TimeSpan.Zero;
                }
                iteration /= 2;
            }

            CurrentIteration = iteration;
            CurrentTime = time;
            CurrentProgress = progress;
        }
        else
        {
            TimeSpan time = TimeSpan.FromTicks(ticks);
            double progress = Math.Min((double)time.Ticks / iterationDuration.TimeSpan.Ticks, 1.0);

            if (Timeline.AutoReverse)
            {
                if ((iteration & 1) == 1) // We are on a reversing segment
                {
                    progress = 1 - progress;
                    time = iterationDuration.TimeSpan - time;
                }
                iteration /= 2;
            }

            CurrentIteration = iteration + 1;
            CurrentTime = time;
            CurrentProgress = progress;
        }
    }

    private void ResetCachedStateToStopped()
    {
        CurrentIteration = 0;
        CurrentTime = TimeSpan.Zero;
        CurrentProgress = 0;
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
        HasControllableRoot = 1 << 7,
    }
}
