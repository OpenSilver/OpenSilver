
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
using OpenSilver.Internal.Media.Animation;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Media.Animation;

/// <summary>
/// Animates the value of a <see cref="Rect"/> property between two target values using linear interpolation.
/// </summary>
public sealed class RectAnimation : AnimationTimeline, IAnimation<Rect>
{
    private AnimationType _animationType;
    private bool _isAnimationFunctionValid;

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
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

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
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

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

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(Rect?),
            typeof(RectAnimation),
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default is null.
    /// </returns>
    public Rect? By
    {
        get => (Rect?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    private static void OnFromToOrByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RectAnimation)d)._isAnimationFunctionValid = false;
    }

    private static bool ValidateFromToOrByValue(object o)
    {
        var value = (Rect?)o;
        return !value.HasValue || AnimatedTypeHelpers.IsValidAnimationValueRect(value.Value);
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the animation's value accumulates when it repeats.
    /// </summary>
    /// <returns>
    /// true if the animation accumulates its values when its <see cref="Timeline.RepeatBehavior"/>property causes it 
    /// to repeat its simple duration. otherwise, false. The default value is false.
    /// </returns>
    public bool IsCumulative
    {
        get => (bool)GetValue(IsCumulativeProperty);
        set => SetValueInternal(IsCumulativeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the target property's current value should be added to this 
    /// animation's starting value.
    /// </summary>
    /// <returns>
    /// true if the target property's current value should be added to this animation's starting value; otherwise,
    /// false. The default value is false.
    /// </returns>
    public bool IsAdditive
    {
        get => (bool)GetValue(IsAdditiveProperty);
        set => SetValueInternal(IsAdditiveProperty, value);
    }

    /// <inheritdoc />
    public sealed override Type TargetPropertyType => typeof(Rect);

    internal sealed override TimelineClock CreateClock() =>
        new AnimationClock<Rect>(this, new Animator<Rect>(this));

    private void ValidateAnimationFunction()
    {
        _animationType = AnimationType.Automatic;

        if (From.HasValue)
        {
            if (To.HasValue)
            {
                _animationType = AnimationType.FromTo;
            }
            else if (By.HasValue)
            {
                _animationType = AnimationType.FromBy;
            }
            else
            {
                _animationType = AnimationType.From;
            }
        }
        else if (To.HasValue)
        {
            _animationType = AnimationType.To;
        }
        else if (By.HasValue)
        {
            _animationType = AnimationType.By;
        }

        _isAnimationFunctionValid = true;
    }

    Rect IAnimation<Rect>.GetCurrentValue(Rect initialValue, DependencyProperty dp, TimelineClock clock)
    {
        Debug.Assert(clock.CurrentState != ClockState.Stopped);

        if (!_isAnimationFunctionValid)
        {
            ValidateAnimationFunction();
        }

        double progress = clock.CurrentProgress.Value;

        if (EasingFunction is IEasingFunction easingFunction)
        {
            progress = easingFunction.Ease(progress);
        }

        var from = new Rect();
        var to = new Rect();
        var accumulated = new Rect();
        var foundation = new Rect();

        // need to validate the default origin value if the animation uses
        // it as the from, to, or foundation values
        bool validateOrigin = false;

        switch (_animationType)
        {
            case AnimationType.Automatic:

                from = initialValue;
                to = initialValue;

                validateOrigin = true;

                break;

            case AnimationType.From:

                from = From.Value;
                to = initialValue;

                validateOrigin = true;

                break;

            case AnimationType.To:

                from = initialValue;
                to = To.Value;

                validateOrigin = true;

                break;

            case AnimationType.By:

                to = By.Value;
                foundation = initialValue;

                validateOrigin = true;

                break;

            case AnimationType.FromTo:

                from = From.Value;
                to = To.Value;

                if (IsAdditive)
                {
                    foundation = initialValue;
                    validateOrigin = true;
                }

                break;

            case AnimationType.FromBy:

                from = From.Value;
                to = AnimatedTypeHelpers.AddRect(from, By.Value);

                if (IsAdditive)
                {
                    foundation = initialValue;
                    validateOrigin = true;
                }

                break;

            default:

                Debug.Fail("Unknown animation type.");
                break;
        }

        if (validateOrigin && !AnimatedTypeHelpers.IsValidAnimationValueRect(initialValue))
        {
            throw new InvalidOperationException(
                string.Format(
                    Strings.Animation_Invalid_DefaultValue,
                    GetType(),
                    "origin",
                    initialValue.ToString(CultureInfo.InvariantCulture)));
        }

        if (IsCumulative)
        {
            double currentRepeat = (double)(clock.CurrentIteration - 1);

            if (currentRepeat > 0.0)
            {
                Rect accumulator = AnimatedTypeHelpers.SubtractRect(to, from);

                accumulated = AnimatedTypeHelpers.ScaleRect(accumulator, currentRepeat);
            }
        }

        return AnimatedTypeHelpers.AddRect(
            foundation,
            AnimatedTypeHelpers.AddRect(
                accumulated,
                AnimatedTypeHelpers.InterpolateRect(from, to, progress)));
    }
}
