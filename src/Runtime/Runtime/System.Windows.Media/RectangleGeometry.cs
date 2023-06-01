
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    public sealed class RectangleGeometry : Geometry
    {
        internal protected override void DefineInCanvas(Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize)
        {
            var ctx = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            Rect rect = Rect;

            ctx.rect(
                rect.X + xOffsetToApplyBeforeMultiplication + xOffsetToApplyAfterMultiplication, 
                rect.Y + yOffsetToApplyBeforeMultiplication + yOffsetToApplyAfterMultiplication,
                rect.Width,
                rect.Height);
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            Rect rect = Rect;
            minX = Math.Min(minX, rect.X);
            maxX = Math.Max(maxX, rect.X + rect.Width);
            minY = Math.Min(minY, rect.Y);
            maxY = Math.Max(maxY, rect.Y + rect.Height);
        }

        /// <summary>
        ///     Rect - Rect.  Default value is Rect.Empty.
        /// </summary>
        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RectangleGeometry.Rect"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty RectProperty = 
            DependencyProperty.Register(
                nameof(Rect), 
                typeof(Rect), 
                typeof(RectangleGeometry), 
                new PropertyMetadata(Rect.Empty));

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
            get { return (double)this.GetValue(RadiusXProperty); }
            set { this.SetValue(RadiusXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RectangleGeometry.RadiusX"/> dependency
        /// property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0d));

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
            get { return (double)this.GetValue(RadiusYProperty); }
            set { this.SetValue(RadiusYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RectangleGeometry.RadiusY" /> dependency
        /// property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(RectangleGeometry),
                new PropertyMetadata(0d));

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
    }
}
