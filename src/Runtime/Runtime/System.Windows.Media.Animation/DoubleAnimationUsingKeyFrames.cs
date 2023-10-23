
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

using System.ComponentModel;
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
        get => _frames ??= new DoubleKeyFrameCollection(this);
        [EditorBrowsable(EditorBrowsableState.Never)]
        set => _frames = value;
    }

    IKeyFrameCollection<double> IKeyFrameAnimation<double>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
       new AnimationClock<double>(this, isRoot, new KeyFramesAnimator<double>(this));
}
