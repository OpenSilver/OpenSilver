// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of BezierFlattener.h / bezierflattener.cpp

using System;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal abstract class CFlatteningSink
{
    internal virtual void Begin(in MilPoint2D pt) { }

    internal virtual void AcceptPoint(in MilPoint2D pt, double t, out bool fAborted) => fAborted = false;

    internal virtual void AcceptPointAndTangent(in MilPoint2D pt, in MilPoint2D tangent, bool fLast) { }
}

internal struct CBezierFlattener
{
    private CFlatteningSink _sink;
    private double _tolerance;
    private bool _withTangents;
    private double _quarterTolerance;
    private double _fuzz;

    private MilPoint2D _ptE0;
    private MilPoint2D _ptE1;
    private MilPoint2D _ptE2;
    private MilPoint2D _ptE3;

    internal MilPoint2D Point0;
    internal MilPoint2D Point1;
    internal MilPoint2D Point2;
    internal MilPoint2D Point3;

    private int _cSteps;
    private double _parameter;
    private double _stepSize;

    internal CBezierFlattener(CFlatteningSink sink, double tolerance)
    {
        Initialize(sink, tolerance);
    }

    internal void SetTarget(CFlatteningSink sink) => _sink = sink;

    internal void Initialize(CFlatteningSink sink, double tolerance)
    {
        _sink = sink;

        _tolerance = tolerance >= 0.0 ? tolerance : 0.0;
        _fuzz = tolerance * tolerance * Utils.SQ_LENGTH_FUZZ;

        _tolerance *= 6;
        _quarterTolerance = _tolerance * 0.25;
    }

    internal readonly bool GetFirstTangent(out MilPoint2D vecTangent)
    {
        vecTangent = Point1 - Point0;
        if (vecTangent * vecTangent > _fuzz)
        {
            return true;
        }

        vecTangent = Point2 - Point0;
        if (vecTangent * vecTangent > _fuzz)
        {
            return true;
        }

        vecTangent = Point3 - Point0;
        return vecTangent * vecTangent > _fuzz;
    }

    internal readonly MilPoint2D GetLastTangent()
    {
        MilPoint2D vecTangent = Point3 - Point2;

        double rLastTangentFuzz = _fuzz / 8.0;

        if (vecTangent * vecTangent <= rLastTangentFuzz)
        {
            vecTangent = Point3 - Point1;
            if (vecTangent * vecTangent <= rLastTangentFuzz)
            {
                vecTangent = Point3 - Point0;
            }
        }

        return vecTangent;
    }

    internal void Flatten(bool fWithTangents)
    {
        if (_sink is null)
        {
            throw new InvalidOperationException();
        }

        _withTangents = fWithTangents;

        _cSteps = 1;
        _parameter = 0;
        _stepSize = 1;

        _ptE0 = Point0;
        _ptE1 = Point3 - Point0;
        _ptE2 = (Point1 - Point2 * 2 + Point3) * 6;
        _ptE3 = (Point0 - Point1 * 2 + Point2) * 6;

        _cSteps = 1;
        while ((_ptE2.ApproxNorm() > _tolerance || _ptE3.ApproxNorm() > _tolerance) && _stepSize > Utils.TWICE_MIN_BEZIER_STEP_SIZE)
        {
            HalveTheStep();
        }

        while (_cSteps > 1)
        {
            Step(out bool fAbort);
            if (fAbort)
            {
                return;
            }

            if (_ptE2.ApproxNorm() > _tolerance && _stepSize > Utils.TWICE_MIN_BEZIER_STEP_SIZE)
            {
                HalveTheStep();
            }
            else
            {
                while (TryDoubleTheStep())
                {
                    continue;
                }
            }
        }

        if (_withTangents)
        {
            _sink.AcceptPointAndTangent(Point3, GetLastTangent(), true);
        }
        else
        {
            _sink.AcceptPoint(Point3, 1, out _);
        }
    }

    private void Step(out bool fAbort)
    {
        fAbort = false;

        MilPoint2D pt;

        _ptE0 = _ptE0 + _ptE1;
        pt = _ptE2;
        _ptE1 = _ptE1 + pt;
        _ptE2 = _ptE2 + pt - _ptE3;
        _ptE3 = pt;

        _parameter += _stepSize;

        if (_withTangents)
        {
            pt = _ptE1 * 6 - _ptE2 - _ptE3 * 2;
            _sink.AcceptPointAndTangent(_ptE0, pt, false);
        }
        else
        {
            _sink.AcceptPoint(_ptE0, _parameter, out fAbort);
        }

        _cSteps--;
    }

    private void HalveTheStep()
    {
        _ptE2 = (_ptE2 + _ptE3) * 0.125;
        _ptE1 = (_ptE1 - _ptE2) * 0.5;
        _ptE3 = _ptE3 * 0.25;

        _cSteps *= 2;
        _stepSize *= 0.5;
    }

    private bool TryDoubleTheStep()
    {
        bool fDoubled = (_cSteps & 1) == 0;
        if (fDoubled)
        {
            MilPoint2D ptTemp = _ptE2 * 2 - _ptE3;

            fDoubled = _ptE3.ApproxNorm() <= _quarterTolerance && ptTemp.ApproxNorm() <= _quarterTolerance;

            if (fDoubled)
            {
                _ptE1 = _ptE1 * 2 + _ptE2;
                _ptE3 = _ptE3 * 4;
                _ptE2 = ptTemp * 4;

                _cSteps /= 2;
                _stepSize *= 2;
            }
        }

        return fDoubled;
    }
}
