
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
using CSHTML5.Internal;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class AnimationManager
{
    private const int DefaultFrameRate = 60;

    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly ClockCollection _rootClocks = new();
    private readonly Queue<(TimelineClock Clock, bool Add)> _pendingRequests = new();

    private EventHandler _requestAnimationFrame;
    private bool _isRunning;
    private bool _isProcessingFrame;
    private int _frameRate = 60;

    private AnimationManager()
    {
        var jsCallback = JavaScriptCallback.Create(OnRequestAnimationFrameNative, true);
        string sHandler = CSHTML5.InteropImplementation.GetVariableStringForJS(jsCallback);
        Interop.ExecuteJavaScriptVoid($"document.createAnimationManager({sHandler});");
        SetFrameRate(DefaultFrameRate);
    }

    public static AnimationManager Current { get; } = new AnimationManager();

    internal event EventHandler RequestAnimationFrame
    {
        add
        {
            _requestAnimationFrame += value;
            if (_requestAnimationFrame is not null)
            {
                Resume();
            }
        }
        remove
        {
            _requestAnimationFrame -= value;
            TryPause();
        }
    }

    internal int FrameRate
    {
        get => _frameRate;
        set
        {
            _frameRate = value;
            SetFrameRate(_frameRate);
        }
    }

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
            Resume();
        }
        else
        {
            RemoveClock(clock);
            TryPause();
        }
    }

    private void Resume()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            Interop.ExecuteJavaScriptVoid("document.animationManager.resume();");
        }
    }

    private void Pause()
    {
        if (_isRunning)
        {
            _isRunning = false;
            Interop.ExecuteJavaScriptVoid("document.animationManager.pause();");
        }
    }

    private void SetFrameRate(int frameRate) =>
        Interop.ExecuteJavaScriptVoid($"document.animationManager.setFrameRate({frameRate.ToInvariantString()});");

    private void AddClock(TimelineClock clock) => _rootClocks.Add(clock);

    private void RemoveClock(TimelineClock clock) => _rootClocks.Remove(clock);

    private void TryPause()
    {
        if (_requestAnimationFrame is null && _rootClocks.Count == 0)
        {
            Pause();
        }
    }

    private void OnRequestAnimationFrameNative()
    {
        _requestAnimationFrame?.Invoke(Dispatcher.CurrentDispatcher, new RenderingEventArgs(_clock.Elapsed));

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
            TryPause();
        }
    }

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
