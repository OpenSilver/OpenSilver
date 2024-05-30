
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using OpenSilver.Internal.Media;

namespace OpenSilver.Internal;

internal sealed class TextMeasurementService
{
    private readonly Window _window;

    public TextMeasurementService(Window owner)
    {
        Debug.Assert(owner is not null);

        _window = owner;

        AttachMeasurementService(owner);
    }

    private void AttachMeasurementService(Window owner)
    {
        Debug.Assert(owner.OuterDiv is not null);

        string sOwner = Interop.GetVariableStringForJS(owner.OuterDiv);
        Interop.ExecuteJavaScriptVoid($"document.attachMeasurementService({sOwner});");
    }

    public Size MeasureView(string id,
                            string whiteSpace,
                            string overflowWrap,
                            double maxWidth,
                            string emptyVal)
    {
        string strMaxWidth = (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
            ? string.Empty : $"{maxWidth.ToInvariantString()}px";

        string strTextSize = Interop.ExecuteJavaScriptString(
            $"document.measureTextView('{_window.OuterDiv.UniqueIdentifier}','{id}','{whiteSpace}','{overflowWrap}','{strMaxWidth}','{emptyVal}')");

        int index = strTextSize.IndexOf('|');
        if (index > -1)
        {
            return new Size(
                double.Parse(strTextSize.Substring(0, index), CultureInfo.InvariantCulture),
                double.Parse(strTextSize.Substring(index + 1), CultureInfo.InvariantCulture));
        }
        return new Size(0, 0);
    }

    public Size MeasureTextBlock(TextBlock textblock)
    {
        (string whiteSpace, string overflowWrap) = UIElementHelpers.ToCssTextWrapping(textblock.TextWrapping);
        string lineHeight = FontProperties.ToCssLineHeight(textblock.LineHeight);
        string lineStackingStrategy = FontProperties.ToCssLineStackingStrategy(textblock.LineStackingStrategy);
        string maxWidth = GetWidthConstraint(textblock);
        string innerHTML = BuildInnerHtml(textblock);

        string size = Interop.ExecuteJavaScriptString(
            $"document.measureTextBlock('{_window.OuterDiv.UniqueIdentifier}','{innerHTML}','{whiteSpace}','{overflowWrap}','{lineHeight}','{lineStackingStrategy}','{maxWidth}')",
            false);

        int index = size.IndexOf('|');
        if (index > -1)
        {
            return new Size(
                double.Parse(size.Substring(0, index), CultureInfo.InvariantCulture),
                double.Parse(size.Substring(index + 1), CultureInfo.InvariantCulture));
        }
        return new Size(0, 0);

        static string BuildInnerHtml(TextBlock tb)
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            foreach (Inline inline in tb.Inlines.InternalItems)
            {
                inline.AppendHtml(builder);
            }
            return StringBuilderCache.GetStringAndRelease(builder);
        }

        static string GetWidthConstraint(TextBlock tb)
        {
            if (tb.TextWrapping == TextWrapping.Wrap)
            {
                double width = tb.Width;
                if (!double.IsNaN(width))
                {
                    Thickness padding = tb.Padding;
                    return $"{Math.Max(0, width - padding.Left - padding.Right).ToInvariantString()}px";
                }
            }

            return string.Empty;
        }
    }

    public double MeasureBaseline(IEnumerable<FontProperties> fonts)
    {
        bool isFirst = true;
        StringBuilder builder = StringBuilderCache.Acquire();
        foreach (FontProperties font in fonts)
        {
            if (!isFirst)
            {
                builder.Append(',');
            }

            isFirst = false;

            builder.Append('\'');
            font.AppendCssFont(builder);
            builder.Append('\'');
        }

        return Interop.ExecuteJavaScriptDouble(
            $"document.measureBaseline('{_window.OuterDiv.UniqueIdentifier}',{StringBuilderCache.GetStringAndRelease(builder)})", false);
    }
}
