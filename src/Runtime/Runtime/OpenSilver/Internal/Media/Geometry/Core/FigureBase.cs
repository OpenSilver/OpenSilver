// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of FigureBase.h / FigureBase.cpp

using System;
using System.Diagnostics;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Wraps an IFigureData and provides operations on it:
/// populating a scanner, updating bounds, etc.
/// </summary>
internal readonly struct CFigureBase
{
    private readonly IFigureData _data;

    internal CFigureBase(IFigureData data)
    {
        _data = data;
    }

    /// <summary>
    /// Populate a scanner (IPopulationSink) with this figure's geometry.
    /// The traversal checks smoothness at each join, which is why it's
    /// separate from CFigureTask::TraverseForward.
    /// </summary>
    internal void Populate(IPopulationSink scanner, Matrix matrix)
    {
        if (_data.HasNoSegments())
        {
            return;
        }

        MilPoint2D ptCurrent = _data.GetStartPoint();
        if (!matrix.IsIdentity)
        {
            ptCurrent *= matrix;
        }

        scanner.StartFigure(ptCurrent);

        if (!_data.SetToFirstSegment())
        {
            return;
        }

        Span<MilPoint2D> pts = stackalloc MilPoint2D[3];

        do
        {
            _data.GetCurrentSegment(out MILCoreSegFlags bType, pts);
            bool fAtGap = _data.IsAtAGap();

            scanner.SetStrokeState(!fAtGap);

            if (MILCoreSegFlags.SegTypeLine == bType)
            {
                if (!matrix.IsIdentity)
                {
                    pts[0] *= matrix;
                }

                ptCurrent = pts[0];
                scanner.AddLine(ptCurrent);
            }
            else
            {
                Debug.Assert(MILCoreSegFlags.SegTypeBezier == bType);
                if (!matrix.IsIdentity)
                {
                    pts[0] *= matrix;
                    pts[1] *= matrix;
                    pts[2] *= matrix;
                }

                scanner.AddCurve(pts[0], pts[1], pts[2]);
            }

            scanner.SetCurrentVertexSmooth(_data.IsAtASmoothJoin());
        }
        while (_data.SetToNextSegment());

        scanner.EndFigure(_data.IsClosed());
    }
}
