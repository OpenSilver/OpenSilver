

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(DurationConverter))]
#endif
    /// <summary>
    /// Represents the duration of time that a Windows.UI.Xaml.Media.Animation.Timeline
    /// is active.
    /// </summary>
    public partial struct Duration
    {
        private DurationType _durationType;
        // Exceptions:
        //   System.ArgumentException:
        //     timeSpan evaluates as less than System.TimeSpan.Zero.
        /// <summary>
        /// Initializes a new instance of the Windows.UI.Xaml.Duration structure with
        /// the supplied System.TimeSpan value.
        /// </summary>
        /// <param name="timeSpan">Represents the initial time interval of this duration.</param>
        public Duration(TimeSpan timeSpan)
        {
            if (timeSpan >= TimeSpan.Zero)
            {
                _timeSpan = timeSpan;
                _durationType = DurationType.TimeSpan;
            }
            else
            {
                throw new ArgumentException("The TimeSpan cannot be a negative TimeSpan.");
            }
        }

        static Duration()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Duration), INTERNAL_ConvertFromString);
        }

        // Returns:
        //     If each Windows.UI.Xaml.Duration has values, a Windows.UI.Xaml.Duration that
        //     represents the value of t1 minus t2. If t1 has a value of Windows.UI.Xaml.Duration.Forever
        //     and t2 has a value of Windows.UI.Xaml.Duration.TimeSpan, this method returns
        //     Windows.UI.Xaml.Duration.Forever. Otherwise this method returns null.
        /// <summary>
        /// Subtracts the value of one Windows.UI.Xaml.Duration from another.
        /// </summary>
        /// <param name="t1">The first Windows.UI.Xaml.Duration.</param>
        /// <param name="t2">The Windows.UI.Xaml.Duration to subtract.</param>
        /// <returns></returns>
        public static Duration operator -(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return new Duration(t1.TimeSpan - t2.TimeSpan);
            }

            if (t1._durationType == DurationType.Forever && t2.HasTimeSpan)
            {
                return Duration.Forever;
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
                return Duration.Automatic;
            }
        }

        /// <summary>
        /// Determines if two Windows.UI.Xaml.Duration cases are not equal.
        /// </summary>
        /// <param name="t1">The first Windows.UI.Xaml.Duration to compare.</param>
        /// <param name="t2">The second Windows.UI.Xaml.Duration to compare.</param>
        /// <returns>
        /// true if exactly one of t1 or t2 represent a value, or if they both represent
        /// values that are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(Duration t1, Duration t2)
        {
            return !(t1.Equals(t2));
        }

        /// <summary>
        /// Returns the specified Windows.UI.Xaml.Duration.
        /// </summary>
        /// <param name="duration">The Windows.UI.Xaml.Duration to get.</param>
        /// <returns>The Windows.UI.Xaml.Duration operation result.</returns>
        public static Duration operator +(Duration duration)
        {
            return duration;
        }

        ///// <summary>
        ///// Adds two Windows.UI.Xaml.Duration values together.
        ///// </summary>
        ///// <param name="t1">The first Windows.UI.Xaml.Duration to add.</param>
        ///// <param name="t2">The second Windows.UI.Xaml.Duration to add.</param>
        ///// <returns>
        ///// If both Windows.UI.Xaml.Duration values have System.TimeSpan values, this
        ///// method returns the sum of those two values. If either value is set to Windows.UI.Xaml.Duration.Automatic,
        ///// the method returns Windows.UI.Xaml.Duration.Automatic. If either value is
        ///// set to Windows.UI.Xaml.Duration.Forever, the method returns Windows.UI.Xaml.Duration.Forever.If
        ///// either t1 or t2 has no value, this method returns null.
        ///// </returns>
        //public static Duration operator +(Duration t1, Duration t2);

        ///// <summary>
        ///// Determines if a Windows.UI.Xaml.Duration is less than the value of another
        ///// instance.
        ///// </summary>
        ///// <param name="t1">The first Windows.UI.Xaml.Duration to compare.</param>
        ///// <param name="t2">The second Windows.UI.Xaml.Duration to compare.</param>
        ///// <returns>true if both t1 and t2 have values and t1 is less than t2; otherwise, false.</returns>
        //public static bool operator <(Duration t1, Duration t2);

        ///// <summary>
        ///// Determines if a Windows.UI.Xaml.Duration is less than or equal to another.
        ///// </summary>
        ///// <param name="t1">The Windows.UI.Xaml.Duration to compare.</param>
        ///// <param name="t2">The Windows.UI.Xaml.Duration to compare.</param>
        ///// <returns>
        ///// true if both t1 and t2 have values and t1 is less than or equal to t2; otherwise,
        ///// false.
        ///// </returns>
        //public static bool operator <=(Duration t1, Duration t2);

        /// <summary>
        /// Determines whether two Windows.UI.Xaml.Duration cases are equal.
        /// </summary>
        /// <param name="t1">The first Windows.UI.Xaml.Duration to compare.</param>
        /// <param name="t2">The second Windows.UI.Xaml.Duration to compare.</param>
        /// <returns>
        /// true if both Windows.UI.Xaml.Duration values have equal property values,
        /// or if all Windows.UI.Xaml.Duration values are null. Otherwise, this method
        /// returns false.
        /// </returns>
        public static bool operator ==(Duration t1, Duration t2)
        {
            return t1.Equals(t2);
        }

        ///// <summary>
        ///// Determines if one Windows.UI.Xaml.Duration is greater than another.
        ///// </summary>
        ///// <param name="t1">The Windows.UI.Xaml.Duration value to compare.</param>
        ///// <param name="t2">The second Windows.UI.Xaml.Duration value to compare.</param>
        ///// <returns>
        ///// true if both t1 and t2 have values and t1 is greater than t2; otherwise,
        ///// false.
        ///// </returns>
        //public static bool operator >(Duration t1, Duration t2);

        ///// <summary>
        ///// Determines whether a Windows.UI.Xaml.Duration is greater than or equal to
        ///// another.
        ///// </summary>
        ///// <param name="t1">The first instance of Windows.UI.Xaml.Duration to compare.</param>
        ///// <param name="t2">The second instance of Windows.UI.Xaml.Duration to compare.</param>
        ///// <returns>
        ///// true if both t1 and t2 have values and t1 is greater than or equal to t2;
        ///// otherwise, false.
        ///// </returns>
        //public static bool operator >=(Duration t1, Duration t2);

        /// <summary>
        /// Implicitly creates a Duration from a given System.TimeSpan.
        /// </summary>
        /// <param name="timeSpan">System.TimeSpan from which a Duration is implicitly created.</param>
        /// <returns>A created Duration.</returns>
        public static implicit operator Duration(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("The TimeSpan cannot be a negative TimeSpan.", "timeSpan");
            }
            return new Duration(timeSpan);
        }

        /// <summary>
        /// Gets a Windows.UI.Xaml.Duration value that is automatically determined.
        /// </summary>
        public static Duration Automatic
        {
            //Note: we put 1 millisecond in the TimeSpan below because Velocity does not like animations with a duration of 0ms (it decides it is something like 1 second or something like that)
            get
            {
                Duration duration = new Duration(new TimeSpan());
                duration._durationType = DurationType.Automatic;
                return duration;
            }
        }

        /// <summary>
        /// Gets a Windows.UI.Xaml.Duration value that represents an infinite interval.
        /// </summary>
        public static Duration Forever
        {
            //get { return new Duration(TimeSpan.MaxValue); }
            get
            {
                Duration duration = new Duration(new TimeSpan(9999, 0, 0, 0));
                duration._durationType = DurationType.Forever;
                return duration;
            } //todo: fix support for TimeSpan.MaxValue and then replace this line with the commented one above.
        }

        /// <summary>
        /// Gets a value that indicates if this Windows.UI.Xaml.Duration represents a
        /// System.TimeSpan value.
        /// </summary>
        public bool HasTimeSpan { //we assume it is like that
            get
            {
                //return (_timeSpan != null
                //    //&& _timeSpan != TimeSpan.MaxValue);
                //    && _timeSpan != new TimeSpan(9999, 0, 0, 0)); //todo: fix support for TimeSpan.MaxValue and then replace this line with the commented one above.
                return _durationType == DurationType.TimeSpan;
            }
        }

        private TimeSpan _timeSpan;
        // Exceptions:
        //   System.InvalidOperationException:
        //     The Windows.UI.Xaml.Duration does not represent a System.TimeSpan.
        /// <summary>
        /// Gets the System.TimeSpan value that this Windows.UI.Xaml.Duration represents.
        /// </summary>
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
                    throw new InvalidOperationException("The Duration does no represent a System.TimeSpan.");
                }
            }
        }

        ///// <summary>
        ///// Adds the value of the specified Windows.UI.Xaml.Duration to this Windows.UI.Xaml.Duration.
        ///// </summary>
        ///// <param name="duration">
        ///// An instance of Windows.UI.Xaml.Duration that represents the value of the
        ///// current instance plus duration.
        ///// </param>
        ///// <returns>
        ///// If each involved Windows.UI.Xaml.Duration has values, a Windows.UI.Xaml.Duration
        ///// that represents the combined values. Otherwise this method returns null.
        ///// </returns>
        //public Duration Add(Duration duration);

        ///// <summary>
        ///// Compares one Windows.UI.Xaml.Duration value to another.
        ///// </summary>
        ///// <param name="t1">The first instance of Windows.UI.Xaml.Duration to compare.</param>
        ///// <param name="t2">The second instance of Windows.UI.Xaml.Duration to compare.</param>
        ///// <returns>
        ///// If t1 is less than t2, a negative value that represents the difference. If
        ///// t1 is equal to t2, a value of 0. If t1 is greater than t2, a positive value
        ///// that represents the difference.
        ///// </returns>
        //public static int Compare(Duration t1, Duration t2);

        /// <summary>
        /// Determines whether a specified Windows.UI.Xaml.Duration is equal to this
        /// Windows.UI.Xaml.Duration.
        /// </summary>
        /// <param name="duration">The Windows.UI.Xaml.Duration to check for equality.</param>
        /// <returns>true if duration is equal to this Windows.UI.Xaml.Duration; otherwise, false.</returns>
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
        /// Determines whether a specified object is equal to a Windows.UI.Xaml.Duration.
        /// </summary>
        /// <param name="value">Object to check for equality.</param>
        /// <returns>true if value is equal to this Windows.UI.Xaml.Duration; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            if (value is Duration)
            {
                return this == (Duration)value;
            }
            return false;
        }

        /// <summary>
        /// Determines whether two Windows.UI.Xaml.Duration values are equal.
        /// </summary>
        /// <param name="t1">First Windows.UI.Xaml.Duration to compare.</param>
        /// <param name="t2">Second Windows.UI.Xaml.Duration to compare.</param>
        /// <returns>true if t1 is equal to t2; otherwise, false.</returns>
        public static bool Equals(Duration t1, Duration t2)
        {
            return t1 == t2;
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code identifier.</returns>
        public override int GetHashCode()
        {
            return TimeSpan.GetHashCode();
        }

        ///// <summary>
        ///// Subtracts the specified Windows.UI.Xaml.Duration from this Windows.UI.Xaml.Duration.
        ///// </summary>
        ///// <param name="duration">The Windows.UI.Xaml.Duration to subtract from this Windows.UI.Xaml.Duration.</param>
        ///// <returns>The subtracted Windows.UI.Xaml.Duration.</returns>
        //public Duration Subtract(Duration duration);

        ///// <summary>
        ///// Converts a Windows.UI.Xaml.Duration to a System.String representation.
        ///// </summary>
        ///// <returns>A System.String representation of this Windows.UI.Xaml.Duration.</returns>
        //public override string ToString();

        internal static object INTERNAL_ConvertFromString(string p)
        {
            if (p.ToLower() == "forever")
                return Duration.Forever;
            if (p.ToLower() == "automatic")
                return Duration.Automatic;
#if BRIDGE
            TimeSpan timeSpan = INTERNAL_BridgeWorkarounds.TimeSpanParse(p);
#else
            TimeSpan timeSpan = TimeSpan.Parse(p);
#endif
            return new Duration(timeSpan);
        }

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