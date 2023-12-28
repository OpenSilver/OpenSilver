
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

namespace System.Windows
{
    /// <summary>
    /// Describes the width and height of an object.
    /// </summary>
    public struct Size
    {
        internal double _width;
        internal double _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> structure and assigns it
        /// an initial width and height.
        /// </summary>
        /// <param name="width">
        /// The initial width of the instance of <see cref="Size"/>.
        /// </param>
        /// <param name="height">
        /// The initial height of the instance of <see cref="Size"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// width or height are less than 0.
        /// </exception>
        public Size(double width, double height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Width and Height cannot be negative.");
            }

            _width = width;
            _height = height;
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the <see cref="CultureInfo.InvariantCulture"/>.
        /// <param name="source">
        /// string with Size data
        /// </param>
        /// </summary>
        public static Size Parse(string source)
        {
            if (source != null)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
                string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2)
                {
                    return new Size(
                        Convert.ToDouble(split[0], formatProvider),
                        Convert.ToDouble(split[1], formatProvider)
                    );
                }
            }

            throw new FormatException($"'{source}' is not an eligible value for a '{typeof(Size)}'.");
        }

        /// <summary>
        /// Gets a value that represents a static empty <see cref="Size"/>.
        /// </summary>
        /// <returns>
        /// An empty instance of <see cref="Size"/>.
        /// </returns>
        public static Size Empty { get; } =
            new Size
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };

        /// <summary>
        /// Gets a value that indicates whether this instance of <see cref="Size"/> is <see cref="Empty"/>.
        /// </summary>
        public bool IsEmpty => _width < 0;

        /// <summary>
        /// Gets or sets the height of this instance of <see cref="Size"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Height"/> of this instance of <see cref="Size"/>, in pixels.
        /// The default is 0. The value cannot be negative.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Specified a value less than 0.
        /// </exception>
        public double Height
        {
            get => _height;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Height cannot be negative.");
                }

                _height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of this instance of <see cref="Size"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Width"/> of this instance of <see cref="Size"/>, in pixels.
        /// The default value is 0. The value cannot be negative.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Specified a value less than 0.
        /// </exception>
        public double Width
        {
            get => _width;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Width cannot be negative.");
                }

                _width = value;
            }
        }

        /// <summary>
        /// Compares an object to an instance of <see cref="Size"/> for equality.
        /// </summary>
        /// <param name="o">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// true if the sizes are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object o) => o is Size size && size == this;

        /// <summary>
        /// Compares a value to an instance of <see cref="Size"/> for equality.
        /// </summary>
        /// <param name="value">
        /// The size to compare to this current instance of <see cref="Size"/>.
        /// </param>
        /// <returns>
        /// true if the instances of <see cref="Size"/> are equal; otherwise, false.
        /// </returns>
        public bool Equals(Size value) => this == value;

        /// <summary>
        /// Gets the hash code for this instance of <see cref="Size"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this instance of <see cref="Size"/>.
        /// </returns>
        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return Width.GetHashCode() ^ Height.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Size"/>.
        /// </summary>
        /// <returns>
        /// A string representation of this <see cref="Size"/>.
        /// </returns>
        public override string ToString() => ConvertToString(null);

        /// <summary>
        /// Compares two instances of <see cref="Size"/> for equality.
        /// </summary>
        /// <param name="size1">
        /// The first instance of <see cref="Size"/> to compare.
        /// </param>
        /// <param name="size2">
        /// The second instance of <see cref="Size"/> to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="Size"/> are equal; otherwise false.
        /// </returns>
        public static bool operator ==(Size size1, Size size2)
            => size1.Width == size2.Width && size1.Height == size2.Height;

        /// <summary>
        /// Compares two instances of <see cref="Size"/> for inequality.
        /// </summary>
        /// <param name="size1">
        /// The first instance of <see cref="Size"/> to compare.
        /// </param>
        /// <param name="size2">
        /// The second instance of <see cref="Size"/> to compare.
        /// </param>
        /// <returns>
        /// true if the instances of <see cref="Size"/> are not equal; otherwise false.
        /// </returns>
        public static bool operator !=(Size size1, Size size2) => !(size1 == size2);

        public static bool Equals(Size size1, Size size2) => size1 == size2;

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(IFormatProvider provider)
        {
            if (IsEmpty)
            {
                return "Empty";
            }

            // Helper to get the numeric list separator for a given culture.
            char separator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider,
                                "{1}{0}{2}",
                                separator,
                                _width,
                                _height);
        }
    }
}