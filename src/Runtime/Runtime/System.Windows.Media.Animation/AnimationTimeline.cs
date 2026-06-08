
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

using OpenSilver.Internal;

namespace System.Windows.Media.Animation;

/// <summary>
/// Defines a segment of time over which output values are produced. These values are used to animate a target property.
/// </summary>
public abstract class AnimationTimeline : Timeline
{
    internal static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Identifies the <b>IsAdditive</b> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAdditiveProperty =
        DependencyProperty.Register(
            "IsAdditive",
            typeof(bool),
            typeof(AnimationTimeline),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <b>IsCumulative</b> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsCumulativeProperty =
        DependencyProperty.Register(
            "IsCumulative",
            typeof(bool),
            typeof(AnimationTimeline),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// When overridden in a derived class, gets the <see cref="Type"/> of property that can be animated.
    /// </summary>
    /// <returns>
    /// The type of property that can be animated by this animation.
    /// </returns>
    public abstract Type TargetPropertyType { get; }

    protected override Duration GetNaturalDurationCore() => DefaultDuration;

    internal Duration NaturalDuration
    {
        get
        {
            Duration duration = GetNaturalDurationCore();
            if (duration == Duration.Automatic)
            {
                duration = DefaultDuration;
            }

            return duration;
        }
    }
}
