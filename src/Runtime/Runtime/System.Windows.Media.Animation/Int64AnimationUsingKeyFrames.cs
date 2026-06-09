
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
/// Animates the value of a <see cref="long"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class Int64AnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<long>
{
    private Int64KeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64AnimationUsingKeyFrames"/> class.
    /// </summary>
    public Int64AnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="Int64KeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="Int64KeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public Int64KeyFrameCollection KeyFrames
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

    IKeyFrameCollection<long> IKeyFrameAnimation<long>.KeyFrames => _frames;

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
    public sealed override Type TargetPropertyType => typeof(long);

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock() =>
       new AnimationClock<long>(this, new KeyFramesAnimator<long>(this));

    long IKeyFrameAnimation<long>.GetCurrentValue(long initialValue, DependencyProperty dp, TimelineClock clock, KeyFramesAnimator<long> animator)
    {
        Debug.Assert(_frames is not null && _frames.Count > 0);

        long currentIterationValue = animator.GetCurrentIterationValue(initialValue, clock);

        // If we're cumulative, we need to multiply the final key frame
        // value by the current repeat count and add this to the return
        // value.
        if (IsCumulative)
        {
            double currentRepeat = (double)(clock.CurrentIteration - 1);

            if (currentRepeat > 0.0)
            {
                currentIterationValue += AnimatedTypeHelpers.ScaleInt64(animator.GetResolvedKeyFrameValue(_frames.Count - 1), currentRepeat);
            }
        }

        // If we're additive we need to add the base value to the return value.
        if (IsAdditive)
        {
            return initialValue + currentIterationValue;
        }

        return currentIterationValue;
    }

    private void SetKeyFrames(Int64KeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="Int16KeyFrame"/> objects.
/// </summary>
public sealed class Int64KeyFrameCollection : PresentationFrameworkCollection<Int64KeyFrame>, IKeyFrameCollection<long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int16KeyFrameCollection"/> class.
    /// </summary>
    public Int64KeyFrameCollection() { }

    internal override void AddOverride(Int64KeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, Int64KeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override Int64KeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, Int64KeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<long> IKeyFrameCollection<long>.this[int index] => GetItemInternal(index);
}
