// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of IntervalArithmetic.h

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Provides NextDouble and PreviousDouble utilities for interval arithmetic
/// using IEEE 754 bit manipulation.
/// </summary>
internal static class IntervalArithmeticUtils
{
    private const long SIGN_MASK = unchecked((long)0x8000000000000000);
    private const long NUMB_MASK = 0x7FFFFFFFFFFFFFFF;

    /// <summary>
    /// Returns the smallest double strictly larger than x.
    /// Works for Normals and Denormals but not NaN/QNaN.
    /// </summary>
    internal static double NextDouble(double x)
    {
        long u = BitConverter.DoubleToInt64Bits(x);

        if ((u & SIGN_MASK) != 0)
        {
            if ((u & NUMB_MASK) != 0)
            {
                u--;
            }
            else
            {
                u = 1;
            }
        }
        else
        {
            u++;
        }

        return BitConverter.Int64BitsToDouble(u);
    }

    /// <summary>
    /// Returns the largest double strictly smaller than x.
    /// Works for Normals and Denormals but not NaN/QNaN.
    /// </summary>
    internal static double PreviousDouble(double x)
    {
        long u = BitConverter.DoubleToInt64Bits(x);

        if ((u & SIGN_MASK) == 0)
        {
            if ((u & NUMB_MASK) != 0)
            {
                u--;
            }
            else
            {
                u = SIGN_MASK | 1;
            }
        }
        else
        {
            u++;
        }

        return BitConverter.Int64BitsToDouble(u);
    }
}

/// <summary>
/// An integral interval is an interval whose bounds are integers
/// represented by double precision floats. The interval is considered
/// as closed.
/// </summary>
internal struct CIntegralInterval
{
    private double m_l;
    private double m_h;

    /// <summary>Default constructor. Initializes to [0, 0].</summary>
    public CIntegralInterval()
    {
        m_l = 0;
        m_h = 0;
    }

    /// <summary>Initializes this interval to the closed interval [v, v].</summary>
    internal CIntegralInterval(double v)
    {
        m_l = v;
        m_h = v;
        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Initializes this interval to an interval containing the
    /// value of the determinant a*d - b*c.
    /// </summary>
    internal CIntegralInterval(double a, double b, double c, double d)
    {
        m_l = a * d;
        m_h = m_l;
        double l2 = b * c;
        double h2 = l2;

        if (Math.Abs(m_l) > IntegerConstants.LARGESTINTEGER53)
        {
            m_l = IntervalArithmeticUtils.PreviousDouble(m_l);
            m_h = IntervalArithmeticUtils.NextDouble(m_h);
        }
        if (Math.Abs(l2) > IntegerConstants.LARGESTINTEGER53)
        {
            l2 = IntervalArithmeticUtils.PreviousDouble(l2);
            h2 = IntervalArithmeticUtils.NextDouble(h2);
        }

        m_l -= h2;
        m_h -= l2;

        if (Math.Abs(m_l) > IntegerConstants.LARGESTINTEGER53)
        {
            m_l = IntervalArithmeticUtils.PreviousDouble(m_l);
        }
        if (Math.Abs(m_h) > IntegerConstants.LARGESTINTEGER53)
        {
            m_h = IntervalArithmeticUtils.NextDouble(m_h);
        }
        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Gets the sign of this interval.
    /// Returns SI_STRICTLY_NEGATIVE iff both bounds are strictly negative,
    /// SI_ZERO iff the (closed) interval contains 0,
    /// and SI_STRICTLY_POSITIVE otherwise.
    /// </summary>
    internal readonly SIGNINDICATOR GetSign()
    {
        Debug.Assert(IsValid());
        return m_h < 0
            ? SIGNINDICATOR.STRICTLY_NEGATIVE
            : (m_l > 0 ? SIGNINDICATOR.STRICTLY_POSITIVE : SIGNINDICATOR.ZERO);
    }

    /// <summary>
    /// Compares this interval with the argument and returns the result.
    /// This interval is the left-hand side term in the comparison,
    /// that is, Compare returns (*this ? other).
    /// </summary>
    internal readonly COMPARISON Compare(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());
        COMPARISON result = COMPARISON.UNDEFINED;
        if (Intersects(in other))
        {
            if (Equals(in other) && m_h == m_l)
            {
                result = COMPARISON.EQUAL;
            }
        }
        else
        {
            result = m_h < other.m_l
                ? COMPARISON.STRICTLYLESSTHAN
                : COMPARISON.STRICTLYGREATERTHAN;
        }
        return result;
    }

    /// <summary>Adds the argument to this interval.</summary>
    internal void Add(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());

        m_l += other.m_l;
        m_h += other.m_h;

        if (Math.Abs(m_l) > IntegerConstants.LARGESTINTEGER53)
        {
            m_l = IntervalArithmeticUtils.PreviousDouble(m_l);
        }
        if (Math.Abs(m_h) > IntegerConstants.LARGESTINTEGER53)
        {
            m_h = IntervalArithmeticUtils.NextDouble(m_h);
        }
        Debug.Assert(IsValid());
    }

