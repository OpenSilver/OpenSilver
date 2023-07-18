
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
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Internal.Controls;

internal sealed class ScrollData
{
    internal ScrollViewer ScrollOwner { get; set; }

    internal Size Viewport { get; set; }

    internal Size Extent { get; set; }

    internal Point Offset { get; set; }

    internal bool CanHorizontallyScroll { get; set; }

    internal bool CanVerticallyScroll { get; set; }

    internal double HorizontalOffset => Offset.X;

    internal double VerticalOffset => Offset.Y;

    internal double ViewportWidth => Viewport.Width;

    internal double ViewportHeight => Viewport.Height;

    internal double ExtentWidth => Extent.Width;

    internal double ExtentHeight => Extent.Height;
}