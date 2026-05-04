
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media;

// Ported from WPF native code (https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/WpfGfx/core/geometry/utils.cpp)
internal static class GeometryUtils
{
    private const double FUZZ = 1.0e-6;
    private const double FOUR_THIRDS = 4.0 / 3.0;
    private const double PI_OVER_180 = Math.PI / 180.0;
    private const double TWO_PI = Math.PI * 2;

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
