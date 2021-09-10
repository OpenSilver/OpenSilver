

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


#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    [OpenSilver.NotImplemented]
    public sealed partial class RenderTargetBitmap : BitmapSource
    {
        [OpenSilver.NotImplemented]
        public RenderTargetBitmap(int pixelWidth, int pixelHeight, double dpiX, double dpiY, PixelFormat pixelFormat)
        {
        }

        [OpenSilver.NotImplemented]
        public void Render(Visual visual)
        {
        }
    }
}
