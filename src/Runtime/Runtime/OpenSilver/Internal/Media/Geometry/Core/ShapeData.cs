// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of ShapeData.h

using System;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Interface for access and queries on figure data.
/// Provides segment iteration over a figure's geometry.
/// </summary>
internal abstract class IFigureData
{
    internal abstract bool IsEmpty();

    internal abstract bool HasNoSegments();

    internal abstract void GetCountsEstimate(out int cSegments, out int cPoints);

    internal abstract bool IsClosed();

    internal abstract bool IsAtASmoothJoin();

    internal abstract bool HasGaps();

    internal abstract bool IsAtAGap();

    internal abstract bool IsFillable();

    internal abstract bool IsAParallelogram();

    internal abstract bool IsAxisAlignedRectangle();

    internal abstract MilRectD GetAsRectangle();

    internal abstract void GetParallelogramVertices(out MilPoint2D p0, out MilPoint2D p1, out MilPoint2D p2, out MilPoint2D p3, Matrix matrix = default);

    internal abstract Rect GetAsWellOrderedRectangle();

    internal abstract bool SetToFirstSegment();

    internal abstract bool GetCurrentSegment(out MILCoreSegFlags bType, Span<MilPoint2D> points);

    internal abstract bool SetToNextSegment();

    internal abstract MilPoint2D GetCurrentSegmentStart();

    internal abstract MilPoint2D GetStartPoint();

    internal abstract MilPoint2D GetEndPoint();

    internal abstract void SetStop();

    internal abstract void ResetStop();

    internal abstract bool IsStopSet();
}
