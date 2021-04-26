

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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.ComponentModel;
#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(RepeatBehaviorConverter))]
#endif
    /// <summary>
    /// Describes how a Windows.UI.Xaml.Media.Animation.Timeline repeats its simple
    /// duration.
    /// </summary>
    public partial struct RepeatBehavior// : IFormattable
    {

        //for details on how the animations are supposed to behave depending on RepeatBehavior, see: https://msdn.microsoft.com/en-us/library/system.windows.media.animation.timeline.repeatbehavior(v=vs.100).aspx
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     count evaluates to infinity, a value that is not a number, or is negative.

        /// <summary>
        /// Initializes a new instance of the Windows.UI.Xaml.Media.Animation.RepeatBehavior
        /// structure with the specified iteration count.
        /// </summary>
        /// <param name="count">
        /// A number greater than or equal to 0 that specifies the number of iterations
        /// for an animation.
        /// </param>
        public RepeatBehavior(double count)
        {
            _type = RepeatBehaviorType.Count;
            _count = count;
            _hasCount = true;
#if WORKINPROGRESS
            HasDuration = false;
            Duration = new TimeSpan();
#endif
        }

        static RepeatBehavior()
        {
            TypeFromStringConverters.RegisterConverter(typeof(RepeatBehavior), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string arg)
        {
            //BRIDGETODO : verify the code below matchs
#if !BRIDGE
            string loweredArg = arg.ToLowerInvariant();
#else
            string loweredArg = arg.ToLower();
#endif
            if (loweredArg == "forever")
            {
                return RepeatBehavior.Forever;
            }
            else if (loweredArg.EndsWith("x"))
            {
                double repeatCount = double.Parse(loweredArg.Substring(0, loweredArg.Length - 1));
                return new RepeatBehavior(repeatCount);
            }
            else
            {
                throw new FormatException("The string: \"" + arg + "\" could not be parsed into a RepeatBehavior. Note: The duration is not supported yet as a RepeatBehavior.");
            }
            //todo: else duration.
        }

        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     duration evaluates to a negative number.
        ///// <summary>
        ///// Initializes a new instance of the Windows.UI.Xaml.Media.Animation.RepeatBehavior
        ///// structure with the specified repeat duration.
        ///// </summary>
        ///// <param name="duration">
        ///// The total length of time that the Windows.UI.Xaml.Media.Animation.Timeline
        ///// should play (its active duration).
        ///// </param>
        //public RepeatBehavior(TimeSpan duration)
        //{
        //    _type = RepeatBehaviorType.Duration;
        //    _count = 1;
        //    _duration = duration;
        //    _hasDuration = true;
        //    _hasCount = false;
        //}

        /// <summary>
        /// Indicates whether the two Windows.UI.Xaml.Media.Animation.RepeatBehavior
        /// values are not equal.
        /// </summary>
        /// <param name="repeatBehavior1">The first value to compare.</param>
        /// <param name="repeatBehavior2">The second value to compare.</param>
        /// <returns>
        /// true if repeatBehavior1 and repeatBehavior2 are different types or the repeat
        /// behavior properties are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
        {
            if (repeatBehavior1._hasCount)
            {
                if ((!repeatBehavior2._hasCount) || repeatBehavior1._count != repeatBehavior2._count)
                {
                    return true;
                }
            }
            else if (repeatBehavior2._hasCount)
            {
                return true;
            }

            //if (repeatBehavior1._hasDuration)
            //{
            //    if ((!repeatBehavior2._hasDuration) || repeatBehavior1._duration != repeatBehavior2._duration)
            //    {
            //        return true;
            //    }
            //}
            //else if (repeatBehavior2._hasDuration)
            //{
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// Indicates whether the two specified Windows.UI.Xaml.Media.Animation.RepeatBehavior
        /// values are equal.
        /// </summary>
        /// <param name="repeatBehavior1">The first value to compare.</param>
        /// <param name="repeatBehavior2">The second value to compare.</param>
        /// <returns>
        /// true if both the type and repeat behavior of repeatBehavior1 are equal to
        /// that of repeatBehavior2; otherwise, false.
        /// </returns>
        public static bool operator ==(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
        {
            return !(repeatBehavior1 != repeatBehavior2);
        }

        double _count;
        // Exceptions:
        //   System.InvalidOperationException:
        //     This Windows.UI.Xaml.Media.Animation.RepeatBehavior describes a repeat duration,
        //     not an iteration count.
        /// <summary>
        /// Gets the number of times a Windows.UI.Xaml.Media.Animation.Timeline should
        /// repeat.
        /// </summary>
        public double Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                _hasCount = true;
                //_hasDuration = false;
                _type = RepeatBehaviorType.Count;
            }
        }

        //TimeSpan _duration;
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     This Windows.UI.Xaml.Media.Animation.RepeatBehavior describes an iteration
        ////     count, not a repeat duration.
        ///// <summary>
        ///// Gets the total length of time a Windows.UI.Xaml.Media.Animation.Timeline
        ///// should play.
        ///// </summary>
        //public TimeSpan Duration
        //{
        //    get
        //    {
        //        return _duration;
        //    }
        //    set
        //    {
        //        _duration = value;
        //        _hasCount = false;
        //        _hasDuration = true;
        //        _type = RepeatBehaviorType.Duration;
        //    }
        //}

        /// <summary>
        /// Gets a Windows.UI.Xaml.Media.Animation.RepeatBehavior that specifies an infinite
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

        bool _hasCount;
        /// <summary>
        /// Gets a value that indicates whether the repeat behavior has a specified iteration
        /// count.
        /// </summary>
        public bool HasCount { get { return _hasCount; } }

        //bool _hasDuration;
        ///// <summary>
        ///// Gets a value that indicates whether the repeat behavior has a specified repeat
        ///// duration.
        ///// </summary>
        //public bool HasDuration { get { return _hasDuration; } }
        ///
        //Note: the following does not exist in WPF or Silverlight (but it does in WinRT).
        RepeatBehaviorType _type;
        /// <summary>
        /// Gets or sets one of the Windows.UI.Xaml.Media.Animation.RepeatBehaviorType
        /// values that describes the way behavior repeats.
        /// </summary>
        public RepeatBehaviorType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                if (value == RepeatBehaviorType.Count)
                {
                    if (_count > 0)
                    {
                        _hasCount = true;
                    }
                    //_hasDuration = false;
                }
                //else if(value == RepeatBehaviorType.Duration)
                //{
                //    _hasCount = false;
                //    if (_duration > new TimeSpan())
                //    {
                //        _hasDuration = true;
                //    }
                //}
                else if (value == RepeatBehaviorType.Forever)
                {
                    _hasCount = false;
                    //_hasDuration = false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the specified object is equal to this Windows.UI.Xaml.Media.Animation.RepeatBehavior.
        /// </summary>
        /// <param name="value">The object to compare with this Windows.UI.Xaml.Media.Animation.RepeatBehavior.</param>
        /// <returns>
        /// true if value is a Windows.UI.Xaml.Media.Animation.RepeatBehavior that represents
        /// the same repeat behavior as this Windows.UI.Xaml.Media.Animation.RepeatBehavior;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is RepeatBehavior))
            {
                return false;
            }
            return this == (RepeatBehavior)value;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified Windows.UI.Xaml.Media.Animation.RepeatBehavior
        /// is equal to this Windows.UI.Xaml.Media.Animation.RepeatBehavior.
        /// </summary>
        /// <param name="repeatBehavior">The value to compare to this Windows.UI.Xaml.Media.Animation.RepeatBehavior.</param>
        /// <returns>
        /// true if both the type and repeat behavior of repeatBehavior are equal to
        /// this Windows.UI.Xaml.Media.Animation.RepeatBehavior; otherwise, false.
        /// </returns>
        public bool Equals(RepeatBehavior repeatBehavior)
        {
            return this == repeatBehavior;
        }

        /// <summary>
        /// Indicates whether the two specified Windows.UI.Xaml.Media.Animation.RepeatBehavior
        /// values are equal.
        /// </summary>
        /// <param name="repeatBehavior1">The first value to compare.</param>
        /// <param name="repeatBehavior2">The second value to compare.</param>
        /// <returns>
        /// true if both the type and repeat behavior of repeatBehavior1 are equal to
        /// that of repeatBehavior2; otherwise, false.
        /// </returns>
        public static bool Equals(RepeatBehavior repeatBehavior1, RepeatBehavior repeatBehavior2)
        {
            return repeatBehavior1 == repeatBehavior2;

        }

        //todo: the following

        ///// <summary>
        ///// Returns the hash code of this instance.
        ///// </summary>
        ///// <returns>A hash code.</returns>
        //public override int GetHashCode();

        ///// <summary>
        ///// Returns a string representation of this Windows.UI.Xaml.Media.Animation.RepeatBehavior.
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString();

        ///// <summary>
        ///// Returns a string representation of this Windows.UI.Xaml.Media.Animation.RepeatBehavior
        ///// with the specified format.
        ///// </summary>
        ///// <param name="formatProvider">The format used to construct the return value.</param>
        ///// <returns>A string representation of this Windows.UI.Xaml.Media.Animation.RepeatBehavior.</returns>
        //public string ToString(IFormatProvider formatProvider);
    }
}