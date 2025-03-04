
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents the geometry of a circle or ellipse.
    /// </summary>
    public sealed class EllipseGeometry : Geometry
    {
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

        internal override Rect BoundsInternal
        {
            get
            {
                // Note: Transform is not supported. This will only be valid
                // if Transform is null or is the Identity transform.

                Rect boundsRect;

                Point currentCenter = Center;
                double currentRadiusX = RadiusX;
                double currentRadiusY = RadiusY;

                boundsRect = new Rect(
                    currentCenter.X - Math.Abs(currentRadiusX),
                    currentCenter.Y - Math.Abs(currentRadiusY),
                    2.0 * Math.Abs(currentRadiusX),
                    2.0 * Math.Abs(currentRadiusY));

                return boundsRect;
            }
        }

        internal override string ToPathData(IFormatProvider formatProvider)
        {
            var cx = Center.X;
            var cy = Center.Y;
            var rx = RadiusX;
            var ry = RadiusY;

            return $"M{cx.ToString(formatProvider)},{(cy - ry).ToString(formatProvider)} A{rx.ToString(formatProvider)},{ry.ToString(formatProvider)} 0 0 0 {cx.ToString(formatProvider)},{(cy + ry).ToString(formatProvider)} A{rx.ToString(formatProvider)},{ry.ToString(formatProvider)} 0 0 0 {cx.ToString(formatProvider)},{(cy - ry).ToString(formatProvider)} Z";
        }
    }
}
