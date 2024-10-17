
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
/// Animates the value of a <see cref="byte"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class ByteAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<byte>
{
    private ByteKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteAnimationUsingKeyFrames"/> class.
    /// </summary>
    public ByteAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets or sets the collection of <see cref="ByteKeyFrame"/> objects that define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="ByteKeyFrame"/> objects that define the animation. The default value is an empty collection.
    /// </returns>
    public ByteKeyFrameCollection KeyFrames
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

    IKeyFrameCollection<byte> IKeyFrameAnimation<byte>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<byte>(this, isRoot, new KeyFramesAnimator<byte>(this));

    private void SetKeyFrames(ByteKeyFrameCollection keyFrames)
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
/// Represents a collection of <see cref="ByteKeyFrame"/> objects.
/// </summary>
public sealed class ByteKeyFrameCollection : PresentationFrameworkCollection<ByteKeyFrame>, IKeyFrameCollection<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteKeyFrameCollection"/> class.
    /// </summary>
    public ByteKeyFrameCollection() { }

    internal override void AddOverride(ByteKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, ByteKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override ByteKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ByteKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<byte> IKeyFrameCollection<byte>.this[int index] => GetItemInternal(index);
}
