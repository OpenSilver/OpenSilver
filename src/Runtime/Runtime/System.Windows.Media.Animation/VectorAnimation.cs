
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
/// Animates the value of a <see cref="Vector"/> property between two target values using linear interpolation over a specified 
/// <see cref="Timeline.Duration"/>.
/// </summary>
public sealed class VectorAnimation : AnimationTimeline, IAnimation<Vector>
{
    private AnimationType _animationType;
    private bool _isAnimationFunctionValid;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimation"/> class.
    /// </summary>
    public VectorAnimation() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimation"/> class that animates to the specified value over the specified duration.
    /// The starting value for the animation is the base value of the property being animated or the output from another animation.
    /// </summary>
    /// <param name="toValue">
    /// The destination value of the animation.
    /// </param>
    /// <param name="duration">
    /// The length of time the animation takes to play from start to finish, once. See the <see cref="Timeline.Duration"/> property for more information.
    /// </param>
    public VectorAnimation(Vector toValue, Duration duration)
    {
        To = toValue;
        Duration = duration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimation"/> class that animates to the specified value over the specified duration 
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
    public VectorAnimation(Vector toValue, Duration duration, FillBehavior fillBehavior)
    {
        To = toValue;
        Duration = duration;
        FillBehavior = fillBehavior;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimation"/> class that animates from the specified starting value to the specified 
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
    public VectorAnimation(Vector fromValue, Vector toValue, Duration duration)
    {
        From = fromValue;
        To = toValue;
        Duration = duration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorAnimation"/> class that animates from the specified starting value to the specified 
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
    public VectorAnimation(Vector fromValue, Vector toValue, Duration duration, FillBehavior fillBehavior)
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
            typeof(VectorAnimation),
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
            typeof(Vector?),
            typeof(VectorAnimation),
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

    /// <summary>
    /// Gets or sets the animation's starting value.
    /// </summary>
    /// <returns>
    /// The starting value of the animation. The default value is null.
    /// </returns>
    public Vector? From
    {
        get => (Vector?)GetValue(FromProperty);
        set => SetValueInternal(FromProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Vector?),
            typeof(VectorAnimation),
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

    /// <summary>
    /// Gets or sets the animation's ending value.
    /// </summary>
    /// <returns>
    /// The ending value of the animation. The default value is null.
    /// </returns>
    public Vector? To
    {
        get => (Vector?)GetValue(ToProperty);
        set => SetValueInternal(ToProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="By"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ByProperty =
        DependencyProperty.Register(
            nameof(By),
            typeof(Vector?),
            typeof(VectorAnimation),
            new PropertyMetadata(null, OnFromToOrByChanged),
            ValidateFromToOrByValue);

    /// <summary>
    /// Gets or sets the total amount by which the animation changes its starting value.
    /// </summary>
    /// <returns>
    /// The total amount by which the animation changes its starting value. The default value is null.
    /// </returns>
    public Vector? By
    {
        get => (Vector?)GetValue(ByProperty);
        set => SetValueInternal(ByProperty, value);
    }

    private static void OnFromToOrByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VectorAnimation)d)._isAnimationFunctionValid = false;
    }

    private static bool ValidateFromToOrByValue(object o)
    {
        var value = (Vector?)o;
        return !value.HasValue || AnimatedTypeHelpers.IsValidAnimationValueVector(value.Value);
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
    public sealed override Type TargetPropertyType => typeof(Vector);

    internal sealed override TimelineClock CreateClock() =>
        new AnimationClock<Vector>(this, new Animator<Vector>(this));

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

    Vector IAnimation<Vector>.GetCurrentValue(Vector initialValue, DependencyProperty dp, TimelineClock clock)
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

        var from = new Vector();
        var to = new Vector();
        var accumulated = new Vector();
        var foundation = new Vector();

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
                to = from + By.Value;

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

        if (validateOrigin && !AnimatedTypeHelpers.IsValidAnimationValueVector(initialValue))
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
                accumulated = (to - from) * currentRepeat;
            }
        }

        return foundation + accumulated + AnimatedTypeHelpers.InterpolateVector(from, to, progress);
    }
}
