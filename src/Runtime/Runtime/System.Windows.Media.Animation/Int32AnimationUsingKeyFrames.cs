
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
/// Animates the value of a <see cref="int"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class Int32AnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<int>
{
    private Int32KeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int32KeyFrameCollection"/> class.
    /// </summary>
    public Int32AnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="Int32KeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="Int32KeyFrame"/> objects that define the animation. The default 
    /// value is an empty collection.
    /// </returns>
    public Int32KeyFrameCollection KeyFrames
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

    IKeyFrameCollection<int> IKeyFrameAnimation<int>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<int>(this, isRoot, new KeyFramesAnimator<int>(this));

    private void SetKeyFrames(Int32KeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="Int32KeyFrame"/> objects.
/// </summary>
public sealed class Int32KeyFrameCollection : PresentationFrameworkCollection<Int32KeyFrame>, IKeyFrameCollection<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int32KeyFrameCollection"/> class.
    /// </summary>
    public Int32KeyFrameCollection() { }

    internal override void AddOverride(Int32KeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, Int32KeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override Int32KeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, Int32KeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<int> IKeyFrameCollection<int>.this[int index] => GetItemInternal(index);
}