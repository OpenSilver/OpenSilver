
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

namespace System.Windows.Media
{
    /// <summary>
    /// Describes a two-dimensional rectangular geometry.
    /// </summary>
    public sealed class RectangleGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleGeometry"/> class, and creates a rectangle with zero area.
        /// </summary>
        public RectangleGeometry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleGeometry"/> class and specifies its dimensions.
        /// </summary>
        /// <param name="rect">
        /// A <see cref="Rect"/> structure with the rectangle's dimensions.
        /// </param>
        public RectangleGeometry(Rect rect)
        {
            Rect = rect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleGeometry"/> class.
        /// </summary>
        /// <param name="rect">
        /// A <see cref="Rect"/> structure with the rectangle's dimensions.
        /// </param>
        /// <param name="radiusX">
        /// The radius of the rounded corner where it connects with the upper and lower edges of the rectangle.
        /// </param>
        /// <param name="radiusY">
        /// The radius of the rounded corner where it connects with the left and right edges of the rectangle.
        /// </param>
        public RectangleGeometry(Rect rect, double radiusX, double radiusY)
            : this(rect)
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleGeometry"/> class.
        /// </summary>
        /// <param name="rect">
        /// A <see cref="Rect"/> structure with the rectangle's dimensions.
        /// </param>
        /// <param name="radiusX">
        /// The radius of the rounded corner where it connects with the upper and lower edges of the rectangle.
        /// </param>
        /// <param name="radiusY">
        /// The radius of the rounded corner where it connects with the left and right edges of the rectangle.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply to the geometry.
        /// </param>
        public RectangleGeometry(Rect rect, double radiusX, double radiusY, Transform transform)
            : this(rect, radiusX, radiusY)
        {
            Transform = transform;
        }

        /// <summary>
        /// Identifies the <see cref="Rect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register(
                nameof(Rect),
                typeof(Rect),
                typeof(RectangleGeometry),
                new PropertyMetadata(new Rect(), OnPathChanged));

        /// <summary>
        /// Gets or sets the dimensions of the rectangle.
        /// </summary>
        /// <returns>
        /// The <see cref="Rect"/> structure that describes the position and size of the
        /// rectangle. The default is null.
        /// </returns>
        public Rect Rect
        {
            get => (Rect)GetValue(RectProperty);
            set => SetValueInternal(RectProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0.0, OnPathChanged));

        /// <summary>
        /// Gets or sets the x-radius of the ellipse that is used to round the corners of
        /// the rectangle.
        /// </summary>
        /// <returns>
        /// The x-radius of the ellipse used to round the corners of the rectangle geometry.
        /// The default is 0.
        /// </returns>
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValueInternal(RadiusXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusY" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0.0, OnPathChanged));

        /// <summary>
        /// Gets or sets the y-radius of the ellipse that is used to round the corners of
        /// the rectangle.
        /// </summary>
        /// <returns>
        /// The y-radius of the ellipse used to round the corners of the rectangle geometry.
        /// The default is 0.
        /// </returns>
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValueInternal(RadiusYProperty, value);
        }

        /// <summary>
        /// Gets the area of the filled region of this <see cref="RectangleGeometry"/> object.
        /// </summary>
        /// <param name="tolerance">
        /// The computational tolerance of error.
        /// </param>
        /// <param name="type">
        /// Specifies how the error tolerance will be interpreted.
        /// </param>
        /// <returns>
        /// The area of the filled region of this <see cref="RectangleGeometry"/> object.
        /// </returns>
        public override double GetArea(double tolerance, ToleranceType type)
        {
            if (IsEmpty())
            {
                return 0.0;
            }

            double radiusX = RadiusX;
            double radiusY = RadiusY;
            Rect rect = Rect;

            // Get the area of the bounding rectangle
            double area = Math.Abs(rect.Width * rect.Height);

            // correct it for the rounded corners
            area -= Math.Abs(radiusX * radiusY) * (4.0 - Math.PI);

            // Adjust to internal transformation
            Matrix transform = Transform.ToMatrix(Transform);
            if (!transform.IsIdentity)
            {
                area *= Math.Abs(transform.Determinant);
            }

            return area;
        }

