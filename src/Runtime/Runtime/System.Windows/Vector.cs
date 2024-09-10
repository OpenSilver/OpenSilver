
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

using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a displacement in 2-D space.
/// </summary>
[TypeConverter(typeof(VectorConverter))]
public struct Vector : IFormattable
{
    internal double _x;
    internal double _y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector"/> structure.
    /// </summary>
    /// <param name="x">
    /// The <see cref="X"/> offset of the new <see cref="Vector"/>.
    /// </param>
    /// <param name="y">
    /// The <see cref="Y"/> offset of the new <see cref="Vector"/>.
    /// </param>
    public Vector(double x, double y)
    {
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Gets the square of the length of this vector.
    /// </summary>
    /// <returns>
    /// The square of the <see cref="Length"/> of this vector.
    /// </returns>
    public double LengthSquared => _x * _x + _y * _y;

    /// <summary>
    /// Gets the length of this vector.
    /// </summary>
    /// <returns>
    /// The length of this vector.
    /// </returns>
    public double Length => Math.Sqrt(_x * _x + _y * _y);

    /// <summary>
    /// Gets or sets the <see cref="X"/> component of this vector.
    /// </summary>
    /// <returns>
    /// The <see cref="X"/> component of this vector. The default value is 0.
    /// </returns>
    public double X
    {
        get => _x;
        set => _x = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="Y"/> component of this vector.
    /// </summary>
    /// <returns>
    /// The <see cref="Y"/> component of this vector. The default value is 0.
    /// </returns>
    public double Y
    {
        get => _y;
        set => _y = value;
    }

    /// <summary>
    /// Adds two vectors and returns the result as a <see cref="Vector"/> structure.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to add.
    /// </param>
    /// <param name="vector2">
    /// The second vector to add.
    /// </param>
    /// <returns>
    /// The sum of vector1 and vector2.
    /// </returns>
    public static Vector Add(Vector vector1, Vector vector2) => new(vector1._x + vector2._x, vector1._y + vector2._y);

    /// <summary>
    /// Translates the specified point by the specified vector and returns the resulting point.
    /// </summary>
    /// <param name="vector">
    /// The amount to translate the specified point.
    /// </param>
    /// <param name="point">
    /// The point to translate.
    /// </param>
    /// <returns>
    /// The result of translating point by vector.
    /// </returns>
    public static Point Add(Vector vector, Point point) => new(point._x + vector._x, point._y + vector._y);

    /// <summary>
    /// Retrieves the angle, expressed in degrees, between the two specified vectors.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to evaluate.
    /// </param>
    /// <param name="vector2">
    /// The second vector to evaluate.
    /// </param>
    /// <returns>
    /// The angle, in degrees, between vector1 and vector2.
    /// </returns>
    public static double AngleBetween(Vector vector1, Vector vector2)
    {
        double sin = vector1._x * vector2._y - vector2._x * vector1._y;
        double cos = vector1._x * vector2._x + vector1._y * vector2._y;

        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

    /// <summary>
    /// Calculates the cross product of two vectors.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to evaluate.
    /// </param>
    /// <param name="vector2">
    /// The second vector to evaluate.
    /// </param>
    /// <returns>
    /// The cross product of vector1 and vector2. The following formula is used to calculate
    /// the cross product: (Vector1.X * Vector2.Y) - (Vector1.Y * Vector2.X)
    /// </returns>
    public static double CrossProduct(Vector vector1, Vector vector2) => vector1._x * vector2._y - vector1._y * vector2._x;

    /// <summary>
    /// Calculates the determinant of two vectors.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to evaluate.
    /// </param>
    /// <param name="vector2">
    /// The second vector to evaluate.
    /// </param>
    /// <returns>
    /// The determinant of vector1 and vector2.
    /// </returns>
    public static double Determinant(Vector vector1, Vector vector2) => vector1._x * vector2._y - vector1._y * vector2._x;

    /// <summary>
    /// Divides the specified vector by the specified scalar and returns the result as a <see cref="Vector"/>.
    /// </summary>
    /// <param name="vector">
    /// The vector structure to divide.
    /// </param>
    /// <param name="scalar">
    /// The amount by which vector is divided.
    /// </param>
    /// <returns>
    /// The result of dividing vector by scalar.
    /// </returns>
    public static Vector Divide(Vector vector, double scalar) => vector * (1.0 / scalar);

    /// <summary>
    /// Compares the two specified vectors for equality.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to compare.
    /// </param>
    /// <param name="vector2">
    /// The second vector to compare.
    /// </param>
    /// <returns>
    /// true if the <see cref="X"/> and <see cref="Y"/> components of
    /// vector1 and vector2 are equal; otherwise, false.
    /// </returns>
    public static bool Equals(Vector vector1, Vector vector2) => vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y);

    /// <summary>
    /// Transforms the coordinate space of the specified vector using the specified <see cref="Matrix"/>.
    /// </summary>
    /// <param name="vector">
    /// The vector structure to transform.
    /// </param>
    /// <param name="matrix">
    /// The transformation to apply to vector.
    /// </param>
    /// <returns>
    /// The result of transforming vector by matrix.
    /// </returns>
    public static Vector Multiply(Vector vector, Matrix matrix) => matrix.Transform(vector);

    /// <summary>
    /// Calculates the dot product of the two specified vectors and returns the result as a <see cref="double"/>.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to multiply.
    /// </param>
    /// <param name="vector2">
    /// The second vector structure to multiply.
    /// </param>
    /// <returns>
    /// A <see cref="double"/> containing the scalar dot product of vector1 and vector2, which is calculated using the 
    /// following formula: (vector1.X * vector2.X) + (vector1.Y * vector2.Y)
    /// </returns>
    public static double Multiply(Vector vector1, Vector vector2) => vector1._x * vector2._x + vector1._y * vector2._y;

    /// <summary>
    /// Multiplies the specified scalar by the specified vector and returns the resulting <see cref="Vector"/>.
    /// </summary>
    /// <param name="scalar">
    /// The scalar to multiply.
    /// </param>
    /// <param name="vector">
    /// The vector to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying scalar and vector.
    /// </returns>
    public static Vector Multiply(double scalar, Vector vector) => new(vector._x * scalar, vector._y * scalar);

    /// <summary>
    /// Multiplies the specified vector by the specified scalar and returns the resulting <see cref="Vector"/>.
    /// </summary>
    /// <param name="vector">
    /// The vector to multiply.
    /// </param>
    /// <param name="scalar">
    /// The scalar to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying vector and scalar.
    /// </returns>
    public static Vector Multiply(Vector vector, double scalar) => new(vector._x * scalar, vector._y * scalar);

    /// <summary>
    /// Converts a string representation of a vector into the equivalent <see cref="Vector"/> structure.
    /// </summary>
    /// <param name="source">
    /// The string representation of the vector.
    /// </param>
    /// <returns>
    /// The equivalent <see cref="Vector"/> structure.
    /// </returns>
    public static Vector Parse(string source)
    {
        if (source != null)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return new Vector(
                    Convert.ToDouble(split[0], formatProvider),
                    Convert.ToDouble(split[1], formatProvider));
            }
        }

        throw new FormatException($"'{source}' is not an eligible value for a '{typeof(Vector)}'.");
    }

