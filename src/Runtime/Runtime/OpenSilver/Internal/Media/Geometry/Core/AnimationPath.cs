// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of CAnimationPath from AnimationPath.h / AnimationPath.cpp

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class CAnimationPath : CFigureTask
{
    private CAnimationSegment[] m_pSegments;
    private MilPoint2D[] m_pPoints;
    private int m_cSegments;
    private double m_rTotalLength;
    private int m_uCurrentSegment;
    private int m_uCurrentPoint;

    private double GetLength(int i)
    {
        Debug.Assert(i <= m_cSegments);
        return (i < m_cSegments) ? m_pSegments[i].GetBaseLength() : m_rTotalLength;
    }

    internal void SetUp(CShapeBase shape)
    {
        int cPoints = 0;
        int cSegments = 0;

        m_rTotalLength = 0;

        for (int i = 0; i < shape.GetFigureCount(); i++)
        {
            shape.GetFigure(i).GetCountsEstimate(out int u, out int v);
            cSegments += u;
            cPoints += v;
        }

        m_pSegments = new CAnimationSegment[cSegments];
        for (int k = 0; k < cSegments; k++)
        {
            m_pSegments[k] = new CAnimationSegment();
        }

        m_pPoints = new MilPoint2D[cPoints];

        m_uCurrentPoint = m_cSegments = 0;
        for (int i = 0; i < shape.GetFigureCount(); i++)
        {
            m_pPoints[m_uCurrentPoint++] = shape.GetFigure(i).GetStartPoint();
            TraverseForward(shape.GetFigure(i));
        }

        if (m_cSegments < 1)
        {
            throw new InvalidOperationException();
        }

        m_uCurrentSegment = m_uCurrentPoint = 0;
    }

    internal override void DoLine(in MilPoint2D ptEnd)
    {
        Debug.Assert(m_pSegments is not null);
        Debug.Assert(m_pPoints is not null);

        m_pPoints[m_uCurrentPoint] = ptEnd;
        if (m_pSegments[m_cSegments].InitAsLine(m_pPoints, m_uCurrentPoint - 1, ref m_rTotalLength))
        {
            m_cSegments++;
            m_uCurrentPoint++;
        }
    }

    internal override void DoBezier(MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3)
    {
        Debug.Assert(m_pSegments is not null);
        Debug.Assert(m_pPoints is not null);

        m_pPoints[m_uCurrentPoint] = pt1;
        m_pPoints[m_uCurrentPoint + 1] = pt2;
        m_pPoints[m_uCurrentPoint + 2] = pt3;
        if (m_pSegments[m_cSegments].InitAsCurve(m_pPoints, m_uCurrentPoint - 1, ref m_rTotalLength))
        {
            m_cSegments++;
            m_uCurrentPoint += 3;
        }
    }

    internal void GetPointAtLengthFraction(
        double rFraction,
        out MilPoint2D pt,
        out MilPoint2D vecTangent)
    {
        Debug.Assert(m_pSegments is not null);
        Debug.Assert(m_cSegments > 0);

        if (rFraction <= 0)
        {
            rFraction = 0;
            m_uCurrentSegment = 0;
        }
        else if (rFraction >= 1)
        {
            rFraction = m_rTotalLength;
            m_uCurrentSegment = m_cSegments - 1;
        }
        else
        {
            rFraction *= m_rTotalLength;

            if (rFraction > GetLength(m_uCurrentSegment + 1))
            {
                m_uCurrentSegment++;
                Debug.Assert(m_uCurrentSegment < m_cSegments);
                if (rFraction > GetLength(m_uCurrentSegment + 1))
                {
                    if (rFraction > GetLength(m_cSegments - 1))
                    {
                        m_uCurrentSegment = m_cSegments - 1;
                    }
                    else
                    {
                        BinarySearch(rFraction, m_uCurrentSegment + 1, m_cSegments);
                    }
                }
            }
            else if (rFraction < GetLength(m_uCurrentSegment))
            {
                m_uCurrentSegment--;
                if (rFraction < GetLength(m_uCurrentSegment))
                {
                    if (rFraction < GetLength(1))
                    {
                        m_uCurrentSegment = 0;
                    }
                    else
                    {
                        BinarySearch(rFraction, 0, m_uCurrentSegment);
                    }
                }
            }
        }

        m_pSegments[m_uCurrentSegment].GetPointAtLength(rFraction, out pt, out vecTangent, true);
    }

    private void BinarySearch(double rLength, int bottom, int top)
    {
        Debug.Assert(0 <= bottom);
        Debug.Assert(bottom <= top);
        Debug.Assert(top <= m_cSegments);

        while (bottom < top - 1)
        {
            int mid = (bottom + top) / 2;
            if (GetLength(mid) < rLength)
            {
                bottom = mid;
            }
            else
            {
                top = mid;
            }
        }

        m_uCurrentSegment = bottom;

        Debug.Assert(m_uCurrentSegment < m_cSegments);
    }
}
