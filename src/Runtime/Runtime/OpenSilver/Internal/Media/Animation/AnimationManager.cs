
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
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class AnimationManager
{
    private readonly Dispatcher _dispatcher;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly ClockCollection _rootClocks = new();
    private readonly Queue<(TimelineClock Clock, bool Add)> _pendingRequests = new();

    private bool _isProcessingFrame;

    private AnimationManager(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _dispatcher.Tick += new EventHandler(OnDispatcherTick);
    }

    private void OnDispatcherTick(object sender, EventArgs e)
    {
        RequestAnimationFrame?.Invoke(this, new RenderingEventArgs(_clock.Elapsed));

        _isProcessingFrame = true;
        try
        {
            foreach (TimelineClock clock in _rootClocks)
            {
                clock.OnFrame(clock.Clock.Elapsed);
            }
        }
        finally
        {
            _isProcessingFrame = false;

            _rootClocks.Purge();
            ProcessPendingRequests();
        }
    }

    public static AnimationManager Current { get; } = new AnimationManager(Dispatcher.CurrentDispatcher);

    internal event EventHandler RequestAnimationFrame;

    internal void RequestNextTicks(TimelineClock clock, bool value)
    {
        if (_isProcessingFrame)
        {
            _pendingRequests.Enqueue((clock, value));
            return;
        }

        if (value)
        {
            AddClock(clock);
        }
        else
        {
            RemoveClock(clock);
        }
    }

    private void AddClock(TimelineClock clock) => _rootClocks.Add(clock);

    private void RemoveClock(TimelineClock clock) => _rootClocks.Remove(clock);

    private void ProcessPendingRequests()
    {
        Debug.Assert(!_isProcessingFrame);

        while (_pendingRequests.Count > 0)
        {
            (TimelineClock clock, bool add) = _pendingRequests.Dequeue();
            if (add)
            {
                AddClock(clock);
            }
            else
            {
                RemoveClock(clock);
            }
        }
    }
}
