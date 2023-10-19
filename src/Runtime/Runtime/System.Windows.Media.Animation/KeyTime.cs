
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Specifies when a particular key frame should take place during an animation.
    /// </summary>
    public struct KeyTime
    {
        internal KeyTime(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        /// <summary>
        /// Creates a new System.Windows.Media.Animation.KeyTime instance, with the System.Windows.Media.Animation.KeyTimeType
        /// property initialized to the value of the specified parameter.
        /// </summary>
        /// <param name="timeSpan">The value of the new System.Windows.Media.Animation.KeyTime.</param>
        /// <returns>
        /// A new System.Windows.Media.Animation.KeyTime instance, initialized to the value
        /// of timeSpan.
        /// </returns>
        public static KeyTime FromTimeSpan(TimeSpan timeSpan)
        {
            return new KeyTime(timeSpan);
        }

        private TimeSpan _timeSpan;

        /// <summary>
        /// Gets the time when the key frame ends, expressed as a time relative to the
        /// beginning of the animation.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            internal set { _timeSpan = value; }
        }

        ///// <summary>
        ///// Compares two Windows.UI.Xaml.Media.Animation.KeyTime values for inequality.
        ///// </summary>
        ///// <param name="keyTime1">The first value to compare.</param>
        ///// <param name="keyTime2">The second value to compare.</param>
        ///// <returns>true if keyTime1 and keyTime2 are not equal; otherwise, false.</returns>
        //public static bool operator !=(KeyTime keyTime1, KeyTime keyTime2);

        ///// <summary>
        ///// Compares two Windows.UI.Xaml.Media.Animation.KeyTime values for equality.
        ///// </summary>
        ///// <param name="keyTime1">The first value to compare.</param>
        ///// <param name="keyTime2">The second value to compare.</param>
        ///// <returns>true if keyTime1 and keyTime2 are equal; otherwise, false.</returns>
        //public static bool operator ==(KeyTime keyTime1, KeyTime keyTime2);

        ///// <summary>
        ///// Implicitly converts a Windows.UI.Xaml.Media.Animation.KeyTime.TimeSpan to
        ///// a Windows.UI.Xaml.Media.Animation.KeyTime.
        ///// </summary>
        ///// <param name="timeSpan">The Windows.UI.Xaml.Media.Animation.KeyTime.TimeSpan value to convert.</param>
        ///// <returns>The created Windows.UI.Xaml.Media.Animation.KeyTime.</returns>
        //public static implicit operator KeyTime(TimeSpan timeSpan);

        ///// <summary>
        ///// Indicates whether a specified Windows.UI.Xaml.Media.Animation.KeyTime is
        ///// equal to this Windows.UI.Xaml.Media.Animation.KeyTime.
        ///// </summary>
        ///// <param name="value">The Windows.UI.Xaml.Media.Animation.KeyTime to compare with this Windows.UI.Xaml.Media.Animation.KeyTime.</param>
        ///// <returns>
        ///// true if value is equal to this Windows.UI.Xaml.Media.Animation.KeyTime; otherwise,
        ///// false.
        ///// </returns>
        //public bool Equals(KeyTime value);

        ///// <summary>
        ///// Indicates whether a Windows.UI.Xaml.Media.Animation.KeyTime is equal to this
        ///// Windows.UI.Xaml.Media.Animation.KeyTime.
        ///// </summary>
        ///// <param name="value">The Windows.UI.Xaml.Media.Animation.KeyTime to compare with this Windows.UI.Xaml.Media.Animation.KeyTime.</param>
        ///// <returns>
        ///// true if value is a Windows.UI.Xaml.Media.Animation.KeyTime that represents
        ///// the same length of time as this Windows.UI.Xaml.Media.Animation.KeyTime;
        ///// otherwise, false.
        ///// </returns>
        //public override bool Equals(object value);

        ///// <summary>
        ///// Indicates whether two Windows.UI.Xaml.Media.Animation.KeyTime values are
        ///// equal.
        ///// </summary>
        ///// <param name="keyTime1">The first value to compare.</param>
        ///// <param name="keyTime2">The second value to compare.</param>
        ///// <returns>true if the values of keyTime1 and keyTime2 are equal; otherwise, false.</returns>
        //public static bool Equals(KeyTime keyTime1, KeyTime keyTime2);

        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The specified timeSpan is outside the allowed range.
        ///// <summary>
        ///// Creates a new Windows.UI.Xaml.Media.Animation.KeyTime, using the supplied
        ///// System.TimeSpan.
        ///// </summary>
        ///// <param name="timeSpan">The value of the new Windows.UI.Xaml.Media.Animation.KeyTime.</param>
        ///// <returns>
        ///// A new Windows.UI.Xaml.Media.Animation.KeyTime, initialized to the value of
        ///// timeSpan.
        ///// </returns>
        //public static KeyTime FromTimeSpan(TimeSpan timeSpan);

        ///// <summary>
        ///// Returns a hash code representing this Windows.UI.Xaml.Media.Animation.KeyTime.
        ///// </summary>
        ///// <returns>A hash code identifier.</returns>
        //public override int GetHashCode();

        ///// <summary>
        ///// Returns a string representation of this Windows.UI.Xaml.Media.Animation.KeyTime.
        ///// </summary>
        ///// <returns>A string representation of this Windows.UI.Xaml.Media.Animation.KeyTime.</returns>
        //public override string ToString();

        #region Implicit Converters

        /// <summary>
        /// Implicitly creates a KeyTime value from a Time value.
        /// </summary>
        /// <param name="timeSpan">The Time value.</param>
        /// <returns>A new KeyTime.</returns>
        public static implicit operator KeyTime(TimeSpan timeSpan)
        {
            return KeyTime.FromTimeSpan(timeSpan);
        }

        #endregion
    }
}
