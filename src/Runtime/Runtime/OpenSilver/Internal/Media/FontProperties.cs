﻿
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

using System.Text;
using System.Web;
using System.Windows.Media;
using System.Windows;

namespace OpenSilver.Internal.Media;

internal readonly struct FontProperties
{
    public FontStyle FontStyle { get; init; }

    public FontWeight FontWeight { get; init; }

    public double FontSize { get; init; }

    public double LineHeight { get; init; }

    public FontFamily FontFamily { get; init; }

    public int CharacterSpacing { get; init; }

    internal void AppendCssFont(StringBuilder builder) =>
        AppendCssFont(builder, FontStyle, FontWeight, FontSize, LineHeight, FontFamily, false);

    internal static void AppendCssFontAsHtml(StringBuilder builder,
        FontStyle fontStyle,
        FontWeight fontWeight,
        double fontSize,
        double lineHeight,
        FontFamily fontFamily) => AppendCssFont(builder, fontStyle, fontWeight, fontSize, lineHeight, fontFamily, true);

    private static void AppendCssFont(StringBuilder builder,
        FontStyle fontStyle,
        FontWeight fontWeight,
        double fontSize,
        double lineHeight,
        FontFamily fontFamily,
        bool htmlEncode)
    {
        builder.Append(ToCssFontStyle(fontStyle))
               .Append(' ')
               .Append(ToCssFontWeight(fontWeight))
               .Append(' ')
               .Append(ToCssPxFontSize(fontSize));

        if (lineHeight != 0.0)
        {
            builder.Append(" / ")
                   .Append(ToCssLineHeight(lineHeight));
        }

        builder.Append(' ')
               .Append(ToCssFontFamily(fontFamily, htmlEncode));
    }

    internal static string ToCssFontStyle(FontStyle fontStyle) =>
        fontStyle.ToOpenTypeStyle() switch
        {
            0 => "normal",
            1 => "oblique",
            _ => "italic",
        };

    internal static string ToCssFontWeight(FontWeight fontWeight) => fontWeight.ToOpenTypeWeight().ToInvariantString();

    internal static string ToCssPxFontSize(double fontSize) => $"{fontSize.ToInvariantString()}px";

    internal static string ToCssLineHeight(double lineHeight) =>
        lineHeight switch
        {
            0.0 => "normal",
            _ => $"{lineHeight.ToInvariantString()}px",
        };

    internal static string ToCssFontFamily(FontFamily family) => ToCssFontFamily(family, false);

    internal static string ToCssFontFamily(FontFamily family, bool htmlEncode)
    {
        string font = family.GetFontFace().CssFontName;
        if (htmlEncode)
        {
            font = HttpUtility.HtmlEncode(font);
        }
        return font;
    }
    
    internal static string ToCssLetterSpacing(int spacing)
    {
        double value = spacing / 1000.0;
        return $"{value.ToInvariantString()}em";
    }
}
