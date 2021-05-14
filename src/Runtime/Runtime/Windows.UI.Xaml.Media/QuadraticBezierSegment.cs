

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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the QuadraticBezierSegment class.
        /// </summary>
        public QuadraticBezierSegment()
        {

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the control point of the curve.
        /// </summary>
        public Point Point1
        {
            get { return (Point)GetValue(Point1Property); }
            set { SetValue(Point1Property, value); }
        }

        /// <summary>
        /// Identifies the <see cref="QuadraticBezierSegment.Point1"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(
                nameof(Point1), 
                typeof(Point), 
                typeof(QuadraticBezierSegment), 
                new PropertyMetadata(new Point(), Point1_Changed));

        private static void Point1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuadraticBezierSegment segment = (QuadraticBezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
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
        /// Identifies the <see cref="QuadraticBezierSegment.Point2"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(
                nameof(Point2), 
                typeof(Point), 
                typeof(QuadraticBezierSegment), 
                new PropertyMetadata(new Point(), Point2_Changed));

        private static void Point2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuadraticBezierSegment segment = (QuadraticBezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        #endregion

        #region Overriden Methods

        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, 
                                               double yOffsetToApplyBeforeMultiplication, 
                                               double xOffsetToApplyAfterMultiplication, 
                                               double yOffsetToApplyAfterMultiplication, 
                                               double horizontalMultiplicator, 
                                               double verticalMultiplicator, 
                                               object canvasDomElement, 
                                               Point previousLastPoint)
        {
            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            // tell the context that there should be a quadratic bezier curve from the starting 
            // point to this point, with the previous point as control point.
            //context.quadraticCurveTo(
            //    (Point1.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
            //    (Point1.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
            //    (Point2.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
            //    (Point2.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            //Note: we replaced the code above with the one below because Bridge.NET has an issue when adding "0" to an Int64 (as of May 1st, 2020), so it is better to first multiply and then add, rather than the contrary:
            context.quadraticCurveTo(
                    Point1.X * horizontalMultiplicator + xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication,
                    Point1.Y * verticalMultiplicator + yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                    Point2.X * horizontalMultiplicator + xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication,
                    Point2.Y * verticalMultiplicator + yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication);

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
            minX = Math.Min(minX, Math.Min(Point1.X, Point2.X));
            maxX = Math.Max(maxX, Math.Max(Point1.X, Point2.X));
            minY = Math.Min(minY, Math.Min(Point1.Y, Point2.Y));
            maxY = Math.Max(maxY, Math.Max(Point1.Y, Point2.Y));
            return Point2;
        }

        #endregion
    }
}
