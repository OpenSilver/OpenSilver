
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

namespace OpenSilver.Internal;

/// <summary>
/// Helpers for <see cref="Matrix3D"/> used as 2D transforms.
/// </summary>
/// <remarks>
/// Matrices use row vectors: a point (x, y) is transformed as (x, y, 0, 1) * M, followed
/// by a division by W. Only the 2D homography (M11, M12, M14, M21, M22, M24, OffsetX,
/// OffsetY, M44) matters for that purpose; see <see cref="Flatten(Matrix3D)"/>.
/// <para>
/// Only points in front of the camera (W &gt; 0) are meaningful:
/// <see cref="TryTransform(Matrix3D, Point, out Point)"/> fails for the other points and
/// <see cref="TransformBounds(Matrix3D, Rect)"/> ignores them.
/// </para>
/// </remarks>
internal static class Matrix3DUtil
{
    // Points whose W is smaller than this fraction of the largest W of the transformed
    // corners are clipped, which bounds the magnification to 1000 times the one of the
    // least magnified corner. Being relative, the threshold does not depend on the
    // homogeneous scale of the matrix.
    private const double RelativeNearPlane = 1e-3;

    internal static Matrix3D FromMatrix(Matrix m) =>
        new(m.M11, m.M12, 0, 0,
            m.M21, m.M22, 0, 0,
            0, 0, 1, 0,
            m.OffsetX, m.OffsetY, 0, 1);

    /// <summary>
    /// Drops the Z input and output of <paramref name="m"/>. The resulting matrix gives the
    /// same 2D results as <paramref name="m"/>, and products of flattened matrices compose
    /// the 2D homographies.
    /// </summary>
    internal static Matrix3D Flatten(Matrix3D m) =>
        new(m.M11, m.M12, 0, m.M14,
            m.M21, m.M22, 0, m.M24,
            0, 0, 1, 0,
            m.OffsetX, m.OffsetY, 0, m.M44);

    internal static Matrix3D CreateTranslation(double x, double y, double z) =>
        new(1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            x, y, z, 1);

    internal static Matrix3D CreateScale(double x, double y, double z) =>
        new(x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1);

    internal static Matrix3D CreateRotationX(double radians)
    {
        double c = Math.Cos(radians);
        double s = Math.Sin(radians);
        return new(
            1, 0, 0, 0,
            0, c, -s, 0,
            0, s, c, 0,
            0, 0, 0, 1);
    }

    internal static Matrix3D CreateRotationY(double radians)
    {
        double c = Math.Cos(radians);
        double s = Math.Sin(radians);
        return new(
            c, 0, s, 0,
            0, 1, 0, 0,
            -s, 0, c, 0,
            0, 0, 0, 1);
    }

