
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

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OpenSilver.Internal.Media;

/// <summary>
/// The visual transform of an element that has a <see cref="Projection"/>: the projection,
/// applied in the local space of the element, followed by its 2D transform (flow direction,
/// layout transform, render transform).
/// </summary>
/// <remarks>
/// Changes of the projection and of the 2D transform are tracked, so that animating them does
/// not require a layout pass. A new instance is created by the layout when the size of the
/// element changes.
/// </remarks>
internal sealed class ProjectedTransform : Transform
{
    private readonly Projection _projection;
    private readonly Transform _transform;
    private readonly Size _size;
    private WeakEventToken _projectionChangedToken;
    private WeakEventToken _transformChangedToken;
    private Matrix3D _projectionMatrix;
    private Matrix3D? _matrix3D;

    public ProjectedTransform(Projection projection, Transform transform, Size size)
    {
        Debug.Assert(projection is not null);

        _projection = projection;
        _transform = transform;
        _size = size;
        _projectionMatrix = GetProjectionMatrix();

        _projectionChangedToken = WeakEvent.Subscribe<ProjectedTransform, Projection, EventArgs>(
            this,
            projection,
            static (instance, sender, args) => instance.OnProjectionChanged(sender, args),
            static (handler, source) => source.Changed -= new EventHandler(handler),
            static (handler, source) => source.Changed += new EventHandler(handler));

        if (_transform is not null)
        {
            _transformChangedToken = WeakEvent.Subscribe<ProjectedTransform, Transform, EventArgs>(
                this,
                transform,
                static (instance, sender, args) => instance.OnInnerTransformChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }
    }

    internal override Matrix3D Matrix3D =>
        _matrix3D ??= _transform is null ? _projectionMatrix : _projectionMatrix * _transform.Matrix3D;

    internal override bool IsAffine => Matrix3DUtil.IsAffine(Matrix3D);

    internal override bool IsIdentity => Matrix3D.IsIdentity;

    /// <inheritdoc />
    public override Matrix Value => Matrix3DUtil.GetAffineApproximation(Matrix3D);

    /// <inheritdoc />
    public override GeneralTransform Inverse
    {
        get
        {
            if (IsAffine)
            {
                return base.Inverse;
            }

            return Matrix3DUtil.TryInvert(Matrix3D, out Matrix3D inverse) ? new Matrix3DTransform(inverse) : null;
        }
    }

    /// <inheritdoc />
    public override bool TryTransform(Point inPoint, out Point outPoint)
    {
        if (IsAffine)
        {
            return base.TryTransform(inPoint, out outPoint);
        }

        return Matrix3DUtil.TryTransform(Matrix3D, inPoint, out outPoint);
    }

    /// <inheritdoc />
    public override Rect TransformBounds(Rect rect)
    {
        if (IsAffine)
        {
            return base.TransformBounds(rect);
        }

        return Matrix3DUtil.TransformBounds(Matrix3D, rect);
    }

    /// <summary>
    /// Stops tracking the projection and the 2D transform, once this instance is no longer the
    /// visual transform of the element.
    /// </summary>
    internal void Detach()
    {
        _projectionChangedToken?.Dispose();
        _projectionChangedToken = null;

        _transformChangedToken?.Dispose();
        _transformChangedToken = null;
    }

    private Matrix3D GetProjectionMatrix() => Matrix3DUtil.Flatten(_projection.GetProjectionMatrix(_size));

    private void OnProjectionChanged(object sender, EventArgs e)
    {
        Matrix3D projectionMatrix = GetProjectionMatrix();
        if (_projectionMatrix == projectionMatrix)
        {
            return;
        }

        _projectionMatrix = projectionMatrix;
        _matrix3D = null;
        OnTransformChanged();
    }

    private void OnInnerTransformChanged(object sender, EventArgs e)
    {
        _matrix3D = null;
        OnTransformChanged();
    }
}
