// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media;

/// <summary>
///     ByteStreamGeometryContext
/// </summary>
internal class ByteStreamGeometryContext : CapacityStreamGeometryContext
{
    /// <summary>
    /// Creates a geometry stream context.
    /// </summary>
    internal ByteStreamGeometryContext()
    {
        // For now, we just write this into the stream.  We'll update its fields as we go.
        AppendData(new MIL_PATHGEOMETRY());

        // Initialize the size to include the MIL_PATHGEOMETRY itself
        // All other fields are intentionally left as 0;
        _currentPathGeometryData.Size = (uint)Unsafe.SizeOf<MIL_PATHGEOMETRY>();
    }

    /// <summary>
    /// Closes the StreamContext and flushes the content.
    /// Afterwards the StreamContext can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    public override void Close()
    {
        VerifyApi();
        ((IDisposable)this).Dispose();
    }

    /// <summary>
    /// BeginFigure - Start a new figure.
    /// </summary>
    public override void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
    {
        VerifyApi();

        // Don't forget to close out the previous segment/figure
        FinishFigure();

        // Remember the location - we set this only after successful allocation in case it throws
        // and we're re-entered.
        int oldOffset = _currOffset;

        AppendData(new MIL_PATHFIGURE());

        _currentPathFigureDataOffset = oldOffset;
        _currentPathFigureData.StartPoint = startPoint;
        _currentPathFigureData.Flags |= isFilled ? MilPathFigureFlags.IsFillable : 0;
        _currentPathFigureData.Flags |= isClosed ? MilPathFigureFlags.IsClosed : 0;
        _currentPathFigureData.BackSize = _lastFigureSize;
        _currentPathFigureData.Size = (uint)(_currOffset - _currentPathFigureDataOffset);
    }

    /// <summary>
    /// LineTo - append a LineTo to the current figure.
    /// </summary>
    public override void LineTo(Point point, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        Span<Point> scratchForLine = stackalloc Point[1];
        scratchForLine[0] = point;
        GenericPolyTo(scratchForLine,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: false,
                      MIL_SEGMENT_TYPE.MilSegmentPolyLine);
    }