    /// <summary>
    /// Subtracts the specified vector from another specified vector.
    /// </summary>
    /// <param name="vector1">
    /// The vector from which vector2 is subtracted.
    /// </param>
    /// <param name="vector2">
    /// The vector to subtract from vector1.
    /// </param>
    /// <returns>
    /// The difference between vector1 and vector2.
    /// </returns>
    public static Vector Subtract(Vector vector1, Vector vector2) => new(vector1._x - vector2._x, vector1._y - vector2._y);

    /// <summary>
    /// Compares two vectors for equality.
    /// </summary>
    /// <param name="value">
    /// The vector to compare with this vector.
    /// </param>
    /// <returns>
    /// true if value has the same <see cref="X"/> and <see cref="Y"/> values as this vector; otherwise, false.
    /// </returns>
    public bool Equals(Vector value) => Equals(this, value);

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is a <see cref="Vector"/> structure
    /// and, if it is, whether it has the same <see cref="X"/> and <see cref="Y"/> values as this vector.
    /// </summary>
    /// <param name="o">
    /// The vector to compare.
    /// </param>
    /// <returns>
    /// true if o is a <see cref="Vector"/> and has the same <see cref="X"/> and <see cref="Y"/> values 
    /// as this vector; otherwise, false.
    /// </returns>
    public override bool Equals(object o) => o is Vector value && Equals(this, value);

