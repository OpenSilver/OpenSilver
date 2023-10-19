
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
    /// Describes the width, height, and point origin of a rectangle.
    /// </summary>
    public struct Rect
    {
        internal double _x;
        internal double _y;
        internal double _width;
        internal double _height;

        /// <summary>
        /// Initializes a <see cref="Rect"/> structure that is exactly large enough to contain
        /// the two specified points.
        /// </summary>
        /// <param name="point1">
        /// The first point that the new rectangle must contain.
        /// </param>
        /// <param name="point2">
        /// The second point that the new rectangle must contain.
        /// </param>
        public Rect(Point point1, Point point2)
        {
            _x = Math.Min(point1._x, point2._x);
            _y = Math.Min(point1._y, point2._y);

            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            _width = Math.Max(Math.Max(point1._x, point2._x) - _x, 0);
            _height = Math.Max(Math.Max(point1._y, point2._y) - _y, 0);
        }

        /// <summary>
        /// Initializes a <see cref="Rect"/> structure based on an origin and size.
        /// </summary>
        /// <param name="location">
        /// The origin of the new <see cref="Rect"/>.
        /// </param>
        /// <param name="size">
        /// The size of the new <see cref="Rect"/>.
        /// </param>
        public Rect(Point location, Size size)
        {
            if (size.IsEmpty)
            {
                this = Empty;
            }
            else
            {
                _x = location.X;
                _y = location.Y;
                _width = size.Width;
                _height = size.Height;
            }
        }

        /// <summary>
        /// Initializes a <see cref="Rect"/> structure that has the specified x-coordinate,
        /// y-coordinate, width, and height.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the top-left corner of the rectangle.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the top-left corner of the rectangle.
        /// </param>
        /// <param name="width">
        /// The width of the rectangle.
        /// </param>
        /// <param name="height">
        /// The height of the rectangle.
        /// </param>
        /// <exception cref="ArgumentException">
        /// width or height are less than 0.
        /// </exception>
        public Rect(double x, double y, double width, double height)
        {
            if (width < 0)
            {
                throw new ArgumentException("Width must be non-negative.", nameof(width));
            }

            if (height < 0)
            {
                throw new ArgumentException("Height must be non-negative.", nameof(height));
            }

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> structure that is of the
        /// specified size and is located at (0,0).
        /// </summary>
        /// <param name="size">
        /// The size of the new <see cref="Rect"/>.
        /// </param>
        public Rect(Size size)
        {
            if (size.IsEmpty)
            {
                this = Empty;
            }
            else
            {
                _x = _y = 0;
                _width = size.Width;
                _height = size.Height;
            }
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the <see cref="CultureInfo.InvariantCulture"/>
        /// </summary>
        /// <param name="source">
        /// string with Rect data
        /// </param>
        public static Rect Parse(string source)
        {
            if (source != null)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
                string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 4)
                {
                    return new Rect(
                        Convert.ToDouble(split[0], formatProvider),
                        Convert.ToDouble(split[1], formatProvider),
                        Convert.ToDouble(split[2], formatProvider),
                        Convert.ToDouble(split[3], formatProvider)
                    );
                }
            }

            throw new FormatException($"'{source}' is not an eligible value for '{typeof(Rect)}'.");
        }

        /// <summary>
        /// Gets a special value that represents a rectangle with no position or area.
        /// </summary>
        /// <returns>
        /// The empty rectangle, which has <see cref="X"/> and <see cref="Y"/> property values 
        /// of <see cref="double.PositiveInfinity"/>, and has <see cref="Width"/> and 
        /// <see cref="Height"/> property values of <see cref="double.NegativeInfinity"/>.
        /// </returns>
        public static Rect Empty { get; } =
            new Rect
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity,
                _x = double.PositiveInfinity,
                _y = double.PositiveInfinity
            };

        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle.
        /// </summary>
        /// <returns>
        /// The y-axis value of the bottom of the rectangle. If the rectangle is empty, the
        /// value is <see cref="double.NegativeInfinity"/>.
        /// </returns>
        public double Bottom
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }

                return _y + _height;
            }
        }

        /// <summary>
        /// Gets the position of the bottom-left corner of the rectangle.
        /// If this is the empty rectangle, the value will be positive infinity, negative infinity.
        /// </summary>
        public Point BottomLeft => new Point(Left, Bottom);

        /// <summary>
        /// Gets the position of the bottom-right corner of the rectangle.
        /// </summary>
        public Point BottomRight => new Point(Right, Bottom);

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        /// <returns>
        /// A value that represents the height of the rectangle. The default is 0.
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
                    throw new ArgumentException("Height cannot be lower than 0");
                }

                _height = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the rectangle is the <see cref="Empty"/>
        /// rectangle.
        /// </summary>
        /// <returns>
        /// true if the rectangle is the <see cref="Empty"/> rectangle; otherwise, false.
        /// </returns>
        public bool IsEmpty => _width < 0;


        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle.
        /// </summary>
        /// <returns>
        /// The x-axis value of the left side of the rectangle.
        /// </returns>
        public double Left => _x;

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        /// <returns>
        /// The x-axis value of the right side of the rectangle.
        /// </returns>
        public double Right
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }

                return _x + _width;
            }
        }

        /// <summary>
        /// Gets the y-axis position of the top of the rectangle.
        /// </summary>
        /// <returns>
        /// The y-axis position of the top of the rectangle.
        /// </returns>
        public double Top => _y;

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        /// <returns>
        /// A value that represents the width of the rectangle in pixels. The default is 0.
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
                    throw new ArgumentException("Width cannot be lower than 0");
                }

                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets the x-axis value of the left side of the rectangle.
        /// </summary>
        /// <returns>
        /// The x-axis value of the left side of the rectangle. This value is interpreted
        /// as pixels within the coordinate space.
        /// </returns>
        public double X
        {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// Gets or sets the y-axis value of the top side of the rectangle.
        /// </summary>
        /// <returns>
        /// The y-axis value of the top side of the rectangle. This value is interpreted
        /// as pixels within the coordinate space.
        /// </returns>
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// Gets or sets the position of the top-left corner of the rectangle.
        /// </summary>
        public Point Location
        {
            get => new Point(_x, _y);
            set
            {
                _x = value._x;
                _y = value._y;
            }
        }

        /// <summary>
        /// Gets or sets the width and height of the rectangle.
        /// </summary>
        public Size Size
        {
            get => new Size(_width, _height);
            set
            {
                _width = value.Width;
                _height = value.Height;
            }
        }

        /// <summary>
        /// Gets the position of the top-left corner of the rectangle
        /// </summary>
        public Point TopLeft => new Point(Left, Top);

        /// <summary>
        /// Gets the position of the top-right corner of the rectangle.
        /// </summary>
        public Point TopRight => new Point(Right, Top);

        /// <summary>
        /// Indicates whether the rectangle described by the <see cref="Rect"/> contains
        /// the specified point.
        /// </summary>
        /// <param name="point">
        /// The point to check.
        /// </param>
        /// <returns>
        /// true if the rectangle described by the <see cref="Rect"/> contains the specified
        /// point; otherwise, false.
        /// </returns>
        public bool Contains(Point point) => ContainsInternal(point._x, point._y);

        /// <summary>
        /// Finds the intersection of the rectangle represented by the current <see cref="Rect"/>
        /// and the rectangle represented by the specified <see cref="Rect"/>, and stores
        /// the result as the current <see cref="Rect"/>.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to intersect with the current rectangle.
        /// </param>
        public void Intersect(Rect rect)
        {
            if (!IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                double left = Math.Max(Left, rect.Left);
                double top = Math.Max(Top, rect.Top);

                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0.0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0.0);

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Expands the rectangle represented by the current <see cref="Rect"/> exactly
        /// enough to contain the specified point.
        /// </summary>
        /// <param name="point">
        /// The point to include.
        /// </param>
        public void Union(Point point) => Union(new Rect(point, point));

        /// <summary>
        /// Expands the rectangle represented by the current <see cref="Rect"/> exactly
        /// enough to contain the specified rectangle.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to include.
        /// </param>
        public void Union(Rect rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                double left = Math.Min(Left, rect.Left);
                double top = Math.Min(Top, rect.Top);

                // We need this check so that the math does not result in NaN
                if ((rect.Width == double.PositiveInfinity) || (Width == double.PositiveInfinity))
                {
                    _width = double.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)                    
                    double maxRight = Math.Max(Right, rect.Right);
                    _width = Math.Max(maxRight - left, 0);
                }

                // We need this check so that the math does not result in NaN
                if ((rect.Height == double.PositiveInfinity) || (Height == double.PositiveInfinity))
                {
                    _height = double.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                    double maxBottom = Math.Max(Bottom, rect.Bottom);
                    _height = Math.Max(maxBottom - top, 0);
                }

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Indicates whether the specified object is equal to the current <see cref="Rect"/>.
        /// </summary>
        /// <param name="o">
        /// The object to compare to the current rectangle.
        /// </param>
        /// <returns>
        /// true if o is a <see cref="Rect"/> and has the same x,y,width,height values as
        /// the current <see cref="Rect"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object o) => o is Rect rect && rect == this;

        /// <summary>
        /// Indicates whether the specified <see cref="Rect"/> is equal to the current <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">
        /// The rectangle to compare to the current rectangle.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="Rect"/> has the same x,y,width,height property
        /// values as the current <see cref="Rect"/>; otherwise, false.
        /// </returns>
        public bool Equals(Rect value) => value == this;

        /// <summary>
        /// Creates a hash code for the <see cref="Rect"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Rect"/> structure.
        /// </returns>
        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Rect"/> structure.
        /// structure.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Rect"/> structure. The string
        /// has the following form: "<see cref="X"/>, <see cref="Y"/>, <see cref="Width"/>, <see cref="Height"/>".
        /// </returns>
        public override string ToString() => ConvertToString(null);

        /// <summary>
        /// Returns a string representation of the rectangle by using the specified format
        /// provider.
        /// </summary>
        /// <param name="provider">
        /// Culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A string representation of the current rectangle that is determined by the specified
        /// format provider.
        /// </returns>
        public string ToString(IFormatProvider provider) => ConvertToString(provider);

        /// <summary>
        /// Compares two <see cref="Rect"/> structures for inequality.
        /// </summary>
        /// <param name="rect1">
        /// The first rectangle to compare.
        /// </param>
        /// <param name="rect2">
        /// The second rectangle to compare.
        /// </param>
        /// <returns>
        /// true if the <see cref="Rect"/> structures do not have the same x,y,width,height
        /// property values; otherwise, false.
        /// </returns>
        public static bool operator !=(Rect rect1, Rect rect2) => !(rect1 == rect2);

        /// <summary>
        /// Compares two <see cref="Rect"/> structures for equality.
        /// </summary>
        /// <param name="rect1">
        /// The first rectangle to compare.
        /// </param>
        /// <param name="rect2">
        /// The second rectangle to compare.
        /// </param>
        /// <returns>
        /// true if the <see cref="Rect"/> structures have the same x,y,width,height property
        /// values; otherwise, false.
        /// </returns>
        public static bool operator ==(Rect rect1, Rect rect2)
            => rect1.X == rect2.X &&
               rect1.Y == rect2.Y &&
               rect1.Width == rect2.Width &&
               rect1.Height == rect2.Height;

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
                                "{1}{0}{2}{0}{3}{0}{4}",
                                separator,
                                _x,
                                _y,
                                _width,
                                _height);
        }

        /// <summary>
        /// ContainsInternal - Performs just the "point inside" logic
        /// </summary>
        /// <returns>
        /// bool - true if the point is inside the rect
        /// </returns>
        /// <param name="x"> The x-coord of the point to test </param>
        /// <param name="y"> The y-coord of the point to test </param>
        private bool ContainsInternal(double x, double y)
        {
            // We include points on the edge as "contained".
            // We do "x - _width <= _x" instead of "x <= _x + _width"
            // so that this check works when _width is PositiveInfinity
            // and _x is NegativeInfinity.
            return (x >= _x) && (x - _width <= _x) &&
                    (y >= _y) && (y - _height <= _y);
        }

        /// <summary>
        /// Checks if the current <see cref="Rect" /> intersects with the specified <see cref="Rect" />.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to intersect with the current rectangle.
        /// </param>
        /// <returns>
        /// true if the current rectangle intersects with the specified rectangle, false otherwise
        /// </returns>
        private bool IntersectsWith(Rect rect)
            => (rect.Left <= Right) &&
               (rect.Right >= Left) &&
               (rect.Top <= Bottom) &&
               (rect.Bottom >= Top);
    }
}