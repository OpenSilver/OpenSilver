
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

using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Animates from the <see cref="Color"/> value of the previous key frame
/// to its own <see cref="ColorKeyFrame.Value"/> using splined interpolation.
/// </summary>
public sealed class SplineColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineColorKeyFrame"/> class.
    /// </summary>
    public SplineColorKeyFrame() { }

    /// <summary>
    /// Identifies the <see cref="KeySpline"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty KeySplineProperty =
        DependencyProperty.Register(
            nameof(KeySpline),
            typeof(KeySpline),
            typeof(SplineColorKeyFrame),
            new PropertyMetadata(new KeySpline()));

    /// <summary>
    /// Gets or sets the two control points that define animation progress for this key frame.
    /// </summary>
    /// <returns>
    /// The two control points that specify the cubic Bezier curve which defines the progress 
    /// of the key frame.
    /// </returns>
    public KeySpline KeySpline
    {
        get => (KeySpline)GetValue(KeySplineProperty);
        set => SetValueInternal(KeySplineProperty, value);
    }

    /// <inheritdoc />
    internal override Color InterpolateValueCore(Color baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateColor(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}
