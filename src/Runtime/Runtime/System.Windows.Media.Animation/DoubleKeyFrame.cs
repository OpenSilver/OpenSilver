
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
/// An abstract class that defines an animation segment with its own target value and interpolation 
/// method for a <see cref="DoubleAnimationUsingKeyFrames"/>.
/// </summary>
public abstract class DoubleKeyFrame : DependencyObject, IKeyFrame<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyFrame"/> class.
    /// </summary>
    protected DoubleKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyFrame"/> class that has the specified target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="DoubleKeyFrame"/> instance.
    /// </param>
    protected DoubleKeyFrame(double value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyFrame"/> class that has the specified target <see cref="Value"/> 
    /// and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="DoubleKeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="DoubleKeyFrame"/> instance.
    /// </param>
    protected DoubleKeyFrame(double value, KeyTime keyTime)
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
            typeof(DoubleKeyFrame),
            new PropertyMetadata(KeyTime.Uniform));

    /// <summary>
    /// Gets or sets the time at which the key frame's target <see cref="Value"/> should be reached.
    /// </summary>
    /// <returns>
    /// The time at which the key frame's current value should be equal to its <see cref="Value"/> property.
    /// The default is null.
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
            typeof(double),
            typeof(DoubleKeyFrame),
            new PropertyMetadata(0d));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified <see cref="KeyTime"/>.
    /// The default is 0.
    /// </returns>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValueInternal(ValueProperty, value);
    }

    /// <summary>
    /// The value of this key frame at the KeyTime specified.
    /// </summary>
    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (double)value;
    }

    /// <summary>
    /// Returns the interpolated value of a specific key frame at the progress increment provided.
    /// </summary>
    /// <param name="baseValue">
    /// The value to animate from.
    /// </param>
    /// <param name="keyFrameProgress">
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has 
    /// elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Occurs if keyFrameProgress is not between 0.0 and 1.0, inclusive.
    /// </exception>
    double IKeyFrame<double>.InterpolateValue(double baseValue, double keyFrameProgress)
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
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time 
    /// that has elapsed for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    internal virtual double InterpolateValueCore(double baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="double"/> value of the previous key frame to its own <see cref="DoubleKeyFrame.Value"/> 
/// using discrete values.
/// </summary>
public sealed class DiscreteDoubleKeyFrame : DoubleKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteDoubleKeyFrame"/> class.
    /// </summary>
    public DiscreteDoubleKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteDoubleKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteDoubleKeyFrame(double value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteDoubleKeyFrame"/> class with the specified ending value 
    /// and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the 
    /// key frame ends.
    /// </param>
    public DiscreteDoubleKeyFrame(double value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    internal override double InterpolateValueCore(double baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}

/// <summary>
/// Animates from the <see cref="double"/> value of the previous key frame to its own <see cref="DoubleKeyFrame.Value"/> 
/// using linear interpolation.
/// </summary>
public sealed class LinearDoubleKeyFrame : DoubleKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearDoubleKeyFrame"/> class.
    /// </summary>
    public LinearDoubleKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearDoubleKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public LinearDoubleKeyFrame(double value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearDoubleKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public LinearDoubleKeyFrame(double value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    internal override double InterpolateValueCore(double baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateDouble(baseValue, Value, keyFrameProgress)
        };
}

/// <summary>
/// Defines a property that enables you to associate an easing function with a <see cref="DoubleAnimationUsingKeyFrames"/> 
/// key-frame animation.
/// </summary>
public sealed class EasingDoubleKeyFrame : DoubleKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingDoubleKeyFrame"/> class.
    /// </summary>
    public EasingDoubleKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingDoubleKeyFrame"/> class with the specified <see cref="double"/> value.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="double"/> value.
    /// </param>
    public EasingDoubleKeyFrame(double value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingDoubleKeyFrame"/> class with the specified <see cref="double"/> value 
    /// and key time.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="double"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    public EasingDoubleKeyFrame(double value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingDoubleKeyFrame"/> class with the specified <see cref="double"/> value, 
    /// key time, and easing function.
    /// </summary>
    /// <param name="value">
    /// The initial System.Double value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    /// <param name="easingFunction">
    /// The easing function.
    /// </param>
    public EasingDoubleKeyFrame(double value, KeyTime keyTime, IEasingFunction easingFunction)
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
            typeof(EasingDoubleKeyFrame),
            null);

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
    internal override double InterpolateValueCore(double baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateDouble(baseValue, Value, keyFrameProgress)
        };
    }
}

/// <summary>
/// Animates from the <see cref="double"/> value of the previous key frame to its own <see cref="DoubleKeyFrame.Value"/> 
/// using splined interpolation.
/// </summary>
public sealed class SplineDoubleKeyFrame : DoubleKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineDoubleKeyFrame"/> class.
    /// </summary>
    public SplineDoubleKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineDoubleKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public SplineDoubleKeyFrame(double value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineDoubleKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public SplineDoubleKeyFrame(double value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineDoubleKeyFrame"/> class with the specified ending value, key time, 
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
    public SplineDoubleKeyFrame(double value, KeyTime keyTime, KeySpline keySpline)
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
            typeof(SplineDoubleKeyFrame),
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
        set => SetValueInternal(KeySplineProperty, value);
    }

    /// <inheritdoc />
    internal override double InterpolateValueCore(double baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateDouble(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}
