// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of utils.h / utils.cpp

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal static class Utils
{
    internal const double FUZZ = 1.0e-6;
    internal const double FUZZ_DOUBLE = 1.0e-12;
    internal const double MIN_TOLERANCE = 1.0e-6;
    internal const double DEFAULT_FLATTENING_TOLERANCE = 0.25;
    internal const double TWICE_MIN_BEZIER_STEP_SIZE = 1.0e-3;
    internal const double MIN_GPREAL = 1.0e-30;
    internal const double MAX_GPREAL = 1.0e+30;
    internal const double SQ_LENGTH_FUZZ = 1.0e-4;
    internal const double ARC_AS_BEZIER = 0.5522847498307933984; // (sqrt(2) - 1) * 4/3
    internal const double ONE_THIRD = 1.0 / 3.0;
    internal const double TWO_THIRDS = 2.0 / 3.0;
    internal const double FOUR_THIRDS = 4.0 / 3.0;
    internal const double PI_OVER_180 = Math.PI / 180.0;
    internal const double TWO_PI = Math.PI * 2;
    internal const double SQRT_2 = 1.4142135623730950;

    internal static void ArcToBezier(
        double xStart,
        double yStart,
        double xRadius,
        double yRadius,
        double rRotation,
        bool fLargeArc,
        bool fSweepUp,
        double xEnd,
        double yEnd,
        Span<Point> pPt,
        out int cPieces)
    {
        double x, y, rHalfChord2, rCos, rSin, rCosArcAngle, rSinArcAngle, xCenter, yCenter, rBezDist;
        Point ptStart, ptEnd;
        Vector vecToBez1, vecToBez2;
        Matrix matToEllipse;

        double rFuzz2 = FUZZ * FUZZ;
        bool fZeroCenter = false;
        int i, j;

        cPieces = -1;

        // In the following, the line segment between between the arc's start and 
        // end points is referred to as "the chord".

        // Transform 1: Shift the origin to the chord's midpoint
        x = 0.5 * (xEnd - xStart);
        y = 0.5 * (yEnd - yStart);

        rHalfChord2 = x * x + y * y;     // (half chord length)^2

        // Degenerate case: single point
        if (rHalfChord2 < rFuzz2)
        {
            // The chord degenerates to a point, the arc will be ignored
            return;
        }

        // Degenerate case: straight line
        if (!AcceptRadius(rHalfChord2, rFuzz2, ref xRadius) ||
            !AcceptRadius(rHalfChord2, rFuzz2, ref yRadius))
        {
            // We have a zero radius, add a straight line segment instead of an arc
            cPieces = 0;
            return;
        }

        // Transform 2: Rotate to the ellipse's coordinate system
        if (Math.Abs(rRotation) < FUZZ)
        {
            // 
            // rRotation will almost always be 0 and Sin/Cos are expensive
            // functions. Let's not call them if we don't have to.
            //
            rCos = 1.0;
            rSin = 0.0;
        }
        else
        {
            rRotation = -rRotation * PI_OVER_180;

            rCos = Math.Cos(rRotation);
            rSin = Math.Sin(rRotation);

            double rTemp = x * rCos - y * rSin;
            y = x * rSin + y * rCos;
            x = rTemp;
        }

        // Transform 3: Scale so that the ellipse will become a unit circle
        x /= xRadius;
        y /= yRadius;

        // We get to the center of that circle along a vector perpendicular to the chord   
        // from the origin, which is the chord's midpoint. By Pythagoras, the length of that
        // vector is sqrt(1 - (half chord)^2).

        rHalfChord2 = x * x + y * y;   // now in the circle coordinates   
        if (rHalfChord2 > 1.0)
        {
            // The chord is longer than the circle's diameter; we scale the radii uniformly so 
            // that the chord will be a diameter. The center will then be the chord's midpoint,
            // which is now the origin.
            double rTemp = Math.Sqrt(rHalfChord2);

            xRadius *= rTemp;
            yRadius *= rTemp;
            xCenter = yCenter = 0;
            fZeroCenter = true;

            // Adjust the unit-circle coordinates x and y
            x /= rTemp;
            y /= rTemp;
        }
        else
        {
            // The length of (-y,x) or (x,-y) is sqrt(rHalfChord2), and we want a vector
            // of length sqrt(1 - rHalfChord2), so we'll multiply it by:
            double rTemp = Math.Sqrt((1.0 - rHalfChord2) / rHalfChord2);
            if (fLargeArc != fSweepUp)
            // Going to the center from the origin=chord-midpoint
            {
                // in the direction of (-y, x)
                xCenter = -rTemp * y;
                yCenter = rTemp * x;
            }
            else
            {
                // in the direction of (y, -x)
                xCenter = rTemp * y;
                yCenter = -rTemp * x;
            }
        }

        // Transformation 4: shift the origin to the center of the circle, which then becomes
        // the unit circle. Since the chord's midpoint is the origin, the start point is (-x, -y)
        // and the endpoint is (x, y).
        ptStart = new Point(-x - xCenter, -y - yCenter);
        ptEnd = new Point(x - xCenter, y - yCenter);

        // Set up the matrix that will take us back to our coordinate system.  This matrix is
        // the inverse of the combination of transformation 1 thru 4.
        matToEllipse = new Matrix(
            rCos * xRadius, -rSin * xRadius,
            rSin * yRadius, rCos * yRadius,
            0.5 * (xEnd + xStart), 0.5 * (yEnd + yStart));

        if (!fZeroCenter)
        {
            // Prepend the translation that will take the origin to the circle's center
            matToEllipse.OffsetX += (matToEllipse.M11 * xCenter + matToEllipse.M21 * yCenter);
            matToEllipse.OffsetY += (matToEllipse.M12 * xCenter + matToEllipse.M22 * yCenter);
        }

        // Get the sine & cosine of the angle that will generate the arc pieces
        GetArcAngle(ptStart, ptEnd, fLargeArc, fSweepUp, out rCosArcAngle, out rSinArcAngle, out cPieces);

        // Get the vector to the first Bezier control point
        rBezDist = GetBezierDistance(rCosArcAngle);
        if (!fSweepUp)
        {
            rBezDist = -rBezDist;
        }
        vecToBez1 = new Vector(-rBezDist * ptStart.Y, rBezDist * ptStart.X);

        // Add the arc pieces, except for the last
        j = 0;
        for (i = 1; i < cPieces; i++)
        {
            // Get the arc piece's endpoint
            var ptPieceEnd = new Point(ptStart.X * rCosArcAngle - ptStart.Y * rSinArcAngle,
                                       ptStart.X * rSinArcAngle + ptStart.Y * rCosArcAngle);
            vecToBez2 = new Vector(-rBezDist * ptPieceEnd.Y, rBezDist * ptPieceEnd.X);

            pPt[j++] = matToEllipse.Transform(ptStart + vecToBez1);
            pPt[j++] = matToEllipse.Transform(ptPieceEnd - vecToBez2);
            pPt[j++] = matToEllipse.Transform(ptPieceEnd);

            // Move on to the next arc
            ptStart = ptPieceEnd;
            vecToBez1 = vecToBez2;
        }

        // Last arc - we know the endpoint
        vecToBez2 = new Vector(-rBezDist * ptEnd.Y, rBezDist * ptEnd.X);

        pPt[j++] = matToEllipse.Transform(ptStart + vecToBez1);
        pPt[j++] = matToEllipse.Transform(ptEnd - vecToBez2);

        // j is less than 3*cPieces
        Debug.Assert(j < 12);
        pPt[j] = new Point(xEnd, yEnd);
    }

    private static double GetBezierDistance(double rDot, double rRadius = 1.0)
    {
        double rRadSquared = rRadius * rRadius;
        double rNumerator;
        double rDenominatorSquared, rDenominator;

        // Ignore NaNs
        Debug.Assert(!(rDot < -rRadSquared * .1));  // angle < 90 degrees
        Debug.Assert(!(rDot > rRadSquared * 1.1));  // as dot product of 2 radius vectors

        double rDist = 0.0;   // Acceptable fallback value

        double rA = 0.5 * (rRadSquared + rDot);

        if (rA < 0.0)
        {
            // Shouldn't happen but dist=0 will work
            return rDist;
        }

        rDenominatorSquared = rRadSquared - rA;

        if (rDenominatorSquared <= 0)
        {
            // 0 angle, we shouldn't be rounding the corner, but dist=0 is OK
            return rDist;
        }

        rDenominator = Math.Sqrt(rDenominatorSquared);
        rNumerator = FOUR_THIRDS * (rRadius - Math.Sqrt(rA));

        if (rNumerator <= rDenominator * FUZZ)
        {
            //
            // Dist is very close to 0, so we'll snap it to 0 and save ourselves a
            // divide.
            //
            rDist = 0.0;
        }
        else
        {
            rDist = rNumerator / rDenominator;
        }

        return rDist;
    }

    private static void GetArcAngle(
        Point ptStart,
        Point ptEnd,
        bool fLargeArc,
        bool fSweepUp,
        out double rCosArcAngle,
        out double rSinArcAngle,
        out int cPieces)
    {
        // The points are on the unit circle, so:
        rCosArcAngle = ptStart.X * ptEnd.X + ptStart.Y * ptEnd.Y;
        rSinArcAngle = ptStart.X * ptEnd.Y - ptStart.Y * ptEnd.X;

        if (rCosArcAngle >= 0)
        {
            if (fLargeArc)
            {
                // The angle is between 270 and 360 degrees, so
                cPieces = 4;
            }
            else
            {
                // The angle is between 0 and 90 degrees, so
                cPieces = 1;
                return;
            }
        }
        else
        {
            if (fLargeArc)
            {
                // The angle is between 180 and 270 degrees, so
                cPieces = 3;
            }
            else
            {
                // The angle is between 90 and 180 degrees, so
                cPieces = 2;
            }
        }

        //
        // We have to chop the arc into the computed number of pieces.  For
        // cPieces=2 and 4 we could have uses the half-angle trig formulas, but for
        // cPieces=3 it requires solving a cubic equation; the performance
        // difference is not worth the extra code, so we'll get the angle, divide
        // it, and get its sine and cosine.
        //

        double rAngle = Math.Atan2(rSinArcAngle, rCosArcAngle);
        if (fSweepUp)
        {
            if (rAngle < 0)
            {
                rAngle += TWO_PI;
            }
        }
        else
        {
            if (rAngle > 0)
            {
                rAngle -= TWO_PI;
            }
        }

        rAngle /= cPieces;
        rCosArcAngle = Math.Cos(rAngle);
        rSinArcAngle = Math.Sin(rAngle);
    }

    private static bool AcceptRadius(double rHalfChord2, double rFuzz2, ref double rRadius)
    {
        // Ignore NaNs
        Debug.Assert(!(rHalfChord2 < rFuzz2));

        //
        // If the above assert is not true, we have no guarantee that the radius is
        // not 0, and we need to divide by the radius.
        //

        //
        // Accept NaN here, because otherwise we risk forgetting we encountered
        // one.
        //
        bool fAccept = !(rRadius * rRadius <= rHalfChord2 * rFuzz2);
        if (fAccept)
        {
            if (rRadius < 0)
            {
                rRadius = -rRadius;
            }
        }
        return fAccept;
    }
}

