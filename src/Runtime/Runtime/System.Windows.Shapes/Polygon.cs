
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
/// Draws a polygon, which is a connected series of lines that form a closed shape.
/// </summary>
public sealed class Polygon : Shape
{
    private WeakEventToken _weakEventToken;

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
                        points.Changed += Promote;
                        return points;

                        void Promote(object sender, EventArgs e)
                        {
                            points.Changed -= Promote;

                            // If someone else hasn't already written a local local value,
                            // promote the default value to local.
                            if (polygon.ReadLocalValue(PointsProperty) == DependencyProperty.UnsetValue)
                            {
                                polygon.SetMutableDefaultValue(PointsProperty, points);
                            }

                            PropertyMetadata metadata = PointsProperty.GetMetadata(polygon.DependencyObjectType);

                            // Remove this value from the DefaultValue cache so we stop
                            // handing it out as the default value now that it has changed.
                            metadata.ClearCachedDefaultValue(polygon, PointsProperty);
                        }
                    }),
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPointsChanged,
                CoercePoints)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Polygon)d).SetSvgPointsAttribute((PointCollection)newValue),
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
        Polygon polygon = (Polygon)d;
        PointCollection points = (PointCollection)e.NewValue;

        if (polygon._weakEventToken is not null)
        {
            polygon._weakEventToken.Dispose();
            polygon._weakEventToken = null;
        }

        if (points is not null)
        {
            polygon._weakEventToken = WeakEvent.Subscribe<Polygon, PointCollection, EventArgs>(
                polygon,
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