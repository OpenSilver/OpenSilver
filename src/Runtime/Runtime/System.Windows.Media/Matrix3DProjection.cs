
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
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace System.Windows.Media
{
    /// <summary>
    /// Enables you to apply a <see cref="Matrix3D"/> to an object.
    /// </summary>
    [ContentProperty(nameof(ProjectionMatrix))]
    public sealed class Matrix3DProjection : Projection
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="Matrix3DProjection"/> class.
        /// </summary>
        public Matrix3DProjection()
        {
        }

        /// <summary>
        /// Identifies the <see cref="ProjectionMatrix"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProjectionMatrixProperty =
            DependencyProperty.Register(
                nameof(ProjectionMatrix),
                typeof(Matrix3D),
                typeof(Matrix3DProjection),
                new PropertyMetadata(Matrix3D.Identity, OnProjectionMatrixChanged));

        /// <summary>
        /// Gets or sets the <see cref="Matrix3D"/> that is used for the projection
        /// that is applied to the object.
        /// </summary>
        public Matrix3D ProjectionMatrix
        {
            get { return (Matrix3D)GetValue(ProjectionMatrixProperty); }
            set { SetValueInternal(ProjectionMatrixProperty, value); }
        }

        private static void OnProjectionMatrixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Matrix3DProjection projection = (Matrix3DProjection)d;
            projection.RaiseChanged();
        }

        /// <summary>
        /// Gets the CSS transform string for this matrix projection.
        /// </summary>
        /// <param name="elementWidth">The width of the element being projected.</param>
        /// <param name="elementHeight">The height of the element being projected.</param>
        /// <returns>A CSS matrix3d transform string.</returns>
        internal override string GetCssTransform(double elementWidth, double elementHeight)
        {
            Matrix3D matrix = ProjectionMatrix;
            
            if (matrix.IsIdentity)
            {
                return string.Empty;
            }

            // CSS matrix3d uses column-major order:
            // matrix3d(m11, m21, m31, m41, m12, m22, m32, m42, m13, m23, m33, m43, m14, m24, m34, m44)
            return string.Format(CultureInfo.InvariantCulture,
                "matrix3d({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15})",
                matrix.M11, matrix.M21, matrix.M31, matrix.OffsetX,
                matrix.M12, matrix.M22, matrix.M32, matrix.OffsetY,
                matrix.M13, matrix.M23, matrix.M33, matrix.OffsetZ,
                matrix.M14, matrix.M24, matrix.M34, matrix.M44);
        }
    }
}

