

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
    /// Represents a line drawn between two points, which can be part of a PathFigure
    /// within Path data.
    /// </summary>
    public sealed partial class LineSegment : PathSegment
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LineSegment class.
        /// </summary>
        public LineSegment()
        {

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the end point of the line segment.
        /// </summary>
        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LineSegment.Point"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register(
                nameof(Point), 
                typeof(Point), 
                typeof(LineSegment), 
                new PropertyMetadata(new Point(), Point_Changed));

        private static void Point_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSegment segment = (LineSegment)d;
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

            // tell the context that there should be a line from the starting point to this point
            //context.lineTo((Point.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
            //               (Point.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            //Note: we replaced the code above with the one below because Bridge.NET has an issue when adding "0" to an Int64 (as of May 1st, 2020), so it is better to first multiply and then add, rather than the contrary:
            context.lineTo(Point.X * horizontalMultiplicator + xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication,
                           Point.Y * verticalMultiplicator  + yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication);

            return Point;
        }

        internal override Point GetMaxXY()
        {
            return new Point(Point.X, Point.X);
        }

        internal override Point GetMinMaxXY(ref double minX, 
                                            ref double maxX, 
                                            ref double minY, 
                                            ref double maxY, 
                                            Point startingPoint)
        {
            minX = Math.Min(minX, Point.X);
            maxX = Math.Max(maxX, Point.X);
            minY = Math.Min(minY, Point.Y);
            maxY = Math.Max(maxY, Point.Y);
            return Point;
        }

        #endregion

    }
}