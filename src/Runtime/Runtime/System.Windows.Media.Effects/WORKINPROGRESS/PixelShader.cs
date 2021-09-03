#if MIGRATION
using System.Windows;
using System;

namespace System.Windows.Media.Effects
{
	[OpenSilver.NotImplemented]
    public sealed partial class PixelShader : DependencyObject
    {
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(PixelShader), new PropertyMetadata());
        

		[OpenSilver.NotImplemented]
        public Uri UriSource
        {
            get { return (Uri)this.GetValue(PixelShader.UriSourceProperty); }
            set { this.SetValue(PixelShader.UriSourceProperty, value); }
        }
        

		[OpenSilver.NotImplemented]
        public PixelShader()
        {
        }
        

    }
}
#endif