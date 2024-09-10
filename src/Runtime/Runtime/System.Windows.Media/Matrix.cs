
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

using System.Diagnostics;
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Represents a 3x3 affine transformation matrix used for transformations in two-dimensional
/// space.
/// </summary>
public struct Matrix : IFormattable
{
    // the transform is identity by default
    private static Matrix s_identity = CreateIdentity();

    /// <summary>
    /// Initializes a <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="m11">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.M11"/> coefficient.
    /// </param>
    /// <param name="m12">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.M12"/> coefficient.
    /// </param>
    /// <param name="m21">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.M21"/> coefficient.
    /// </param>
    /// <param name="m22">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.M22"/> coefficient.
    /// </param>
    /// <param name="offsetX">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.OffsetX"/>
    /// coefficient.
    /// </param>
    /// <param name="offsetY">
    /// The <see cref="Matrix"/> structure's <see cref="Matrix.OffsetY"/>
    /// coefficient.
    /// </param>
    public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
    {
        _m11 = m11;
        _m12 = m12;
        _m21 = m21;
        _m22 = m22;
        _offsetX = offsetX;
        _offsetY = offsetY;
        _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;

        // We will detect EXACT identity, scale, translation or
        // scale+translation and use special case algorithms.
        DeriveMatrixType();
    }

    /// <summary>
    /// Gets an identity <see cref="Matrix"/>.
    /// </summary>
    public static Matrix Identity => s_identity;

    /// <summary>
    /// Converts a <see cref="string"/> representation of a matrix into the equivalent <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="source">
    /// The <see cref="string"/> representation of the matrix.
    /// </param>
    /// <returns>
    /// The equivalent <see cref="Matrix"/> structure.
    /// </returns>
    public static Matrix Parse(string source)
    {
        if (source != null)
        {
            if (source == "Identity")
            {
                return Identity;
            }

            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 6)
            {
                return new Matrix(
                    Convert.ToDouble(split[0], formatProvider),
                    Convert.ToDouble(split[1], formatProvider),
                    Convert.ToDouble(split[2], formatProvider),
                    Convert.ToDouble(split[3], formatProvider),
                    Convert.ToDouble(split[4], formatProvider),
                    Convert.ToDouble(split[5], formatProvider)
                );
            }
        }

