
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
/// Abstract class that, when implemented, defines an animation segment with its own target value and interpolation method for 
/// a <see cref="Int64AnimationUsingKeyFrames"/>.
/// </summary>
public abstract class Int64KeyFrame : DependencyObject, IKeyFrame<long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int64KeyFrame"/> class.
    /// </summary>
    protected Int64KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64KeyFrame"/> class that has the specified target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="Int64KeyFrame"/> instance.
    /// </param>
    protected Int64KeyFrame(long value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64KeyFrame"/> class that has the specified target <see cref="Value"/>
    /// and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="Int64KeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="Int64KeyFrame"/> instance.
    /// </param>
    protected Int64KeyFrame(long value, KeyTime keyTime)
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
            typeof(Int64KeyFrame),
            new PropertyMetadata(KeyTime.Uniform));

    /// <summary>
    /// Gets or sets the time at which the key frame's target <see cref="Value"/> should be reached.
    /// </summary>
    /// <returns>
    /// The time at which the key frame's current value should be equal to its <see cref="Value"/> property. The default value 
    /// is <see cref="KeyTime.Uniform"/>.
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
            typeof(long),
            typeof(Int64KeyFrame),
            new PropertyMetadata(0L));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified <see cref="KeyTime"/>. The default 
    /// value is 0.
    /// </returns>
    public long Value
    {
        get => (long)GetValue(ValueProperty);
        set => SetValueInternal(ValueProperty, value);
    }

    /// <summary>
    /// The value of this key frame at the KeyTime specified.
    /// </summary>
    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (long)value;
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
    public long InterpolateValue(long baseValue, double keyFrameProgress)
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
    protected virtual long InterpolateValueCore(long baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="long"/> value of the previous key frame to its own <see cref="Int64KeyFrame.Value"/> using 
/// discrete interpolation.
/// </summary>
public sealed class DiscreteInt64KeyFrame : Int64KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt64KeyFrame"/> class.
    /// </summary>
    public DiscreteInt64KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt64KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteInt64KeyFrame(long value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt64KeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public DiscreteInt64KeyFrame(long value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override long InterpolateValueCore(long baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}

/// <summary>
/// Animates from the <see cref="long"/> value of the previous key frame to its own <see cref="Int64KeyFrame.Value"/> using 
/// linear interpolation.
/// </summary>
public sealed class LinearInt64KeyFrame : Int64KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt64KeyFrame"/> class.
    /// </summary>
    public LinearInt64KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt64KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public LinearInt64KeyFrame(long value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt64KeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public LinearInt64KeyFrame(long value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override long InterpolateValueCore(long baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt64(baseValue, Value, keyFrameProgress)
        };
}

/// <summary>
/// A class that enables you to associate easing functions with a <see cref="Int64AnimationUsingKeyFrames"/> key frame animation.
/// </summary>
public sealed class EasingInt64KeyFrame : Int64KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt64KeyFrame"/> class.
    /// </summary>
    public EasingInt64KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt64KeyFrame"/> class with the specified <see cref="long"/> value.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="long"/> value.
    /// </param>
    public EasingInt64KeyFrame(long value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt64KeyFrame"/> class with the specified <see cref="long"/> value 
    /// and key time.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="long"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    public EasingInt64KeyFrame(long value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt64KeyFrame"/> class with the specified System.Int64 value, key time, 
    /// and easing function.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="long"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    /// <param name="easingFunction">
    /// The easing function.
    /// </param>
    public EasingInt64KeyFrame(long value, KeyTime keyTime, IEasingFunction easingFunction)
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
            typeof(EasingInt64KeyFrame),
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
        set => SetValueInternal(EasingFunctionProperty, value);
    }

    /// <inheritdoc />
    protected override long InterpolateValueCore(long baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt64(baseValue, Value, keyFrameProgress),
        };
    }
}

/// <summary>
/// Animates from the <see cref="long"/> value of the previous key frame to its own <see cref="Int64KeyFrame.Value"/> using 
/// splined interpolation.
/// </summary>
public sealed class SplineInt64KeyFrame : Int64KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt64KeyFrame"/> class.
    /// </summary>
    public SplineInt64KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt64KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public SplineInt64KeyFrame(long value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt64KeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public SplineInt64KeyFrame(long value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt64KeyFrame"/> class with the specified ending value, key time, and 
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
    public SplineInt64KeyFrame(long value, KeyTime keyTime, KeySpline keySpline)
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
            typeof(SplineInt64KeyFrame),
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
    protected override long InterpolateValueCore(long baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt64(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}