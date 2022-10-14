
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
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
#endif

namespace System.Windows.Media.Effects
{
    /// <summary>
    /// Applies a shadow behind a visual object at a slight offset. The offset is determined
    /// by mimicking a casting shadow from an imaginary light source.
    /// </summary>
    public sealed partial class DropShadowEffect : Effect
    {
        // Note: If we want to allow inwards shadows, we can use "inset" in the string that is set.
        // From what I could gather, the shadows in WPF are naturally going both ways (inwards and
        // outwards), and the trick the people use to have an inwards shadow is to add a transparent
        // border (inside the original border) on which they put the DropShadowEffect. This allows
        // the inwards shadow to be seen (otherwise it is hidden by the parent element).

        /// <summary>
        /// Initializes a new instance of the <see cref="DropShadowEffect"/> class.
        /// </summary>
        public DropShadowEffect() { }

        /// <summary>
        /// Identifies the <see cref="BlurRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(
                nameof(BlurRadius),
                typeof(double),
                typeof(DropShadowEffect),
                new PropertyMetadata(5.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets how defined the edges of the shadow are (how blurry the shadow is).
        /// </summary>
        /// <returns>
        /// How blurry the shadow is. The default is 5.
        /// </returns>
        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(DropShadowEffect),
                new PropertyMetadata(Colors.Black, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <returns>
        /// The color of the shadow. The default is a color with ARGB value of FF000000 (black).
        /// </returns>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Direction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                nameof(Direction),
                typeof(double),
                typeof(DropShadowEffect),
                new PropertyMetadata(315.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the angle at which the shadow is cast.
        /// </summary>
        /// <returns>
        /// The angle at which the shadow is cast, where 0 is immediately to the right of
        /// the object and positive values move the shadow counterclockwise. The default
        /// is 315.
        /// </returns>
        public double Direction
        {
            get => (double)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity),
                typeof(double),
                typeof(DropShadowEffect),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the degree of opacity of the shadow.
        /// </summary>
        /// <returns>
        /// The degree of opacity. The valid range of values is from 0 through 1, where 0
        /// is completely transparent and 1 is completely opaque. The default is 1.
        /// </returns>
        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShadowDepth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(
                nameof(ShadowDepth),
                typeof(double),
                typeof(DropShadowEffect),
                new PropertyMetadata(5.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the distance between the object and the shadow that it casts.
        /// </summary>
        /// <returns>
        /// The distance between the plane of the object casting the shadow and the shadow
        /// plane measured in devicepixels. The valid range of values is from 0 through 300.
        /// The default is 5.
        /// </returns>
        public double ShadowDepth
        {
            get => (double)GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DropShadowEffect)d).RaiseChanged();

        internal override void Render(UIElement renderTarget)
        {
            if (renderTarget != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(renderTarget))
            {
                var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(renderTarget);

                double x = Math.Cos(Direction * Math.PI / 180d) * ShadowDepth;
                double y = -(Math.Sin(Direction * Math.PI / 180d) * ShadowDepth);
                double opacity = Math.Max(Math.Min(1.0, Opacity), 0.0);

                string shadowString = x.ToInvariantString() + "px " +
                    y.ToInvariantString() + "px " +
                    BlurRadius.ToInvariantString() + "px " +
                    Color.FromArgb(Convert.ToByte(opacity * 255d), Color.R, Color.G, Color.B).INTERNAL_ToHtmlString(1d);

                if (renderTarget is TextBlock)
                {
                    domStyle.textShadow = shadowString;
                }
                else
                {
                    domStyle.boxShadow = shadowString;

                    domStyle.borderCollapse = "separate"; // This is required for IE only. If this property is not set or equal to "collapse", the shadow does not render at all on IE. See: http://stackoverflow.com/questions/9949396/css-box-shadow-not-working-in-ie
                    domStyle.borderSpacing = "0px"; // This is required to fix a bug that comes with the line above: a 2 px margin appears around the children of the element, which can lead to some elements overflowing relative to those children when they shouldn't.
                }
            }
        }
    }
}