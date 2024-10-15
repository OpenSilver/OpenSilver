
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
        get => _frames ??= new Int16KeyFrameCollection(this);
        set => _frames = value;
    }

    IKeyFrameCollection<short> IKeyFrameAnimation<short>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<short>(this, isRoot, new KeyFramesAnimator<short>(this));
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

    internal Int16KeyFrameCollection(Int16AnimationUsingKeyFrames owner)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(Int16KeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, Int16KeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override Int16KeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, Int16KeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<short> IKeyFrameCollection<short>.this[int index] => GetItemInternal(index);
}
