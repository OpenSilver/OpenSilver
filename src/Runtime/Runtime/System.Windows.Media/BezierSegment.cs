
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

namespace System.Windows.Media;

/// <summary>
/// Represents a cubic Bezier curve drawn between two points.
/// </summary>
public sealed class BezierSegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BezierSegment"/> class.
    /// </summary>
    public BezierSegment() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BezierSegment"/> class with the specified control 
    /// points, end point, and stroke option.
    /// </summary>
    /// <param name="point1">
    /// The first control point, which determines the beginning portion of the curve.
    /// </param>
    /// <param name="point2">
    /// The second control point, which determines the ending portion of the curve.
    /// </param>
    /// <param name="point3">
    /// The point to which the curve is drawn.
    /// </param>
    /// <param name="isStroked">
    /// true to stroke the curve when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    public BezierSegment(Point point1, Point point2, Point point3, bool isStroked)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        IsStroked = isStroked;
    }

    /// <summary>
    /// Identifies the <see cref="Point1"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point1Property =
        DependencyProperty.Register(
            nameof(Point1),
            typeof(Point),
            typeof(BezierSegment),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the first control point of the curve.
    /// </summary>
    /// <returns>
    /// The first control point of the curve. The default is a <see cref="Point"/> with
    /// value 0,0.
    /// </returns>
    public Point Point1
    {
        get => (Point)GetValue(Point1Property);
        set => SetValueInternal(Point1Property, value);
    }

    /// <summary>
    /// Identifies the <see cref="Point2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point2Property =
        DependencyProperty.Register(
            nameof(Point2),
            typeof(Point),
            typeof(BezierSegment),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the second control point of the curve.
    /// </summary>
    /// <returns>
    /// The second control point of the curve.
    /// </returns>
    public Point Point2
    {
        get => (Point)GetValue(Point2Property);
        set => SetValueInternal(Point2Property, value);
    }

    /// <summary>
    /// Identifies the <see cref="Point3"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point3Property =
        DependencyProperty.Register(
            nameof(Point3),
            typeof(Point),
            typeof(BezierSegment),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the end point of the curve.
    /// </summary>
    /// <returns>
    /// The end point of the curve.
    /// </returns>
    public Point Point3
    {
        get => (Point)GetValue(Point3Property);
        set => SetValueInternal(Point3Property, value);
    }

    internal override void SerializeData(StreamGeometryContext context, Matrix transform, ref Point current)
    {
        Point point1 = Point1;
        Point point2 = Point2;
        Point point3 = Point3;

        current = point3;

        if (!transform.IsIdentity)
        {
            point1 *= transform;
            point2 *= transform;
            point3 *= transform;
        }

        context.BezierTo(point1, point2, point3, IsStroked, IsSmoothJoin);
    }

    internal override bool IsCurved() => true;
}
