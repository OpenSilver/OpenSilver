
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

using System.ComponentModel;
using System.Text;

namespace System.Windows.Media.Animation;

/// <summary>
/// Describes how a <see cref="Timeline"/> repeats its simple duration.
/// </summary>
public struct RepeatBehavior : IFormattable
{
    private double _iterationCount;
    private TimeSpan _repeatDuration;
    private RepeatBehaviorType _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepeatBehavior"/>
    /// structure with the specified iteration count.
    /// </summary>
    /// <param name="count">
    /// A number greater than or equal to 0 that specifies the number of iterations for
    /// an animation.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// count evaluates to infinity, a value that is not a number, or is negative.
    /// </exception>
    public RepeatBehavior(double count)
    {
        if (double.IsInfinity(count)
            || double.IsNaN(count)
            || count < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                $"'{count}' is not a valid IterationCount value for a RepeatBehavior structure. An IterationCount value must represent a number that is greater than or equal to zero but not infinite.");
        }

        _repeatDuration = new TimeSpan(0);
        _iterationCount = count;
        _type = RepeatBehaviorType.Count;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepeatBehavior"/>
    /// structure with the specified repeat duration.
    /// </summary>
    /// <param name="duration">
    /// The total length of time that the <see cref="Timeline"/> should
    /// play (its active duration).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// duration evaluates to a negative number.
    /// </exception>
    public RepeatBehavior(TimeSpan duration)
    {
        if (duration < new TimeSpan(0))
        {
            throw new ArgumentOutOfRangeException(nameof(duration),
                string.Format("'{0}' is not a valid RepeatDuration value for a RepeatBehavior structure. A RepeatDuration value must be a TimeSpan value greater than or equal to zero ticks.", duration)
            );
        }

        _iterationCount = 0.0;
        _repeatDuration = duration;
        _type = RepeatBehaviorType.Duration;
    }

    /// <summary>
    /// Gets a <see cref="RepeatBehavior"/> that specifies an infinite
    /// number of repetitions.
    /// </summary>
    public static RepeatBehavior Forever
    {
        get
        {
            RepeatBehavior forever = new RepeatBehavior();
            forever._type = RepeatBehaviorType.Forever;

            return forever;
        }
    }

    /// <summary>
    /// Gets the number of times a <see cref="Timeline"/> should repeat.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// This <see cref="RepeatBehavior"/> describes a repeat duration,
    /// not an iteration count.
    /// </exception>
    public double Count
    {
        get
        {
            if (_type != RepeatBehaviorType.Count)
            {
                throw new InvalidOperationException(
                    $"'{this}' RepeatBehavior does not represent an iteration count and does not have an IterationCount value.");
            }

            return _iterationCount;
        }
    }

