

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


#if !MIGRATION
using Windows.UI.Xaml;
#endif

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
