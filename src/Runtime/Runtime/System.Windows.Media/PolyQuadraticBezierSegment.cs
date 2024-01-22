
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

using OpenSilver.Internal;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a set of quadratic Bezier segments.
    /// </summary>
    [ContentProperty(nameof(Points))]
    public sealed class PolyQuadraticBezierSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyQuadraticBezierSegment"/> class.
        /// </summary>
        public PolyQuadraticBezierSegment() { }

        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(PolyQuadraticBezierSegment),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            PolyQuadraticBezierSegment segment = (PolyQuadraticBezierSegment)d;
                            var points = new PointCollection();
                            points.CollectionChanged += new NotifyCollectionChangedEventHandler(segment.OnPointsCollectionChanged);
                            return points;
                        }),
                    OnPointsChanged,
                    CoercePoints));

        /// <summary>
        /// Gets or sets the <see cref="PointCollection"/> that defines this <see cref="PolyQuadraticBezierSegment"/>
        /// object.
        /// </summary>
        /// <returns>
        /// A collection of points that defines the shape of this <see cref="PolyQuadraticBezierSegment"/>
        /// object. The default value is an empty collection.
        /// </returns>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValueInternal(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyQuadraticBezierSegment segment = (PolyQuadraticBezierSegment)d;
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
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#quadratic_b%C3%A9zier_curve
            var points = Points;

            if (points.Count % 2 != 0)
            {
                throw new InvalidOperationException("PolyQuadraticBezierSegment points must use pair points.");
            }

            yield return "Q";

            for (var i = 0; i < points.Count; i++)
            {
                yield return points[i].X.ToString(formatProvider);
                yield return points[i].Y.ToString(formatProvider);
            }
        }
    }
}
