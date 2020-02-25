﻿
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
    // Summary:
    //     Represents a set of quadratic Bezier segments.
    [ContentProperty("Points")]
    public sealed partial class PolyQuadraticBezierSegment : PathSegment
    {
        ///// <summary>
        ///// Initializes a new instance of the PolyQuadraticBezierSegment class.
        ///// </summary>
        //public PolyQuadraticBezierSegment();

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
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(PolyQuadraticBezierSegment), new PropertyMetadata(null, Points_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Points_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: find a way to know when the points changed in the collection
            PolyQuadraticBezierSegment segment = (PolyQuadraticBezierSegment)d;
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
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
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
                context.quadraticCurveTo((controlPoint1X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (controlPoint1Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                    (endPointX + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (endPointY + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication); // tell the context that there should be a cubic bezier curve from the starting point to this point, with the two previous points as control points.
            }
            return lastPoint;
        }

        internal override Point GetMaxXY() //todo: make this give the size of the actual curve, not the control points.
        {
            Point currentMax = new Point(0, 0);
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
