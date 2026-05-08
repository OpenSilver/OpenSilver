// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of LineSegmentIntersection.h / LineSegmentIntersection.cpp

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class CLineSegmentIntersection
{
    // Describes the nature of the intersection between the line segments AB and CD. 
    internal enum KIND
    {
        EMPTY = 0,             // The intersection is empty.
        NONTRANSVERSE = 1,     // AB and CD are parallel and AB intersects CD.
        TRANSVERSE = 2,        // AB and CD are not parallel and AB intersects CD.
        UNDEFINED = 3,         // The intersection is undefined, either because the input points were invalid or the computation failed.
    }

    // Describes the various line segment pairs association flavors.
    // Do not modify the enumeration member values.
    internal enum PAIRING
    {
        FIRST_FIRST = 0,     // The first line segment of both pairs. 
        FIRST_LAST = 1,      // First on the first pair and last on the second pair.
        LAST_FIRST = 2,      // Last on the first pair and first on the second pair.
        LAST_LAST = 3,       // The last line segment on both pairs.
    }

    // Describes the location of the intersection point I with respect to the line segment
    // AB (resp. CD) when the intersection exists and is transverse.
    internal enum LOCATION
    {
        AT_FIRST_POINT = 0,     // I equals A (resp. C)  
        ON_OPEN_SEGMENT = 1,    // I is in the open line segment AB (resp. CD)     
        AT_LAST_POINT = 2,      // I equals B (resp. D)  
        UNDEFINED = 3,          // Either the intersection is empty, or not transverse, or the input points were invalid, or the computation failed.
    }

    // Describes the position of a point relative to an oriented line.
    // This definition assumes a right-handed coordinate system. Client
    // applications should swap rigt and left if they use a left-handed coordinate system.
    // Do not modify the enumeration member values.
    internal enum SIDEINDICATOR
    {
        RIGHT = -1,      // The point lies in the open half-plane right of the oriented line.
        INCIDENT = 0,    // The point is incident to the oriented line.
        LEFT = 1,        // The point lies in the open half-plane left of the oriented line.
    }

    // Describes where a point M lies relative to an oriented line segment defined by 
    // its endpoints First and Last. We assume that First and Last are distinct. 
    // Do not modify the enumeration member values.
    internal enum REGION
    {
        HALFLINE_BEFOREFIRST = 0,    // M is incident to the line and located before First.
        EQUAL_TO_FIRST = 1,          // M equals First.        
        OPEN_LINESEGMENT = 2,        // M is inside the open line segment (First, Last).   
        EQUAL_TO_LAST = 3,           // M equals Last.        
        HALFLINE_AFTERLAST = 4,      // M is incident to the line and located after Last.  
        LEFT_OPENHALFPLANE = 5,      // M is not incident to the line and lies in the open half plane left of (First, Last).
        RIGHT_OPENHALFPLANE = 6,     // M is not incident to the line and lies in the open half plane right of (First, Last).
        UNDEFINED = 7,               // The region is undefined. 
    }

    // Data members: components of vectors AB, DC, AC.
    private double m_xAB, m_yAB, m_xDC, m_yDC, m_xAC, m_yAC;

    // Coordinates of point A.
    private double m_xA, m_yA;

    // Exact or approximate determinant values.
    private double m_rDeterminantABDC, m_rDeterminantACDC, m_rDeterminantABAC;

    private KIND m_eKind;
    private LOCATION m_eLocationAB, m_eLocationCD;

    private SIGNINDICATOR m_eSignABDC, m_eSignACDC, m_eSignABAC;
    private bool m_fExactABDC, m_fExactACDC, m_fExactABAC;

    internal CLineSegmentIntersection()
    {
        Initialize();
    }

    internal void Initialize()
    {
        m_eKind = KIND.UNDEFINED;
        m_eLocationAB = m_eLocationCD = LOCATION.UNDEFINED;
    }

    private static bool DeterminantIsExactDouble(double a, double b, double c, double d)
    {
        return Math.Abs(a) <= IntegerConstants.LARGESTINTEGER26 &&
               Math.Abs(b) <= IntegerConstants.LARGESTINTEGER26 &&
               Math.Abs(c) <= IntegerConstants.LARGESTINTEGER26 &&
               Math.Abs(d) <= IntegerConstants.LARGESTINTEGER26;
    }

    /// <summary>
    /// Computes the sign of a*d - b*c and sets r to an approximate value.
    /// Arguments must be valid Integer31 values.
    /// </summary>
    private static SIGNINDICATOR ComputeDeterminantExactSign(
        double a, double b, double c, double d, out double r)
    {
        double ad = a * d;
        double bc = b * c;
        SIGNINDICATOR eResult = SIGNINDICATOR.ZERO;

        r = ad - bc;
        if (ad != bc || Math.Abs(ad) <= IntegerConstants.LARGESTINTEGER53)
        {
            eResult = ad > bc ? SIGNINDICATOR.STRICTLY_POSITIVE :
                     (ad < bc ? SIGNINDICATOR.STRICTLY_NEGATIVE : SIGNINDICATOR.ZERO);
        }
        else
        {
            var aCZ = new CZ64(a);
            var bCZ = new CZ64(b);
            var cCZ = new CZ64(c);
            var dCZ = new CZ64(d);
            aCZ.Multiply(dCZ);
            bCZ.Multiply(cCZ);
            eResult = (SIGNINDICATOR)(int)aCZ.Compare(bCZ);
        }
        return eResult;
    }

    /// <summary>
    /// Computes the sign of a*d - b*c.
    /// Arguments may need up to 33 bits.
    /// </summary>
    private static SIGNINDICATOR ComputeDeterminantExactSign(
        double a, double b, double c, double d)
    {
        SIGNINDICATOR eResult = SIGNINDICATOR.ZERO;
        double ad = a * d;
        double bc = b * c;
        if (ad != bc)
        {
            eResult = ad > bc ? SIGNINDICATOR.STRICTLY_POSITIVE : SIGNINDICATOR.STRICTLY_NEGATIVE;
        }
        else if (Math.Abs(ad) <= IntegerConstants.LARGESTINTEGER53)
        {
            eResult = SIGNINDICATOR.ZERO;
        }
        else
        {
            var aCZ = new CZ128(a);
            var bCZ = new CZ128(b);
            var cCZ = new CZ128(c);
            var dCZ = new CZ128(d);
            aCZ.Multiply(dCZ);
            bCZ.Multiply(cCZ);
            eResult = (SIGNINDICATOR)(int)aCZ.Compare(bCZ);
        }
        return eResult;
    }

    private static double ClampToZeroOne(double v)
    {
        return Math.Max(0.0, Math.Min(v, 1.0));
    }

    private static COMPARISON YXComparePoints(double xA, double yA, double xB, double yB)
    {
        return yA > yB ? COMPARISON.STRICTLYGREATERTHAN :
              (yA < yB ? COMPARISON.STRICTLYLESSTHAN :
              (xA > xB ? COMPARISON.STRICTLYGREATERTHAN :
              (xA < xB ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.EQUAL)));
    }

    // ===================== Internal Properties =====================

    internal KIND GetKind() => m_eKind;

    internal LOCATION GetLocationAB() => m_eLocationAB;

    internal LOCATION GetLocationCD() => m_eLocationCD;

    internal bool IntersectionIsNotEmpty() =>
        m_eKind == KIND.NONTRANSVERSE || m_eKind == KIND.TRANSVERSE;

    internal bool IntersectionIsTransverse() =>
        m_eKind == KIND.TRANSVERSE;

    internal bool IntersectionIsNonTransverse() =>
        m_eKind == KIND.NONTRANSVERSE;

    internal bool IntersectionIsTransverseOnOpenSegments() =>
        m_eKind == KIND.TRANSVERSE &&
        m_eLocationAB == LOCATION.ON_OPEN_SEGMENT &&
        m_eLocationCD == LOCATION.ON_OPEN_SEGMENT;

    // ===================== Private Accessors =====================

    private bool LambdaIsZero()
    {
        Debug.Assert(IntersectionIsTransverse());
        return m_eLocationAB == LOCATION.AT_FIRST_POINT;
    }

    private bool LambdaIsOne()
    {
        Debug.Assert(IntersectionIsTransverse());
        return m_eLocationAB == LOCATION.AT_LAST_POINT;
    }

    private bool MuIsZero()
    {
        Debug.Assert(IntersectionIsTransverse());
        return m_eLocationCD == LOCATION.AT_FIRST_POINT;
    }

    private bool MuIsOne()
    {
        Debug.Assert(IntersectionIsTransverse());
        return m_eLocationCD == LOCATION.AT_LAST_POINT;
    }

    private bool DeterminantABDCIsExact() => m_fExactABDC;
    private bool DeterminantACDCIsExact() => m_fExactACDC;
    private bool DeterminantABACIsExact() => m_fExactABAC;

    private double DeterminantABDC() => m_rDeterminantABDC;
    private double DeterminantACDC() => m_rDeterminantACDC;
    private double DeterminantABAC() => m_rDeterminantABAC;

    private SIGNINDICATOR SignDeterminantABDC() => m_eSignABDC;
    private SIGNINDICATOR SignDeterminantACDC() => m_eSignACDC;
    private SIGNINDICATOR SignDeterminantABAC() => m_eSignABAC;

    // ===================== Internal Methods =====================

    internal KIND PairwiseIntersect(
        ReadOnlySpan<double> ab,
        ReadOnlySpan<double> cd,
        out LOCATION eLocationAB,
        out LOCATION eLocationCD)
    {
        Reset();
        eLocationAB = eLocationCD = LOCATION.UNDEFINED;

        if (Math.Min(ab[0], ab[2]) > Math.Max(cd[0], cd[2]) ||
            Math.Max(ab[0], ab[2]) < Math.Min(cd[0], cd[2]) ||
            Math.Min(ab[1], ab[3]) > Math.Max(cd[1], cd[3]) ||
            Math.Max(ab[1], ab[3]) < Math.Min(cd[1], cd[3]))
        {
            m_eKind = KIND.EMPTY;
        }
        else
        {
            m_xAB = ab[2] - ab[0];
            m_yAB = ab[3] - ab[1];
            m_xDC = cd[0] - cd[2];
            m_yDC = cd[1] - cd[3];
            m_xAC = cd[0] - ab[0];
            m_yAC = cd[1] - ab[1];

            m_xA = ab[0];
            m_yA = ab[1];

            // Compute Determinant(AB, DC).
            m_fExactABDC = DeterminantIsExactDouble(m_xAB, m_yAB, m_xDC, m_yDC);
            if (m_fExactABDC)
            {
                m_rDeterminantABDC = (m_xAB * m_yDC) - (m_yAB * m_xDC);
                m_eSignABDC = m_rDeterminantABDC > 0 ? SIGNINDICATOR.STRICTLY_POSITIVE :
                    (m_rDeterminantABDC < 0 ? SIGNINDICATOR.STRICTLY_NEGATIVE : SIGNINDICATOR.ZERO);
            }
            else
            {
                m_eSignABDC = ComputeDeterminantExactSign(
                    m_xAB, m_yAB, m_xDC, m_yDC, out m_rDeterminantABDC);
            }

            if (m_eSignABDC == SIGNINDICATOR.ZERO)
            {
                // Lines AB and CD are parallel.
                m_fExactABAC = DeterminantIsExactDouble(m_xAB, m_yAB, m_xAC, m_yAC);
                if (m_fExactABAC)
                {
                    m_rDeterminantABAC = (m_xAB * m_yAC) - (m_yAB * m_xAC);
                    m_eSignABAC = m_rDeterminantABAC > 0 ? SIGNINDICATOR.STRICTLY_POSITIVE :
                        (m_rDeterminantABAC < 0 ? SIGNINDICATOR.STRICTLY_NEGATIVE : SIGNINDICATOR.ZERO);
                }
                else
                {
                    m_eSignABAC = ComputeDeterminantExactSign(
                        m_xAB, m_yAB, m_xAC, m_yAC, out m_rDeterminantABAC);
                }

                if (m_eSignABAC == SIGNINDICATOR.ZERO)
                {
                    REGION eRegionC = ComputeRegionWhenPointPIsOnAB(m_xAB, m_yAB, m_xAC, m_yAC);
                    REGION eRegionD = ComputeRegionWhenPointPIsOnAB(
                        m_xAB, m_yAB, cd[2] - ab[0], cd[3] - ab[1]);

                    if ((eRegionC == REGION.HALFLINE_BEFOREFIRST &&
                         eRegionD == REGION.HALFLINE_BEFOREFIRST) ||
                        (eRegionC == REGION.HALFLINE_AFTERLAST &&
                         eRegionD == REGION.HALFLINE_AFTERLAST))
                    {
                        m_eKind = KIND.EMPTY;
                    }
                    else
                    {
                        m_eKind = KIND.NONTRANSVERSE;
                    }
                }
                else
                {
                    m_eKind = KIND.EMPTY;
                }
            }
            else
            {
                // Compute Determinant(AC, DC).
                m_fExactACDC = DeterminantIsExactDouble(m_xAC, m_yAC, m_xDC, m_yDC);
                if (m_fExactACDC)
                {
                    m_rDeterminantACDC = (m_xAC * m_yDC) - (m_yAC * m_xDC);
                    m_eSignACDC = m_rDeterminantACDC > 0 ? SIGNINDICATOR.STRICTLY_POSITIVE :
                        (m_rDeterminantACDC < 0 ? SIGNINDICATOR.STRICTLY_NEGATIVE : SIGNINDICATOR.ZERO);
                }
                else
                {
                    m_eSignACDC = ComputeDeterminantExactSign(
                        m_xAC, m_yAC, m_xDC, m_yDC, out m_rDeterminantACDC);
                }

                // Test lambda >= 0.
                if ((m_eSignABDC == SIGNINDICATOR.STRICTLY_NEGATIVE && m_eSignACDC == SIGNINDICATOR.STRICTLY_POSITIVE) ||
                    (m_eSignABDC == SIGNINDICATOR.STRICTLY_POSITIVE && m_eSignACDC == SIGNINDICATOR.STRICTLY_NEGATIVE))
                {
                    m_eKind = KIND.EMPTY;
                }
                else
                {
                    // Test lambda <= 1.
                    COMPARISON eCompareLambdaAndOne = CompareDeterminantABDCandDeterminantACDC();
                    if (m_eSignABDC == SIGNINDICATOR.STRICTLY_POSITIVE)
                    {
                        eCompareLambdaAndOne = ComparisonHelper.Opposite(eCompareLambdaAndOne);
                    }
                    if (eCompareLambdaAndOne == COMPARISON.STRICTLYGREATERTHAN)
                    {
                        m_eKind = KIND.EMPTY;
                    }
                    else
                    {
                        // Compute Determinant(AB, AC).
                        m_fExactABAC = DeterminantIsExactDouble(m_xAB, m_yAB, m_xAC, m_yAC);
                        if (m_fExactABAC)
                        {
                            m_rDeterminantABAC = (m_xAB * m_yAC) - (m_yAB * m_xAC);
                            m_eSignABAC = m_rDeterminantABAC > 0 ? SIGNINDICATOR.STRICTLY_POSITIVE :
                                (m_rDeterminantABAC < 0 ? SIGNINDICATOR.STRICTLY_NEGATIVE : SIGNINDICATOR.ZERO);
                        }
                        else
                        {
                            m_eSignABAC = ComputeDeterminantExactSign(
                                m_xAB, m_yAB, m_xAC, m_yAC, out m_rDeterminantABAC);
                        }

                        // Test mu >= 0.
                        if ((m_eSignABDC == SIGNINDICATOR.STRICTLY_NEGATIVE &&
                             m_eSignABAC == SIGNINDICATOR.STRICTLY_POSITIVE) ||
                            (m_eSignABDC == SIGNINDICATOR.STRICTLY_POSITIVE &&
                             m_eSignABAC == SIGNINDICATOR.STRICTLY_NEGATIVE))
                        {
                            m_eKind = KIND.EMPTY;
                        }
                        else
                        {
                            // Test mu <= 1.
                            COMPARISON eCompareMuAndOne = CompareDeterminantABDCandDeterminantABAC();
                            if (m_eSignABDC == SIGNINDICATOR.STRICTLY_POSITIVE)
                            {
                                eCompareMuAndOne = ComparisonHelper.Opposite(eCompareMuAndOne);
                            }
                            if (eCompareMuAndOne == COMPARISON.STRICTLYGREATERTHAN)
                            {
                                m_eKind = KIND.EMPTY;
                            }
                            else
                            {
                                m_eKind = KIND.TRANSVERSE;

                                if (m_eSignACDC == SIGNINDICATOR.ZERO)
                                {
                                    m_eLocationAB = LOCATION.AT_FIRST_POINT;
                                }
                                else if (eCompareLambdaAndOne == COMPARISON.STRICTLYLESSTHAN)
                                {
                                    m_eLocationAB = LOCATION.ON_OPEN_SEGMENT;
                                }
                                else
                                {
                                    m_eLocationAB = LOCATION.AT_LAST_POINT;
                                }

                                if (m_eSignABAC == SIGNINDICATOR.ZERO)
                                {
                                    m_eLocationCD = LOCATION.AT_FIRST_POINT;
                                }
                                else if (eCompareMuAndOne == COMPARISON.STRICTLYLESSTHAN)
                                {
                                    m_eLocationCD = LOCATION.ON_OPEN_SEGMENT;
                                }
                                else
                                {
                                    m_eLocationCD = LOCATION.AT_LAST_POINT;
                                }
                            }
                        }
                    }
                }
            }
        }

        eLocationAB = m_eLocationAB;
        eLocationCD = m_eLocationCD;
        return m_eKind;
    }

    internal bool IsEqual(CLineSegmentIntersection efgh)
    {
        return m_xA == efgh.m_xA && m_yA == efgh.m_yA &&
               m_xAB == efgh.m_xAB && m_yAB == efgh.m_yAB &&
               m_xAC == efgh.m_xAC && m_yAC == efgh.m_yAC &&
               m_xDC == efgh.m_xDC && m_yDC == efgh.m_yDC;
    }

    internal double ParameterAlongAB()
    {
        Debug.Assert(IntersectionIsTransverse() && m_rDeterminantABDC != 0.0);

        double rResult = -1.0;

        if (IntersectionIsTransverse() && m_rDeterminantABDC != 0.0)
        {
            if (m_eLocationAB == LOCATION.AT_FIRST_POINT)
            {
                rResult = 0.0;
            }
            else if (m_eLocationAB == LOCATION.AT_LAST_POINT)
            {
                rResult = 1.0;
            }
            else
            {
                rResult = ClampToZeroOne(m_rDeterminantACDC / m_rDeterminantABDC);
            }
        }
        return rResult;
    }

    internal double ParameterAlongCD()
    {
        Debug.Assert(IntersectionIsTransverse() && m_rDeterminantABDC != 0.0);

        double rResult = -1.0;

        if (IntersectionIsTransverse() && m_rDeterminantABDC != 0.0)
        {
            if (m_eLocationCD == LOCATION.AT_FIRST_POINT)
            {
                rResult = 0.0;
            }
            else if (m_eLocationCD == LOCATION.AT_LAST_POINT)
            {
                rResult = 1.0;
            }
            else
            {
                rResult = ClampToZeroOne(m_rDeterminantABAC / m_rDeterminantABDC);
            }
        }
        return rResult;
    }

    internal SIDEINDICATOR LocateTransverseIntersectionRelativeToLine(ReadOnlySpan<double> ef)
    {
        Debug.Assert(IntersectionIsTransverse() &&
            m_eLocationAB == LOCATION.ON_OPEN_SEGMENT &&
            m_eLocationCD == LOCATION.ON_OPEN_SEGMENT);
        Debug.Assert(ef[0] != ef[2] || ef[1] != ef[3]);

        SIGNINDICATOR eResult = SIGNINDICATOR.ZERO;

        SIGNINDICATOR eSignA = ComputeDeterminantExactSign(
            ef[2] - ef[0], ef[3] - ef[1], m_xA - ef[0], m_yA - ef[1]);
        SIGNINDICATOR eSignB = ComputeDeterminantExactSign(
            ef[2] - ef[0], ef[3] - ef[1], m_xAB + (m_xA - ef[0]), m_yAB + (m_yA - ef[1]));

        if (eSignA == eSignB)
        {
            eResult = eSignA;
        }
        else if (eSignA == SIGNINDICATOR.ZERO)
        {
            eResult = eSignB;
        }
        else if (eSignB == SIGNINDICATOR.ZERO)
        {
            eResult = eSignA;
        }
        else
        {
            SIGNINDICATOR eSignC = ComputeDeterminantExactSign(
                ef[2] - ef[0],
                ef[3] - ef[1],
                m_xAC + (m_xA - ef[0]),
                m_yAC + (m_yA - ef[1]));

            SIGNINDICATOR eSignD = ComputeDeterminantExactSign(
                ef[2] - ef[0],
                ef[3] - ef[1],
                (m_xAC - m_xDC) + (m_xA - ef[0]),
                (m_yAC - m_yDC) + (m_yA - ef[1]));

            if (eSignC == eSignD)
            {
                eResult = eSignC;
            }
            else if (eSignC == SIGNINDICATOR.ZERO)
            {
                eResult = eSignD;
            }
            else if (eSignD == SIGNINDICATOR.ZERO)
            {
                eResult = eSignC;
            }
            else
            {
                // Try interval arithmetic.
                var oDetABDC = new CIntegralInterval(m_xAB, m_yAB, m_xDC, m_yDC);
                var oDetACDC = new CIntegralInterval(m_xAC, m_yAC, m_xDC, m_yDC);
                var oDetABFE = new CIntegralInterval(m_xAB, m_yAB, ef[2] - ef[0], ef[3] - ef[1]);
                var oDetAEFE = new CIntegralInterval(ef[0] - m_xA, ef[1] - m_yA, ef[2] - ef[0], ef[3] - ef[1]);
                SIGNINDICATOR eSignABDC = oDetABDC.GetSign();
                SIGNINDICATOR eSignABFE = oDetABFE.GetSign();
                COMPARISON eComparison = COMPARISON.UNDEFINED;

                if (eSignABDC != SIGNINDICATOR.ZERO && eSignABFE != SIGNINDICATOR.ZERO)
                {
                    if ((int)eSignABDC * (int)eSignABFE == 1)
                    {
                        oDetACDC.Multiply(oDetABFE);
                        oDetABDC.Multiply(oDetAEFE);
                        eComparison = oDetACDC.Compare(oDetABDC);
                    }
                    else
                    {
                        oDetABDC.Multiply(oDetAEFE);
                        oDetACDC.Multiply(oDetABFE);
                        eComparison = oDetABDC.Compare(oDetACDC);
                    }
                    if (eComparison != COMPARISON.UNDEFINED)
                    {
                        eResult = eComparison == COMPARISON.STRICTLYLESSTHAN ? eSignA :
                            (eComparison == COMPARISON.STRICTLYGREATERTHAN ? eSignB : SIGNINDICATOR.ZERO);
                    }
                }

                if (eComparison == COMPARISON.UNDEFINED)
                {
                    // Use exact integer arithmetic.
                    var z1 = new CZ192(m_xAB);
                    var z2 = new CZ192(m_yAB);
                    var z3 = new CZ192(m_xDC);
                    var z4 = new CZ192(m_yDC);
                    z1.Multiply(z4);
                    z2.Multiply(z3);
                    z1.Subtract(z2);
                    // z1 = Determinant(AB, DC)

                    var z5 = new CZ192(m_xAC);
                    var z6 = new CZ192(m_yAC);
                    z5.Multiply(z4);
                    z6.Multiply(z3);
                    z5.Subtract(z6);
                    // z5 = Determinant(AC, DC)

                    var zz1 = new CZ192(m_xAB);
                    var zz2 = new CZ192(m_yAB);
                    var zz3 = new CZ192(ef[2] - ef[0]);
                    var zz4 = new CZ192(ef[3] - ef[1]);
                    zz1.Multiply(zz4);
                    zz2.Multiply(zz3);
                    zz1.Subtract(zz2);
                    // zz1 = Determinant(AB, FE)

                    var zz5 = new CZ192(ef[0] - m_xA);
                    var zz6 = new CZ192(ef[1] - m_yA);
                    zz5.Multiply(zz4);
                    zz6.Multiply(zz3);
                    zz5.Subtract(zz6);
                    // zz5 = Determinant(AE, FE)

                    Debug.Assert((int)z1.GetSign() == -1 || (int)z1.GetSign() == 1);
                    Debug.Assert((int)zz1.GetSign() == -1 || (int)zz1.GetSign() == 1);

                    if ((int)z1.GetSign() * (int)zz1.GetSign() == 1)
                    {
                        z5.Multiply(zz1);
                        z1.Multiply(zz5);
                        eComparison = z5.Compare(z1);
                    }
                    else
                    {
                        z1.Multiply(zz5);
                        z5.Multiply(zz1);
                        eComparison = z1.Compare(z5);
                    }
                    Debug.Assert(eComparison != COMPARISON.UNDEFINED);
                    eResult = eComparison == COMPARISON.STRICTLYLESSTHAN ? eSignA :
                        (eComparison == COMPARISON.STRICTLYGREATERTHAN ? eSignB : SIGNINDICATOR.ZERO);
                }
            }
        }
        return (SIDEINDICATOR)(int)eResult;
    }

    internal void GetTransverseIntersectionYSpan(Span<double> y)
    {
        Debug.Assert(IntersectionIsTransverse());

        double yA = m_yA;
        double yB = yA + m_yAB;
        double yC = yA + m_yAC;
        double yD = yC - m_yDC;
        y[0] = Math.Max(Math.Min(yA, yB), Math.Min(yC, yD));
        y[1] = Math.Min(Math.Max(yA, yB), Math.Max(yC, yD));
    }

    internal static COMPARISON YXSortTransverseIntersectionPair(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efgh)
    {
        Debug.Assert(abcd.IntersectionIsTransverse() && efgh.IntersectionIsTransverse());

        COMPARISON eResult = COMPARISON.UNDEFINED;

        Span<double> v1 = stackalloc double[2];
        Span<double> v2 = stackalloc double[2];
        abcd.GetTransverseIntersectionYSpan(v1);
        efgh.GetTransverseIntersectionYSpan(v2);

        if (v1[1] < v2[0])
        {
            eResult = COMPARISON.STRICTLYLESSTHAN;
        }
        else if (v1[0] > v2[1])
        {
            eResult = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (abcd.IntersectionIsTransverseOnOpenSegments() &&
                 efgh.IntersectionIsTransverseOnOpenSegments())
        {
            if (abcd.DeterminantACDCIsExact() && abcd.DeterminantABDCIsExact() &&
                efgh.DeterminantACDCIsExact() && efgh.DeterminantABDCIsExact())
            {
                abcd.ComputeIntersectionPointYCoordinateInterval(out double rMin1, out double rMax1);
                efgh.ComputeIntersectionPointYCoordinateInterval(out double rMin2, out double rMax2);
                if (rMin1 > rMax2)
                {
                    eResult = COMPARISON.STRICTLYGREATERTHAN;
                }
                else if (rMax1 < rMin2)
                {
                    eResult = COMPARISON.STRICTLYLESSTHAN;
                }
            }
            if (eResult == COMPARISON.UNDEFINED)
            {
                eResult = YXSortTransverseIntersectionPairUsingIntervalArithmetic(abcd, efgh);
                if (eResult == COMPARISON.UNDEFINED)
                {
                    eResult = YXSortTransverseIntersectionPairUsingExactArithmetic(abcd, efgh);
                    Debug.Assert(eResult != COMPARISON.UNDEFINED);
                }
            }
        }
        else
        {
            eResult = YXSortSpecificPosition(abcd, efgh);
        }

        return eResult;
    }

    internal static COMPARISON YXSortTransverseIntersectionAndPoint(
        CLineSegmentIntersection abcd,
        Span<double> e)
    {
        Debug.Assert(abcd.IntersectionIsTransverse());

        COMPARISON eResult = COMPARISON.UNDEFINED;
        bool fXComparisonOnly = false;
        Span<double> v = stackalloc double[2];

        abcd.GetTransverseIntersectionYSpan(v);
        if (e[1] < v[0])
        {
            eResult = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (e[1] > v[1])
        {
            eResult = COMPARISON.STRICTLYLESSTHAN;
        }
        else if (abcd.DeterminantACDCIsExact() && abcd.DeterminantABDCIsExact())
        {
            double yLHS = abcd.DeterminantACDC() * abcd.m_yAB;
            double yRHS = abcd.DeterminantABDC() * (e[1] - abcd.m_yA);

            if (yLHS != yRHS)
            {
                if (abcd.SignDeterminantABDC() == SIGNINDICATOR.STRICTLY_POSITIVE)
                {
                    eResult = yLHS > yRHS ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                }
                else
                {
                    eResult = yLHS > yRHS ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.STRICTLYGREATERTHAN;
                }
            }
            else if (Math.Abs(abcd.DeterminantACDC()) <= IntegerConstants.LARGESTINTEGER26 &&
                     Math.Abs(abcd.m_yAB) <= IntegerConstants.LARGESTINTEGER26 &&
                     Math.Abs(abcd.DeterminantABDC()) <= IntegerConstants.LARGESTINTEGER26 &&
                     Math.Abs(e[1] - abcd.m_yA) <= IntegerConstants.LARGESTINTEGER26)
            {
                double xLHS = abcd.DeterminantACDC() * abcd.m_xAB;
                double xRHS = abcd.DeterminantABDC() * (e[0] - abcd.m_xA);
                fXComparisonOnly = true;
                if (xLHS != xRHS)
                {
                    if (abcd.SignDeterminantABDC() == SIGNINDICATOR.STRICTLY_POSITIVE)
                    {
                        eResult = xLHS > xRHS ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                    }
                    else
                    {
                        eResult = xLHS > xRHS ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.STRICTLYGREATERTHAN;
                    }
                }
                else if (Math.Abs(abcd.m_xAB) <= IntegerConstants.LARGESTINTEGER26 &&
                         Math.Abs(e[0] - abcd.m_xA) <= IntegerConstants.LARGESTINTEGER26)
                {
                    eResult = COMPARISON.EQUAL;
                }
            }
        }

        if (eResult == COMPARISON.UNDEFINED)
        {
            eResult = YXSortTransverseIntersectionAndPointUsingIntervalArithmetic(
                abcd, e, fXComparisonOnly);

            if (eResult == COMPARISON.UNDEFINED)
            {
                eResult = YXSortTransverseIntersectionAndPointUsingExactArithmetic(
                    abcd, e, fXComparisonOnly);
            }
        }
        return eResult;
    }

    internal static SIDEINDICATOR LocatePointRelativeToLine(
        ReadOnlySpan<double> c,
        ReadOnlySpan<double> ab)
    {
        Debug.Assert(ab[0] != ab[2] || ab[1] != ab[3]);

        SIDEINDICATOR eResult = SIDEINDICATOR.INCIDENT;

        double xAB = ab[2] - ab[0], yAB = ab[3] - ab[1];
        double xAC = c[0] - ab[0], yAC = c[1] - ab[1];
        if (DeterminantIsExactDouble(xAB, yAB, xAC, yAC))
        {
            double det = xAB * yAC - yAB * xAC;
            eResult = det > 0 ? SIDEINDICATOR.LEFT :
                     (det < 0 ? SIDEINDICATOR.RIGHT : SIDEINDICATOR.INCIDENT);
        }
        else
        {
            eResult = (SIDEINDICATOR)(int)ComputeDeterminantExactSign(xAB, yAB, xAC, yAC);
        }
        return eResult;
    }

    internal COMPARISON SortCDAlongAB()
    {
        if (m_eKind != KIND.NONTRANSVERSE || m_eSignABDC != SIGNINDICATOR.ZERO)
        {
            return COMPARISON.UNDEFINED;
        }

        double xAD = m_xAC - m_xDC;
        double yAD = m_yAC - m_yDC;

        if (m_xAC == xAD && m_yAC == yAD)
        {
            return COMPARISON.EQUAL;
        }

        if (m_xAB != 0)
        {
            Debug.Assert(m_xAC != xAD);
            if (m_xAB > 0)
            {
                return m_xAC > xAD ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
            }
            else
            {
                return m_xAC > xAD ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.STRICTLYGREATERTHAN;
            }
        }
        else
        {
            Debug.Assert(m_yAB != 0);
            Debug.Assert(m_yAC != yAD);
            if (m_yAB > 0)
            {
                return m_yAC > yAD ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
            }
            else
            {
                return m_yAC > yAD ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.STRICTLYGREATERTHAN;
            }
        }
    }

    internal static COMPARISON SortTransverseIntersectionsAlongCommonLineSegment(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efgh,
        PAIRING pairing)
    {
        Debug.Assert(abcd.IntersectionIsTransverse() && efgh.IntersectionIsTransverse());

        COMPARISON eResult = COMPARISON.UNDEFINED;

        switch (pairing)
        {
            case PAIRING.FIRST_FIRST:
                eResult = LambdaABSortTransverseIntersectionPair(abcd, efgh);
                break;

            case PAIRING.FIRST_LAST:
            {
                var swapped = new CLineSegmentIntersection();
                swapped.SetToSwappedTransverseIntersection(efgh);
                eResult = LambdaABSortTransverseIntersectionPair(abcd, swapped);
            }
            break;

            case PAIRING.LAST_FIRST:
            {
                var swapped = new CLineSegmentIntersection();
                swapped.SetToSwappedTransverseIntersection(efgh);
                eResult = LambdaCDSortTransverseIntersectionPair(abcd, swapped);
            }
            break;

            case PAIRING.LAST_LAST:
                eResult = LambdaCDSortTransverseIntersectionPair(abcd, efgh);
                break;
        }

        Debug.Assert(eResult != COMPARISON.UNDEFINED);
        return eResult;
    }

    internal static COMPARISON LambdaABSortTransverseIntersectionPair(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection abef)
    {
        Debug.Assert(abcd.IntersectionIsTransverse() && abef.IntersectionIsTransverse());

        COMPARISON eResult = COMPARISON.UNDEFINED;

        if (abcd.LambdaIsZero())
        {
            eResult = abef.LambdaIsZero() ? COMPARISON.EQUAL : COMPARISON.STRICTLYLESSTHAN;
        }
        else if (abcd.LambdaIsOne())
        {
            eResult = abef.LambdaIsOne() ? COMPARISON.EQUAL : COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (abef.LambdaIsZero())
        {
            eResult = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (abef.LambdaIsOne())
        {
            eResult = COMPARISON.STRICTLYLESSTHAN;
        }
        else if (abcd.DeterminantABDCIsExact() && abcd.DeterminantACDCIsExact() &&
                 abef.DeterminantABDCIsExact() && abef.DeterminantACDCIsExact())
        {
            Debug.Assert(abcd.DeterminantABDC() != 0.0 && abef.DeterminantABDC() != 0.0);

            double d1 = Math.Abs(abcd.DeterminantACDC());
            double d2 = Math.Abs(abcd.DeterminantABDC());
            double d3 = Math.Abs(abef.DeterminantACDC());
            double d4 = Math.Abs(abef.DeterminantABDC());
            double d1d4 = d1 * d4;
            double d2d3 = d2 * d3;

            if (d1 < IntegerConstants.LARGESTINTEGER26 && d2 < IntegerConstants.LARGESTINTEGER26 &&
                d3 < IntegerConstants.LARGESTINTEGER26 && d4 < IntegerConstants.LARGESTINTEGER26)
            {
                eResult = d1d4 > d2d3 ? COMPARISON.STRICTLYGREATERTHAN :
                         (d1d4 < d2d3 ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.EQUAL);
            }
            else
            {
                if (d1d4 != d2d3)
                {
                    eResult = d1d4 > d2d3 ? COMPARISON.STRICTLYGREATERTHAN :
                             (d1d4 < d2d3 ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.EQUAL);
                }
                else
                {
                    var z1 = new CZ128(d1);
                    var z2 = new CZ128(d2);
                    var z3 = new CZ128(d3);
                    var z4 = new CZ128(d4);
                    z1.Multiply(z4);
                    z3.Multiply(z2);
                    eResult = z1.Compare(z3);
                }
            }
        }
        else
        {
            eResult = LambdaABSortTransverseIntersectionPairUsingIntervalArithmetic(abcd, abef);

            if (eResult == COMPARISON.UNDEFINED)
            {
                eResult = LambdaABSortTransverseIntersectionPairUsingExactArithmetic(abcd, abef);
            }
        }

        Debug.Assert(eResult != COMPARISON.UNDEFINED);
        return eResult;
    }

    internal static COMPARISON LambdaCDSortTransverseIntersectionPair(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efcd)
    {
        Debug.Assert(abcd.IntersectionIsTransverse() && efcd.IntersectionIsTransverse());

        COMPARISON eResult = COMPARISON.UNDEFINED;

        if (abcd.MuIsZero())
        {
            eResult = efcd.MuIsZero() ? COMPARISON.EQUAL : COMPARISON.STRICTLYLESSTHAN;
        }
        else if (abcd.MuIsOne())
        {
            eResult = efcd.MuIsOne() ? COMPARISON.EQUAL : COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (efcd.MuIsZero())
        {
            eResult = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (efcd.MuIsOne())
        {
            eResult = COMPARISON.STRICTLYLESSTHAN;
        }
        else if (abcd.DeterminantABDCIsExact() && abcd.DeterminantABACIsExact() &&
                 efcd.DeterminantABDCIsExact() && efcd.DeterminantABACIsExact())
        {
            Debug.Assert(abcd.DeterminantABDC() != 0.0 && efcd.DeterminantABDC() != 0.0);

            double d1 = Math.Abs(abcd.DeterminantABAC());
            double d2 = Math.Abs(abcd.DeterminantABDC());
            double d3 = Math.Abs(efcd.DeterminantABAC());
            double d4 = Math.Abs(efcd.DeterminantABDC());
            double d1d4 = d1 * d4;
            double d2d3 = d2 * d3;

            if (d1 < IntegerConstants.LARGESTINTEGER26 && d2 < IntegerConstants.LARGESTINTEGER26 &&
                d3 < IntegerConstants.LARGESTINTEGER26 && d4 < IntegerConstants.LARGESTINTEGER26)
            {
                eResult = d1d4 > d2d3 ? COMPARISON.STRICTLYGREATERTHAN :
                         (d1d4 < d2d3 ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.EQUAL);
            }
            else
            {
                if (d1d4 != d2d3)
                {
                    eResult = d1d4 > d2d3 ? COMPARISON.STRICTLYGREATERTHAN :
                             (d1d4 < d2d3 ? COMPARISON.STRICTLYLESSTHAN : COMPARISON.EQUAL);
                }
                else
                {
                    var z1 = new CZ128(d1);
                    var z2 = new CZ128(d2);
                    var z3 = new CZ128(d3);
                    var z4 = new CZ128(d4);
                    z1.Multiply(z4);
                    z3.Multiply(z2);
                    eResult = z1.Compare(z3);
                }
            }
        }
        else
        {
            eResult = LambdaCDSortTransverseIntersectionPairUsingIntervalArithmetic(abcd, efcd);

            if (eResult == COMPARISON.UNDEFINED)
            {
                eResult = LambdaCDSortTransverseIntersectionPairUsingExactArithmetic(abcd, efcd);
            }
        }

        Debug.Assert(eResult != COMPARISON.UNDEFINED);
        return eResult;
    }

    // ===================== Private Methods =====================

    private void Reset()
    {
        m_eKind = KIND.UNDEFINED;
        m_eLocationAB = m_eLocationCD = LOCATION.UNDEFINED;
        m_fExactABDC = m_fExactACDC = m_fExactABAC = false;
        m_eSignABDC = m_eSignACDC = m_eSignABAC = SIGNINDICATOR.ZERO;
    }

    private COMPARISON CompareDeterminantABDCandDeterminantACDC()
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        if (m_fExactABDC && m_fExactACDC)
        {
            eResult = m_rDeterminantABDC < m_rDeterminantACDC ? COMPARISON.STRICTLYLESSTHAN :
                (m_rDeterminantABDC > m_rDeterminantACDC ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.EQUAL);
        }
        else
        {
            double dy = m_yAC - m_yAB;
            double dx = m_xAC - m_xAB;
            if ((m_xDC == 0 && dx == 0) || (dy == 0 && m_yDC == 0) || (dy == 0 && dx == 0))
            {
                eResult = COMPARISON.EQUAL;
            }
            else
            {
                double p1 = m_xDC * dy;
                double p2 = m_yDC * dx;
                if (p1 >= 0 && p2 <= 0)
                {
                    eResult = COMPARISON.STRICTLYGREATERTHAN;
                }
                else if (p1 <= 0 && p2 >= 0)
                {
                    eResult = COMPARISON.STRICTLYLESSTHAN;
                }
                else
                {
                    if (p1 != p2)
                    {
                        eResult = p1 > p2 ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                    }
                    else
                    {
                        var z1 = new CZ64(m_xDC);
                        var z2 = new CZ64(m_yDC);
                        var z3 = new CZ64(dx);
                        var z4 = new CZ64(dy);
                        z1.Multiply(z4);
                        z2.Multiply(z3);
                        return z1.Compare(z2);
                    }
                }
            }
        }
        return eResult;
    }

    private COMPARISON CompareDeterminantABDCandDeterminantABAC()
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        if (m_fExactABDC && m_fExactABAC)
        {
            eResult = m_rDeterminantABDC < m_rDeterminantABAC ? COMPARISON.STRICTLYLESSTHAN :
                (m_rDeterminantABDC > m_rDeterminantABAC ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.EQUAL);
        }
        else
        {
            double dy = m_yDC - m_yAC;
            double dx = m_xDC - m_xAC;
            if ((m_xAB == 0 && dx == 0) || (dy == 0 && m_yAB == 0) || (dy == 0 && dx == 0))
            {
                eResult = COMPARISON.EQUAL;
            }
            else
            {
                double p1 = m_xAB * dy;
                double p2 = m_yAB * dx;
                if (p1 >= 0 && p2 <= 0)
                {
                    eResult = COMPARISON.STRICTLYGREATERTHAN;
                }
                else if (p1 <= 0 && p2 >= 0)
                {
                    eResult = COMPARISON.STRICTLYLESSTHAN;
                }
                else
                {
                    if (p1 != p2)
                    {
                        eResult = p1 > p2 ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                    }
                    else
                    {
                        var z1 = new CZ64(m_xAB);
                        var z2 = new CZ64(m_yAB);
                        var z3 = new CZ64(dx);
                        var z4 = new CZ64(dy);
                        z1.Multiply(z4);
                        z2.Multiply(z3);
                        eResult = z1.Compare(z2);
                    }
                }
            }
        }
        return eResult;
    }

    private void ComputeIntersectionPointXCoordinateInterval(
        out double rMin, out double rMax)
    {
        Debug.Assert(DeterminantACDCIsExact() && DeterminantABDCIsExact());
        Debug.Assert(DeterminantABDC() != 0);

        double lambda = Math.Abs(DeterminantACDC() / DeterminantABDC());
        rMin = IntervalArithmeticUtils.PreviousDouble(lambda);
        rMax = IntervalArithmeticUtils.NextDouble(lambda);

        if (m_xAB > 0)
        {
            rMin = IntervalArithmeticUtils.PreviousDouble(IntervalArithmeticUtils.PreviousDouble(rMin * m_xAB) + m_xA);
            rMax = IntervalArithmeticUtils.NextDouble(IntervalArithmeticUtils.NextDouble(rMax * m_xAB) + m_xA);
        }
        else if (m_xAB == 0)
        {
            rMin = rMax = m_xA;
        }
        else
        {
            double rMinAux = rMin;
            rMin = IntervalArithmeticUtils.PreviousDouble(IntervalArithmeticUtils.PreviousDouble(rMax * m_xAB) + m_xA);
            rMax = IntervalArithmeticUtils.NextDouble(IntervalArithmeticUtils.NextDouble(rMinAux * m_xAB) + m_xA);
        }
    }

    private void ComputeIntersectionPointYCoordinateInterval(
        out double rMin, out double rMax)
    {
        Debug.Assert(DeterminantACDCIsExact() && DeterminantABDCIsExact());
        Debug.Assert(DeterminantABDC() != 0);

        double lambda = Math.Abs(DeterminantACDC() / DeterminantABDC());
        rMin = IntervalArithmeticUtils.PreviousDouble(lambda);
        rMax = IntervalArithmeticUtils.NextDouble(lambda);

        if (m_yAB > 0)
        {
            rMin = IntervalArithmeticUtils.PreviousDouble(IntervalArithmeticUtils.PreviousDouble(rMin * m_yAB) + m_yA);
            rMax = IntervalArithmeticUtils.NextDouble(IntervalArithmeticUtils.NextDouble(rMax * m_yAB) + m_yA);
        }
        else if (m_yAB == 0)
        {
            rMin = rMax = m_yA;
        }
        else
        {
            double rMinAux = rMin;
            rMin = IntervalArithmeticUtils.PreviousDouble(IntervalArithmeticUtils.PreviousDouble(rMax * m_yAB) + m_yA);
            rMax = IntervalArithmeticUtils.NextDouble(IntervalArithmeticUtils.NextDouble(rMinAux * m_yAB) + m_yA);
        }
    }

    private void SetToSwappedTransverseIntersection(CLineSegmentIntersection other)
    {
        Debug.Assert(other.IntersectionIsTransverse());

        m_xAB = -other.m_xDC;
        m_yAB = -other.m_yDC;
        m_xDC = -other.m_xAB;
        m_yDC = -other.m_yAB;
        m_xAC = -other.m_xAC;
        m_yAC = -other.m_yAC;
        m_xA = other.m_xA + other.m_xAC;
        m_yA = other.m_yA + other.m_yAC;

        m_rDeterminantABDC = -other.m_rDeterminantABDC;
        m_rDeterminantACDC = -other.m_rDeterminantABAC;
        m_rDeterminantABAC = -other.m_rDeterminantACDC;
        m_eSignABDC = (SIGNINDICATOR)(-(int)other.m_eSignABDC);
        m_eSignACDC = (SIGNINDICATOR)(-(int)other.m_eSignABAC);
        m_eSignABAC = (SIGNINDICATOR)(-(int)other.m_eSignACDC);
        m_fExactABDC = other.m_fExactABDC;
        m_fExactACDC = other.m_fExactABAC;
        m_fExactABAC = other.m_fExactACDC;

        m_eKind = other.m_eKind;
        m_eLocationAB = other.m_eLocationCD;
        m_eLocationCD = other.m_eLocationAB;
    }

    private bool GetTransverseIntersectionWhenNotOnOpenSegments(Span<double> p)
    {
        bool eResult = false;
        if (IntersectionIsTransverse())
        {
            eResult = true;
            if (m_eLocationAB == LOCATION.AT_FIRST_POINT)
            {
                p[0] = m_xA;
                p[1] = m_yA;
            }
            else if (m_eLocationAB == LOCATION.AT_LAST_POINT)
            {
                p[0] = m_xA + m_xAB;
                p[1] = m_yA + m_yAB;
            }
            else if (m_eLocationCD == LOCATION.AT_FIRST_POINT)
            {
                p[0] = m_xA + m_xAC;
                p[1] = m_yA + m_yAC;
            }
            else if (m_eLocationCD == LOCATION.AT_LAST_POINT)
            {
                p[0] = m_xA + m_xAC - m_xDC;
                p[1] = m_yA + m_yAC - m_yDC;
            }
            else
            {
                eResult = false;
            }
        }
        return eResult;
    }

    private static REGION ComputeRegionWhenPointPIsOnAB(double xAB, double yAB, double xAP, double yAP)
    {
        REGION eResult = REGION.UNDEFINED;
        if (xAB != 0)
        {
            if (xAB > 0)
            {
                if (xAP < 0)
                {
                    eResult = REGION.HALFLINE_BEFOREFIRST;
                }
                else if (xAP == 0)
                {
                    eResult = REGION.EQUAL_TO_FIRST;
                }
                else if (xAP < xAB)
                {
                    eResult = REGION.OPEN_LINESEGMENT;
                }
                else if (xAP == xAB)
                {
                    eResult = REGION.EQUAL_TO_LAST;
                }
                else
                {
                    eResult = REGION.HALFLINE_AFTERLAST;
                }
            }
            else
            {
                if (xAP > 0)
                {
                    eResult = REGION.HALFLINE_BEFOREFIRST;
                }
                else if (xAP == 0)
                {
                    eResult = REGION.EQUAL_TO_FIRST;
                }
                else if (xAP > xAB)
                {
                    eResult = REGION.OPEN_LINESEGMENT;
                }
                else if (xAP == xAB)
                {
                    eResult = REGION.EQUAL_TO_LAST;
                }
                else
                {
                    eResult = REGION.HALFLINE_AFTERLAST;
                }
            }
        }
        else
        {
            Debug.Assert(yAB != 0);
            if (yAB > 0)
            {
                if (yAP < 0)
                {
                    eResult = REGION.HALFLINE_BEFOREFIRST;
                }
                else if (yAP == 0)
                {
                    eResult = REGION.EQUAL_TO_FIRST;
                }
                else if (yAP < yAB)
                {
                    eResult = REGION.OPEN_LINESEGMENT;
                }
                else if (yAP == yAB)
                {
                    eResult = REGION.EQUAL_TO_LAST;
                }
                else
                {
                    eResult = REGION.HALFLINE_AFTERLAST;
                }
            }
            else
            {
                if (yAP > 0)
                {
                    eResult = REGION.HALFLINE_BEFOREFIRST;
                }
                else if (yAP == 0)
                {
                    eResult = REGION.EQUAL_TO_FIRST;
                }
                else if (yAP > yAB)
                {
                    eResult = REGION.OPEN_LINESEGMENT;
                }
                else if (yAP == yAB)
                {
                    eResult = REGION.EQUAL_TO_LAST;
                }
                else
                {
                    eResult = REGION.HALFLINE_AFTERLAST;
                }
            }
        }
        return eResult;
    }

    private static COMPARISON YXSortTransverseIntersectionPairUsingIntervalArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efgh)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CIntegralInterval(abcd.m_xAB, abcd.m_yAB, abcd.m_xDC, abcd.m_yDC);
        var z7 = new CIntegralInterval(abcd.m_yAB);
        var z5 = new CIntegralInterval(abcd.m_xAC, abcd.m_yAC, abcd.m_xDC, abcd.m_yDC);

        var zz1 = new CIntegralInterval(efgh.m_xAB, efgh.m_yAB, efgh.m_xDC, efgh.m_yDC);
        var zz7 = new CIntegralInterval(efgh.m_yAB);
        var zz5 = new CIntegralInterval(efgh.m_xAC, efgh.m_yAC, efgh.m_xDC, efgh.m_yDC);

        z5.Multiply(zz1);
        zz5.Multiply(z1);
        z1.Multiply(zz1);

        if (z1.GetSign() != SIGNINDICATOR.ZERO)
        {
            var yLHS = new CIntegralInterval(abcd.m_yA);
            var yRHS = new CIntegralInterval(efgh.m_yA);

            yLHS.Multiply(z1);
            z7.Multiply(z5);
            yLHS.Add(z7);

            yRHS.Multiply(z1);
            zz7.Multiply(zz5);
            yRHS.Add(zz7);

            if (z1.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
            {
                if ((eResult = yRHS.Compare(yLHS)) == COMPARISON.EQUAL)
                {
                    var z8 = new CIntegralInterval(abcd.m_xAB);
                    var xLHS = new CIntegralInterval(abcd.m_xA);
                    var zz8 = new CIntegralInterval(efgh.m_xAB);
                    var xRHS = new CIntegralInterval(efgh.m_xA);

                    xLHS.Multiply(z1);
                    z8.Multiply(z5);
                    xLHS.Add(z8);

                    xRHS.Multiply(z1);
                    zz8.Multiply(zz5);
                    xRHS.Add(zz8);

                    eResult = xRHS.Compare(xLHS);
                }
            }
            else
            {
                if ((eResult = yLHS.Compare(yRHS)) == COMPARISON.EQUAL)
                {
                    var z8 = new CIntegralInterval(abcd.m_xAB);
                    var xLHS = new CIntegralInterval(abcd.m_xA);
                    var zz8 = new CIntegralInterval(efgh.m_xAB);
                    var xRHS = new CIntegralInterval(efgh.m_xA);

                    xLHS.Multiply(z1);
                    z8.Multiply(z5);
                    xLHS.Add(z8);

                    xRHS.Multiply(z1);
                    zz8.Multiply(zz5);
                    xRHS.Add(zz8);

                    eResult = xLHS.Compare(xRHS);
                }
            }
        }
        return eResult;
    }

    private static COMPARISON YXSortTransverseIntersectionPairUsingExactArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efgh)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CZ192(abcd.m_xAB);
        var z2 = new CZ192(abcd.m_yAB);
        var z7 = new CZ192(abcd.m_yAB);
        var z3 = new CZ192(abcd.m_xDC);
        var z4 = new CZ192(abcd.m_yDC);
        var z5 = new CZ192(abcd.m_xAC);
        var z6 = new CZ192(abcd.m_yAC);

        var zz1 = new CZ192(efgh.m_xAB);
        var zz2 = new CZ192(efgh.m_yAB);
        var zz7 = new CZ192(efgh.m_yAB);
        var zz3 = new CZ192(efgh.m_xDC);
        var zz4 = new CZ192(efgh.m_yDC);
        var zz5 = new CZ192(efgh.m_xAC);
        var zz6 = new CZ192(efgh.m_yAC);

        var yLHS = new CZ192(abcd.m_yA);
        var yRHS = new CZ192(efgh.m_yA);

        z1.Multiply(z4);
        z2.Multiply(z3);
        z1.Subtract(z2);
        // z1 = abcd.DeterminantABDC()

        z5.Multiply(z4);
        z6.Multiply(z3);
        z5.Subtract(z6);
        // z5 = abcd.DeterminantACDC()

        zz1.Multiply(zz4);
        zz2.Multiply(zz3);
        zz1.Subtract(zz2);
        // zz1 = efgh.DeterminantABDC()

        zz5.Multiply(zz4);
        zz6.Multiply(zz3);
        zz5.Subtract(zz6);
        // zz5 = efgh.DeterminantACDC()

        z5.Multiply(zz1);
        zz5.Multiply(z1);
        z1.Multiply(zz1);
        Debug.Assert((int)z1.GetSign() == -1 || (int)z1.GetSign() == 1);

        z7.Multiply(z5);
        yLHS.Multiply(z1);
        yLHS.Add(z7);

        zz7.Multiply(zz5);
        yRHS.Multiply(z1);
        yRHS.Add(zz7);

        if ((int)z1.GetSign() == -1)
        {
            if ((eResult = yRHS.Compare(yLHS)) == COMPARISON.EQUAL)
            {
                var z8 = new CZ192(abcd.m_xAB);
                var xLHS = new CZ192(abcd.m_xA);
                var zz8 = new CZ192(efgh.m_xAB);
                var xRHS = new CZ192(efgh.m_xA);

                z8.Multiply(z5);
                xLHS.Multiply(z1);
                xLHS.Add(z8);

                zz8.Multiply(zz5);
                xRHS.Multiply(z1);
                xRHS.Add(zz8);

                eResult = xRHS.Compare(xLHS);
            }
        }
        else
        {
            if ((eResult = yLHS.Compare(yRHS)) == COMPARISON.EQUAL)
            {
                var z8 = new CZ192(abcd.m_xAB);
                var xLHS = new CZ192(abcd.m_xA);
                var zz8 = new CZ192(efgh.m_xAB);
                var xRHS = new CZ192(efgh.m_xA);

                z8.Multiply(z5);
                xLHS.Multiply(z1);
                xLHS.Add(z8);

                zz8.Multiply(zz5);
                xRHS.Multiply(z1);
                xRHS.Add(zz8);

                eResult = xLHS.Compare(xRHS);
            }
        }
        return eResult;
    }

    private static COMPARISON YXSortSpecificPosition(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efgh)
    {
        Debug.Assert(abcd.IntersectionIsTransverse() && efgh.IntersectionIsTransverse());
        Debug.Assert(!(abcd.IntersectionIsTransverseOnOpenSegments() && efgh.IntersectionIsTransverseOnOpenSegments()));

        COMPARISON eResult = COMPARISON.UNDEFINED;

        if (abcd.IntersectionIsTransverse() && efgh.IntersectionIsTransverse())
        {
            Span<double> p = stackalloc double[2];
            Span<double> q = stackalloc double[2];
            if (abcd.GetTransverseIntersectionWhenNotOnOpenSegments(p))
            {
                if (efgh.GetTransverseIntersectionWhenNotOnOpenSegments(q))
                {
                    eResult = YXComparePoints(p[0], p[1], q[0], q[1]);
                }
                else
                {
                    eResult = ComparisonHelper.Opposite(YXSortTransverseIntersectionAndPoint(efgh, p));
                }
            }
            else
            {
                if (efgh.GetTransverseIntersectionWhenNotOnOpenSegments(q))
                {
                    eResult = YXSortTransverseIntersectionAndPoint(abcd, q);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
        return eResult;
    }

    private static COMPARISON YXSortTransverseIntersectionAndPointUsingIntervalArithmetic(
        CLineSegmentIntersection abcd,
        Span<double> e,
        bool fXComparisonOnly)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;
        var z1 = new CIntegralInterval(abcd.m_xAB, abcd.m_yAB, abcd.m_xDC, abcd.m_yDC);

        if (z1.GetSign() != SIGNINDICATOR.ZERO)
        {
            var z5 = new CIntegralInterval(abcd.m_xAC, abcd.m_yAC, abcd.m_xDC, abcd.m_yDC);
            if (!fXComparisonOnly)
            {
                var yRHS = new CIntegralInterval(e[1] - abcd.m_yA);
                var yLHS = new CIntegralInterval(abcd.m_yAB);

                yRHS.Multiply(z1);
                yLHS.Multiply(z5);
                eResult = yLHS.Compare(yRHS);

                if (eResult == COMPARISON.STRICTLYLESSTHAN || eResult == COMPARISON.STRICTLYGREATERTHAN)
                {
                    if (z1.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
                    {
                        eResult = (eResult == COMPARISON.STRICTLYLESSTHAN) ?
                            COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                    }
                }
            }

            if (eResult == COMPARISON.EQUAL || fXComparisonOnly)
            {
                var xRHS = new CIntegralInterval(e[0] - abcd.m_xA);
                var xLHS = new CIntegralInterval(abcd.m_xAB);

                xRHS.Multiply(z1);
                xLHS.Multiply(z5);

                if (z1.GetSign() == SIGNINDICATOR.STRICTLY_POSITIVE)
                {
                    eResult = xLHS.Compare(xRHS);
                }
                else
                {
                    eResult = xRHS.Compare(xLHS);
                }
            }
        }
        return eResult;
    }

    private static COMPARISON YXSortTransverseIntersectionAndPointUsingExactArithmetic(
        CLineSegmentIntersection abcd,
        Span<double> e,
        bool fXComparisonOnly)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;
        var z1 = new CZ192(abcd.m_xAB);
        var z2 = new CZ192(abcd.m_yAB);
        var z3 = new CZ192(abcd.m_xDC);
        var z4 = new CZ192(abcd.m_yDC);
        var z5 = new CZ192(abcd.m_xAC);
        var z6 = new CZ192(abcd.m_yAC);

        z1.Multiply(z4);
        z2.Multiply(z3);
        z1.Subtract(z2);
        Debug.Assert(z1.GetSign() != SIGNINDICATOR.ZERO);

        z5.Multiply(z4);
        z6.Multiply(z3);
        z5.Subtract(z6);

        if (!fXComparisonOnly)
        {
            var yRHS = new CZ192(e[1] - abcd.m_yA);
            var yLHS = new CZ192(abcd.m_yAB);
            yRHS.Multiply(z1);
            yLHS.Multiply(z5);

            if ((eResult = yLHS.Compare(yRHS)) != COMPARISON.EQUAL)
            {
                if (z1.GetSign() != SIGNINDICATOR.STRICTLY_POSITIVE)
                {
                    eResult = (eResult == COMPARISON.STRICTLYLESSTHAN) ?
                        COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
                }
            }
        }

        if (eResult == COMPARISON.EQUAL || eResult == COMPARISON.UNDEFINED)
        {
            var xRHS = new CZ192(e[0] - abcd.m_xA);
            var xLHS = new CZ192(abcd.m_xAB);
            xRHS.Multiply(z1);
            xLHS.Multiply(z5);
            if (z1.GetSign() == SIGNINDICATOR.STRICTLY_POSITIVE)
            {
                eResult = xLHS.Compare(xRHS);
            }
            else
            {
                eResult = xRHS.Compare(xLHS);
            }
        }
        return eResult;
    }

    private static COMPARISON LambdaABSortTransverseIntersectionPairUsingIntervalArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection abef)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CIntegralInterval(abcd.m_xAC, abcd.m_yAC, abcd.m_xDC, abcd.m_yDC);
        var z2 = new CIntegralInterval(abcd.m_xAB, abcd.m_yAB, abcd.m_xDC, abcd.m_yDC);
        var z3 = new CIntegralInterval(abef.m_xAC, abef.m_yAC, abef.m_xDC, abef.m_yDC);
        var z4 = new CIntegralInterval(abef.m_xAB, abef.m_yAB, abef.m_xDC, abef.m_yDC);

        if (z2.GetSign() != SIGNINDICATOR.ZERO && z4.GetSign() != SIGNINDICATOR.ZERO)
        {
            z1.Multiply(z4);
            z3.Multiply(z2);
            eResult = z1.Compare(z3);

            if ((int)z2.GetSign() * (int)z4.GetSign() == (int)SIGNINDICATOR.STRICTLY_NEGATIVE)
            {
                eResult = ComparisonHelper.Opposite(eResult);
            }
        }
        return eResult;
    }

    private static COMPARISON LambdaABSortTransverseIntersectionPairUsingExactArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection abef)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CZ192(abcd.m_xAB);
        var z2 = new CZ192(abcd.m_yAB);
        var z3 = new CZ192(abcd.m_xDC);
        var z4 = new CZ192(abcd.m_yDC);
        var z5 = new CZ192(abcd.m_xAC);
        var z6 = new CZ192(abcd.m_yAC);
        var zz1 = new CZ192(abef.m_xAB);
        var zz2 = new CZ192(abef.m_yAB);
        var zz3 = new CZ192(abef.m_xDC);
        var zz4 = new CZ192(abef.m_yDC);
        var zz5 = new CZ192(abef.m_xAC);
        var zz6 = new CZ192(abef.m_yAC);

        // |abcd.DeterminantABDC()|
        z2.Multiply(z3);
        z1.Multiply(z4);
        z1.Subtract(z2);
        if (z1.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
        {
            z1.Negate();
        }

        // |abcd.DeterminantACDC()|
        z6.Multiply(z3);
        z5.Multiply(z4);
        z5.Subtract(z6);
        if (z5.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
        {
            z5.Negate();
        }

        // |abef.DeterminantABDC()|
        zz2.Multiply(zz3);
        zz1.Multiply(zz4);
        zz1.Subtract(zz2);
        if (zz1.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
        {
            zz1.Negate();
        }

        // |abef.DeterminantACDC()|
        zz6.Multiply(zz3);
        zz5.Multiply(zz4);
        zz5.Subtract(zz6);
        if (zz5.GetSign() == SIGNINDICATOR.STRICTLY_NEGATIVE)
        {
            zz5.Negate();
        }

        if (z1.Compare(zz1) == COMPARISON.STRICTLYLESSTHAN && z5.Compare(zz5) == COMPARISON.STRICTLYGREATERTHAN)
        {
            eResult = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if (z1.Compare(zz1) == COMPARISON.STRICTLYGREATERTHAN && z5.Compare(zz5) == COMPARISON.STRICTLYLESSTHAN)
        {
            eResult = COMPARISON.STRICTLYLESSTHAN;
        }
        else
        {
            z5.Multiply(zz1);
            z1.Multiply(zz5);
            eResult = z5.Compare(z1);
        }
        return eResult;
    }

    private static COMPARISON LambdaCDSortTransverseIntersectionPairUsingIntervalArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efcd)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CIntegralInterval(abcd.m_xAB, abcd.m_yAB, abcd.m_xAC, abcd.m_yAC);
        var z2 = new CIntegralInterval(abcd.m_xAB, abcd.m_yAB, abcd.m_xDC, abcd.m_yDC);
        var z3 = new CIntegralInterval(efcd.m_xAB, efcd.m_yAB, efcd.m_xAC, efcd.m_yAC);
        var z4 = new CIntegralInterval(efcd.m_xAB, efcd.m_yAB, efcd.m_xDC, efcd.m_yDC);

        if (z2.GetSign() != SIGNINDICATOR.ZERO && z4.GetSign() != SIGNINDICATOR.ZERO)
        {
            z1.Multiply(z4);
            z3.Multiply(z2);
            eResult = z1.Compare(z3);

            if ((int)z2.GetSign() * (int)z4.GetSign() == (int)SIGNINDICATOR.STRICTLY_NEGATIVE)
            {
                eResult = ComparisonHelper.Opposite(eResult);
            }
        }
        return eResult;
    }

    private static COMPARISON LambdaCDSortTransverseIntersectionPairUsingExactArithmetic(
        CLineSegmentIntersection abcd,
        CLineSegmentIntersection efcd)
    {
        COMPARISON eResult = COMPARISON.UNDEFINED;

        var z1 = new CZ192(abcd.m_xAB);
        var z2 = new CZ192(abcd.m_yAB);
        var z3 = new CZ192(abcd.m_xAC);
        var z4 = new CZ192(abcd.m_yAC);
        var z5 = new CZ192(abcd.m_xDC);
        var z6 = new CZ192(abcd.m_yDC);
        var z7 = new CZ192(efcd.m_xAB);
        var z8 = new CZ192(efcd.m_yAB);
        var z9 = new CZ192(efcd.m_xAC);
        var z10 = new CZ192(efcd.m_yAC);
        var z11 = new CZ192(efcd.m_xDC);
        var z12 = new CZ192(efcd.m_yDC);

        // z4 = abcd.DeterminantABAC() = yAC * xAB - xAC * yAB
        z3.Multiply(z2);
        z4.Multiply(z1);
        z4.Subtract(z3);

        // z6 = abcd.DeterminantABDC() = yDC * xAB - xDC * yAB
        z5.Multiply(z2);
        z6.Multiply(z1);
        z6.Subtract(z5);

        // z10 = efcd.DeterminantABAC()
        z9.Multiply(z8);
        z10.Multiply(z7);
        z10.Subtract(z9);

        // z12 = efcd.DeterminantABDC()
        z11.Multiply(z8);
        z12.Multiply(z7);
        z12.Subtract(z11);

        Debug.Assert(z6.GetSign() != SIGNINDICATOR.ZERO && z12.GetSign() != SIGNINDICATOR.ZERO);

        if ((int)z6.GetSign() * (int)z12.GetSign() == (int)SIGNINDICATOR.STRICTLY_NEGATIVE)
        {
            z4.Multiply(z12);
            z10.Multiply(z6);
            eResult = ComparisonHelper.Opposite(z4.Compare(z10));
        }
        else
        {
            z4.Multiply(z12);
            z10.Multiply(z6);
            eResult = z4.Compare(z10);
        }
        return eResult;
    }
}
