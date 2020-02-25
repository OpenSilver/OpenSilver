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
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a subsection of a geometry, a single connected series of two-dimensional
    /// geometric segments.
    /// </summary>
    [ContentProperty("Segments")]
    public sealed partial class PathFigure : DependencyObject
    {
        internal Path INTERNAL_parentPath = null;
        internal void SetParentPath(Path path)
        {
            INTERNAL_parentPath = path;
            Segments.SetParentPath(path);
        }

        ///// <summary>
        ///// Initializes a new instance of the PathFigure class.
        ///// </summary>
        //public PathFigure();

        /// <summary>
        /// Gets or sets a value that indicates whether this figure's first and last
        /// segments are connected.
        /// </summary>
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }
        /// <summary>
        /// Identifies the IsClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register("IsClosed", typeof(bool), typeof(PathFigure), new PropertyMetadata(false, IsClosed_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void IsClosed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (e.NewValue != e.OldValue && figure.INTERNAL_parentPath != null && figure.INTERNAL_parentPath._isLoaded)
            {
                figure.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the contained area of this PathFigure
        /// is to be used for hit-testing, rendering, and clipping.
        /// </summary>
        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(IsFilledProperty, value); }
        }
        /// <summary>
        /// Identifies the IsFilled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register("IsFilled", typeof(bool), typeof(PathFigure), new PropertyMetadata(false, IsFilled_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void IsFilled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //note: this doesn't currently work, we need extra work to make it work (see DefineInCanvas in this class).
            PathFigure figure = (PathFigure)d;
            if (e.NewValue != e.OldValue && figure.INTERNAL_parentPath != null && figure.INTERNAL_parentPath._isLoaded)
            {
                figure.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the collection of segments that define the shape of this PathFigure
        /// object.
        /// </summary>
        public PathSegmentCollection Segments
        {
            get
            {
                PathSegmentCollection collection = (PathSegmentCollection)GetValue(SegmentsProperty);
                if (collection == null)
                {
                    collection = new PathSegmentCollection();
                    SetValue(SegmentsProperty, collection);
                }
                return collection;
            }
            set { SetValue(SegmentsProperty, value); }
        }
        /// <summary>
        /// Identifies the Segments dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(PathSegmentCollection), typeof(PathFigure), new PropertyMetadata(null, Segments_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Segments_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            PathSegmentCollection oldCollection = (PathSegmentCollection)e.OldValue;
            PathSegmentCollection newCollection = (PathSegmentCollection)e.NewValue;
            if (oldCollection != null)
            {
                oldCollection.SetParentPath(null);
            }
            if (figure.INTERNAL_parentPath != null && figure.INTERNAL_parentPath._isLoaded)
            {
                if (newCollection != null)
                {
                    newCollection.SetParentPath(figure.INTERNAL_parentPath);
                }

                figure.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        /// <summary>
        /// Gets or sets the Point where the PathFigure begins.
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }
        /// <summary>
        /// Identifies the StartPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(PathFigure), new PropertyMetadata(new Point(), StartPoint_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void StartPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (e.NewValue != e.OldValue && figure.INTERNAL_parentPath != null && figure.INTERNAL_parentPath._isLoaded)
            {
                figure.INTERNAL_parentPath.ScheduleRedraw();
            }
        }


        internal void DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, double strokeThickness, Size shapeActualSize)
        {
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            //todo: In order to support IsFilled, add a call to context.beginPath() here (instead the call to beginPath() that is located in "Path.cs") and handle the filling here (in the PathFigure) instead of in the Redraw of the Path.
            Point segmentStartingPosition = new Point(StartPoint.X, StartPoint.Y);
            context.moveTo((StartPoint.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, (StartPoint.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication); // tell the context that there should be a line from the starting point to this point
            foreach (PathSegment segment in Segments)
            {
                if (segment is ArcSegment)
                {
                    ((ArcSegment)segment).UpdateStartPosition(segmentStartingPosition);
                    ((ArcSegment)segment).UpdateStrokeThickness(strokeThickness);
                }
                segmentStartingPosition = segment.DefineInCanvas(xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, horizontalMultiplicator, verticalMultiplicator, canvasDomElement, segmentStartingPosition);
            }
            if (IsClosed) //we close the figure:
            {
                context.closePath(); // tell the context that there should be a line from the starting point to this point
            }
        }

        internal Point GetMaxXY()
        {
            Point globalMax = new Point();
            foreach (PathSegment segment in Segments)
            {
                Point segmentMaxXY = segment.GetMaxXY();
                if (segmentMaxXY.X > globalMax.X)
                {
                    globalMax.X = segmentMaxXY.X;
                }
                if (segmentMaxXY.Y > globalMax.Y)
                {
                    globalMax.Y = segmentMaxXY.Y;
                }
            }
            return globalMax;
        }

        internal void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, double strokeThickness)
        {
            if (minX > StartPoint.X)
            {
                minX = StartPoint.X;
            }
            if (maxX < StartPoint.X)
            {
                maxX = StartPoint.X;
            }
            if (minY > StartPoint.Y)
            {
                minY = StartPoint.Y;
            }
            if (maxY < StartPoint.Y)
            {
                maxY = StartPoint.Y;
            }

            Point segmentStartingPosition = new Point(StartPoint.X, StartPoint.Y);
            foreach (PathSegment segment in Segments)
            {
                if (segment is ArcSegment)
                {
                    ((ArcSegment)segment).UpdateStartPosition(segmentStartingPosition);
                    ((ArcSegment)segment).UpdateStrokeThickness(strokeThickness);
                    
                }
                segmentStartingPosition = segment.GetMinMaxXY(ref minX, ref maxX, ref minY, ref maxY, segmentStartingPosition);
            }
        }
    }
}