internal sealed class CBounds
{
    private double _xMin = double.MaxValue;
    private double _xMax = double.MinValue;
    private double _yMin = double.MaxValue;
    private double _yMax = double.MinValue;
    private bool _encounteredNaN;

    internal bool NotUpdated => _xMax < _xMin && _yMax < _yMin;

    internal Rect GetRect()
    {
        if (_encounteredNaN)
        {
            return new Rect(double.NaN, double.NaN, double.NaN, double.NaN);
        }
        else if (_xMin <= _xMax && _yMin <= _yMax)
        {
            return new Rect(_xMin, _yMin, _xMax - _xMin, _yMax - _yMin);
        }
        else
        {
            return new Rect(0, 0, 0, 0);
        }
    }

    internal void UpdateWithPoint(in MilPoint2D pt)
    {
        if (pt.X < _xMin) _xMin = pt.X;
        if (pt.X > _xMax) _xMax = pt.X;
        if (pt.Y < _yMin) _yMin = pt.Y;
        if (pt.Y > _yMax) _yMax = pt.Y;
        UpdateNaN(pt);
    }

    internal void UpdateWithBezier(in MilPoint2D pt0, in MilPoint2D pt1, in MilPoint2D pt2, in MilPoint2D pt3)
    {
        UpdateWithPoint(pt3);

        UpdateNaN(pt1);
        UpdateNaN(pt2);

        Span<double> r = stackalloc double[2];

        int nZeros = GetDerivativeZeros(pt0.X, pt1.X, pt2.X, pt3.X, r);
        for (int j = 0; j < nZeros; j++)
        {
            double x = GetBezierPolynomValue(pt0.X, pt1.X, pt2.X, pt3.X, r[j]);
            if (x < _xMin)
            {
                _xMin = x;
            }
            else if (x > _xMax)
            {
                _xMax = x;
            }
            UpdateNaN(x);
        }

        nZeros = GetDerivativeZeros(pt0.Y, pt1.Y, pt2.Y, pt3.Y, r);
        for (int j = 0; j < nZeros; j++)
        {
            double y = GetBezierPolynomValue(pt0.Y, pt1.Y, pt2.Y, pt3.Y, r[j]);
            if (y < _yMin)
            {
                _yMin = y;
            }
            else if (y > _yMax)
            {
                _yMax = y;
            }
            UpdateNaN(y);
        }
    }

