
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
    /// A bitmap effect that paints a drop shadow around the target texture.
    /// </summary>
    public sealed partial class DropShadowEffect : Effect
    {
        //Note: If we want to allow inwards shadows, we can use "inset" in the string that is set.
        //      From what I could gather, the shadows in WPF are naturally going both ways (inwards and outwards), and the trick the people use to have an inwards shadow is
        //      to add a transparent border (inside the original border) on which they put the DropShadowEffect. This allows the inwards shadow to be seen (otherwise it is hidden by the parent element).


        /// <summary>
        /// Initializes a new instance of the <see cref="DropShadowEffect"/>
        /// class.
        /// </summary>
        public DropShadowEffect()
        {
        }

        internal override void SetParentUIElement(UIElement newParent)
        {
            base.SetParentUIElement(newParent);
            RefreshEffect();
        }

        private void RefreshEffect() //todo: if we add support for multiple effects on a same element, return the string instead of directly setting the property.
        {
            if (_parentUIElement != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(_parentUIElement))
            {
                var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(_parentUIElement);
                double x, y;
                GetXYPositionFromDirectionAndDepth(out x, out y);
                double opacity = Math.Max(Math.Min(1.0, this.Opacity), 0.0);
               
                string shadowString = x.ToInvariantString() + "px " +
                    y.ToInvariantString() + "px " +
                    BlurRadius.ToInvariantString() + "px " +
                    Color.FromArgb(Convert.ToByte(opacity * 255d), Color.R, Color.G, Color.B).INTERNAL_ToHtmlString(1d);

                if (_parentUIElement is TextBlock)
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

        private void GetXYPositionFromDirectionAndDepth(out double x, out double y)
        {
            x = Math.Cos(Direction * Math.PI / 180d) * ShadowDepth;
            y = -(Math.Sin(Direction * Math.PI / 180d) * ShadowDepth);
        }

        /// <summary>
        /// Gets or sets a value that indicates the radius of the shadow's blur effect.
        /// </summary>
        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BlurRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(
                nameof(BlurRadius), 
                typeof(double), 
                typeof(DropShadowEffect), 
                new PropertyMetadata(5d, ManageChange));

        private static void ManageChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropShadowEffect)d).RefreshEffect();
        }

        /// <summary>
        /// Gets or sets the color of the drop shadow.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color), 
                typeof(Color), 
                typeof(DropShadowEffect), 
                new PropertyMetadata(Colors.Black, ManageChange));

        /// <summary>
        /// Gets or sets the direction of the drop shadow.
        /// </summary>
        public double Direction
        {
            get { return (double)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Direction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                nameof(Direction), 
                typeof(double), 
                typeof(DropShadowEffect), 
                new PropertyMetadata(315d, ManageChange));

        /// <summary>
        /// Gets or sets the opacity of the drop shadow.
        /// </summary>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity), 
                typeof(double), 
                typeof(DropShadowEffect), 
                new PropertyMetadata(1d, ManageChange));

        //// Returns:
        ////     A System.Windows.Media.Effects.RenderingBias value that indicates whether
        ////     the system renders the drop shadow with emphasis on speed or quality. The
        ////     default is System.Windows.Media.Effects.RenderingBias.Performance.
        ///// <summary>
        ///// Gets or sets a value that indicates whether the system renders the drop shadow
        ///// with emphasis on speed or quality.
        ///// </summary>
        //public RenderingBias RenderingBias
        //{
        //    get { return (RenderingBias)GetValue(RenderingBiasProperty); }
        //    set { SetValue(RenderingBiasProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the System.Windows.Media.Effects.DropShadowEffect.RenderingBias
        ///// dependency property.
        ///// </summary>
        //public static readonly DependencyProperty RenderingBiasProperty =
        //    DependencyProperty.Register("RenderingBias", typeof(RenderingBias), typeof(DropShadowEffect), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the distance of the drop shadow below the texture.
        /// </summary>
        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShadowDepth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(
                nameof(ShadowDepth), 
                typeof(double), 
                typeof(DropShadowEffect), 
                new PropertyMetadata(5d, ManageChange));

        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.Effects.Effect object,
        ///// making deep copies of this object's values. When copying this object's dependency
        ///// properties, this method copies resource references and data bindings (which
        ///// may no longer resolve), but not animations or their current values.
        ///// </summary>
        ///// <returns>
        ///// A modifiable clone of this instance. The returned clone is effectively a
        ///// deep copy of the current object. The clone's System.Windows.Freezable.IsFrozen
        ///// property is false.
        ///// </returns>
        //public DropShadowEffect Clone();
        
        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.Effects.Effect object,
        ///// making deep copies of this object's current values. Resource references,
        ///// data bindings, and animations are not copied, but their current values are
        ///// copied.
        ///// </summary>
        ///// <returns>
        ///// A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        ///// property will be false even if the source's System.Windows.Freezable.IsFrozen
        ///// property was true.
        ///// </returns>
        //public DropShadowEffect CloneCurrentValue();
        //protected override Freezable CreateInstanceCore();
    }
}