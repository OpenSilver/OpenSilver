
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
/// Abstract class that, when implemented, defines an animation segment with its own target value 
/// and interpolation method for a <see cref="Int32AnimationUsingKeyFrames"/>.
/// </summary>
public abstract class Int32KeyFrame : DependencyObject, IKeyFrame<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int32KeyFrame"/> class.
    /// </summary>
    protected Int32KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int32KeyFrame"/> class that has the specified 
    /// target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="Int32KeyFrame"/> instance.
    /// </param>
    protected Int32KeyFrame(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int32KeyFrame"/> class that has the specified 
    /// target <see cref="Value"/> and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="Int32KeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="Int32KeyFrame"/> instance.
    /// </param>
    protected Int32KeyFrame(int value, KeyTime keyTime)
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
            typeof(Int32KeyFrame),
            new PropertyMetadata(KeyTime.Uniform));

    /// <summary>
    /// Gets or sets the time at which the key frame's target <see cref="Value"/> should be reached.
    /// </summary>
    /// <returns>
    /// The time at which the key frame's current value should be equal to its <see cref="Value"/>
    /// property. The default value is <see cref="KeyTime.Uniform"/>.
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
            typeof(int),
            typeof(Int32KeyFrame),
            new PropertyMetadata(0));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified
    /// <see cref="KeyTime"/>. The default value is 0.
    /// </returns>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// The value of this key frame at the KeyTime specified.
    /// </summary>
    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (int)value;
    }

    /// <summary>
    /// Returns the interpolated value of a specific key frame at the progress increment provided.
    /// </summary>
    /// <param name="baseValue">
    /// The value to animate from.
    /// </param>
    /// <param name="keyFrameProgress">
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has elapsed 
    /// for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Occurs if keyFrameProgress is not between 0.0 and 1.0, inclusive.
    /// </exception>
    int IKeyFrame<int>.InterpolateValue(int baseValue, double keyFrameProgress)
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
    /// A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has elapsed
    /// for this key frame.
    /// </param>
    /// <returns>
    /// The output value of this key frame given the specified base value and progress.
    /// </returns>
    internal virtual int InterpolateValueCore(int baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="int"/> value of the previous key frame to its own <see cref="Int32KeyFrame.Value"/>
/// using discrete interpolation.
/// </summary>
public sealed class DiscreteInt32KeyFrame : Int32KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt32KeyFrame"/> class.
    /// </summary>
    public DiscreteInt32KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt32KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteInt32KeyFrame(int value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteInt32KeyFrame"/> class with the specified ending value 
    /// and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the 
    /// key frame ends.
    /// </param>
    public DiscreteInt32KeyFrame(int value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    internal override int InterpolateValueCore(int baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}

/// <summary>
/// Animates from the <see cref="int"/> value of the previous key frame to its own <see cref="Int32KeyFrame.Value"/> using 
/// linear interpolation.
/// </summary>
public sealed class LinearInt32KeyFrame : Int32KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt32KeyFrame"/> class.
    /// </summary>
    public LinearInt32KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt32KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public LinearInt32KeyFrame(int value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearInt32KeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public LinearInt32KeyFrame(int value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    internal override int InterpolateValueCore(int baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt32(baseValue, Value, keyFrameProgress)
        };
}

/// <summary>
/// A class that enables you to associate easing functions with a <see cref="Int32AnimationUsingKeyFrames"/> key frame animation.
/// </summary>
public sealed class EasingInt32KeyFrame : Int32KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt32KeyFrame"/> class.
    /// </summary>
    public EasingInt32KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt32KeyFrame"/> class with the specified <see cref="int"/> value.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="int"/> value.
    /// </param>
    public EasingInt32KeyFrame(int value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt32KeyFrame"/> class with the specified <see cref="int"/> value 
    /// and key time.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="int"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    public EasingInt32KeyFrame(int value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EasingInt32KeyFrame"/> class with the specified <see cref="int"/> value, 
    /// key time, and easing function.
    /// </summary>
    /// <param name="value">
    /// The initial <see cref="int"/> value.
    /// </param>
    /// <param name="keyTime">
    /// The initial key time.
    /// </param>
    /// <param name="easingFunction">
    /// The easing function.
    /// </param>
    public EasingInt32KeyFrame(int value, KeyTime keyTime, IEasingFunction easingFunction)
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
            typeof(EasingInt32KeyFrame),
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
    internal override int InterpolateValueCore(int baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt32(baseValue, Value, keyFrameProgress),
        };
    }
}

/// <summary>
/// Animates from the <see cref="int"/> value of the previous key frame to its own <see cref="Int32KeyFrame.Value"/> using splined 
/// interpolation.
/// </summary>
public sealed class SplineInt32KeyFrame : Int32KeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt32KeyFrame"/> class.
    /// </summary>
    public SplineInt32KeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt32KeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public SplineInt32KeyFrame(int value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt32KeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public SplineInt32KeyFrame(int value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplineInt32KeyFrame"/> class with the specified ending value, key time, 
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
    public SplineInt32KeyFrame(int value, KeyTime keyTime, KeySpline keySpline)
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
            typeof(SplineInt32KeyFrame),
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
    internal override int InterpolateValueCore(int baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateInt32(baseValue, Value, KeySpline.GetSplineProgress(KeySpline, keyFrameProgress))
        };
}
