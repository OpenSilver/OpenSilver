
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a 3x3 affine transformation matrix used for transformations in two-dimensional
    /// space.
    /// </summary>
    public struct Matrix : IFormattable
    {
        // the transform is identity by default
        private static Matrix s_identity = CreateIdentity();

        #region Constructors

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
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;

            // We will detect EXACT identity, scale, translation or
            // scale+translation and use special case algorithms.
            DeriveMatrixType();
        }

        /// <summary>
        /// Gets an identity <see cref="Matrix"/>.
        /// </summary>
        public static Matrix Identity
        {
            get => s_identity;
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the culture "en-US"
        /// <param name="source"> string with Matrix data </param>
        /// </summary>
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

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets a value that indicates whether this <see cref="Matrix"/> structure
        /// is an identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get => (_type == MatrixTypes.TRANSFORM_IS_IDENTITY ||
                        (_m11 == 1 && _m12 == 0 && _m21 == 0 && _m22 == 1 && _offsetX == 0 && _offsetY == 0));
        }

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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Transforms the specified point by the <see cref="Matrix"/> and returns
        /// the result.
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
        /// Determines whether the specified System.Object is a <see cref="Matrix"/>
        /// structure that is identical to this <see cref="Matrix"/>.
        /// </summary>
        /// <param name="o">
        /// The <see cref="Object"/> to compare.
        /// </param>
        /// <returns>
        /// true if o is a <see cref="Matrix"/> structure that is identical to this
        /// <see cref="Matrix"/> structure; otherwise, false.
        /// </returns>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Matrix))
            {
                return false;
            }

            return this == (Matrix)o;
        }

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
        public bool Equals(Matrix value)
        {
            return this == value;
        }

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
        /// A <see cref="String"/> containing the <see cref="Matrix.M11"/>, <see cref="Matrix.M12"/>,
        /// <see cref="Matrix.M21"/>, <see cref="Matrix.M22"/>, <see cref="Matrix.OffsetX"/>,
        /// and <see cref="Matrix.OffsetY"/> values of this <see cref="Matrix"/>.
        /// </returns>
        public override string ToString()
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, null /* format provider */);
        }

        /// <summary>
        /// Creates a <see cref="String"/> representation of this <see cref="Matrix"/> structure
        /// with culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// The culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A <see cref="String"/> containing the <see cref="Matrix.M11"/>, <see cref="Matrix.M12"/>,
        /// <see cref="Matrix.M21"/>, <see cref="Matrix.M22"/>, <see cref="Matrix.OffsetX"/>,
        /// and <see cref="Matrix.OffsetY"/> values of this <see cref="Matrix"/>.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, provider);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(format, provider);
        }

        #endregion Public Methods

        #region Operators

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11 == matrix2.M11 &&
                       matrix1.M12 == matrix2.M12 &&
                       matrix1.M21 == matrix2.M21 &&
                       matrix1.M22 == matrix2.M22 &&
                       matrix1.OffsetX == matrix2.OffsetX &&
                       matrix1.OffsetY == matrix2.OffsetY;
            }
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        #endregion Operators

        #region Internal Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        internal double Determinant
        {
            get
            {
                switch (_type)
                {
                    case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return 1.0;
                    case MatrixTypes.TRANSFORM_IS_SCALING:
                    case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return (_m11 * _m22);
                    default:
                        return (_m11 * _m22) - (_m12 * _m21);
                }
            }
        }

        /// <summary>
        /// HasInverse Property - returns true if this matrix is invertable, false otherwise.
        /// </summary>
        internal bool HasInverse
        {
            get
            {
                return !IsZero(Determinant); /* smallest such that 1.0+DBL_EPSILON != 1.0 */
            }
        }

        internal static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        #endregion Internal Properties

        #region Internal Methods

        /// <summary>
        /// Replaces matrix with the inverse of the transformation.  This will throw an InvalidOperationException
        /// if !HasInverse
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This will throw an InvalidOperationException if the matrix is non-invertable
        /// </exception>
        internal void Invert()
        {
            double determinant = Determinant;

            if (IsZero(determinant))
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
        /// Multiply
        /// </summary>
        internal static Matrix Multiply(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            trans1.Debug_CheckType();
            return trans1;
        }

        /// <summary>
        /// Compares two Matrix instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        internal static bool Equals(Matrix matrix1, Matrix matrix2)
        {
            return matrix1 == matrix2;
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

            return String.Format(provider,
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
        /// Translates this matrix
        /// </summary>
        /// <param name='offsetX'>The offset in the x dimension</param>
        /// <param name='offsetY'>The offset in the y dimension</param>
        internal void Translate(double offsetX, double offsetY)
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
        /// Skews this matrix
        /// </summary>
        /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
        /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
        internal void Skew(double skewX, double skewY)
        {
            skewX %= 360;
            skewY %= 360;
            this = Matrix.Multiply(this, CreateSkewRadians(skewX * (Math.PI / 180.0),
                                                           skewY * (Math.PI / 180.0)));
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
        /// Scales this matrix around the center provided
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        /// <param name="centerX">The centerX about which to scale</param>
        /// <param name="centerY">The centerY about which to scale</param>
        internal void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            this = Matrix.Multiply(this, CreateScaling(scaleX, scaleY, centerX, centerY));
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
        /// Rotates this matrix about the given point
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in degrees</param>
        /// <param name='centerX'>The centerX of rotation</param>
        /// <param name='centerY'>The centerY of rotation</param>
        internal void RotateAt(double angle, double centerX, double centerY)
        {
            angle %= 360.0; // Doing the modulo before converting to radians reduces total error
            this = Matrix.Multiply(this, CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY));
        }

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

        #endregion Internal Methods

        #region Private Methods

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
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = type;
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

        #endregion Private Methods

        #region Private Properties and Fields

        /// <summary>
        /// Efficient but conservative test for identity.  Returns
        /// true if the the matrix is identity.  If it returns false
        /// the matrix may still be identity.
        /// </summary>
        private bool IsDistinguishedIdentity
        {
            get
            {
                return _type == MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
        }

        // The hash code for a matrix is the xor of its element's hashes.
        // Since the identity matrix has 2 1's and 4 0's its hash is 0.
        private const int c_identityHashCode = 0;

        private const double DBL_EPSILON = 2.2204460492503131e-016;

        #endregion Private Properties and Fields

        internal double _m11;
        internal double _m12;
        internal double _m21;
        internal double _m22;
        internal double _offsetX;
        internal double _offsetY;
        internal MatrixTypes _type;
    }

    // MatrixTypes
    [Flags]
    internal enum MatrixTypes
    {
        TRANSFORM_IS_IDENTITY = 0,
        TRANSFORM_IS_TRANSLATION = 1,
        TRANSFORM_IS_SCALING = 2,
        TRANSFORM_IS_UNKNOWN = 4
    }

    internal static class MatrixUtil
    {
        /// <summary>
        /// TransformRect - Internal helper for perf
        /// </summary>
        /// <param name="rect"> The Rect to transform. </param>
        /// <param name="matrix"> The Matrix with which to transform the Rect. </param>
        internal static void TransformRect(ref Rect rect, ref Matrix matrix)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            MatrixTypes matrixType = matrix._type;

            // If the matrix is identity, don't worry.
            if (matrixType == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // Scaling
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_SCALING))
            {
                rect._x *= matrix._m11;
                rect._y *= matrix._m22;
                rect._width *= matrix._m11;
                rect._height *= matrix._m22;

                // Ensure the width is always positive.  For example, if there was a reflection about the
                // y axis followed by a translation into the visual area, the width could be negative.
                if (rect._width < 0.0)
                {
                    rect._x += rect._width;
                    rect._width = -rect._width;
                }

                // Ensure the height is always positive.  For example, if there was a reflection about the
                // x axis followed by a translation into the visual area, the height could be negative.
                if (rect._height < 0.0)
                {
                    rect._y += rect._height;
                    rect._height = -rect._height;
                }
            }

            // Translation
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_TRANSLATION))
            {
                // X
                rect._x += matrix._offsetX;

                // Y
                rect._y += matrix._offsetY;
            }

            if (matrixType == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                // Al Bunny implementation.
                Point point0 = matrix.Transform(rect.TopLeft);
                Point point1 = matrix.Transform(rect.TopRight);
                Point point2 = matrix.Transform(rect.BottomRight);
                Point point3 = matrix.Transform(rect.BottomLeft);

                // Width and height is always positive here.
                rect._x = Math.Min(Math.Min(point0.X, point1.X), Math.Min(point2.X, point3.X));
                rect._y = Math.Min(Math.Min(point0.Y, point1.Y), Math.Min(point2.Y, point3.Y));

                rect._width = Math.Max(Math.Max(point0.X, point1.X), Math.Max(point2.X, point3.X)) - rect._x;
                rect._height = Math.Max(Math.Max(point0.Y, point1.Y), Math.Max(point2.Y, point3.Y)) - rect._y;
            }
        }

        /// <summary>
        /// Multiplies two transformations, where the behavior is matrix1 *= matrix2.
        /// This code exists so that we can efficient combine matrices without copying
        /// the data around, since each matrix is 52 bytes.
        /// To reduce duplication and to ensure consistent behavior, this is the
        /// method which is used to implement Matrix * Matrix as well.
        /// </summary>
        internal static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
        {
            MatrixTypes type1 = matrix1._type;
            MatrixTypes type2 = matrix2._type;

            // Check for idents

            // If the second is ident, we can just return
            if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // If the first is ident, we can just copy the memory across.
            if (type1 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix1 = matrix2;
                return;
            }

            // Optimize for translate case, where the second is a translate
            if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // 2 additions
                matrix1._offsetX += matrix2._offsetX;
                matrix1._offsetY += matrix2._offsetY;

                // If matrix 1 wasn't unknown we added a translation
                if (type1 != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }

                return;
            }

            // Check for the first value being a translate
            if (type1 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // Save off the old offsets
                double offsetX = matrix1._offsetX;
                double offsetY = matrix1._offsetY;

                // Copy the matrix
                matrix1 = matrix2;

                matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;

                if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
                else
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
                return;
            }

            // The following code combines the type of the transformations so that the high nibble
            // is "this"'s type, and the low nibble is mat's type.  This allows for a switch rather
            // than nested switches.

            // trans1._type |  trans2._type
            //  7  6  5  4   |  3  2  1  0
            int combinedType = ((int)type1 << 4) | (int)type2;

            switch (combinedType)
            {
                case 34:  // S * S
                    // 2 multiplications
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    return;

                case 35:  // S * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._offsetX;
                    matrix1._offsetY = matrix2._offsetY;

                    // Transform set to Translate and Scale
                    matrix1._type = MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING;
                    return;

                case 50: // S|T * S
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX *= matrix2._m11;
                    matrix1._offsetY *= matrix2._m22;
                    return;

                case 51: // S|T * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                    matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                    return;
                case 36: // S * U
                case 52: // S|T * U
                case 66: // U * S
                case 67: // U * S|T
                case 68: // U * U
                    matrix1 = new Matrix(
                        matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                        matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,

                        matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                        matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,

                        matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                        matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                    return;
#if DEBUG
                default:
                    Debug.Fail("Matrix multiply hit an invalid case: " + combinedType);
                    break;
#endif
            }
        }
    }
}