
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

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using CSHTML5.Internal;

namespace OpenSilver.Internal;

internal static class UIElementHelpers
{
    internal static void SetCharacterSpacing(this UIElement uie, int cSpacing)
    {
        uie.OuterDiv.Style.letterSpacing = ToCssLetterSpacing(cSpacing);
    }

    internal static string ToCssLetterSpacing(int spacing)
    {
        double value = spacing / 1000.0;
        return $"{value.ToInvariantString()}em";
    }

    internal static void SetFontFamily(this UIElement uie, FontFamily font)
    {
        uie.OuterDiv.Style.fontFamily = font.ToCssString(uie);
    }

    internal static void SetFontStyle(this UIElement uie, FontStyle fontStyle)
    {
        uie.OuterDiv.Style.fontStyle = fontStyle.ToCssString();
    }

    internal static void SetFontWeight(this UIElement uie, FontWeight fontWeight)
    {
        uie.OuterDiv.Style.fontWeight = fontWeight.ToCssString();
    }

    internal static void SetForeground(this UIElement uie, Brush oldForeground, Brush newForeground)
    {
        var style = uie.OuterDiv.Style;
        switch ((oldForeground, newForeground))
        {
            case (GradientBrush, SolidColorBrush scb):
                style.backgroundImage = string.Empty;
                style.backgroundClip = string.Empty;
                style.color = scb.ToHtmlString();
                break;

            case (_, SolidColorBrush scb):
                style.color = scb.ToHtmlString();
                break;

            case (GradientBrush, LinearGradientBrush lgb):
                style.backgroundImage = lgb.ToHtmlString(uie);
                break;

            case (_, LinearGradientBrush lgb):
                style.backgroundImage = lgb.ToHtmlString(uie);
                style.color = "transparent";
                style.backgroundClip = "text";
                break;

            case (GradientBrush, RadialGradientBrush rgb):
                style.backgroundImage = rgb.ToHtmlString(uie);
                break;

            case (_, RadialGradientBrush rgb):
                style.backgroundImage = rgb.ToHtmlString(uie);
                style.color = "transparent";
                style.backgroundClip = "text";
                break;

            case (SolidColorBrush, null):
                style.color = string.Empty;
                break;

            case (GradientBrush, null):
                style.backgroundImage = string.Empty;
                style.backgroundClip = string.Empty;
                style.color = string.Empty;
                break;

            default:
                // ImageBrush and custom brushes are not supported. Keep using old brush.
                break;
        }
    }

    internal static async Task SetBackgroundAsync(this UIElement uie, Brush brush)
    {
        uie.OuterDiv.Style.background = brush switch
        {
            Brush => await brush.GetDataStringAsync(uie),
            _ => string.Empty,
        };
    }

    internal static void SetLineHeight(this UIElement uie, double lineHeight)
    {
        uie.OuterDiv.Style.lineHeight = ToCssLineHeight(lineHeight);
    }

    internal static string ToCssLineHeight(double lineHeight) =>
        lineHeight switch
        {
            0.0 => "normal",
            _ => $"{lineHeight.ToInvariantString()}px",
        };

    internal static void SetPadding(this UIElement uie, Thickness padding)
    {
        uie.OuterDiv.Style.padding = CollapseThicknessHelper(padding);
    }

    internal static void SetTextAlignment(this UIElement uie, TextAlignment textAlignment)
    {
        uie.OuterDiv.Style.textAlign = textAlignment switch
        {
            TextAlignment.Center => "center",
            TextAlignment.Right => "end",
            TextAlignment.Justify => "justify",
            _ => "start",
        };
    }

    internal static void SetTextDecorations(this UIElement uie, TextDecorationCollection tdc)
    {
        uie.OuterDiv.Style.textDecoration = tdc?.ToHtmlString() ?? string.Empty;
    }

    internal static void SetTextTrimming(this UIElement uie, TextTrimming textTrimming)
    {
        uie.OuterDiv.Style.textOverflow = textTrimming switch
        {
            TextTrimming.WordEllipsis or TextTrimming.CharacterEllipsis => "ellipsis",
            _ => "clip",
        };
    }

    internal static void SetTextWrapping(this UIElement uie, TextWrapping textWrapping)
    {
        var style = uie.OuterDiv.Style;
        (style.whiteSpace, style.overflowWrap) = ToCssTextWrapping(textWrapping);
    }

    internal static (string WhiteSpace, string OverflowWrap) ToCssTextWrapping(TextWrapping textWrapping) =>
        textWrapping switch
        {
            TextWrapping.Wrap => ("pre-wrap", "break-word"),
            _ => ("pre", string.Empty),
        };

    internal static void SetFontSize(this UIElement uie, double fontSize)
    {
        uie.OuterDiv.Style.fontSize = ToCssPxFontSize(fontSize);
    }

    internal static string ToCssPxFontSize(double fontSize) => $"{fontSize.ToInvariantString()}px";

