// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of PathGeometryWrapper.h / PathGeometryWrapper.cpp

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class PathGeometryWrapper : CShapeBase
{
    private readonly byte[] _data;
    private readonly FillRule _fillRule;
    private readonly Matrix _transform;

    private readonly MIL_PATHGEOMETRY _pPath;
    private readonly PathFigureData _pathFigure;

    private MIL_PATHFIGURE _currentFigure;
    private int _currentIndex;
    private int _offset;

    public PathGeometryWrapper(byte[] data, FillRule fillRule, Matrix transform)
    {
        _data = data;
        _fillRule = fillRule;
        _transform = transform;

        _pPath = MemoryMarshal.Read<MIL_PATHGEOMETRY>(data);
        _offset = Unsafe.SizeOf<MIL_PATHGEOMETRY>();
        _pathFigure = new PathFigureData(data);

        _currentIndex = 0;

        if (_pPath.FigureCount > 0)
        {
            _currentFigure = MemoryMarshal.Read<MIL_PATHFIGURE>(_data.AsSpan(_offset));
        }
    }

    internal override IFigureData GetFigure(int index)
    {
        Debug.Assert(index < GetFigureCount());

        if (index != _currentIndex)
        {
            if (index == 0)
            {
                _offset = Unsafe.SizeOf<MIL_PATHGEOMETRY>();
                _currentFigure = MemoryMarshal.Read<MIL_PATHFIGURE>(_data.AsSpan(_offset));
                _currentIndex = 0;
            }
            else
            {
                while (_currentIndex < index)
                {
                    if (!NextFigure())
                    {
                        return null;
                    }
                }

                while (_currentIndex > index)
                {
                    if (!PrevFigure())
                    {
                        return null;
                    }
                }
            }
        }

        _pathFigure.SetFigureData(_currentFigure, _offset, _transform);
        return _pathFigure;
    }

    internal override int GetFigureCount() => (int)_pPath.FigureCount;

    internal override FillRule GetFillMode() => _fillRule;

    internal override bool HasGaps() => (_pPath.Flags & MilPathGeometryFlags.HasGaps) != 0;

    internal override bool HasHollows() => (_pPath.Flags & MilPathGeometryFlags.HasHollows) != 0;

    internal override bool IsEmpty() => _pPath.FigureCount <= 0;

    internal override bool IsAxisAlignedRectangle() => GetFigureCount() == 1 && GetFigure(0).IsAxisAlignedRectangle();

    private bool NextFigure()
    {
        if (_currentIndex >= _pPath.FigureCount)
        {
            return false;
        }

        _offset += (int)_currentFigure.Size;

        _currentFigure = MemoryMarshal.Read<MIL_PATHFIGURE>(_data.AsSpan(_offset));
        _currentIndex++;

        return true;
    }

    private bool PrevFigure()
    {
        if (_currentIndex <= 0)
        {
            return false;
        }

        _offset -= (int)_currentFigure.BackSize;

        _currentFigure = MemoryMarshal.Read<MIL_PATHFIGURE>(_data.AsSpan(_offset));
        _currentIndex--;

        return true;
    }
}

internal sealed class PathFigureData : IFigureData
{
    private readonly byte[] _data;
    private int _offset;
    private Matrix _transform;
    private MIL_PATHFIGURE _pFigure;

    private int _currentIndex;
    private int _currentSegmentOffset;
    private MIL_SEGMENT _currentSegment;
    private int _innerIndex;
    private MilPoint2D? _cachedEndPoint;

    // Stop index
    private int _stop;
    private int _innerStop;

    // Scratch area for returned points
    private Point[] _points;

    // Specific arc data, not used for other types
    private MILCoreSegFlags _arcType;
    private int _currentPoint;
    private int _lastInnerIndex;

    public PathFigureData(byte[] data)
    {
        _data = data;
        SetFigureData(default, 0, Matrix.Identity);
    }

