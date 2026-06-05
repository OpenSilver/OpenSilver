
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

using System.Windows.Shapes;

namespace System.Windows.Media;

/// <summary>
/// Represents a line drawn between two points, which can be part of a <see cref="PathFigure"/>
/// within <see cref="Path"/> data.
/// </summary>
public sealed class LineSegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> class.
    /// </summary>
    public LineSegment() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> class that has
    /// the specified end <see cref="Point"/> and Boolean that determines whether this <see cref="LineSegment"/> is stroked.
    /// </summary>
    /// <param name="point">The end point of this <see cref="LineSegment"/>.</param>
    /// <param name="isStroked"><see langword="true"/> to stroke this <see cref="LineSegment"/>; otherwise, <see langword="false"/>.</param>
    public LineSegment(Point point, bool isStroked)
    {
        Point = point;
        IsStroked = isStroked;
    }

    /// <summary>
    /// Identifies the <see cref="Point"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PointProperty =
        DependencyProperty.Register(
            nameof(Point),
            typeof(Point),
            typeof(LineSegment),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the end point of the line segment.
    /// </summary>
    /// <returns>
    /// The end point of the line segment. The default is a <see cref="Point"/> with 
    /// value 0,0.
    /// </returns>
    public Point Point
    {
        get => (Point)GetValue(PointProperty);
        set => SetValueInternal(PointProperty, value);
    }

    internal override void SerializeData(StreamGeometryContext context, Matrix transform, ref Point current)
    {
        Point point = Point;

        current = point;

        if (!transform.IsIdentity)
        {
            point *= transform;
        }

        context.LineTo(point, IsStroked, IsSmoothJoin);
    }

    internal override bool IsCurved() => false;
}