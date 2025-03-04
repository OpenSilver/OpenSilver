
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

namespace System.Windows.Media
{
    /// <summary>
    /// Creates an arbitrary affine matrix transformation that is used to manipulate
    /// objects or coordinate systems in a two-dimensional plane.
    /// </summary>
    public sealed class MatrixTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixTransform"/> class.
        /// </summary>
        public MatrixTransform() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixTransform"/> class with the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">
        /// The transformation matrix of the new <see cref="MatrixTransform"/>.
        /// </param>
        public MatrixTransform(Matrix matrix)
        {
            Matrix = matrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixTransform"/> class with the specified transformation matrix values.
        /// </summary>
        /// <param name="m11">
        /// The value at position (1, 1) in the transformation matrix.
        /// </param>
        /// <param name="m12">
        /// The value at position (1, 2) in the transformation matrix.
        /// </param>
        /// <param name="m21">
        /// The value at position (2, 1) in the transformation matrix.
        /// </param>
        /// <param name="m22">
        /// The value at position (2, 2) in the transformation matrix.
        /// </param>
        /// <param name="offsetX">
        /// The X-axis translation factor, which is located at position (3,1) in the transformation matrix.
        /// </param>
        /// <param name="offsetY">
        /// The Y-axis translation factor, which is located at position (3,2) in the transformation matrix.
        /// </param>
        public MatrixTransform(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            Matrix = new Matrix(m11, m12, m21, m22, offsetX, offsetY);
        }

        /// <summary>
        /// Identifies the <see cref="Matrix"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MatrixProperty =
            DependencyProperty.Register(
                nameof(Matrix),
                typeof(Matrix),
                typeof(MatrixTransform),
                new PropertyMetadata(Matrix.Identity, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="Media.Matrix"/> that defines this transformation.
        /// </summary>
        /// <returns>
        /// The <see cref="Media.Matrix"/> structure that defines this transformation. The
        /// default is an identity <see cref="Media.Matrix"/>. An identity matrix has a
        /// value of 1 in coefficients [1,1], [2,2], and [3,3]; and a value of 0 in the rest
        /// of the coefficients.
        /// </returns>
        public new Matrix Matrix
        {
            get => (Matrix)GetValue(MatrixProperty);
            set => SetValueInternal(MatrixProperty, value);
        }

        /// <inheritdoc />
        public override Matrix Value => Matrix;

        internal override bool IsIdentity => Matrix.IsIdentity;

        internal static string MatrixToHtmlString(Matrix m)
        {
            string m11 = Math.Round(m.M11, 4).ToInvariantString();
            string m12 = Math.Round(m.M12, 4).ToInvariantString();
            string m21 = Math.Round(m.M21, 4).ToInvariantString();
            string m22 = Math.Round(m.M22, 4).ToInvariantString();
            string offsetX = Math.Round(m.OffsetX, 4).ToInvariantString();
            string offsetY = Math.Round(m.OffsetY, 4).ToInvariantString();
            return $"matrix({m11},{m12},{m21},{m22},{offsetX},{offsetY})";
        }
    }
}