    internal void SetFigureData(MIL_PATHFIGURE figure, int offset, Matrix matrix)
    {
        _pFigure = figure;
        _offset = offset;
        _transform = matrix.IsIdentity ? Matrix.Identity : matrix;

        _currentIndex = _stop = _innerStop = int.MaxValue;
        _innerIndex = 0;

        _cachedEndPoint = null;

        _currentPoint = 0;
    }

    internal override MilRectD GetAsRectangle()
    {
        Debug.Assert(IsAxisAlignedRectangle());

        GetParallelogramVertices(out MilPoint2D p0, out _, out MilPoint2D p2, out _);

        return new MilRectD(p0.X, p0.Y, p2.X, p2.Y);
    }

    internal override Rect GetAsWellOrderedRectangle()
    {
        Debug.Assert(IsAxisAlignedRectangle());

        GetParallelogramVertices(out MilPoint2D p0, out MilPoint2D p1, out MilPoint2D p2, out MilPoint2D p3);

        RectF_RBFromParallelogramPointsF(p0, p1, p2, p3, out Rect rect);

        return rect;
    }

    internal override void GetCountsEstimate(out int cSegments, out int cPoints)
    {
        cPoints = 1; // for the start point
        cSegments = 0;

        if (_pFigure.Count == 0)
        {
            return;
        }

        int currentOffset = _offset + Unsafe.SizeOf<MIL_PATHFIGURE>();

        for (int i = 0; i < _pFigure.Count; i++)
        {
            MIL_SEGMENT pSegment = MemoryMarshal.Read<MIL_SEGMENT>(_data.AsSpan(currentOffset));

            switch (pSegment.Type)
            {
                case MIL_SEGMENT_TYPE.MilSegmentLine:
                    cSegments++;
                    cPoints++;
                    currentOffset += Unsafe.SizeOf<MIL_SEGMENT_LINE>();
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentBezier:
                    cSegments++;
                    cPoints += 3;
                    currentOffset += Unsafe.SizeOf<MIL_SEGMENT_BEZIER>();
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentQuadraticBezier:
                    // Quadratic bezier segment are converted to cubic bezier segment
                    cSegments++;
                    cPoints += 3;
                    currentOffset += Unsafe.SizeOf<MIL_SEGMENT_QUADRATICBEZIER>();
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentArc:
                    // An arc may have up to 4 Bezier segments
                    cSegments += 4;
                    cPoints += 12;
                    currentOffset += Unsafe.SizeOf<MIL_SEGMENT_ARC>();
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                    {
                        MIL_SEGMENT_POLY pPoly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(currentOffset));
                        int count = (int)pPoly.Count;
                        cSegments += count;
                        cPoints += count;
                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * count;
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                    {
                        MIL_SEGMENT_POLY pPoly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(currentOffset));
                        int count = (int)pPoly.Count;
                        int segCount = count / 3;
                        cSegments += segCount;
                        cPoints += segCount * 3;
                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * count;
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                    {
                        MIL_SEGMENT_POLY pPoly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(currentOffset));
                        int count = (int)pPoly.Count;
                        int segCount = count / 2;
                        cSegments += segCount;
                        cPoints += segCount * 3;
                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * count;
                    }
                    break;
            }
        }

