
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
/// Represents an easing function that retracts the motion of an animation slightly
/// before it begins to animate in the path indicated.
/// </summary>
public class BackEase : EasingFunctionBase
{
    /// <summary>
    /// Identifies the <see cref="Amplitude"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AmplitudeProperty =
        DependencyProperty.Register(
            nameof(Amplitude),
            typeof(double),
            typeof(BackEase),
            new PropertyMetadata(1.0));

    /// <summary>
    /// Gets or sets the amplitude of retraction associated with a <see cref="BackEase"/>
    /// animation.
    /// </summary>
    /// <returns>
    /// The amplitude of retraction associated with a <see cref="BackEase"/> animation. 
    /// This value must be greater than or equal to 0. The default is 1.
    /// </returns>
    public double Amplitude
    {
        get => (double)GetValue(AmplitudeProperty);
        set => SetValueInternal(AmplitudeProperty, value);
    }

    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        double amp = Math.Max(0.0, Amplitude);
        return Math.Pow(normalizedTime, 3.0) - normalizedTime * amp * Math.Sin(Math.PI * normalizedTime);
    }
}
