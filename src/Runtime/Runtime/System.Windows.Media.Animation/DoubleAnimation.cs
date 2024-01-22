
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
/// Animates the value of a System.Double property between two target values using
/// linear interpolation over a specified <see cref="Timeline.Duration"/>.
/// </summary>
public class DoubleAnimation : AnimationTimeline, IFromByToAnimation<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleAnimation"/> class.
    /// </summary>
    public DoubleAnimation() { }

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(double?),
            typeof(DoubleAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default
    /// is null.
    /// </returns>
    [OpenSilver.NotImplemented]
    public double? By
    {
        get => (double?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(DoubleAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the easing function applied to this animation.
    /// </summary>
    /// <returns>
    /// The easing function applied to this animation.
    /// </returns>
    public IEasingFunction EasingFunction
    {
        get => (IEasingFunction)GetValue(EasingFunctionProperty);
        set => SetValueInternal(EasingFunctionProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="From"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(double?),
            typeof(DoubleAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's starting value.
    /// </summary>
    /// <returns>
    /// The starting value of the animation. The default is null.
    /// </returns>
    public double? From
    {
        get => (double?)GetValue(FromProperty);
        set => SetValueInternal(FromProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(double?),
            typeof(DoubleAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's ending value.
    /// </summary>
    /// <returns>
    /// The ending value of the animation. The default is null.
    /// </returns>
    public double? To
    {
        get => (double?)GetValue(ToProperty);
        set => SetValueInternal(ToProperty, value);
    }

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<double>(
            this,
            isRoot,
            new FromToByAnimator<double>(this));

    double IFromByToAnimation<double>.InterpolateValue(double from, double to, double progress) =>
        AnimatedTypeHelpers.InterpolateDouble(from, to, progress);
}
