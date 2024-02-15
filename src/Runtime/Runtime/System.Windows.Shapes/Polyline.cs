
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

using System.Linq;
using System.Collections.Specialized;
using System.Windows.Media;
using OpenSilver.Internal;
using System.Collections.Generic;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Draws a series of connected straight lines.
    /// </summary>
    public sealed class Polyline : Shape
    {
        private WeakEventListener<Polyline, PointCollection, NotifyCollectionChangedEventArgs> _pointsCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polyline"/> class.
        /// </summary>
        public Polyline() { }

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule),
                typeof(FillRule),
                typeof(Polyline),
                new PropertyMetadata(FillRule.EvenOdd)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Polyline)d).SetFillRuleAttribute((FillRule)newValue),
                });

        /// <summary>
        /// Gets or sets a value that specifies how the interior fill of the shape is determined.
        /// </summary>
        /// <returns>
        /// A value of the enumeration that specifies the fill behavior. The default is <see cref="FillRule.EvenOdd"/>.
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
                typeof(Polyline),
                new FrameworkPropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            Polyline polyline = (Polyline)d;
                            var points = new PointCollection();
                            polyline.OnPointsChanged(null, points);
                            return points;
                        }),
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnPointsChanged,
                    CoercePoints)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Polyline polyline = (Polyline)d;
                        if (newValue is PointCollection points)
                        {
                            polyline.SetSvgAttribute(
                                "points",
                                string.Join(" ",
                                    points.InternalItems.Select(static p => $"{Math.Round(p.X, 2).ToInvariantString()},{Math.Round(p.Y, 2).ToInvariantString()}")));
                        }
                        else
                        {
                            polyline.RemoveSvgAttribute("points");
                        }
                    },
                });

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the <see cref="Polyline"/>.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Point"/> structures that describe the vertex points
        /// of the <see cref="Polyline"/>. The default is null.
        /// </returns>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValueInternal(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Polyline)d).OnPointsChanged((PointCollection)e.OldValue, (PointCollection)e.NewValue);
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

        internal sealed override string SvgTagName => "polyline";

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