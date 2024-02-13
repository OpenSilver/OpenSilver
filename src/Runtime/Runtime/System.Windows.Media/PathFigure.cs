
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

using System.Collections.Specialized;
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a subsection of a geometry, a single connected series of two-dimensional
    /// geometric segments.
    /// </summary>
    [ContentProperty(nameof(Segments))]
    public sealed class PathFigure : DependencyObject
    {
        private Geometry _parentGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigure"/> class.
        /// </summary>
        public PathFigure() { }

        /// <summary>
        /// Identifies the <see cref="IsClosed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register(
                nameof(IsClosed),
                typeof(bool),
                typeof(PathFigure),
                new PropertyMetadata(false, PropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether this figure's first and last segments
        /// are connected.
        /// </summary>
        /// <returns>
        /// true if the first and last segments of the figure are connected; otherwise, false.
        /// The default is false.
        /// </returns>
        public bool IsClosed
        {
            get => (bool)GetValue(IsClosedProperty);
            set => SetValueInternal(IsClosedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsFilled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(
                nameof(IsFilled),
                typeof(bool),
                typeof(PathFigure),
                new PropertyMetadata(false, PropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the contained area of this <see cref="PathFigure"/>
        /// is to be used for hit-testing, rendering, and clipping.
        /// </summary>
        /// <returns>
        /// true if the contained area of this <see cref="PathFigure"/> is to be used
        /// for hit-testing, rendering, and clipping; otherwise, false. The default is true.
        /// </returns>
        public bool IsFilled
        {
            get => (bool)GetValue(IsFilledProperty);
            set => SetValueInternal(IsFilledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Segments" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register(
                nameof(Segments),
                typeof(PathSegmentCollection),
                typeof(PathFigure),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<PathSegment>(
                        static () => new PathSegmentCollection(),
                        static (d, dp) =>
                        {
                            PathFigure figure = (PathFigure)d;
                            var segments = new PathSegmentCollection();
                            segments.SetParentGeometry(figure._parentGeometry);
                            segments.CollectionChanged += new NotifyCollectionChangedEventHandler(figure.OnSegmentsCollectionChanged);
                            return segments;
                        }),
                    OnSegmentsChanged,
                    CoerceSegments));

        /// <summary>
        /// Gets or sets the collection of segments that define the shape of this <see cref="PathFigure"/>
        /// object.
        /// </summary>
        /// <returns>
        /// The collection of segments that define the shape of this <see cref="PathFigure"/>
        /// object. The default is an empty collection.
        /// </returns>
        public PathSegmentCollection Segments
        {
            get => (PathSegmentCollection)GetValue(SegmentsProperty);
            set => SetValueInternal(SegmentsProperty, value);
        }

        private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathFigure figure = (PathFigure)d;
            if (e.OldValue is PathSegmentCollection oldSegments)
            {
                oldSegments.SetParentGeometry(null);
                oldSegments.CollectionChanged -= new NotifyCollectionChangedEventHandler(figure.OnSegmentsCollectionChanged);
            }
            if (e.NewValue is PathSegmentCollection newSegments)
            {
                newSegments.SetParentGeometry(figure._parentGeometry);
                newSegments.CollectionChanged += new NotifyCollectionChangedEventHandler(figure.OnSegmentsCollectionChanged);
            }
            
            PropertyChanged(d, e);
        }

        private static object CoerceSegments(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PathSegmentCollection();
        }

        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => InvalidateParentGeometry();

        /// <summary>
        /// Identifies the <see cref="StartPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(
                nameof(StartPoint),
                typeof(Point),
                typeof(PathFigure),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="Point"/> where the <see cref="PathFigure"/> begins.
        /// </summary>
        /// <returns>
        /// The <see cref="Point"/> where the <see cref="PathFigure"/> begins. The
        /// default is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValueInternal(StartPointProperty, value);
        }

        internal void SetParentGeometry(Geometry geometry)
        {
            _parentGeometry = geometry;
            foreach (var segment in Segments.InternalItems)
            {
                segment.SetParentGeometry(geometry);
            }
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathFigure)d).InvalidateParentGeometry();
        }

        private void InvalidateParentGeometry() => _parentGeometry?.RaisePathChanged();
    }
}