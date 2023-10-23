
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
using CSHTML5.Internal;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class AnimationManager
{
    private const int DefaultFrameRate = 60;

    private readonly JavaScriptCallback _handler;
    private readonly ClockCollection _rootClocks = new();
    private readonly Queue<(TimelineClock Clock, bool Add)> _pendingRequests = new();

    private bool _isRunning;
    private bool _isProcessingFrame;
    private int _frameRate = 60;

    private AnimationManager()
    {
        _handler = JavaScriptCallback.Create(OnRequestAnimationFrameNative, true);
        string sHandler = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_handler);
        Interop.ExecuteJavaScriptVoid($"document.createAnimationManager({sHandler});");
        SetFrameRate(DefaultFrameRate);
    }

    public static AnimationManager Current { get; } = new AnimationManager();

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
            if (_rootClocks.Count == 0)
            {
                Pause();
            }
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

    private void OnRequestAnimationFrameNative()
    {
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

            if (_rootClocks.Count == 0)
            {
                Pause();
            }
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
