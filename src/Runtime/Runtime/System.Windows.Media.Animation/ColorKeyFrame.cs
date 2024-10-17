
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
/// Provides a base class for specific animation key-frame techniques that define an animation segment with a 
/// <see cref="Color"/> target value. Derived classes each provide a different key-frame interpolation method 
/// for a <see cref="Color"/> value that is provided for a <see cref="ColorAnimationUsingKeyFrames"/> animation.
/// </summary>
public abstract class ColorKeyFrame : DependencyObject, IKeyFrame<Color>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorKeyFrame"/> class.
    /// </summary>
    protected ColorKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorKeyFrame"/> class that has the specified target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="ColorKeyFrame"/> instance.
    /// </param>
    protected ColorKeyFrame(Color value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorKeyFrame"/> class that has the specified target <see cref="Value"/>
    /// and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="ColorKeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="ColorKeyFrame"/> instance.
    /// </param>
    protected ColorKeyFrame(Color value, KeyTime keyTime)
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
            typeof(ColorKeyFrame),
            new PropertyMetadata(KeyTime.Uniform));

    /// <summary>
    /// Gets or sets the time at which the key frame's target <see cref="Value"/>.
    /// </summary>
    /// <returns>
    /// The time at which the key frame's current value should be equal to its <see cref="Value"/>
    /// property. The default is null.
    /// </returns>
    public KeyTime KeyTime
    {
        get => (KeyTime)GetValue(KeyTimeProperty);
        set => SetValueInternal(KeyTimeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(Color),
            typeof(ColorKeyFrame),
            null);

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value at its specified <see cref="KeyTime"/>.
    /// The default is a <see cref="Color"/> with an ARGB value of #00000000.
    /// </returns>
    public Color Value
    {
        get => (Color)GetValue(ValueProperty);
        set => SetValueInternal(ValueProperty, value);
    }

    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (Color)value;
    }

    /// <summary>
    /// Returns the interpolated value of a specific key frame at the progress increment provided.
    /// </summary>
    /// <param name="baseValue">
    /// The value to animate from; typically the value of the previous key frame.
    /// </param>
    /// <param name="keyFrameProgress">
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that 
    /// has elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Occurs if keyFrameProgress is not between 0.0 and 1.0, inclusive.
    /// </exception>
    public Color InterpolateValue(Color baseValue, double keyFrameProgress)
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
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of 
    /// time that has elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    protected virtual Color InterpolateValueCore(Color baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="Color"/> value of the previous key frame to its own <see cref="ColorKeyFrame.Value"/> 
/// using discrete values.
/// </summary>
public sealed class DiscreteColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteColorKeyFrame"/> class.
    /// </summary>
    public DiscreteColorKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteColorKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteColorKeyFrame(Color value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteColorKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public DiscreteColorKeyFrame(Color value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override Color InterpolateValueCore(Color baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}

/// <summary>
/// Animates from the <see cref="Color"/> value of the previous key frame to its own <see cref="ColorKeyFrame.Value"/> 
/// using linear interpolation.
/// </summary>
public sealed class LinearColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearColorKeyFrame"/> class.
    /// </summary>
    public LinearColorKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearColorKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public LinearColorKeyFrame(Color value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearColorKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public LinearColorKeyFrame(Color value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override Color InterpolateValueCore(Color baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateColor(baseValue, Value, keyFrameProgress)
        };
}

/// <summary>
/// A class that enables you to associate easing functions with a <see cref="ColorAnimationUsingKeyFrames"/> key frame animation.
/// </summary>
public sealed class EasingColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class.
    /// </summary>
    public EasingColorKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class with the specified <see cref="Color"/>.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Color"/>.
    /// </param>
    public EasingColorKeyFrame(Color value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class with the specified <see cref="Color"/> and key time.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Color"/>.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    public EasingColorKeyFrame(Color value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class with the specified <see cref="Color"/>, key time, and 
    /// easing function.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="Color"/>.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    /// <param name="easingFunction">
    /// The easing function.
    /// </param>
    public EasingColorKeyFrame(Color value, KeyTime keyTime, IEasingFunction easingFunction)
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
            typeof(EasingColorKeyFrame),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the easing function that is applied to the key frame.
    /// </summary>
    /// <returns>
    /// The easing function that is applied to the key frame.
    /// </returns>
    public IEasingFunction EasingFunction
    {
        get => (IEasingFunction)GetValue(EasingFunctionProperty);
        set => SetValueInternal(EasingFunctionProperty, value);
    }

    /// <inheritdoc />
    protected override Color InterpolateValueCore(Color baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateColor(baseValue, Value, keyFrameProgress)
        };
    }
}

/// <summary>
/// Animates from the <see cref="Color"/> value of the previous key frame to its own <see cref="ColorKeyFrame.Value"/> 
/// using splined interpolation.
/// </summary>
public sealed class SplineColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineColorKeyFrame"/> class.
    /// </summary>
    public SplineColorKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineColorKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public SplineColorKeyFrame(Color value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineColorKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public SplineColorKeyFrame(Color value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineColorKeyFrame"/> class with the specified ending value, key time, and 
    /// <see cref="Animation.KeySpline"/>.
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
    public SplineColorKeyFrame(Color value, KeyTime keyTime, KeySpline keySpline)
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
            typeof(SplineColorKeyFrame),
            new PropertyMetadata(new KeySpline()));

    /// <summary>
    /// Gets or sets the two control points that define animation progress for this key frame.
    /// </summary>
    /// <returns>
    /// The two control points that specify the cubic Bezier curve which defines the progress 
    /// of the key frame.
    /// </returns>
    public KeySpline KeySpline
    {
        get => (KeySpline)GetValue(KeySplineProperty);
        set => SetValueInternal(KeySplineProperty, value);
    }

    /// <inheritdoc />
    protected override Color InterpolateValueCore(Color baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateColor(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}
