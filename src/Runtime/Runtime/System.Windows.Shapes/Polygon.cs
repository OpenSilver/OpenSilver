
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
using System.Linq;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Draws a polygon, which is a connected series of lines that form a closed shape.
    /// </summary>
    public sealed class Polygon : Shape
    {
        private WeakEventListener<Polygon, PointCollection, NotifyCollectionChangedEventArgs> _pointsCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon() { }

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule),
                typeof(FillRule),
                typeof(Polygon),
                new PropertyMetadata(FillRule.EvenOdd)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Polygon)d).SetFillRuleAttribute((FillRule)newValue),
                });

        /// <summary>
        /// Gets or sets a value that specifies how the interior fill of the shape is determined.
        /// </summary>
        /// <returns>
        /// A value of the enumeration. The default is <see cref="FillRule.EvenOdd"/>.
        /// </returns>
        public FillRule FillRule
        {
            get => (FillRule)GetValue(FillRuleProperty);
            set => SetValueInternal(FillRuleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(Polygon),
                new FrameworkPropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            Polygon polygon = (Polygon)d;
                            var points = new PointCollection();
                            polygon.OnPointsChanged(null, points);
                            return points;
                        }),
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnPointsChanged,
                    CoercePoints)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Polygon polygon = (Polygon)d;
                        if (newValue is PointCollection points)
                        {
                            polygon.SetSvgAttribute(
                                "points",
                                string.Join(" ",
                                    points.InternalItems.Select(static p => $"{Math.Round(p.X, 2).ToInvariantString()},{Math.Round(p.Y, 2).ToInvariantString()}")));
                        }
                        else
                        {
                            polygon.RemoveSvgAttribute("points");
                        }
                    },
                });

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the polygon.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Point"/> structures that describes the vertex points
        /// of the polygon. The default is null. The value can be expressed as a string as
        /// described in "pointSet Grammar" below.
        /// </returns>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValueInternal(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Polygon)d).OnPointsChanged((PointCollection)e.OldValue, (PointCollection)e.NewValue);
        }

        private static object CoercePoints(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PointCollection();
        }

        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => InvalidateMeasure();

        private void OnPointsChanged(PointCollection oldPoints, PointCollection newPoints)
        {
            if (_pointsCollectionChanged is not null)
            {
                _pointsCollectionChanged.Detach();
                _pointsCollectionChanged = null;
            }

            if (newPoints is not null)
            {
                _pointsCollectionChanged = new(this, newPoints)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnPointsCollectionChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.CollectionChanged -= listener.OnEvent,
                };
                newPoints.CollectionChanged += _pointsCollectionChanged.OnEvent;
            }
        }

        internal sealed override string SvgTagName => "polygon";

        /// <summary>
        /// Get the natural size of the geometry that defines this shape
        /// </summary>
        internal sealed override Size GetNaturalSize()
        {
            Rect bounds = GetDefiningGeometryBounds();
            double margin = Math.Ceiling(GetStrokeThickness() / 2);
            return new Size(Math.Max(bounds.Right + margin, 0), Math.Max(bounds.Bottom + margin, 0));
        }

        /// <summary>
        /// Get the bonds of the geometry that defines this shape
        /// </summary>
        internal sealed override Rect GetDefiningGeometryBounds()
        {
            List<Point> points = Points.InternalItems;
            if (points.Count == 0)
            {
                return new Rect();
            }

            Point startPoint = points[0];

            double minX = startPoint.X;
            double minY = startPoint.Y;
            double maxX = startPoint.X;
            double maxY = startPoint.Y;

            for (int i = 1; i < points.Count; i++)
            {
                Point p = points[i];

                if (p.X < minX)
                {
                    minX = p.X;
                }
                else if (p.X > maxX)
                {
                    maxX = p.X;
                }

                if (p.Y < minY)
                {
                    minY = p.Y;
                }
                else if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}