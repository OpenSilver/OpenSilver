
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
using System.Windows.Markup;

namespace System.Windows.Media;

/// <summary>
/// Represents a subsection of a geometry, a single connected series of two-dimensional
/// geometric segments.
/// </summary>
[ContentProperty(nameof(Segments))]
public sealed class PathFigure : DependencyObject
{
    private Geometry _parentGeometry;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigure"/> class.
    /// </summary>
    public PathFigure() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigure"/> class with the specified <see cref="StartPoint"/>,
    /// <see cref="Segments"/>, and <see cref="IsClosed"/> values.
    /// </summary>
    /// <param name="start">
    /// The <see cref="StartPoint"/> for the <see cref="PathFigure"/>.
    /// </param>
    /// <param name="segments">
    /// The <see cref="Segments"/> for the <see cref="PathFigure"/>.
    /// </param>
    /// <param name="closed">
    /// The <see cref="IsClosed"/> for the <see cref="PathFigure"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// segments is null.
    /// </exception>
    public PathFigure(Point start, IEnumerable<PathSegment> segments, bool closed)
    {
        ArgumentNullException.ThrowIfNull(segments);

        StartPoint = start;
        IsClosed = closed;

        PathSegmentCollection collection = Segments;
        foreach (PathSegment item in segments)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Identifies the <see cref="IsClosed"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsClosedProperty =
        DependencyProperty.Register(
            nameof(IsClosed),
            typeof(bool),
            typeof(PathFigure),
            new PropertyMetadata(false, PropertyChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether this figure's first and last segments
    /// are connected.
    /// </summary>
    /// <returns>
    /// true if the first and last segments of the figure are connected; otherwise, false.
    /// The default is false.
    /// </returns>
    public bool IsClosed
    {
        get => (bool)GetValue(IsClosedProperty);
        set => SetValueInternal(IsClosedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsFilled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsFilledProperty =
        DependencyProperty.Register(
            nameof(IsFilled),
            typeof(bool),
            typeof(PathFigure),
            new PropertyMetadata(false, PropertyChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether the contained area of this <see cref="PathFigure"/>
    /// is to be used for hit-testing, rendering, and clipping.
    /// </summary>
    /// <returns>
    /// true if the contained area of this <see cref="PathFigure"/> is to be used
    /// for hit-testing, rendering, and clipping; otherwise, false. The default is true.
    /// </returns>
    public bool IsFilled
    {
        get => (bool)GetValue(IsFilledProperty);
        set => SetValueInternal(IsFilledProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Segments" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register(
            nameof(Segments),
            typeof(PathSegmentCollection),
            typeof(PathFigure),
            new PropertyMetadata(
                new PFCDefaultValueFactory<PathSegment>(
                    static () => new PathSegmentCollection(),
                    static (d, dp) =>
                    {
                        PathFigure figure = (PathFigure)d;
                        var segments = new PathSegmentCollection();
                        figure.ProvideSelfAsInheritanceContext(segments, null);
                        segments.SetParentGeometry(figure._parentGeometry);
                        return segments;
                    }),
                OnSegmentsChanged,
                CoerceSegments));

    /// <summary>
    /// Gets or sets the collection of segments that define the shape of this <see cref="PathFigure"/>
    /// object.
    /// </summary>
    /// <returns>
    /// The collection of segments that define the shape of this <see cref="PathFigure"/>
    /// object. The default is an empty collection.
    /// </returns>
    public PathSegmentCollection Segments
    {
        get => (PathSegmentCollection)GetValue(SegmentsProperty);
        set => SetValueInternal(SegmentsProperty, value);
    }

    private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PathFigure figure = (PathFigure)d;
        if (e.OldValue is PathSegmentCollection oldSegments)
        {
            oldSegments.SetParentGeometry(null);
        }
        if (e.NewValue is PathSegmentCollection newSegments)
        {
            newSegments.SetParentGeometry(figure._parentGeometry);
        }

        PropertyChanged(d, e);
    }

    private static object CoerceSegments(DependencyObject d, object baseValue)
    {
        return baseValue ?? new PathSegmentCollection();
    }

    /// <summary>
    /// Identifies the <see cref="StartPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartPointProperty =
        DependencyProperty.Register(
            nameof(StartPoint),
            typeof(Point),
            typeof(PathFigure),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the <see cref="Point"/> where the <see cref="PathFigure"/> begins.
    /// </summary>
    /// <returns>
    /// The <see cref="Point"/> where the <see cref="PathFigure"/> begins. The
    /// default is a <see cref="Point"/> with value 0,0.
    /// </returns>
    public Point StartPoint
    {
        get => (Point)GetValue(StartPointProperty);
        set => SetValueInternal(StartPointProperty, value);
    }

    /// <summary>
    /// Determines whether this <see cref="PathFigure"/> object may have curved segments.
    /// </summary>
    /// <returns>
    /// true if this <see cref="PathFigure"/> object may have curved segments; otherwise, false.
    /// </returns>
    public bool MayHaveCurves()
    {
        foreach (PathSegment segment in Segments.InternalItems)
        {
            if (segment.IsCurved())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets a <see cref="PathFigure"/> object that is a polygonal approximation of this 
    /// <see cref="PathFigure"/> object.
    /// </summary>
    /// <returns>
    /// The polygonal approximation of this <see cref="PathFigure"/> object.
    /// </returns>
    public PathFigure GetFlattenedPathFigure() => GetFlattenedPathFigure(Geometry.StandardFlatteningTolerance, ToleranceType.Absolute);

    /// <summary>
    /// Gets a <see cref="PathFigure"/> object, within the specified error of tolerance, that is
    /// a polygonal approximation of this <see cref="PathFigure"/> object. 
    /// </summary>
    /// <param name="tolerance">
    /// The computational tolerance of error.
    /// </param>
    /// <param name="type">
    /// Specifies how the error of tolerance is interpreted.
    /// </param>
    /// <returns>
    /// The polygonal approximation of this <see cref="PathFigure"/> object.
    /// </returns>
    public PathFigure GetFlattenedPathFigure(double tolerance, ToleranceType type)
    {
        var context = new PathStreamGeometryContext();

        var pathGeometry = new PathGeometryWrapper(GetFigureData(), FillRule.EvenOdd, Matrix.Identity);
        pathGeometry.FlattenToShape(tolerance, type == ToleranceType.Relative, context, Matrix.Identity);

        var figures = context.GetPathFigures();

        return figures?.Count switch
        {
            null or 0 => new PathFigure(),
            1 => figures[0],
            _ => throw new InvalidOperationException(Strings.PathGeometry_InternalReadBackError),
        };
    }

    internal void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
    {
        List<PathSegment> segments = Segments.InternalItems;

        if (segments.Count == 0)
        {
            return;
        }

        Point current = StartPoint;
        Point startPoint = current * transform;

        context.BeginFigure(startPoint, IsFilled, IsClosed);

        foreach (var segment in segments)
        {
            segment.SerializeData(context, transform, ref current);
        }
    }

    private byte[] GetFigureData()
    {
        var context = new ByteStreamGeometryContext();
        SerializeData(context, Matrix.Identity);
        context.Close();
        return context.GetData();
    }

    internal void SetParentGeometry(Geometry geometry)
    {
        _parentGeometry = geometry;
        foreach (var segment in Segments.InternalItems)
        {
            segment.SetParentGeometry(geometry);
        }
    }

    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PathFigure)d).InvalidateParentGeometry();
    }

    private void InvalidateParentGeometry() => _parentGeometry?.RaisePathChanged();
}