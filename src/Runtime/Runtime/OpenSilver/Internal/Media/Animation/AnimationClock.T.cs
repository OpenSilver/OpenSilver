
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class AnimationClock<TValue> : AnimationClock
{
    private readonly IValueAnimator<TValue> _animator;

    private DependencyObject _target;
    private DependencyProperty _dp;
    private TValue _initialValue;

    public AnimationClock(AnimationTimeline owner, IValueAnimator<TValue> animator)
        : base(owner)
    {
        Debug.Assert(animator is not null);
        _animator = animator;
    }

    public new AnimationTimeline Timeline => (AnimationTimeline)base.Timeline;

    public override void SetContext(DependencyObject target, DependencyProperty targetProperty)
    {
        Debug.Assert(target is not null);
        Debug.Assert(targetProperty is not null);

        _target = target;
        _dp = targetProperty;
        _initialValue = (TValue)target.GetValue(targetProperty);

        target.AttachAnimationClock(targetProperty, this);
    }

    public override object GetCurrentValue()
    {
        Debug.Assert(CurrentState != ClockState.Stopped);
        return _animator.GetCurrentValue(_initialValue, _dp, this);
    }

    protected override void OnFrameCore() => _target.RefreshAnimation(_dp, this);

    protected override void OnStopCore() => _target.DetachAnimationClock(_dp, this);

    public override Duration IterationDuration
    {
        get
        {
            Duration duration = Timeline.Duration;
            if (duration == Duration.Automatic)
            {
                duration = Timeline.NaturalDuration;
            }

            return duration;
        }
    }
}
