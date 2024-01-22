
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
/// An abstract class that defines an animation segment with its own target value
/// and interpolation method for a <see cref="DoubleAnimationUsingKeyFrames"/>.
/// </summary>
public abstract class DoubleKeyFrame : DependencyObject, IKeyFrame<double>
{
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
            typeof(double),
            typeof(DoubleKeyFrame),
            new PropertyMetadata(0d));

    /// <summary>
    /// Gets or sets the key frame's target value.
    /// </summary>
    /// <returns>
    /// The key frame's target value, which is the value of this key frame at its specified
    /// <see cref="KeyTime"/>. The default is 0.
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
