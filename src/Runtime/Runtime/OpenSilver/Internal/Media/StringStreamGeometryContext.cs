
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

using OpenSilver.Buffers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media;

/// <summary>
/// Transform the figures collection into a SVG Path according to :
/// https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d
/// </summary>
internal sealed class StringStreamGeometryContext : CapacityStreamGeometryContext
{
    private readonly IFormatProvider _formatProvider;
    private CharArrayBuilder _buffer;

    private int _figuresCount;
    private bool _isClosed;

    public StringStreamGeometryContext(IFormatProvider formatProvider)
    {
        _formatProvider = formatProvider;
        _buffer = new CharArrayBuilder(512);
    }

    public override void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
    {
        ThrowIfDisposed();

        // Don't forget to close out the previous segment/figure
        FinishFigure();

        _isClosed = isClosed;

        Append("M", _figuresCount > 0);
        AppendPoint(startPoint);

        _figuresCount++;
    }

    public override void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        Append("A");
        Append(size.Width.ToString(_formatProvider));
        Append(size.Height.ToString(_formatProvider));
        Append(rotationAngle.ToString(_formatProvider));
        Append(isLargeArc ? "1" : "0");
        Append(sweepDirection == SweepDirection.Clockwise ? "1" : "0");
        AppendPoint(point);
    }

    public override void LineTo(Point point, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        Append("L");
        AppendPoint(point);
    }

    public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        Append("C");
        AppendPoint(point1);
        AppendPoint(point2);
        AppendPoint(point3);
    }

    public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        Append("Q");
        AppendPoint(point1);
        AppendPoint(point2);
    }

    public override void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        int count = points.Count;

        if (count <= 0)
        {
            return;
        }

        Append("L");

        for (int i = 0; i < count; i++)
        {
            AppendPoint(points[i]);
        }
    }

    public override void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        int count = points.Count;
        count -= count % 3;

        if (count <= 0)
        {
            return;
        }

        Append("C");

        for (int i = 0; i < count; i++)
        {
            AppendPoint(points[i]);
        }
    }

    public override void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        ThrowIfDisposed();
        VerifyFigure();

        int count = points.Count;
        count -= count % 2;

        if (count <= 0)
        {
            return;
        }

        Append("Q");

        for (int i = 0; i < count; i++)
        {
            AppendPoint(points[i]);
        }
    }

    public override string ToString()
    {
        ThrowIfDisposed();

        FinishFigure();
        return _buffer.ToString();
    }

    internal override void DisposeCore()
    {
        ThrowIfDisposed();

        _buffer.Dispose();
        _buffer = null;
    }

    internal override void SetClosedState(bool closed)
    {
        Debug.Assert(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Append(string s, bool addSeparator = true)
    {
        if (addSeparator)
        {
            _buffer.Append(" ");
        }

        _buffer.Append(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendPoint(Point point)
    {
        Append(point.X.ToString(_formatProvider));
        Append(point.Y.ToString(_formatProvider));
    }

    private void ThrowIfDisposed()
    {
        if (_buffer is null)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    private void VerifyFigure()
    {
        if (_figuresCount == 0)
        {
            throw new InvalidOperationException(Strings.StreamGeometry_NeedBeginFigure);
        }
    }

    private void FinishFigure()
    {
        if (_figuresCount == 0)
        {
            return;
        }

        if (_isClosed)
        {
            Append("Z");
        }
    }
}