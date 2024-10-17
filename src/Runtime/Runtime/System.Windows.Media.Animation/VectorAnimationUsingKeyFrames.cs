
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
/// Animates the value of a <see cref="Vector"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class VectorAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Vector>
{
    private VectorKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimationUsingKeyFrames"/> class.
    /// </summary>
    public VectorAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="VectorKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="VectorKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public VectorKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<Vector> IKeyFrameAnimation<Vector>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<Vector>(this, isRoot, new KeyFramesAnimator<Vector>(this));

    private void SetKeyFrames(VectorKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="VectorKeyFrame"/> objects.
/// </summary>
public sealed class VectorKeyFrameCollection : PresentationFrameworkCollection<VectorKeyFrame>, IKeyFrameCollection<Vector>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VectorKeyFrameCollection"/> class.
    /// </summary>
    public VectorKeyFrameCollection() { }

    internal override void AddOverride(VectorKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, VectorKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override VectorKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, VectorKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Vector> IKeyFrameCollection<Vector>.this[int index] => GetItemInternal(index);
}