        if (IsClosed())
        {
            cSegments++;
            cPoints++;
        }
    }

    internal override bool GetCurrentSegment(out MILCoreSegFlags bType, Span<MilPoint2D> points)
    {
        if (_currentIndex >= _pFigure.Count)
        {
            // We're at the implied closing line segment
            bType = MILCoreSegFlags.SegTypeLine;
            points[0] = GetStartPoint();
        }
        else
        {
            switch (_currentSegment.Type)
            {
                case MIL_SEGMENT_TYPE.MilSegmentLine:
                    {
                        bType = MILCoreSegFlags.SegTypeLine;

                        MilPoint2D pt = MemoryMarshal.Read<MIL_SEGMENT_LINE>(_data.AsSpan(_currentSegmentOffset)).Point;
                        if (!_transform.IsIdentity)
                        {
                            pt *= _transform;
                        }

                        points[0] = pt;
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentBezier:
                    {
                        bType = MILCoreSegFlags.SegTypeBezier;
                        MIL_SEGMENT_BEZIER bezier = MemoryMarshal.Read<MIL_SEGMENT_BEZIER>(_data.AsSpan(_currentSegmentOffset));

                        points[0] = bezier.Point1;
                        points[1] = bezier.Point2;
                        points[2] = bezier.Point3;

                        if (!_transform.IsIdentity)
                        {
                            points[0] *= _transform;
                            points[1] *= _transform;
                            points[2] *= _transform;
                        }
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentQuadraticBezier:
                    {
                        bType = MILCoreSegFlags.SegTypeBezier;
                        MIL_SEGMENT_QUADRATICBEZIER bezier = MemoryMarshal.Read<MIL_SEGMENT_QUADRATICBEZIER>(_data.AsSpan(_currentSegmentOffset));
                        SetQuadraticBezier(GetCurrentSegmentStartD(), bezier.Point1, bezier.Point2, points);
                        if (!_transform.IsIdentity)
                        {
                            TransformPoints(points, 3);
                        }
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentArc:
                    {
                        bType = _arcType;
                        if (_arcType == MILCoreSegFlags.SegTypeLine)
                        {
                            points[0] = _points[_currentPoint];
                        }
                        else
                        {
                            points[0] = _points[_currentPoint];
                            points[1] = _points[_currentPoint + 1];
                            points[2] = _points[_currentPoint + 2];
                        }
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                    {
                        bType = MILCoreSegFlags.SegTypeLine;
                        int offset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + _innerIndex * Unsafe.SizeOf<Point>();
                        points[0] = MemoryMarshal.Read<Point>(_data.AsSpan(offset));
                        if (!_transform.IsIdentity)
                        {
                            points[0] *= _transform;
                        }
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                    {
                        bType = MILCoreSegFlags.SegTypeBezier;
                        int offset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + _innerIndex * 3 * Unsafe.SizeOf<Point>();

                        points[0] = MemoryMarshal.Read<Point>(_data.AsSpan(offset));
                        points[1] = MemoryMarshal.Read<Point>(_data.AsSpan(offset + Unsafe.SizeOf<Point>()));
                        points[2] = MemoryMarshal.Read<Point>(_data.AsSpan(offset + 2 * Unsafe.SizeOf<Point>()));

                        if (!_transform.IsIdentity)
                        {
                            points[0] *= _transform;
                            points[1] *= _transform;
                            points[2] *= _transform;
                        }
                    }
                    break;

                case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                    {
                        bType = MILCoreSegFlags.SegTypeBezier;
                        int offset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + _innerIndex * 2 * Unsafe.SizeOf<Point>();

                        SetQuadraticBezier(
                            _innerIndex > 0 ? MemoryMarshal.Read<Point>(_data.AsSpan(offset - Unsafe.SizeOf<Point>())) : GetCurrentSegmentStartD(),
                            MemoryMarshal.Read<Point>(_data.AsSpan(offset)),
                            MemoryMarshal.Read<Point>(_data.AsSpan(offset + Unsafe.SizeOf<Point>())),
                            points);

                        if (!_transform.IsIdentity)
                        {
                            TransformPoints(points, 3);
                        }
                    }
                    break;

                default:
                    // This case should never be hit, but the output needs to be initialized anyway
                    Debug.Assert(false, "Invalid segment type");
                    bType = MILCoreSegFlags.SegTypeLine;
                    break;
            }
        }

        // Returning true if this is the last figure or if this is a stop
        return _currentIndex == _stop && _innerIndex == _innerStop;
    }

    internal override MilPoint2D GetCurrentSegmentStart()
    {
        MilPoint2D pt = GetCurrentSegmentStartD();

        if (!_transform.IsIdentity)
        {
            pt *= _transform;
        }

        return pt;
    }

    internal override MilPoint2D GetEndPoint()
    {
        if (IsClosed())
        {
            return GetStartPoint();
        }

        if (_cachedEndPoint.HasValue)
        {
            return _cachedEndPoint.Value;
        }

        MilPoint2D pt = GetSegmentLastPoint(_offset + (int)_pFigure.OffsetToLastSegment);
        if (!_transform.IsIdentity)
        {
            pt *= _transform;
        }
        _cachedEndPoint = pt;
        return pt;
    }

    internal override MilPoint2D GetStartPoint()
    {
        MilPoint2D pt = _pFigure.StartPoint;
        if (!_transform.IsIdentity)
        {
            pt *= _transform;
        }
        return pt;
    }

    internal override bool HasGaps() => (_pFigure.Flags & MilPathFigureFlags.HasGaps) != 0;

    internal override bool HasNoSegments() => _pFigure.Count == 0;

    internal override bool IsAParallelogram() => (_pFigure.Flags & MilPathFigureFlags.IsRectangleData) != 0;

    internal override bool IsAtAGap()
    {
        bool fIsAtGap;

        if (_currentIndex >= _pFigure.Count)
        {
            // We're at the implied closing line segment
            fIsAtGap = false;
        }
        else
        {
            fIsAtGap = (_currentSegment.Flags & MILCoreSegFlags.SegIsAGap) != 0;
        }

        return fIsAtGap;
    }

    internal override bool IsAtASmoothJoin()
    {
        return ((_currentSegment.Flags & MILCoreSegFlags.SegSmoothJoin) != 0) ||
               (_currentSegment.Type == MIL_SEGMENT_TYPE.MilSegmentArc && _innerIndex < _lastInnerIndex);
    }

    internal override bool IsAxisAlignedRectangle()
    {
        bool fIsAxisAlignedRect = false;

        if (IsAParallelogram())
        {
            GetParallelogramVertices(out MilPoint2D p0, out MilPoint2D p1, out MilPoint2D p2, out MilPoint2D p3);

            fIsAxisAlignedRect = RectF_RBFromParallelogramPointsF(p0, p1, p2, p3, out _, false) == true;
        }

        return fIsAxisAlignedRect;
    }

    internal override bool IsClosed() => (_pFigure.Flags & MilPathFigureFlags.IsClosed) != 0;

    internal override bool IsEmpty() => false;

    internal override bool IsFillable() => (_pFigure.Flags & MilPathFigureFlags.IsFillable) != 0;

    internal override bool IsStopSet() => _stop < int.MaxValue || _innerStop < int.MaxValue;

    internal override void ResetStop() => _stop = _innerStop = int.MaxValue;

    internal override void SetStop()
    {
        _stop = _currentIndex;
        _innerStop = _innerIndex;
    }

    internal override bool SetToFirstSegment()
    {
        bool fSet = _pFigure.Count > 0;
        if (fSet)
        {
            _currentSegmentOffset = _offset + Unsafe.SizeOf<MIL_PATHFIGURE>();
            _currentSegment = MemoryMarshal.Read<MIL_SEGMENT>(_data.AsSpan(_currentSegmentOffset));
            _currentIndex = 0;

            if (_currentSegment.Type == MIL_SEGMENT_TYPE.MilSegmentArc)
            {
                SetArcData();
                _currentPoint = 0;
            }

            _innerIndex = 0;
        }

        return fSet;
    }

    internal override bool SetToNextSegment()
    {
        bool fSet = false;
        int nextSegmentOffset = -1;

        switch (_currentSegment.Type)
        {
            case MIL_SEGMENT_TYPE.MilSegmentLine:
                fSet = _currentIndex < _pFigure.Count - 1;
                if (fSet)
                {
                    nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_LINE>();
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentBezier:
                fSet = _currentIndex < _pFigure.Count - 1;
                if (fSet)
                {
                    nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_BEZIER>();
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentQuadraticBezier:
                fSet = _currentIndex < _pFigure.Count - 1;
                if (fSet)
                {
                    nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_QUADRATICBEZIER>();
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentArc:
                fSet = _innerIndex < _lastInnerIndex;
                if (fSet)
                {
                    Debug.Assert(_arcType == MILCoreSegFlags.SegTypeBezier);
                    _currentPoint += 3;
                    Debug.Assert(_currentPoint < 12);
                    break;
                }

                fSet = _currentIndex < _pFigure.Count - 1;
                if (fSet)
                {
                    nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_ARC>();
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(_currentSegmentOffset));
                    Debug.Assert(poly.Count != 0);
                    fSet = _innerIndex < (int)poly.Count - 1;
                    if (fSet)
                    {
                        // We'll move to the next inner segment
                        break;
                    }

                    fSet = _currentIndex < _pFigure.Count - 1;
                    if (fSet)
                    {
                        nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + (int)poly.Count * Unsafe.SizeOf<Point>();
                    }
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(_currentSegmentOffset));
                    Debug.Assert(poly.Count % 3 == 0);

                    int lastInnerIndex = (int)poly.Count / 3 - 1;
                    Debug.Assert(poly.Count >= 3);

                    fSet = _innerIndex < lastInnerIndex;
                    if (fSet)
                    {
                        // We'll move to the next inner segment
                        break;
                    }

                    fSet = _currentIndex < _pFigure.Count - 1;
                    if (fSet)
                    {
                        nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + (int)poly.Count * Unsafe.SizeOf<Point>();
                    }
                }
                break;

            case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(_currentSegmentOffset));
                    Debug.Assert((poly.Count & 1) == 0);
                    Debug.Assert(poly.Count >= 2);

                    int lastInnerIndex = (int)poly.Count / 2 - 1;

                    fSet = _innerIndex < lastInnerIndex;
                    if (fSet)
                    {
                        // We'll move to the next inner segmen
                        break;
                    }

                    fSet = _currentIndex < _pFigure.Count - 1;
                    if (fSet)
                    {
                        nextSegmentOffset = _currentSegmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + (int)poly.Count * Unsafe.SizeOf<Point>();
                    }
                }
                break;

            default:
                fSet = false;
                Debug.Assert(false);
                break;
        }

        if (fSet)
        {
            if (nextSegmentOffset >= 0)
            {
                _currentSegmentOffset = nextSegmentOffset;
                _currentSegment = MemoryMarshal.Read<MIL_SEGMENT>(_data.AsSpan(_currentSegmentOffset));
                _currentIndex++;

                if (_currentSegment.Type == MIL_SEGMENT_TYPE.MilSegmentArc)
                {
                    SetArcData();
                    _currentPoint = 0;
                }

                _innerIndex = 0;
            }
            else
            {
                _innerIndex++;
            }
        }
        else
        {
            // Last chance - the implied closing line segment, if not redundant
            if (IsClosed() && _currentIndex < _pFigure.Count)
            {
                Point ptStart = _pFigure.StartPoint;
                MilPoint2D ptCurrent = GetSegmentLastPoint(_currentSegmentOffset);
                fSet = (ptStart.X != ptCurrent.X) || (ptStart.Y != ptCurrent.Y);
                _currentIndex++;
            }
        }

        return fSet;
    }

    private void TransformPoints(Span<MilPoint2D> points, int count)
    {
        for (int i = 0; i < count; i++)
        {
            points[i] *= _transform;
        }
    }

    private void SetQuadraticBezier(MilPoint2D pt0, MilPoint2D pt1, MilPoint2D pt2, Span<MilPoint2D> dest)
    {
        // By the degree-elevation formula for Bezier curves (found in any geometric
        // modeling textbook) the cubic Bezier points of this quadratic Bezier curve  
        // are pt0 (not set here), (1/3)*pt0+ (2/3)*pt1, (2/3)*pt1 + (1/3)*pt2, pt2.

        dest[0].X = Utils.ONE_THIRD * pt0.X + Utils.TWO_THIRDS * pt1.X;
        dest[0].Y = Utils.ONE_THIRD * pt0.Y + Utils.TWO_THIRDS * pt1.Y;

        dest[1].X = Utils.TWO_THIRDS * pt1.X + Utils.ONE_THIRD * pt2.X;
        dest[1].Y = Utils.TWO_THIRDS * pt1.Y + Utils.ONE_THIRD * pt2.Y;

        dest[2].X = pt2.X;
        dest[2].Y = pt2.Y;
    }

    internal override void GetParallelogramVertices(out MilPoint2D p0, out MilPoint2D p1, out MilPoint2D p2, out MilPoint2D p3, Matrix matrix = default)
    {
        Debug.Assert(_pFigure.Count == 1);

        int segmentOffset = _offset + Unsafe.SizeOf<MIL_PATHFIGURE>();
        Debug.Assert(MemoryMarshal.Read<MIL_SEGMENT>(_data.AsSpan(segmentOffset)).Type == MIL_SEGMENT_TYPE.MilSegmentPolyLine);

        MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(segmentOffset));
        Debug.Assert(poly.Count == 3);

        // First vertex
        p0 = _pFigure.StartPoint;

        // The remaining 3 vertices
        int pointsOffset = segmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>();
        p1 = MemoryMarshal.Read<Point>(_data.AsSpan(pointsOffset));
        p2 = MemoryMarshal.Read<Point>(_data.AsSpan(pointsOffset + Unsafe.SizeOf<Point>()));
        p3 = MemoryMarshal.Read<Point>(_data.AsSpan(pointsOffset + 2 * Unsafe.SizeOf<Point>()));

        if (!_transform.IsIdentity)
        {
            if (!matrix.IsIdentity)
            {
                matrix = _transform * matrix;
            }
            else
            {
                matrix = _transform;
            }
        }

        if (!matrix.IsIdentity)
        {
            p0 *= matrix;
            p1 *= matrix;
            p2 *= matrix;
            p3 *= matrix;
        }
    }

    private MilPoint2D GetCurrentSegmentStartD()
    {
        if (_currentIndex == 0)
        {
            return _pFigure.StartPoint;
        }

        int prevSegOffset;
        if (_currentIndex < _pFigure.Count)
        {
            prevSegOffset = _currentSegmentOffset - (int)_currentSegment.BackSize;
        }
        else
        {
            // At the implied closing segment; previous is the current stored segment
            prevSegOffset = _currentSegmentOffset;
        }

        return GetSegmentLastPoint(prevSegOffset);
    }

    private void SetArcData()
    {
        Debug.Assert(_currentSegment.Type == MIL_SEGMENT_TYPE.MilSegmentArc);

        MIL_SEGMENT_ARC arc = MemoryMarshal.Read<MIL_SEGMENT_ARC>(_data.AsSpan(_currentSegmentOffset));

        MilPoint2D lastPoint = GetCurrentSegmentStartD();

        Point pt = arc.Point;
        Size size = arc.Size;

        if (size.IsEmpty)
        {
            // This way we will end up drawing nothing.
            size.Width = 0;
            size.Height = 0;
        }

        _points ??= new Point[12];

        Utils.ArcToBezier(
            lastPoint.X,
            lastPoint.Y,
            size.Width,
            size.Height,
            arc.XRotation,
            arc.LargeArc != 0,
            arc.Sweep != 0,
            pt.X,
            pt.Y,
            _points,
            out int cPieces);

        int cPoints;

        if (cPieces <= 0)
        {
            _points[0] = pt;
            _arcType = MILCoreSegFlags.SegTypeLine;
            cPoints = cPieces = 1;
        }
        else
        {
            _arcType = MILCoreSegFlags.SegTypeBezier;
            cPoints = 3 * cPieces;
        }

        if (!_transform.IsIdentity)
        {
            for (int i = 0; i < cPoints; i++)
            {
                _points[i] = _transform.Transform(_points[i]);
            }
        }

        _lastInnerIndex = cPieces - 1;
        _currentPoint = 0;
    }

    private MilPoint2D GetSegmentLastPoint(int segmentOffset)
    {
        MIL_SEGMENT pSegment = MemoryMarshal.Read<MIL_SEGMENT>(_data.AsSpan(segmentOffset));

        switch (pSegment.Type)
        {
            case MIL_SEGMENT_TYPE.MilSegmentLine:
                return MemoryMarshal.Read<MIL_SEGMENT_LINE>(_data.AsSpan(segmentOffset)).Point;

            case MIL_SEGMENT_TYPE.MilSegmentBezier:
                return MemoryMarshal.Read<MIL_SEGMENT_BEZIER>(_data.AsSpan(segmentOffset)).Point3;

            case MIL_SEGMENT_TYPE.MilSegmentQuadraticBezier:
                return MemoryMarshal.Read<MIL_SEGMENT_QUADRATICBEZIER>(_data.AsSpan(segmentOffset)).Point2;

            case MIL_SEGMENT_TYPE.MilSegmentArc:
                return MemoryMarshal.Read<MIL_SEGMENT_ARC>(_data.AsSpan(segmentOffset)).Point;

            case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(segmentOffset));
                    Debug.Assert(poly.Count >= 1);
                    int index = (int)poly.Count - 1;
                    int offset = segmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * index;
                    return MemoryMarshal.Read<Point>(_data.AsSpan(offset));
                }
            case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(segmentOffset));
                    Debug.Assert(poly.Count >= 3);
                    int index = 3 * (int)(poly.Count / 3) - 1;
                    int offset = segmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * index;
                    return MemoryMarshal.Read<Point>(_data.AsSpan(offset));
                }
            case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                {
                    MIL_SEGMENT_POLY poly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(_data.AsSpan(segmentOffset));
                    Debug.Assert(poly.Count >= 2);
                    int index = 2 * (int)(poly.Count / 2) - 1;
                    int offset = segmentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + Unsafe.SizeOf<Point>() * index;
                    return MemoryMarshal.Read<Point>(_data.AsSpan(offset));
                }

            default:
                Debug.Assert(false, "Invalid segment type");
                return default;
        }
    }

    private bool RectF_RBFromParallelogramPointsF(MilPoint2D pt0, MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3, out Rect pRectF_RB, bool fillRect = true)
    {
        //
        // The points can start at either 0, 1, 2, or 3 and then be ordered in
        // either clock-wise or counter-clockwise order. It is assumed that
        // the points form a parallelogram.
        //     Examples:
        //       0--------1         1--------2
        //      /        /          |        |
        //     3--------2           0--------3
        //
        // If two sides of the parallelogram are axis-aligned, then the other 
        // sides must also be axis-aligned, making the figure a rectangle.
        //

        pRectF_RB = new Rect();

        if ((pt0.X == pt3.X) && (pt0.Y == pt1.Y))
        {
            //
            // With the assumption that the points are already a parallelogram,
            // the points, have been validated to be a rectangle.
            //

            if (fillRect)
            {
                if (pt0.Y < pt3.Y)
                {
                    pRectF_RB.Y = pt0.Y;
                    pRectF_RB.Height = pt3.Y - pt0.Y;
                }
                else
                {
                    pRectF_RB.Y = pt3.Y;
                    pRectF_RB.Height = pt0.Y - pt3.Y;
                }

                if (pt0.X < pt1.X)
                {
                    pRectF_RB.X = pt0.X;
                    pRectF_RB.Width = pt1.X - pt0.X;
                }
                else
                {
                    pRectF_RB.X = pt1.X;
                    pRectF_RB.Width = pt0.X - pt1.X;
                }
            }

            return true;
        }
        else if ((pt0.Y == pt3.Y) && (pt0.X == pt1.X))
        {
            //
            // With the assumption that the points are already a parallelogram,
            // the points, have been validated to be a rectangle.
            //

            if (fillRect)
            {
                if (pt0.Y < pt1.Y)
                {
                    pRectF_RB.Y = pt0.Y;
                    pRectF_RB.Height = pt1.Y - pt0.Y;
                }
                else
                {
                    pRectF_RB.Y = pt1.Y;
                    pRectF_RB.Height = pt0.Y - pt1.Y;
                }

                if (pt0.X < pt3.X)
                {
                    pRectF_RB.X = pt0.X;
                    pRectF_RB.Width = pt3.X - pt0.X;
                }
                else
                {
                    pRectF_RB.X = pt3.X;
                    pRectF_RB.Width = pt0.X - pt3.X;
                }
            }

            return true;
        }
        else
        {
            // The points are not a rectangle
            return false;
        }
    }
}
