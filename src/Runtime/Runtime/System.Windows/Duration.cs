
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

using System;

namespace System.Windows
{
    /// <summary>
    /// Represents the duration of time that a <see cref="Media.Animation.Timeline"/> is active.
    /// </summary>
    public struct Duration
    {
        private TimeSpan _timeSpan;
        private DurationType _durationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> structure with the
        /// supplied <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="timeSpan">
        /// Represents the initial time interval of this duration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// timeSpan evaluates as less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public Duration(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("Property value must be greater than or equal to zero or indefinite.", nameof(timeSpan));
            }

            _durationType = DurationType.TimeSpan;
            _timeSpan = timeSpan;
        }

        /// <summary>
        /// Gets a <see cref="Duration"/> value that is automatically determined.
        /// </summary>
        /// <value>
        /// A <see cref="Duration"/> initialized to an automatic value.
        /// </value>
        public static Duration Automatic => new Duration { _durationType = DurationType.Automatic };

        /// <summary>
        /// Gets a <see cref="Duration"/> value that represents an infinite interval.
        /// </summary>
        /// <value>
        /// A <see cref="Duration"/> initialized to a forever value.
        /// </value>
        public static Duration Forever => new Duration { _durationType = DurationType.Forever };

        /// <summary>
        /// Gets a value that indicates if this <see cref="Duration"/> represents a <see cref="TimeSpan"/>
        /// value.
        /// </summary>
        /// <value>
        /// true if this <see cref="Duration"/> is a <see cref="TimeSpan"/> value; otherwise, false.
        /// </value>
        public bool HasTimeSpan => _durationType == DurationType.TimeSpan;

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> value that this <see cref="Duration"/> represents.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/> value that this <see cref="Duration"/> represents.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Duration"/> does not represent a <see cref="TimeSpan"/>.
        /// </exception>
        public TimeSpan TimeSpan
        {
            get
            {
                if (HasTimeSpan)
                {
                    return _timeSpan;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Unable to return a TimeSpan property value for a Duration value of '{0}'. Check the HasTimeSpan property before requesting the TimeSpan property value from a Duration.",
                            this));
                }
            }
        }

        /// <summary>
        /// Adds the value of the specified <see cref="Duration"/> to this <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">
        /// An instance of <see cref="Duration"/> that represents the value of the current
        /// instance plus duration.
        /// </param>
        /// <returns>
        /// If each involved <see cref="Duration"/> has values, a <see cref="Duration"/>
        /// that represents the combined values. Otherwise this method returns null.
        /// </returns>
        public Duration Add(Duration duration) => this + duration;

        /// <summary>
        /// Determines whether a specified object is equal to a <see cref="Duration"/>.
        /// </summary>
        /// <param name="value">
        /// Object to check for equality.
        /// </param>
        /// <returns>
        /// true if value is equal to this <see cref="Duration"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object value) => value is Duration duration && Equals(duration);

        /// <summary>
        /// Determines whether a specified <see cref="Duration"/> is equal to this <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">
        /// The <see cref="Duration"/> to check for equality.
        /// </param>
        /// <returns>
        /// true if duration is equal to this <see cref="Duration"/>; otherwise, false.
        /// </returns>
        public bool Equals(Duration duration)
        {
            if (HasTimeSpan)
            {
                if (duration.HasTimeSpan)
                {
                    return _timeSpan == duration._timeSpan;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return _durationType == duration._durationType;
            }
        }

        /// <summary>
        /// Determines whether two <see cref="Duration"/> values are equal.
        /// </summary>
        /// <param name="t1">
        /// First <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// Second <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if t1 is equal to t2; otherwise, false.
        /// </returns>
        public static bool Equals(Duration t1, Duration t2) => t1.Equals(t2);

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code identifier.
        /// </returns>
        public override int GetHashCode()
        {
            if (HasTimeSpan)
            {
                return _timeSpan.GetHashCode();
            }
            else
            {
                return _durationType.GetHashCode() + 17;
            }
        }

        /// <summary>
        /// Subtracts the specified <see cref="Duration"/> from this <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">
        /// The <see cref="Duration"/> to subtract from this <see cref="Duration"/>.
        /// </param>
        /// <returns>
        /// The subtracted <see cref="Duration"/>.
        /// </returns>
        public Duration Subtract(Duration duration) => this - duration;

        /// <summary>
        /// Converts a <see cref="Duration"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representation of this <see cref="Duration"/>.
        /// </returns>
        public override string ToString()
        {
            if (HasTimeSpan)
            {
                return _timeSpan.ToString();
            }
            else if (_durationType == DurationType.Forever)
            {
                return "Forever";
            }
            else // IsAutomatic
            {
                return "Automatic";
            }
        }

        //
        // Since Duration has two special values, for comparison purposes 
        // Duration.Forever behaves like Double.PositiveInfinity and
        // Duration.Automatic behaves almost entirely like Double.NaN
        // Any comparision with Automatic returns false, except for ==.
        // Unlike NaN, Automatic == Automatic is true.
        //

        /// <summary>
        /// Implicitly creates a <see cref="Duration"/> from a given <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeSpan">
        /// <see cref="TimeSpan"/> from which a <see cref="Duration"/> is implicitly created.
        /// </param>
        /// <returns>
        /// A created <see cref="Duration"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// timeSpan evaluates as less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public static implicit operator Duration(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("Property value must be greater than or equal to zero or indefinite.", nameof(timeSpan));
            }

            return new Duration(timeSpan);
        }

        /// <summary>
        /// Adds two <see cref="Duration"/> values together.
        /// </summary>
        /// <param name="t1">
        /// The first <see cref="Duration"/> to add.
        /// </param>
        /// <param name="t2">
        /// The second <see cref="Duration"/> to add.
        /// </param>
        /// <returns>
        /// If both <see cref="Duration"/> values have <see cref="TimeSpan"/> values, this method
        /// returns the sum of those two values. If either value is set to <see cref="Automatic"/>,
        /// the method returns <see cref="Automatic"/>. If either value is set to <see cref="Forever"/>, 
        /// the method returns <see cref="Forever"/>. If either t1 or t2 has no value, this method 
        /// returns null.
        /// </returns>
        public static Duration operator +(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return new Duration(t1._timeSpan + t2._timeSpan);
            }
            else if (t1._durationType != DurationType.Automatic
                     && t2._durationType != DurationType.Automatic)
            {
                // Neither t1 nor t2 are Automatic, so one is Forever
                // while the other is Forever or a TimeSpan.  Either way 
                // the sum is Forever.
                return Forever;
            }
            else
            {
                // Automatic + anything is Automatic
                return Automatic;
            }
        }

        /// <summary>
        /// Subtracts the value of one <see cref="Duration"/> from another.
        /// </summary>
        /// <param name="t1">
        /// The first <see cref="Duration"/>.
        /// </param>
        /// <param name="t2">
        /// The <see cref="Duration"/> to subtract.
        /// </param>
        /// <returns>
        /// If each <see cref="Duration"/> has values, a <see cref="Duration"/> that represents
        /// the value of t1 minus t2. If t1 has a value of <see cref="Forever"/> and t2 has a 
        /// value of <see cref="TimeSpan"/>, this method returns <see cref="Forever"/>.
        /// Otherwise this method returns null.
        /// </returns>
        public static Duration operator -(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return new Duration(t1._timeSpan - t2._timeSpan);
            }
            else if (t1._durationType == DurationType.Forever
                     && t2.HasTimeSpan)
            {
                // The only way for the result to be Forever is
                // if t1 is Forever and t2 is a TimeSpan
                return Forever;
            }
            else
            {
                // This covers the following conditions:
                // Forever - Forever
                // TimeSpan - Forever
                // TimeSpan - Automatic
                // Forever - Automatic
                // Automatic - Automatic
                // Automatic - Forever
                // Automatic - TimeSpan
                return Automatic;
            }
        }

        /// <summary>
        /// Determines whether two <see cref="Duration"/> cases are equal.
        /// </summary>
        /// <param name="t1">
        /// The first <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The second <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if both <see cref="Duration"/> values have equal property values, or if
        /// all <see cref="Duration"/> values are null. Otherwise, this method returns false.
        /// </returns>
        public static bool operator ==(Duration t1, Duration t2) => t1.Equals(t2);

        /// <summary>
        /// Determines if two <see cref="Duration"/> cases are not equal.
        /// </summary>
        /// <param name="t1">
        /// The first <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The second <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if exactly one of t1 or t2 represent a value, or if they both represent
        /// values that are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(Duration t1, Duration t2) => !t1.Equals(t2);

        /// <summary>
        /// Determines if one <see cref="Duration"/> is greater than another.
        /// </summary>
        /// <param name="t1">
        /// The <see cref="Duration"/> value to compare.
        /// </param>
        /// <param name="t2">
        /// The second <see cref="Duration"/> value to compare.
        /// </param>
        /// <returns>
        /// true if both t1 and t2 have values and t1 is greater than t2; otherwise, false.
        /// </returns>
        public static bool operator >(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return t1._timeSpan > t2._timeSpan;
            }
            else if (t1.HasTimeSpan && t2._durationType == DurationType.Forever)
            {
                // TimeSpan > Forever is false;
                return false;
            }
            else if (t1._durationType == DurationType.Forever && t2.HasTimeSpan)
            {
                // Forever > TimeSpan is true;
                return true;
            }
            else
            {
                // Cases covered:
                // Either t1 or t2 are Automatic, 
                // or t1 and t2 are both Forever 
                return false;
            }
        }

        /// <summary>
        /// Determines whether a <see cref="Duration"/> is greater than or equal to another.
        /// </summary>
        /// <param name="t1">
        /// The first instance of <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The second instance of <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if both t1 and t2 have values and t1 is greater than or equal to t2; otherwise,
        /// false.
        /// </returns>
        public static bool operator >=(Duration t1, Duration t2)
        {
            if (t1._durationType == DurationType.Automatic && t2._durationType == DurationType.Automatic)
            {
                // Automatic == Automatic
                return true;
            }
            else if (t1._durationType == DurationType.Automatic || t2._durationType == DurationType.Automatic)
            {
                // Automatic compared to anything else is false
                return false;
            }
            else
            {
                return !(t1 < t2);
            }
        }

        /// <summary>
        /// Determines if a <see cref="Duration"/> is less than the value of another instance.
        /// </summary>
        /// <param name="t1">
        /// The first <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The second <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if both t1 and t2 have values and t1 is less than t2; otherwise, false.
        /// </returns>
        public static bool operator <(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return t1._timeSpan < t2._timeSpan;
            }
            else if (t1.HasTimeSpan && t2._durationType == DurationType.Forever)
            {
                // TimeSpan < Forever is true;
                return true;
            }
            else if (t1._durationType == DurationType.Forever && t2.HasTimeSpan)
            {
                // Forever < TimeSpan is true;
                return false;
            }
            else
            {
                // Cases covered:
                // Either t1 or t2 are Automatic, 
                // or t1 and t2 are both Forever 
                return false;
            }
        }

        /// <summary>
        /// Determines if a <see cref="Duration"/> is less than or equal to another.
        /// </summary>
        /// <param name="t1">
        /// The <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// true if both t1 and t2 have values and t1 is less than or equal to t2; otherwise,
        /// false.
        /// </returns>
        public static bool operator <=(Duration t1, Duration t2)
        {
            if (t1._durationType == DurationType.Automatic && t2._durationType == DurationType.Automatic)
            {
                // Automatic == Automatic
                return true;
            }
            else if (t1._durationType == DurationType.Automatic || t2._durationType == DurationType.Automatic)
            {
                // Automatic compared to anything else is false
                return false;
            }
            else
            {
                return !(t1 > t2);
            }
        }

        /// <summary>
        /// Compares one <see cref="Duration"/> value to another.
        /// </summary>
        /// <param name="t1">
        /// The first instance of <see cref="Duration"/> to compare.
        /// </param>
        /// <param name="t2">
        /// The second instance of <see cref="Duration"/> to compare.
        /// </param>
        /// <returns>
        /// If t1 is less than t2, a negative value that represents the difference. If t1
        /// is equal to t2, a value of 0. If t1 is greater than t2, a positive value that
        /// represents the difference.
        /// </returns>
        public static int Compare(Duration t1, Duration t2)
        {
            if (t1._durationType == DurationType.Automatic)
            {
                if (t2._durationType == DurationType.Automatic)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (t2._durationType == DurationType.Automatic)
            {
                return 1;
            }
            else // Neither are Automatic, do a standard comparison
            {
                if (t1 < t2)
                {
                    return -1;
                }
                else if (t1 > t2)
                {
                    return 1;
                }
                else  // Neither is greater than the other
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the specified instance of <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">
        /// The instance of <see cref="Duration"/> to get.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Duration"/>.
        /// </returns>
        public static Duration Plus(Duration duration) => duration;

        /// <summary>
        /// Returns the specified <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">
        /// The <see cref="Duration"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="Duration"/> operation result.
        /// </returns>
        public static Duration operator +(Duration duration) => duration;

        /// <summary>
        /// An enumeration of the different types of Duration behaviors.
        /// </summary>
        private enum DurationType
        {
            Automatic,
            TimeSpan,
            Forever
        }
    }
}