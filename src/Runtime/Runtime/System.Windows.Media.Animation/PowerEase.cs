
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Represents an easing function that creates an animation that accelerates and/or
/// decelerates using the formula f(t) = tp where p is equal to the <see cref="Power"/>
/// property.
/// </summary>
public class PowerEase : EasingFunctionBase
{
    /// <summary>
    /// Identifies the <see cref="Power"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PowerProperty =
        DependencyProperty.Register(
            nameof(Power),
            typeof(double),
            typeof(PowerEase),
            new PropertyMetadata(2.0));

    /// <summary>
    /// Gets or sets the exponential power of the animation interpolation. For example,
    /// a value of 7 creates an animation interpolation curve that follows the formula
    /// f(t) = t7.
    /// </summary>
    /// <returns>
    /// The exponential power of the animation interpolation. This value must be greater
    /// or equal to 0. The default is 2.
    /// </returns>
    public double Power
    {
        get => (double)GetValue(PowerProperty);
        set => SetValueInternal(PowerProperty, value);
    }

    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        double power = Math.Max(0.0, Power);
        return Math.Pow(normalizedTime, power);
    }
}