    private void UpdateNaN(double x)
    {
        _encounteredNaN = _encounteredNaN || double.IsNaN(x);
    }

    private void UpdateNaN(in MilPoint2D pt)
    {
        _encounteredNaN = _encounteredNaN || double.IsNaN(pt.X) || double.IsNaN(pt.Y);
    }

    private int SolveSpecialQuadratic(double a, double b, double c, Span<double> r)
    {
        int nZeros = 0;
        double d = b * b - a * c;

        UpdateNaN(d);

        if (d > 0)
        {
            d = Math.Sqrt(d);
            b = -b;
            r[nZeros] = (b - d) / a;
            UpdateNaN(r[nZeros]);
            if (r[nZeros] > 0)
            {
                nZeros++;
            }
            r[nZeros] = (b + d) / a;
            UpdateNaN(r[nZeros]);
            if (r[nZeros] > 0)
            {
                nZeros++;
            }
        }

        return nZeros;
    }

    private int GetDerivativeZeros(double a, double b, double c, double d, Span<double> r)
    {
        int nZeros = 0;

        if ((b - a) * (d - b) >= 0 && (c - a) * (d - c) >= 0)
        {
            return nZeros;
        }

        a = b - a;
        b = c - b;
        c = d - c;
        double fa = Math.Abs(a);
        double fb = Math.Abs(b);
        double fc = Math.Abs(c);
        double fuzz = fb * Utils.FUZZ;

        if (fa < fuzz && fc < fuzz)
        {
            return nZeros;
        }

        if (fa > fc)
        {
            nZeros = SolveSpecialQuadratic(a, b, c, r);
            for (int i = 0; i < nZeros; i++)
            {
                r[i] = 1.0 / (1 + r[i]);
            }
        }
        else
        {
            nZeros = SolveSpecialQuadratic(c, b, a, r);
            for (int i = 0; i < nZeros; i++)
            {
                r[i] = r[i] / (1 + r[i]);
            }
        }

        return nZeros;
    }

    private static double GetBezierPolynomValue(double a, double b, double c, double d, double t)
    {
        double t2 = t * t;
        double s = 1 - t;
        double s2 = s * s;

        return a * s * s2 + 3 * b * t * s2 + 3 * c * t2 * s + d * t * t2;
    }
}
