// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a single time format used for parsing and formatting.
    /// </summary>
    public sealed class CustomTimeFormat : ITimeFormat
    {
        /// <summary>
        /// Gets or sets the custom format that is used to parse or display
        /// a String or DateTime.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimeFormat"/> class.
        /// </summary>
        /// <param name="format">The format that is used to parse or display
        /// a String or DateTime.</param>
        public CustomTimeFormat(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimeFormat"/> class.
        /// </summary>
        public CustomTimeFormat()
        {
        }

        /// <summary>
        /// Gets the format to use to display a DateTime as a time value.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// A format to use during display of a DateTime.
        /// </returns>
        public string GetTimeDisplayFormat(CultureInfo culture)
        {
            return Format;
        }

        /// <summary>
        /// Gets the format to use to parse a string to a DateTime.
        /// </summary>
        /// <param name="culture">Culture used to determine formats.</param>
        /// <returns>
        /// An array of formats to be used during parsing.
        /// </returns>
        public string[] GetTimeParseFormats(CultureInfo culture)
        {
            List<string> formats = new List<string>(7) { Format };

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
            CustomTimeFormat comparison = obj as CustomTimeFormat;
            return comparison != null && comparison == this;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(CustomTimeFormat left, CustomTimeFormat right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Format == right.Format;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(CustomTimeFormat left, CustomTimeFormat right)
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
            return Format.GetHashCode();
        }
        #endregion
    }
}
