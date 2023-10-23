
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
        get => _frames ??= new ColorKeyFrameCollection(this);
        [EditorBrowsable(EditorBrowsableState.Never)]
        set => _frames = value;
    }

    IKeyFrameCollection<Color> IKeyFrameAnimation<Color>.KeyFrames => _frames;

    protected sealed override Duration GetNaturalDurationCore() =>
        KeyFrameAnimationHelpers.GetLargestTimeSpanKeyTime(this);

    internal override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Color>(this, isRoot, new KeyFramesAnimator<Color>(this));
}
