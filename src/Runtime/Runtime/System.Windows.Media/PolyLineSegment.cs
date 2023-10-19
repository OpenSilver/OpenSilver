
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a set of line segments defined by a <see cref="PointCollection"/>
    /// with each <see cref="Point"/> specifying the end point of a line segment.
    /// </summary>
    [ContentProperty(nameof(Points))]
    public sealed class PolyLineSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineSegment"/> class.
        /// </summary>
        public PolyLineSegment() { }

        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(PolyLineSegment),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            PolyLineSegment segment = (PolyLineSegment)d;
                            var points = new PointCollection();
                            points.CollectionChanged += new NotifyCollectionChangedEventHandler(segment.OnPointsCollectionChanged);
                            return points;
                        }),
                    OnPointsChanged,
                    CoercePoints));

        /// <summary>
        /// Gets or sets the collection of <see cref="Point"/> values that defines this
        /// <see cref="PolyLineSegment"/> object.
        /// </summary>
        /// <return>
        /// The points that define this <see cref="PolyLineSegment"/> object.
        /// </return>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyLineSegment segment = (PolyLineSegment)d;
            if (e.OldValue is PointCollection oldPoints)
            {
                oldPoints.CollectionChanged -= new NotifyCollectionChangedEventHandler(segment.OnPointsCollectionChanged);
            }
            if (e.NewValue is PointCollection newPoints)
            {
                newPoints.CollectionChanged += new NotifyCollectionChangedEventHandler(segment.OnPointsCollectionChanged);
            }

            PropertyChanged(d, e);
        }

        private static object CoercePoints(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PointCollection();
        }

        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => InvalidateParentGeometry();

        internal override IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#lineto_path_commands

            yield return "L";

            var points = Points;

            for (var i = 0; i < points.Count; i++)
            {
                yield return points[i].X.ToString(formatProvider);
                yield return points[i].Y.ToString(formatProvider);
            }
        }
    }
}