    /// <summary>
    /// Returns the hash code for this vector.
    /// </summary>
    /// <returns>
    /// The hash code for this instance.
    /// </returns>
    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    /// <summary>
    /// Negates this vector. The vector has the same magnitude as before, but its direction is now opposite.
    /// </summary>
    public void Negate()
    {
        _x = -_x;
        _y = -_y;
    }

    /// <summary>
    /// Normalizes this vector.
    /// </summary>
    public void Normalize()
    {
        // Avoid overflow
        this /= Math.Max(Math.Abs(_x), Math.Abs(_y));
        this /= Length;
    }

    /// <summary>
    /// Returns the string representation of this <see cref="Vector"/> structure.
    /// </summary>
    /// <returns>
    /// A string that represents the <see cref="X"/> and <see cref="Y"/> values of this <see cref="Vector"/>.
    /// </returns>
    public override string ToString() => ConvertToString(null, null);

    /// <summary>
    /// Returns the string representation of this <see cref="Vector"/> structure with the specified 
    /// formatting information.
    /// </summary>
    /// <param name="provider">
    /// The culture-specific formatting information.
    /// </param>
    /// <returns>
    /// A string that represents the <see cref="X"/> and <see cref="Y"/> values of this <see cref="Vector"/>.
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
        // Helper to get the numeric list separator for a given culture.
        char separator = TokenizerHelper.GetNumericListSeparator(provider);
        return string.Format(provider,
                             "{1:" + format + "}{0}{2:" + format + "}",
                             separator,
                             _x,
                             _y);
    }

    /// <summary>
    /// Adds two vectors and returns the result as a vector.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to add.
    /// </param>
    /// <param name="vector2">
    /// The second vector to add.
    /// </param>
    /// <returns>
    /// The sum of vector1 and vector2.
    /// </returns>
    public static Vector operator +(Vector vector1, Vector vector2) => new(vector1._x + vector2._x, vector1._y + vector2._y);

    /// <summary>
    /// Translates a point by the specified vector and returns the resulting point.
    /// </summary>
    /// The vector used to translate point.
    /// <param name="vector">
    /// </param>
    /// <param name="point">
    /// The point to translate.
    /// </param>
    /// <returns>
    /// The result of translating point by vector.
    /// </returns>
    public static Point operator +(Vector vector, Point point) => new(point._x + vector._x, point._y + vector._y);

    /// <summary>
    /// Negates the specified vector.
    /// </summary>
    /// <param name="vector">
    /// The vector to negate.
    /// </param>
    /// <returns>
    /// A vector with <see cref="X"/> and <see cref="Y"/> values opposite of the <see cref="X"/> 
    /// and <see cref="Y"/> values of vector.
    /// </returns>
    public static Vector operator -(Vector vector) => new(-vector._x, -vector._y);

    /// <summary>
    /// Subtracts one specified vector from another.
    /// </summary>
    /// <param name="vector1">
    /// The vector from which vector2 is subtracted.
    /// </param>
    /// <param name="vector2">
    /// The vector to subtract from vector1.
    /// </param>
    /// <returns>
    /// The difference between vector1 and vector2.
    /// </returns>
    public static Vector operator -(Vector vector1, Vector vector2) => new(vector1._x - vector2._x, vector1._y - vector2._y);

    /// <summary>
    /// Transforms the coordinate space of the specified vector using the specified <see cref="Matrix"/>.
    /// </summary>
    /// <param name="vector">
    /// The vector to transform.
    /// </param>
    /// <param name="matrix">
    /// The transformation to apply to vector.
    /// </param>
    /// <returns>
    /// The result of transforming vector by matrix.
    /// </returns>
    public static Vector operator *(Vector vector, Matrix matrix) => matrix.Transform(vector);

    /// <summary>
    /// Calculates the dot product of the two specified vector structures and returns the result as a <see cref="double"/>.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to multiply.
    /// </param>
    /// <param name="vector2">
    /// The second vector to multiply.
    /// </param>
    /// <returns>
    /// Returns a <see cref="double"/> containing the scalar dot product of vector1 and vector2, which is calculated using 
    /// the following formula: vector1.X * vector2.X + vector1.Y * vector2.Y
    /// </returns>
    public static double operator *(Vector vector1, Vector vector2) => vector1._x * vector2._x + vector1._y * vector2._y;

    /// <summary>
    /// Multiplies the specified scalar by the specified vector and returns the resulting vector.
    /// </summary>
    /// <param name="scalar">
    /// The scalar to multiply.
    /// </param>
    /// <param name="vector">
    /// The vector to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying scalar and vector.
    /// </returns>
    public static Vector operator *(double scalar, Vector vector) => new(vector._x * scalar, vector._y * scalar);

    /// <summary>
    /// Multiplies the specified vector by the specified scalar and returns the resulting vector.
    /// </summary>
    /// <param name="vector">
    /// The vector to multiply.
    /// </param>
    /// <param name="scalar">
    /// The scalar to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying vector and scalar.
    /// </returns>
    public static Vector operator *(Vector vector, double scalar) => new(vector._x * scalar, vector._y * scalar);

    /// <summary>
    /// Divides the specified vector by the specified scalar and returns the resulting vector.
    /// </summary>
    /// <param name="vector">
    /// The vector to divide.
    /// </param>
    /// <param name="scalar">
    /// The scalar by which vector will be divided.
    /// </param>
    /// <returns>
    /// The result of dividing vector by scalar.
    /// </returns>
    public static Vector operator /(Vector vector, double scalar) => vector * (1.0 / scalar);

    /// <summary>
    /// Compares two vectors for equality.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to compare.
    /// </param>
    /// <param name="vector2">
    /// The second vector to compare.
    /// </param>
    /// <returns>
    /// true if the <see cref="X"/> and <see cref="Y"/> components of vector1 and vector2 are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(Vector vector1, Vector vector2) => vector1.X == vector2.X && vector1.Y == vector2.Y;

    /// <summary>
    /// Compares two vectors for inequality.
    /// </summary>
    /// <param name="vector1">
    /// The first vector to compare.
    /// </param>
    /// <param name="vector2">
    /// The second vector to compare.
    /// </param>
    /// <returns>
    /// true if the <see cref="X"/> and <see cref="Y"/> components of vector1 and vector2 are different; otherwise, false.
    /// </returns>
    public static bool operator !=(Vector vector1, Vector vector2) => !(vector1 == vector2);

    /// <summary>
    /// Creates a <see cref="Size"/> from the offsets of this vector.
    /// </summary>
    /// <param name="vector">
    /// The vector to convert.
    /// </param>
    /// <returns>
    /// A <see cref="Size"/> with a <see cref="Size.Width"/> equal to the absolute value of this vector's 
    /// <see cref="X"/> property and a <see cref="Size.Height"/> equal to the absolute value of this vector's 
    /// <see cref="Y"/> property.
    /// </returns>
    public static explicit operator Size(Vector vector) => new(Math.Abs(vector._x), Math.Abs(vector._y));

    /// <summary>
    /// Creates a <see cref="Point"/> with the <see cref="X"/> and <see cref="Y"/> values of this vector.
    /// </summary>
    /// <param name="vector">
    /// The vector to convert.
    /// </param>
    /// <returns>
    /// A point with <see cref="Point.X"/> and <see cref="Point.Y"/> coordinate values equal to the <see cref="X"/> 
    /// and <see cref="Y"/> offset values of vector.
    /// </returns>
    public static explicit operator Point(Vector vector) => new(vector._x, vector._y);
}
