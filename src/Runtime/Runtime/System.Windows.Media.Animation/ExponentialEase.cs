
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
/// Represents an easing function that creates an animation that accelerates and/or
/// decelerates using an exponential formula.
/// </summary>
public sealed class ExponentialEase : EasingFunctionBase
{
    /// <summary>
    /// Identifies the <see cref="Exponent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExponentProperty =
        DependencyProperty.Register(
            nameof(Exponent),
            typeof(double),
            typeof(ExponentialEase),
            new PropertyMetadata(2d));

    /// <summary>
    /// Gets or sets the exponent used to determine the interpolation of the animation.
    /// </summary>
    /// <returns>
    /// The exponent used to determine the interpolation of the animation. The default
    /// is 2.
    /// </returns>
    public double Exponent
    {
        get => (double)GetValue(ExponentProperty);
        set => SetValueInternal(ExponentProperty, value);
    }

    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        double factor = Exponent;
        if (DoubleUtil.IsZero(factor))
        {
            return normalizedTime;
        }
        else
        {
            return (Math.Exp(factor * normalizedTime) - 1.0) / (Math.Exp(factor) - 1.0);
        }
    }
}