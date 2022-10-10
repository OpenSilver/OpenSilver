
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

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Media.Effects
{
    /// <summary>
    /// Represents an effect that you can apply to an object that simulates looking at
    /// the object through an out-of-focus lens.
    /// </summary>
    public sealed class BlurEffect : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlurEffect"/> class.
        /// </summary>
        public BlurEffect() { }

        /// <summary>
        /// Identifies the <see cref="Radius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                nameof(Radius),
                typeof(double),
                typeof(BlurEffect),
                new PropertyMetadata(5.0, OnRadiusChanged));

        /// <summary>
        /// Gets or sets the amount of blurring applied by the <see cref="BlurEffect"/>.
        /// </summary>
        /// <returns>
        /// The radius used in the blur, as a pixel-based factor. The default is 5.
        /// </returns>
        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double radius = (double)e.NewValue;
            if (double.IsNaN(radius) || double.IsInfinity(radius))
            {
                throw new ArgumentException();
            }

            ((BlurEffect)d).RaiseChanged();
        }

        internal override void Render(UIElement renderTarget)
        {
            if (renderTarget != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(renderTarget))
            {
                var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(renderTarget);

                // This gets a result very similar to Silverlight. Using the Radius directly
                // makes the element much more blurry than Silverlight.
                double cssRadius = Math.Min(Math.Floor(Math.Max(Radius, 0) / 2.0), 6.0);
                domStyle.filter = $"blur({cssRadius.ToInvariantString()}px)";
            }
        }
    }
}
