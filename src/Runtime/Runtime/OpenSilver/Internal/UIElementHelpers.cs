
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Threading.Tasks;
using System;
using CSHTML5.Internal;

namespace OpenSilver.Internal;

internal static class UIElementHelpers
{
    internal static void SetCharacterSpacing(this UIElement uie, int cSpacing)
    {
        double value = cSpacing / 1000.0;
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.letterSpacing = $"{value.ToInvariantString()}em";
    }

    internal static void SetFontFamily(this UIElement uie, FontFamily font)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.fontFamily = font.GetFontFace(uie).CssFontName;
    }

    internal static void SetFontStyle(this UIElement uie, FontStyle fontStyle)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.fontStyle = fontStyle.ToString().ToLower();
    }

    internal static void SetFontWeight(this UIElement uie, FontWeight fontWeight)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.fontWeight = fontWeight.ToOpenTypeWeight().ToInvariantString();
    }

    internal static void SetForeground(this UIElement uie, Brush foreground)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        switch (foreground)
        {
            case SolidColorBrush scb:
                style.color = scb.INTERNAL_ToHtmlString();
                break;

            case null:
                style.color = string.Empty;
                break;

            default:
                // GradientBrush, ImageBrush and custom brushes are not supported.
                // Keep using old brush.
                break;
        }
    }

    internal static async Task SetBackgroundAsync(this UIElement uie, Brush brush)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.background = brush switch
        {
            Brush => await brush.GetDataStringAsync(uie),
            _ => string.Empty,
        };
    }

    internal static void SetLineHeight(this UIElement uie, double lineHeight)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.lineHeight = lineHeight switch
        {
            0.0 => "normal",
            _ => $"{lineHeight.ToInvariantString()}px",
        };
    }

    internal static void SetPadding(this UIElement uie, Thickness padding)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.padding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
    }

    internal static void SetTextAlignment(this UIElement uie, TextAlignment textAlignment)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.textAlign = textAlignment switch
        {
            TextAlignment.Center => "center",
            TextAlignment.Right => "end",
            TextAlignment.Justify => "justify",
            _ => "start",
        };
    }

    internal static void SetTextDecorations(this UIElement uie, TextDecorationCollection tdc)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.textDecoration = tdc?.ToHtmlString() ?? string.Empty;
    }

    internal static void SetTextTrimming(this UIElement uie, TextTrimming textTrimming)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.textOverflow = textTrimming switch
        {
            TextTrimming.WordEllipsis or TextTrimming.CharacterEllipsis => "ellipsis",
            _ => "clip",
        };
    }

    internal static void SetTextWrapping(this UIElement uie, TextWrapping textWrapping)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        switch (textWrapping)
        {
            case TextWrapping.Wrap:
                style.whiteSpace = "pre-wrap";
                style.overflowWrap = "break-word";
                break;

            case TextWrapping.NoWrap:
            default:
                style.whiteSpace = "pre";
                style.overflowWrap = string.Empty;
                break;
        }
    }

    internal static void SetFontSize(this UIElement uie, double fontSize)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.fontSize = $"{fontSize.ToInvariantString()}px";
    }

    internal static void SetTextSelection(this UIElement uie, bool enabled)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.userSelect = enabled ? "auto" : "none";
    }

    internal static void SetInnerText(this UIElement uie, string text)
    {
        string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(uie.INTERNAL_OuterDomElement);
        string escapedText = INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text);
        Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.innerText = \"{escapedText}\";");
    }

    internal static void SetOpacity(this UIElement uie, double opacity)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.opacity = Math.Round(opacity, 3).ToInvariantString();
    }

    internal static void SetTransform(this UIElement uie, Transform transform)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.transform = transform switch
        {
            Transform when !transform.IsIdentity => MatrixTransform.MatrixToHtmlString(transform.ValueInternal),
            _ => string.Empty,
        };
    }

    internal static void SetTransformOrigin(this UIElement uie, Point origin)
    {
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.transformOrigin = $"{(origin.X * 100).ToInvariantString()}% {(origin.Y * 100).ToInvariantString()}%";
    }

    internal static void SetZIndex(this UIElement uie, int value)
    {
        Debug.Assert(uie is not null);
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.zIndex = value.ToInvariantString();
    }

    internal static void SetCursor(this UIElement uie, Cursor cursor)
    {
        Debug.Assert(uie is not null);
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.cursor = cursor?.ToHtmlString() ?? string.Empty;
    }

    internal static void SetBorderRadius(this UIElement uie, CornerRadius radius)
    {
        Debug.Assert(uie is not null);
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.borderRadius = $"{radius.TopLeft.ToInvariantString()}px {radius.TopRight.ToInvariantString()}px {radius.BottomRight.ToInvariantString()}px {radius.BottomLeft.ToInvariantString()}px";
    }

    internal static void SetBorderWidth(this UIElement uie, Thickness width)
    {
        Debug.Assert(uie is not null);
        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(uie.INTERNAL_OuterDomElement);
        style.boxSizing = "border-box";
        style.borderStyle = "solid";
        style.borderWidth = $"{width.Top.ToInvariantString()}px {width.Right.ToInvariantString()}px {width.Bottom.ToInvariantString()}px {width.Left.ToInvariantString()}px";
    }

    internal static double GetBaseLineOffset(this UIElement uie)
    {
        Debug.Assert(uie is not null);
        if (uie.INTERNAL_OuterDomElement is not null)
        {
            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(uie.INTERNAL_OuterDomElement);
            return Interop.ExecuteJavaScriptDouble($"document.getBaseLineOffset({sDiv});");
        }

        return 0.0;
    }

    internal static void InvalidateMeasureOnFontFamilyChanged(UIElement uie, FontFamily font)
    {
        var face = font.GetFontFace(uie);
        if (face.IsLoaded)
        {
            InvalidateMeasure(uie);
        }
        else
        {
            face.RegisterForMeasure(uie);
            _ = face.LoadAsync();
        }
    }

    internal static void InvalidateMeasure(UIElement uie)
    {
        DependencyObject d = uie;
        while (d is not null)
        {
            if (d is FrameworkElement fe)
            {
                if (fe is TextBlock tb)
                {
                    tb.InvalidateCacheAndMeasure();
                }
                else
                {
                    fe.InvalidateMeasure();
                }
                return;
            }

            d = VisualTreeHelper.GetParent(d);
        }
    }
}
