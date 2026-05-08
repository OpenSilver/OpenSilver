// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of FigureTask.h / FigureTask.cpp

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Base class for traversing a figure and performing a task on every segment.
/// Subclasses override DoLine and DoBezier.
/// </summary>
internal abstract class CFigureTask : CFlatteningSink
{
    protected bool _aborted;

    internal CFigureTask()
    {
        _aborted = false;
    }

    internal abstract void DoLine(in MilPoint2D ptEnd);

    internal abstract void DoBezier(MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3);

    internal bool WasAborted => _aborted;

    /// <summary>
    /// Traverse the figure forward, calling DoLine/DoBezier on each segment.
    /// </summary>
    internal void TraverseForward(IFigureData figure)
    {
        _aborted = false;

        if (!figure.SetToFirstSegment())
        {
            return;
        }

        Span<MilPoint2D> pts = stackalloc MilPoint2D[3];

        do
        {
            figure.GetCurrentSegment(out MILCoreSegFlags bType, pts);
            if (MILCoreSegFlags.SegTypeLine == bType)
            {
                DoLine(pts[0]);
            }
            else
            {
                Debug.Assert(MILCoreSegFlags.SegTypeBezier == bType);
                DoBezier(pts[0], pts[1], pts[2]);
            }
        }
        while (!_aborted && figure.SetToNextSegment());
    }
}

/// <summary>
/// Computes axis-aligned bounds for a figure by traversing its segments.
/// </summary>
internal sealed class CBoundsTask : CFigureTask
{
    private readonly CBounds _bounds;
    private MilPoint2D _ptCurrent;

    internal CBoundsTask(CBounds bounds, in MilPoint2D ptFirst)
    {
        _bounds = bounds;
        _ptCurrent = ptFirst;
        _bounds.UpdateWithPoint(ptFirst);
    }

    internal override void DoLine(in MilPoint2D ptEnd)
    {
        _ptCurrent = ptEnd;
        _bounds.UpdateWithPoint(_ptCurrent);
    }

    internal override void DoBezier(MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3)
    {
        _bounds.UpdateWithBezier(_ptCurrent, pt1, pt2, pt3);
        _ptCurrent = pt3;
    }
}
