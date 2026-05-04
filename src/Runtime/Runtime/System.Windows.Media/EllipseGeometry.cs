
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents the geometry of a circle or ellipse.
    /// </summary>
    public sealed class EllipseGeometry : Geometry
    {
        // Approximating a 1/4 circle with a Bezier curve
        internal const double c_arcAsBezier = 0.5522847498307933984;

        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseGeometry"/> class.
        /// </summary>
        public EllipseGeometry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseGeometry"/> class that has a horizontal diameter equal to 
        /// the width of the passed <see cref="Rect"/>, a vertical diameter equal to the length of the passed <see cref="Rect"/>, 
        /// and a center point location equal to the center of the passed <see cref="Rect"/>.
        /// </summary>
        /// <param name="rect">
        /// The rectangle that describes the ellipse dimensions.
        /// </param>
        public EllipseGeometry(Rect rect)
        {
            if (rect.IsEmpty)
            {
                throw new ArgumentException(string.Format(Strings.Rect_Empty, nameof(rect)));
            }

            RadiusX = (rect.Right - rect.X) * (1.0 / 2.0);
            RadiusY = (rect.Bottom - rect.Y) * (1.0 / 2.0);
            Center = new Point(rect.X + RadiusX, rect.Y + RadiusY);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseGeometry"/> class as an ellipse that has a specified center 
        /// location, x radius, and y radius.
        /// </summary>
        /// <param name="center">
        /// The location of the center of the ellipse.
        /// </param>
        /// <param name="radiusX">
        /// The horizontal radius of the ellipse.
        /// </param>
        /// <param name="radiusY">
        /// The vertical radius of the ellipse.
        /// </param>
        public EllipseGeometry(Point center, double radiusX, double radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseGeometry"/> class that has the specified position, size, 
        /// and transformation.
        /// </summary>
        /// <param name="center">
        /// The location of the center of the ellipse.
        /// </param>
        /// <param name="radiusX">
        /// The horizontal radius of the ellipse.
        /// </param>
        /// <param name="radiusY">
        /// The vertical radius of the ellipse.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply to the ellipse.
        /// </param>
        public EllipseGeometry(Point center, double radiusX, double radiusY, Transform transform)
            : this(center, radiusX, radiusY)
        {
            Transform = transform;
        }

        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(EllipseGeometry),
                new PropertyMetadata(new Point(), OnPathChanged));

        /// <summary>
        /// Gets or sets the center point of the <see cref="EllipseGeometry"/>.
        /// </summary>
        /// <returns>
        /// The center point of the <see cref="EllipseGeometry"/>.
        /// </returns>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValueInternal(CenterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(EllipseGeometry),
                new PropertyMetadata(0.0, OnPathChanged));

        /// <summary>
        /// Gets or sets the x-radius value of the <see cref="EllipseGeometry"/>.
        /// </summary>
        /// <returns>
        /// The x-radius value of the <see cref="EllipseGeometry"/>.
        /// </returns>
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValueInternal(RadiusXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(EllipseGeometry),
                new PropertyMetadata(0.0, OnPathChanged));

        /// <summary>
        /// Gets or sets the y-radius value of the <see cref="EllipseGeometry"/>.
        /// </summary>
        /// <returns>
        /// The y-radius value of the <see cref="EllipseGeometry"/>.
        /// </returns>
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValueInternal(RadiusYProperty, value);
        }

        /// <summary>
        /// Determines whether this <see cref="EllipseGeometry"/> object is empty.
        /// </summary>
        /// <returns>
        /// true if this <see cref="EllipseGeometry"/> is empty; otherwise, false.
        /// </returns>
        public override bool IsEmpty() => false;

        /// <summary>
        /// Determines whether this <see cref="EllipseGeometry"/> object can have curved segments.
        /// </summary>
        /// <returns>
        /// true if this <see cref="EllipseGeometry"/> object can have curved segments; otherwise, false.
        /// </returns>
        public override bool MayHaveCurves() => true;

        internal override Rect BoundsInternal
        {
            get
            {
                Point currentCenter = Center;
                double currentRadiusX = Math.Abs(RadiusX);
                double currentRadiusY = Math.Abs(RadiusY);

                var boundsRect = new Rect(
                    currentCenter.X - currentRadiusX,
                    currentCenter.Y - currentRadiusY,
                    2.0 * currentRadiusX,
                    2.0 * currentRadiusY);

                if (Transform is Transform transform && !Transform.IsIdentityTransform(transform))
                {
                    boundsRect = transform.TransformBounds(boundsRect);
                }

                return boundsRect;
            }
        }

        internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
        {
            Span<Point> points = stackalloc Point[13];

            double radiusX = Math.Abs(RadiusX);
            double radiusY = Math.Abs(RadiusY);
            Point center = Center;

            // Set the X coordinates
            double mid = radiusX * c_arcAsBezier;

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = radiusY * c_arcAsBezier;

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;

            Matrix matrix = GetCombinedMatrix(transform);
            if (!matrix.IsIdentity)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] *= matrix;
                }
            }

            context.BeginFigure(points[0], true, true);

            for (int i = 0; i < 4; i++)
            {
                context.BezierTo(points[3 * i + 1], points[3 * i + 2], points[3 * i + 3], true, true);
            }
        }
    }
}
