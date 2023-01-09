
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OpenSilver.Internal;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Paints an area with a radial gradient. A focal point defines the beginning
    /// of the gradient, and a circle defines the end point of the gradient.
    /// </summary>
    public sealed class RadialGradientBrush : GradientBrush
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class.
        /// </summary>
        public RadialGradientBrush() {}
       
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class that has the specified gradient stops.
        /// </summary>
        /// <param name="gradientStopCollection">The gradient stops to set on this brush.</param>
        public RadialGradientBrush(GradientStopCollection gradientStopCollection)
        {
            GradientStops = gradientStopCollection;
        }
  
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class with the specified start and stop colors.
        /// </summary>
        /// <param name="startColor">
        /// Color value at the focus (System.Windows.Media.RadialGradientBrush.GradientOrigin)
        /// of the radial gradient.
        /// </param>
        /// <param name="endColor">Color value at the outer edge of the radial gradient.</param>
        public RadialGradientBrush(Color startColor, Color endColor)
        {
            GradientStopCollection gradientStops = new GradientStopCollection();
            gradientStops.Add(new GradientStop() { Color = startColor, Offset = 0 });
            gradientStops.Add(new GradientStop() { Color = endColor, Offset = 1 });
            GradientStops = gradientStops;
        }

        /// <summary>
        /// Gets or sets the center of the outermost circle of the radial gradient.
        /// </summary>
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RadialGradientBrush.Center"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center), 
                typeof(Point), 
                typeof(RadialGradientBrush), 
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the location of the two-dimensional focal point that defines
        /// the beginning of the gradient.
        /// The default is (0.5, 0.5).
        /// </summary>
        public Point GradientOrigin
        {
            get { return (Point)GetValue(GradientOriginProperty); }
            set { SetValue(GradientOriginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RadialGradientBrush.GradientOrigin"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty GradientOriginProperty =
            DependencyProperty.Register(
                nameof(GradientOrigin), 
                typeof(Point), 
                typeof(RadialGradientBrush), 
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the horizontal radius of the outermost circle of the radial
        /// gradient.
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RadialGradientBrush.RadiusX"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX), 
                typeof(double), 
                typeof(RadialGradientBrush), 
                new PropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets the vertical radius of the outermost circle of a radial gradient.
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RadialGradientBrush.RadiusY"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY), 
                typeof(double), 
                typeof(RadialGradientBrush), 
                new PropertyMetadata(0.5));

        internal override Task<string> GetDataStringAsync(UIElement parent)
            => Task.FromResult(INTERNAL_ToHtmlString(parent));

        private string GetGradientStopsString()
        {
            return string.Join(", ", 
                GradientStops.OrderBy(gs => gs.Offset)
                             .Select(gs => gs.Color.INTERNAL_ToHtmlString(this.Opacity) + gs.Offset * 100 + "%"));
        }

        internal string INTERNAL_ToHtmlString(DependencyObject parent)
        {
            // background-image: radial-gradient(50% 50% at 50% 100%,
            // blue 0%, white 10%, purple 20%, purple 99%, black 100%);
            string gradientStopsString = GetGradientStopsString();

            string percentageSymbol = "%";
            int multiplicatorForPercentage = 100;
            if (MappingMode == BrushMappingMode.Absolute)
            {
                percentageSymbol = "px";
                multiplicatorForPercentage = 1;
            }

            string gradientString = string.Format(CultureInfo.InvariantCulture, 
                "radial-gradient({0}{5} {1}{5} at {2}{5} {3}{5}, {4})",
                (int)(RadiusX * multiplicatorForPercentage), //{0}
                (int)(RadiusY * multiplicatorForPercentage), //{1}
                (int)(Center.X * multiplicatorForPercentage), //{2}
                (int)(Center.Y * multiplicatorForPercentage), //{3}
                gradientStopsString, //{4}
                percentageSymbol); //{5}

            if(SpreadMethod == GradientSpreadMethod.Repeat)
            {
                gradientString = "repeating-" + gradientString;
            }

            return gradientString;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            string gradientString = INTERNAL_ToHtmlString(parent);
            return new List<object>(4)
            {
                "-webkit-" + gradientString,
                "-o-" + gradientString,
                "-moz-" + gradientString,
                gradientString,
            };
        }
    }
}
