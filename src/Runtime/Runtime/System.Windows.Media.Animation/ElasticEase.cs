
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
/// Represents an easing function that creates an animation that resembles a spring oscillating back and 
/// forth until it comes to rest.
/// </summary>
public class ElasticEase : EasingFunctionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticEase"/> class.
    /// </summary>
    public ElasticEase() { }

    /// <summary>
    /// Identifies the <see cref="Oscillations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OscillationsProperty =
        DependencyProperty.Register(
            nameof(Oscillations),
            typeof(int),
            typeof(ElasticEase),
            new PropertyMetadata(3));

    /// <summary>
    /// Gets or sets the number of times the target slides back and forth over the animation destination.
    /// </summary>
    /// <returns>
    /// The number of times the target slides back and forth over the animation destination. This value 
    /// must be greater than or equal to 0. The default is 3.
    /// </returns>
    public int Oscillations
    {
        get => (int)GetValue(OscillationsProperty);
        set => SetValueInternal(OscillationsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Springiness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpringinessProperty =
        DependencyProperty.Register(
            nameof(Springiness),
            typeof(double),
            typeof(ElasticEase),
            new PropertyMetadata(3.0));

    /// <summary>
    /// Gets or sets the stiffness of the spring. The smaller the Springiness value is, the stiffer the 
    /// spring and the faster the elasticity decreases in intensity over each oscillation.
    /// </summary>
    /// <returns>
    /// A positive number that specifies the stiffness of the spring. The default value is 3.
    /// </returns>
    public double Springiness
    {
        get => (double)GetValue(SpringinessProperty);
        set => SetValueInternal(SpringinessProperty, value);
    }

    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        double oscillations = Math.Max(0.0, Oscillations);
        double springiness = Math.Max(0.0, Springiness);
        double expo;
        if (DoubleUtil.IsZero(springiness))
        {
            expo = normalizedTime;
        }
        else
        {
            expo = (Math.Exp(springiness * normalizedTime) - 1.0) / (Math.Exp(springiness) - 1.0);
        }

        return expo * Math.Sin((Math.PI * 2.0 * oscillations + Math.PI * 0.5) * normalizedTime);
    }
}