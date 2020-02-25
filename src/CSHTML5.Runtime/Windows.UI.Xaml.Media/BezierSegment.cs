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
            DependencyProperty.Register("Point1", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point1_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Point1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
            {
                segment.INTERNAL_parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("Point2", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point2_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Point2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
            {
                segment.INTERNAL_parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("Point3", typeof(Point), typeof(BezierSegment), new PropertyMetadata(new Point(), Point3_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Point3_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierSegment segment = (BezierSegment)d;
            if (e.NewValue != e.OldValue && segment.INTERNAL_parentPath != null && segment.INTERNAL_parentPath._isLoaded)
            {
                segment.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, Point previousLastPoint)
        {
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            context.bezierCurveTo(
                (Point1.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (Point1.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                (Point2.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (Point2.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication,
                (Point3.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (Point3.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication); // tell the context that there should be a cubic bezier curve from the starting point to this point, with the two previous points as control points.
            return Point3;
        }

        internal override Point GetMaxXY() //todo: make this give the size of the actual curve, not the control points.
        {
            double maxX = Point1.X > Point2.X ? Point1.X : Point2.X;
            double maxY = Point1.Y > Point2.Y ? Point1.Y : Point2.Y;
            if (Point3.X > maxX)
            {
                maxX = Point3.X;
            }
            if (Point3.Y > maxY)
            {
                maxY = Point3.Y;
            }
            return new Point(maxX, maxY);
        }

        internal override Point GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, Point startingPoint)
        {
            if (minX > Point1.X)
            {
                minX = Point1.X;
            }
            if (minX > Point2.X)
            {
                minX = Point2.X;
            }
            if (minX > Point3.X)
            {
                minX = Point3.X;
            }
            if (maxX < Point1.X)
            {
                maxX = Point1.X;
            }
            if (maxX < Point2.X)
            {
                maxX = Point2.X;
            }
            if (maxX < Point3.X)
            {
                maxX = Point3.X;
            }
            if (minY > Point1.Y)
            {
                minY = Point1.Y;
            }
            if (minY > Point2.Y)
            {
                minY = Point2.Y;
            }
            if (minY > Point3.Y)
            {
                minY = Point3.Y;
            }
            if (maxY < Point1.Y)
            {
                maxY = Point1.Y;
            }
            if (maxY < Point2.Y)
            {
                maxY = Point2.Y;
            }
            if (maxY < Point3.Y)
            {
                maxY = Point3.Y;
            }
            return Point3;
        }

    }
}
