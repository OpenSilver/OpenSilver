
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
    /// Describes the width and height of an object.
    /// </summary>
    public partial struct Size
    {
        private double _width;
        private double _height;

        /// <summary>
        /// Initializes a new instance of the Windows.Foundation.Size
        /// structure and assigns it an initial width and height.
        /// </summary>
        /// <param name="width">The initial width of the instance of Windows.Foundation.Size.</param>
        /// <param name="height">The initial height of the instance of Windows.Foundation.Size.</param>
        public Size(double width, double height)
        {
            if(width < 0 || height < 0)
            {
                throw new ArgumentException("Width and Height cannot be negative.");
            }
            this._width = width;
            this._height = height;
        }

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
        /// Compares two instances of Windows.Foundation.Size for
        /// inequality.</summary>
        /// <param name="size1">The first instance of Windows.Foundation.Size to compare.</param>
        /// <param name="size2">The second instance of Windows.Foundation.Size to compare.</param>
        /// <returns>
        /// true if the instances of Windows.Foundation.Size are not equal; otherwise
        /// false.
        /// </returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return size1.Height != size2.Height || size1.Width != size2.Width;
        }

        /// <summary>
        /// Compares two instances of Windows.Foundation.Size for
        /// equality.</summary>
        /// <param name="size1">The first instance of Windows.Foundation.Size to compare.</param>
        /// <param name="size2">The second instance of Windows.Foundation.Size to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.Foundation.Size are equal; otherwise
        /// false.
        /// </returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Height == size2.Height && size1.Width == size2.Width;
        }

        /// <summary>
        /// Gets a value that represents a static empty Windows.Foundation.Size.
        /// </summary>
        public static Size Empty { get; } =
            new Size
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };

        /// <summary>
        /// Gets or sets the height of this instance of Windows.Foundation.Size in pixels. The default is 0. The value cannot be negative.
        /// </summary>
        public double Height
        {
            get
            {
                return this._height;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Cannot modify Empty size.");
                }
                if(value < 0)
                {
                    throw new ArgumentException("Height cannot be negative.");
                }
                this._height = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance of
        /// Windows.Foundation.Size is Windows.Foundation.Size.Empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this._width < 0;
            }
        }

        /// <summary>
        /// Gets or sets the width of this instance of Windows.Foundation.Size.
        /// </summary>
        public double Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Cannot modify Empty size.");
                }
                if(value < 0)
                {
                    throw new ArgumentException("Width cannot be negative.");
                }
                this._width = value;
            }
        }

        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            else
            {
                return size1.Width == size2.Width && size1.Height == size2.Height;
            }
        }

        /// <summary>
        /// Compares an object to an instance of Windows.Foundation.Size
        /// for equality.
        /// </summary>
        /// <param name="o">The System.Object to compare.</param>
        /// <returns>true if the sizes are equal; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if (o is Size value)
            {
               return Size.Equals(this, value);
            }
            return false;
        }


        /// <summary>
        /// Compares a value to an instance of Windows.Foundation.Size
        /// for equality.
        /// </summary>
        /// <param name="value">The size to compare to this current instance of Windows.Foundation.Size.</param>
        /// <returns>true if the instances of Windows.Foundation.Size are equal; otherwise, false.</returns>
        public bool Equals(Size value)
        {
            return Size.Equals(this, value);
        }

        /// <summary>
        /// Gets the hash code for this instance of Windows.Foundation.Size.
        /// </summary>
        /// <returns>The hash code for this instance of Windows.Foundation.Size.</returns>
        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return this.Width.GetHashCode() ^ this.Height.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a string representation of this Windows.Foundation.Size.
        /// </summary>
        /// <returns>A string representation of this Windows.Foundation.Size.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

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
    
    internal static class SizeExtensions
    {
        public static bool IsWidthAuto(this Size size)
        {
            return double.IsNaN(size.Width);
        }

        public static bool IsHeightAuto(this Size size)
        {
            return double.IsNaN(size.Height);
        }

        public static bool IsPartiallyAuto(this Size size)
        {
            return size.IsWidthAuto() || size.IsHeightAuto();
        }

        public static bool IsAuto(this Size size)
        {
            return size.IsHeightAuto() && size.IsHeightAuto();
        }

        public static Size Combine(this Size size, Size fallback)
        {
            if (!(Double.IsNaN(size.Width) || Double.IsNaN(size.Height)))
            {
                return size;
            }

            if (Double.IsNaN(size.Width) && Double.IsNaN(size.Height))
            {
                return fallback;
            }

            return new Size(
                Double.IsNaN(size.Width) ? fallback.Width : size.Width,
                Double.IsNaN(size.Height) ? fallback.Height : size.Height);
        }

        public static Size Bounds(this Size size, Size minimum, Size maximum)
        {
            if (minimum.Width > maximum.Width || minimum.Height > maximum.Height)
            {
                throw new Exception($"Invalid bounds (minimum: {minimum}, maximum: {maximum})");
            }

            return size.Max(minimum).Min(maximum);
        }


        public static bool IsClose(this Size @this, Size size)
        {
            return @this.Width.IsClose(size.Width) && @this.Height.IsClose(size.Height);
        }

        public static Size Min(this Size @this, Size size)
        {
            if (@this.IsAuto())
            {
                return size;
            }

            if (size.IsAuto())
            {
                return @this;
            }

            if (!@this.IsPartiallyAuto() && !@size.IsPartiallyAuto())
            {
                if (@this.Width < size.Width && @this.Height < size.Height)
                {
                    return @this;
                }

                if (@this.Width >= size.Width && @this.Height >= size.Height)
                {
                    return size;
                }
            }

            return new Size(
                @this.IsWidthAuto() ? size.Width : (size.IsWidthAuto() ? @this.Width : Math.Min(@this.Width, size.Width)),
                @this.IsHeightAuto() ? size.Height : (size.IsHeightAuto() ? @this.Height : Math.Min(@this.Height, size.Height)));
        }

        public static Size Max(this Size @this, Size size)
        {
            if (@this.IsAuto())
            {
                return size;
            }

            if (size.IsAuto())
            {
                return @this;
            }

            if (!@this.IsPartiallyAuto() && !@size.IsPartiallyAuto())
            {
                if (@this.Width > size.Width && @this.Height > size.Height)
                {
                    return @this;
                }

                if (@this.Width <= size.Width && @this.Height <= size.Height)
                {
                    return size;
                }
            }

            return new Size(
                @this.IsWidthAuto() ? size.Width : (size.IsWidthAuto() ? @this.Width : Math.Max(@this.Width, size.Width)),
                @this.IsHeightAuto() ? size.Height : (size.IsHeightAuto() ? @this.Height : Math.Max(@this.Height, size.Height)));
        }

        internal static Size Add(this Size left, Size right)
        {
            if (right == new Size())
            {
                return left;
            }

            return new Size(
                left.Width + right.Width,
                left.Height + right.Height
            );
        }

        public static Size Subtract(this Size left, Size right)
        {
            if (right == new Size())
            {
                return left;
            }

            return new Size(
                Math.Max(left.Width - right.Width, 0),
                Math.Max(left.Height - right.Height, 0)
            );
        }
    }
}