    /// <summary>
    /// Gets the total length of time a <see cref="Timeline"/> should
    /// play.
    /// </summary>
    /// <returns>
    /// The total length of time a timeline should play.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// This <see cref="RepeatBehavior"/> describes an iteration count,
    /// not a repeat duration.
    /// </exception>
    public TimeSpan Duration
    {
        get
        {
            if (_type != RepeatBehaviorType.Duration)
            {
                throw new InvalidOperationException(
                    $"'{this}' RepeatBehavior does not represent a repeat duration and does not have a RepeatDuration value.");
            }

            return _repeatDuration;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the repeat behavior has a specified iteration
    /// count.
    /// </summary>
    public bool HasCount
    {
        get
        {
            return _type == RepeatBehaviorType.Count;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the repeat behavior has a specified repeat
    /// duration.
    /// </summary>
    /// <returns>
    /// true if the instance represents a repeat duration; otherwise, false.
    /// </returns>
    public bool HasDuration
    {
        get
        {
            return _type == RepeatBehaviorType.Duration;
        }
    }

    /// <summary>
    /// Indicates whether the two specified <see cref="RepeatBehavior"/>
    /// values are equal.
    /// </summary>
    /// <param name="repeatBehavior1">The first value to compare.</param>
    /// <param name="repeatBehavior2">The second value to compare.</param>
    /// <returns>
    /// true if both the type and repeat behavior of repeatBehavior1 are equal to that
    /// of repeatBehavior2; otherwise, false.
    /// </returns>
    public static bool Equals(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
    {
        return repeatBehavior1.Equals(repeatBehavior2);
    }

    /// <summary>
    /// Indicates whether the specified object is equal to this <see cref="RepeatBehavior"/>.
    /// </summary>
    /// <param name="value">
    /// The object to compare with this <see cref="RepeatBehavior"/>.
    /// </param>
    /// <returns>
    /// true if value is a <see cref="RepeatBehavior"/> that represents
    /// the same repeat behavior as this <see cref="RepeatBehavior"/>;
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object value)
    {
        if (value is RepeatBehavior)
        {
            return this.Equals((RepeatBehavior)value);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the specified <see cref="RepeatBehavior"/>
    /// is equal to this <see cref="RepeatBehavior"/>.
    /// </summary>
    /// <param name="repeatBehavior">
    /// The value to compare to this <see cref="RepeatBehavior"/>.
    /// </param>
    /// <returns>
    /// true if both the type and repeat behavior of repeatBehavior are equal to this
    /// <see cref="RepeatBehavior"/>; otherwise, false.
    /// </returns>
    public bool Equals(RepeatBehavior repeatBehavior)
    {
        if (_type == repeatBehavior._type)
        {
            switch (_type)
            {
                case RepeatBehaviorType.Forever:

                    return true;

                case RepeatBehaviorType.Count:

                    return _iterationCount == repeatBehavior._iterationCount;

                case RepeatBehaviorType.Duration:

                    return _repeatDuration == repeatBehavior._repeatDuration;

                default:

                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the hash code of this instance.
    /// </summary>
    public override int GetHashCode()
    {
        switch (_type)
        {
            case RepeatBehaviorType.Count:

                return _iterationCount.GetHashCode();

            case RepeatBehaviorType.Duration:

                return _repeatDuration.GetHashCode();

            case RepeatBehaviorType.Forever:

                // We try to choose an unlikely hash code value for Forever.
                // All Forevers need to return the same hash code value.
                return int.MaxValue - 42;

            default:

                return base.GetHashCode();
        }
    }

    /// <summary>
    /// Returns a string representation of this <see cref="RepeatBehavior"/>.
    /// </summary>
    /// <returns>
    /// A string representation of this <see cref="RepeatBehavior"/>.
    /// </returns>
    public override string ToString()
    {
        return InternalToString(null, null);
    }

    /// <summary>
    /// Returns a string representation of this <see cref="RepeatBehavior"/>
    /// with the specified format.
    /// </summary>
    /// <param name="formatProvider">
    /// The format used to construct the return value.
    /// </param>
    /// <returns>
    /// A string representation of this <see cref="RepeatBehavior"/>.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
        return InternalToString(null, formatProvider);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    string IFormattable.ToString(string format, IFormatProvider formatProvider)
    {
        return InternalToString(format, formatProvider);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    internal string InternalToString(string format, IFormatProvider formatProvider)
    {
        switch (_type)
        {
            case RepeatBehaviorType.Forever:

                return "Forever";

            case RepeatBehaviorType.Count:

                StringBuilder sb = new StringBuilder();

                sb.Append(
                    string.Format(formatProvider, "{0:" + format + "}x", _iterationCount)
                );

                return sb.ToString();

            case RepeatBehaviorType.Duration:

                return _repeatDuration.ToString();

            default:

                return null;
        }
    }

    /// <summary>
    /// Indicates whether the two specified <see cref="RepeatBehavior"/>
    /// values are equal.
    /// </summary>
    /// <param name="repeatBehavior1">The first value to compare.</param>
    /// <param name="repeatBehavior2">The second value to compare.</param>
    /// <returns>
    /// true if both the type and repeat behavior of repeatBehavior1 are equal to that
    /// of repeatBehavior2; otherwise, false.
    /// </returns>
    public static bool operator ==(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
    {
        return repeatBehavior1.Equals(repeatBehavior2);
    }

    /// <summary>
    /// Indicates whether the two <see cref="RepeatBehavior"/> values
    /// are not equal.
    /// </summary>
    /// <param name="repeatBehavior1">The first value to compare.</param>
    /// <param name="repeatBehavior2">The second value to compare.</param>
    /// <returns>
    /// true if repeatBehavior1 and repeatBehavior2 are different types or the repeat
    /// behavior properties are not equal; otherwise, false.
    /// </returns>
    public static bool operator !=(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
    {
        return !repeatBehavior1.Equals(repeatBehavior2);
    }

    /// <summary>
    /// Gets or sets one of the <see cref="RepeatBehaviorType"/>
    /// values that describes the way behavior repeats.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public RepeatBehaviorType Type => _type;
}