
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public sealed class EllipseGeometry : Geometry
    {
        ///// <summary>
        ///// Initializes a new instance of the EllipseGeometry class.
        ///// </summary>
        //public EllipseGeometry();

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
            DependencyProperty.Register("Center", typeof(Point), typeof(EllipseGeometry), new PropertyMetadata(new Point(), Point_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Point_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (e.NewValue != e.OldValue && geometry.INTERNAL_parentPath != null && geometry.INTERNAL_parentPath._isLoaded)
            {
                geometry.INTERNAL_parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("RadiusX", typeof(double), typeof(EllipseGeometry), new PropertyMetadata(0d, RadiusX_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void RadiusX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (e.NewValue != e.OldValue && geometry.INTERNAL_parentPath != null && geometry.INTERNAL_parentPath._isLoaded)
            {
                geometry.INTERNAL_parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("RadiusY", typeof(double), typeof(EllipseGeometry), new PropertyMetadata(0d, RadiusY_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void RadiusY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EllipseGeometry geometry = (EllipseGeometry)d;
            if (e.NewValue != e.OldValue && geometry.INTERNAL_parentPath != null && geometry.INTERNAL_parentPath._isLoaded)
            {
                geometry.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            double maxAbs = Center.X + RadiusX;
            double minAbs = Center.X - RadiusX;
            double minOrd = Center.Y - RadiusY;
            double maxOrd = Center.Y + RadiusY;
            if (maxX < maxAbs)
            {
                maxX = maxAbs;
            }
            if (maxY < maxOrd)
            {
                maxY = maxOrd;
            }
            if (minX > minAbs)
            {
                minX = minAbs;
            }
            if (minY > minOrd)
            {
                minY = minOrd;
            }
        }

        internal protected override void DefineInCanvas(Shapes.Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize)
        {
            string strokeAsString = string.Empty;
            if (path.Stroke == null || path.Stroke is SolidColorBrush) //todo: make sure we want the same behaviour when it is null and when it is a SolidColorBrush (basically, check if null means default value)
            {
                if (path.Stroke != null) //if stroke is null, we want to set it as an empty string, otherwise, it is a SolidColorBrush and we want to get its color.
                {
                    strokeAsString = ((SolidColorBrush)path.Stroke).INTERNAL_ToHtmlString();
                }
            }
            else
            {
                throw new NotSupportedException("The specified brush is not supported.");
            }
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            double actualWidth = RadiusX * 2; //it's a radius and we want the whole width (basically a "diameter")
            double actualHeight = RadiusY * 2; //it's a radius and we want the whole height

            INTERNAL_ShapesDrawHelpers.PrepareEllipse(canvasDomElement, actualWidth, actualHeight, Center.X + xOffsetToApplyBeforeMultiplication + xOffsetToApplyAfterMultiplication, Center.Y + yOffsetToApplyBeforeMultiplication + yOffsetToApplyAfterMultiplication);
            context.strokeStyle = strokeAsString;
        }
    }
}
