
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

namespace System.Windows.Media
{
    /// <summary>
    /// Defines functionality that enables transformations in a two-dimensional plane.
    /// </summary>
    public abstract class Transform : GeneralTransform
    {
        private Matrix? _matrix;

        internal Transform() { }

        internal Matrix Matrix => _matrix ??= GetMatrixCore();

        private protected abstract Matrix GetMatrixCore();

        ///<summary>
        /// Returns true if transformation if the transformation is definitely an identity.  There are cases where it will
        /// return false because of computational error or presence of animations (And we're interpolating through a
        /// transient identity) -- this is intentional.  This property is used internally only.  If you need to check the
        /// current matrix value for identity, use Transform.Value.Identity.
        ///</summary>
        internal abstract bool IsIdentity { get; }

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

        /// <summary>
        /// Parse - returns an instance converted from the provided string
        /// using the current culture
        /// <param name="source"> string with Transform data </param>
        /// </summary>
        internal static Transform Parse(string source)
        {
            Matrix matrix = Matrix.Parse(source);

            return new MatrixTransform(matrix);
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
