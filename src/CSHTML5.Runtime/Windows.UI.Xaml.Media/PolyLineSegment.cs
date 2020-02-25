
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
using System.Windows.Markup;
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
    /// Represents a set of line segments defined by a Point collection with each
    /// Point specifying the end point of a line segment.
    /// </summary>
    [ContentProperty("Points")]
    public sealed partial class PolyLineSegment : PathSegment
    {
        ///// <summary>
        ///// Initializes a new instance of the PolyLineSegment class.
        ///// </summary>
        //public PolyLineSegment();

        /// <summary>
        /// Gets or sets the collection of Point values that defines this PolyLineSegment
        /// object.
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(PolyLineSegment), new PropertyMetadata(null, Points_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Points_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: find a way to know when the points changed in the collection
            PolyLineSegment segment = (PolyLineSegment)d;
            PointCollection oldCollection = (PointCollection)e.OldValue;
            PointCollection newCollection = (PointCollection)e.NewValue;
            if (oldCollection != newCollection)
            {
                if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
                {
                    segment.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }

        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, Point previousLastPoint)
        {
            //todo: the size of the canvas
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            Point lastPoint = previousLastPoint;
            foreach (Point point in Points)
            {
                context.lineTo((point.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (point.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication); // tell the context that there should be a line from the starting point to this point
                lastPoint = point;
            }
            return lastPoint;
        }

        internal override Point GetMaxXY()
        {
            Point currentMaxXY = new Point();
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
            Point lastPoint = startingPoint;
            foreach (Point point in Points)
            {
                if (minX > point.X)
                {
                    minX = point.X;
                }
                if (maxX < point.X)
                {
                    maxX = point.X;
                }
                if (minY > point.Y)
                {
                    minY = point.Y;
                }
                if (maxY < point.Y)
                {
                    maxY = point.Y;
                }
                lastPoint = point;
            }
            return lastPoint;
        }

    }
}


