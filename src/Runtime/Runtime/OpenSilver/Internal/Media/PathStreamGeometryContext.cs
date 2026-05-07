
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media;

/// <summary>
///     PathStreamGeometryContext
/// </summary>
internal sealed class PathStreamGeometryContext : CapacityStreamGeometryContext
{
    static PathStreamGeometryContext()
    {
        // We grab the default values for these properties so that we can avoid setting 
        // properties to their default values (as this will require that we reserve 
        // storage for these values).

        var pathGeometryType = DependencyObjectType.FromSystemTypeInternal(typeof(PathGeometry));
        var pathFigureType = DependencyObjectType.FromSystemTypeInternal(typeof(PathFigure));
        var pathSegmentType = DependencyObjectType.FromSystemTypeInternal(typeof(PathSegment));
        var arcSegmentType = DependencyObjectType.FromSystemTypeInternal(typeof(ArcSegment));

        s_defaultFillRule = (FillRule)PathGeometry.FillRuleProperty.GetDefaultValue(pathGeometryType);

        s_defaultValueForPathFigureIsClosed = (bool)PathFigure.IsClosedProperty.GetDefaultValue(pathFigureType);
        s_defaultValueForPathFigureIsFilled = (bool)PathFigure.IsFilledProperty.GetDefaultValue(pathFigureType);
        s_defaultValueForPathFigureStartPoint = (Point)PathFigure.StartPointProperty.GetDefaultValue(pathFigureType);

        // This code assumes that sub-classes of PathSegment don't override the default value for these properties
        s_defaultValueForPathSegmentIsStroked = (bool)PathSegment.IsStrokedProperty.GetDefaultValue(pathSegmentType);
        s_defaultValueForPathSegmentIsSmoothJoin = (bool)PathSegment.IsSmoothJoinProperty.GetDefaultValue(pathSegmentType);

        s_defaultValueForArcSegmentIsLargeArc = (bool)ArcSegment.IsLargeArcProperty.GetDefaultValue(arcSegmentType);
        s_defaultValueForArcSegmentSweepDirection = (SweepDirection)ArcSegment.SweepDirectionProperty.GetDefaultValue(arcSegmentType);
        s_defaultValueForArcSegmentRotationAngle = (double)ArcSegment.RotationAngleProperty.GetDefaultValue(arcSegmentType);
    }

    internal PathStreamGeometryContext() { }

    internal PathStreamGeometryContext(FillRule fillRule, Transform transform)
    {
        _fillRule = fillRule;

        if (transform is not null && !transform.IsIdentity)
        {
            _transform = new MatrixTransform(transform.Matrix);
        }
    }

    internal override void SetFigureCount(int figureCount)
    {
        Debug.Assert(_figures == null, "It is illegal to call SetFigureCount multiple times or after BeginFigure.");
        Debug.Assert(figureCount > 0);

        _figures = new PathFigureCollection(figureCount);
    }

    internal override void SetSegmentCount(int segmentCount)
    {
        Debug.Assert(_figures != null, "It is illegal to call SetSegmentCount before BeginFigure.");
        Debug.Assert(_currentFigure != null, "It is illegal to call SetSegmentCount before BeginFigure.");
        Debug.Assert(_segments == null, "It is illegal to call SetSegmentCount multiple times per BeginFigure or after a *To method.");
        Debug.Assert(segmentCount > 0);

        _segments = new PathSegmentCollection(segmentCount);
        _currentFigure.Segments = _segments;
    }

    /// <summary>
    /// SetClosed - Sets the current closed state of the figure. 
    /// </summary>
    override internal void SetClosedState(bool isClosed)
    {
        Debug.Assert(_currentFigure != null);

        if (isClosed != _currentIsClosed)
        {
            _currentFigure.IsClosed = isClosed;
            _currentIsClosed = isClosed;
        }
    }

    /// <summary>
    /// BeginFigure - Start a new figure.
    /// </summary>
    public override void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
    {
        // _currentFigure != null -> _figures != null
        Debug.Assert(_currentFigure == null || _figures != null);

        // Is this the first figure?
        if (_currentFigure == null)
        {
            // If so, have we not yet allocated the collection?
            if (_figures == null)
            {
                // While we could always just retrieve _pathGeometry.Figures (which would auto-promote)
                // it's more efficient to create the collection ourselves and set it explicitly.

                _figures = new PathFigureCollection();
            }
        }

        FinishSegment();

        // Clear the old reference to the segment collection
        _segments = null;

        _currentFigure = new PathFigure();
        _currentIsClosed = isClosed;

        if (startPoint != s_defaultValueForPathFigureStartPoint)
        {
            _currentFigure.StartPoint = startPoint;
        }

        if (isClosed != s_defaultValueForPathFigureIsClosed)
        {
            _currentFigure.IsClosed = isClosed;
        }

        if (isFilled != s_defaultValueForPathFigureIsFilled)
        {
            _currentFigure.IsFilled = isFilled;
        }

        _figures.Add(_currentFigure);

        _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentNone;
    }

