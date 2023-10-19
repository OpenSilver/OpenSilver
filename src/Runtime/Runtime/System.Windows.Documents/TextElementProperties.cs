
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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Documents;

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

    public static readonly DependencyProperty CharacterSpacingProperty =
        DependencyProperty.RegisterAttached(
            "CharacterSpacing",
            typeof(int),
            typeof(TextElementProperties),
            new PropertyMetadata(0) { Inherits = true, });

    public static readonly DependencyProperty FontFamilyProperty =
        DependencyProperty.RegisterAttached(
            "FontFamily",
            typeof(FontFamily),
            typeof(TextElementProperties),
            new PropertyMetadata(FontFamily.Default) { Inherits = true, },
            IsValidFontFamily);

    private static bool IsValidFontFamily(object o) => o is FontFamily;

    internal static void InvalidateMeasureOnFontFamilyChanged(UIElement uie, FontFamily font)
    {
        var face = font.GetFontFace(uie);
        if (face.IsLoaded)
        {
            if (uie is TextBlock tb)
            {
                tb.InvalidateCacheAndMeasure();
            }
            else
            {
                uie.InvalidateMeasure();
            }
        }
        else
        {
            face.RegisterForMeasure(uie);
            _ = face.LoadAsync();
        }
    }

    internal static double GetBaseLineOffsetNative(UIElement uie)
    {
        Debug.Assert(uie is not null);
        if (uie.INTERNAL_OuterDomElement is not null)
        {
            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(uie.INTERNAL_OuterDomElement);
            return OpenSilver.Interop.ExecuteJavaScriptDouble($"document.getBaseLineOffset({sDiv});");
        }

        return 0.0;
    }
}
