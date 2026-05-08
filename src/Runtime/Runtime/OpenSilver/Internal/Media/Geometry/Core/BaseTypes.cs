// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of BaseTypes.h

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

// Do not modify the member values.
internal enum SIGNINDICATOR
{
    STRICTLY_NEGATIVE = -1,
    ZERO = 0,
    STRICTLY_POSITIVE = 1,
}

// Do not modify the member values.
internal enum COMPARISON
{
    STRICTLYLESSTHAN = -1,
    EQUAL = 0,
    STRICTLYGREATERTHAN = 1,
    UNDEFINED = 255,
}

internal static class ComparisonHelper
{
    internal static COMPARISON Opposite(COMPARISON c)
    {
        Debug.Assert(c != COMPARISON.UNDEFINED);
        return (COMPARISON)(-(int)c);
    }
}

// Integer types are represented as double in C++ (IEEE 754 exact representation).
// In C# we use double directly; these constants define the valid ranges.
internal static class IntegerConstants
{
    internal const double LARGESTINTEGER26 = 0x4000000;       // 2^26
    internal const double LARGESTINTEGER30 = 0x40000000;      // 2^30
    internal const double LARGESTINTEGER31 = 0x80000000;      // 2^31
    internal const double LARGESTINTEGER33 = LARGESTINTEGER31 * 4.0; // 2^33
    internal const double LARGESTINTEGER53 = LARGESTINTEGER33 * 0x100000; // 2^53

    internal static bool IsValidInteger30(double v)
    {
        Debug.Assert(!double.IsNaN(v));
        return v == Math.Floor(v) && v <= LARGESTINTEGER30 && v >= -LARGESTINTEGER30;
    }

    internal static bool IsValidInteger31(double v)
    {
        Debug.Assert(!double.IsNaN(v));
        return v == Math.Floor(v) && v <= LARGESTINTEGER31 && v >= -LARGESTINTEGER31;
    }

    internal static bool IsValidInteger33(double v)
    {
        Debug.Assert(!double.IsNaN(v));
        return v == Math.Floor(v) && v <= LARGESTINTEGER33 && v >= -LARGESTINTEGER33;
    }

    internal static bool IsValidInteger53(double v)
    {
        Debug.Assert(!double.IsNaN(v));
        return v == Math.Floor(v) && v <= LARGESTINTEGER53 && v >= -LARGESTINTEGER53;
    }
}
