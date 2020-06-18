

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
    /// Represents a cubic Bezier curve drawn between two points.
    /// </summary>
    public sealed partial class BezierSegment : PathSegment
    {
        ///// <summary>
        ///// Initializes a new instance of the BezierSegment class.
        ///// </summary>
        //public BezierSegment();

        /// <summary>
        /// Gets or sets the first control point of the curve.
        /// </summary>
        public Point Point1
        {
            get { return (Point)GetValue(Point1Property); }
            set { SetValue(Point1Property, value); }
        }
        /// <summary>
        /// Identifies the Point1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register("Point1", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point1_Changed));

        private static void Point1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the second control point of the curve.
        /// </summary>
        public Point Point2
        {
            get { return (Point)GetValue(Point2Property); }
            set { SetValue(Point2Property, value); }
        }
        /// <summary>
        /// Identifies the Point2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register("Point2", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point2_Changed));

        private static void Point2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the end point of the curve.
        /// </summary>
        public Point Point3
        {
            get { return (Point)GetValue(Point3Property); }
            set { SetValue(Point3Property, value); }
        }
        /// <summary>
        /// Identifies the Point3 dependency property.
        /// </summary>
        public static readonly DependencyProperty Point3Property =
            DependencyProperty.Register("Point3", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point3_Changed));

        private static void Point3_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, 
                                               double yOffsetToApplyBeforeMultiplication, 
                                               double xOffsetToApplyAfterMultiplication, 
                                               double yOffsetToApplyAfterMultiplication, 
                                               double horizontalMultiplicator, 
                                               double verticalMultiplicator, 
                                               object canvasDomElement, 
                                               Point previousLastPoint)
        {
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            // tell the context that there should be a cubic bezier curve from the 
            // starting point to this point, with the two previous points as control points.
            context.bezierCurveTo(
                (Point1.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                (Point1.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                (Point2.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                (Point2.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                (Point3.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                (Point3.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            return Point3;
        }

        // todo: make this give the size of the actual curve, not the control points.
        internal override Point GetMaxXY() 
        {
            return new Point(Math.Max(Point1.X, Math.Max(Point2.X, Point3.X)),
                             Math.Max(Point1.Y, Math.Max(Point2.Y, Point3.Y)));
        }

        internal override Point GetMinMaxXY(ref double minX, 
                                            ref double maxX, 
                                            ref double minY, 
                                            ref double maxY, 
                                            Point startingPoint)
        {
            minX = Math.Min(minX, Math.Min(Point1.X, Math.Min(Point2.X, Point3.X)));
            maxX = Math.Max(maxX, Math.Max(Point1.X, Math.Max(Point2.X, Point3.X)));
            minY = Math.Min(minY, Math.Min(Point1.Y, Math.Min(Point2.Y, Point3.Y)));
            maxY = Math.Max(maxY, Math.Max(Point1.Y, Math.Max(Point2.Y, Point3.Y)));

            return Point3;
        }

    }
}
