
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
    public sealed class LongTimeFormat : ITimeFormat
    {
        /// <summary>
        /// Gets the format to use to display a DateTime as a time value.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>A format to use during display of a DateTime.</returns>
        public string GetTimeDisplayFormat(CultureInfo culture)
        {
            return culture.DateTimeFormat.LongTimePattern;
        }

        /// <summary>
        /// Gets the formats to use to parse a string to a DateTime.
        /// </summary>
        /// <param name="culture">Culture used to determine formats.</param>
        /// <returns>An array of formats to be used during parsing.</returns>
        public string[] GetTimeParseFormats(CultureInfo culture)
        {
            List<string> stringList = new List<string>(6);
            DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;
            if (dateTimeFormat != null)
            {
                stringList.Add(dateTimeFormat.LongTimePattern);
                stringList.Add(dateTimeFormat.ShortTimePattern);
            }
            return stringList.ToArray();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is
        /// equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare
        /// with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// True if the specified <see cref="T:System.Object" /> is equal
        /// to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            LongTimeFormat longTimeFormat = obj as LongTimeFormat;
            return longTimeFormat != (LongTimeFormat)null && longTimeFormat == this;
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(LongTimeFormat left, LongTimeFormat right)
        {
            return (object)left == null && (object)right == null || (object)left != null && (object)right != null;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(LongTimeFormat left, LongTimeFormat right)
        {
            return !(left == right);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return this.GetType().Name.GetHashCode();
        }
    }
}
