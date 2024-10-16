
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
using System.Windows.Markup;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Animates the value of a property that takes a <see cref="Rect"/> along a set of key frames.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class RectAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Rect>
{
    private RectKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimationUsingKeyFrames"/> class.
    /// </summary>
    public RectAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="RectKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="RectKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public RectKeyFrameCollection KeyFrames
    {
        get => _frames ??= new RectKeyFrameCollection(this);
        set => _frames = value;
    }

    IKeyFrameCollection<Rect> IKeyFrameAnimation<Rect>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<Rect>(this, isRoot, new KeyFramesAnimator<Rect>(this));
}

/// <summary>
/// Represents a collection of <see cref="RectKeyFrame"/> objects.
/// </summary>
public sealed class RectKeyFrameCollection : PresentationFrameworkCollection<RectKeyFrame>, IKeyFrameCollection<Rect>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RectKeyFrameCollection"/> class.
    /// </summary>
    public RectKeyFrameCollection() { }

    internal RectKeyFrameCollection(RectAnimationUsingKeyFrames owner)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(RectKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, RectKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override RectKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, RectKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Rect> IKeyFrameCollection<Rect>.this[int index] => GetItemInternal(index);
}
