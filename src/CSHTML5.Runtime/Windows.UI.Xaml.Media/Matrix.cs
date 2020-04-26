

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a 3x3 affine transformation matrix used for transformations in
    /// two-dimensional space.
    /// </summary>
    public partial struct Matrix
    {
        bool _isInitialized; // This is due to the fact that we cannot set default values in structs, and we cannot implement a default constructor in structs, so we need to initialize with the Matrix so that it becomes the Identity matrix by default.
        double _m11;
        double _m12;
        double _m21;
        double _m22;
        double _offsetX;
        double _offsetY;

        private void Initialize()
        {
            this._m11 = 1;
            this._m21 = 0;
            this._m12 = 0;
            this._m22 = 1;
            this._offsetX = 0;
            this._offsetY = 0;
            this._isInitialized = true;
        }

        /// <summary>
        /// Initializes a Matrix structure.
        /// </summary>
        /// <param name="m11">The Matrix structure's Matrix.M11 coefficient.</param>
        /// <param name="m12">The Matrix structure's Matrix.M12 coefficient.</param>
        /// <param name="m21">The Matrix structure's Matrix.M21 coefficient.</param>
        /// <param name="m22">The Matrix structure's Matrix.M22 coefficient.</param>
        /// <param name="offsetX">The Matrix structure's Matrix.OffsetX coefficient.</param>
        /// <param name="offsetY">The Matrix structure's Matrix.OffsetY coefficient.</param>
        public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this._m11 = m11;
            this._m21 = m21;
            this._m12 = m12;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._isInitialized = true;
        }

        /// <summary>
        /// Determines whether the two specified Matrix structures are not identical.
        /// </summary>
        /// <param name="matrix1">The first Matrix structure to compare.</param>
        /// <param name="matrix2">The second Matrix structure to compare.</param>
        /// <returns>true if matrix1 and matrix2 are not identical; otherwise, false.</returns>
        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !matrix1.Equals(matrix2);
        }

        /// <summary>
        /// Determines whether the two specified Matrix structures are identical.
        /// </summary>
        /// <param name="matrix1">The first Matrix structure to compare.</param>
        /// <param name="matrix2">The second Matrix structure to compare.</param>
        /// <returns>true if matrix1 and matrix2 are identical; otherwise, false.</returns>
        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return matrix1.Equals(matrix2);
        }

        /// <summary>
        /// Gets an identity Matrix.
        /// </summary>
        public static Matrix Identity
        {
            get
            {
                return new Matrix();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this Matrix structure is an identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return this.Equals(Matrix.Identity);
            }
        }

        /// <summary>
        /// Gets or sets the value of the first row and first column of this Matrix structure. The default value is 1.
        /// </summary>
        public double M11
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _m11;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _m11 = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the first row and second column of this Matrix structure. The default value is 0.
        /// </summary>
        public double M12
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _m12;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _m12 = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the second row and first column of this Matrix structure. The default value is 0.
        /// </summary>
        public double M21
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _m21;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _m21 = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the second row and second column of this Matrix structure. The default value is 1.
        /// </summary>
        public double M22
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _m22;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _m22 = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the third row and first column of this Matrix structure. The default value is 0.
        /// </summary>
        public double OffsetX
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _offsetX;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _offsetX = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the third row and second column of this Matrix structure. The default value is 0.
        /// </summary>
        public double OffsetY
        {
            get
            {
                if (!_isInitialized)
                    Initialize();
                return _offsetY;
            }
            set
            {
                if (!_isInitialized)
                    Initialize();
                _offsetY = value;
            }
        }

        /// <summary>
        /// Determines whether the specified Matrix structure is identical to this instance.
        /// </summary>
        /// <param name="value">The instance of Matrix to compare to this instance.</param>
        /// <returns>true if instances are equal; otherwise, false.</returns>
        public bool Equals(Matrix value)
        {
            if (!this._isInitialized)
                this.Initialize();

            if (!value._isInitialized)
                value.Initialize();

            return (value._m11 == this._m11
                && value._m12 == this._m12
                && value._m21 == this._m21
                && value._m22 == this._m22
                && value._offsetX == this._offsetX
                && value.OffsetY == this.OffsetY);
        }

        /// <summary>
        /// Determines whether the specified System.Object is a Matrix structure that is identical to this Matrix.
        /// </summary>
        /// <param name="o">The System.Object to compare.</param>
        /// <returns>true if o is a Matrix structure that is identical to this Matrix structure; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if (!this._isInitialized)
                this.Initialize();

            if (o is Matrix && !((Matrix)o)._isInitialized)
                ((Matrix)o).Initialize();

            return (o is Matrix
                && ((Matrix)o)._m11 == this._m11
                && ((Matrix)o)._m12 == this._m12
                && ((Matrix)o)._m21 == this._m21
                && ((Matrix)o)._m22 == this._m22
                && ((Matrix)o)._offsetX == this._offsetX
                && ((Matrix)o).OffsetY == this.OffsetY);
        }

        /// <summary>
        /// Returns the hash code for this Matrix structure.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!this._isInitialized)
                this.Initialize();

            return this._m11.GetHashCode()
                ^ this._m12.GetHashCode()
                ^ this._m21.GetHashCode()
                ^ this._m22.GetHashCode()
                ^ this._offsetX.GetHashCode()
                ^ this._offsetY.GetHashCode();
        }

        /// <summary>
        /// Creates a System.String representation of this Matrix
        /// structure.
        /// </summary>
        /// <returns>A System.String containing the Matrix.M11, Matrix.M12,
        /// Matrix.M21, Matrix.M22, Matrix.OffsetX,
        /// and Matrix.OffsetY values of this Matrix.</returns>
        public override string ToString()
        {
            if (!this._isInitialized)
                this.Initialize();

            if (this.IsIdentity)
                return "Identity";
            else
                return this._m11.ToString() + ";"
                    + this._m12.ToString() + ";"
                    + this._m21.ToString() + ";"
                    + this._m22.ToString() + ";"
                    + this._offsetX.ToString() + ";"
                    + this._offsetY.ToString();
        }



        static Matrix()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Matrix), s => Parse(s));
        }

        public static Matrix Parse(string matrixAsString)
        {
            if (matrixAsString == "Identity")
                return Identity;
            
            string[] splittedString = matrixAsString.Split(new[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);

            if (splittedString.Length == 6)
            {
                double m11, m12, m21, m22, offsetX, offsetY;
#if OPENSILVER
                if (double.TryParse(splittedString[0], NumberStyles.Any, CultureInfo.InvariantCulture, out m11) && 
                    double.TryParse(splittedString[1], NumberStyles.Any, CultureInfo.InvariantCulture, out m12) && 
                    double.TryParse(splittedString[2], NumberStyles.Any, CultureInfo.InvariantCulture, out m21) && 
                    double.TryParse(splittedString[3], NumberStyles.Any, CultureInfo.InvariantCulture, out m22) && 
                    double.TryParse(splittedString[4], NumberStyles.Any, CultureInfo.InvariantCulture, out offsetX) && 
                    double.TryParse(splittedString[5], NumberStyles.Any, CultureInfo.InvariantCulture, out offsetY))
#else
                if (double.TryParse(splittedString[0], out m11) &&
                    double.TryParse(splittedString[1], out m12) &&
                    double.TryParse(splittedString[2], out m21) &&
                    double.TryParse(splittedString[3], out m22) &&
                    double.TryParse(splittedString[4], out offsetX) &&
                    double.TryParse(splittedString[5], out offsetY))
#endif
                    return new Matrix(m11, m12, m21, m22, offsetX, offsetY);
            }

            throw new FormatException(matrixAsString + " is not an eligible value for a Matrix");
        }

        //
        // Summary:
        //     Creates a System.String representation of this Matrix
        //     structure with culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     The culture-specific formatting information.
        //
        // Returns:
        //     A System.String containing the Matrix.M11, Matrix.M12,
        //     Matrix.M21, Matrix.M22, Matrix.OffsetX,
        //     and Matrix.OffsetY values of this Matrix.
        //public string ToString(IFormatProvider provider)
        //{
        //    throw new NotImplementedException();
        //}
        //
        // Summary:
        //     Transforms the specified point by the Matrix and returns
        //     the result.
        //
        // Parameters:
        //   point:
        //     The point to transform.
        //
        // Returns:
        //     The result of transforming point by this Matrix.
        //public Point Transform(Point point)
        //{
        //    throw new NotImplementedException();
        //}

#if WORKINPROGRESS
        internal void MultiplyPoint(ref double x, ref double y)
        {
            double xadd = y * _m21 + _offsetX;
            double yadd = x * _m12 + _offsetY;
            x *= _m11;
            x += xadd;
            y *= _m22;
            y += yadd;
        }

        public Point Transform(Point point)
        {
            Point newPoint = point;
            MultiplyPoint(ref newPoint._x, ref newPoint._y);
            return newPoint;
        }
#endif
    }

}