
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal readonly struct ClockController
{
    private readonly TimelineClock _clock;

    public ClockController(TimelineClock clock)
    {
        Debug.Assert(clock is not null);
        _clock = clock;
    }

    public void Pause() => _clock?.InternalPause();

    public void Resume() => _clock?.InternalResume();

    public void Seek(TimeSpan offset, TimeSeekOrigin origin)
    {
        if (!IsValidTimeSeekOrigin(origin))
        {
            throw new InvalidEnumArgumentException($"'{origin}' enumeration value is not valid.");
        }

        if (_clock is TimelineClock clock)
        {
            if (origin == TimeSeekOrigin.Duration)
            {
                Duration duration = _clock.IterationDuration;

                if (!duration.HasTimeSpan)
                {
                    // Can't seek relative to the Duration if it has been specified as Forevor or if
                    // it has not yet been resolved.
                    throw new InvalidOperationException(Strings.Timing_SeekDestinationIndefinite);
                }

                offset += duration.TimeSpan;
            }

            // Any offset greater than zero is OK here. If it's past the effective
            // duration it means execute the FillBehavior.
            if (offset < TimeSpan.Zero)
            {
                throw new InvalidOperationException(Strings.Timing_SeekDestinationNegative);
            }

            clock.InternalSeek(offset);
        }
    }

    public void SeekAlignedToLastTick(TimeSpan offset, TimeSeekOrigin origin)
    {
        if (!IsValidTimeSeekOrigin(origin))
        {
            throw new InvalidEnumArgumentException($"'{origin}' enumeration value is not valid.");
        }

        if (_clock is TimelineClock clock)
        {
            if (origin == TimeSeekOrigin.Duration)
            {
                Duration duration = _clock.IterationDuration;

                if (!duration.HasTimeSpan)
                {
                    // Can't seek relative to the Duration if it has been specified as Forevor or if
                    // it has not yet been resolved.
                    throw new InvalidOperationException(Strings.Timing_SeekDestinationIndefinite);
                }

                offset += duration.TimeSpan;
            }

            // Any offset greater than zero is OK here. If it's past the effective
            // duration it means execute the FillBehavior.
            if (offset < TimeSpan.Zero)
            {
                throw new InvalidOperationException(Strings.Timing_SeekDestinationNegative);
            }

            clock.InternalSeekAlignedToLastTick(offset);
        }
    }

    public void SkipToFill() => _clock?.InternalSkipToFill();

    public void Stop() => _clock?.InternalStop();

    public void Remove() => _clock?.InternalRemove();

    private static bool IsValidTimeSeekOrigin(TimeSeekOrigin value)
    {
        return value == TimeSeekOrigin.BeginTime || value == TimeSeekOrigin.Duration;
    }
}
