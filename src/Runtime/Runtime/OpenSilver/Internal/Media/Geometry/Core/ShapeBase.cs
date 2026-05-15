// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of ShapeBase.h / shapebase.cpp

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// The methods for processing shapes.
/// </summary>
internal abstract class CShapeBase
{
    internal CShapeBase() { }

    internal abstract bool HasGaps();

    internal abstract bool HasHollows();

    internal abstract bool IsEmpty();

    internal abstract int GetFigureCount();

    internal abstract IFigureData GetFigure(int index);

    internal abstract FillRule GetFillMode();

    internal abstract bool IsAxisAlignedRectangle();

    /// <summary>
    /// Populate a scanner with this shape's figure data.
    /// </summary>
    internal void Populate(IPopulationSink scanner, Matrix transform)
    {
        scanner.SetFillMode(GetFillMode());

        for (int i = 0; i < GetFigureCount(); i++)
        {
            IFigureData figure = GetFigure(i);
            if (figure.IsFillable())
            {
                var figureBase = new CFigureBase(figure);
                figureBase.Populate(scanner, transform);
            }
        }
    }

    /// <summary>
    /// Compute tight axis-aligned bounds for this shape.
    /// </summary>
    internal Rect GetTightBounds(Matrix transform, bool fSkipHollows = true)
    {
        var bounds = new CBounds();

        for (int i = 0; i < GetFigureCount(); i++)
        {
            IFigureData figure = GetFigure(i);

            if (fSkipHollows && !figure.IsFillable())
            {
                continue;
            }

            if (figure.IsEmpty())
            {
                continue;
            }

            MilPoint2D startPt = figure.GetStartPoint();
            var boundsTask = new CBoundsTask(bounds, startPt);
            boundsTask.TraverseForward(figure);
        }

        Rect rect = bounds.GetRect();

        if (!transform.IsIdentity)
        {
            rect = Rect.Transform(rect, transform);
        }

        return rect;
    }

    /// <summary>
    /// Performs a Boolean combination of two shapes and writes the result to a StreamGeometryContext.
    /// This is the C# port of WPF's CShapeBase::Combine.
    /// </summary>
    internal static void Combine(
        CShapeBase first,
        CShapeBase second,
        GeometryCombineMode operation,
        bool fRetrieveCurves,
        CapacityStreamGeometryContext context,
        Matrix firstTransform,
        Matrix secondTransform,
        double tolerance = Utils.DEFAULT_FLATTENING_TOLERANCE,
        bool fRelative = false)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(context);

        if (operation == GeometryCombineMode.Intersect &&
            first.IsAxisAlignedRectangle() &&
            second.IsAxisAlignedRectangle())
        {
            if (IntersectAxisAlignedRectangles(first, second, context, firstTransform, secondTransform))
            {
                return;
            }
        }

        // Compute tight bounds for both shapes and union them
        Rect bounds = first.GetTightBounds(firstTransform);
        bounds.Union(second.GetTightBounds(secondTransform));

        double rExtent = Math.Max(bounds.Width, bounds.Height);

        // Clamp and compute absolute tolerance if necessary
        if (fRelative)
        {
            tolerance = Math.Max(tolerance, Utils.FUZZ_DOUBLE) * rExtent;
        }
        else
        {
            tolerance = Math.Max(tolerance, rExtent * Utils.FUZZ_DOUBLE);
        }

        // Set up the boolean operation machinery
        var boolean = new CBoolean(context, operation, fRetrieveCurves, tolerance);
        bool fDegenerate = boolean.SetWorkspaceTransform(bounds);
        if (fDegenerate)
        {
            return; // Degenerate bounds
        }

        // Organize the first shape into chains
        first.Populate(boolean, firstTransform);

        // Mark boundary between shape 1 and shape 2
        boolean.SetNext();

        // Organize the second shape into chains
        second.Populate(boolean, secondTransform);

        // Scan the chains to obtain the result
        boolean.Scan();
    }

    /// <summary>
    /// Fast path for intersecting two axis-aligned rectangles.
    /// Returns true if the fast path was taken.
    /// If the transforms are not axis-aligned-preserving, returns false
    /// to fall through to the full sweep-line algorithm.
    /// </summary>
    private static bool IntersectAxisAlignedRectangles(
        CShapeBase first,
        CShapeBase second,
        CapacityStreamGeometryContext context,
        Matrix firstTransform,
        Matrix secondTransform)
    {
        Debug.Assert(first.IsAxisAlignedRectangle());
        Debug.Assert(second.IsAxisAlignedRectangle());

        if (firstTransform == secondTransform)
        {
            Rect rc1 = first.GetFigure(0).GetAsWellOrderedRectangle();
            Rect rc2 = second.GetFigure(0).GetAsWellOrderedRectangle();

            rc1.Intersect(rc2);

            if (rc1.Width > 0 && rc1.Height > 0)
            {
                if (firstTransform.IsIdentity)
                {
                    context.AddRect(rc1);
                }
                else if (IsAxisAlignedPreserving(firstTransform))
                {
                    context.AddRect(Rect.Transform(rc1, firstTransform));
                }
                else
                {
                    context.AddRect(rc1, firstTransform);
                }
            }

            return true;
        }

        if (IsAxisAlignedPreserving(firstTransform) && IsAxisAlignedPreserving(secondTransform))
        {
            Rect rc1 = first.GetFigure(0).GetAsWellOrderedRectangle();
            Rect rc2 = second.GetFigure(0).GetAsWellOrderedRectangle();

            if (!firstTransform.IsIdentity)
            {
                rc1 = Rect.Transform(rc1, firstTransform);
            }

            if (!secondTransform.IsIdentity)
            {
                rc2 = Rect.Transform(rc2, secondTransform);
            }

            rc1.Intersect(rc2);

            if (rc1.Width > 0 && rc1.Height > 0)
            {
                context.AddRect(rc1);
            }

            return true;
        }

        return false;
    }

    private static bool IsAxisAlignedPreserving(Matrix m) => (m.M12 == 0 && m.M21 == 0) || (m.M11 == 0 && m.M22 == 0);
}
