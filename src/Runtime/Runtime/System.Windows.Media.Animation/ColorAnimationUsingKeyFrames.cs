﻿
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
/// Animates the value of a <see cref="Color"/> property along a set of <see cref="KeyFrames"/>
/// over a specified <see cref="Duration"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public class ColorAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Color>
{
    private ColorKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorAnimationUsingKeyFrames"/> class.
    /// </summary>
    public ColorAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets the collection of <see cref="ColorKeyFrame"/> objects that
    /// define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="ColorKeyFrame"/> objects that define
    /// the animation. The default is an empty collection.
    /// </returns>
    public ColorKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<Color> IKeyFrameAnimation<Color>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Color>(this, isRoot, new KeyFramesAnimator<Color>(this));

    private void SetKeyFrames(ColorKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="ColorKeyFrame" /> objects that can be individually accessed by index. 
/// </summary>
public sealed class ColorKeyFrameCollection : PresentationFrameworkCollection<ColorKeyFrame>, IKeyFrameCollection<Color>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorKeyFrameCollection"/> class.
    /// </summary>
    public ColorKeyFrameCollection() { }

    internal override void AddOverride(ColorKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, ColorKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override ColorKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ColorKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<Color> IKeyFrameCollection<Color>.this[int index] => GetItemInternal(index);
}
