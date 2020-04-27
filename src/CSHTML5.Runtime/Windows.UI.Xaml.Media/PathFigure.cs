

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
        #region Data

        private Path _parentPath = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PathFigure class.
        /// </summary>
        public PathFigure()
        {

        }

        #endregion

        #region Dependency Properties

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
            DependencyProperty.Register("IsClosed", typeof(bool), typeof(PathFigure), new PropertyMetadata(false, IsClosed_Changed));

        private static void IsClosed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (figure._parentPath != null)
            {
                figure._parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("IsFilled", typeof(bool), typeof(PathFigure), new PropertyMetadata(false, IsFilled_Changed));

        private static void IsFilled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //note: this doesn't currently work, we need extra work to make it work (see DefineInCanvas in this class).
            PathFigure figure = (PathFigure)d;
            if (figure._parentPath != null)
            {
                figure._parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("Segments", typeof(PathSegmentCollection), typeof(PathFigure), new PropertyMetadata(null, Segments_Changed));

        private static void Segments_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (null != e.OldValue)
            {
                ((PathSegmentCollection)e.OldValue).SetParentPath(null);
            }
            if (figure._parentPath != null)
            {
                if (null != e.NewValue)
                {
                    ((PathSegmentCollection)e.NewValue).SetParentPath(figure._parentPath);
                }

                figure._parentPath.ScheduleRedraw();
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
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(PathFigure), new PropertyMetadata(new Point(), StartPoint_Changed));

        private static void StartPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (figure._parentPath != null)
            {
                figure._parentPath.ScheduleRedraw();
            }
        }

        #endregion

        #region Internal Methods

        internal void SetParentPath(Path path)
        {
            if (_parentPath != path)
            {
                _parentPath = path;
                Segments.SetParentPath(path);
            }
        }

        internal void DefineInCanvas(double xOffsetToApplyBeforeMultiplication, 
                                     double yOffsetToApplyBeforeMultiplication, 
                                     double xOffsetToApplyAfterMultiplication, 
                                     double yOffsetToApplyAfterMultiplication, 
                                     double horizontalMultiplicator, 
                                     double verticalMultiplicator, 
                                     object canvasDomElement, 
                                     double strokeThickness, 
                                     Size shapeActualSize)
        {
            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            // todo: In order to support IsFilled, add a call to context.beginPath() here 
            // (instead the call to beginPath() that is located in "Path.cs") and handle 
            // the filling here (in the PathFigure) instead of in the Redraw of the Path.

            Point segmentStartingPosition = new Point(StartPoint.X, StartPoint.Y);
            
            // tell the context that there should be a line from the starting point to this point
            context.moveTo((StartPoint.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, 
                           (StartPoint.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            foreach (PathSegment segment in Segments)
            {
                segmentStartingPosition = segment.DefineInCanvas(xOffsetToApplyBeforeMultiplication, 
                                                                 yOffsetToApplyBeforeMultiplication, 
                                                                 xOffsetToApplyAfterMultiplication, 
                                                                 yOffsetToApplyAfterMultiplication, 
                                                                 horizontalMultiplicator, 
                                                                 verticalMultiplicator, 
                                                                 canvasDomElement, 
                                                                 segmentStartingPosition);
            }
            if (IsClosed) // we close the figure:
            {
                // tell the context that there should be a line from the starting point to this point
                context.closePath();
            }
        }

        internal void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            minX = Math.Min(minX, StartPoint.X);
            maxX = Math.Max(maxX, StartPoint.X);
            minY = Math.Min(minY, StartPoint.Y);
            maxY = Math.Max(maxY, StartPoint.Y);

            Point segmentStartingPosition = new Point(StartPoint.X, StartPoint.Y);
            foreach (PathSegment segment in Segments)
            {
                segmentStartingPosition = segment.GetMinMaxXY(ref minX, ref maxX, ref minY, ref maxY, segmentStartingPosition);
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

        #endregion
    }
}