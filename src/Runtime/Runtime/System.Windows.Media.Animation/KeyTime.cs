
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Specifies when a particular key frame should take place during an animation.
/// </summary>
public readonly struct KeyTime
{
    private readonly TimeSpan _value;

    private KeyTime(TimeSpan timeSpan)
    {
        Type = KeyTimeType.TimeSpan;
        _value = timeSpan;
    }

    private KeyTime(KeyTimeType type)
    {
        Type = type;
    }

    /// <summary>
    /// Creates a new <see cref="KeyTime"/>, using the supplied <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The value of the new <see cref="KeyTime"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="KeyTime"/>, initialized to the value of timeSpan.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The specified timeSpan is outside the allowed range.
    /// </exception>
    public static KeyTime FromTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan));
        }

        return new KeyTime(timeSpan);
    }

    /// <summary>
    /// Gets a uniform value, which divides the allotted time of the animation evenly
    /// between key frames.
    /// </summary>
    /// <returns>
    /// A uniform value, which divides the allotted time of the animation evenly between
    /// key frames.
    /// </returns>
    public static KeyTime Uniform => new(KeyTimeType.Uniform);

    /// <summary>
    /// Gets the time when the key frame ends, expressed as a time relative to the beginning
    /// of the animation.
    /// </summary>
    /// <returns>
    /// The time when the key frame ends, expressed as a time relative to the beginning
    /// of the animation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If this instance is not of type <see cref="KeyTimeType.TimeSpan"/>.
    /// </exception>
    public TimeSpan TimeSpan
    {
        get
        {
            if (Type != KeyTimeType.TimeSpan)
            {
                throw new InvalidOperationException();
            }

            return _value;
        }
    }

    /// <summary>
    /// Gets the <see cref="KeyTimeType"/> value this instance represents.
    /// </summary>
    /// <returns>
    /// One of the enumeration values.
    /// </returns>
    public KeyTimeType Type { get; }

    /// <summary>
    /// Indicates whether a specified <see cref="KeyTime"/> is equal to this 
    /// <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="KeyTime"/> to compare with this <see cref="KeyTime"/>.
    /// </param>
    /// <returns>
    /// true if value is equal to this <see cref="KeyTime"/>; otherwise, false.
    /// </returns>
    public bool Equals(KeyTime value) => Equals(this, value);

    /// <summary>
    /// Indicates whether a <see cref="KeyTime"/> is equal to this <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="KeyTime"/> to compare with this <see cref="KeyTime"/>.
    /// </param>
    /// <returns>
    /// true if value is a <see cref="KeyTime"/> that represents the same length 
    /// of time as this <see cref="KeyTime"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object value) => value is KeyTime other && Equals(this, other);

    /// <summary>
    /// Returns a hash code representing this <see cref="KeyTime"/>.
    /// </summary>
    /// <returns>
    /// A hash code identifier.
    /// </returns>
    public override int GetHashCode() => Type == KeyTimeType.TimeSpan ? _value.GetHashCode() : Type.GetHashCode();

    /// <summary>
    /// Returns a string representation of this <see cref="KeyTime"/>.
    /// </summary>
    /// <returns>
    /// A string representation of this <see cref="KeyTime"/>.
    /// </returns>
    public override string ToString() =>
        Type switch
        {
            KeyTimeType.Uniform => "Uniform",
            KeyTimeType.TimeSpan => _value.ToString(),
            _ => string.Empty,
        };

    /// <summary>
    /// Indicates whether two <see cref="KeyTime"/> values are equal.
    /// </summary>
    /// <param name="keyTime1">
    /// The first value to compare.
    /// </param>
    /// <param name="keyTime2">
    /// The second value to compare.
    /// </param>
    /// <returns>
    /// true if the values of keyTime1 and keyTime2 are equal; otherwise, false.
    /// </returns>
    public static bool Equals(KeyTime keyTime1, KeyTime keyTime2) =>
        keyTime1.Type switch
        {
            KeyTimeType.Uniform => keyTime2.Type == KeyTimeType.Uniform,
            KeyTimeType.TimeSpan => keyTime2.Type == KeyTimeType.TimeSpan && keyTime1._value == keyTime2._value,
            _ => false,
        };

    /// <summary>
    /// Compares two <see cref="KeyTime"/> values for equality.
    /// </summary>
    /// <param name="keyTime1">
    /// The first value to compare.
    /// </param>
    /// <param name="keyTime2">
    /// The second value to compare.
    /// </param>
    /// <returns>
    /// true if keyTime1 and keyTime2 are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(KeyTime keyTime1, KeyTime keyTime2) => Equals(keyTime1, keyTime2);

    /// <summary>
    /// Compares two <see cref="KeyTime"/> values for inequality.
    /// </summary>
    /// <param name="keyTime1">
    /// The first value to compare.
    /// </param>
    /// <param name="keyTime2">
    /// The second value to compare.
    /// </param>
    /// <returns>
    /// true if keyTime1 and keyTime2 are not equal; otherwise, false.
    /// </returns>
    public static bool operator !=(KeyTime keyTime1, KeyTime keyTime2) => !Equals(keyTime1, keyTime2);

    /// <summary>
    /// Implicitly converts a <see cref="System.TimeSpan"/> to a <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="timeSpan">
    /// The <see cref="TimeSpan"/> value to convert.
    /// </param>
    /// <returns>
    /// The created <see cref="KeyTime"/>.
    /// </returns>
    public static implicit operator KeyTime(TimeSpan timeSpan) => FromTimeSpan(timeSpan);
}