    /// <summary>
    /// QuadraticBezierTo - append a QuadraticBezierTo to the current figure.
    /// </summary>
    public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        Span<Point> scratchForQuadraticBezier = stackalloc Point[2];
        scratchForQuadraticBezier[0] = point1;
        scratchForQuadraticBezier[1] = point2;
        GenericPolyTo(scratchForQuadraticBezier,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: true,
                      MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);
    }

    /// <summary>
    /// BezierTo - apply a BezierTo to the current figure.
    /// </summary>
    public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        Span<Point> scratchForBezier = stackalloc Point[3];
        scratchForBezier[0] = point1;
        scratchForBezier[1] = point2;
        scratchForBezier[2] = point3;
        GenericPolyTo(scratchForBezier,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: true,
                      MIL_SEGMENT_TYPE.MilSegmentPolyBezier);
    }

    /// <summary>
    /// PolyLineTo - append a PolyLineTo to the current figure.
    /// </summary>
    public override void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: false,
                      pointCountMultiple: 1,
                      MIL_SEGMENT_TYPE.MilSegmentPolyLine);
    }

    /// <summary>
    /// PolyQuadraticBezierTo - append a PolyQuadraticBezierTo to the current figure.
    /// </summary>
    public override void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: true,
                      pointCountMultiple: 2,
                      MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);
    }

    /// <summary>
    /// PolyBezierTo - append a PolyBezierTo to the current figure.
    /// </summary>
    public override void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      hasCurves: true,
                      pointCountMultiple: 3,
                      MIL_SEGMENT_TYPE.MilSegmentPolyBezier);
    }

    /// <summary>
    /// ArcTo - append an ArcTo to the current figure.
    /// </summary>
    public override void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
    {
        VerifyApi();

        if (_currentPathFigureDataOffset == -1)
        {
            throw new InvalidOperationException(Strings.StreamGeometry_NeedBeginFigure);
        }

        FinishSegment();

        MIL_SEGMENT_ARC arcToSegment = new MIL_SEGMENT_ARC
        {
            Type = MIL_SEGMENT_TYPE.MilSegmentArc
        };

        arcToSegment.Flags |= isStroked ? 0 : MILCoreSegFlags.SegIsAGap;
        arcToSegment.Flags |= isSmoothJoin ? MILCoreSegFlags.SegSmoothJoin : 0;
        arcToSegment.Flags |= MILCoreSegFlags.SegIsCurved;
        arcToSegment.BackSize = _lastSegmentSize;

        arcToSegment.Point = point;
        arcToSegment.Size = size;
        arcToSegment.XRotation = rotationAngle;
        arcToSegment.LargeArc = (uint)(isLargeArc ? 1 : 0);
        arcToSegment.Sweep = (uint)(sweepDirection == SweepDirection.Clockwise ? 1 : 0);

        int offsetToArcToSegment = _currOffset;

        AppendData(arcToSegment);
        _lastSegmentSize = (uint)Unsafe.SizeOf<MIL_SEGMENT_ARC>();

        // Update the current path figure data
        _currentPathFigureData.Flags |= isStroked ? 0 : MilPathFigureFlags.HasGaps;

        _currentPathFigureData.Flags |= MilPathFigureFlags.HasCurves;

        _currentPathFigureData.Count++;

        // Always keep the OffsetToLastSegment and Size accurate
        _currentPathFigureData.Size = (uint)(_currOffset - _currentPathFigureDataOffset);

        _currentPathFigureData.OffsetToLastSegment =
            (uint)(offsetToArcToSegment - _currentPathFigureDataOffset);
    }

    /// <summary>
    /// GetData - Retrieves the data stream built by this Context.
    /// </summary>
    internal byte[] GetData()
    {
        if (_arrayToReturnToPool is not null || _buffer.Length != _currOffset)
        {
            byte[] buffer = new byte[_currOffset];

            _buffer.AsSpan(0, _currOffset).CopyTo(buffer);

            byte[] toReturn = _arrayToReturnToPool;

            _buffer = buffer;
            _arrayToReturnToPool = null;

            if (toReturn is not null)
            {
                ArrayPool<byte>.Shared.Return(toReturn);
            }
        }

        return _buffer;
    }

    internal override void SetClosedState(bool isClosed)
    {
        if (_currentPathFigureDataOffset == -1)
        {
            throw new InvalidOperationException(Strings.StreamGeometry_NeedBeginFigure);
        }

        // Clear out the IsClosed flag, then set it as appropriate.
        _currentPathFigureData.Flags &= ~MilPathFigureFlags.IsClosed;
        _currentPathFigureData.Flags |= isClosed ? MilPathFigureFlags.IsClosed : 0;
    }

    internal override void AddRect(Rect rect, Matrix transform)
    {
        base.AddRect(rect, transform);

        _currentPathFigureData.Flags |= MilPathFigureFlags.IsRectangleData;
    }

    /// <summary>
    /// This verifies that the API can be called at this time. 
    /// </summary>
    private void VerifyApi()
    {
        VerifyAccess();

        if (_disposed)
        {
            throw new ObjectDisposedException(typeof(ByteStreamGeometryContext).FullName);
        }
    }

    /// <summary>
    /// CloseCore - This method is implemented by derived classes to hand off the content 
    /// to its eventual destination.
    /// </summary>
    protected virtual void CloseCore(byte[] geometryData) { }

    /// <summary>
    /// This is the same as the Close call:
    /// Closes the Context and flushes the content.
    /// Afterwards the Context can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    internal override void DisposeCore()
    {
        if (!_disposed)
        {
            FinishFigure();

            // We have to have at least this much data already in the stream
            checked
            {
                Debug.Assert(Unsafe.SizeOf<MIL_PATHGEOMETRY>() <= _currOffset);
                Debug.Assert(_currentPathGeometryData.Size == (uint)_currOffset);
            }

            OverwriteData(_currentPathGeometryData, 0);

            CloseCore(GetData());

            _disposed = true;
        }
    }

    private void OverwriteData<T>(T data, int bufferOffset) where T : unmanaged
    {
        OverwriteData(MemoryMarshal.AsBytes(stackalloc T[1] { data }), bufferOffset);
    }

    /// <summary>
    /// OverwriteData - overwrite data in the buffer.
    /// </summary>
    /// <param name="pbData">
    ///   byte* pointing to at least cbDataSize bytes which will be copied to the stream.
    /// </param>
    /// <param name="bufferOffset"> int - the offset, in bytes, at which the data should be writen. Must be >= 0. </param>
    private void OverwriteData(Span<byte> pbData, int bufferOffset)
    {
        int newOffset;

        checked
        {
            newOffset = bufferOffset + pbData.Length;

            Debug.Assert(newOffset <= _currOffset);
        }

        EnsureCapacity(newOffset);

        pbData.CopyTo(_buffer.AsSpan(bufferOffset));
    }

    private void AppendData<T>(T data) where T : unmanaged
    {
        AppendData(stackalloc T[1] { data });
    }

    private void AppendData<T>(Span<T> data) where T : unmanaged
    {
        AppendData(MemoryMarshal.AsBytes(data));
    }

    /// <summary>
    /// AppendData - append data to the buffer.
    /// </summary>
    /// <param name="pbData">
    ///   byte* pointing to at least cbDataSize bytes which will be copied to the stream.
    /// </param>
    private void AppendData(Span<byte> pbData)
    {
        int newOffset;

        checked
        {
            newOffset = _currOffset + pbData.Length;
        }

        EnsureCapacity(newOffset);

        pbData.CopyTo(_buffer.AsSpan(_currOffset));

        _currOffset = newOffset;
    }

    private void EnsureCapacity(int capacity)
    {
        if (_buffer is not null && capacity <= _buffer.Length)
        {
            return;
        }

        int newCapacity = GetNewCapacity(capacity);

        byte[] newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);

        if (_buffer is not null)
        {
            _buffer.AsSpan(0, _currOffset).CopyTo(newBuffer);
        }

        byte[] toReturn = _arrayToReturnToPool;
        _buffer = _arrayToReturnToPool = newBuffer;

        if (toReturn is not null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }

    private int GetNewCapacity(int capacity)
    {
        int newCapacity = _buffer is null ? c_defaultChunkSize : _buffer.Length * 2;

        while ((uint)newCapacity < (uint)capacity) newCapacity *= 2;

        if ((uint)newCapacity > c_maxChunkSize) newCapacity = c_maxChunkSize;

        if (newCapacity < capacity) newCapacity = capacity;

        return newCapacity;
    }

    /// <summary>
    /// FinishFigure - called to completed any outstanding Figure which may be present.
    /// If there is one, we write its data into the stream at the appropriate offset
    /// and update the path's flags/size/figure count/etc based on this Figure.
    /// After this call, a new figure needs to be started for any segment-building APIs
    /// to be legal.
    /// </summary>
    private void FinishFigure()
    {
        if (_currentPathFigureDataOffset != -1)
        {
            FinishSegment();

            // We have to have at least this much data already in the stream
            checked
            {
                Debug.Assert(_currentPathFigureDataOffset + Unsafe.SizeOf<MIL_PATHFIGURE>() <= _currOffset);
            }

            OverwriteData(_currentPathFigureData, _currentPathFigureDataOffset);

            _currentPathGeometryData.Flags |= ((_currentPathFigureData.Flags & MilPathFigureFlags.HasCurves) != 0) ? MilPathGeometryFlags.HasCurves : 0;
            _currentPathGeometryData.Flags |= ((_currentPathFigureData.Flags & MilPathFigureFlags.HasGaps) != 0) ? MilPathGeometryFlags.HasGaps : 0;
            _currentPathGeometryData.Flags |= ((_currentPathFigureData.Flags & MilPathFigureFlags.IsFillable) == 0) ? MilPathGeometryFlags.HasHollows : 0;
            _currentPathGeometryData.FigureCount++;
            _currentPathGeometryData.Size = (uint)(_currOffset);

            _lastFigureSize = _currentPathFigureData.Size;

            // Initialize _currentPathFigureData (this really just 0's out the memory)
            _currentPathFigureDataOffset = -1;
            _currentPathFigureData = new MIL_PATHFIGURE();

            // We must also clear _lastSegmentSize, since there is now no "last segment"
            _lastSegmentSize = 0;
        }
    }

    /// <summary>
    /// FinishSegment - called to completed any outstanding Segment which may be present.
    /// If there is one, we write its data into the stream at the appropriate offset
    /// and update the figure's flags/size/segment count/etc based on this Segment.
    /// </summary>
    private void FinishSegment()
    {
        if (_currentPolySegmentDataOffset != -1)
        {
            // We have to have at least this much data already in the stream
            checked
            {
                Debug.Assert(_currentPolySegmentDataOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() <= _currOffset);
            }

            OverwriteData(_currentPolySegmentData, _currentPolySegmentDataOffset);

            _lastSegmentSize = (uint)(Unsafe.SizeOf<MIL_SEGMENT_POLY>() + (Unsafe.SizeOf<Point>() * _currentPolySegmentData.Count));

            // Update the current path figure data
            if ((_currentPolySegmentData.Flags & MILCoreSegFlags.SegIsAGap) != 0)
            {
                _currentPathFigureData.Flags |= MilPathFigureFlags.HasGaps;
            }

            if ((_currentPolySegmentData.Flags & MILCoreSegFlags.SegIsCurved) != 0)
            {
                _currentPathFigureData.Flags |= MilPathFigureFlags.HasCurves;
            }

            _currentPathFigureData.Count++;

            // Always keep the OffsetToLastSegment and Size accurate
            _currentPathFigureData.Size = (uint)(_currOffset - _currentPathFigureDataOffset);

            _currentPathFigureData.OffsetToLastSegment =
                (uint)(_currentPolySegmentDataOffset - _currentPathFigureDataOffset);

            // Initialize _currentPolySegmentData (this really just 0's out the memory)
            _currentPolySegmentDataOffset = -1;
            _currentPolySegmentData = new MIL_SEGMENT_POLY();
        }
    }

    private void GenericPolyTo(IList<Point> points,
                               bool isStroked,
                               bool isSmoothJoin,
                               bool hasCurves,
                               int pointCountMultiple,
                               MIL_SEGMENT_TYPE segmentType)
    {
        if (_currentPathFigureDataOffset == -1)
        {
            throw new InvalidOperationException(Strings.StreamGeometry_NeedBeginFigure);
        }

        if (points == null)
        {
            return;
        }

        int count = points.Count;
        count -= count % pointCountMultiple;

        if (count <= 0)
        {
            return;
        }

        GenericPolyToHelper(isStroked, isSmoothJoin, hasCurves, segmentType);

        for (int i = 0; i < count; i++)
        {
            AppendData(points[i]);

            _currentPolySegmentData.Count++;
        }
    }

    private void GenericPolyTo(Span<Point> points,
                               bool isStroked,
                               bool isSmoothJoin,
                               bool hasCurves,
                               MIL_SEGMENT_TYPE segmentType)
    {
        Debug.Assert(points.Length > 0);

        if (_currentPathFigureDataOffset == -1)
        {
            throw new InvalidOperationException(Strings.StreamGeometry_NeedBeginFigure);
        }

        GenericPolyToHelper(isStroked, isSmoothJoin, hasCurves, segmentType);

        AppendData(points);
        _currentPolySegmentData.Count += (uint)points.Length;
    }

    private void GenericPolyToHelper(bool isStroked, bool isSmoothJoin, bool hasCurves, MIL_SEGMENT_TYPE segmentType)
    {
        // Do we need to finish the old segment?
        // Yes, if there is an old segment and if its type or flags are different from 
        // the new segment.
        if ((_currentPolySegmentDataOffset != -1) &&
             (
               (_currentPolySegmentData.Type != segmentType) ||
               (((_currentPolySegmentData.Flags & MILCoreSegFlags.SegIsAGap) == 0) != isStroked) ||
               (((_currentPolySegmentData.Flags & MILCoreSegFlags.SegSmoothJoin) != 0) != isSmoothJoin)
             )
           )
        {
            FinishSegment();
        }

        // Do we need to start a new segment?
        if (_currentPolySegmentDataOffset == -1)
        {
            int oldOffset = _currOffset;

            AppendData(new MIL_SEGMENT_POLY());

            _currentPolySegmentDataOffset = oldOffset;
            _currentPolySegmentData.Type = segmentType;
            _currentPolySegmentData.Flags |= isStroked ? 0 : MILCoreSegFlags.SegIsAGap;
            _currentPolySegmentData.Flags |= hasCurves ? MILCoreSegFlags.SegIsCurved : 0;
            _currentPolySegmentData.Flags |= isSmoothJoin ? MILCoreSegFlags.SegSmoothJoin : 0;
            _currentPolySegmentData.BackSize = _lastSegmentSize;
        }

        // Assert that everything is ready to go
        Debug.Assert((_currentPolySegmentDataOffset != -1) &&
                     (_currentPolySegmentData.Type == segmentType) &&
                     (((_currentPolySegmentData.Flags & MILCoreSegFlags.SegIsAGap) == 0) == isStroked) &&
                     (((_currentPolySegmentData.Flags & MILCoreSegFlags.SegSmoothJoin) != 0) == isSmoothJoin));
    }

    private bool _disposed;
    private byte[] _buffer;
    private byte[] _arrayToReturnToPool;
    private int _currOffset;
    private MIL_PATHGEOMETRY _currentPathGeometryData;
    private MIL_PATHFIGURE _currentPathFigureData;
    private int _currentPathFigureDataOffset = -1;
    private MIL_SEGMENT_POLY _currentPolySegmentData;
    private int _currentPolySegmentDataOffset = -1;
    private uint _lastSegmentSize = 0;
    private uint _lastFigureSize = 0;

    private const int c_defaultChunkSize = 2 * 1024;
    private const int c_maxChunkSize = 0X7FFFFFC7; // Array.MaxLength
}
