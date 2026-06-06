// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of CRealFunction and CIncreasingFunction from utils.h / utils.cpp

using System;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal abstract class CRealFunction
{
    internal abstract void GetValueAndDerivative(
        double t,
        out double f,
        out double df);

    internal virtual bool SolveNewtonRaphson(
        double from,
        double to,
        double seed,
        double delta,
        double epsilon,
        out double root)
    {
        bool fTopClamped = false;
        bool fBottomClamped = false;
        double rAbs = 0;
        double correction;

        root = seed;

        for (int iter = 1; iter < 100; iter++)
        {
            GetValueAndDerivative(root, out double f, out double df);
            rAbs = Math.Abs(f);
            if (rAbs < epsilon)
            {
                break;
            }

            if (Math.Abs(df) <= rAbs * Utils.FUZZ)
            {
                break;
            }

            correction = -f / df;
            if (Math.Abs(correction) < delta)
            {
                break;
            }
            root += correction;

            if (root < from)
            {
                root = from;
                if (fBottomClamped)
                {
                    break;
                }
                fBottomClamped = true;
            }
            else if (root > to)
            {
                root = to;
                if (fTopClamped)
                {
                    break;
                }
                fTopClamped = true;
            }
        }

        return rAbs < epsilon;
    }
}

internal abstract class CIncreasingFunction : CRealFunction
{
    internal override bool SolveNewtonRaphson(
        double from,
        double to,
        double seed,
        double delta,
        double epsilon,
        out double root)
    {
        double top = to;
        double bottom = from;
        double rAbs = double.MaxValue;

        root = seed;

        for (int iter = 1; top - bottom > delta && iter < 100; iter++)
        {
            GetValueAndDerivative(root, out double f, out double df);
            rAbs = Math.Abs(f);
            if (rAbs < epsilon)
            {
                break;
            }

            if (f > 0)
            {
                top = root;
            }
            else
            {
                bottom = root;
            }

            if (Math.Abs(df) <= rAbs * Utils.FUZZ)
            {
                root = (bottom + top) / 2;
            }
            else
            {
                root -= f / df;

                if (root < from || root > to)
                {
                    root = (bottom + top) / 2;
                }
            }
        }

        return rAbs < epsilon;
    }
}
