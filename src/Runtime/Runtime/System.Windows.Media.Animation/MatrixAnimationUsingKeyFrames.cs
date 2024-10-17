
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
/// Animates the value of a <see cref="Matrix"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class MatrixAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Matrix>
{
    private MatrixKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixAnimationUsingKeyFrames"/> class.
    /// </summary>
    public MatrixAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="MatrixKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="MatrixKeyFrame"/> objects that define the animation. The default is an empty collection.
    /// </returns>
    public MatrixKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<Matrix> IKeyFrameAnimation<Matrix>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<Matrix>(this, isRoot, new KeyFramesAnimator<Matrix>(this));

    private void SetKeyFrames(MatrixKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="MatrixKeyFrame"/> objects.
/// </summary>
public sealed class MatrixKeyFrameCollection : PresentationFrameworkCollection<MatrixKeyFrame>, IKeyFrameCollection<Matrix>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixKeyFrameCollection"/> class.
    /// </summary>
    public MatrixKeyFrameCollection() { }

    internal override void AddOverride(MatrixKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, MatrixKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override MatrixKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, MatrixKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Matrix> IKeyFrameCollection<Matrix>.this[int index] => GetItemInternal(index);
}
