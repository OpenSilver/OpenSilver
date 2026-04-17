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
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class ClockGroup : TimelineClock
{
    private readonly List<TimelineClock> _children = [];

    public ClockGroup(TimelineGroup owner)
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
