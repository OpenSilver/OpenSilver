
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
/// Animates the value of a <see cref="Point"/> property along a set of <see cref="KeyFrames"/>.
/// </summary>
[ContentProperty(nameof(KeyFrames))]
public sealed class PointAnimationUsingKeyFrames : AnimationTimeline, IKeyFrameAnimation<Point>
{
    private PointKeyFrameCollection _frames;

    /// <summary>
    /// Initializes a new instance of the <see cref="PointAnimationUsingKeyFrames"/> class.
    /// </summary>
    public PointAnimationUsingKeyFrames() { }

    /// <summary>
    /// Gets the collection of <see cref="PointKeyFrame"/> objects that
    /// define the animation.
    /// </summary>
    /// <returns>
    /// The collection of <see cref="PointKeyFrame"/> objects that define
    /// the animation. The default is an empty collection.
    /// </returns>
    public PointKeyFrameCollection KeyFrames => _frames ??= new PointKeyFrameCollection(this);

    IKeyFrameCollection<Point> IKeyFrameAnimation<Point>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Point>(this, isRoot, new KeyFramesAnimator<Point>(this));
}
