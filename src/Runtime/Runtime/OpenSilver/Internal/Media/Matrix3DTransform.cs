
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

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OpenSilver.Internal.Media;

/// <summary>
/// A <see cref="Transform"/> defined by a 2D homography, stored as a flattened
/// <see cref="System.Windows.Media.Media3D.Matrix3D"/>. It is also the transform 
/// returned by <see cref="UIElement.TransformToVisual(UIElement)"/> when the result 
/// is not a 2D affine transform.
/// </summary>
internal sealed class Matrix3DTransform : Transform
{
    private Matrix3D _matrix3D;

    public Matrix3DTransform(Matrix3D matrix)
    {
        _matrix3D = Matrix3DUtil.Flatten(matrix);
    }

    /// <inheritdoc />
    public override Matrix Value => Matrix3DUtil.GetAffineApproximation(_matrix3D);

    /// <inheritdoc />
    public override GeneralTransform Inverse
    {
        get
        {
            if (IsAffine)
            {
                return base.Inverse;
            }

            return Matrix3DUtil.TryInvert(_matrix3D, out Matrix3D inverse) ? new Matrix3DTransform(inverse) : null;
        }
    }

    /// <inheritdoc />
    public override bool TryTransform(Point inPoint, out Point outPoint)
    {
        if (IsAffine)
        {
            return base.TryTransform(inPoint, out outPoint);
        }

        return Matrix3DUtil.TryTransform(_matrix3D, inPoint, out outPoint);
    }

    /// <inheritdoc />
    public override Rect TransformBounds(Rect rect)
    {
        if (IsAffine)
        {
            return base.TransformBounds(rect);
        }

        return Matrix3DUtil.TransformBounds(_matrix3D, rect);
    }

    internal override Matrix3D Matrix3D => _matrix3D;

    internal override bool IsAffine => Matrix3DUtil.IsAffine(_matrix3D);

    internal override bool IsIdentity => _matrix3D.IsIdentity;

    internal void SetMatrix3D(Matrix3D matrix)
    {
        matrix = Matrix3DUtil.Flatten(matrix);

        if (_matrix3D != matrix)
        {
            _matrix3D = matrix;
            OnTransformChanged();
        }
    }
}
