
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

namespace System.Windows.Media
{
    /// <summary>
    /// Describes a two-dimensional rectangular geometry.
    /// </summary>
    public sealed class RectangleGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleGeometry"/> class,
        /// and creates a rectangle with zero area.
        /// </summary>
        public RectangleGeometry() { }

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
            set => SetValue(RectProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the x-radius of the ellipse that is used to round the corners of
        /// the rectangle.
        /// </summary>
        /// <returns>
        /// The x-radius of the ellipse used to round the corners of the rectangle geometry.
        /// The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusY" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the y-radius of the ellipse that is used to round the corners of
        /// the rectangle.
        /// </summary>
        /// <returns>
        /// The y-radius of the ellipse used to round the corners of the rectangle geometry.
        /// The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }

        internal override Rect BoundsInternal
        {
            get
            {
                Rect boundsRect;

                Rect currentRect = Rect;
                Transform transform = Transform;

                if (currentRect.IsEmpty)
                {
                    boundsRect = Rect.Empty;
                }
                else if (transform == null || transform.IsIdentity)
                {
                    boundsRect = currentRect;
                }
                else
                {
                    boundsRect = transform.TransformBounds(currentRect);
                }

                return boundsRect;
            }
        }

        internal override string ToPathData(IFormatProvider formatProvider)
        {
            var rect = Rect;

            var left = rect.Left.ToString(formatProvider);
            var top = rect.Top.ToString(formatProvider);
            var right = rect.Right.ToString(formatProvider);
            var bottom = rect.Bottom.ToString(formatProvider);

            return $"M{left},{top} L{right},{top} {right},{bottom} {left},{bottom} Z";
        }
    }
}
