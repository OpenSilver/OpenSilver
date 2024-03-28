
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
using System.Diagnostics;
using System.Windows.Markup;
using OpenSilver.Internal;
using Stop = (double Offset, System.Windows.Media.Color Color);

namespace System.Windows.Media
{
    /// <summary>
    /// An abstract class that describes a gradient, composed of gradient stops. Classes
    /// that derive from <see cref="GradientBrush"/> describe different ways of
    /// interpreting gradient stops.
    /// </summary>
    [ContentProperty(nameof(GradientStops))]
    public class GradientBrush : Brush
    {
        private WeakEventListener<GradientBrush, GradientStopCollection, NotifyCollectionChangedEventArgs> _collectionChangedListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientBrush"/> class.
        /// </summary>
        protected GradientBrush() { }

        private protected GradientBrush(GradientBrush original)
            : base(original)
        {
            MappingMode = original.MappingMode;
            SpreadMethod = original.SpreadMethod;
            ColorInterpolationMode = original.ColorInterpolationMode;
            foreach (GradientStop stop in original.GradientStops.InternalItems)
            {
                GradientStops.Add(new GradientStop { Offset = stop.Offset, Color = stop.Color });
            }
        }

        /// <summary>
        /// Identifies the <see cref="GradientStops"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GradientStopsProperty =
            DependencyProperty.Register(
                nameof(GradientStops),
                typeof(GradientStopCollection),
                typeof(GradientBrush),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<GradientStop>(
                        static () => new GradientStopCollection(),
                        static (d, dp) =>
                        {
                            GradientBrush gb = (GradientBrush)d;
                            var collection = new GradientStopCollection();
                            gb.OnGradientStopsChanged(null, collection);
                            return collection;
                        }),
                    OnGradientStopsChanged,
                    CoerceGradientStops));

        /// <summary>
        /// Gets or sets the brush's gradient stops.
        /// </summary>
        /// <returns>
        /// A collection of the <see cref="GradientStop"/> objects associated with
        /// the brush, each of which specifies a color and an offset along the brush's gradient
        /// axis. The default is an empty <see cref="GradientStopCollection"/>.
        /// </returns>
        public GradientStopCollection GradientStops
        {
            get => (GradientStopCollection)GetValue(GradientStopsProperty);
            set => SetValueInternal(GradientStopsProperty, value);
        }

