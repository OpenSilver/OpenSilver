#if WORKINPROGRESS
#if MIGRATION
using System.Windows;
using System;

namespace System.Windows.Media.Effects
{
    public sealed partial class PixelShader : DependencyObject
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(PixelShader), new PropertyMetadata());
        

        public Uri UriSource
        {
            get { return (Uri)this.GetValue(PixelShader.UriSourceProperty); }
            set { this.SetValue(PixelShader.UriSourceProperty, value); }
        }
        

        public PixelShader()
        {
        }
        

    }
}
#endif
#endif