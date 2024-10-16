
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
/// Animates the value of a <see cref="Thickness"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class ThicknessAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Thickness>
{
    private ThicknessKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessAnimationUsingKeyFrames"/> class.
    /// </summary>
    public ThicknessAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="ThicknessKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="ThicknessKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public ThicknessKeyFrameCollection KeyFrames
    {
        get => _frames ??= new ThicknessKeyFrameCollection(this);
        set => _frames = value;
    }

    IKeyFrameCollection<Thickness> IKeyFrameAnimation<Thickness>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<Thickness>(this, isRoot, new KeyFramesAnimator<Thickness>(this));
}

/// <summary>
/// Represents a collection of <see cref="ThicknessKeyFrame"/> objects.
/// </summary>
public sealed class ThicknessKeyFrameCollection : PresentationFrameworkCollection<ThicknessKeyFrame>, IKeyFrameCollection<Thickness>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessKeyFrameCollection"/> class.
    /// </summary>
    public ThicknessKeyFrameCollection() { }

    internal ThicknessKeyFrameCollection(ThicknessAnimationUsingKeyFrames owner)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(ThicknessKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, ThicknessKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override ThicknessKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ThicknessKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Thickness> IKeyFrameCollection<Thickness>.this[int index] => GetItemInternal(index);
}