        private static void OnGradientStopsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GradientBrush brush = (GradientBrush)d;
            brush.OnGradientStopsChanged((GradientStopCollection)e.OldValue, (GradientStopCollection)e.NewValue);
            brush.RaiseChanged();
        }

        private void OnGradientStopCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RaiseChanged();

        private void OnGradientStopsChanged(GradientStopCollection oldStops, GradientStopCollection newStops)
        {
            oldStops?.SetOwner(null);

            if (_collectionChangedListener != null)
            {
                _collectionChangedListener.Detach();
                _collectionChangedListener = null;
            }

            if (newStops is not null)
            {
                newStops.SetOwner(this);

                _collectionChangedListener = new(this, newStops)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnGradientStopCollectionChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.CollectionChanged -= listener.OnEvent,
                };
                newStops.CollectionChanged += _collectionChangedListener.OnEvent;
            }
        }

        private static object CoerceGradientStops(DependencyObject d, object baseValue)
        {
            return baseValue ?? new GradientStopCollection();
        }

        /// <summary>
        /// Identifies the <see cref="MappingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MappingModeProperty =
            DependencyProperty.Register(
                nameof(MappingMode),
                typeof(BrushMappingMode),
                typeof(GradientBrush),
                new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox));

        /// <summary>
        /// Gets or sets a <see cref="BrushMappingMode"/> enumeration value that specifies
        /// whether the positioning coordinates of the gradient brush are absolute or relative
        /// to the output area.
        /// </summary>
        /// <returns>
        /// A <see cref="BrushMappingMode"/> value that specifies how to interpret the gradient 
        /// brush's positioning coordinates. The default is <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
        /// </returns>
        public BrushMappingMode MappingMode
        {
            get => (BrushMappingMode)GetValue(MappingModeProperty);
            set => SetValueInternal(MappingModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SpreadMethod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpreadMethodProperty =
            DependencyProperty.Register(
                nameof(SpreadMethod),
                typeof(GradientSpreadMethod),
                typeof(GradientBrush),
                new PropertyMetadata(GradientSpreadMethod.Pad));

        /// <summary>
        /// Gets or sets the type of spread method that specifies how to draw a gradient
        /// that starts or ends inside the bounds of the object to be painted.
        /// </summary>
        /// <returns>
        /// The type of spread method used to paint the gradient. The default is 
        /// <see cref="GradientSpreadMethod.Pad"/>.
        /// </returns>
        public GradientSpreadMethod SpreadMethod
        {
            get => (GradientSpreadMethod)GetValue(SpreadMethodProperty);
            set => SetValueInternal(SpreadMethodProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ColorInterpolationMode"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ColorInterpolationModeProperty =
            DependencyProperty.Register(
                nameof(ColorInterpolationMode),
                typeof(ColorInterpolationMode),
                typeof(GradientBrush),
                new PropertyMetadata(ColorInterpolationMode.SRgbLinearInterpolation));

        /// <summary>
        /// Gets or sets a <see cref="ColorInterpolationMode"/> enumeration value
        /// that specifies how the gradient's colors are interpolated.
        /// </summary>
        /// <returns>
        /// Specifies how the colors in a gradient are interpolated. The default is 
        /// <see cref="ColorInterpolationMode.SRgbLinearInterpolation"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public ColorInterpolationMode ColorInterpolationMode
        {
            get => (ColorInterpolationMode)GetValue(ColorInterpolationModeProperty);
            set => SetValueInternal(ColorInterpolationModeProperty, value);
        }

        internal IEnumerable<Stop> GetGradientStops()
        {
            Stop[] stops = GradientStops.GetSortedCollection();
            if (stops.Length == 0)
            {
                yield break;
            }

            if (stops.Length == 1)
            {
                yield return stops[0];
                yield break;
            }

            int i = 0;
            int firstStop = -1;
            while (i < stops.Length)
            {
                var stop = stops[i];
                if (stop.Offset >= 0) break;

                firstStop = i;
                i++;
            }

            if (firstStop >= 0)
            {
                if (i == stops.Length)
                {
                    // All stops have negative offset
                    yield return stops[firstStop];
                    yield break;
                }

                yield return (0, InterpolateGradientStopsArgbColor(stops[firstStop], stops[i], 0.0));
            }

            while (i < stops.Length)
            {
                var stop = stops[i];
                if (stop.Offset > 1) break;

                yield return stop;
                i++;
            }

            // No more stops, exit
            if (i == stops.Length)
            {
                yield break;
            }

            // At this points, all remaining stops have an offset > 1.
            // We just want to take the first one and interpolate the Color
            // if it is not the first stop.

            var finalStop = stops[i];
            if (i == 0)
            {
                // First relevant stop, no need to interpolate
                yield return finalStop;
                yield break;
            }

            yield return (1.0, InterpolateGradientStopsArgbColor(stops[i - 1], finalStop, 1.0));

            static Color InterpolateGradientStopsArgbColor(Stop from, Stop to, double midPoint)
            {
                Debug.Assert(from.Offset <= midPoint && to.Offset >= midPoint);

                Color fromColor = from.Color;
                Color toColor = to.Color;

                double value = (midPoint - from.Offset) / (to.Offset - from.Offset);

                return Color.FromArgb(
                    (byte)(fromColor.A + (toColor.A - fromColor.A) * value),
                    (byte)(fromColor.R + (toColor.R - fromColor.R) * value),
                    (byte)(fromColor.G + (toColor.G - fromColor.G) * value),
                    (byte)(fromColor.B + (toColor.B - fromColor.B) * value));
            }
        }

        internal static string ConvertBrushMappingModeToString(BrushMappingMode mappingMode)
            => mappingMode switch
            {
                BrushMappingMode.Absolute => "userSpaceOnUse",
                _ => "objectBoundingBox",
            };

        internal static string ConvertSpreadMethodToString(GradientSpreadMethod spreadMethod)
            => spreadMethod switch
            {
                GradientSpreadMethod.Reflect => "reflect",
                GradientSpreadMethod.Repeat => "repeat",
                _ => "pad",
            };
    }
}
