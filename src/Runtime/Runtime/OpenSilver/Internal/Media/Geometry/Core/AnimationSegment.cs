// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of CAnimationSegment from AnimationPath.h / AnimationPath.cpp

using System;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class CAnimationSegment : CIncreasingFunction
{
    private static readonly double[] g_rgGaussSample = [0.2113248654051875, 0.7886751345948125];
    private const double g_rFuzzBreaks = 0.01;

    // Line/curve segment
    private MILCoreSegFlags m_bType;
    private MilPoint2D[] m_ppt;
    private int m_pptOffset;
    private MilPoint2D m_vecTangent;

    // For curve only
    private MilPoint2D m_vecD1_0;
    private MilPoint2D m_vecD1_1;
    private MilPoint2D m_vecD1_2;

    private MilPoint2D m_vecD2_0;
    private MilPoint2D m_vecD2_1;

    private MilPoint2D m_vecD3;

    // Breaks, lengths etc.
    private int m_cBreaks;
    private readonly double[] m_rgBreak = new double[5];
    private readonly double[] m_rgLength = new double[5];
    private readonly double[] m_rgMid = new double[4];
    private double m_rBaseLength;

    // Computation variables
    private double m_rTargetLength;
    private int m_uiCurrentSpan;
    private double m_rLatest;

    internal CAnimationSegment()
    {
        m_rBaseLength = 0;
        m_uiCurrentSpan = 0;
        m_rLatest = 0;
    }

    internal double GetLength()
    {
        return m_rgLength[m_cBreaks - 1];
    }

    internal double GetBaseLength()
    {
        return m_rBaseLength;
    }

    internal bool InitAsLine(
        MilPoint2D[] ppt,
        int offset,
        ref double rLength)
    {
        m_bType = MILCoreSegFlags.SegTypeLine;
        m_cBreaks = 2;
        m_rBaseLength = rLength;
        m_rgLength[0] = 0;

        m_ppt = ppt;
        m_pptOffset = offset;

        m_vecTangent = ppt[offset + 1] - ppt[offset + 0];
        m_rgLength[1] = m_vecTangent.Norm();
        if (m_rgLength[1] < Utils.FUZZ)
        {
            return false;
        }
        else
        {
            m_vecTangent *= 1.0 / m_rgLength[1];
        }

        rLength += m_rgLength[1];

        return true;
    }

    internal bool InitAsCurve(
        MilPoint2D[] ppt,
        int offset,
        ref double rLength)
    {
        m_rBaseLength = rLength;

        m_cBreaks = 2;
        m_rgBreak[0] = 0;
        m_rgBreak[1] = 1;
        m_rgLength[0] = 0;
        m_rgLength[1] = 0;
        m_rgMid[0] = 0;

        m_bType = MILCoreSegFlags.SegTypeBezier;
        m_ppt = ppt;
        m_pptOffset = offset;

        m_vecD1_0 = (ppt[offset + 1] - ppt[offset + 0]) * 3;
        m_vecD1_1 = (ppt[offset + 2] - ppt[offset + 1]) * 3;
        m_vecD1_2 = (ppt[offset + 3] - ppt[offset + 2]) * 3;

        m_vecD2_0 = (m_vecD1_1 - m_vecD1_0) * 2;
        m_vecD2_1 = (m_vecD1_2 - m_vecD1_1) * 2;

        m_vecD3 = m_vecD2_1 - m_vecD2_0;

        m_vecD1_1 *= 2; // To avoid the need to multiply by 2 when evaluating

        m_vecTangent = m_vecD1_0;
        if (!Unitize(ref m_vecTangent))
        {
            m_vecTangent = ppt[offset + 2] - ppt[offset + 1];
            if (!Unitize(ref m_vecTangent))
            {
                m_vecTangent = ppt[offset + 3] - ppt[offset + 1];
                if (!Unitize(ref m_vecTangent))
                {
                    return false;
                }
            }
        }

        SetBreaks();

        for (int i = 1; i < m_cBreaks; i++)
        {
            m_rgMid[i - 1] = (m_rgBreak[i - 1] + m_rgBreak[i]) / 2;
            m_rgLength[i] = m_rgLength[i - 1] + GetLengthBetween(m_rgBreak[i - 1], m_rgMid[i - 1])
                                              + GetLengthBetween(m_rgMid[i - 1], m_rgBreak[i]);
        }

        rLength += m_rgLength[m_cBreaks - 1];

        return true;
    }

    internal void GetPointAtLength(
        double rLength,
        out MilPoint2D pt,
        out MilPoint2D vecTangent,
        bool wantTangent)
    {
        rLength -= m_rBaseLength;
        if (rLength > m_rgLength[m_cBreaks - 1])
        {
            rLength = m_rgLength[m_cBreaks - 1];
        }

        if (MILCoreSegFlags.SegTypeLine == m_bType)
        {
            GetPointAndTangentOnLine(rLength, out pt, out vecTangent, wantTangent);
        }
        else
        {
            GetPointAndTangentOnCurve(rLength, out pt, out vecTangent, wantTangent);
        }
    }

    private void GetPointAndTangentOnLine(
        double rLength,
        out MilPoint2D pt,
        out MilPoint2D vecTangent,
        bool wantTangent)
    {
        if (0 == rLength)
        {
            pt = m_ppt[m_pptOffset + 0];
        }
        else
        {
            pt = m_ppt[m_pptOffset + 0] + m_vecTangent * rLength;
        }

        vecTangent = wantTangent ? m_vecTangent : default;
    }

    private void GetPointAndTangentOnCurve(
        double rLength,
        out MilPoint2D pt,
        out MilPoint2D vecTangent,
        bool wantTangent)
    {
        double t = GetParameterFromLength(rLength);

        if (t <= 0)
        {
            pt = m_ppt[m_pptOffset + 0];
            if (wantTangent)
            {
                vecTangent = m_vecD1_0;
            }
            else
            {
                vecTangent = default;
            }
        }
        else if (t >= 1)
        {
            pt = m_ppt[m_pptOffset + 3];
            if (wantTangent)
            {
                vecTangent = m_vecD1_2;
            }
            else
            {
                vecTangent = default;
            }
        }
        else
        {
            double s = 1 - t;
            double s2 = s * s;
            double t2 = t * t;

            pt = m_ppt[m_pptOffset + 0] * (s2 * s) + m_ppt[m_pptOffset + 1] * (3 * s2 * t) +
                 m_ppt[m_pptOffset + 2] * (3 * s * t2) + m_ppt[m_pptOffset + 3] * (t * t2);

            if (wantTangent)
            {
                vecTangent = m_vecD1_0 * s2 + m_vecD1_1 * (s * t) + m_vecD1_2 * t2;
            }
            else
            {
                vecTangent = default;
            }
        }

        if (wantTangent)
        {
            if (Unitize(ref vecTangent))
            {
                m_vecTangent = vecTangent;
            }
            else
            {
                vecTangent = m_vecTangent;
            }
        }
    }

    private void Get2Derivatives(
        double t,
        out MilPoint2D vecD1,
        out MilPoint2D vecD2)
    {
        double s = 1 - t;

        vecD1 = m_vecD1_0 * (s * s) + m_vecD1_1 * (s * t) + m_vecD1_2 * (t * t);
        vecD2 = m_vecD2_0 * s + m_vecD2_1 * t;
    }

    private double GetSpeed(double t)
    {
        double s = 1 - t;

        MilPoint2D vecVelocity = m_vecD1_0 * (s * s) + m_vecD1_1 * (s * t) + m_vecD1_2 * (t * t);

        return vecVelocity.Norm();
    }

    private void GetSpeedAndDerivative(
        double t,
        out double speed,
        out double derivative)
    {
        Get2Derivatives(t, out MilPoint2D vecVelocity, out MilPoint2D vecAcceleration);

        speed = vecVelocity.Norm();

        derivative = vecVelocity * vecAcceleration;

        if (speed > Math.Abs(derivative) * Utils.FUZZ)
        {
            derivative /= speed;
        }
        else
        {
            derivative = 0;
        }
    }

    private double GetLengthBetween(double from, double to)
    {
        double integral = GetSpeed((1 - g_rgGaussSample[0]) * from + g_rgGaussSample[0] * to) +
                          GetSpeed((1 - g_rgGaussSample[1]) * from + g_rgGaussSample[1] * to);

        return integral * (to - from) * 0.5;
    }

    private double GetExtent()
    {
        double xMin = m_ppt[m_pptOffset + 0].X;
        double xMax = xMin;
        double yMin = m_ppt[m_pptOffset + 0].Y;
        double yMax = yMin;

        for (int i = 1; i < 4; i++)
        {
            if (m_ppt[m_pptOffset + i].X < xMin)
            {
                xMin = m_ppt[m_pptOffset + i].X;
            }
            else if (m_ppt[m_pptOffset + i].X > xMax)
            {
                xMax = m_ppt[m_pptOffset + i].X;
            }
            if (m_ppt[m_pptOffset + i].Y < yMin)
            {
                yMin = m_ppt[m_pptOffset + i].Y;
            }
            else if (m_ppt[m_pptOffset + i].Y > yMax)
            {
                yMax = m_ppt[m_pptOffset + i].Y;
            }
        }

        xMax -= xMin;
        yMax -= yMin;
        return Math.Max(xMax, yMax);
    }

    private double GetParameterFromLength(double rLength)
    {
        if (rLength <= 0)
        {
            m_rLatest = 0;
        }
        else if (rLength >= m_rgLength[m_cBreaks - 1])
        {
            m_rLatest = 1;
        }
        else
        {
            m_rTargetLength = rLength;
            while (m_rTargetLength < m_rgLength[m_uiCurrentSpan])
            {
                m_uiCurrentSpan--;
            }

            while (m_uiCurrentSpan < 3 && m_rTargetLength > m_rgLength[m_uiCurrentSpan + 1])
            {
                m_uiCurrentSpan++;
            }

            if (m_rLatest < m_rgBreak[m_uiCurrentSpan])
            {
                m_rLatest = m_rgBreak[m_uiCurrentSpan];
            }
            else if (m_rLatest > m_rgBreak[m_uiCurrentSpan + 1])
            {
                m_rLatest = m_rgBreak[m_uiCurrentSpan + 1];
            }

            SolveNewtonRaphson(
                m_rgBreak[m_uiCurrentSpan],
                m_rgBreak[m_uiCurrentSpan + 1],
                m_rLatest,
                Utils.FUZZ,
                Utils.FUZZ * m_rgLength[m_cBreaks - 1],
                out double rLatest);

            m_rLatest = rLatest;
        }

        return m_rLatest;
    }

    private void AcceptBreak(double t)
    {
        if (t > g_rFuzzBreaks && t < 1 - g_rFuzzBreaks && m_cBreaks < 5)
        {
            int i = 0;
            do
            {
                i++;
            }
            while (i < m_cBreaks && t > m_rgBreak[i]);

            if (((0 == i) || t > m_rgBreak[i - 1] + g_rFuzzBreaks) && ((m_cBreaks == i) || t < m_rgBreak[i] - g_rFuzzBreaks))
            {
                int k = m_cBreaks - i;
                if (k > 0)
                {
                    Array.Copy(m_rgBreak, i, m_rgBreak, i + 1, k);
                }
                m_rgBreak[i] = t;
                m_cBreaks++;
            }
        }
    }

    private void SetBreaks()
    {
        double delta = Utils.FUZZ * 10;

        double epsilon = GetExtent();
        epsilon = epsilon * epsilon * delta;

        m_rgBreak[0] = 0;
        m_rgBreak[1] = 1;
        m_cBreaks = 2;

        var squaredSpeedDerivative = new CSquaredSpeedDerivative(this);

        if (squaredSpeedDerivative.SolveNewtonRaphson(0, 1, 0, delta, epsilon, out double t))
        {
            AcceptBreak(t);
        }

        if (squaredSpeedDerivative.SolveNewtonRaphson(0, 1, 0.5, delta, epsilon, out t))
        {
            AcceptBreak(t);
        }

        if (squaredSpeedDerivative.SolveNewtonRaphson(0, 1, 1, delta, epsilon, out t))
        {
            AcceptBreak(t);
        }

        if (2 == m_cBreaks)
        {
            m_rgBreak[4] = m_rgBreak[1];
            m_rgBreak[2] = (m_rgBreak[0] + m_rgBreak[4]) / 2;
            m_rgBreak[1] = (m_rgBreak[0] + m_rgBreak[2]) / 2;
            m_rgBreak[3] = (m_rgBreak[2] + m_rgBreak[4]) / 2;
            m_cBreaks = 5;
        }
        else if (3 == m_cBreaks)
        {
            m_rgBreak[4] = m_rgBreak[2];
            m_rgBreak[2] = m_rgBreak[1];
            m_rgBreak[1] = (m_rgBreak[0] + m_rgBreak[2]) / 2;
            m_rgBreak[3] = (m_rgBreak[2] + m_rgBreak[4]) / 2;
            m_cBreaks = 5;
        }
    }

    internal override void GetValueAndDerivative(
        double t,
        out double f,
        out double df)
    {
        int reference;

        if (t > m_rgMid[m_uiCurrentSpan])
        {
            reference = m_uiCurrentSpan + 1;
        }
        else
        {
            reference = m_uiCurrentSpan;
        }

        f = df = 0;

        for (int i = 0; i < 2; i++)
        {
            double r = (1 - g_rgGaussSample[i]) * m_rgBreak[reference] + g_rgGaussSample[i] * t;
            GetSpeedAndDerivative(r, out double speed, out double derivative);
            derivative *= g_rgGaussSample[i];

            f += speed;
            df += derivative;
        }

        f *= 0.5;
        df *= 0.5;

        t -= m_rgBreak[reference];
        df = t * df + f;
        f = m_rgLength[reference] + t * f - m_rTargetLength;
    }

    private static bool Unitize(ref MilPoint2D vec)
    {
        double rLength = vec.Norm();

        if (rLength >= Utils.FUZZ)
        {
            rLength = 1.0 / rLength;
            vec = new MilPoint2D(vec.X * rLength, vec.Y * rLength);
            return true;
        }
        else
        {
            return false;
        }
    }

    private sealed class CSquaredSpeedDerivative : CRealFunction
    {
        private readonly CAnimationSegment m_refCurve;

        internal CSquaredSpeedDerivative(CAnimationSegment curve)
        {
            m_refCurve = curve;
        }

        internal override void GetValueAndDerivative(
            double t,
            out double f,
            out double df)
        {
            m_refCurve.Get2Derivatives(t, out MilPoint2D vecVelocity, out MilPoint2D vecAcceleration);

            f = vecAcceleration * vecVelocity;
            df = m_refCurve.m_vecD3 * vecVelocity + vecAcceleration * vecAcceleration;
        }
    }
}