    /// <summary>
    /// LineTo - append a LineTo to the current figure.
    /// </summary>
    public override void LineTo(Point point, bool isStroked, bool isSmoothJoin)
    {
        PrepareToAddPoints(1 /*count*/,
                           isStroked,
                           isSmoothJoin,
                           MIL_SEGMENT_TYPE.MilSegmentPolyLine);

        _currentSegmentPoints.Add(point);
    }

    /// <summary>
    /// QuadraticBezierTo - append a QuadraticBezierTo to the current figure.
    /// </summary>
    public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
    {
        PrepareToAddPoints(2 /*count*/,
                           isStroked,
                           isSmoothJoin,
                           MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);

        _currentSegmentPoints.Add(point1);
        _currentSegmentPoints.Add(point2);
    }

    /// <summary>
    /// BezierTo - apply a BezierTo to the current figure.
    /// </summary>
    public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
    {
        PrepareToAddPoints(3 /*count*/,
                           isStroked,
                           isSmoothJoin,
                           MIL_SEGMENT_TYPE.MilSegmentPolyBezier);

        _currentSegmentPoints.Add(point1);
        _currentSegmentPoints.Add(point2);
        _currentSegmentPoints.Add(point3);
    }

    /// <summary>
    /// PolyLineTo - append a PolyLineTo to the current figure.
    /// </summary>
    public override void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      MIL_SEGMENT_TYPE.MilSegmentPolyLine);
    }

    /// <summary>
    /// PolyQuadraticBezierTo - append a PolyQuadraticBezierTo to the current figure.
    /// </summary>
    public override void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);
    }

    /// <summary>
    /// PolyBezierTo - append a PolyBezierTo to the current figure.
    /// </summary>
    public override void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
    {
        GenericPolyTo(points,
                      isStroked,
                      isSmoothJoin,
                      MIL_SEGMENT_TYPE.MilSegmentPolyBezier);
    }

    /// <summary>
    /// ArcTo - append an ArcTo to the current figure.
    /// </summary>
    public override void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
    {
        Debug.Assert(_figures != null);
        Debug.Assert(_currentFigure != null);

        FinishSegment();

        // Is this the first segment?
        if (_segments == null)
        {
            // While we could always just retrieve _currentFigure.Segments (which would auto-promote)
            // it's more efficient to create the collection ourselves and set it explicitly.

            _segments = new PathSegmentCollection();
            _currentFigure.Segments = _segments;
        }

        ArcSegment segment = new ArcSegment();

        segment.Point = point;
        segment.Size = size;

        if (isLargeArc != s_defaultValueForArcSegmentIsLargeArc)
        {
            segment.IsLargeArc = isLargeArc;
        }

        if (sweepDirection != s_defaultValueForArcSegmentSweepDirection)
        {
            segment.SweepDirection = sweepDirection;
        }

        if (rotationAngle != s_defaultValueForArcSegmentRotationAngle)
        {
            segment.RotationAngle = rotationAngle;
        }

        // Handle common PathSegment properties.
        if (isStroked != s_defaultValueForPathSegmentIsStroked)
        {
            segment.IsStroked = isStroked;
        }

        if (isSmoothJoin != s_defaultValueForPathSegmentIsSmoothJoin)
        {
            segment.IsSmoothJoin = isSmoothJoin;
        }

        _segments.Add(segment);

        _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentArc;
    }


    /// <summary>
    /// PathStreamGeometryContext is never opened, so it shouldn't be closed.
    /// </summary>
    public override void Close()
    {
        Debug.Assert(false);
    }

    internal PathGeometry GetPathGeometry()
    {
        var pathGeometry = new PathGeometry();

        if (_fillRule != s_defaultFillRule)
        {
            pathGeometry.FillRule = _fillRule;
        }

        if (_transform is not null)
        {
            pathGeometry.Transform = _transform;
        }

        pathGeometry.Figures = GetPathFigures();

        return pathGeometry;
    }

    internal PathFigureCollection GetPathFigures()
    {
        FinishSegment();

        Debug.Assert(_currentSegmentPoints == null);

        return _figures;
    }

    private void GenericPolyTo(IList<Point> points, bool isStroked, bool isSmoothJoin, MIL_SEGMENT_TYPE segmentType)
    {
        Debug.Assert(points != null);

        int count = points.Count;
        PrepareToAddPoints(count, isStroked, isSmoothJoin, segmentType);

        for (int i = 0; i < count; ++i)
        {
            _currentSegmentPoints.Add(points[i]);
        }
    }

    private void PrepareToAddPoints(int count, bool isStroked, bool isSmoothJoin, MIL_SEGMENT_TYPE segmentType)
    {
        Debug.Assert(_figures != null);
        Debug.Assert(_currentFigure != null);

        Debug.Assert(count != 0);

        if (_currentSegmentType != segmentType ||
            _currentSegmentIsStroked != isStroked ||
            _currentSegmentIsSmoothJoin != isSmoothJoin)
        {
            FinishSegment();

            _currentSegmentType = segmentType;
            _currentSegmentIsStroked = isStroked;
            _currentSegmentIsSmoothJoin = isSmoothJoin;
        }

        if (_currentSegmentPoints == null)
        {
            _currentSegmentPoints = new PointCollection();
        }
    }

    /// <summary>
    /// FinishSegment - called to completed any outstanding Segment which may be present.
    /// </summary>
    private void FinishSegment()
    {
        if (_currentSegmentPoints != null)
        {
            Debug.Assert(_currentFigure != null);

            int count = _currentSegmentPoints.Count;

            Debug.Assert(count > 0);

            // Is this the first segment?
            if (_segments == null)
            {
                // While we could always just retrieve _currentFigure.Segments (which would auto-promote)
                // it's more efficient to create the collection ourselves and set it explicitly.

                _segments = new PathSegmentCollection();
                _currentFigure.Segments = _segments;
            }

            PathSegment segment;

            switch (_currentSegmentType)
            {
                case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                    if (count == 1)
                    {
                        LineSegment lSegment = new LineSegment();
                        lSegment.Point = _currentSegmentPoints[0];
                        segment = lSegment;
                    }
                    else
                    {
                        PolyLineSegment pSegment = new PolyLineSegment();
                        pSegment.Points = _currentSegmentPoints;
                        segment = pSegment;
                    }
                    break;
                case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                    if (count == 3)
                    {
                        BezierSegment bSegment = new BezierSegment();
                        bSegment.Point1 = _currentSegmentPoints[0];
                        bSegment.Point2 = _currentSegmentPoints[1];
                        bSegment.Point3 = _currentSegmentPoints[2];
                        segment = bSegment;
                    }
                    else
                    {
                        Debug.Assert(count % 3 == 0);

                        PolyBezierSegment pSegment = new PolyBezierSegment();
                        pSegment.Points = _currentSegmentPoints;
                        segment = pSegment;
                    }
                    break;
                case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                    if (count == 2)
                    {
                        QuadraticBezierSegment qSegment = new QuadraticBezierSegment();
                        qSegment.Point1 = _currentSegmentPoints[0];
                        qSegment.Point2 = _currentSegmentPoints[1];
                        segment = qSegment;
                    }
                    else
                    {
                        Debug.Assert(count % 2 == 0);

                        PolyQuadraticBezierSegment pSegment = new PolyQuadraticBezierSegment();
                        pSegment.Points = _currentSegmentPoints;
                        segment = pSegment;
                    }
                    break;
                default:
                    segment = null;
                    Debug.Assert(false);
                    break;
            }

            // Handle common PathSegment properties.
            if (_currentSegmentIsStroked != s_defaultValueForPathSegmentIsStroked)
            {
                segment.IsStroked = _currentSegmentIsStroked;
            }

            if (_currentSegmentIsSmoothJoin != s_defaultValueForPathSegmentIsSmoothJoin)
            {
                segment.IsSmoothJoin = _currentSegmentIsSmoothJoin;
            }

            _segments.Add(segment);

            _currentSegmentPoints = null;
            _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentNone;
        }
    }

    private readonly FillRule _fillRule;
    private readonly Transform _transform;

    private PathFigureCollection _figures;
    private PathFigure _currentFigure;
    private PathSegmentCollection _segments;
    private bool _currentIsClosed;

    private MIL_SEGMENT_TYPE _currentSegmentType;
    private PointCollection _currentSegmentPoints;
    private bool _currentSegmentIsStroked;
    private bool _currentSegmentIsSmoothJoin;

    private static readonly FillRule s_defaultFillRule;

    private static readonly bool s_defaultValueForPathFigureIsClosed;
    private static readonly bool s_defaultValueForPathFigureIsFilled;
    private static readonly Point s_defaultValueForPathFigureStartPoint;

    // This code assumes that sub-classes of PathSegment don't override the default value for these properties
    private static readonly bool s_defaultValueForPathSegmentIsStroked;
    private static readonly bool s_defaultValueForPathSegmentIsSmoothJoin;

    private static readonly bool s_defaultValueForArcSegmentIsLargeArc;
    private static readonly SweepDirection s_defaultValueForArcSegmentSweepDirection;
    private static readonly double s_defaultValueForArcSegmentRotationAngle;
}
