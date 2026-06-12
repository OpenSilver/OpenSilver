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
    /// Produce the flattened version of this shape
    /// </summary>
    internal void FlattenToShape(
        double rTolerance,
        bool fRelative,
        CapacityStreamGeometryContext context,
        Matrix matrix = default)
    {
        double rAbsoluteTolerance = GetAbsoluteTolerance(rTolerance, fRelative, matrix);

        var sink = new CShapeFlattener(context, rAbsoluteTolerance);

        // Organize the shape into chains
        Populate(sink, matrix);
    }

    /// <summary>
    /// Build the outline of this shape
    /// </summary>
    internal void Outline(
        CapacityStreamGeometryContext context,
        double rTolerance = Utils.DEFAULT_FLATTENING_TOLERANCE,
        bool fRelative = false,
        Matrix matrix = default,
        bool fRetrieveCurves = true)
    {
        double rAbsoluteTolerance = GetAbsoluteTolerance(rTolerance, fRelative, matrix);

        var outline = new COutline(context, fRetrieveCurves, rAbsoluteTolerance);

        // Set scanner workspace
        Rect rect = GetTightBounds(matrix);
        bool fDegenerate = outline.SetWorkspaceTransform(rect);
        if (fDegenerate)
        {
            return;
        }

        // Organize the shape into chains
        Populate(outline, matrix);

        // Scan the chains to obtain the outline
        outline.Scan();
    }

    /// <summary>
    /// Find if a given point is in or near the fill of this shape
    /// </summary>
    internal void HitTestFill(
        MilPoint2D ptHit,
        double rThreshold,
        bool fRelative,
        Matrix matrix,
        out bool fHit,
        out bool fIsNear)
    {
        double rAbsoluteTolerance = GetAbsoluteTolerance(rThreshold, fRelative, Matrix.Identity);

        var tester = new CHitTest(ptHit, matrix, rAbsoluteTolerance);

        HitTestFiguresFill(tester);

        fHit = fIsNear = tester.WasAborted;

        if (!fHit)
        {
            if (GetFillMode() == FillRule.Nonzero)
            {
                fHit = tester.GetWindingNumber() != 0;
            }
            else
            {
                Debug.Assert(GetFillMode() == FillRule.EvenOdd);
                fHit = (tester.GetWindingNumber() & 1) != 0;
            }
        }
    }

    /// <summary>
    /// Hit test all figures fill with a hit-tester
    /// </summary>
    internal void HitTestFiguresFill(CHitTest tester)
    {
        // Traverse the figures to get the winding number at the hit point
        for (int i = 0; i < GetFigureCount(); i++)
        {
            if (GetFigure(i) is not IFigureData figure)
            {
                continue;
            }

            if (!figure.IsEmpty() && figure.IsFillable())
            {
                if (tester.StartAt(figure.GetStartPoint()))
                {
                    // We have a hit near the figure's start point
                    break;
                }

                tester.TraverseForward(figure);
                if (tester.WasAborted)
                {
                    // A hit was detected near this figure
                    break;
                }

                if (!figure.IsClosed() && tester.EndAt(figure.GetStartPoint()))
                {
                    // We have a hit near the figure's closing segment
                    break;
                }
            }
        }
    }

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

    /// <summary>
    /// Get the relation with another shape
    /// </summary>
    internal IntersectionDetail GetRelation(CShapeBase data, double rTolerance, bool fRelative)
    {
        Rect rcThis = GetTightBounds(Matrix.Identity);
        Rect rcOther = data.GetTightBounds(Matrix.Identity);

        if (rcThis.IntersectsWith(rcOther))
        {
            double rAbsoluteTolerance = GetAbsoluteTolerance(rTolerance, fRelative, Matrix.Identity);
            var relation = new CRelation(rAbsoluteTolerance);

            // Set scanner workspace
            rcThis.Union(rcOther);
            bool fDegenerate = relation.SetWorkspaceTransform(rcThis);
            if (fDegenerate)
            {
                // The bounding boxes intersect and are miniscule, so we assume the
                // geometries intersect.
                return IntersectionDetail.Intersects;
            }
            else
            {
                // Organize this shape into chains
                Populate(relation, Matrix.Identity);

                // Organize the other shape into chains
                relation.SetNext();
                data.Populate(relation, Matrix.Identity);

                // Scan the chains to obtain the result of the operation.
                relation.Scan();
                return relation.GetResult();
            }
        }
        else
        {
            // Bounding boxes do not overlap, the shapes are disjoint
            return IntersectionDetail.Empty;
        }
    }

    /// <summary>
    /// Compute area for this shape.
    /// </summary>
    internal double GetArea(double rTolerance, bool fRelative, Matrix matrix)
    {
        double result;

        if (IsAxisAlignedRectangle())
        {
            MilRectD rc = GetFigure(0).GetAsRectangle();

            result = Math.Abs((rc._right - rc._left) * (rc._bottom - rc._top));

            if (!matrix.IsIdentity)
            {
                result *= Math.Abs(matrix.Determinant);
            }
        }
        else
        {
            double rAbsoluteTolerance = GetAbsoluteTolerance(rTolerance, fRelative, matrix);

            var area = new CArea(rAbsoluteTolerance);

            // Set scanner workspace
            Rect rect = GetTightBounds(matrix);
            bool fDegenerate = area.SetWorkspaceTransform(rect);
            if (fDegenerate)
            {
                return 0;
            }

            // Organize the shape into chains
            Populate(area, matrix);

            // Scan the chains to obtain the area
            area.Scan();
            result = area.GetResult();
        }

        return result;
    }

    /// <summary>
    /// Get the absolute tolerance from a relative one.
    /// Port of CShapeBase::GetAbsoluteTolerance.
    /// </summary>
    private double GetAbsoluteTolerance(
        double rTolerance,
        bool fRelative,
        Matrix matrix)
    {
        Rect rcLooseBounds = GetTightBounds(matrix);

        double rBoundsWidth = rcLooseBounds.Width;
        double rBoundsHeight = rcLooseBounds.Height;

        if (double.IsNaN(rBoundsWidth) || double.IsNaN(rBoundsHeight))
        {
            return rTolerance;
        }

        double rExtent = Math.Max(rBoundsWidth, rBoundsHeight);

        if (fRelative)
        {
            return Math.Max(rTolerance, Utils.FUZZ_DOUBLE) * rExtent;
        }
        else
        {
            return Math.Max(rTolerance, rExtent * Utils.FUZZ_DOUBLE);
        }
    }

    private static bool IsAxisAlignedPreserving(Matrix m) => (m.M12 == 0 && m.M21 == 0) || (m.M11 == 0 && m.M22 == 0);
}
