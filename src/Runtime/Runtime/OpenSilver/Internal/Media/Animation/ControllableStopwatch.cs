
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
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Animation;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal sealed class ControllableStopwatch
{
    private const long TicksPerMillisecond = 10000;
    private const long TicksPerSecond = TicksPerMillisecond * 1000;

    private long _elapsed;
    private long _startTimeStamp;
    private bool _isRunning;

    // "Frequency" stores the frequency of the high-resolution performance counter,
    // if one exists. Otherwise it will store TicksPerSecond.
    // The frequency cannot change while the system is running,
    // so we only need to initialize it once.
    public static readonly long Frequency = Stopwatch.Frequency;
    public static readonly bool IsHighResolution = true;

    // performance-counter frequency, in counts per ticks.
    // This can speed up conversion from high frequency performance-counter
    // to ticks.
    private static readonly double s_tickFrequency = (double)TicksPerSecond / Frequency;

    public ControllableStopwatch()
    {
        _elapsed = 0;
        _isRunning = false;
        _startTimeStamp = 0;
    }

    public void Start()
    {
        // Calling start on a running Stopwatch is a no-op.
        if (!_isRunning)
        {
            _startTimeStamp = Stopwatch.GetTimestamp();
            _isRunning = true;
        }
    }

    public void Stop()
    {
        // Calling stop on a stopped Stopwatch is a no-op.
        if (_isRunning)
        {
            long endTimeStamp = Stopwatch.GetTimestamp();
            long elapsedThisPeriod = endTimeStamp - _startTimeStamp;
            if (elapsedThisPeriod < 0)
            {
                // When measuring small time periods the Stopwatch.Elapsed*
                // properties can return negative values.  This is due to
                // bugs in the basic input/output system (BIOS) or the hardware
                // abstraction layer (HAL) on machines with variable-speed CPUs
                // (e.g. Intel SpeedStep).

                elapsedThisPeriod = 0;
            }

            _elapsed += elapsedThisPeriod;
            _isRunning = false;
        }
    }

    public void Seek(TimeSpan offset)
    {
        _elapsed = unchecked((long)(offset.Ticks / s_tickFrequency));
        _startTimeStamp = Stopwatch.GetTimestamp();
    }

    public TimeSpan Elapsed
    {
        get { return new TimeSpan(GetElapsedDateTimeTicks()); }
    }

    // Get the elapsed ticks.
    private long GetRawElapsedTicks()
    {
        long timeElapsed = _elapsed;

        if (_isRunning)
        {
            // If the Stopwatch is running, add elapsed time since
            // the Stopwatch is started last time.
            long currentTimeStamp = Stopwatch.GetTimestamp();
            long elapsedUntilNow = currentTimeStamp - _startTimeStamp;
            timeElapsed += elapsedUntilNow;
        }
        return timeElapsed;
    }

    // Get the elapsed ticks.
    private long GetElapsedDateTimeTicks()
    {
        Debug.Assert(IsHighResolution);
        // convert high resolution perf counter to DateTime ticks
        return unchecked((long)(GetRawElapsedTicks() * s_tickFrequency));
    }

    private string DebuggerDisplay => $"{Elapsed} (IsRunning = {_isRunning})";
}
