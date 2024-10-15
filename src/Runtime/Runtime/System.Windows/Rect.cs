
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

using System.Globalization;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Describes the width, height, and point origin of a rectangle.
/// </summary>
public struct Rect : IFormattable
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
    /// Initializes a new instance of the <see cref="Rect"/> structure that is exactly
    /// large enough to contain the specified point and the sum of the specified point
    /// and the specified vector.
    /// </summary>
    /// <param name="point">
    /// The first point the rectangle must contain.
    /// </param>
    /// <param name="vector">
    /// The amount to offset the specified point. The resulting rectangle will be exactly
    /// large enough to contain both points.
    /// </param>
    public Rect(Point point, Vector vector)
        : this(point, point + vector)
    {
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
            throw new ArgumentException(Strings.Size_WidthAndHeightCannotBeNegative, nameof(width));
        }

        if (height < 0)
        {
            throw new ArgumentException(Strings.Size_WidthAndHeightCannotBeNegative, nameof(height));
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
    /// Creates a new rectangle from the specified string representation.
    /// </summary>
    /// <param name="source">
    /// The string representation of the rectangle, in the form "x, y, width, height".
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
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
                throw new ArgumentException(Strings.Size_HeightCannotBeNegative);
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
                throw new ArgumentException(Strings.Size_WidthCannotBeNegative);
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
    /// Creates a rectangle that results from expanding or shrinking the specified rectangle
    /// by the specified width and height amounts, in all directions.
    /// </summary>
    /// <param name="rect">
    /// The System.Windows.Rect structure to modify.
    /// </param>
    /// <param name="width">
    /// The amount by which to expand or shrink the left and right sides of the rectangle.
    /// </param>
    /// <param name="height">
    /// The amount by which to expand or shrink the top and bottom sides of the rectangle.
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// rect is an System.Windows.Rect.Empty rectangle.
    /// </exception>
    public static Rect Inflate(Rect rect, double width, double height)
    {
        rect.Inflate(width, height);
        return rect;
    }

    /// <summary>
    /// Returns the rectangle that results from expanding the specified rectangle by
    /// the specified <see cref="Windows.Size"/>, in all directions.
    /// </summary>
    /// <param name="rect">
    /// The <see cref="Rect"/> structure to modify.
    /// </param>
    /// <param name="size">
    /// Specifies the amount to expand the rectangle. The <see cref="Windows.Size"/> structure's
    /// <see cref="Size.Width"/> property specifies the amount to increase the rectangle's
    /// <see cref="Left"/> and <see cref="Right"/> properties. The <see cref="Windows.Size"/>
    /// structure's <see cref="Size.Height"/> property specifies the amount to increase
    /// the rectangle's <see cref="Top"/> and <see cref="Bottom"/> properties.
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// rect is an <see cref="Empty"/> rectangle.
    /// </exception>
    public static Rect Inflate(Rect rect, Size size)
    {
        rect.Inflate(size._width, size._height);
        return rect;
    }

    /// <summary>
    /// Returns the intersection of the specified rectangles.
    /// </summary>
    /// <param name="rect1">
    /// The first rectangle to compare.
    /// </param>
    /// <param name="rect2">
    /// The second rectangle to compare.
    /// </param>
    /// <returns>
    /// The intersection of the two rectangles, or <see cref="Empty"/> if no intersection exists.
    /// </returns>
    public static Rect Intersect(Rect rect1, Rect rect2)
    {
        rect1.Intersect(rect2);
        return rect1;
    }

    /// <summary>
    /// Returns a rectangle that is offset from the specified rectangle by using the specified vector.
    /// </summary>
    /// <param name="rect">
    /// The original rectangle.
    /// </param>
    /// <param name="offsetVector">
    /// A vector that specifies the horizontal and vertical offsets for the new rectangle.
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// rect is <see cref="Empty"/>.
    /// </exception>
    public static Rect Offset(Rect rect, Vector offsetVector)
    {
        rect.Offset(offsetVector.X, offsetVector.Y);
        return rect;
    }

    /// <summary>
    /// Returns a rectangle that is offset from the specified rectangle by using the
    /// specified horizontal and vertical amounts.
    /// </summary>
    /// <param name="rect">
    /// The rectangle to move.
    /// </param>
    /// <param name="offsetX">
    /// The horizontal offset for the new rectangle.
    /// </param>
    /// <param name="offsetY">
    /// The vertical offset for the new rectangle.
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// rect is <see cref="Empty"/>.
    /// </exception>
    public static Rect Offset(Rect rect, double offsetX, double offsetY)
    {
        rect.Offset(offsetX, offsetY);
        return rect;
    }

    /// <summary>
    /// Returns the rectangle that results from applying the specified matrix to the specified rectangle.
    /// </summary>
    /// <param name="rect">
    /// A rectangle that is the basis for the transformation.
    /// </param>
    /// <param name="matrix">
    /// A matrix that specifies the transformation to apply.
    /// </param>
    /// <returns>
    /// The rectangle that results from the operation.
    /// </returns>
    public static Rect Transform(Rect rect, Matrix matrix)
    {
        MatrixUtil.TransformRect(ref rect, ref matrix);
        return rect;
    }

    /// <summary>
    /// Creates a rectangle that is exactly large enough to include the specified rectangle and the specified point.
    /// </summary>
    /// <param name="rect">
    /// The rectangle to include.
    /// </param>
    /// <param name="point">
    /// The point to include.
    /// </param>
    /// <returns>
    /// A rectangle that is exactly large enough to contain the specified rectangle and the specified point.
    /// </returns>
    public static Rect Union(Rect rect, Point point)
    {
        rect.Union(new Rect(point, point));
        return rect;
    }

    /// <summary>
    /// Creates a rectangle that is exactly large enough to contain the two specified rectangles.
    /// </summary>
    /// <param name="rect1">
    /// The first rectangle to include.
    /// </param>
    /// <param name="rect2">
    /// The second rectangle to include.
    /// </param>
    /// <returns>
    /// The resulting rectangle.
    /// </returns>
    public static Rect Union(Rect rect1, Rect rect2)
    {
        rect1.Union(rect2);
        return rect1;
    }

    /// <summary>
    /// Indicates whether the rectangle contains the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the point to check.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the point to check.
    /// </param>
    /// <returns>
    /// true if (x, y) is contained by the rectangle; otherwise, false.
    /// </returns>
    public bool Contains(double x, double y)
    {
        if (IsEmpty)
        {
            return false;
        }

        return ContainsInternal(x, y);
    }

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
    public bool Contains(Point point) => Contains(point._x, point._y);

    /// <summary>
    /// Indicates whether the rectangle contains the specified rectangle.
    /// </summary>
    /// <param name="rect">
    /// The rectangle to check.
    /// </param>
    /// <returns>
    /// true if rect is entirely contained by the rectangle; otherwise, false.
    /// </returns>
    public bool Contains(Rect rect)
    {
        if (IsEmpty || rect.IsEmpty)
        {
            return false;
        }

        return _x <= rect._x &&
               _y <= rect._y &&
               _x + _width >= rect._x + rect._width &&
               _y + _height >= rect._y + rect._height;
    }

    /// <summary>
    /// Expands the rectangle by using the specified <see cref="Windows.Size"/>, in all directions.
    /// </summary>
    /// <param name="size">
    /// Specifies the amount to expand the rectangle. The <see cref="Windows.Size"/> structure's
    /// <see cref="Size.Width"/> property specifies the amount to increase the rectangle's
    /// <see cref="Left"/> and <see cref="Right"/> properties. The <see cref="Windows.Size"/>
    /// structure's <see cref="Size.Height"/> property specifies the amount to increase
    /// the rectangle's <see cref="Top"/> and <see cref="Bottom"/> properties.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// This method is called on the <see cref="Empty"/> rectangle.
    /// </exception>
    public void Inflate(Size size) => Inflate(size._width, size._height);

    /// <summary>
    /// Expands or shrinks the rectangle by using the specified width and height amounts,
    /// in all directions.
    /// </summary>
    /// <param name="width">
    /// The amount by which to expand or shrink the left and right sides of the rectangle.
    /// </param>
    /// <param name="height">
    /// The amount by which to expand or shrink the top and bottom sides of the rectangle.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// This method is called on the <see cref="Empty"/> rectangle.
    /// </exception>
    public void Inflate(double width, double height)
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(Strings.Rect_CannotCallMethod);
        }

        _x -= width;
        _y -= height;

        // Do two additions rather than multiplication by 2 to avoid spurious overflow
        // That is: (A + 2 * B) != ((A + B) + B) if 2*B overflows.
        // Note that multiplication by 2 might work in this case because A should start
        // positive & be "clamped" to positive after, but consider A = Inf & B = -MAX.
        _width += width;
        _width += width;
        _height += height;
        _height += height;

        // We catch the case of inflation by less than -width/2 or -height/2 here.  This also
        // maintains the invariant that either the Rect is Empty or _width and _height are
        // non-negative, even if the user parameters were NaN, though this isn't strictly maintained
        // by other methods.
        if (!(_width >= 0 && _height >= 0))
        {
            this = Empty;
        }
    }

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
    /// Indicates whether the specified rectangle intersects with the current rectangle.
    /// </summary>
    /// <param name="rect">
    /// The rectangle to check.
    /// </param>
    /// <returns>
    /// true if the specified rectangle intersects with the current rectangle; otherwise, false.
    /// </returns>
    public bool IntersectsWith(Rect rect)
    {
        if (IsEmpty || rect.IsEmpty)
        {
            return false;
        }

        return (rect.Left <= Right) &&
               (rect.Right >= Left) &&
               (rect.Top <= Bottom) &&
               (rect.Bottom >= Top);
    }

    /// <summary>
    /// Moves the rectangle by the specified horizontal and vertical amounts.
    /// </summary>
    /// <param name="offsetX">
    /// The amount to move the rectangle horizontally.
    /// </param>
    /// <param name="offsetY">
    /// The amount to move the rectangle vertically.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// This method is called on the <see cref="Empty"/> rectangle.
    /// </exception>
    public void Offset(double offsetX, double offsetY)
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(Strings.Rect_CannotCallMethod);
        }

        _x += offsetX;
        _y += offsetY;
    }

    /// <summary>
    /// Moves the rectangle by the specified vector.
    /// </summary>
    /// <param name="offsetVector">
    /// A vector that specifies the horizontal and vertical amounts to move the rectangle.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// This method is called on the <see cref="Empty"/> rectangle.
    /// </exception>
    public void Offset(Vector offsetVector)
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(Strings.Rect_CannotCallMethod);
        }

        _x += offsetVector._x;
        _y += offsetVector._y;
    }

    /// <summary>
    /// Multiplies the size of the current rectangle by the specified x and y values.
    /// </summary>
    /// <param name="scaleX">
    /// The scale factor in the x-direction.
    /// </param>
    /// <param name="scaleY">
    /// The scale factor in the y-direction.
    /// </param>
    public void Scale(double scaleX, double scaleY)
    {
        if (IsEmpty)
        {
            return;
        }

        _x *= scaleX;
        _y *= scaleY;
        _width *= scaleX;
        _height *= scaleY;

        // If the scale in the X dimension is negative, we need to normalize X and Width
        if (scaleX < 0)
        {
            // Make X the left-most edge again
            _x += _width;

            // and make Width positive
            _width *= -1;
        }

        // Do the same for the Y dimension
        if (scaleY < 0)
        {
            // Make Y the top-most edge again
            _y += _height;

            // and make Height positive
            _height *= -1;
        }
    }

    /// <summary>
    /// Transforms the rectangle by applying the specified matrix.
    /// </summary>
    /// <param name="matrix">
    /// A matrix that specifies the transformation to apply.
    /// </param>
    public void Transform(Matrix matrix) => MatrixUtil.TransformRect(ref this, ref matrix);

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
    /// Indicates whether the specified rectangles are equal.
    /// </summary>
    /// <param name="rect1">
    /// The first rectangle to compare.
    /// </param>
    /// <param name="rect2">
    /// The second rectangle to compare.
    /// </param>
    /// <returns>
    /// true if the rectangles have the same <see cref="Location"/> and <see cref="Size"/> values; otherwise, false.
    /// </returns>
    public static bool Equals(Rect rect1, Rect rect2)
    {
        if (rect1.IsEmpty)
        {
            return rect2.IsEmpty;
        }
        else
        {
            return rect1.X.Equals(rect2.X) &&
                   rect1.Y.Equals(rect2.Y) &&
                   rect1.Width.Equals(rect2.Width) &&
                   rect1.Height.Equals(rect2.Height);
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
    public override bool Equals(object o) => o is Rect rect && this == rect;

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
    public bool Equals(Rect value) => this == value;

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
    public override string ToString() => ConvertToString(null, null);

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
    public string ToString(IFormatProvider provider) => ConvertToString(null, provider);

    /// <summary>
    /// Creates a string representation of this object based on the format string
    /// and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// See the documentation for IFormattable for more information.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    string IFormattable.ToString(string format, IFormatProvider provider) => ConvertToString(format, provider);

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
        if (IsEmpty)
        {
            return "Empty";
        }

        // Helper to get the numeric list separator for a given culture.
        char separator = TokenizerHelper.GetNumericListSeparator(provider);
        return string.Format(provider,
                             "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}",
                             separator,
                             _x,
                             _y,
                             _width,
                             _height);
    }

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
}