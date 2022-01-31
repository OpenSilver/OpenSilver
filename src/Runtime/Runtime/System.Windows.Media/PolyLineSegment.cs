

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
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a set of line segments defined by a Point collection with each
    /// Point specifying the end point of a line segment.
    /// </summary>
    [ContentProperty("Points")]
    public sealed partial class PolyLineSegment : PathSegment
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PolyLineSegment class.
        /// </summary>
        public PolyLineSegment()
        {
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the collection of Point values that defines this PolyLineSegment
        /// object.
        /// </summary>
        public PointCollection Points
        {
            get 
            {
                PointCollection points = (PointCollection)GetValue(PointsProperty);

                if (points == null)
                {
                    points = new PointCollection();
                    SetValue(PointsProperty, points);
                }

                return points;
            }

            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PolyLineSegment.Points"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points), 
                typeof(PointCollection), 
                typeof(PolyLineSegment), 
                new PropertyMetadata(null, Points_Changed));

        private static void Points_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: find a way to know when the points changed in the collection
            PolyLineSegment segment = (PolyLineSegment)d;
            if (segment.ParentPath != null)
            {
                segment.ParentPath.ScheduleRedraw();
            }
        }

        #endregion

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
            //todo: the size of the canvas
            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            foreach (Point point in Points)
            {
                // tell the context that there should be a line from the starting point to this point
                context.lineTo(
                    (point.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                    (point.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            }
            return Points.Count == 0 ? previousLastPoint : Points[Points.Count - 1];
        }

        internal override Point GetMaxXY()
        {
            Point currentMaxXY = new Point(double.NegativeInfinity, double.NegativeInfinity);
            foreach (Point point in Points)
            {
                if (point.X > currentMaxXY.X)
                {
                    currentMaxXY.X = point.X;
                }
                if (point.Y > currentMaxXY.Y)
                {
                    currentMaxXY.Y = point.Y;
                }
            }
            return currentMaxXY;
        }

        internal override Point GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, Point startingPoint)
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

    }
}


