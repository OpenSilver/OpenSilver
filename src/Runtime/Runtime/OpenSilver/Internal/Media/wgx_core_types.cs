// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace OpenSilver.Internal.Media;

internal enum MIL_SEGMENT_TYPE
{
    MilSegmentNone,
    MilSegmentLine,
    MilSegmentBezier,
    MilSegmentQuadraticBezier,
    MilSegmentArc,
    MilSegmentPolyLine,
    MilSegmentPolyBezier,
    MilSegmentPolyQuadraticBezier,

    MIL_SEGMENT_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
}

[Flags]
internal enum MILCoreSegFlags
{
    SegTypeLine = 0x00000001,
    SegTypeBezier = 0x00000002,
    SegTypeMask = 0x00000003,

    // When this bit is set then this segment is not to be stroked
    SegIsAGap = 0x00000004,

    // When this bit is set then the join between this segment and the PREVIOUS segment
    // will be rounded upon widening, regardless of the pen line join property.
    SegSmoothJoin = 0x00000008,

    // When this bit is set on the first type then the figure should be closed.
    SegClosed = 0x00000010,

    // This bit indicates whether the segment is curved.
    SegIsCurved = 0x00000020,

    FORCE_DWORD = unchecked((int)0xffffffff)
}

[Flags]
internal enum MilPathGeometryFlags
{
    HasCurves = 0x00000001,
    BoundsValid = 0x00000002,
    HasGaps = 0x00000004,
    HasHollows = 0x00000008,
    IsRegionData = 0x00000010,
    Mask = 0x0000001F,

    FORCE_DWORD = unchecked((int)0xffffffff)
}

[Flags]
internal enum MilPathFigureFlags
{
    HasGaps = 0x00000001,
    HasCurves = 0x00000002,
    IsClosed = 0x00000004,
    IsFillable = 0x00000008,
    IsRectangleData = 0x00000010,
    Mask = 0x0000001F,

    FORCE_DWORD = unchecked((int)0xffffffff)
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_PATHGEOMETRY
{
    [FieldOffset(0)] internal uint Size;
    [FieldOffset(4)] internal MilPathGeometryFlags Flags;
    [FieldOffset(8)] internal MilRectD Bounds;
    [FieldOffset(40)] internal uint FigureCount;
    [FieldOffset(44)] internal uint ForcePacking;    // See "ForcePacking" comment at beginning of this file.
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_PATHFIGURE
{
    [FieldOffset(0)] internal uint BackSize;
    [FieldOffset(4)] internal MilPathFigureFlags Flags;
    [FieldOffset(8)] internal uint Count;
    [FieldOffset(12)] internal uint Size;
    [FieldOffset(16)] internal Point StartPoint;
    [FieldOffset(32)] internal uint OffsetToLastSegment;
    [FieldOffset(36)] internal uint ForcePacking;         // See "ForcePacking" comment at beginning of this file.
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT_LINE
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
    [FieldOffset(12)] internal uint ForcePacking;         // See "ForcePacking" comment at beginning of this file.
    [FieldOffset(16)] internal Point Point;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT_BEZIER
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
    [FieldOffset(12)] internal uint ForcePacking;         // See "ForcePacking" comment at beginning of this file.
    [FieldOffset(16)] internal Point Point1;
    [FieldOffset(32)] internal Point Point2;
    [FieldOffset(48)] internal Point Point3;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT_QUADRATICBEZIER
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
    [FieldOffset(12)] internal uint ForcePacking;          // See "ForcePacking" comment at beginning of this file.
    [FieldOffset(16)] internal Point Point1;
    [FieldOffset(32)] internal Point Point2;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT_ARC
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
    [FieldOffset(12)] internal uint LargeArc;
    [FieldOffset(16)] internal Point Point;
    [FieldOffset(32)] internal Size Size;
    [FieldOffset(48)] internal double XRotation;
    [FieldOffset(56)] internal uint Sweep;
    [FieldOffset(60)] internal uint ForcePacking;          // See "ForcePacking" comment at beginning of this file.
}

[StructLayout(LayoutKind.Explicit)]
internal struct MIL_SEGMENT_POLY
{
    [FieldOffset(0)] internal MIL_SEGMENT_TYPE Type;
    [FieldOffset(4)] internal MILCoreSegFlags Flags;
    [FieldOffset(8)] internal uint BackSize;
    [FieldOffset(12)] internal uint Count;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MilRectD
{
    internal MilRectD(double left, double top, double right, double bottom)
    {
        _left = left;
        _top = top;
        _right = right;
        _bottom = bottom;
    }

    internal static MilRectD Empty => new MilRectD(0, 0, 0, 0);

    internal static MilRectD NaN => new MilRectD(double.NaN, double.NaN, double.NaN, double.NaN);


    internal Rect AsRect
    {
        get
        {
            if (_right >= _left && _bottom >= _top)
            {
                return new Rect(_left, _top, _right - _left, _bottom - _top);
            }
            else
            {
                // In particular, we treat NaN rectangles as empty rects.
                return Rect.Empty;
            }
        }
    }

    [FieldOffset(0)] internal double _left;
    [FieldOffset(8)] internal double _top;
    [FieldOffset(16)] internal double _right;
    [FieldOffset(24)] internal double _bottom;
}