        throw new FormatException($"'{source}' is not an eligible value for a Matrix.");
    }

    /// <summary>
    /// Gets a value that indicates whether this <see cref="Matrix"/> structure
    /// is an identity matrix.
    /// </summary>
    public bool IsIdentity =>
        _type == MatrixTypes.TRANSFORM_IS_IDENTITY ||
        (_m11 == 1 && _m12 == 0 && _m21 == 0 && _m22 == 1 && _offsetX == 0 && _offsetY == 0);

    /// <summary>
    /// Gets or sets the value of the first row and first column of this <see cref="Matrix"/>
    /// structure. The default value is 1.
    /// </summary>
    public double M11
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
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
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(value, 0,
                          0, 1,
                          0, 0,
                          MatrixTypes.TRANSFORM_IS_SCALING);
            }
            else
            {
                _m11 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the first row and second column of this <see cref="Matrix"/>
    /// structure. The default value is 0.
    /// </summary>
    public double M12
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _m12;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, value,
                          0, 1,
                          0, 0,
                          MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
            else
            {
                _m12 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and first column of this <see cref="Matrix"/>
    /// structure. The default value is 0.
    /// </summary>
    public double M21
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _m21;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                          value, 1,
                          0, 0,
                          MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
            else
            {
                _m21 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the second row and second column of this <see cref="Matrix"/>
    /// structure. The default value is 1.
    /// </summary>
    public double M22
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
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
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                          0, value,
                          0, 0,
                          MatrixTypes.TRANSFORM_IS_SCALING);
            }
            else
            {
                _m22 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and first column of this <see cref="Matrix"/>
    /// structure. The default value is 0.
    /// </summary>
    public double OffsetX
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _offsetX;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                          0, 1,
                          value, 0,
                          MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else
            {
                _offsetX = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the value of the third row and second column of this <see cref="Matrix"/>
    /// structure. The default value is 0.
    /// </summary>
    public double OffsetY
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _offsetY;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                          0, 1,
                          0, value,
                          MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else
            {
                _offsetY = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }

    /// <summary>
    /// Gets the determinant of this <see cref="Matrix"/> structure.
    /// </summary>
    /// <returns>
    /// The determinant of this <see cref="Matrix"/>.
    /// </returns>
    public double Determinant =>
        _type switch
        {
            MatrixTypes.TRANSFORM_IS_IDENTITY or MatrixTypes.TRANSFORM_IS_TRANSLATION => 1.0,
            MatrixTypes.TRANSFORM_IS_SCALING or MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION => (_m11 * _m22),
            _ => (_m11 * _m22) - (_m12 * _m21),
        };

    /// <summary>
    /// Gets a value that indicates whether this <see cref="Matrix"/> structure is invertible.
    /// </summary>
    /// <returns>
    /// true if the System.Windows.Media.Matrix has an inverse; otherwise, false. The default is true.
    /// </returns>
    public bool HasInverse => !DoubleUtil.IsZero(Determinant);

    /// <summary>
    /// Multiplies a <see cref="Matrix"/> structure by another <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="trans1">
    /// The first <see cref="Matrix"/> structure to multiply.
    /// </param>
    /// <param name="trans2">
    /// The second <see cref="Matrix"/> structure to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying trans1 by trans2.
    /// </returns>
    public static Matrix Multiply(Matrix trans1, Matrix trans2)
    {
        MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
        trans1.Debug_CheckType();
        return trans1;
    }

    /// <summary>
    /// Appends the specified <see cref="Matrix"/> structure to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="matrix">
    /// The <see cref="Matrix"/> structure to append to this <see cref="Matrix"/> structure.
    /// </param>
    public void Append(Matrix matrix) => this *= matrix;

    /// <summary>
    /// Inverts this <see cref="Matrix"/> structure.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="Matrix"/> structure is not invertible.
    /// </exception>
    public void Invert()
    {
        double determinant = Determinant;

        if (DoubleUtil.IsZero(determinant))
        {
            throw new InvalidOperationException("Transform is not invertible.");
        }

        // Inversion does not change the type of a matrix.
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                break;
            case MatrixTypes.TRANSFORM_IS_SCALING:
                {
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                }
                break;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                _offsetX = -_offsetX;
                _offsetY = -_offsetY;
                break;
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                {
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                    _offsetX = -_offsetX * _m11;
                    _offsetY = -_offsetY * _m22;
                }
                break;
            default:
                {
                    double invdet = 1.0 / determinant;
                    SetMatrix(_m22 * invdet,
                              -_m12 * invdet,
                              -_m21 * invdet,
                              _m11 * invdet,
                              (_m21 * _offsetY - _offsetX * _m22) * invdet,
                              (_offsetX * _m12 - _m11 * _offsetY) * invdet,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                break;
        }
    }

    /// <summary>
    /// Prepends the specified <see cref="Matrix"/> structure onto this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="matrix">
    /// The <see cref="Matrix"/> structure to prepend to this <see cref="Matrix"/> structure.
    /// </param>
    public void Prepend(Matrix matrix) => this = matrix * this;

    /// <summary>
    /// Applies a rotation of the specified angle about the origin of this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="angle">
    /// The angle of rotation.
    /// </param>
    public void Rotate(double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this *= CreateRotationRadians(angle * (Math.PI / 180.0));
    }

    /// <summary>
    /// Rotates this matrix about the specified point.
    /// </summary>
    /// <param name='angle'>
    /// The angle, in degrees, by which to rotate this matrix.
    /// </param>
    /// <param name='centerX'>
    /// The x-coordinate of the point about which to rotate this matrix.
    /// </param>
    /// <param name='centerY'>
    /// The y-coordinate of the point about which to rotate this matrix.
    /// </param>
    public void RotateAt(double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this *= CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY);
    }

    /// <summary>
    /// Prepends a rotation of the specified angle at the specified point to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="angle">
    /// The rotation angle, in degrees.
    /// </param>
    /// <param name="centerX">
    /// The x-coordinate of the rotation center.
    /// </param>
    /// <param name="centerY">
    /// The y-coordinate of the rotation center.
    /// </param>
    public void RotateAtPrepend(double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this = CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY) * this;
    }

    /// <summary>
    /// Prepends a rotation of the specified angle to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="angle">
    /// The angle of rotation to prepend.
    /// </param>
    public void RotatePrepend(double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this = CreateRotationRadians(angle * (Math.PI / 180.0)) * this;
    }

    /// <summary>
    /// Appends the specified scale vector to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="scaleX">
    /// The value by which to scale this <see cref="Matrix"/> along the x-axis.
    /// </param>
    /// <param name="scaleY">
    /// The value by which to scale this <see cref="Matrix"/> along the y-axis.
    /// </param>
    public void Scale(double scaleX, double scaleY) => this *= CreateScaling(scaleX, scaleY);

    /// <summary>
    /// Scales this <see cref="Matrix"/> by the specified amount about the specified point.
    /// </summary>
    /// <param name='scaleX'>
    /// The amount by which to scale this <see cref="Matrix"/> along the x-axis.
    /// </param>
    /// <param name='scaleY'>
    /// The amount by which to scale this <see cref="Matrix"/> along the y-axis.
    /// </param>
    /// <param name="centerX">
    /// The x-coordinate of the scale operation's center point.
    /// </param>
    /// <param name="centerY">
    /// The y-coordinate of the scale operation's center point.
    /// </param>
    internal void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        => this *= CreateScaling(scaleX, scaleY, centerX, centerY);

    /// <summary>
    /// Prepends the specified scale about the specified point of this <see cref="Matrix"/>.
    /// </summary>
    /// <param name="scaleX">
    /// The x-axis scale factor.
    /// </param>
    /// <param name="scaleY">
    /// The y-axis scale factor.
    /// </param>
    /// <param name="centerX">
    /// The x-coordinate of the point about which the scale operation is performed.
    /// </param>
    /// <param name="centerY">
    /// The y-coordinate of the point about which the scale operation is performed.
    /// </param>
    public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        => this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;

    /// <summary>
    /// Prepends the specified scale vector to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="scaleX">
    /// The value by which to scale this <see cref="Matrix"/> structure along the x-axis.
    /// </param>
    /// <param name="scaleY">
    /// The value by which to scale this <see cref="Matrix"/> structure along the y-axis.
    /// </param>
    public void ScalePrepend(double scaleX, double scaleY) => this = CreateScaling(scaleX, scaleY) * this;

    /// <summary>
    /// Changes this <see cref="Matrix"/> structure into an identity matrix.
    /// </summary>
    public void SetIdentity() => _type = MatrixTypes.TRANSFORM_IS_IDENTITY;

    /// <summary>
    /// Appends a skew of the specified degrees in the x and y dimensions to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="skewX">
    /// The angle in the x dimension by which to skew this <see cref="Matrix"/>.
    /// </param>
    /// <param name="skewY">
    /// The angle in the y dimension by which to skew this <see cref="Matrix"/>.
    /// </param>
    public void Skew(double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        this *= CreateSkewRadians(skewX * (Math.PI / 180.0), skewY * (Math.PI / 180.0));
    }

    /// <summary>
    /// Prepends a skew of the specified degrees in the x and y dimensions to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="skewX">
    /// The angle in the x dimension by which to skew this <see cref="Matrix"/>.
    /// </param>
    /// <param name="skewY">
    /// The angle in the y dimension by which to skew this <see cref="Matrix"/>.
    /// </param>
    public void SkewPrepend(double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        this = CreateSkewRadians(skewX * (Math.PI / 180.0), skewY * (Math.PI / 180.0)) * this;
    }

    /// <summary>
    /// Transforms the specified vector by this <see cref="Matrix"/>.
    /// </summary>
    /// <param name="vector">
    /// The vector to transform.
    /// </param>
    /// <returns>
    /// The result of transforming vector by this <see cref="Matrix"/>.
    /// </returns>
    public Vector Transform(Vector vector)
    {
        Vector newVector = vector;
        MultiplyVector(ref newVector._x, ref newVector._y);
        return newVector;
    }

    /// <summary>
    /// Transforms the specified vectors by this <see cref="Matrix"/>.
    /// </summary>
    /// <param name="vectors">
    /// The vectors to transform. The original vectors in the array are replaced by their transformed values.
    /// </param>
    public void Transform(Vector[] vectors)
    {
        if (vectors != null)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
            }
        }
    }

    /// <summary>
    /// Transforms the specified points by this <see cref="Matrix"/>.
    /// </summary>
    /// <param name="points">
    /// The points to transform. The original points in the array are replaced by their transformed values.
    /// </param>
    public void Transform(Point[] points)
    {
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                MultiplyPoint(ref points[i]._x, ref points[i]._y);
            }
        }
    }

    /// <summary>
    /// Transforms the specified point by the <see cref="Matrix"/> and returns the result.
    /// </summary>
    /// <param name="point">
    /// The point to transform.
    /// </param>
    /// <returns>
    /// The result of transforming point by this <see cref="Matrix"/>.
    /// </returns>
    public Point Transform(Point point)
    {
        Point newPoint = point;
        MultiplyPoint(ref newPoint._x, ref newPoint._y);
        return newPoint;
    }

    /// <summary>
    /// Appends a translation of the specified offsets to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="offsetX">
    /// The amount to offset this <see cref="Matrix"/> along the x-axis.
    /// </param>
    /// <param name="offsetY">
    /// The amount to offset this <see cref="Matrix"/> along the y-axis.
    /// </param>
    public void Translate(double offsetX, double offsetY)
    {
        //
        // / a b 0 \   / 1 0 0 \    / a      b       0 \
        // | c d 0 | * | 0 1 0 | = |  c      d       0 |
        // \ e f 1 /   \ x y 1 /    \ e+x    f+y     1 /
        //
        // (where e = _offsetX and f == _offsetY)
        //

        if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
        {
            // Values would be incorrect if matrix was created using default constructor.
            // or if SetIdentity was called on a matrix which had values.
            //
            SetMatrix(1, 0,
                      0, 1,
                      offsetX, offsetY,
                      MatrixTypes.TRANSFORM_IS_TRANSLATION);
        }
        else if (_type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
        {
            _offsetX += offsetX;
            _offsetY += offsetY;
        }
        else
        {
            _offsetX += offsetX;
            _offsetY += offsetY;

            // If matrix wasn't unknown we added a translation
            _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
        }

        Debug_CheckType();
    }

    /// <summary>
    /// Prepends a translation of the specified offsets to this <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="offsetX">
    /// The amount to offset this <see cref="Matrix"/> along the x-axis.
    /// </param>
    /// <param name="offsetY">
    /// The amount to offset this <see cref="Matrix"/> along the y-axis.
    /// </param>
    public void TranslatePrepend(double offsetX, double offsetY) => this = CreateTranslation(offsetX, offsetY) * this;

    /// <summary>
    /// Determines whether the two specified <see cref="Matrix"/> structures are identical.
    /// </summary>
    /// <param name="matrix1">
    /// The first <see cref="Matrix"/> structure to compare.
    /// </param>
    /// <param name="matrix2">
    /// The second <see cref="Matrix"/> structure to compare.
    /// </param>
    /// <returns>
    /// true if matrix1 and matrix2 are identical; otherwise, false.
    /// </returns>
    public static bool Equals(Matrix matrix1, Matrix matrix2) => matrix1 == matrix2;

    /// <summary>
    /// Determines whether the specified System.Object is a <see cref="Matrix"/>
    /// structure that is identical to this <see cref="Matrix"/>.
    /// </summary>
    /// <param name="o">
    /// The <see cref="object"/> to compare.
    /// </param>
    /// <returns>
    /// true if o is a <see cref="Matrix"/> structure that is identical to this
    /// <see cref="Matrix"/> structure; otherwise, false.
    /// </returns>
    public override bool Equals(object o) => o is Matrix matrix && this == matrix;

    /// <summary>
    /// Determines whether the specified <see cref="Matrix"/> structure is identical
    /// to this instance.
    /// </summary>
    /// <param name="value">
    /// The instance of <see cref="Matrix"/> to compare to this instance.
    /// </param>
    /// <returns>
    /// true if instances are equal; otherwise, false.
    /// </returns>
    public bool Equals(Matrix value) => this == value;

    /// <summary>
    /// Returns the hash code for this <see cref="Matrix"/> structure.
    /// </summary>
    /// <returns>
    /// The hash code for this instance.
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
                   M21.GetHashCode() ^
                   M22.GetHashCode() ^
                   OffsetX.GetHashCode() ^
                   OffsetY.GetHashCode();
        }
    }

    /// <summary>
    /// Creates a System.String representation of this <see cref="Matrix"/> structure.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> containing the <see cref="Matrix.M11"/>, <see cref="Matrix.M12"/>,
    /// <see cref="Matrix.M21"/>, <see cref="Matrix.M22"/>, <see cref="Matrix.OffsetX"/>,
    /// and <see cref="Matrix.OffsetY"/> values of this <see cref="Matrix"/>.
    /// </returns>
    public override string ToString()
    {
        // Delegate to the internal method which implements all ToString calls.
        return ConvertToString(null /* format string */, null /* format provider */);
    }

    /// <summary>
    /// Creates a <see cref="string"/> representation of this <see cref="Matrix"/> structure with 
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="provider">
    /// The culture-specific formatting information.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> containing the <see cref="M11"/>, <see cref="M12"/>, <see cref="M21"/>, 
    /// <see cref="M22"/>, <see cref="OffsetX"/>, and <see cref="OffsetY"/> values of this <see cref="Matrix"/>.
    /// </returns>
    public string ToString(IFormatProvider provider) => ConvertToString(null /* format string */, provider);

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
    /// Multiplies a <see cref="Matrix"/> structure by another <see cref="Matrix"/> structure.
    /// </summary>
    /// <param name="trans1">
    /// The first <see cref="Matrix"/> structure to multiply.
    /// </param>
    /// <param name="trans2">
    /// The second <see cref="Matrix"/> structure to multiply.
    /// </param>
    /// <returns>
    /// The result of multiplying trans1 by trans2.
    /// </returns>
    public static Matrix operator *(Matrix trans1, Matrix trans2)
    {
        MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
        trans1.Debug_CheckType();
        return trans1;
    }

    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
        {
            return matrix1.IsIdentity == matrix2.IsIdentity;
        }
        else
        {
            return matrix1.M11.Equals(matrix2.M11) &&
                   matrix1.M12.Equals(matrix2.M12) &&
                   matrix1.M21.Equals(matrix2.M21) &&
                   matrix1.M22.Equals(matrix2.M22) &&
                   matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                   matrix1.OffsetY.Equals(matrix2.OffsetY);
        }
    }

    public static bool operator !=(Matrix matrix1, Matrix matrix2) => !(matrix1 == matrix2);

    /// <summary>
    /// MultiplyVector
    /// </summary>
    internal void MultiplyVector(ref double x, ref double y)
    {
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x *= _m11;
                y *= _m22;
                break;
            default:
                double xadd = y * _m21;
                double yadd = x * _m12;
                x *= _m11;
                x += xadd;
                y *= _m22;
                y += yadd;
                break;
        }
    }

    /// <summary>
    /// MultiplyPoint
    /// </summary>
    internal void MultiplyPoint(ref double x, ref double y)
    {
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                return;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x += _offsetX;
                y += _offsetY;
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
                x *= _m11;
                y *= _m22;
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x *= _m11;
                x += _offsetX;
                y *= _m22;
                y += _offsetY;
                break;
            default:
                double xadd = y * _m21 + _offsetX;
                double yadd = x * _m12 + _offsetY;
                x *= _m11;
                x += xadd;
                y *= _m22;
                y += yadd;
                break;
        }
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
    internal string ConvertToString(string format, IFormatProvider provider)
    {
        if (IsIdentity)
        {
            return "Identity";
        }

        char separator = ',';

        return string.Format(provider,
                             "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}",
                             separator,
                             _m11,
                             _m12,
                             _m21,
                             _m22,
                             _offsetX,
                             _offsetY);
    }

    /// <summary>
    /// Creates a skew transform
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    internal static Matrix CreateSkewRadians(double skewX, double skewY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(1.0, Math.Tan(skewY),
                         Math.Tan(skewX), 1.0,
                         0.0, 0.0,
                         MatrixTypes.TRANSFORM_IS_UNKNOWN);

        return matrix;
    }

    /// <summary>
    /// Sets the transformation to the given translation specified by the offset vector.
    /// </summary>
    /// <param name='offsetX'>The offset in X</param>
    /// <param name='offsetY'>The offset in Y</param>
    internal static Matrix CreateTranslation(double offsetX, double offsetY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(1, 0,
                         0, 1,
                         offsetX, offsetY,
                         MatrixTypes.TRANSFORM_IS_TRANSLATION);

        return matrix;
    }

    /// <summary>
    /// Creates a scaling transform around the given point
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name='centerX'>The centerX of scaling</param>
    /// <param name='centerY'>The centerY of scaling</param>
    internal static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(scaleX, 0,
                         0, scaleY,
                         centerX - scaleX * centerX, centerY - scaleY * centerY,
                         MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION);

        return matrix;
    }

    /// <summary>
    /// Creates a scaling transform around the origin
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    internal static Matrix CreateScaling(double scaleX, double scaleY)
    {
        Matrix matrix = new Matrix();
        matrix.SetMatrix(scaleX, 0,
                         0, scaleY,
                         0, 0,
                         MatrixTypes.TRANSFORM_IS_SCALING);
        return matrix;
    }

    /// <summary>
    /// Creates a rotation transformation about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in radians</param>
    internal static Matrix CreateRotationRadians(double angle) => CreateRotationRadians(angle, /* centerX = */ 0, /* centerY = */ 0);

    /// <summary>
    /// Creates a rotation transformation about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specifed in radians</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    internal static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
    {
        Matrix matrix = new Matrix();

        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        double dx = (centerX * (1.0 - cos)) + (centerY * sin);
        double dy = (centerY * (1.0 - cos)) - (centerX * sin);

        matrix.SetMatrix(cos, sin,
                         -sin, cos,
                         dx, dy,
                         MatrixTypes.TRANSFORM_IS_UNKNOWN);

        return matrix;
    }

    /// <summary>
    /// Sets the transformation to the identity.
    /// </summary>
    private static Matrix CreateIdentity()
    {
        Matrix matrix = new Matrix();
        matrix.SetMatrix(1, 0,
                         0, 1,
                         0, 0,
                         MatrixTypes.TRANSFORM_IS_IDENTITY);
        return matrix;
    }

    ///<summary>
    /// Sets the transform to
    ///             / m11, m12, 0 \
    ///             | m21, m22, 0 |
    ///             \ offsetX, offsetY, 1 /
    /// where offsetX, offsetY is the translation.
    ///</summary>
    private void SetMatrix(double m11, double m12,
                           double m21, double m22,
                           double offsetX, double offsetY,
                           MatrixTypes type)
    {
        _m11 = m11;
        _m12 = m12;
        _m21 = m21;
        _m22 = m22;
        _offsetX = offsetX;
        _offsetY = offsetY;
        _type = type;
    }

    /// <summary>
    /// Set the type of the matrix based on its current contents
    /// </summary>
    private void DeriveMatrixType()
    {
        _type = 0;

        // Now classify our matrix.
        if (!(_m21 == 0 && _m12 == 0))
        {
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            return;
        }

        if (!(_m11 == 1 && _m22 == 1))
        {
            _type = MatrixTypes.TRANSFORM_IS_SCALING;
        }

        if (!(_offsetX == 0 && _offsetY == 0))
        {
            _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
        }

        if (0 == (_type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)))
        {
            // We have an identity matrix.
            _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        }
    }

    /// <summary>
    /// Asserts that the matrix tag is one of the valid options and
    /// that coefficients are correct.   
    /// </summary>
    [Conditional("DEBUG")]
    private void Debug_CheckType()
    {
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                return;
            case MatrixTypes.TRANSFORM_IS_UNKNOWN:
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                Debug.Assert(_offsetX == 0);
                Debug.Assert(_offsetY == 0);
                return;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                Debug.Assert(_m11 == 1);
                Debug.Assert(_m22 == 1);
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                return;
            default:
                Debug.Assert(false);
                return;
        }
    }

    /// <summary>
    /// Efficient but conservative test for identity.  Returns
    /// true if the the matrix is identity.  If it returns false
    /// the matrix may still be identity.
    /// </summary>
    private bool IsDistinguishedIdentity => _type == MatrixTypes.TRANSFORM_IS_IDENTITY;

    // The hash code for a matrix is the xor of its element's hashes.
    // Since the identity matrix has 2 1's and 4 0's its hash is 0.
    private const int c_identityHashCode = 0;

    internal double _m11;
    internal double _m12;
    internal double _m21;
    internal double _m22;
    internal double _offsetX;
    internal double _offsetY;
    internal MatrixTypes _type;
}
