

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public sealed partial class EllipseGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the EllipseGeometry class.
        /// </summary>
        public EllipseGeometry()
        {

        }

        /// <summary>
        /// Gets or sets the center point of the EllipseGeometry.
        /// </summary>
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }
        /// <summary>
        /// Identifies the Center dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(EllipseGeometry), new PropertyMetadata(new Point(), Point_Changed));

        private static void Point_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (geometry.ParentPath != null && geometry.ParentPath._isLoaded)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the x-radius value of the EllipseGeometry.
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }
        /// <summary>
        /// Identifies the RadiusX dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(EllipseGeometry), new PropertyMetadata(0d, RadiusX_Changed));

        private static void RadiusX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (geometry.ParentPath != null && geometry.ParentPath._isLoaded)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the y-radius value of the EllipseGeometry.
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }
        /// <summary>
        /// Identifies the RadiusY dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(EllipseGeometry), new PropertyMetadata(0d, RadiusY_Changed));

        private static void RadiusY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (geometry.ParentPath != null && geometry.ParentPath._isLoaded)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            double maxAbs = Center.X + RadiusX;
            double minAbs = Center.X - RadiusX;
            double minOrd = Center.Y - RadiusY;
            double maxOrd = Center.Y + RadiusY;
            minX = Math.Min(minX, minAbs);
            maxX = Math.Max(maxX, maxAbs);
            minY = Math.Min(minY, minOrd);
            maxY = Math.Max(maxY, maxOrd);
        }

        internal protected override void DefineInCanvas(Shapes.Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize)
        {
            dynamic ctx = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            ctx.ellipse(
                Center.X + xOffsetToApplyBeforeMultiplication + xOffsetToApplyAfterMultiplication,
                Center.Y + yOffsetToApplyBeforeMultiplication + yOffsetToApplyAfterMultiplication,
                RadiusX,
                RadiusY,
                0,
                0,
                2 * Math.PI);
        }
    }
}
