﻿
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Abstract class that, when implemented, defines a segment with its own target value and interpolation method 
/// for a <see cref="BooleanAnimationUsingKeyFrames"/>.
/// </summary>
public abstract class BooleanKeyFrame : DependencyObject, IKeyFrame<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanKeyFrame"/> class.
    /// </summary>
    protected BooleanKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanKeyFrame"/> class that has the specified target <see cref="Value"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="BooleanKeyFrame"/> instance.
    /// </param>
    protected BooleanKeyFrame(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanKeyFrame"/> class that has the specified target <see cref="Value"/>
    /// and <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> of the new <see cref="BooleanKeyFrame"/> instance.
    /// </param>
    /// <param name="keyTime">
    /// The <see cref="KeyTime"/> of the new <see cref="BooleanKeyFrame"/> instance.
    /// </param>
    protected BooleanKeyFrame(bool value, KeyTime keyTime)
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
            typeof(BooleanKeyFrame),
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
        set => SetValueInternal(KeyTimeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(bool),
            typeof(BooleanKeyFrame),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified <see cref="KeyTime"/>.
    /// The default value is false.
    /// </returns>
    public bool Value
    {
        get => (bool)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    object IKeyFrame.Value
    {
        get => Value;
        set => Value = (bool)value;
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
    public bool InterpolateValue(bool baseValue, double keyFrameProgress)
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
    protected virtual bool InterpolateValueCore(bool baseValue, double keyFrameProgress) => baseValue;
}

/// <summary>
/// Animates from the <see cref="bool"/> value of the previous key frame to its own <see cref="BooleanKeyFrame.Value"/>
/// using discrete interpolation.
/// </summary>
public sealed class DiscreteBooleanKeyFrame : BooleanKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteBooleanKeyFrame"/> class.
    /// </summary>
    public DiscreteBooleanKeyFrame() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteBooleanKeyFrame"/> class with the specified ending value.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    public DiscreteBooleanKeyFrame(bool value)
        : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteBooleanKeyFrame"/> class with the specified ending value and key time.
    /// </summary>
    /// <param name="value">
    /// Ending value (also known as "target value") for the key frame.
    /// </param>
    /// <param name="keyTime">
    /// Key time for the key frame. The key time determines when the target value is reached which is also when the key frame ends.
    /// </param>
    public DiscreteBooleanKeyFrame(bool value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }

    /// <inheritdoc />
    protected override bool InterpolateValueCore(bool baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}
