
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using OpenSilver.Internal.Data;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class AnimationClock<TValue> : AnimationClock
{
    private readonly IValueAnimator<TValue> _animator;

    private DependencyObject _target;
    private DependencyProperty _dp;
    private TValue _initialValue;

    public AnimationClock(AnimationTimeline owner, bool isRoot, IValueAnimator<TValue> animator)
        : base(owner, isRoot)
    {
        Debug.Assert(animator is not null);
        _animator = animator;
    }

    public new AnimationTimeline Timeline => (AnimationTimeline)base.Timeline;

    public override void SetContext(DependencyObject rootTarget, PropertyPath targetProperty)
    {
        Initialize(rootTarget, targetProperty);
        _target.AttachAnimationClock(_dp, this);
    }

    public override object GetCurrentValue() => _animator.GetCurrentValue(_initialValue, _dp, this);

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

    private void Initialize(DependencyObject rootTarget, PropertyPath targetProperty)
    {
        if (targetProperty.DependencyProperty is not null)
        {
            _target = rootTarget;
            _dp = targetProperty.DependencyProperty;
            return;
        }

        IReadOnlyList<SourceValueInfo> parts = targetProperty.SVI;
        if (parts.Count > 0)
        {
            DependencyObject target = rootTarget;
            for (int i = 0; i < parts.Count - 1; i++)
            {
                SourceValueInfo svi = parts[i];
                switch (svi.type)
                {
                    case PropertyNodeType.Property:
                        DependencyProperty dp = DPFromName(svi.propertyName, svi.typeName, target.GetType());
                        var value = AsDependencyObject(target.GetValue(dp));
                        if (i == 0 && value is ICloneOnAnimation<DependencyObject> cloneable && !cloneable.IsClone)
                        {
                            value = cloneable.Clone();
                            target.SetValueInternal(dp, value);
                        }
                        target = value;
                        break;

                    case PropertyNodeType.Indexed:
                        if (target is not IList list)
                        {
                            throw new InvalidOperationException($"'{target}' must implement IList.");
                        }
                        if (!int.TryParse(svi.param, out int index))
                        {
                            throw new InvalidOperationException($"'{svi.param}' can't be converted to an integer value.");
                        }

                        target = AsDependencyObject(list[index]);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            _target = target;
            _dp = DPFromName(parts[parts.Count - 1].propertyName, parts[parts.Count - 1].typeName, target.GetType());
            _initialValue = (TValue)_target.GetValue(_dp);
            return;
        }

        throw new InvalidOperationException();
    }

    private static DependencyObject AsDependencyObject(object o) =>
        o as DependencyObject ??
        throw new InvalidOperationException($"'{o}' must be a DependencyObject.");

    private static DependencyProperty DPFromName(string propertyName, string typeName, Type ownerType) =>
        GetKnownProperty(propertyName, typeName) ??
        DependencyProperty.FromName(propertyName, ownerType) ??
        throw new InvalidOperationException($"No DependencyProperty named '{propertyName}' could be found in '{ownerType}'.");
}
