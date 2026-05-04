
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

using OpenSilver.Internal.Media;

namespace System.Windows.Media;

/// <summary>
/// Represents an elliptical arc between two points.
/// </summary>
public sealed partial class ArcSegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArcSegment"/> class.
    /// </summary>
    public ArcSegment() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcSegment"/> class.
    /// </summary>
    /// <param name="point">
    /// The destination point of the arc; the start point of the arc is defined as the current point of the 
    /// <see cref="PathFigure"/> to which the <see cref="ArcSegment"/> is added.
    /// </param>
    /// <param name="size">
    /// The x- and y-radius of the arc. The x-radius is specified by the <see cref="Windows.Size"/> structure's 
    /// <see cref="Size.Width"/> property, and the y-radius is specified by the <see cref="Windows.Size"/> 
    /// structure's <see cref="Size.Height"/> property.
    /// </param>
    /// <param name="rotationAngle">
    /// The x-axis rotation of the ellipse.
    /// </param>
    /// <param name="isLargeArc">
    /// Whether the arc should be greater than 180 degrees.
    /// </param>
    /// <param name="sweepDirection">
    /// Set to <see cref="SweepDirection.Clockwise"/> to draw the arc in a positive angle direction; set to 
    /// <see cref="SweepDirection.Counterclockwise"/> to draw the arc in a negative angle direction.
    /// </param>
    /// <param name="isStroked">
    /// Set to true to stroke the arc when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    public ArcSegment(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
    {
        Point = point;
        Size = size;
        RotationAngle = rotationAngle;
        IsLargeArc = isLargeArc;
        SweepDirection = sweepDirection;
        IsStroked = isStroked;
    }

    /// <summary>
    /// Identifies the <see cref="IsLargeArc"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLargeArcProperty =
        DependencyProperty.Register(
            nameof(IsLargeArc),
            typeof(bool),
            typeof(ArcSegment),
            new PropertyMetadata(false, PropertyChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether the arc should be greater than 180
    /// degrees.
    /// </summary>
    /// <returns>
    /// true if the arc should be greater than 180 degrees; otherwise, false. The default
    /// is false.
    /// </returns>
    public bool IsLargeArc
    {
        get => (bool)GetValue(IsLargeArcProperty);
        set => SetValueInternal(IsLargeArcProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Point"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PointProperty =
        DependencyProperty.Register(
            nameof(Point),
            typeof(Point),
            typeof(ArcSegment),
            new PropertyMetadata(new Point(), PropertyChanged));

    /// <summary>
    /// Gets or sets the endpoint of the elliptical arc.
    /// </summary>
    /// <returns>
    /// The point to which the arc is drawn. The default is a <see cref="Point"/> with
    /// value 0,0.
    /// </returns>
    public Point Point
    {
        get => (Point)GetValue(PointProperty);
        set => SetValueInternal(PointProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RotationAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationAngleProperty =
        DependencyProperty.Register(
            nameof(RotationAngle),
            typeof(double),
            typeof(ArcSegment),
            new PropertyMetadata(0.0, PropertyChanged));

    /// <summary>
    /// Gets or sets the amount (in degrees) by which the ellipse is rotated about the
    /// x-axis.
    /// </summary>
    /// <returns>
    /// The amount (in degrees) by which the ellipse is rotated about the x-axis. The
    /// default value is 0.
    /// </returns>
    public double RotationAngle
    {
        get => (double)GetValue(RotationAngleProperty);
        set => SetValueInternal(RotationAngleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Size"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(Size),
            typeof(ArcSegment),
            new PropertyMetadata(new Size(), PropertyChanged));

    /// <summary>
    /// Gets or sets the x- and y-radius of the arc as a <see cref="Size"/> structure.
    /// </summary>
    /// <returns>
    /// A <see cref="Size"/> structure that describes the x- and y-radius of the elliptical
    /// arc. The <see cref="Size"/> structure's <see cref="Size.Width"/> property specifies
    /// the arc's x-radius; its <see cref="Size.Height"/> property specifies the arc's
    /// y-radius. The default is a <see cref="Size"/> with value 0,0.
    /// </returns>
    public Size Size
    {
        get => (Size)GetValue(SizeProperty);
        set => SetValueInternal(SizeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SweepDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SweepDirectionProperty =
        DependencyProperty.Register(
            nameof(SweepDirection),
            typeof(SweepDirection),
            typeof(ArcSegment),
            new PropertyMetadata(SweepDirection.Counterclockwise, PropertyChanged));

    /// <summary>
    /// Gets or sets a value that specifies whether the arc is drawn in the <see cref="SweepDirection.Clockwise"/>
    /// or <see cref="SweepDirection.Counterclockwise"/> direction.
    /// </summary>
    /// <returns>
    /// One of the enumeration values that specifies the direction in which the arc is
    /// drawn. The default is <see cref="SweepDirection.Counterclockwise"/>.
    /// </returns>
    public SweepDirection SweepDirection
    {
        get => (SweepDirection)GetValue(SweepDirectionProperty);
        set => SetValueInternal(SweepDirectionProperty, value);
    }

    internal override void SerializeData(StreamGeometryContext context, Matrix transform, ref Point current)
    {
        Point point = Point;
        Size size = Size;
        double rotationAngle = RotationAngle;
        bool isLargeArc = IsLargeArc;
        SweepDirection sweepDirection = SweepDirection;
        bool isStroked = IsStroked;
        bool isSmoothJoin = IsSmoothJoin;

        if (transform.IsIdentity)
        {
            context.ArcTo(point, size, rotationAngle, isLargeArc, sweepDirection, isStroked, isSmoothJoin);
        }
        else
        {
            ArcToBezierHelper(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection,
                isStroked,
                isSmoothJoin,
                current,
                transform,
                context);
        }

        current = point;
    }

    internal static void ArcToBezierHelper(
        Point point,
        Size size,
        double rotationAngle,
        bool isLargeArc,
        SweepDirection sweepDirection,
        bool isStroked,
        bool isSmoothJoin,
        Point current,
        Matrix transform,
        StreamGeometryContext context)
    {
        Span<Point> points = stackalloc Point[12];

        GeometryUtils.ArcToBezier(
            current.X,
            current.Y,
            size.Width,
            size.Height,
            rotationAngle,
            isLargeArc,
            sweepDirection == SweepDirection.Clockwise,
            point.X,
            point.Y,
            points,
            out int count);

        // To ensure no buffer overflows
        count = Math.Min(count, 4);

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Point p1 = transform.Transform(points[3 * i]);
                Point p2 = transform.Transform(points[3 * i + 1]);
                Point p3 = transform.Transform(points[3 * i + 2]);

                context.BezierTo(p1, p2, p3, isStroked, (i < count - 1) || isSmoothJoin);
            }
        }
        else if (count == 0)
        {
            Point p = transform.Transform(points[0]);

            context.LineTo(p, isStroked, isSmoothJoin);
        }
    }

    internal override bool IsCurved() => true;
}
