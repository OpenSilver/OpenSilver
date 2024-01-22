
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
/// Provides a base class for specific animation key-frame techniques that define
/// an animation segment with a <see cref="Color"/> target value. Derived
/// classes each provide a different key-frame interpolation method for a <see cref="Color"/>
/// value that is provided for a <see cref="ColorAnimationUsingKeyFrames"/> animation.
/// </summary>
public abstract class ColorKeyFrame : DependencyObject, IKeyFrame<Color>
{
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
    Color IKeyFrame<Color>.InterpolateValue(Color baseValue, double keyFrameProgress)
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
    internal virtual Color InterpolateValueCore(Color baseValue, double keyFrameProgress) => baseValue;
}
