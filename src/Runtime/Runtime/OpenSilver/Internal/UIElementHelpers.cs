
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using CSHTML5.Internal;
using OpenSilver.Internal.Media;

namespace OpenSilver.Internal;

internal static class UIElementHelpers
{
    internal static void SetCharacterSpacing(this UIElement uie, int cSpacing)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.LetterSpacing, FontProperties.ToCssLetterSpacing(cSpacing));
    }

    internal static void SetFontFamily(this UIElement uie, FontFamily font)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.FontFamily, FontProperties.ToCssFontFamily(font));
    }

    internal static void SetFontStyle(this UIElement uie, FontStyle fontStyle)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.FontStyle, FontProperties.ToCssFontStyle(fontStyle));
    }

    internal static void SetFontWeight(this UIElement uie, FontWeight fontWeight)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.FontWeight, FontProperties.ToCssFontWeight(fontWeight));
    }

    internal static void SetForeground(this UIElement uie, Brush oldForeground, Brush newForeground)
    {
        switch ((oldForeground, newForeground))
        {
            case (GradientBrush, SolidColorBrush scb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundClip, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, scb.ToHtmlString());
                break;

            case (_, SolidColorBrush scb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, scb.ToHtmlString());
                break;

            case (GradientBrush, LinearGradientBrush lgb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, lgb.ToHtmlString(uie));
                break;

            case (_, LinearGradientBrush lgb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, lgb.ToHtmlString(uie));
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, "transparent");
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundClip, "text");
                break;

            case (GradientBrush, RadialGradientBrush rgb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, rgb.ToHtmlString(uie));
                break;

            case (_, RadialGradientBrush rgb):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, rgb.ToHtmlString(uie));
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, "transparent");
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundClip, "text");
                break;

            case (SolidColorBrush, null):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, string.Empty);
                break;

            case (GradientBrush, null):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundImage, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BackgroundClip, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Color, string.Empty);
                break;

            default:
                // ImageBrush and custom brushes are not supported. Keep using old brush.
                break;
        }
    }

    internal static void SetBackground(this UIElement uie, Brush brush)
    {
        string background;

        switch (brush)
        {
            case Brush:
                var task = brush.GetDataStringAsync(uie);
                if (!task.IsCompletedSuccessfully)
                {
                    return;
                }
                background = task.Result;
                break;

            default:
                background = string.Empty;
                break;
        }

        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Background, background);
    }

    internal static void SetLineHeight(this UIElement uie, double lineHeight)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.LineHeight, FontProperties.ToCssLineHeight(lineHeight));
    }

    internal static void SetLineStackingStrategy(this UIElement uie, LineStackingStrategy strategy)
    {
        const string LineStackingStrategyProperty = "--line-stacking-strategy";
        uie.OuterDiv.SetCssStyleProperty(LineStackingStrategyProperty, FontProperties.ToCssLineStackingStrategy(strategy));
    }

    internal static void SetPadding(this UIElement uie, Thickness padding)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Padding, CollapseThicknessHelper(padding));
    }

    internal static void SetMargin(this UIElement uie, Thickness margin)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Margin, CollapseThicknessHelper(margin));
    }

    internal static void SetTextAlignment(this UIElement uie, TextAlignment textAlignment)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.TextAlign, FontProperties.ToCssTextAlignment(textAlignment));
    }

    internal static void SetTextDecorations(this UIElement uie, TextDecorationCollection tdc)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.TextDecoration, FontProperties.ToCssTextDecoration(tdc));
    }

    internal static void SetTextTrimming(this UIElement uie, TextTrimming textTrimming)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Overflow, textTrimming switch
        {
            TextTrimming.WordEllipsis or TextTrimming.CharacterEllipsis => "clip",
            _ => string.Empty,
        });
    }

    internal static void SetTextWrapping(this UIElement uie, TextWrapping textWrapping)
    {
        (string whiteSpace, string overflowWrap) = ToCssTextWrapping(textWrapping);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.WhiteSpace, whiteSpace);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.OverflowWrap, overflowWrap);
    }

    internal static (string WhiteSpace, string OverflowWrap) ToCssTextWrapping(TextWrapping textWrapping) =>
        textWrapping switch
        {
            TextWrapping.Wrap => ("pre-wrap", "break-word"),
            _ => ("pre", string.Empty),
        };

    internal static void SetFontSize(this UIElement uie, double fontSize)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.FontSize, FontProperties.ToCssPxFontSize(fontSize));
    }

    internal static void SetTextSelection(this UIElement uie, bool enabled)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.UserSelect, enabled ? "text" : "none");
    }

    internal static void SetInnerText(this UIElement uie, string text)
    {
        uie.OuterDiv.SetProperty("innerText", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text));
    }

    internal static void SetDirection(this UIElement uie, FlowDirection flowDirection)
    {
        uie.OuterDiv.SetAttribute("dir", flowDirection == FlowDirection.LeftToRight ? "ltr" : "rtl");
    }

    internal static void SetOpacity(this UIElement uie, double opacity)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Opacity, Math.Round(opacity, 2).ToInvariantString());
    }

    internal static void SetMaskImage(this UIElement uie, Brush mask)
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
                var task = ib.GetDataStringAsync(uie);
                if (!task.IsCompletedSuccessfully)
                {
                    return;
                }
                maskImage = task.Result;
                break;

            default:
                maskImage = string.Empty;
                break;
        }

        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.MaskImage, maskImage);
    }

    internal static void SetOutline(this UIElement uie, bool enable)
    {
        // Every UIElement has a css class that sets 'outline' to 'none'. 'revert' will
        // roll back that change so that 'outline' can be set to the default computed value.
        HtmlElementReference focusTarget = uie.GetFocusTarget();
        if (focusTarget.IsConnected)
        {
            focusTarget.SetCssStyleProperty(CssPropertyNames.Outline, enable ? "revert" : "none");
        }
    }

    internal static void SetTransform(this UIElement uie, Transform transform)
    {
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Transform, transform switch
        {
            Transform when !Transform.IsIdentityTransform(transform) => MatrixTransform.MatrixToHtmlString(transform.Matrix),
            _ => string.Empty,
        });
    }

    internal static void SetZIndex(this UIElement uie, int value)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.ZIndex, value.ToInvariantString());
    }

    internal static void SetCursor(this UIElement uie, Cursor cursor)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.Cursor, cursor?.ToHtmlString() ?? string.Empty);
    }

    internal static void SetBorderRadius(this UIElement uie, CornerRadius radius)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderRadius, CollapseCornerRadiusHelper(radius));
    }

    internal static void SetBorderWidth(this UIElement uie, Thickness width)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderWidth, CollapseThicknessHelper(width));
    }

    internal static void SetBorderColor(this UIElement uie, Brush oldBrush, Brush newBrush)
    {
        switch (oldBrush, newBrush)
        {
            case (GradientBrush, SolidColorBrush solid):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSource, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSlice, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderColor, solid.ToHtmlString());
                break;

            case (_, SolidColorBrush solid):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderColor, solid.ToHtmlString());
                break;

            case (_, LinearGradientBrush linear):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderColor, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSource, linear.ToHtmlString(uie));
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSlice, "1");
                break;

            case (_, RadialGradientBrush radial):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderColor, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSource, radial.ToHtmlString(uie));
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSlice, "1");
                break;

            case (_, null):
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderColor, "transparent");
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSource, string.Empty);
                uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.BorderImageSlice, string.Empty);
                break;

            default:
                // ImageBrush and custom brushes are not supported.
                // Keep using old brush.
                break;
        }
    }

    internal static void SetClipPath(this UIElement uie, Geometry geometry)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.ClipPath, geometry switch
        {
            Geometry => $"path(\"{geometry.ToPathData(CultureInfo.InvariantCulture)}\")",
            _ => string.Empty,
        });
    }

    internal static void SetTouchAction(this UIElement uie, string value)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.TouchAction, value);
    }

    internal static void SetCaretColor(this UIElement uie, Brush brush)
    {
        Debug.Assert(uie is not null);
        uie.OuterDiv.SetCssStyleProperty(CssPropertyNames.CaretColor, brush switch
        {
            SolidColorBrush scb => scb.ToHtmlString(),
            _ => string.Empty,
        });
    }

    internal static void InvalidateMeasureOnFontFamilyChanged(UIElement uie, FontFamily font)
    {
        var face = font.GetFontFace();
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
