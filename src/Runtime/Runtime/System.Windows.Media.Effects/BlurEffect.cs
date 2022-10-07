#if !MIGRATION
using Windows.UI.Xaml;
#endif

using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Media.Effects
{
    //
    // Summary:
    //     Represents an effect that you can apply to an object that simulates looking at
    //     the object through an out-of-focus lens.
    public sealed partial class BlurEffect : Effect
    {
        //
        // Summary:
        //     Identifies the System.Windows.Media.Effects.BlurEffect.Radius dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Media.Effects.BlurEffect.Radius dependency
        //     property.
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(BlurEffect), new PropertyMetadata(5d, ManageChange));


        internal override void SetParentUIElement(UIElement newParent)
        {
            base.SetParentUIElement(newParent);
            RefreshEffect();
        }

        private static void ManageChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlurEffect)d).RefreshEffect();
        }

        private void RefreshEffect() //todo: if we add support for multiple effects on a same element, return the string instead of directly setting the property.
        {
            if (_parentUIElement != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(_parentUIElement))
            {
                var domStyle = OpenSilver.Interop.GetDiv(_parentUIElement);
                INTERNAL_HtmlDomManager.SetDomElementStyleProperty(domStyle, new Collections.Generic.List<string>() { "filter" }, "blur(" + Radius.ToInvariantString() + "px)");
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Media.Effects.BlurEffect class.
        public BlurEffect()
        {

        }

        //
        // Summary:
        //     Gets or sets the amount of blurring applied by the System.Windows.Media.Effects.BlurEffect.
        //
        // Returns:
        //     The radius used in the blur, as a pixel-based factor. The default is 5.
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
    }
}