        /// <summary>
        /// Determines whether this <see cref="RectangleGeometry"/> object is empty.
        /// </summary>
        /// <returns>
        /// true if this <see cref="RectangleGeometry"/> is empty; otherwise, false.
        /// </returns>
        public override bool IsEmpty() => Rect.IsEmpty;

        /// <summary>
        /// Determines whether this <see cref="RectangleGeometry"/> object may have curved segments.
        /// </summary>
        /// <returns>
        /// true if this <see cref="RectangleGeometry"/> object may have curved segments; otherwise, false.
        /// </returns>
        public override bool MayHaveCurves() => IsRounded(RadiusX, RadiusY);

        internal override Rect GetBoundsInternal()
        {
            Rect boundsRect;

            Rect currentRect = Rect;
            Transform transform = Transform;

            if (currentRect.IsEmpty)
            {
                boundsRect = Rect.Empty;
            }
            else if (transform is null || Transform.IsIdentityTransform(transform))
            {
                boundsRect = currentRect;
            }
            else
            {
                double radiusX = RadiusX;
                double radiusY = RadiusY;

                if (radiusX == 0 && radiusY == 0)
                {
                    boundsRect = transform.TransformBounds(currentRect);
                }
                else
                {
                    boundsRect = base.GetBoundsInternal();
                }
            }

            return boundsRect;
        }

        internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
        {
            Rect rect = Rect;

            if (rect.IsEmpty)
            {
                return;
            }

            double radiusX = RadiusX;
            double radiusY = RadiusY;

            Matrix matrix = GetCombinedMatrix(transform);

            if (IsRounded(radiusX, radiusY))
            {
                radiusX = Math.Min(rect.Width * (1.0 / 2.0), Math.Abs(radiusX));
                radiusY = Math.Min(rect.Height * (1.0 / 2.0), Math.Abs(radiusY));

                Span<Point> points = stackalloc Point[16];

                double bezierX = (1.0 - EllipseGeometry.c_arcAsBezier) * radiusX;
                double bezierY = (1.0 - EllipseGeometry.c_arcAsBezier) * radiusY;

                points[1].X = points[0].X = points[15].X = points[14].X = rect.X;
                points[2].X = points[13].X = rect.X + bezierX;
                points[3].X = points[12].X = rect.X + radiusX;
                points[4].X = points[11].X = rect.Right - radiusX;
                points[5].X = points[10].X = rect.Right - bezierX;
                points[6].X = points[7].X = points[8].X = points[9].X = rect.Right;

                points[2].Y = points[3].Y = points[4].Y = points[5].Y = rect.Y;
                points[1].Y = points[6].Y = rect.Y + bezierY;
                points[0].Y = points[7].Y = rect.Y + radiusY;
                points[15].Y = points[8].Y = rect.Bottom - radiusY;
                points[14].Y = points[9].Y = rect.Bottom - bezierY;
                points[13].Y = points[12].Y = points[11].Y = points[10].Y = rect.Bottom;

                if (!matrix.IsIdentity)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] *= matrix;
                    }
                }

                context.BeginFigure(points[0], true, true);
                context.BezierTo(points[1], points[2], points[3], true, false);
                context.LineTo(points[4], true, false);
                context.BezierTo(points[5], points[6], points[7], true, false);
                context.LineTo(points[8], true, false);
                context.BezierTo(points[9], points[10], points[11], true, false);
                context.LineTo(points[12], true, false);
                context.BezierTo(points[13], points[14], points[15], true, false);
            }
            else
            {
                context.AddRect(rect, matrix);
            }
        }

        private static bool IsRounded(double radiusX, double radiusY) => radiusX != 0.0 && radiusY != 0.0;
    }
}
