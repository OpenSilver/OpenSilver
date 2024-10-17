
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
/// Animates the value of a System.String property along a set of <see cref="KeyFrames"/> over 
/// a specified <see cref="Timeline.Duration"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class StringAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<string>
{
    private StringKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringAnimationUsingKeyFrames"/> class.
    /// </summary>
    public StringAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="StringKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="StringKeyFrame"/> objects that define the animation. The default is an empty collection.
    /// </returns>
    public StringKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<string> IKeyFrameAnimation<string>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<string>(this, isRoot, new KeyFramesAnimator<string>(this));

    private void SetKeyFrames(StringKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="StringKeyFrame"/> objects.
/// </summary>
public sealed class StringKeyFrameCollection : PresentationFrameworkCollection<StringKeyFrame>, IKeyFrameCollection<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringKeyFrameCollection"/> class.
    /// </summary>
    public StringKeyFrameCollection() { }

    internal override void AddOverride(StringKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, StringKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override StringKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, StringKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<string> IKeyFrameCollection<string>.this[int index] => GetItemInternal(index);
}
