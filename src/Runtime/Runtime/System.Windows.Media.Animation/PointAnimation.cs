
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
/// Animates the value of a <see cref="Point"/> property between two target values
/// using linear interpolation over a specified <see cref="Timeline.Duration"/>.
/// </summary>
public sealed class PointAnimation : AnimationTimeline, IFromByToAnimation<Point>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointAnimation"/> class.
    /// </summary>
    public PointAnimation() { }

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(Point?),
            typeof(PointAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default
    /// is null.
    /// </returns>
    public Point? By
    {
        get => (Point?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(PointAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the easing function you are applying to the animation.
    /// </summary>
    /// <returns>
    /// The easing function you are applying to the animation. The default is null.
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
            typeof(Point?),
            typeof(PointAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's starting value.
    /// </summary>
    /// <returns>
    /// The starting value of the animation. The default is null.
    /// </returns>
    public Point? From
    {
        get => (Point?)GetValue(FromProperty);
        set => SetValueInternal(FromProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Point?),
            typeof(PointAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's ending value.
    /// </summary>
    /// <returns>
    /// The ending value of the animation. The default is null.
    /// </returns>
    public Point? To
    {
        get => (Point?)GetValue(ToProperty);
        set => SetValueInternal(ToProperty, value);
    }

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Point>(
            this,
            isRoot,
            new FromToByAnimator<Point>(this));

    Point IFromByToAnimation<Point>.InterpolateValue(Point from, Point to, double progress) =>
        AnimatedTypeHelpers.InterpolatePoint(from, to, progress);
}
