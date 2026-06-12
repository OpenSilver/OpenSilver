// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of ShapeFlattener.h

using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class CShapeFlattener : CFlatteningSink, IPopulationSink
{
    private readonly double m_rTolerance;
    private CPopulationSinkAdapter m_pSink;
    private MilPoint2D m_ptCurrent;

    public CShapeFlattener(CapacityStreamGeometryContext context, double rTolerance)
    {
        m_pSink = new CPopulationSinkAdapter(context);
        m_rTolerance = rTolerance;
    }

    public void StartFigure(in MilPoint2D pt)
    {
        m_ptCurrent = pt;
        m_pSink.StartFigure(pt);
    }

    public void AddLine(in MilPoint2D ptNew)
    {
        m_ptCurrent = ptNew;
        m_pSink.AddLine(ptNew);
    }

    public void AddCurve(in MilPoint2D pt1, in MilPoint2D pt2, in MilPoint2D pt3)
    {
        var flattener = new CBezierFlattener(this, m_rTolerance)
        {
            Point0 = m_ptCurrent,
            Point1 = pt1,
            Point2 = pt2,
            Point3 = pt3
        };

        m_ptCurrent = pt3;

        flattener.Flatten(false);
    }

    public void SetCurrentVertexSmooth(bool val) => m_pSink.SetCurrentVertexSmooth(val);

    public void SetStrokeState(bool val) => m_pSink.SetStrokeState(val);

    public void EndFigure(bool fClosed) => m_pSink.EndFigure(fClosed);

    public void SetFillMode(FillRule fillMode) => m_pSink.SetFillMode(fillMode);

    internal override void Begin(in MilPoint2D pt)
    {
        // Ignore -- we've already dealt with this point.
    }

    internal override void AcceptPoint(in MilPoint2D pt, double t, out bool fAborted)
    {
        fAborted = false;
        m_pSink.AddLine(pt);
    }
}
