
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
/// Animates the value of a <see cref="double"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class DoubleAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<double>
{
    private DoubleKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleAnimationUsingKeyFrames"/> class.
    /// </summary>
    public DoubleAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets the collection of <see cref="DoubleKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="DoubleKeyFrame"/> objects that define  the animation. The 
    /// default is an empty collection.
    /// </returns>
    public DoubleKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<double> IKeyFrameAnimation<double>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<double>(this, isRoot, new KeyFramesAnimator<double>(this));

    private void SetKeyFrames(DoubleKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="DoubleKeyFrame"/> objects that can be individually accessed by index.
/// </summary>
public sealed class DoubleKeyFrameCollection : PresentationFrameworkCollection<DoubleKeyFrame>, IKeyFrameCollection<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyFrameCollection"/> class.
    /// </summary>
    public DoubleKeyFrameCollection() { }

    internal override void AddOverride(DoubleKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, DoubleKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override DoubleKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, DoubleKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<double> IKeyFrameCollection<double>.this[int index] => GetItemInternal(index);
}
