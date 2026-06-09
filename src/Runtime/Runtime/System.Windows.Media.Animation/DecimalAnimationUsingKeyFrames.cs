
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

using OpenSilver.Internal.Media.Animation;
using System.Diagnostics;
using System.Windows.Markup;

namespace System.Windows.Media.Animation;

/// <summary>
/// Animates the value of a <see cref="decimal"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class DecimalAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<decimal>
{
    private DecimalKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalAnimationUsingKeyFrames"/> class.
    /// </summary>
    public DecimalAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="DecimalKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="DecimalKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public DecimalKeyFrameCollection KeyFrames
    {
        get
        {
            if (_frames is null)
            {
                SetKeyFrames([]);
            }
            return _frames;
        }
        set { SetKeyFrames(value); }
    }

    IKeyFrameCollection<decimal> IKeyFrameAnimation<decimal>.KeyFrames => _frames;

    /// <summary>
    /// Gets a value that specifies whether the animation's output value is added to the base 
    /// value of the property being animated.
    /// </summary>
    /// <returns>
    /// true if the animation adds its output value to the base value of the property being 
    /// animated instead of replacing it; otherwise, false. The default value is false.
    /// </returns>
    public bool IsAdditive
    {
        get => (bool)GetValue(IsAdditiveProperty);
        set => SetValueInternal(IsAdditiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the animation's value accumulates when it repeats.
    /// </summary>
    /// <returns>
    /// true if the animation accumulates its values when its <see cref="Timeline.RepeatBehavior"/> 
    /// property causes it to repeat its simple duration; otherwise, false. The default value is false.
    /// </returns>
    public bool IsCumulative
    {
        get => (bool)GetValue(IsCumulativeProperty);
        set => SetValueInternal(IsCumulativeProperty, value);
    }

    /// <inheritdoc />
    public sealed override Type TargetPropertyType => typeof(decimal);

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock() =>
       new AnimationClock<decimal>(this, new KeyFramesAnimator<decimal>(this));

    decimal IKeyFrameAnimation<decimal>.GetCurrentValue(decimal initialValue, DependencyProperty dp, TimelineClock clock, KeyFramesAnimator<decimal> animator)
    {
        Debug.Assert(_frames is not null && _frames.Count > 0);

        decimal currentIterationValue = animator.GetCurrentIterationValue(initialValue, clock);

        // If we're cumulative, we need to multiply the final key frame
        // value by the current repeat count and add this to the return
        // value.
        if (IsCumulative)
        {
            double currentRepeat = (double)(clock.CurrentIteration - 1);

            if (currentRepeat > 0.0)
            {
                currentIterationValue += animator.GetResolvedKeyFrameValue(_frames.Count - 1) * (decimal)currentRepeat;
            }
        }

        // If we're additive we need to add the base value to the return value.
        if (IsAdditive)
        {
            return initialValue + currentIterationValue;
        }

        return currentIterationValue;
    }

    private void SetKeyFrames(DecimalKeyFrameCollection keyFrames)
    {
        if (_frames is not null)
        {
            RemoveSelfAsInheritanceContext(_frames, null);
        }

        _frames = keyFrames;

        if (_frames is not null)
        {
            ProvideSelfAsInheritanceContext(_frames, null);
        }
    }
}

/// <summary>
/// Represents a collection of <see cref="DecimalKeyFrame"/> objects.
/// </summary>
public sealed class DecimalKeyFrameCollection : PresentationFrameworkCollection<DecimalKeyFrame>, IKeyFrameCollection<decimal>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalKeyFrameCollection"/> class.
    /// </summary>
    public DecimalKeyFrameCollection() { }

    internal override void AddOverride(DecimalKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, DecimalKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override DecimalKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, DecimalKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<decimal> IKeyFrameCollection<decimal>.this[int index] => GetItemInternal(index);
}
