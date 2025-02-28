
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
using System.Diagnostics;
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Defines functionality that enables transformations in a two-dimensional plane.
    /// </summary>
    [TypeConverter(typeof(TransformConverter))]
    public abstract class Transform : GeneralTransform
    {
        private Matrix? _matrix;

        internal Transform() { }

        /// <summary>
        /// Creates a new <see cref="Transform"/> from the specified string representation of a transformation matrix.
        /// </summary>
        /// <param name="source">
        /// Six comma-delimited <see cref="double"/> values that describe the new <see cref="Transform"/>.
        /// </param>
        /// <returns>
        /// A new transform that is constructed from the specified string.
        /// </returns>
        public static Transform Parse(string source) => Parsers.ParseTransform(source, CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets an identity transform.
        /// </summary>
        /// <returns>
        /// An identity transform.
        /// </returns>
        public static Transform Identity { get; } = MakeIdentityTransform();

        private static MatrixTransform MakeIdentityTransform()
        {
            var identity = new MatrixTransform(Matrix.Identity);
            identity.Seal();
            return identity;
        }

        internal Matrix Matrix => _matrix ??= GetMatrixCore();

        private protected abstract Matrix GetMatrixCore();

        ///<summary>
        /// Returns true if transformation if the transformation is definitely an identity.  There are cases where it will
        /// return false because of computational error or presence of animations (And we're interpolating through a
        /// transient identity) -- this is intentional.  This property is used internally only.  If you need to check the
        /// current matrix value for identity, use Transform.Value.Identity.
        ///</summary>
        internal abstract bool IsIdentity { get; }

        internal static bool IsIdentityTransform(Transform transform)
        {
            Debug.Assert(transform is not null);
            return transform == Identity || transform.IsIdentity;
        }

        /// <summary>
        /// Attempts to transform the specified point and returns a value that indicates
        /// whether the transformation was successful.
        /// </summary>
        /// <param name="inPoint">
        /// The point to transform.
        /// </param>
        /// <param name="outPoint">
        /// The result of transforming inPoint.
        /// </param>
        /// <returns>
        /// true if inPoint was transformed; otherwise, false.
        /// </returns>
        public override bool TryTransform(Point inPoint, out Point outPoint)
        {
            Matrix m = Matrix;
            outPoint = m.Transform(inPoint);
            return true;
        }

        /// <summary>
        /// Transforms the specified bounding box and returns an axis-aligned bounding box
        /// that is exactly large enough to contain it.
        /// </summary>
        /// <param name="rect">
        /// The bounding box to transform.
        /// </param>
        /// <returns>
        /// The smallest axis-aligned bounding box that can contain the transformed rect.
        /// </returns>
        public override Rect TransformBounds(Rect rect)
        {
            Matrix matrix = Matrix;
            MatrixUtil.TransformRect(ref rect, ref matrix);
            return rect;
        }

        /// <summary>
        /// Gets the inverse of this transform, if it exists.
        /// </summary>
        /// <returns>
        /// The inverse of this transform, if it exists; otherwise, null.
        /// </returns>
        public override GeneralTransform Inverse
        {
            get
            {
                Matrix matrix = Matrix;
                if (!matrix.HasInverse)
                {
                    return null;
                }

                matrix.Invert();
                return new MatrixTransform(matrix);
            }
        }

        internal event EventHandler Changed;

        internal static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Transform)d).OnTransformChanged();
        }

        internal void OnTransformChanged()
        {
            _matrix = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
