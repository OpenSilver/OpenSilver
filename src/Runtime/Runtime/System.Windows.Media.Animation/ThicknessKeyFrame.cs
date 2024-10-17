
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
/// Abstract class that, when implemented, defines an animation segment with its own target value and interpolation method 
/// for a <see cref="ThicknessAnimationUsingKeyFrames"/>.
/// </summary>
public abstract class ThicknessKeyFrame : DependencyObject, IKeyFrame<Thickness>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessKeyFrame"/> class.
    /// </summary>
    protected ThicknessKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessKeyFrame"/> class that has the specified target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="ThicknessKeyFrame"/> instance.
    /// </param>
    protected ThicknessKeyFrame(Thickness value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessKeyFrame"/> class that has the specified target <see cref="Value"/>
    /// and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="ThicknessKeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="ThicknessKeyFrame"/> instance.
    /// </param>
    protected ThicknessKeyFrame(Thickness value, KeyTime keyTime)
    {
        Value = value;
        KeyTime = keyTime;
    }

    /// <summary>
    /// Identifies the <see cref="KeyTime"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty KeyTimeProperty =
        DependencyProperty.Register(
            nameof(KeyTime),
            typeof(KeyTime),
            typeof(ThicknessKeyFrame),
            new PropertyMetadata(KeyTime.Uniform));

    /// <summary>
    /// Gets or sets the time at which the key frame's target <see cref="Value"/> should be reached.
    /// </summary>
    /// <returns>
    /// The time at which the key frame's current value should be equal to its <see cref="Value"/> property.
    /// The default value is <see cref="KeyTime.Uniform"/>.
    /// </returns>
    public KeyTime KeyTime
    {
        get => (KeyTime)GetValue(KeyTimeProperty);
        set => SetValue(KeyTimeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(Thickness),
            typeof(ThicknessKeyFrame),
            new PropertyMetadata(new Thickness()));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified <see cref="KeyTime"/>.
    /// The default value is 0.
    /// </returns>
    public Thickness Value
    {
        get => (Thickness)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (Thickness)value;
    }

    /// <summary>
    /// Returns the interpolated value of a specific key frame at the progress increment provided.
    /// </summary>
    /// <param name="baseValue">
    /// The value to animate from.
    /// </param>
    /// <param name="keyFrameProgress">
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Occurs if keyFrameProgress is not between 0.0 and 1.0, inclusive.
    /// </exception>
    public Thickness InterpolateValue(Thickness baseValue, double keyFrameProgress)
    {
        if (keyFrameProgress < 0.0 || keyFrameProgress > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(keyFrameProgress));
        }

        return InterpolateValueCore(baseValue, keyFrameProgress);
    }

    /// <summary>
    /// Calculates the value of a key frame at the progress increment provided.
    /// </summary>
    /// <param name="baseValue">
    /// The value to animate from; typically the value of the previous key frame.
    /// </param>
    /// <param name="keyFrameProgress">
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    protected virtual Thickness InterpolateValueCore(Thickness baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="Thickness"/> value of the previous key frame to its own <see cref="ThicknessKeyFrame.Value"/> 
/// using discrete interpolation.
/// </summary>
public sealed class DiscreteThicknessKeyFrame : ThicknessKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteThicknessKeyFrame"/> class.
    /// </summary>
    public DiscreteThicknessKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteThicknessKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// The ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteThicknessKeyFrame(Thickness value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteThicknessKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// The ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// The key time for the key frame. The key time determines when the target value is reached, which is also when the key frame ends.
    /// </param>
    public DiscreteThicknessKeyFrame(Thickness value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override Thickness InterpolateValueCore(Thickness baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}

/// <summary>
/// Animates from the <see cref="Thickness"/> value of the previous key frame to its own <see cref="ThicknessKeyFrame.Value"/> 
/// using linear interpolation.
/// </summary>
public sealed class LinearThicknessKeyFrame : ThicknessKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearThicknessKeyFrame"/> class.
    /// </summary>
    public LinearThicknessKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearThicknessKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public LinearThicknessKeyFrame(Thickness value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearThicknessKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public LinearThicknessKeyFrame(Thickness value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override Thickness InterpolateValueCore(Thickness baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateThickness(baseValue, Value, keyFrameProgress)
        };
}

/// <summary>
/// A class that enables you to associate easing functions with a <see cref="ThicknessAnimationUsingKeyFrames"/> key frame animation.
/// </summary>
public sealed class EasingThicknessKeyFrame : ThicknessKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingThicknessKeyFrame"/> class.
    /// </summary>
    public EasingThicknessKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingThicknessKeyFrame"/> class with the specified <see cref="Thickness"/> value.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Thickness"/> value.
    /// </param>
    public EasingThicknessKeyFrame(Thickness value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingThicknessKeyFrame"/> class with the specified <see cref="Thickness"/> value and key time.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Thickness"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    public EasingThicknessKeyFrame(Thickness value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingThicknessKeyFrame"/> class with the specified <see cref="Thickness"/> value, key time, 
    /// and easing function.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Thickness"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    /// <param name="easingFunction">
    /// The easing function.
    /// </param>
    public EasingThicknessKeyFrame(Thickness value, KeyTime keyTime, IEasingFunction easingFunction)
        : base(value, keyTime)
    {
        EasingFunction = easingFunction;
    }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(EasingThicknessKeyFrame),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the easing function applied to the key frame.
    /// </summary>
    /// <returns>
    /// The easing function applied to the key frame.
    /// </returns>
    public IEasingFunction EasingFunction
    {
        get => (IEasingFunction)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc />
    protected override Thickness InterpolateValueCore(Thickness baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateThickness(baseValue, Value, keyFrameProgress),
        };
    }
}

/// <summary>
/// Animates from the <see cref="Thickness"/> value of the previous key frame to its own <see cref="ThicknessKeyFrame.Value"/> 
/// using splined interpolation.
/// </summary>
public sealed class SplineThicknessKeyFrame : ThicknessKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineThicknessKeyFrame"/> class.
    /// </summary>
    public SplineThicknessKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineThicknessKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public SplineThicknessKeyFrame(Thickness value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineThicknessKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public SplineThicknessKeyFrame(Thickness value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineThicknessKeyFrame"/> class with the specified ending value, key time, 
    /// and <see cref="Animation.KeySpline"/>.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    /// <param name="keySpline">
    /// <see cref="Animation.KeySpline"/> for the key frame. The <see cref="Animation.KeySpline"/> represents a Bezier curve which 
    /// defines animation progress of the key frame.
    /// </param>
    public SplineThicknessKeyFrame(Thickness value, KeyTime keyTime, KeySpline keySpline)
        : base(value, keyTime)
    {
        KeySpline = keySpline;
    }

    /// <summary>
    /// Identifies the <see cref="KeySpline"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty KeySplineProperty =
        DependencyProperty.Register(
            nameof(KeySpline),
            typeof(KeySpline),
            typeof(SplineThicknessKeyFrame),
            new PropertyMetadata(new KeySpline()));

    /// <summary>
    /// Gets or sets the two control points that define animation progress for this key frame.
    /// </summary>
    /// <returns>
    /// The two control points that specify the cubic Bezier curve which defines the progress of the key frame.
    /// </returns>
    public KeySpline KeySpline
    {
        get => (KeySpline)GetValue(KeySplineProperty);
        set => SetValue(KeySplineProperty, value);
    }

    /// <inheritdoc />
    protected override Thickness InterpolateValueCore(Thickness baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateThickness(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}