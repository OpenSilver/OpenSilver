
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
/// Represents an easing function that creates an animated bouncing effect.
/// </summary>
public class BounceEase : EasingFunctionBase
{
    /// <summary>
    /// Identifies the <see cref="Bounces"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BouncesProperty =
        DependencyProperty.Register(
            nameof(Bounces),
            typeof(int),
            typeof(BounceEase),
            new PropertyMetadata(3));

    /// <summary>
    /// Gets or sets the number of bounces.
    /// </summary>
    /// <returns>
    /// The number of bounces. The value must be greater or equal to zero. Negative values
    /// will resolve to zero. The default is 3.
    /// </returns>
    public int Bounces
    {
        get => (int)GetValue(BouncesProperty);
        set => SetValueInternal(BouncesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Bounciness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BouncinessProperty =
        DependencyProperty.Register(
            nameof(Bounciness),
            typeof(double),
            typeof(BounceEase),
            new PropertyMetadata(2d));

    /// <summary>
    /// Gets or sets a value that specifies how bouncy the bounce animation is. Low values
    /// of this property result in bounces with little lose of height between bounces
    /// (more bouncy) while high values result in dampened bounces (less bouncy).
    /// </summary>
    /// <returns>
    /// The value that specifies how bouncy the bounce animation is. This value must
    /// be positive. The default value is 2.
    /// </returns>
    public double Bounciness
    {
        get => (double)GetValue(BouncinessProperty);
        set => SetValueInternal(BouncinessProperty, value);
    }

    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        // The math below is complicated because we have a few requirements to get the correct look for bounce:
        //  1) The bounces should be symetrical
        //  2) Bounciness should control both the amplitude and the period of the bounces
        //  3) Bounces should control the number of bounces without including the final half bounce to get you back to 1.0
        //
        //  Note: Simply modulating a expo or power curve with a abs(sin(...)) wont work because it violates 1) above.
        //

        // Constants
        double bounces = Math.Max(0.0, (double)Bounces);
        double bounciness = Bounciness;

        // Clamp the bounciness so we dont hit a divide by zero
        if (bounciness < 1.0 || DoubleUtil.IsOne(bounciness))
        {
            // Make it just over one.  In practice, this will look like 1.0 but avoid divide by zeros.
            bounciness = 1.001;
        }

        double pow = Math.Pow(bounciness, bounces);
        double oneMinusBounciness = 1.0 - bounciness;

        // 'unit' space calculations.
        // Our bounces grow in the x axis exponentially.  we define the first bounce as having a 'unit' width of 1.0 and compute
        // the total number of 'units' using a geometric series.
        // We then compute which 'unit' the current time is in.
        double sumOfUnits = (1.0 - pow) / oneMinusBounciness + pow * 0.5; // geometric series with only half the last sum
        double unitAtT = normalizedTime * sumOfUnits;

        // 'bounce' space calculations.
        // Now that we know which 'unit' the current time is in, we can determine which bounce we're in by solving the geometric equation:
        // unitAtT = (1 - bounciness^bounce) / (1 - bounciness), for bounce.
        double bounceAtT = Math.Log(-unitAtT * (1.0 - bounciness) + 1.0, bounciness);
        double start = Math.Floor(bounceAtT);
        double end = start + 1.0;

        // 'time' space calculations.
        // We then project the start and end of the bounce into 'time' space
        double startTime = (1.0 - Math.Pow(bounciness, start)) / (oneMinusBounciness * sumOfUnits);
        double endTime = (1.0 - Math.Pow(bounciness, end)) / (oneMinusBounciness * sumOfUnits);

        // Curve fitting for bounce.
        double midTime = (startTime + endTime) * 0.5;
        double timeRelativeToPeak = normalizedTime - midTime;
        double radius = midTime - startTime;
        double amplitude = Math.Pow(1.0 / bounciness, (bounces - start));

        // Evaluate a quadratic that hits (startTime,0), (endTime, 0), and peaks at amplitude.
        return (-amplitude / (radius * radius)) * (timeRelativeToPeak - radius) * (timeRelativeToPeak + radius);
    }
}
