// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of PopulationSink.h

using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal interface IPopulationSink
{
    void StartFigure(in MilPoint2D pt);

    void AddLine(in MilPoint2D ptNew);

    void AddCurve(in MilPoint2D pt1, in MilPoint2D pt2, in MilPoint2D pt3);

    void SetCurrentVertexSmooth(bool val);

    void SetStrokeState(bool val);

    void EndFigure(bool fClosed);

    void SetFillMode(FillRule fillMode);
}
