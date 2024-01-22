
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
    /// Represents one or more cubic Bezier curves.
    /// </summary>
    [ContentProperty(nameof(Points))]
    public sealed class PolyBezierSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyBezierSegment"/> class.
        /// </summary>
        public PolyBezierSegment() { }

        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(PolyBezierSegment),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            PolyBezierSegment segment = (PolyBezierSegment)d;
                            var points = new PointCollection();
                            points.CollectionChanged += new NotifyCollectionChangedEventHandler(segment.OnPointsCollectionChanged);
                            return points;
                        }),
                    OnPointsChanged,
                    CoercePoints));

        /// <summary>
        /// Gets or sets the <see cref="PointCollection"/> that defines this <see cref="PolyBezierSegment"/>
        /// object.
        /// </summary>
        /// <returns>
        /// The collection of points that defines this <see cref="PolyBezierSegment"/> object.
        /// </returns>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValueInternal(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyBezierSegment segment = (PolyBezierSegment)d;
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
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#cubic_b%C3%A9zier_curve
            var points = Points;

            if (points.Count % 3 != 0)
            {
                throw new InvalidOperationException("PolyBezierSegment points must use triplet points.");
            }

            yield return $"C";

            for (var i = 0; i < points.Count; i++)
            {
                yield return points[i].X.ToString(formatProvider);
                yield return points[i].Y.ToString(formatProvider);
            }
        }
    }
}

