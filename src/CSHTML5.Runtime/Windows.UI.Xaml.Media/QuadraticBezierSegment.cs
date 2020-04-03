

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
    /// Creates a quadratic Bezier curve between two points in a PathFigure.
    /// </summary>
    public sealed partial class QuadraticBezierSegment : PathSegment
    {
        ///// <summary>
        ///// Initializes a new instance of the QuadraticBezierSegment class.
        ///// </summary>
        //public QuadraticBezierSegment();

        /// <summary>
        /// Gets or sets the control point of the curve.
        /// </summary>
        public Point Point1
        {
            get { return (Point)GetValue(Point1Property); }
            set { SetValue(Point1Property, value); }
        }
        /// <summary>
        /// Identifies the Point1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register("Point1", typeof(Point), typeof(QuadraticBezierSegment), new PropertyMetadata(new Point(), Point1_Changed));

        private static void Point1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuadraticBezierSegment segment = (QuadraticBezierSegment)d;
            if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
            {
                segment.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the end Point of this QuadraticBezierSegment.
        /// </summary>
        public Point Point2
        {
            get { return (Point)GetValue(Point2Property); }
            set { SetValue(Point2Property, value); }
        }
        /// <summary>
        /// Identifies the Point2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register("Point2", typeof(Point), typeof(QuadraticBezierSegment), new PropertyMetadata(new Point(), Point2_Changed));

        private static void Point2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuadraticBezierSegment segment = (QuadraticBezierSegment)d;
            if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
            {
                segment.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, Point previousLastPoint)
        {
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            context.quadraticCurveTo((Point1.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (Point1.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                   (Point2.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (Point2.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication); // tell the context that there should be a quadratic bezier curve from the starting point to this point, with the previous point as control point.
            return Point2;
        }

        internal override Point GetMaxXY() //todo: make this give the size of the actual curve, not the control points.
        {
            double maxX = Point1.X;
            double maxY = Point1.Y;
            if (Point2.X > maxX)
            {
                maxX = Point2.X;
            }
            if (Point2.Y > maxY)
            {
                maxY = Point2.Y;
            }
            return new Point(maxX, maxY);
        }

        internal override Point GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, Point startingPoint)
        {
            if (minX > Point1.X)
            {
                minX = Point1.X;
            }
            if (maxX < Point1.X)
            {
                maxX = Point1.X;
            }
            if (minY > Point1.Y)
            {
                minY = Point1.Y;
            }
            if (maxY < Point1.Y)
            {
                maxY = Point1.Y;
            }

            if (minX > Point2.X)
            {
                minX = Point2.X;
            }
            if (maxX < Point2.X)
            {
                maxX = Point2.X;
            }
            if (minY > Point2.Y)
            {
                minY = Point2.Y;
            }
            if (maxY < Point2.Y)
            {
                maxY = Point2.Y;
            }
            return Point2;
        }

    }
}
