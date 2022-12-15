
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
using System.Globalization;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional space. Can also represent
    /// a logical point for certain property usages.
    /// </summary>
    public struct Point : IFormattable
    {
        internal double _x;
        internal double _y;

        /// <summary>
        /// Initializes a <see cref="Point"/> structure that contains the specified values.
        /// contains the specified values.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate value of the <see cref="Point"/> structure.
        /// </param>
        /// <param name="y">
        /// The y-coordinate value of the <see cref="Point"/> structure.
        /// </param>
        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="source">
        /// string with Point data
        /// </param>
        public static Point Parse(string source)
        {
            if (source != null)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
                string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2)
                {
                    return new Point(
                        Convert.ToDouble(split[0], formatProvider),
                        Convert.ToDouble(split[1], formatProvider)
                    );
                }
            }

            throw new FormatException($"'{source}' is not an eligible value for a '{typeof(Point)}'.");
        }

        /// <summary>
        /// Gets or sets the <see cref="X"/>-coordinate value of this <see cref="Point"/>
        /// structure.
        /// </summary>
        /// <returns>
        /// The <see cref="X"/>-coordinate value of this <see cref="Point"/> structure.
        /// The default value is 0.
        /// </returns>
        public double X
        {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Y"/>-coordinate value of this <see cref="Point"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Y"/>-coordinate value of this <see cref="Point"/> structure.
        /// The default value is 0.
        /// </returns>
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// Determines whether the specified object is a <see cref="Point"/> and whether
        /// it contains the same values as this <see cref="Point"/>.
        /// </summary>
        /// <param name="o">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// true if obj is a <see cref="Point"/> and contains the same <see cref="X"/>
        /// and <see cref="Y"/> values as this <see cref="Point"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object o) => o is Point point && point == this;

        /// <summary>
        /// Compares two <see cref="Point"/> structures for equality.
        /// </summary>
        /// <param name="value">
        /// The point to compare to this instance.
        /// </param>
        /// <returns>
        /// true if both <see cref="Point"/> structures contain the same <see cref="X"/>
        /// and <see cref="Y"/> values; otherwise, false.
        /// </returns>
        public bool Equals(Point value) => this == value;

        /// <summary>
        /// Returns the hash code for this <see cref="Point"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="Point"/> structure.
        /// </returns>
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Point"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> containing the <see cref="X"/> and <see cref="Y"/>
        /// values of this <see cref="Point"/> structure.
        /// </returns>
        public override string ToString()
            => ConvertToString(null /* format string */, null /* format provider */);

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Point"/>.
        /// </summary>
        /// <param name="provider">
        /// Culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the <see cref="X"/> and <see cref="Y"/>
        /// values of this <see cref="Point"/> structure.
        /// </returns>
        public string ToString(IFormatProvider provider)
            => ConvertToString(null /* format string */, provider);

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
            => ConvertToString(format, provider);

        /// <summary>
        /// Compares two <see cref="Point"/> structures for equality.
        /// </summary>
        /// <param name="point1">
        /// The first <see cref="Point"/> structure to compare.
        /// </param>
        /// <param name="point2">
        /// The second <see cref="Point"/> structure to compare.
        /// </param>
        /// <returns>
        /// true if both the <see cref="X"/> and <see cref="Y"/> values of
        /// point1 and point2 are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(Point point1, Point point2)
            => point1.X == point2.X &&
               point1.Y == point2.Y;

        /// <summary>
        /// Compares two <see cref="Point"/> structures for inequality
        /// </summary>
        /// <param name="point1">
        /// The first point to compare.
        /// </param>
        /// <param name="point2">
        /// The second point to compare.
        /// </param>
        /// <returns>
        /// true if point1 and point2 have different <see cref="X"/> or <see cref="Y"/>
        /// values; false if point1 and point2 have the same <see cref="X"/> and <see cref="Y"/>
        /// values.
        /// </returns>
        public static bool operator !=(Point point1, Point point2)
            => !(point1 == point2);

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            // Helper to get the numeric list separator for a given culture.
            char separator = TokenizerHelper.GetNumericListSeparator(provider);

            return string.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}",
                                 separator,
                                 _x,
                                 _y);
        }
    }
}