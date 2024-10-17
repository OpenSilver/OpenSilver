
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
/// Animates the value of a <see cref="Size"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class SizeAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Size>
{
    private SizeKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeAnimationUsingKeyFrames"/> class.
    /// </summary>
    public SizeAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="SizeKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="SizeKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public SizeKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<Size> IKeyFrameAnimation<Size>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<Size>(this, isRoot, new KeyFramesAnimator<Size>(this));

    private void SetKeyFrames(SizeKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="SizeKeyFrame"/> objects.
/// </summary>
public sealed class SizeKeyFrameCollection : PresentationFrameworkCollection<SizeKeyFrame>, IKeyFrameCollection<Size>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeKeyFrameCollection"/> class.
    /// </summary>
    public SizeKeyFrameCollection() { }

    internal override void AddOverride(SizeKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, SizeKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override SizeKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, SizeKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Size> IKeyFrameCollection<Size>.this[int index] => GetItemInternal(index);
}
