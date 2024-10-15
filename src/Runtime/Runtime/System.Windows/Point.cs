
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
    /// Constructs a <see cref="Point"/> from the specified <see cref="string"/>.
    /// </summary>
    /// <param name="source">
    /// A string representation of a point.
    /// </param>
    /// <returns>
    /// The equivalent <see cref="Point"/> structure.
    /// </returns>
    /// <exception cref="FormatException">
    /// source is not composed of two comma- or space-delimited double values.
    /// </exception>
    public static Point Parse(string source)
    {
        IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        var th = new TokenizerHelper(source, formatProvider);

        var point = new Point(
            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
            Convert.ToDouble(th.NextTokenRequired(), formatProvider));

        // There should be no more tokens in this string.
        th.LastTokenRequired();

        return point;
    }

    /// <summary>
    /// Adds a <see cref="Vector"/> to a <see cref="Point"/> and returns the result as a <see cref="Point"/> structure.
    /// </summary>
    /// <param name="point">
    /// The <see cref="Point"/> structure to add.
    /// </param>
    /// <param name="vector">
    /// The <see cref="Vector"/> structure to add.
    /// </param>
    /// <returns>
    /// Returns the sum of point and vector.
    /// </returns>
    public static Point Add(Point point, Vector vector) => new(point._x + vector._x, point._y + vector._y);

    /// <summary>
    /// Transforms the specified <see cref="Point"/> structure by the specified <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="point">
    /// The point to transform.
    /// </param>
    /// <param name="matrix">
    /// The transformation matrix.
    /// </param>
    /// <returns>
    /// The transformed point.
    /// </returns>
    public static Point Multiply(Point point, Matrix matrix) => matrix.Transform(point);

    /// <summary>
    /// Subtracts the specified <see cref="Point"/> from another specified <see cref="Point"/> and returns the 
    /// difference as a <see cref="Vector"/>.
    /// </summary>
    /// <param name="point1">
    /// The point from which point2 is subtracted.
    /// </param>
    /// <param name="point2">
    /// The point to subtract from point1.
    /// </param>
    /// <returns>
    /// The difference between point1 and point2.
    /// </returns>
    public static Vector Subtract(Point point1, Point point2) => new(point1._x - point2._x, point1._y - point2._y);

    /// <summary>
    /// Subtracts the specified <see cref="Vector"/> from the specified <see cref="Point"/> and returns the 
    /// resulting <see cref="Point"/>.
    /// </summary>
    /// The point from which vector is subtracted.
    /// <param name="point">
    /// </param>
    /// <param name="vector">
    /// The vector to subtract from point.
    /// </param>
    /// <returns>
    /// The difference between point and vector.
    /// </returns>
    public static Point Subtract(Point point, Vector vector) => new(point._x - vector._x, point._y - vector._y);

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
    /// Offsets a point's <see cref="X"/> and <see cref="Y"/> coordinates by the specified amounts.
    /// </summary>
    /// <param name="offsetX">
    /// The amount to offset the point's <see cref="X"/> coordinate.
    /// </param>
    /// <param name="offsetY">
    /// The amount to offset the point's <see cref="Y"/> coordinate.
    /// </param>
    public void Offset(double offsetX, double offsetY)
    {
        _x += offsetX;
        _y += offsetY;
    }

    /// <summary>
    /// Compares two <see cref="Point"/> structures for equality.
    /// </summary>
    /// <param name="point1">
    /// The first point to compare.
    /// </param>
    /// <param name="point2">
    /// The second point to compare.
    /// </param>
    /// <returns>
    /// true if point1 and point2 contain the same <see cref="X"/> and <see cref="Y"/> values; otherwise, false.
    /// </returns>
    public static bool Equals(Point point1, Point point2) => point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y);

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

    /// <summary>
    /// Translates the specified <see cref="Point"/> by the specified <see cref="Vector"/> and returns the result.
    /// </summary>
    /// <param name="point">
    /// The point to translate.
    /// </param>
    /// <param name="vector">
    /// The amount by which to translate point.
    /// </param>
    /// <returns>
    /// The result of translating the specified point by the specified vector.
    /// </returns>
    public static Point operator +(Point point, Vector vector) => new(point._x + vector._x, point._y + vector._y);

    /// <summary>
    /// Subtracts the specified <see cref="Point"/> from another specified <see cref="Point"/>
    /// and returns the difference as a <see cref="Vector"/>.
    /// </summary>
    /// <param name="point1">
    /// The point from which point2 is subtracted.
    /// </param>
    /// <param name="point2">
    /// The point to subtract from point1.
    /// </param>
    /// <returns>
    /// The difference between point1 and point2.
    /// </returns>
    public static Vector operator -(Point point1, Point point2) => new(point1._x - point2._x, point1._y - point2._y);

    /// <summary>
    /// Subtracts the specified <see cref="Vector"/> from the specified <see cref="Point"/> and returns the 
    /// resulting <see cref="Point"/>.
    /// </summary>
    /// <param name="point">
    /// The point from which vector is subtracted.
    /// </param>
    /// <param name="vector">
    /// The vector to subtract from point1.
    /// </param>
    /// <returns>
    /// The difference between point and vector.
    /// </returns>
    public static Point operator -(Point point, Vector vector) => new(point._x - vector._x, point._y - vector._y);

    /// <summary>
    /// Transforms the specified <see cref="Point"/> by the specified <see cref="Matrix"/>.
    /// </summary>
    /// <param name="point">
    /// The point to transform.
    /// </param>
    /// <param name="matrix">
    /// The transformation matrix.
    /// </param>
    /// <returns>
    /// The result of transforming the specified point using the specified matrix.
    /// </returns>
    public static Point operator *(Point point, Matrix matrix) => matrix.Transform(point);

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
    /// Creates a <see cref="Size"/> structure with a <see cref="Size.Width"/> equal to this point's 
    /// <see cref="X"/> value and a <see cref="Size.Height"/> equal to this point's <see cref="Y"/> value.
    /// </summary>
    /// <param name="point">
    /// The point to convert.
    /// </param>
    /// <returns>
    /// A <see cref="Size"/> structure with a <see cref="Size.Width"/> equal to this point's <see cref="X"/> 
    /// value and a <see cref="Size.Height"/> equal to this point's <see cref="Y"/> value.
    /// </returns>
    public static explicit operator Size(Point point) => new(Math.Abs(point._x), Math.Abs(point._y));

    /// <summary>
    /// Creates a <see cref="Vector"/> structure with an <see cref="Vector.X"/> value equal to the point's 
    /// <see cref="X"/> value and a <see cref="Vector.Y"/> value equal to the point's <see cref="Y"/> value.
    /// </summary>
    /// <param name="point">
    /// The point to convert.
    /// </param>
    /// <returns>
    /// A vector with an <see cref="Vector.X"/> value equal to the point's <see cref="X"/> value and a 
    /// <see cref="Vector.Y"/> value equal to the point's <see cref="Y"/> value.
    /// </returns>
    public static explicit operator Vector(Point point) => new(point._x, point._y);
}