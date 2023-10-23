
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

        internal MatrixTransform(Matrix matrix)
        {
            Matrix = matrix;
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
        public Matrix Matrix
        {
            get => (Matrix)GetValue(MatrixProperty);
            set => SetValue(MatrixProperty, value);
        }

        internal override Matrix ValueInternal => Matrix;

        internal override bool IsIdentity => Matrix.IsIdentity;

        internal static string MatrixToHtmlString(Matrix m)
            => $"matrix({m.M11.ToInvariantString()}, {m.M12.ToInvariantString()}, {m.M21.ToInvariantString()}, {m.M22.ToInvariantString()}, {m.OffsetX.ToInvariantString()}, {m.OffsetY.ToInvariantString()})";
    }
}