    /// <summary>Subtracts the argument from this interval.</summary>
    internal void Subtract(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());

        m_l -= other.m_h;
        m_h -= other.m_l;

        if (Math.Abs(m_l) > IntegerConstants.LARGESTINTEGER53)
        {
            m_l = IntervalArithmeticUtils.PreviousDouble(m_l);
        }
        if (Math.Abs(m_h) > IntegerConstants.LARGESTINTEGER53)
        {
            m_h = IntervalArithmeticUtils.NextDouble(m_h);
        }
        Debug.Assert(IsValid());
    }

    /// <summary>Multiplies this interval by the argument.</summary>
    internal void Multiply(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());

        if (IsZero() || other.IsZero())
        {
            m_l = 0;
            m_h = 0;
        }
        else
        {
            if (m_l >= 0)
            {
                Debug.Assert(m_h > 0);
                if (other.m_l >= 0)
                {
                    Debug.Assert(other.m_h > 0);
                    m_l *= other.m_l;
                    m_h *= other.m_h;
                }
                else if (other.m_h <= 0)
                {
                    Debug.Assert(other.m_l < 0);
                    double temp = m_h * other.m_l;
                    m_h = m_l * other.m_h;
                    m_l = temp;
                }
                else
                {
                    Debug.Assert(other.m_l < 0 && other.m_h > 0);
                    m_l = m_h * other.m_l;
                    m_h *= other.m_h;
                }
            }
            else if (m_h <= 0)
            {
                Debug.Assert(m_l < 0);
                if (other.m_l >= 0)
                {
                    Debug.Assert(other.m_h > 0);
                    m_l *= other.m_h;
                    m_h *= other.m_l;
                }
                else if (other.m_h <= 0)
                {
                    Debug.Assert(other.m_l < 0);
                    double temp = m_h * other.m_h;
                    m_h = m_l * other.m_l;
                    m_l = temp;
                }
                else
                {
                    Debug.Assert(other.m_l < 0 && other.m_h > 0);
                    m_h = m_l * other.m_l;
                    m_l *= other.m_h;
                }
            }
            else
            {
                Debug.Assert(m_l < 0 && m_h > 0);
                if (other.m_l >= 0)
                {
                    Debug.Assert(other.m_h > 0);
                    m_l *= other.m_h;
                    m_h *= other.m_h;
                }
                else if (other.m_h <= 0)
                {
                    Debug.Assert(other.m_l < 0);
                    double temp = m_h * other.m_l;
                    m_h = m_l * other.m_l;
                    m_l = temp;
                }
                else
                {
                    // Both intervals contain zero and are not equal to [0, 0].
                    Debug.Assert(other.m_l < 0 && other.m_h > 0);

                    double minmin = m_l * other.m_l;
                    double minmax = m_l * other.m_h;
                    double maxmin = m_h * other.m_l;
                    double maxmax = m_h * other.m_h;
                    m_l = Math.Min(minmax, maxmin);
                    m_h = Math.Max(minmin, maxmax);
                }
            }

            if (Math.Abs(m_l) > IntegerConstants.LARGESTINTEGER53)
            {
                m_l = IntervalArithmeticUtils.PreviousDouble(m_l);
            }
            if (Math.Abs(m_h) > IntegerConstants.LARGESTINTEGER53)
            {
                m_h = IntervalArithmeticUtils.NextDouble(m_h);
            }
        }
        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Validity check. Returns true when valid.
    /// A valid interval has finite integer bounds with lo &lt;= hi.
    /// </summary>
    internal readonly bool IsValid()
    {
        return !double.IsNaN(m_l) && !double.IsInfinity(m_l)
            && !double.IsNaN(m_h) && !double.IsInfinity(m_h)
            && m_l == Math.Floor(m_l) && m_h == Math.Floor(m_h)
            && m_l <= m_h;
    }

    /// <summary>
    /// Equality test. Returns true when this interval is equal to the argument.
    /// </summary>
    internal readonly bool Equals(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());
        return m_l == other.m_l && m_h == other.m_h;
    }

    /// <summary>
    /// Intersection test. Returns true when this interval intersects the argument.
    /// </summary>
    internal readonly bool Intersects(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());
        return other.m_l <= m_h && other.m_h >= m_l;
    }

    /// <summary>
    /// Inclusion test. Returns true when this interval contains the argument interval.
    /// </summary>
    internal readonly bool Contains(in CIntegralInterval other)
    {
        Debug.Assert(IsValid() && other.IsValid());
        return m_l <= other.m_l && m_h >= other.m_h;
    }

    /// <summary>
    /// Inclusion test. Returns true when this interval contains the value v.
    /// </summary>
    internal readonly bool Contains(double v)
    {
        Debug.Assert(IsValid());
        return m_l <= v && m_h >= v;
    }

    /// <summary>
    /// Test for equality with [0, 0]. Returns true when this interval equals [0, 0].
    /// </summary>
    internal readonly bool IsZero()
    {
        Debug.Assert(IsValid());
        return m_l == 0.0 && m_h == 0.0;
    }
}
