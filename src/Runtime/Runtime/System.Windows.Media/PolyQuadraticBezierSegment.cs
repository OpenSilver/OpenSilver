

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
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a set of quadratic Bezier segments.
    /// </summary>
    [ContentProperty("Points")]
    public sealed partial class PolyQuadraticBezierSegment : PathSegment
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PolyQuadraticBezierSegment class.
        /// </summary>
        public PolyQuadraticBezierSegment()
        {

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the Point collection that defines this PolyQuadraticBezierSegment
        /// object.
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PolyQuadraticBezierSegment.Points"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points), 
                typeof(PointCollection), 
                typeof(PolyQuadraticBezierSegment), 
                new PropertyMetadata(null, Points_Changed));

        private static void Points_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: find a way to know when the points changed in the collection
            PolyQuadraticBezierSegment segment = (PolyQuadraticBezierSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        #endregion

        #region Overriden Methods

        internal override void SetParentPath(Path path)
        {
            base.SetParentPath(path);
            Points.SetParentShape(path);
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
            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            int i = 0;
            Point lastPoint = previousLastPoint;
            while (i < Points.Count - 1)
            {
                double controlPoint1X = Points[i].X;
                double controlPoint1Y = Points[i].Y;
                ++i;
                lastPoint = Points[i];
                double endPointX = lastPoint.X;
                double endPointY = lastPoint.Y;
                ++i;

                // tell the context that there should be a cubic bezier curve from the starting 
                // point to this point, with the two previous points as control points.
                //context.quadraticCurveTo(
                //    (controlPoint1X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                //    (controlPoint1Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                //    (endPointX + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                //    (endPointY + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
                //Note: we replaced the code above with the one below because Bridge.NET has an issue when adding "0" to an Int64 (as of May 1st, 2020), so it is better to first multiply and then add, rather than the contrary:
                context.quadraticCurveTo(
                    controlPoint1X * horizontalMultiplicator + xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication,
                    controlPoint1Y * verticalMultiplicator + yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                    endPointX * horizontalMultiplicator + xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication,
                    endPointY * verticalMultiplicator + yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            }
            return lastPoint;
        }

        internal override Point GetMaxXY() //todo: make this give the size of the actual curve, not the control points.
        {
            Point currentMax = new Point(double.NegativeInfinity, double.NegativeInfinity);
            foreach (Point point in Points)
            {
                if (point.X > currentMax.X)
                {
                    currentMax.X = point.X;
                }
                if (point.Y > currentMax.Y)
                {
                    currentMax.Y = point.Y;
                }
            }
            return currentMax;
        }

        internal override Point GetMinMaxXY(ref double minX, 
                                            ref double maxX, 
                                            ref double minY, 
                                            ref double maxY, 
                                            Point startingPoint)
        {
            foreach (Point point in Points)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }
            return Points.Count == 0 ? startingPoint : Points[Points.Count - 1];
        }

        #endregion
    }
}
