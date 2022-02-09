
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

using System.Collections.Generic;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the short time format used for parsing and formatting.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class ShortTimeFormat : ITimeFormat
    {
        public string GetTimeDisplayFormat(CultureInfo culture)
        {
            DateTimeFormatInfo info = culture.DateTimeFormat;
            return info.ShortTimePattern;
        }

        public string[] GetTimeParseFormats(CultureInfo culture)
        {
            var formats = new List<string>(6);

            DateTimeFormatInfo info = culture.DateTimeFormat;
            if (info != null)
            {
                formats.Add(info.ShortTimePattern);
                formats.Add(info.LongTimePattern);
            }
            return formats.ToArray();
        }

#region Equals and hashcode.
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is 
        /// equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare 
        /// with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// True if the specified <see cref="T:System.Object"/> is equal 
        /// to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            ShortTimeFormat comparison = obj as ShortTimeFormat;
            return comparison != null && comparison == this;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ShortTimeFormat left, ShortTimeFormat right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ShortTimeFormat left, ShortTimeFormat right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode();
        }
#endregion
    }
}
