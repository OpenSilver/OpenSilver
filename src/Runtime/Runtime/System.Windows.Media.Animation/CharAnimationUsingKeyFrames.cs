
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
/// Animates the value of a <see cref="char"/> property along a set of <see cref="KeyFrames"/> over a 
/// specified <see cref="Timeline.Duration"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class CharAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<char>
{
    private CharKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharAnimationUsingKeyFrames"/> class.
    /// </summary>
    public CharAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="CharKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="CharKeyFrame"/> objects that define the animation. The default is an empty collection.
    /// </returns>
    public CharKeyFrameCollection KeyFrames
    {
        get => _frames ??= new CharKeyFrameCollection(this);
        set => _frames = value;
    }

    IKeyFrameCollection<char> IKeyFrameAnimation<char>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<char>(this, isRoot, new KeyFramesAnimator<char>(this));
}

/// <summary>
/// Represents a collection of <see cref="CharKeyFrame"/> objects.
/// </summary>
public sealed class CharKeyFrameCollection : PresentationFrameworkCollection<CharKeyFrame>, IKeyFrameCollection<char>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharKeyFrameCollection"/> class.
    /// </summary>
    public CharKeyFrameCollection() { }

    internal CharKeyFrameCollection(CharAnimationUsingKeyFrames owner)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(CharKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, CharKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override CharKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, CharKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<char> IKeyFrameCollection<char>.this[int index] => GetItemInternal(index);
}
