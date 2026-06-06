
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

using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using OpenSilver.Internal.Media.Geometry.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace System.Windows.Media;

/// <summary>
/// Represents a complex shape that may be composed of arcs, curves, ellipses, lines,
/// and rectangles.
/// </summary>
[ContentProperty(nameof(Figures))]
public sealed partial class PathGeometry : Geometry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathGeometry"/> class.
    /// </summary>
    public PathGeometry() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathGeometry"/> class with the specified <see cref="Figures"/>.
    /// </summary>
    /// <param name="figures">
    /// The <see cref="Figures"/> of the <see cref="PathGeometry"/> which describes the contents of the <see cref="Path"/>.
    /// </param>
    public PathGeometry(IEnumerable<PathFigure> figures)
    {
        ArgumentNullException.ThrowIfNull(figures);

        PathFigureCollection items = Figures;
        foreach (PathFigure figure in figures)
        {
            items.Add(figure);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathGeometry"/> class with the specified <see cref="Figures"/>, 
    /// <see cref="FillRule"/>, and <see cref="Geometry.Transform"/>.
    /// </summary>
    /// <param name="figures">
    /// The <see cref="Figures"/> of the <see cref="PathGeometry"/> which describes the contents of the <see cref="Path"/>.
    /// </param>
    /// <param name="fillRule">
    /// The <see cref="FillRule"/> of the <see cref="PathGeometry"/>.
    /// </param>
    /// <param name="transform">
    /// The <see cref="Geometry.Transform"/> which specifies the transform applied.
    /// </param>
    public PathGeometry(IEnumerable<PathFigure> figures, FillRule fillRule, Transform transform)
    {
        ArgumentNullException.ThrowIfNull(figures);

        FillRule = fillRule;
        Transform = transform;

        PathFigureCollection items = Figures;
        foreach (PathFigure figure in figures)
        {
            items.Add(figure);
        }
    }

    /// <summary>
    /// Identifies the <see cref="Figures"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FiguresProperty =
        DependencyProperty.Register(
            nameof(Figures),
            typeof(PathFigureCollection),
            typeof(PathGeometry),
            new PropertyMetadata(
                new PFCDefaultValueFactory<PathFigure>(
                    static () => new PathFigureCollection(),
                    static (d, dp) =>
                    {
                        PathGeometry pathGeometry = (PathGeometry)d;
                        var figures = new PathFigureCollection();
                        pathGeometry.ProvideSelfAsInheritanceContext(figures, null);
                        figures.SetParentGeometry(pathGeometry);
                        return figures;
                    }),
                OnFiguresChanged,
                CoerceFigures));

    /// <summary>
    /// Gets or sets the collection of <see cref="PathFigure"/> objects that describe
    /// the contents of a path.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="PathFigure"/> objects that describe the contents
    /// of a path. Each individual <see cref="PathFigure"/> describes a shape.
    /// </returns>
    public PathFigureCollection Figures
    {
        get => (PathFigureCollection)GetValue(FiguresProperty);
        set => SetValueInternal(FiguresProperty, value);
    }

    private static void OnFiguresChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PathGeometry pathGeometry = (PathGeometry)d;
        if (e.OldValue is PathFigureCollection oldFigures)
        {
            oldFigures.SetParentGeometry(null);
        }
        if (e.NewValue is PathFigureCollection newFigures)
        {
            newFigures.SetParentGeometry(pathGeometry);
        }

        OnPathChanged(d, e);
    }

    private static object CoerceFigures(DependencyObject d, object baseValue)
    {
        return baseValue ?? new PathFigureCollection();
    }

    /// <summary>
    /// Identifies the <see cref="FillRule"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FillRuleProperty =
        DependencyProperty.Register(
            nameof(FillRule),
            typeof(FillRule),
            typeof(PathGeometry),
            new PropertyMetadata(FillRule.EvenOdd, OnFillRuleChanged),
            ValidateEnums.IsFillRuleValid);

    /// <summary>
    /// Gets or sets a value that determines how the intersecting areas contained in
    /// the <see cref="PathGeometry"/> are combined.
    /// </summary>
    /// <returns>
    /// A <see cref="FillRule"/> enumeration value that indicates how the intersecting
    /// areas of the <see cref="PathGeometry"/> are combined. The default is <see cref="FillRule.EvenOdd"/>.
    /// </returns>
    public FillRule FillRule
    {
        get => (FillRule)GetValue(FillRuleProperty);
        set => SetValueInternal(FillRuleProperty, value);
    }

    /// <summary>
    /// Determines whether this <see cref="PathGeometry"/> object is empty.
    /// </summary>
    /// <returns>
    /// true if this <see cref="PathGeometry"/> is empty; otherwise, false.
    /// </returns>
    public override bool IsEmpty() => Figures.Count == 0;

    /// <summary>
    /// Determines whether this <see cref="PathGeometry"/> object may have curved segments.
    /// </summary>
    /// <returns>
    /// true if this <see cref="PathGeometry"/> object may have curved segments; otherwise, false.
    /// </returns>
    public override bool MayHaveCurves()
    {
        foreach (PathFigure figure in Figures.InternalItems)
        {
            if (figure.MayHaveCurves())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the <see cref="Point"/> and a tangent vector on this <see cref="PathGeometry"/> at the 
    /// specified fraction of its length.
    /// </summary>
    /// <param name="progress">
    /// The fraction of the length of this <see cref="PathGeometry"/>.
    /// </param>
    /// <param name="point">
    /// When this method returns, contains the location on this <see cref="PathGeometry"/> at the 
    /// specified fraction of its length. This parameter is passed uninitialized.
    /// </param>
    /// <param name="tangent">
    /// When this method returns, contains the tangent vector. This parameter is passed uninitialized.
    /// </param>
    public void GetPointAtFractionLength(double progress, out Point point, out Point tangent)
    {
        GetPointAtLengthFraction(GetPathGeometryData(), progress, out point, out tangent);
    }

    internal static void GetPointAtLengthFraction(PathGeometryData pathData, double fraction, out Point point, out Point tangent)
    {
        if (pathData.IsEmpty())
        {
            point = new Point();
            tangent = new Point();
            return;
        }

        var pathGeometry = new PathGeometryWrapper(pathData.SerializedData, pathData.FillRule, pathData.Matrix);

        var animationPath = new CAnimationPath();
        animationPath.SetUp(pathGeometry);

        animationPath.GetPointAtLengthFraction(
            fraction,
            out MilPoint2D ptD,
            out MilPoint2D vecTangentD);

        point = new Point(ptD.X, ptD.Y);
        tangent = new Point(vecTangentD.X, vecTangentD.Y);
    }

    /// <summary>
    /// Returns a string representation of this <see cref="PathGeometry"/>.
    /// </summary>
    /// <returns>
    /// A string representation of this <see cref="PathGeometry"/>.
    /// </returns>
    public override string ToString()
    {
        string figuresString = ToPathData(CultureInfo.InvariantCulture);
        if (FillRule != FillRule.EvenOdd)
        {
            figuresString = $"F1{figuresString}";
        }
        return figuresString;
    }

    internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
    {
        List<PathFigure> figures = Figures.InternalItems;

        if (figures.Count == 0)
        {
            return;
        }

        Matrix matrix = GetCombinedMatrix(transform);

        foreach (var figure in figures)
        {
            var segments = figure.Segments.InternalItems;

            if (segments.Count == 0)
            {
                continue;
            }

            Point current = figure.StartPoint;
            Point startPoint = current * matrix;

            context.BeginFigure(startPoint, figure.IsFilled, figure.IsClosed);

            foreach (var segment in segments)
            {
                segment.SerializeData(context, matrix, ref current);
            }
        }
    }

    internal override FillRule GetFillRule() => FillRule;

    /// <summary>
    /// Static method which parses a PathGeometryData and makes calls into the provided context sink.
    /// This can be used to build a PathGeometry, for readback, etc.
    /// </summary>
    internal static void ParsePathGeometryData(PathGeometryData pathData, Matrix transform, CapacityStreamGeometryContext ctx)
    {
        var parser = new PathGeometryDataParser(pathData, transform, ctx);
        parser.Parse();
    }

    private struct PathGeometryDataParser
    {
        private readonly PathGeometryData _pathData;
        private readonly Matrix _transform;
        private readonly CapacityStreamGeometryContext _context;

        private Point _current;

        public PathGeometryDataParser(PathGeometryData pathData, Matrix transform, CapacityStreamGeometryContext context)
        {
            _pathData = pathData;
            _transform = transform;
            _context = context;
        }

        public void Parse()
        {
            if (_pathData.IsEmpty())
            {
                return;
            }

            int currentOffset = 0;

            byte[] pbData = _pathData.SerializedData;

            // This assert is a logical correctness test
            Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_PATHGEOMETRY>());

            // ... while this assert tests "physical" correctness (i.e. are we running out of buffer).
            Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_PATHGEOMETRY>());

            MIL_PATHGEOMETRY pPathGeometry = MemoryMarshal.Read<MIL_PATHGEOMETRY>(pbData);

            // Move the current offset to after the Path's data
            currentOffset += Unsafe.SizeOf<MIL_PATHGEOMETRY>();

            // Are there any Figures to add?
            if (pPathGeometry.FigureCount > 0)
            {
                // Allocate the correct number of Figures up front
                _context.SetFigureCount((int)pPathGeometry.FigureCount);

                // ... and iterate on the Figures.
                for (int i = 0; i < pPathGeometry.FigureCount; i++)
                {
                    // We only expect well-formed data, but we should assert that we're not reading
                    // too much data.
                    Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_PATHFIGURE>());

                    MIL_PATHFIGURE pPathFigure = MemoryMarshal.Read<MIL_PATHFIGURE>(pbData.AsSpan(currentOffset));

                    // Move the current offset to the after of the Figure's data
                    currentOffset += Unsafe.SizeOf<MIL_PATHFIGURE>();

                    BeginFigure(pPathFigure.StartPoint,
                                (pPathFigure.Flags & MilPathFigureFlags.IsFillable) != 0,
                                (pPathFigure.Flags & MilPathFigureFlags.IsClosed) != 0);

                    if (pPathFigure.Count > 0)
                    {
                        // Allocate the correct number of Segments up front
                        _context.SetSegmentCount((int)pPathFigure.Count);

                        // ... and iterate on the Segments.
                        for (int j = 0; j < pPathFigure.Count; j++)
                        {
                            // We only expect well-formed data, but we should assert that we're not reading too much data.
                            Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT>());
                            Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT>());

                            MIL_SEGMENT pSegment = MemoryMarshal.Read<MIL_SEGMENT>(pbData.AsSpan(currentOffset));

                            switch (pSegment.Type)
                            {
                                case MIL_SEGMENT_TYPE.MilSegmentLine:
                                    {
                                        // We only expect well-formed data, but we should assert that we're not reading too much data.
                                        Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_LINE>());
                                        Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_LINE>());

                                        MIL_SEGMENT_LINE pSegmentLine = MemoryMarshal.Read<MIL_SEGMENT_LINE>(pbData.AsSpan(currentOffset));

                                        LineTo(pSegmentLine.Point,
                                               (pSegmentLine.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                               (pSegmentLine.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);

                                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_LINE>();
                                    }
                                    break;
                                case MIL_SEGMENT_TYPE.MilSegmentBezier:
                                    {
                                        // We only expect well-formed data, but we should assert that we're not reading too much data.
                                        Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_BEZIER>());
                                        Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_BEZIER>());

                                        MIL_SEGMENT_BEZIER pSegmentBezier = MemoryMarshal.Read<MIL_SEGMENT_BEZIER>(pbData.AsSpan(currentOffset));

                                        BezierTo(pSegmentBezier.Point1,
                                                 pSegmentBezier.Point2,
                                                 pSegmentBezier.Point3,
                                                 (pSegmentBezier.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                                 (pSegmentBezier.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);

                                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_BEZIER>();
                                    }
                                    break;
                                case MIL_SEGMENT_TYPE.MilSegmentQuadraticBezier:
                                    {
                                        // We only expect well-formed data, but we should assert that we're not reading too much data.
                                        Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_QUADRATICBEZIER>());
                                        Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_QUADRATICBEZIER>());

                                        MIL_SEGMENT_QUADRATICBEZIER pSegmentQuadraticBezier = MemoryMarshal.Read<MIL_SEGMENT_QUADRATICBEZIER>(pbData.AsSpan(currentOffset));

                                        QuadraticBezierTo(pSegmentQuadraticBezier.Point1,
                                                          pSegmentQuadraticBezier.Point2,
                                                          (pSegmentQuadraticBezier.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                                          (pSegmentQuadraticBezier.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);

                                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_QUADRATICBEZIER>();
                                    }
                                    break;
                                case MIL_SEGMENT_TYPE.MilSegmentArc:
                                    {
                                        // We only expect well-formed data, but we should assert that we're not reading too much data.
                                        Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_ARC>());
                                        Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_ARC>());

                                        MIL_SEGMENT_ARC pSegmentArc = MemoryMarshal.Read<MIL_SEGMENT_ARC>(pbData.AsSpan(currentOffset));

                                        ArcTo(pSegmentArc.Point,
                                              pSegmentArc.Size,
                                              pSegmentArc.XRotation,
                                              (pSegmentArc.LargeArc != 0),
                                              (pSegmentArc.Sweep == 0) ? SweepDirection.Counterclockwise : SweepDirection.Clockwise,
                                              (pSegmentArc.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                              (pSegmentArc.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);

                                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_ARC>();
                                    }
                                    break;
                                case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                                case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                                case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                                    {
                                        // We only expect well-formed data, but we should assert that we're not reading too much data.
                                        Debug.Assert(_pathData.SerializedData.Length >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>());
                                        Debug.Assert(_pathData.Size >= currentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>());

                                        MIL_SEGMENT_POLY pSegmentPoly = MemoryMarshal.Read<MIL_SEGMENT_POLY>(pbData.AsSpan(currentOffset));

                                        Debug.Assert(pSegmentPoly.Count <= int.MaxValue);

                                        if (pSegmentPoly.Count > 0)
                                        {
                                            List<Point> points = new List<Point>((int)pSegmentPoly.Count);

                                            // We only expect well-formed data, but we should assert that we're not reading too much data.
                                            Debug.Assert(_pathData.SerializedData.Length >=
                                                         currentOffset +
                                                         Unsafe.SizeOf<MIL_SEGMENT_POLY>() +
                                                         (int)pSegmentPoly.Count * Unsafe.SizeOf<Point>());
                                            Debug.Assert(_pathData.Size >=
                                                         currentOffset +
                                                         Unsafe.SizeOf<MIL_SEGMENT_POLY>() +
                                                         (int)pSegmentPoly.Count * Unsafe.SizeOf<Point>());

                                            for (int k = 0; k < pSegmentPoly.Count; k++)
                                            {
                                                Point pPoint = MemoryMarshal.Read<Point>(
                                                    pbData.AsSpan(currentOffset + Unsafe.SizeOf<MIL_SEGMENT_POLY>() + k * Unsafe.SizeOf<Point>()));

                                                points.Add(pPoint);
                                            }

                                            switch (pSegment.Type)
                                            {
                                                case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                                                    PolyLineTo(points,
                                                               (pSegmentPoly.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                                               (pSegmentPoly.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);
                                                    break;
                                                case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                                                    PolyBezierTo(points,
                                                                 (pSegmentPoly.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                                                 (pSegmentPoly.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);
                                                    break;
                                                case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                                                    PolyQuadraticBezierTo(points,
                                                                          (pSegmentPoly.Flags & MILCoreSegFlags.SegIsAGap) == 0,
                                                                          (pSegmentPoly.Flags & MILCoreSegFlags.SegSmoothJoin) != 0);
                                                    break;
                                            }
                                        }

                                        currentOffset += Unsafe.SizeOf<MIL_SEGMENT_POLY>() + (int)pSegmentPoly.Count * Unsafe.SizeOf<Point>();
                                    }
                                    break;
#if DEBUG
                            case MIL_SEGMENT_TYPE.MilSegmentNone:
                                throw new InvalidOperationException();
                            default:
                                throw new InvalidOperationException();
#endif
                            }
                        }
                    }
                }
            }
        }

        private void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
        {
            _current = startPoint;

            if (!_transform.IsIdentity)
            {
                startPoint *= _transform;
            }

            _context.BeginFigure(startPoint, isFilled, isClosed);
        }

        private void LineTo(Point point, bool isStroked, bool isSmoothJoin)
        {
            _current = point;

            if (!_transform.IsIdentity)
            {
                point *= _transform;
            }

            _context.LineTo(point, isStroked, isSmoothJoin);
        }

        private void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
        {
            _current = point3;

            if (!_transform.IsIdentity)
            {
                point1 *= _transform;
                point2 *= _transform;
                point3 *= _transform;
            }

            _context.BezierTo(point1, point2, point3, isStroked, isSmoothJoin);
        }

        private void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
        {
            _current = point2;

            if (!_transform.IsIdentity)
            {
                point1 *= _transform;
                point2 *= _transform;
            }

            _context.QuadraticBezierTo(point1, point2, isStroked, isSmoothJoin);
        }

        private void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            if (_transform.IsIdentity)
            {
                _context.ArcTo(point, size, rotationAngle, isLargeArc, sweepDirection, isStroked, isSmoothJoin);
            }
            else
            {
                ArcSegment.ArcToBezierHelper(
                    point,
                    size, rotationAngle,
                    isLargeArc,
                    sweepDirection,
                    isStroked,
                    isSmoothJoin,
                    _current,
                    _transform,
                    _context);
            }

            _current = point;
        }

        private void PolyLineTo(List<Point> points, bool isStroked, bool isSmoothJoin)
        {
            Debug.Assert(points.Count > 0);

            int count = points.Count;

            _current = points[count - 1];

            if (!_transform.IsIdentity)
            {
                for (int i = 0; i < count; i++)
                {
                    points[i] *= _transform;
                }
            }

            _context.PolyLineTo(points, isStroked, isSmoothJoin);
        }

        private void PolyBezierTo(List<Point> points, bool isStroked, bool isSmoothJoin)
        {
            Debug.Assert(points.Count > 0);

            int count = points.Count;

            if (count >= 3)
            {
                _current = points[count - 1 - (count % 3)];
            }

            if (!_transform.IsIdentity)
            {
                for (int i = 0; i < count; i++)
                {
                    points[i] *= _transform;
                }
            }

            _context.PolyBezierTo(points, isStroked, isSmoothJoin);
        }

        private void PolyQuadraticBezierTo(List<Point> points, bool isStroked, bool isSmoothJoin)
        {
            Debug.Assert(points.Count > 0);

            int count = points.Count;

            if (count >= 2)
            {
                _current = points[count - 1 - (count % 2)];
            }

            if (!_transform.IsIdentity)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] *= _transform;
                }
            }

            _context.PolyQuadraticBezierTo(points, isStroked, isSmoothJoin);
        }
    }
}
