// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of FigureTask.h / FigureTask.cpp

using System;
using System.Diagnostics;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal struct CMILBezierFlattener
{
    private CBezierFlattener _bezierFlattener;

    public CMILBezierFlattener(
        MilPoint2D ptFirst,
        MilPoint2D pt0,
        MilPoint2D pt1,
        MilPoint2D pt2,
        CFlatteningSink CFpSink,
        double rTolerance,
        Matrix matrix)
    {
        _bezierFlattener = new CBezierFlattener(CFpSink, rTolerance);
        SetPoints(0, 1, ptFirst, pt0, pt1, pt2, matrix);
    }

    public void Flatten(bool fWithTangents) => _bezierFlattener.Flatten(fWithTangents);

    private void SetPoints(
        double rStart,
        double rEnd,
        MilPoint2D ptFirst,
        MilPoint2D pt0,
        MilPoint2D pt1,
        MilPoint2D pt2,
        Matrix matrix)
    {
        // The caller should not be asking for trimming outside [0,1].
        // Ignore NaNs
        Debug.Assert(!(0 > rStart));
        Debug.Assert(!(rStart > rEnd));
        Debug.Assert(!(rEnd > 1));

        _bezierFlattener.Point0 = ptFirst;

        if (matrix.IsIdentity)
        {
            _bezierFlattener.Point1 = pt0;
            _bezierFlattener.Point2 = pt1;
            _bezierFlattener.Point3 = pt2;
        }
        else
        {
            _bezierFlattener.Point1 = pt0 * matrix;
            _bezierFlattener.Point2 = pt1 * matrix;
            _bezierFlattener.Point3 = pt2 * matrix;
        }

        // Trimming = computing Bezier points for a curve that represents a portion
        // of the curve defined by the input points.
        if (rEnd <= rStart + Utils.FUZZ)
        {
            // The trimmed curve degenerates to a point
            _bezierFlattener.Point0 = pt0;
            _bezierFlattener.Point1 = pt0;
            _bezierFlattener.Point2 = pt0;
            _bezierFlattener.Point3 = pt0;
        }
        else
        {
            if (rStart > 0)
            {
                _bezierFlattener.TrimToStartAt(rStart);
            }

            if (rEnd < 1)
            {
                // If rStart > 0 then the curve has been trimmed, but the Bezier points represent a
                // curve with parameter domain [0,1], oblivious to that trimming.  So we need to
                // adjust the second trimming parameter to reflect the first trimming. For example, 
                // supposed rStart = 0.2 and rEnd = 0.6. After trimming 0.2 from the start, we want the
                // second trim to leave us with [0.2, 0.6].  The size of this domain is 0.4, which is
                // 0.5 of 0.8 - the size remaining after the first trim.  We get that with 
                // (0.6-0.2)/(1-0.2). In general, the new trim parameter is 
                // (rEnd - rStart) / (1 - rStart).

                if (rStart > 0)
                {
                    // Ignore NaNs
                    Debug.Assert(!(Utils.FUZZ >= 1 - rStart));  // Since rStart + FUZZ < rEnd <= 1
                    rEnd = (rEnd - rStart) / (1 - rStart);
                }

                _bezierFlattener.TrimToEndAt(rEnd);
            }
        }
    }
}

/// <summary>
/// Base class for traversing a figure and performing a task on every segment.
/// Subclasses override DoLine and DoBezier.
/// </summary>
internal abstract class CFigureTask : CFlatteningSink
{
    protected bool m_fAborted;

    internal CFigureTask()
    {
        m_fAborted = false;
    }

    internal abstract void DoLine(in MilPoint2D ptEnd);

    internal abstract void DoBezier(MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3);

    internal bool WasAborted => m_fAborted;

