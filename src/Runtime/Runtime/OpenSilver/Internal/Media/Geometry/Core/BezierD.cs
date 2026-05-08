// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of BezierD.h / BezierD.cpp

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal sealed class CBezier
{
    private MilPoint2D _ptB0;
    private MilPoint2D _ptB1;
    private MilPoint2D _ptB2;
    private MilPoint2D _ptB3;

    internal CBezier() { }

    internal CBezier(MilPoint2D pt0, MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3)
    {
        _ptB0 = pt0;
        _ptB1 = pt1;
        _ptB2 = pt2;
        _ptB3 = pt3;
    }

    internal CBezier(CBezier other)
    {
        Copy(other);
    }

    internal void Copy(CBezier other)
    {
        _ptB0 = other._ptB0;
        _ptB1 = other._ptB1;
        _ptB2 = other._ptB2;
        _ptB3 = other._ptB3;
    }

    internal void Initialize(in MilPoint2D ptFirst, MilPoint2D pt1, MilPoint2D pt2, MilPoint2D pt3)
    {
        _ptB0 = ptFirst;
        _ptB1 = pt1;
        _ptB2 = pt2;
        _ptB3 = pt3;
    }

    internal ref MilPoint2D ControlPoint0 => ref _ptB0;

    internal ref MilPoint2D ControlPoint1 => ref _ptB1;

    internal ref MilPoint2D ControlPoint2 => ref _ptB2;

    internal ref MilPoint2D ControlPoint3 => ref _ptB3;

    internal void GetPoint(double t, out MilPoint2D pt)
    {
        double s = 1 - t;
        double s2 = s * s;
        double t2 = t * t;

        pt = _ptB0 * (s * s2) + _ptB1 * (3 * s2 * t) +
             _ptB2 * (3 * s * t2) + _ptB3 * (t * t2);
    }

    internal void GetPointAndDerivatives(double t, Span<MilPoint2D> values)
    {
        Debug.Assert(values.Length >= 3);

        double s = 1 - t;
        double s2 = s * s;
        double t2 = t * t;
        double st = 2 * s * t;

        values[0] = _ptB0 * (s * s2) +
                    _ptB1 * (3 * s2 * t) +
                    _ptB2 * (3 * s * t2) +
                    _ptB3 * (t * t2);

        values[1] = (_ptB0 * (-s2) +
                     _ptB1 * (s2 - st) +
                     _ptB2 * (st - t2) +
                     _ptB3 * t2) * 3;

        values[2] = (_ptB0 * s +
                     _ptB1 * (t - 2 * s) +
                     _ptB2 * (s - 2 * t) +
                     _ptB3 * t) * 3;
    }

    internal void TrimToStartAt(double t)
    {
        Debug.Assert(t > 0 && t < 1);
        double s = 1 - t;

        _ptB0 = _ptB0 * s + _ptB1 * t;
        _ptB1 = _ptB1 * s + _ptB2 * t;
        _ptB2 = _ptB2 * s + _ptB3 * t;

        _ptB0 = _ptB0 * s + _ptB1 * t;
        _ptB1 = _ptB1 * s + _ptB2 * t;

        _ptB0 = _ptB0 * s + _ptB1 * t;
    }

    internal void TrimToEndAt(double t)
    {
        Debug.Assert(t > 0 && t < 1);
        double s = 1 - t;

        _ptB3 = _ptB2 * s + _ptB3 * t;
        _ptB2 = _ptB1 * s + _ptB2 * t;
        _ptB1 = _ptB0 * s + _ptB1 * t;

        _ptB3 = _ptB2 * s + _ptB3 * t;
        _ptB2 = _ptB1 * s + _ptB2 * t;

        _ptB3 = _ptB2 * s + _ptB3 * t;
    }

    internal bool TrimBetween(double rStart, double rEnd)
    {
        Debug.Assert(0 <= rStart);
        Debug.Assert(rStart <= rEnd);
        Debug.Assert(rEnd <= 1);

        if (rEnd - rStart < Utils.FUZZ)
        {
            GetPoint(rStart, out _ptB0);
            _ptB1 = _ptB2 = _ptB3 = _ptB0;
            return false;
        }

        if (rEnd < 1)
        {
            TrimToEndAt(rEnd);
        }

        if (rStart > 0)
        {
            TrimToStartAt(rStart / rEnd);
        }

        return true;
    }
}

internal struct CBezierFragment
{
    private CBezier _bezierNoRef;
    private double _start;
    private double _end;

    internal CBezierFragment(CBezier bezier, double startParameter, double endParameter)
    {
        _bezierNoRef = bezier;
        _start = startParameter;
        _end = endParameter;
    }

    internal void Clear() => _bezierNoRef = null;

    internal readonly bool Assigned() => _bezierNoRef != null;

    internal readonly double Start => _start;

    internal readonly double End => _end;

    internal readonly CBezier Bezier => _bezierNoRef;

    internal bool TryExtend(in CBezierFragment other, bool fAppend)
    {
        bool fExtended = false;

        if (Assigned() && _bezierNoRef == other._bezierNoRef)
        {
            if (fAppend)
            {
                if (_end == other._start)
                {
                    _end = other._end;
                    fExtended = true;
                }
            }
            else
            {
                if (_start == other._end)
                {
                    _start = other._start;
                    fExtended = true;
                }
            }
        }

        return fExtended;
    }

    internal readonly bool ConstructBezier(out CBezier bezier)
    {
        bezier = new CBezier();
        bezier.Copy(_bezierNoRef);
        return bezier.TrimBetween(_start, _end);
    }
}
