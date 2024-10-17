
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
/// Animates the value of a <see cref="long"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class Int64AnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<long>
{
    private Int64KeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64AnimationUsingKeyFrames"/> class.
    /// </summary>
    public Int64AnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="Int64KeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="Int64KeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public Int64KeyFrameCollection KeyFrames
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

    IKeyFrameCollection<long> IKeyFrameAnimation<long>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<long>(this, isRoot, new KeyFramesAnimator<long>(this));

    private void SetKeyFrames(Int64KeyFrameCollection keyFrames)
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
public sealed class Int64KeyFrameCollection : PresentationFrameworkCollection<Int64KeyFrame>, IKeyFrameCollection<long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int16KeyFrameCollection"/> class.
    /// </summary>
    public Int64KeyFrameCollection() { }

    internal override void AddOverride(Int64KeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, Int64KeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override Int64KeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, Int64KeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<long> IKeyFrameCollection<long>.this[int index] => GetItemInternal(index);
}
