
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
using Windows.UI.Text;
#endif

#if MIGRATION
namespace System.Windows.Documents;
#else
namespace Windows.UI.Xaml.Documents;
#endif

internal static class TextElementProperties
{
    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.RegisterAttached(
            "FontSize",
            typeof(double),
            typeof(TextElementProperties),
            new PropertyMetadata(11d) { Inherits = true, });

    public static readonly DependencyProperty FontWeightProperty =
        DependencyProperty.RegisterAttached(
            "FontWeight",
            typeof(FontWeight),
            typeof(TextElementProperties),
            new PropertyMetadata(FontWeights.Normal) { Inherits = true, });
}
