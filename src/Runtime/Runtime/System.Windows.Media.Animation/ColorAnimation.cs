
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
/// Animates the value of a <see cref="Color"/> property between two target
/// values using linear interpolation over a specified <see cref="Timeline.Duration"/>.
/// </summary>
public sealed class ColorAnimation : AnimationTimeline, IFromByToAnimation<Color>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorAnimation"/> class.
    /// </summary>
    public ColorAnimation() { }

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(Color?),
            typeof(ColorAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default
    /// is null.
    /// </returns>
    [OpenSilver.NotImplemented]
    public Color? By
    {
        get => (Color?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(ColorAnimation),
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
    /// Identifies the <see cref="From"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(Color?),
            typeof(ColorAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's starting value.
    /// </summary>
    /// <returns>
    /// The starting value of the animation. The default is null.
    /// </returns>
    public Color? From
    {
        get => (Color?)GetValue(FromProperty);
        set => SetValueInternal(FromProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Color?),
            typeof(ColorAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's ending value.
    /// </summary>
    /// <returns>
    /// The ending value of the animation. The default is null.
    /// </returns>
    public Color? To
    {
        get => (Color?)GetValue(ToProperty);
        set => SetValueInternal(ToProperty, value);
    }

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Color>(
            this,
            isRoot,
            new FromToByAnimator<Color>(this));

    Color IFromByToAnimation<Color>.InterpolateValue(Color from, Color to, double progress) =>
        AnimatedTypeHelpers.InterpolateColor(from, to, progress);
}
