
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Media.Media3D;

/// <summary>
/// Represents a 4 × 4 matrix that is used for transformations in a three-dimensional (3-D) space.
/// </summary>
[TypeConverter(typeof(Matrix3DConverter))]
public struct Matrix3D : IFormattable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix3D"/> structure.
    /// </summary>
    /// <param name="m11">The value of the (1,1) field of the new matrix.</param>
    /// <param name="m12">The value of the (1,2) field of the new matrix.</param>
    /// <param name="m13">The value of the (1,3) field of the new matrix.</param>
    /// <param name="m14">The value of the (1,4) field of the new matrix.</param>
    /// <param name="m21">The value of the (2,1) field of the new matrix.</param>
    /// <param name="m22">The value of the (2,2) field of the new matrix.</param>
    /// <param name="m23">The value of the (2,3) field of the new matrix.</param>
    /// <param name="m24">The value of the (2,4) field of the new matrix.</param>
    /// <param name="m31">The value of the (3,1) field of the new matrix.</param>
    /// <param name="m32">The value of the (3,2) field of the new matrix.</param>
    /// <param name="m33">The value of the (3,3) field of the new matrix.</param>
    /// <param name="m34">The value of the (3,4) field of the new matrix.</param>
    /// <param name="offsetX">The value of the X offset field of the new matrix.</param>
    /// <param name="offsetY">The value of the Y offset field of the new matrix.</param>
    /// <param name="offsetZ">The value of the Z offset field of the new matrix.</param>
    /// <param name="m44">The value of the (4,4) field of the new matrix.</param>
    public Matrix3D(double m11, double m12, double m13, double m14,
                    double m21, double m22, double m23, double m24,
                    double m31, double m32, double m33, double m34,
                    double offsetX, double offsetY, double offsetZ, double m44)
    {
        _m11 = m11;
        _m12 = m12;
        _m13 = m13;
        _m14 = m14;
        _m21 = m21;
        _m22 = m22;
        _m23 = m23;
        _m24 = m24;
        _m31 = m31;
        _m32 = m32;
        _m33 = m33;
        _m34 = m34;
        _offsetX = offsetX;
        _offsetY = offsetY;
        _offsetZ = offsetZ;
        _m44 = m44;

        // This is not known to be an identity matrix so we need
        // to change our flag from it's default value.  We use the field
        // in the ctor rather than the property because of CS0188.
        _isNotKnownToBeIdentity = true;
    }

    /// <summary>
    /// Converts a string representation of a <see cref="Matrix3D"/> structure into the equivalent 
    /// <see cref="Matrix3D"/> structure.
    /// </summary>
    /// <param name="source">
    /// <see cref="string"/> representation of the Matrix3D.
    /// </param>
    /// <returns>
    /// <see cref="Matrix3D"/> structure represented by the string.
    /// </returns>
    public static Matrix3D Parse(string source)
    {
        IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        var th = new TokenizerHelper(source, formatProvider);

        Matrix3D value;

        string firstToken = th.NextTokenRequired();

        // The token will already have had whitespace trimmed so we can do a
        // simple string compare.
        if (firstToken == "Identity")
        {
            value = Identity;
        }
        else
        {
            value = new Matrix3D(
                Convert.ToDouble(firstToken, formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider));
        }

        // There should be no more tokens in this string.
        th.LastTokenRequired();

        return value;
    }

    /// <summary>
    /// The identity <see cref="Matrix3D"/>.
    /// </summary>
    public static Matrix3D Identity => s_identity;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="Matrix3D"/> is invertible.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Matrix3D"/> has an inverse; otherwise, false. The default value is true.
    /// </returns>
    public bool HasInverse => !DoubleUtil.IsZero(Determinant);

    /// <summary>
    /// Gets a value that indicates whether this <see cref="Matrix3D"/> structure is affine.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Matrix3D"/> structure is affine; otherwise, false.
    /// </returns>
    public bool IsAffine => IsDistinguishedIdentity || (_m14 == 0.0 && _m24 == 0.0 && _m34 == 0.0 && _m44 == 1.0);

    /// <summary>
    /// Retrieves the determinant of this <see cref="Matrix3D"/> structure.
    /// </summary>
    /// <returns>
    /// The determinant of this <see cref="Matrix3D"/> structure.
    /// </returns>
    public double Determinant
    {
        get
        {
            if (IsDistinguishedIdentity)
                return 1.0;
            if (IsAffine)
                return GetNormalizedAffineDeterminant();

            // NOTE: The beginning of this code is duplicated between
            //       the Invert method and the Determinant property.

            // compute all six 2x2 determinants of 2nd two columns
            double y01 = _m13 * _m24 - _m23 * _m14;
            double y02 = _m13 * _m34 - _m33 * _m14;
            double y03 = _m13 * _m44 - _offsetZ * _m14;
            double y12 = _m23 * _m34 - _m33 * _m24;
            double y13 = _m23 * _m44 - _offsetZ * _m24;
            double y23 = _m33 * _m44 - _offsetZ * _m34;

            // Compute 3x3 cofactors for 1st the column
            double z30 = _m22 * y02 - _m32 * y01 - _m12 * y12;
            double z20 = _m12 * y13 - _m22 * y03 + _offsetY * y01;
            double z10 = _m32 * y03 - _offsetY * y02 - _m12 * y23;
            double z00 = _m22 * y23 - _m32 * y13 + _offsetY * y12;

            return _offsetX * z30 + _m31 * z20 + _m21 * z10 + _m11 * z00;
        }
    }

    /// <summary>
    /// Determines whether this <see cref="Matrix3D"/> structure is an identity <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Matrix3D"/> is an identity <see cref="Matrix3D"/>; otherwise, false. The default value is true.
    /// </returns>
    public bool IsIdentity
    {
        get
        {
            if (IsDistinguishedIdentity)
            {
                return true;
            }
            else
            {
                // Otherwise check all elements one by one.
                if (_m11 == 1.0 && _m12 == 0.0 && _m13 == 0.0 && _m14 == 0.0 &&
                    _m21 == 0.0 && _m22 == 1.0 && _m23 == 0.0 && _m24 == 0.0 &&
                    _m31 == 0.0 && _m32 == 0.0 && _m33 == 1.0 && _m34 == 0.0 &&
                    _offsetX == 0.0 && _offsetY == 0.0 && _offsetZ == 0.0 && _m44 == 1.0)
                {
                    // If matrix is identity, cache this with the IsDistinguishedIdentity flag.
                    IsDistinguishedIdentity = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the first row and first column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the first row and first column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M11
    {
        get
        {
            if (IsDistinguishedIdentity)
            {
                return 1.0;
            }
            else
            {
                return _m11;
            }
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m11 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the first row and second column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the first row and second column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M12
    {
        get
        {
            return _m12;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m12 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the first row and third column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the first row and third column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M13
    {
        get
        {
            return _m13;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m13 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the first row and fourth column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the first row and fourth column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M14
    {
        get
        {
            return _m14;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m14 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and first column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the second row and first column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M21
    {
        get
        {
            return _m21;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m21 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and second column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the second row and second column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M22
    {
        get
        {
            if (IsDistinguishedIdentity)
            {
                return 1.0;
            }
            else
            {
                return _m22;
            }
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m22 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and third column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the second row and third column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M23
    {
        get
        {
            return _m23;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m23 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and fourth column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the second row and fourth column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M24
    {
        get
        {
            return _m24;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m24 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and first column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the third row and first column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M31
    {
        get
        {
            return _m31;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m31 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and second column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the third row and second column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M32
    {
        get
        {
            return _m32;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m32 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and third column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the third row and third column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M33
    {
        get
        {
            if (IsDistinguishedIdentity)
            {
                return 1.0;
            }
            else
            {
                return _m33;
            }
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m33 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and fourth column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the third row and fourth column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M34
    {
        get
        {
            return _m34;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m34 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the fourth row and fourth column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the fourth row and fourth column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double M44
    {
        get
        {
            if (IsDistinguishedIdentity)
            {
                return 1.0;
            }
            else
            {
                return _m44;
            }
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _m44 = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the fourth row and first column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the fourth row and first column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double OffsetX
    {
        get
        {
            return _offsetX;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _offsetX = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the fourth row and second column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the fourth row and second column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double OffsetY
    {
        get
        {
            return _offsetY;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _offsetY = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the fourth row and third column of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// The value of the fourth row and third column of this <see cref="Matrix3D"/>.
    /// </returns>
    public double OffsetZ
    {
        get
        {
            return _offsetZ;
        }
        set
        {
            if (IsDistinguishedIdentity)
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
            }
            _offsetZ = value;
        }
    }

    /// <summary>
    /// Tests equality between two matrices.
    /// </summary>
    /// <param name="matrix1">
    /// The first <see cref="Matrix3D"/> to compare.
    /// </param>
    /// <param name="matrix2">
    /// The second <see cref="Matrix3D"/> to compare.
    /// </param>
    /// <returns>
    /// <see cref="bool"/> that indicates whether the matrices are equal.
    /// </returns>
    public static bool Equals(Matrix3D matrix1, Matrix3D matrix2)
    {
        if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
        {
            return matrix1.IsIdentity == matrix2.IsIdentity;
        }
        else
        {
            return matrix1.M11.Equals(matrix2.M11) &&
                   matrix1.M12.Equals(matrix2.M12) &&
                   matrix1.M13.Equals(matrix2.M13) &&
                   matrix1.M14.Equals(matrix2.M14) &&
                   matrix1.M21.Equals(matrix2.M21) &&
                   matrix1.M22.Equals(matrix2.M22) &&
                   matrix1.M23.Equals(matrix2.M23) &&
                   matrix1.M24.Equals(matrix2.M24) &&
                   matrix1.M31.Equals(matrix2.M31) &&
                   matrix1.M32.Equals(matrix2.M32) &&
                   matrix1.M33.Equals(matrix2.M33) &&
                   matrix1.M34.Equals(matrix2.M34) &&
                   matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                   matrix1.OffsetY.Equals(matrix2.OffsetY) &&
                   matrix1.OffsetZ.Equals(matrix2.OffsetZ) &&
                   matrix1.M44.Equals(matrix2.M44);
        }
    }

    /// <summary>
    /// Tests equality between two matrices.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Matrix3D"/> to compare to.
    /// </param>
    /// <returns>
    /// true if the matrices are equal; otherwise, false.
    /// </returns>
    public bool Equals(Matrix3D value) => Equals(this, value);

    /// <summary>
    /// Tests equality between two matrices.
    /// </summary>
    /// <param name="o">
    /// The object to test for equality.
    /// </param>
    /// <returns>
    /// true if the matrices are equal; otherwise, false.
    /// </returns>
    public override bool Equals(object o) => o is Matrix3D value && Equals(this, value);

    /// <summary>
    /// Returns the hash code for this matrix.
    /// </summary>
    /// <returns>
    /// An integer that specifies the hash code for this matrix.
    /// </returns>
    public override int GetHashCode()
    {
        if (IsDistinguishedIdentity)
        {
            return c_identityHashCode;
        }
        else
        {
            // Perform field-by-field XOR of HashCodes
            return M11.GetHashCode() ^
                   M12.GetHashCode() ^
                   M13.GetHashCode() ^
                   M14.GetHashCode() ^
                   M21.GetHashCode() ^
                   M22.GetHashCode() ^
                   M23.GetHashCode() ^
                   M24.GetHashCode() ^
                   M31.GetHashCode() ^
                   M32.GetHashCode() ^
                   M33.GetHashCode() ^
                   M34.GetHashCode() ^
                   OffsetX.GetHashCode() ^
                   OffsetY.GetHashCode() ^
                   OffsetZ.GetHashCode() ^
                   M44.GetHashCode();
        }
    }

    /// <summary>
    /// Inverts this <see cref="Matrix3D"/> structure.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The matrix is not invertible.
    /// </exception>
    public void Invert()
    {
        if (!InvertCore())
        {
            throw new InvalidOperationException(Strings.Matrix3D_NotInvertible);
        }
    }

    /// <summary>
    /// Creates a string representation of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <returns>
    /// A string representation of this <see cref="Matrix3D"/>.
    /// </returns>
    public override string ToString() => ConvertToString(null, null);

    /// <summary>
    /// Creates a string representation of this <see cref="Matrix3D"/>.
    /// </summary>
    /// <param name="provider">
    /// Culture-specified formatting information.
    /// </param>
    /// <returns>
    /// The string representation of this <see cref="Matrix3D"/>.
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
        if (IsIdentity)
        {
            return "Identity";
        }

        // Helper to get the numeric list separator for a given culture.
        char separator = TokenizerHelper.GetNumericListSeparator(provider);
        return string.Format(provider,
                             "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}{0}{7:" + format + "}{0}{8:" + format + "}{0}{9:" + format + "}{0}{10:" + format + "}{0}{11:" + format + "}{0}{12:" + format + "}{0}{13:" + format + "}{0}{14:" + format + "}{0}{15:" + format + "}{0}{16:" + format + "}",
                             separator,
                             _m11,
                             _m12,
                             _m13,
                             _m14,
                             _m21,
                             _m22,
                             _m23,
                             _m24,
                             _m31,
                             _m32,
                             _m33,
                             _m34,
                             _offsetX,
                             _offsetY,
                             _offsetZ,
                             _m44);
    }

    /// <summary>
    /// Multiplies the specified matrices.
    /// </summary>
    /// <param name="matrix1">
    /// Matrix to multiply.
    /// </param>
    /// <param name="matrix2">
    /// Matrix by which the first matrix is multiplied.
    /// </param>
    /// <returns>
    /// <see cref="Matrix3D"/> that is the result of multiplication.
    /// </returns>
    public static Matrix3D Multiply(Matrix3D matrix1, Matrix3D matrix2) => matrix1 * matrix2;

    /// <summary>
    /// Multiplies the specified matrices.
    /// </summary>
    /// <param name="matrix1">
    /// The matrix to multiply.
    /// </param>
    /// <param name="matrix2">
    /// The matrix by which the first matrix is multiplied.
    /// </param>
    /// <returns>
    /// The <see cref="Matrix3D"/> that is the result of multiplication.
    /// </returns>
    public static Matrix3D operator *(Matrix3D matrix1, Matrix3D matrix2)
    {
        // Check if multiplying by identity.
        if (matrix1.IsDistinguishedIdentity)
            return matrix2;
        if (matrix2.IsDistinguishedIdentity)
            return matrix1;

        // Regular 4x4 matrix multiplication.
        var result = new Matrix3D(
            matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21 +
            matrix1._m13 * matrix2._m31 + matrix1._m14 * matrix2._offsetX,
            matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22 +
            matrix1._m13 * matrix2._m32 + matrix1._m14 * matrix2._offsetY,
            matrix1._m11 * matrix2._m13 + matrix1._m12 * matrix2._m23 +
            matrix1._m13 * matrix2._m33 + matrix1._m14 * matrix2._offsetZ,
            matrix1._m11 * matrix2._m14 + matrix1._m12 * matrix2._m24 +
            matrix1._m13 * matrix2._m34 + matrix1._m14 * matrix2._m44,
            matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21 +
            matrix1._m23 * matrix2._m31 + matrix1._m24 * matrix2._offsetX,
            matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22 +
            matrix1._m23 * matrix2._m32 + matrix1._m24 * matrix2._offsetY,
            matrix1._m21 * matrix2._m13 + matrix1._m22 * matrix2._m23 +
            matrix1._m23 * matrix2._m33 + matrix1._m24 * matrix2._offsetZ,
            matrix1._m21 * matrix2._m14 + matrix1._m22 * matrix2._m24 +
            matrix1._m23 * matrix2._m34 + matrix1._m24 * matrix2._m44,
            matrix1._m31 * matrix2._m11 + matrix1._m32 * matrix2._m21 +
            matrix1._m33 * matrix2._m31 + matrix1._m34 * matrix2._offsetX,
            matrix1._m31 * matrix2._m12 + matrix1._m32 * matrix2._m22 +
            matrix1._m33 * matrix2._m32 + matrix1._m34 * matrix2._offsetY,
            matrix1._m31 * matrix2._m13 + matrix1._m32 * matrix2._m23 +
            matrix1._m33 * matrix2._m33 + matrix1._m34 * matrix2._offsetZ,
            matrix1._m31 * matrix2._m14 + matrix1._m32 * matrix2._m24 +
            matrix1._m33 * matrix2._m34 + matrix1._m34 * matrix2._m44,
            matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 +
            matrix1._offsetZ * matrix2._m31 + matrix1._m44 * matrix2._offsetX,
            matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 +
            matrix1._offsetZ * matrix2._m32 + matrix1._m44 * matrix2._offsetY,
            matrix1._offsetX * matrix2._m13 + matrix1._offsetY * matrix2._m23 +
            matrix1._offsetZ * matrix2._m33 + matrix1._m44 * matrix2._offsetZ,
            matrix1._offsetX * matrix2._m14 + matrix1._offsetY * matrix2._m24 +
            matrix1._offsetZ * matrix2._m34 + matrix1._m44 * matrix2._m44);

        return result;
    }

    /// <summary>
    /// Compares two <see cref="Matrix3D"/> instances for exact equality.
    /// </summary>
    /// <param name="matrix1">
    /// The first matrix to compare.
    /// </param>
    /// <param name="matrix2">
    /// The second matrix to compare.
    /// </param>
    /// <returns>
    /// true if the matrices are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(Matrix3D matrix1, Matrix3D matrix2) => Equals(matrix1, matrix2);

    /// <summary>
    /// Compares two <see cref="Matrix3D"/> instances for inequality.
    /// </summary>
    /// <param name="matrix1">
    /// The first matrix to compare.
    /// </param>
    /// <param name="matrix2">
    /// The second matrix to compare.
    /// </param>
    /// <returns>
    /// true if the matrices are different; otherwise, false.
    /// </returns>
    public static bool operator !=(Matrix3D matrix1, Matrix3D matrix2) => !Equals(matrix1, matrix2);

    /// <summary>
    /// Changes this <see cref="Matrix3D"/> structure into an identity matrix.
    /// </summary>
    public void SetIdentity()
    {
        this = s_identity;
    }

    /// <summary>
    /// Prepends a specified matrix to the current matrix.
    /// </summary>
    /// <param name="matrix">
    /// Matrix to prepend.
    /// </param>
    public void Prepend(Matrix3D matrix)
    {
        this = matrix * this;
    }

    /// <summary>
    /// Appends a specified matrix to the current matrix.
    /// </summary>
    /// <param name="matrix">
    /// Matrix to append.
    /// </param>
    public void Append(Matrix3D matrix)
    {
        this *= matrix;
    }

    //  Computes the determinant of the matrix assuming that it's
    //  fourth column is 0,0,0,1 and it isn't identity
    internal double GetNormalizedAffineDeterminant()
    {
        Debug.Assert(!IsDistinguishedIdentity);
        Debug.Assert(IsAffine);

        // NOTE: The beginning of this code is duplicated between
        //       GetNormalizedAffineDeterminant() and NormalizedAffineInvert()

        double z20 = _m12 * _m23 - _m22 * _m13;
        double z10 = _m32 * _m13 - _m12 * _m33;
        double z00 = _m22 * _m33 - _m32 * _m23;

        return _m31 * z20 + _m21 * z10 + _m11 * z00;
    }

    // Assuming this matrix has fourth column of 0,0,0,1 and isn't identity this function:
    // Returns false if HasInverse is false, otherwise inverts the matrix.
    internal bool NormalizedAffineInvert()
    {
        Debug.Assert(!IsDistinguishedIdentity);
        Debug.Assert(IsAffine);

        // NOTE: The beginning of this code is duplicated between
        //       GetNormalizedAffineDeterminant() and NormalizedAffineInvert()

        double z20 = _m12 * _m23 - _m22 * _m13;
        double z10 = _m32 * _m13 - _m12 * _m33;
        double z00 = _m22 * _m33 - _m32 * _m23;
        double det = _m31 * z20 + _m21 * z10 + _m11 * z00;

        // Fancy logic here avoids using equality with possible nan values.
        Debug.Assert(!(det < Determinant || det > Determinant),
                     "Matrix3D.Inverse: Determinant property does not match value computed in Inverse.");

        if (DoubleUtil.IsZero(det))
        {
            return false;
        }

        // Compute 3x3 non-zero cofactors for the 2nd column
        double z21 = _m21 * _m13 - _m11 * _m23;
        double z11 = _m11 * _m33 - _m31 * _m13;
        double z01 = _m31 * _m23 - _m21 * _m33;

        // Compute all six 2x2 determinants of 1st two columns
        double y01 = _m11 * _m22 - _m21 * _m12;
        double y02 = _m11 * _m32 - _m31 * _m12;
        double y03 = _m11 * _offsetY - _offsetX * _m12;
        double y12 = _m21 * _m32 - _m31 * _m22;
        double y13 = _m21 * _offsetY - _offsetX * _m22;
        double y23 = _m31 * _offsetY - _offsetX * _m32;

        // Compute all non-zero and non-one 3x3 cofactors for 2nd
        // two columns
        double z23 = _m23 * y03 - _offsetZ * y01 - _m13 * y13;
        double z13 = _m13 * y23 - _m33 * y03 + _offsetZ * y02;
        double z03 = _m33 * y13 - _offsetZ * y12 - _m23 * y23;
        double z22 = y01;
        double z12 = -y02;
        double z02 = y12;

        double rcp = 1.0 / det;

        // Multiply all 3x3 cofactors by reciprocal & transpose
        _m11 = z00 * rcp;
        _m12 = z10 * rcp;
        _m13 = z20 * rcp;

        _m21 = z01 * rcp;
        _m22 = z11 * rcp;
        _m23 = z21 * rcp;

        _m31 = z02 * rcp;
        _m32 = z12 * rcp;
        _m33 = z22 * rcp;

        _offsetX = z03 * rcp;
        _offsetY = z13 * rcp;
        _offsetZ = z23 * rcp;

        return true;
    }

    // RETURNS true if has inverse & invert was done.  Otherwise returns false & leaves matrix unchanged.
    internal bool InvertCore()
    {
        if (IsDistinguishedIdentity)
            return true;

        if (IsAffine)
        {
            return NormalizedAffineInvert();
        }

        // NOTE: The beginning of this code is duplicated between
        //       the Invert method and the Determinant property.

        // compute all six 2x2 determinants of 2nd two columns
        double y01 = _m13 * _m24 - _m23 * _m14;
        double y02 = _m13 * _m34 - _m33 * _m14;
        double y03 = _m13 * _m44 - _offsetZ * _m14;
        double y12 = _m23 * _m34 - _m33 * _m24;
        double y13 = _m23 * _m44 - _offsetZ * _m24;
        double y23 = _m33 * _m44 - _offsetZ * _m34;

        // Compute 3x3 cofactors for 1st the column
        double z30 = _m22 * y02 - _m32 * y01 - _m12 * y12;
        double z20 = _m12 * y13 - _m22 * y03 + _offsetY * y01;
        double z10 = _m32 * y03 - _offsetY * y02 - _m12 * y23;
        double z00 = _m22 * y23 - _m32 * y13 + _offsetY * y12;

        // Compute 4x4 determinant
        double det = _offsetX * z30 + _m31 * z20 + _m21 * z10 + _m11 * z00;

        // If Determinant is computed using a different method then Inverse can throw
        // NotInvertable when HasInverse is true.  (Windows OS #901174)
        //
        // The strange logic below is equivalent to "det == Determinant", but NaN safe.
        Debug.Assert(!(det < Determinant || det > Determinant),
            "Matrix3D.Inverse: Determinant property does not match value computed in Inverse.");

        if (DoubleUtil.IsZero(det))
        {
            return false;
        }

        // Compute 3x3 cofactors for the 2nd column
        double z31 = _m11 * y12 - _m21 * y02 + _m31 * y01;
        double z21 = _m21 * y03 - _offsetX * y01 - _m11 * y13;
        double z11 = _m11 * y23 - _m31 * y03 + _offsetX * y02;
        double z01 = _m31 * y13 - _offsetX * y12 - _m21 * y23;

        // Compute all six 2x2 determinants of 1st two columns
        y01 = _m11 * _m22 - _m21 * _m12;
        y02 = _m11 * _m32 - _m31 * _m12;
        y03 = _m11 * _offsetY - _offsetX * _m12;
        y12 = _m21 * _m32 - _m31 * _m22;
        y13 = _m21 * _offsetY - _offsetX * _m22;
        y23 = _m31 * _offsetY - _offsetX * _m32;

        // Compute all 3x3 cofactors for 2nd two columns
        double z33 = _m13 * y12 - _m23 * y02 + _m33 * y01;
        double z23 = _m23 * y03 - _offsetZ * y01 - _m13 * y13;
        double z13 = _m13 * y23 - _m33 * y03 + _offsetZ * y02;
        double z03 = _m33 * y13 - _offsetZ * y12 - _m23 * y23;
        double z32 = _m24 * y02 - _m34 * y01 - _m14 * y12;
        double z22 = _m14 * y13 - _m24 * y03 + _m44 * y01;
        double z12 = _m34 * y03 - _m44 * y02 - _m14 * y23;
        double z02 = _m24 * y23 - _m34 * y13 + _m44 * y12;

        double rcp = 1.0 / det;

        // Multiply all 3x3 cofactors by reciprocal & transpose
        _m11 = z00 * rcp;
        _m12 = z10 * rcp;
        _m13 = z20 * rcp;
        _m14 = z30 * rcp;

        _m21 = z01 * rcp;
        _m22 = z11 * rcp;
        _m23 = z21 * rcp;
        _m24 = z31 * rcp;

        _m31 = z02 * rcp;
        _m32 = z12 * rcp;
        _m33 = z22 * rcp;
        _m34 = z32 * rcp;

        _offsetX = z03 * rcp;
        _offsetY = z13 * rcp;
        _offsetZ = z23 * rcp;
        _m44 = z33 * rcp;

        return true;
    }

    private static Matrix3D CreateIdentity()
    {
        // Don't call this function, use s_identity.
        Matrix3D matrix = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)
        {
            IsDistinguishedIdentity = true
        };
        return matrix;
    }

    // Returns true if this matrix is guaranteed to be the identity matrix.
    // This is true when a new matrix has been created or after the Identity
    // has already been computed.
    //
    // NOTE: In the case of a new matrix, the _m* fields on the diagonal
    // will be uninitialized.  You should either use the properties which interpret
    // this state and return 1.0 or you can frequently early exit with a known
    // value for the identity matrix.
    //
    // NOTE: This property being false does not mean that the matrix is
    // not the identity matrix, it means that we do not know for certain if
    // it is the identity matrix.  Use the Identity property if you need to
    // know if the matrix is the identity matrix.  (The result will be cached
    // and this property will start returning true.)
    //
    private bool IsDistinguishedIdentity
    {
        get
        {
            Debug.Assert(
                _isNotKnownToBeIdentity
                    || (
                        (_m11 == 0.0 || _m11 == 1.0) && (_m12 == 0.0) && (_m13 == 0.0) && (_m14 == 0.0) &&
                        (_m21 == 0.0) && (_m22 == 0.0 || _m22 == 1.0) && (_m23 == 0.0) && (_m24 == 0.0) &&
                        (_m31 == 0.0) && (_m32 == 0.0) && (_m33 == 0.0 || _m33 == 1.0) && (_m34 == 0.0) &&
                        (_offsetX == 0.0) && (_offsetY == 0.0) && (_offsetZ == 0.0) && (_m44 == 0.0 || _m44 == 1.0)),
                "Matrix3D.IsDistinguishedIdentity - _isNotKnownToBeIdentity flag is inconsistent with matrix state.");

            return !_isNotKnownToBeIdentity;
        }

        set
        {
            _isNotKnownToBeIdentity = !value;

            // This not only verifies we got the inversion right, but we also hit the
            // the assert above which verifies the value matches the state of the matrix.
            Debug.Assert(IsDistinguishedIdentity == value,
                "Matrix3D.IsDistinguishedIdentity - Error detected setting IsDistinguishedIdentity.");
        }
    }

    private double _m11;
    private double _m12;
    private double _m13;
    private double _m14;

    private double _m21;
    private double _m22;
    private double _m23;
    private double _m24;

    private double _m31;
    private double _m32;
    private double _m33;
    private double _m34;

    private double _offsetX;
    private double _offsetY;
    private double _offsetZ;

    private double _m44;

    // Internal matrix representation
    private bool _isNotKnownToBeIdentity;

    // NOTE: The ctor used to create this identity sets the
    //       _isNotKnownToBeIdentity flag to true (i.e., s_identity
    //       is assumed not to be the identity by methods which
    //       early exit on the identity matrix.)
    //
    //       For performance you should only use s_identity to
    //       initialize a matrix before writing new values.  If
    //       you actually want an identity matrix you should use
    //       "new Matrix3D()" which takes advantage of the various
    //       opitimizations in the identity case.
    //
    private static readonly Matrix3D s_identity = CreateIdentity();

    // The hash code for a matrix is the xor of its element's hashes.
    // Since the identity matrix has 4 1's and 12 0's its hash is 0.
    private const int c_identityHashCode = 0;
}