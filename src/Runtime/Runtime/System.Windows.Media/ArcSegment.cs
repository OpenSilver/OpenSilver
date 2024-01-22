
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents an elliptical arc between two points.
    /// </summary>
    public sealed class ArcSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcSegment"/> class.
        /// </summary>
        public ArcSegment() { }

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

        internal override IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#elliptical_arc_curve

            yield return "A";
            yield return Size.Width.ToString(formatProvider);
            yield return Size.Height.ToString(formatProvider);
            yield return RotationAngle.ToString(formatProvider);
            yield return IsLargeArc ? "1" : "0";
            yield return SweepDirection == SweepDirection.Clockwise ? "1" : "0";
            yield return Point.X.ToString(formatProvider);
            yield return Point.Y.ToString(formatProvider);
        }
    }
}
