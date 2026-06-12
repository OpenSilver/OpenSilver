// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of PopulationSinkAdapter.h

using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Converts IPopulationSink method calls to CapacityStreamGeometryContext calls.
/// </summary>
internal struct CPopulationSinkAdapter : IPopulationSink
{
    private readonly CapacityStreamGeometryContext m_context;

    private MIL_SEGMENT_TYPE m_eLastSegmentType;

    private MilPoint2D m_lastPoint0;
    private MilPoint2D m_lastPoint1;
    private MilPoint2D m_lastPoint2;

    private bool m_fLastPointSmooth;
    private bool m_fStrokedState;
    private bool m_fStrokeStateUpdated;

    public CPopulationSinkAdapter(CapacityStreamGeometryContext context)
    {
        m_context = context;
    }

    public void StartFigure(in MilPoint2D pt)
    {
        m_context.BeginFigure(new Point(pt.X, pt.Y), isFilled: true, isClosed: true);
        m_eLastSegmentType = MIL_SEGMENT_TYPE.MilSegmentNone;
    }

    public void AddLine(in MilPoint2D ptNew)
    {
        AddLastSegment();

        m_lastPoint0 = ptNew;

        m_eLastSegmentType = MIL_SEGMENT_TYPE.MilSegmentLine;
        m_fLastPointSmooth = false;
    }

    public void AddCurve(in MilPoint2D pt1, in MilPoint2D pt2, in MilPoint2D pt3)
    {
        AddLastSegment();

        m_lastPoint0 = pt1;
        m_lastPoint1 = pt2;
        m_lastPoint2 = pt3;

        m_eLastSegmentType = MIL_SEGMENT_TYPE.MilSegmentBezier;
        m_fLastPointSmooth = false;
    }

    public void SetCurrentVertexSmooth(bool val)
    {
        Debug.Assert(m_eLastSegmentType == MIL_SEGMENT_TYPE.MilSegmentLine ||
                     m_eLastSegmentType == MIL_SEGMENT_TYPE.MilSegmentBezier);

        m_fLastPointSmooth = val;
    }

    public void SetStrokeState(bool val)
    {
        if (val != m_fStrokedState)
        {
            m_fStrokeStateUpdated = true;
            m_fStrokedState = val;
        }
    }

    public void EndFigure(bool fClosed)
    {
        AddLastSegment();

        if (fClosed)
        {
            m_context.SetClosedState(fClosed);
        }
    }

    public void SetFillMode(FillRule fillMode)
    {
        // Nothing to do, fillMode was already provided to the context.
    }

    private void AddLastSegment()
    {
        switch (m_eLastSegmentType)
        {
            case MIL_SEGMENT_TYPE.MilSegmentLine:
                m_context.LineTo(new Point(m_lastPoint0.X, m_lastPoint0.Y), m_fStrokeStateUpdated, m_fLastPointSmooth);
                break;

            case MIL_SEGMENT_TYPE.MilSegmentBezier:
                m_context.BezierTo(
                    new Point(m_lastPoint0.X, m_lastPoint0.Y),
                    new Point(m_lastPoint1.X, m_lastPoint1.Y),
                    new Point(m_lastPoint2.X, m_lastPoint2.Y),
                    m_fStrokeStateUpdated,
                    m_fLastPointSmooth);
                break;

            default:
                Debug.Assert(m_eLastSegmentType == MIL_SEGMENT_TYPE.MilSegmentNone);
                break;
        }

        m_fStrokeStateUpdated = false;
    }
}
