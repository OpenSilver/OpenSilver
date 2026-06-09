
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
/// Animates the value of a <see cref="short"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class Int16AnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<short>
{
    private Int16KeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int16AnimationUsingKeyFrames"/> class.
    /// </summary>
    public Int16AnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="Int16KeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="Int16KeyFrame"/> objects that define the animation. The default value is an 
    /// empty collection.
    /// </returns>
    public Int16KeyFrameCollection KeyFrames
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

    IKeyFrameCollection<short> IKeyFrameAnimation<short>.KeyFrames => _frames;

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
    public sealed override Type TargetPropertyType => typeof(short);

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock() =>
       new AnimationClock<short>(this, new KeyFramesAnimator<short>(this));

    short IKeyFrameAnimation<short>.GetCurrentValue(short initialValue, DependencyProperty dp, TimelineClock clock, KeyFramesAnimator<short> animator)
    {
        Debug.Assert(_frames is not null && _frames.Count > 0);

        short currentIterationValue = animator.GetCurrentIterationValue(initialValue, clock);

        // If we're cumulative, we need to multiply the final key frame
        // value by the current repeat count and add this to the return
        // value.
        if (IsCumulative)
        {
            double currentRepeat = (double)(clock.CurrentIteration - 1);

            if (currentRepeat > 0.0)
            {
                currentIterationValue = AnimatedTypeHelpers.AddInt16(
                    currentIterationValue,
                    AnimatedTypeHelpers.ScaleInt16(animator.GetResolvedKeyFrameValue(_frames.Count - 1), currentRepeat));
            }
        }

        // If we're additive we need to add the base value to the return value.
        if (IsAdditive)
        {
            return AnimatedTypeHelpers.AddInt16(initialValue, currentIterationValue);
        }

        return currentIterationValue;
    }

    private void SetKeyFrames(Int16KeyFrameCollection keyFrames)
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
public sealed class Int16KeyFrameCollection : PresentationFrameworkCollection<Int16KeyFrame>, IKeyFrameCollection<short>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int16KeyFrameCollection"/> class.
    /// </summary>
    public Int16KeyFrameCollection() { }

    internal override void AddOverride(Int16KeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, Int16KeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override Int16KeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, Int16KeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<short> IKeyFrameCollection<short>.this[int index] => GetItemInternal(index);
}
