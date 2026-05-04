
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
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media;

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
                        points.Changed += new EventHandler(segment.OnPointsCollectionChanged);
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
            oldPoints.Changed -= new EventHandler(segment.OnPointsCollectionChanged);
        }
        if (e.NewValue is PointCollection newPoints)
        {
            newPoints.Changed += new EventHandler(segment.OnPointsCollectionChanged);
        }

        PropertyChanged(d, e);
    }

    private static object CoercePoints(DependencyObject d, object baseValue)
    {
        return baseValue ?? new PointCollection();
    }

    private void OnPointsCollectionChanged(object sender, EventArgs e) => InvalidateParentGeometry();

    internal override void SerializeData(StreamGeometryContext context, Matrix transform, ref Point current)
    {
        List<Point> points = Points.InternalItems;

        int count = points.Count;

        if (count == 0)
        {
            return;
        }

        if (points.Count >= 3)
        {
            current = points[count - 1 - (count % 3)];
        }

        if (!transform.IsIdentity)
        {
            var copy = new List<Point>(points.Count);
            foreach (Point p in points)
            {
                copy.Add(p * transform);
            }

            points = copy;
        }

        context.PolyBezierTo(points, IsStroked, IsSmoothJoin);
    }

    internal override bool IsCurved() => !IsEmpty();

    private bool IsEmpty() => Points.InternalItems.Count < 3;
}
