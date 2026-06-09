
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
/// Animates the value of a <see cref="byte"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class ByteAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<byte>
{
    private ByteKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteAnimationUsingKeyFrames"/> class.
    /// </summary>
    public ByteAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="ByteKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="ByteKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public ByteKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<byte> IKeyFrameAnimation<byte>.KeyFrames => _frames;

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
    public sealed override Type TargetPropertyType => typeof(byte);

    bool IKeyFrameAnimation<byte>.IsAdditive => throw new NotImplementedException();

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock() =>
       new AnimationClock<byte>(this, new KeyFramesAnimator<byte>(this));

    private void SetKeyFrames(ByteKeyFrameCollection keyFrames)
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

    byte IKeyFrameAnimation<byte>.GetCurrentValue(byte initialValue, DependencyProperty dp, TimelineClock clock, KeyFramesAnimator<byte> animator)
    {
        Debug.Assert(_frames is not null && _frames.Count > 0);

        byte currentIterationValue = animator.GetCurrentIterationValue(initialValue, clock);

        // If we're cumulative, we need to multiply the final key frame
        // value by the current repeat count and add this to the return
        // value.
        if (IsCumulative)
        {
            double currentRepeat = (double)(clock.CurrentIteration - 1);

            if (currentRepeat > 0.0)
            {
                currentIterationValue = AnimatedTypeHelpers.AddByte(
                    currentIterationValue,
                    AnimatedTypeHelpers.ScaleByte(animator.GetResolvedKeyFrameValue(_frames.Count - 1), currentRepeat));
            }
        }

        // If we're additive we need to add the base value to the return value.
        if (IsAdditive)
        {
            return AnimatedTypeHelpers.AddByte(initialValue, currentIterationValue);
        }

        return currentIterationValue;
    }
}

/// <summary>
/// Represents a collection of <see cref="ByteKeyFrame"/> objects.
/// </summary>
public sealed class ByteKeyFrameCollection : PresentationFrameworkCollection<ByteKeyFrame>, IKeyFrameCollection<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteKeyFrameCollection"/> class.
    /// </summary>
    public ByteKeyFrameCollection() { }

    internal override void AddOverride(ByteKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, ByteKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override ByteKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ByteKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<byte> IKeyFrameCollection<byte>.this[int index] => GetItemInternal(index);
}
