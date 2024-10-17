
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
/// Animates the value of a <see cref="Rect"/> property between two target values using linear interpolation.
/// </summary>
public sealed class RectAnimation : AnimationTimeline, IFromByToAnimation<Rect>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimation"/> class.
    /// </summary>
    public RectAnimation() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimation"/> class that animates to the specified value over the specified duration.
    /// The starting value for the animation is the base value of the property being animated or the output from another animation.
    /// </summary>
    /// <param name="toValue">
    /// The destination value of the animation.
    /// </param>
    /// <param name="duration">
    /// The length of time the animation takes to play from start to finish, once. See the <see cref="Timeline.Duration"/> property for more information.
    /// </param>
    public RectAnimation(Rect toValue, Duration duration)
    {
        To = toValue;
        Duration = duration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimation"/> class that animates to the specified value over the specified duration 
    /// and has the specified fill behavior. The starting value for the animation is the base value of the property being animated or the output 
    /// from another animation.
    /// </summary>
    /// <param name="toValue">
    /// The destination value of the animation.
    /// </param>
    /// <param name="duration">
    /// The length of time the animation takes to play from start to finish, once. See the <see cref="Timeline.Duration"/> property for more information.
    /// </param>
    /// <param name="fillBehavior">
    /// Specifies how the animation behaves when it is not active.
    /// </param>
    public RectAnimation(Rect toValue, Duration duration, FillBehavior fillBehavior)
    {
        To = toValue;
        Duration = duration;
        FillBehavior = fillBehavior;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimation"/> class that animates from the specified starting value to the specified 
    /// destination value over the specified duration.
    /// </summary>
    /// <param name="fromValue">
    /// The starting value of the animation.
    /// </param>
    /// <param name="toValue">
    /// The destination value of the animation.
    /// </param>
    /// <param name="duration">
    /// The length of time the animation takes to play from start to finish, once. See the <see cref="Timeline.Duration"/> property for more information.
    /// </param>
    public RectAnimation(Rect fromValue, Rect toValue, Duration duration)
    {
        From = fromValue;
        To = toValue;
        Duration = duration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectAnimation"/> class that animates from the specified starting value to the specified 
    /// destination value over the specified duration and has the specified fill behavior.
    /// </summary>
    /// <param name="fromValue">
    /// The starting value of the animation.
    /// </param>
    /// <param name="toValue">
    /// The destination value of the animation.
    /// </param>
    /// <param name="duration">
    /// The length of time the animation takes to play from start to finish, once. See the <see cref="Timeline.Duration"/> property for more information.
    /// </param>
    /// <param name="fillBehavior">
    /// Specifies how the animation behaves when it is not active.
    /// </param>
    public RectAnimation(Rect fromValue, Rect toValue, Duration duration, FillBehavior fillBehavior)
    {
        From = fromValue;
        To = toValue;
        Duration = duration;
        FillBehavior = fillBehavior;
    }

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(Rect?),
            typeof(RectAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default is null.
    /// </returns>
    [OpenSilver.NotImplemented]
    public Rect? By
    {
        get => (Rect?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(RectAnimation),
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
            typeof(Rect?),
            typeof(RectAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's starting value.
    /// </summary>
    /// <returns>
    /// The starting value of the animation. The default is null.
    /// </returns>
    public Rect? From
    {
        get => (Rect?)GetValue(FromProperty);
        set => SetValueInternal(FromProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Rect?),
            typeof(RectAnimation),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the animation's ending value.
    /// </summary>
    /// <returns>
    /// The ending value of the animation. The default is null.
    /// </returns>
    public Rect? To
    {
        get => (Rect?)GetValue(ToProperty);
        set => SetValueInternal(ToProperty, value);
    }

    internal sealed override TimelineClock CreateClock(bool isRoot) =>
        new AnimationClock<Rect>(
            this,
            isRoot,
            new FromToByAnimator<Rect>(this));

    Rect IFromByToAnimation<Rect>.InterpolateValue(Rect from, Rect to, double progress) =>
        AnimatedTypeHelpers.InterpolateRect(from, to, progress);
}