    internal static Matrix3D CreateRotationZ(double radians)
    {
        double c = Math.Cos(radians);
        double s = Math.Sin(radians);
        return new(
            c, -s, 0, 0,
            s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
    }

    /// <summary>
    /// Returns true if <paramref name="m"/> is a 2D affine transform once divided by W, with
    /// all points in front of the camera (unlike <see cref="Matrix3D.IsAffine"/>, which
    /// requires M44 = 1 and ignores the Z output).
    /// </summary>
    internal static bool IsAffine(Matrix3D m) => m.M14 == 0 && m.M24 == 0 && m.M44 > 0;

    /// <summary>
    /// Gets the 2D affine matrix equivalent to <paramref name="m"/>, if it is affine and in
    /// front of the camera (see <see cref="IsAffine(Matrix3D)"/>).
    /// </summary>
    internal static bool TryGetAffineMatrix(Matrix3D m, out Matrix matrix)
    {
        if (IsAffine(m))
        {
            double w = m.M44;
            matrix = new Matrix(m.M11 / w, m.M12 / w, m.M21 / w, m.M22 / w, m.OffsetX / w, m.OffsetY / w);
            return true;
        }

        matrix = default;
        return false;
    }

    /// <summary>
    /// Gets the 2D affine matrix equivalent to <paramref name="m"/>, or an approximation
    /// that ignores the perspective terms when there is none.
    /// </summary>
    internal static Matrix GetAffineApproximation(Matrix3D m)
    {
        double w = m.M44 == 0 ? 1 : m.M44;
        return new Matrix(m.M11 / w, m.M12 / w, m.M21 / w, m.M22 / w, m.OffsetX / w, m.OffsetY / w);
    }

    /// <summary>
    /// Transforms <paramref name="point"/>, and fails if it is not in front of the camera
    /// (W &lt;= 0) or if the result is not finite.
    /// </summary>
    internal static bool TryTransform(Matrix3D m, Point point, out Point result)
    {
        HPoint p = HTransform(m, point.X, point.Y);

        if (p.W > 0)
        {
            double x = p.X / p.W;
            double y = p.Y / p.W;

            if (IsFinite(x) && IsFinite(y))
            {
                result = new Point(x, y);
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Returns the bounds of the part of <paramref name="rect"/> that is in front of the
    /// camera once transformed, or <see cref="Rect.Empty"/> if there is none.
    /// </summary>
    internal static Rect TransformBounds(Matrix3D m, Rect rect)
    {
        if (rect.IsEmpty)
        {
            return rect;
        }

        if (TryGetAffineMatrix(m, out Matrix matrix))
        {
            return Rect.Transform(rect, matrix);
        }

        HPoint p0 = HTransform(m, rect.Left, rect.Top);
        HPoint p1 = HTransform(m, rect.Right, rect.Top);
        HPoint p2 = HTransform(m, rect.Right, rect.Bottom);
        HPoint p3 = HTransform(m, rect.Left, rect.Bottom);

        double maxW = Math.Max(Math.Max(p0.W, p1.W), Math.Max(p2.W, p3.W));

        if (maxW <= 0)
        {
            return Rect.Empty;
        }

        return GetBounds(p0, p1, p2, p3, maxW * RelativeNearPlane);
    }

    /// <summary>
    /// Inverts the 2D homography of <paramref name="m"/>, which must be flattened (see
    /// <see cref="Flatten(Matrix3D)"/>). Otherwise, the Z terms are taken into account and the
    /// result is not the inverse of the 2D homography.
    /// </summary>
    internal static bool TryInvert(Matrix3D m, out Matrix3D inverse)
    {
        Debug.Assert(
            m.M13 == 0 && m.M23 == 0 && m.OffsetZ == 0 &&
            m.M31 == 0 && m.M32 == 0 && m.M33 == 1 && m.M34 == 0,
            $"The matrix is not flattened: {m}.");

        if (Is2D(m))
        {
            Matrix matrix = ToMatrix(m);

            if (matrix.HasInverse)
            {
                matrix.Invert();
                inverse = FromMatrix(matrix);
                return true;
            }

            inverse = default;
            return false;
        }

        if (m.InvertCore())
        {
            inverse = m;
            return true;
        }

        inverse = default;
        return false;
    }

    private static bool Is2D(Matrix3D m) => m.M14 == 0 && m.M24 == 0 && m.M44 == 1;

    private static Matrix ToMatrix(Matrix3D m) => new(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);

    private static HPoint HTransform(Matrix3D m, double x, double y) =>
        new(x * m.M11 + y * m.M21 + m.OffsetX,
            x * m.M12 + y * m.M22 + m.OffsetY,
            x * m.M14 + y * m.M24 + m.M44);

    private static bool IsFinite(double d) => !double.IsNaN(d) && !double.IsInfinity(d);

    private static Rect GetBounds(HPoint p0, HPoint p1, HPoint p2, HPoint p3, double minW)
    {
        // The clipped polygon is made of the corners in front of the near plane and of the
        // intersections of the edges with that plane, so its bounds can be accumulated
        // edge by edge.
        var bounds = new Bounds();
        bounds.AddClippedEdge(p0, p1, minW);
        bounds.AddClippedEdge(p1, p2, minW);
        bounds.AddClippedEdge(p2, p3, minW);
        bounds.AddClippedEdge(p3, p0, minW);
        return bounds.ToRect();
    }

    private readonly struct HPoint
    {
        public HPoint(double x, double y, double w)
        {
            (X, Y, W) = (x, y, w);
        }

        public readonly double X, Y, W;
    }

    private struct Bounds
    {
        private double _minX, _minY, _maxX, _maxY;

        public Bounds()
        {
            _minX = _minY = double.PositiveInfinity;
            _maxX = _maxY = double.NegativeInfinity;
        }

        public readonly Rect ToRect() => new(_minX, _minY, _maxX - _minX, _maxY - _minY);

        public void AddClippedEdge(HPoint a, HPoint b, double minW)
        {
            bool aIn = a.W >= minW;
            bool bIn = b.W >= minW;

            if (aIn)
            {
                Add(a.X / a.W, a.Y / a.W);
            }

            if (aIn != bIn)
            {
                double t = (minW - a.W) / (b.W - a.W);
                Add((a.X + t * (b.X - a.X)) / minW, (a.Y + t * (b.Y - a.Y)) / minW);
            }
        }

        private void Add(double x, double y)
        {
            _minX = Math.Min(_minX, x);
            _minY = Math.Min(_minY, y);
            _maxX = Math.Max(_maxX, x);
            _maxY = Math.Max(_maxY, y);
        }
    }
}
