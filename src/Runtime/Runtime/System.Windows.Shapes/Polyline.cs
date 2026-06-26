
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
using OpenSilver.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace System.Windows.Shapes;

/// <summary>
/// Draws a series of connected straight lines.
/// </summary>
public sealed class Polyline : Shape
{
    private WeakEventToken _weakEventToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Polyline"/> class.
    /// </summary>
    public Polyline() { }

    /// <inheritdoc />
    protected override Geometry DefiningGeometry
    {
        get
        {
            List<Point> pointCollection = Points.InternalItems;
            var pathFigure = new PathFigure();

            if (pointCollection.Count > 0)
            {
                pathFigure.StartPoint = pointCollection[0];

                if (pointCollection.Count > 1)
                {
                    var points = new PointCollection(pointCollection.Count - 1);

                    for (int i = 1; i < pointCollection.Count; i++)
                    {
                        points.Add(pointCollection[i]);
                    }

                    pathFigure.Segments.Add(new PolyLineSegment
                    {
                        Points = points,
                        IsStroked = true,
                    });
                }
            }

            var polylineGeometry = new PathGeometry();
            polylineGeometry.Figures.Add(pathFigure);

            // Set FillRule
            polylineGeometry.FillRule = FillRule;

            if (polylineGeometry.Bounds == Rect.Empty)
            {
                return Geometry.Empty;
            }
            else
            {
                return polylineGeometry;
            }
        }
    }

    /// <summary>
    /// Identifies the <see cref="FillRule"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FillRuleProperty =
        DependencyProperty.Register(
            nameof(FillRule),
            typeof(FillRule),
            typeof(Polyline),
            new FrameworkPropertyMetadata(FillRule.EvenOdd)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Polyline)d).SetFillRuleAttribute((FillRule)newValue),
            },
            ValidateEnums.IsFillRuleValid);

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
                        points.Changed += Promote;
                        return points;

                        void Promote(object sender, EventArgs e)
                        {
                            points.Changed -= Promote;

                            // If someone else hasn't already written a local local value,
                            // promote the default value to local.
                            if (polyline.ReadLocalValue(PointsProperty) == DependencyProperty.UnsetValue)
                            {
                                polyline.SetMutableDefaultValue(PointsProperty, points);
                            }

                            PropertyMetadata metadata = PointsProperty.GetMetadata(polyline.DependencyObjectType);

                            // Remove this value from the DefaultValue cache so we stop
                            // handing it out as the default value now that it has changed.
                            metadata.ClearCachedDefaultValue(polyline, PointsProperty);
                        }
                    }),
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPointsChanged,
                CoercePoints)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Polyline)d).SetSvgPointsAttribute((PointCollection)newValue),
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
        Polyline polyline = (Polyline)d;
        PointCollection points = (PointCollection)e.NewValue;

        if (polyline._weakEventToken is not null)
        {
            polyline._weakEventToken.Dispose();
            polyline._weakEventToken = null;
        }

        if (points is not null)
        {
            polyline._weakEventToken = WeakEvent.Subscribe<Polyline, PointCollection, EventArgs>(
                polyline,
                points,
                static (instance, sender, args) => instance.OnPointsCollectionChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }
    }

    private static object CoercePoints(DependencyObject d, object baseValue)
    {
        return baseValue ?? new PointCollection();
    }

    private void OnPointsCollectionChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            SetSvgPointsAttribute((PointCollection)sender);
        }

        InvalidateMeasure();
    }

    private void SetSvgPointsAttribute(PointCollection points)
    {
        if (points is null || points.InternalCount == 0)
        {
            RemoveSvgAttribute("points");
            return;
        }

        SetSvgAttribute("points",
            string.Join(" ",
                points.InternalItems.Select(static p => $"{Math.Round(p.X, 2).ToInvariantString()},{Math.Round(p.Y, 2).ToInvariantString()}")));
    }

    internal sealed override string SvgTagName => "polyline";

    internal sealed override bool UseDefaultRendering => false;

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