    internal static void SetTextSelection(this UIElement uie, bool enabled)
    {
        uie.OuterDiv.Style.userSelect = enabled ? "auto" : "none";
    }

    internal static void SetInnerText(this UIElement uie, string text)
    {
        INTERNAL_HtmlDomManager.SetDomElementProperty(uie.OuterDiv,
            "innerText",
            INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text));
    }

    internal static void SetOpacity(this UIElement uie, double opacity)
    {
        uie.OuterDiv.Style.opacity = Math.Round(opacity, 2).ToInvariantString();
    }

    internal static async Task SetMaskImageAsync(this UIElement uie, Brush mask)
    {
        string maskImage;

        switch (mask)
        {
            case SolidColorBrush scb:
                string color = scb.ToHtmlString();
                maskImage = $"linear-gradient({color}, {color})";
                break;

            case LinearGradientBrush lgb:
                maskImage = lgb.ToHtmlString(uie);
                break;

            case RadialGradientBrush rgb:
                maskImage = rgb.ToHtmlString(uie);
                break;

            case ImageBrush ib:
                maskImage = await ib.GetDataStringAsync(uie);
                break;

            default:
                maskImage = string.Empty;
                break;
        }

        uie.OuterDiv.Style.maskImage = maskImage;
    }

    internal static void SetOutline(this UIElement uie, bool enable)
    {
        // Every UIElement has a css class that sets 'outline' to 'none'. 'revert' will
        // roll back that change so that 'outline' can be set to the default computed value.
        if (uie.GetFocusTarget() is INTERNAL_HtmlDomElementReference focusTarget)
        {
            focusTarget.Style.outline = enable ? "revert" : "none";
        }
    }

    internal static void SetTransform(this UIElement uie, Transform transform)
    {
        uie.OuterDiv.Style.transform = transform switch
        {
            Transform when !transform.IsIdentity => MatrixTransform.MatrixToHtmlString(transform.ValueInternal),
            _ => string.Empty,
        };
    }

    internal static void SetTransformOrigin(this UIElement uie, Point origin)
    {
        uie.OuterDiv.Style.transformOrigin = $"{Math.Round(origin.X * 100, 4).ToInvariantString()}% {Math.Round(origin.Y * 100, 4).ToInvariantString()}%";
    }

    internal static void SetZIndex(this UIElement uie, int value)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.zIndex = value.ToInvariantString();
    }

    internal static void SetCursor(this UIElement uie, Cursor cursor)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.cursor = cursor?.ToHtmlString() ?? string.Empty;
    }

    internal static void SetBorderRadius(this UIElement uie, CornerRadius radius)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.borderRadius = CollapseCornerRadiusHelper(radius);
    }

    internal static void SetBorderWidth(this UIElement uie, Thickness width)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.borderWidth = CollapseThicknessHelper(width);
    }

    internal static void SetClipPath(this UIElement uie, Geometry geometry)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.clipPath = geometry switch
        {
            Geometry => $"path(\"{geometry.ToPathData(CultureInfo.InvariantCulture)}\")",
            _ => string.Empty,
        };
    }

    internal static void SetTouchAction(this UIElement uie, string value)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.Style.touchAction = value;
    }

    internal static double GetBaseLineOffset(this UIElement uie)
    {
        Debug.Assert(uie is not null);
        if (uie.OuterDiv is not null)
        {
            string sDiv = Interop.GetVariableStringForJS(uie.OuterDiv);
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

    private static string CollapseThicknessHelper(Thickness t) =>
        (t.Top == t.Bottom, t.Left == t.Right) switch
        {
            (true, true) when t.Top == t.Left => $"{t.Top.ToInvariantString()}px",
            (true, true) => $"{t.Top.ToInvariantString()}px {t.Left.ToInvariantString()}px",
            (false, true) => $"{t.Top.ToInvariantString()}px {t.Left.ToInvariantString()}px {t.Bottom.ToInvariantString()}px",
            _ => $"{t.Top.ToInvariantString()}px {t.Right.ToInvariantString()}px {t.Bottom.ToInvariantString()}px {t.Left.ToInvariantString()}px",
        };

    private static string CollapseCornerRadiusHelper(CornerRadius cr) =>
        (cr.TopLeft == cr.BottomRight, cr.BottomLeft == cr.TopRight) switch
        {
            (true, true) when cr.TopLeft == cr.BottomLeft => $"{cr.TopLeft.ToInvariantString()}px",
            (true, true) => $"{cr.TopLeft.ToInvariantString()}px {cr.BottomLeft.ToInvariantString()}px",
            (false, true) => $"{cr.TopLeft.ToInvariantString()}px {cr.BottomLeft.ToInvariantString()}px {cr.BottomRight.ToInvariantString()}px",
            _ => $"{cr.TopLeft.ToInvariantString()}px {cr.TopRight.ToInvariantString()}px {cr.BottomRight.ToInvariantString()}px {cr.BottomLeft.ToInvariantString()}px",
        };
}
