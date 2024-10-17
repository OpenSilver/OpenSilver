
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
/// Animates the value of a property that takes a <see cref="bool"/> along a set of <see cref="KeyFrames"/> over a 
/// specified <see cref="Timeline.Duration"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class BooleanAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<bool>
{
    private BooleanKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanAnimationUsingKeyFrames"/> class.
    /// </summary>
    public BooleanAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="BooleanKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="BooleanKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public BooleanKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<bool> IKeyFrameAnimation<bool>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<bool>(this, isRoot, new KeyFramesAnimator<bool>(this));

    private void SetKeyFrames(BooleanKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="BooleanKeyFrame"/> objects.
/// </summary>
public sealed class BooleanKeyFrameCollection : PresentationFrameworkCollection<BooleanKeyFrame>, IKeyFrameCollection<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanKeyFrameCollection"/> class.
    /// </summary>
    public BooleanKeyFrameCollection() { }

    internal override void AddOverride(BooleanKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, BooleanKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override BooleanKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, BooleanKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<bool> IKeyFrameCollection<bool>.this[int index] => GetItemInternal(index);
}
