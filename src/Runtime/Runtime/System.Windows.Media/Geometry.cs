
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

using System;

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides a base class for objects that define geometric shapes. <see cref="Geometry"/>
    /// objects can be used for clipping regions and as geometry definitions for rendering
    /// two-dimensional graphic data as a <see cref="Path"/>.
    /// </summary>
    public abstract class Geometry : DependencyObject
    {
        internal Geometry() { }

        /// <summary>
        /// Gets an empty geometry object.
        /// </summary>
        /// <returns>
        /// The empty geometry object.
        /// </returns>
        public static Geometry Empty => new PathGeometry();

        /// <summary>
        /// Identifies the <see cref="Transform"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransformProperty = 
            DependencyProperty.Register(
                nameof(Transform), 
                typeof(Transform), 
                typeof(Geometry), 
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the <see cref="Media.Transform"/> object applied to a <see cref="Geometry"/>.
        /// </summary>
        /// <returns>
        /// The transformation applied to the <see cref="Geometry"/>. Note that this
        /// value may be a single <see cref="Media.Transform"/> or a <see cref="TransformCollection"/>
        /// cast as a <see cref="Media.Transform"/>.
        /// </returns>
        public Transform Transform
        {
            get => (Transform)GetValue(TransformProperty);
            set => SetValue(TransformProperty, value);
        }

        /// <summary>
        /// Gets the standard tolerance used for polygonal approximation.
        /// </summary>
        /// <returns>
        /// The standard tolerance. The default value is 0.25.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static double StandardFlatteningTolerance { get; } = 0.25;

        /// <summary>
        /// Gets a <see cref="Rect"/> that specifies the axis-aligned bounding box of the
        /// <see cref="Geometry"/>.
        /// </summary>
        /// <returns>
        /// The axis-aligned bounding box of the <see cref="Geometry"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Rect Bounds => BoundsInternal;

        internal virtual Rect BoundsInternal => new Rect();

        internal event EventHandler<GeometryInvalidatedEventsArgs> Invalidated;

        internal static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Geometry geometry = (Geometry)d;
            geometry.RaisePathChanged();
        }

        internal void RaisePathChanged() => Invalidated?.Invoke(this, new GeometryInvalidatedEventsArgs(true, false));

        internal static void OnFillRuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Geometry geometry = (Geometry)d;
            geometry.RaiseFillRuleChanged();
        }

        private void RaiseFillRuleChanged() => Invalidated?.Invoke(this, new GeometryInvalidatedEventsArgs(false, true));

        internal abstract string ToPathData(IFormatProvider formatProvider);

        internal virtual FillRule GetFillRule() => FillRule.EvenOdd;
    }

    internal class GeometryInvalidatedEventsArgs : EventArgs
    {
        public GeometryInvalidatedEventsArgs(bool affectsMeasure, bool affectsFillRule)
        {
            AffectsMeasure = affectsMeasure;
            AffectsFillRule = affectsFillRule;
        }

        public bool AffectsMeasure { get; }

        public bool AffectsFillRule { get; }
    }
}