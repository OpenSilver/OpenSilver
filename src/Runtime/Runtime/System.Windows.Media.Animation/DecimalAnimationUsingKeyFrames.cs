
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

using System.Windows.Markup;
using OpenSilver.Internal.Media.Animation;

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
                SetKeyFrames(new());
            }
            return _frames;
        }
        set { SetKeyFrames(value); }
    }

    IKeyFrameCollection<decimal> IKeyFrameAnimation<decimal>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<decimal>(this, isRoot, new KeyFramesAnimator<decimal>(this));

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