    /// <summary>
    /// Traverse the figure forward, calling DoLine/DoBezier on each segment.
    /// </summary>
    internal void TraverseForward(IFigureData figure)
    {
        m_fAborted = false;

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
        while (!m_fAborted && figure.SetToNextSegment());
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

internal sealed class CHitTest : CFigureTask
{
    private readonly Matrix m_oMatrix;
    private readonly double m_rSquaredThreshold;
    private MilPoint2D m_ptCurrent;
    private int m_iWinding;

    public CHitTest(MilPoint2D ptHit, Matrix matrix, double rThreshold)
    {
        m_rSquaredThreshold = rThreshold * rThreshold;

        // The winding number computation can produce an incorrect result if the hit point
        // is close to the boundary, so the following is necessary for the integrity of the
        // algorithm.
        if (m_rSquaredThreshold < Utils.SQ_LENGTH_FUZZ)
        {
            m_rSquaredThreshold = Utils.SQ_LENGTH_FUZZ;
        }

        m_oMatrix = matrix;

        // Set the transformation to shift the hit point to the origin
        m_oMatrix.Translate(-ptHit.X, -ptHit.Y);
    }

    internal override void DoLine(in MilPoint2D ptEnd)
    {
        MilPoint2D pt = ptEnd * m_oMatrix;
        AcceptPoint(pt, 1, out m_fAborted);
    }

    internal override void DoBezier(MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3)
    {
        var curve = new CMILBezierFlattener(m_ptCurrent, pt1, pt2, pt3, this, Utils.DEFAULT_FLATTENING_TOLERANCE, m_oMatrix);
        curve.Flatten(false);
    }

    internal override void AcceptPoint(in MilPoint2D pt, double t, out bool fAborted)
    {
        AcceptPointNoHRESULT(pt, t, out fAborted);
    }

    internal bool StartAt(in MilPoint2D ptFirst)
    {
        m_ptCurrent = ptFirst * m_oMatrix;
        return m_fAborted = m_ptCurrent * m_ptCurrent < m_rSquaredThreshold;
    }

    internal bool EndAt(in MilPoint2D ptFirst)
    {
        MilPoint2D pt = ptFirst * m_oMatrix;

        Debug.Assert(!m_fAborted);  // Otherwise we should have aborted

        AcceptPointNoHRESULT(pt, 0.0, out m_fAborted);

        return m_fAborted;
    }

    internal int GetWindingNumber()
    {
        Debug.Assert(!m_fAborted);  // Otherwise the number may be bogus due to early out

        return m_iWinding;
    }

    private void AcceptPointNoHRESULT(MilPoint2D ptEnd, double t, out bool fHit)
    {
        Debug.Assert(!m_fAborted);   // Should have bailed out otherwise

        // This segment is close enough to the origin
        CheckIfNearTheOrigin(ptEnd);
        fHit = m_fAborted;

        if (!m_fAborted)
        {
            // Update the number of path intersections with the positive x axis
            UpdateWith(ptEnd);
        }

        m_ptCurrent = ptEnd;
    }

    private void CheckIfNearTheOrigin(MilPoint2D ptEnd)
    {
        // Check if the endpoint is near the origin.  No need to check the start
        // point, it was checked as the endpoint of the previous segment
        m_fAborted = ptEnd * ptEnd < m_rSquaredThreshold;

        if (!m_fAborted)
        {
            // Now check if there is point in the segment that is close enough to the origin
            // Let vec = ptEnd - m_ptCurrent be the segment vector.  The segment is 
            //  
            //      P(t) = m_ptCurrent + t*vec.
            //
            // If P(t) is the point on the line nearest to the origin then P(t) is 
            // perpendicular to segment, i.e. P(t) * vec = 0. The equation for t is then
            //
            //      (m_ptCurrent + t*vec) * vec = 0.
            //
            // The solution is
            //
            //      t = -(m_ptCurrent * vec) / (vec * vec)
            //
            // and it is inside the segment if 0 < t < 1. 
            //
            // The point at t is 
            //
            //      P = m_ptCurrent + ( (m_ptCurrent * vec) / (vec * vec) )*vec.
            //
            // and its squared distance from the origin is P * P.  If (0<t<=1) we
            // want to check if P * P < m_rSquaredThreshold.  But to avoid 
            // divisions, we'll set r = vec * vec, and multiply 0<t<=1 by r
            // and P * P < m_rSquaredThreshold by r*r.

            var vec = new MilPoint2D(m_ptCurrent, ptEnd);
            double r = vec * vec;
            double t = -(m_ptCurrent * vec);
            if (0 <= t && t <= r)
            {
                // The nearest point is inside the segment, examine its distance
                MilPoint2D Pr = m_ptCurrent * r + vec * t;  //=P*r
                m_fAborted = Pr * Pr < m_rSquaredThreshold * r * r;
            }
        }
    }

    private void UpdateWith(MilPoint2D ptEnd)
    {
        // The talying of crossing of the positive x axis may fail if the origin is
        // very closed to the (transformed) path, but then we'll be saved by the
        // nearness test, provided the tolerance is not too small.  So:
        Debug.Assert(!(m_rSquaredThreshold <= Utils.FUZZ)); // Ignore NaNs

        // If this segment crosses the x axis we have to determine whether it
        // does it at the positive half.  The x of the intersection is a weighted
        // average of the x coordinates of the segment's endpoints. By triangle
        // similarity, the ratio of the distances between the crossing x and
        // the x's of the endpoints is equal to |ptEnd.Y| / |m_ptCurrent.Y|.
        // 
        //
        //           * ptEnd
        //           |\
        //      *----*-\--*-------------
        //              \ |
        //               \|
        //                * m_ptCurrent
        //          
        // This translates to x = s * m_ptCurrent.X + t * ptEnd.X, where
        // s = |ptEnd.Y|/r, t = |m_ptCurrent.Y|/r, r = |m_ptCurrent.Y|+|ptEnd.Y|.
        // Since we are only interested in the sign of x, we can multiply that by 
        // r (which is known to be positive) and examine the sign of 
        // |m_ptCurrent.Y| * ptEnd.X + |ptEnd.Y| * m_ptCurrent.X.
        //
        // Instead of taking abs of both Y's we check their signs, which we need anyway,
        // and adjust them to be + when we test.

        if (m_ptCurrent.Y > 0)
        {
            if (ptEnd.Y <= 0)
            {
                // We have crossed the x axis going down
                if (m_ptCurrent.X * ptEnd.Y - ptEnd.X * m_ptCurrent.Y >= 0)
                {
                    // The crossing was on the positive side
                    m_iWinding--;
                }
            }
        }
        else    // m_ptCurrent.Y <= 0
        {
            if (ptEnd.Y > 0)
            {
                // We have crossed the x axis going up
                if (ptEnd.X * m_ptCurrent.Y - m_ptCurrent.X * ptEnd.Y >= 0)
                {
                    // The crossing was on the positive side 
                    m_iWinding++;
                }
            }
        }
    }
}
