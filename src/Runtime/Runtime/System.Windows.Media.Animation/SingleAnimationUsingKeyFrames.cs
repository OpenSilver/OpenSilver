
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
/// Animates the value of a <see cref="float"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class SingleAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<float>
{
    private SingleKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleAnimationUsingKeyFrames"/> class.
    /// </summary>
    public SingleAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="SingleKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="SingleKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public SingleKeyFrameCollection KeyFrames
    {
        get => _frames ??= new SingleKeyFrameCollection(this);
        set => _frames = value;
    }

    IKeyFrameCollection<float> IKeyFrameAnimation<float>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<float>(this, isRoot, new KeyFramesAnimator<float>(this));
}

/// <summary>
/// Represents a collection of <see cref="SingleKeyFrame"/> objects.
/// </summary>
public sealed class SingleKeyFrameCollection : PresentationFrameworkCollection<SingleKeyFrame>, IKeyFrameCollection<float>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleKeyFrameCollection"/> class.
    /// </summary>
    public SingleKeyFrameCollection() { }

    internal SingleKeyFrameCollection(SingleAnimationUsingKeyFrames owner)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(SingleKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, SingleKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override SingleKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, SingleKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<float> IKeyFrameCollection<float>.this[int index] => GetItemInternal(index);
}
