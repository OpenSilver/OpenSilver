
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

using System;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
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
            set => SetValue(CenterProperty, value);
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
            set => SetValue(RadiusXProperty, value);
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
            set => SetValue(RadiusYProperty, value);
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
