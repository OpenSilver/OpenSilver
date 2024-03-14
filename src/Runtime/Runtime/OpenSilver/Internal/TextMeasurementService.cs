
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Media;
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
        return new Size(
            double.Parse(strTextSize.Substring(0, index), CultureInfo.InvariantCulture),
            double.Parse(strTextSize.Substring(index + 1), CultureInfo.InvariantCulture));
    }

    public Size MeasureText(string text,
                            double maxWidth,
                            double fontSize,
                            FontFamily fontFamily,
                            FontStyle fontStyle,
                            FontWeight fontWeight,
                            double lineHeight,
                            int characterSpacing,
                            TextWrapping textWrapping)
    {
        string escapedText = HttpUtility.JavaScriptStringEncode(text, true);
        string sMaxWidth = double.IsPositiveInfinity(maxWidth) ? string.Empty : $"{maxWidth.ToInvariantString()}px";
        string sFontSize = FontProperties.ToCssPxFontSize(fontSize);
        string sFontFamily = FontProperties.ToCssFontFamily(fontFamily);
        string sFontStyle = FontProperties.ToCssFontStyle(fontStyle);
        string sFontWeight = FontProperties.ToCssFontWeight(fontWeight);
        string sLineHeight = FontProperties.ToCssLineHeight(lineHeight);
        string sSpacing = FontProperties.ToCssLetterSpacing(characterSpacing);
        (string sWhiteSpace, string sOverflowWrap) = UIElementHelpers.ToCssTextWrapping(textWrapping);

        string size = Interop.ExecuteJavaScriptString(
            $"document.measureText('{_window.OuterDiv.UniqueIdentifier}',{escapedText},'{sMaxWidth}','{sFontSize}','{sFontFamily}','{sFontStyle}','{sFontWeight}','{sLineHeight}','{sSpacing}','{sWhiteSpace}','{sOverflowWrap}')",
            false);

        int index = size.IndexOf('|');
        return new Size(
            double.Parse(size.Substring(0, index), CultureInfo.InvariantCulture),
            double.Parse(size.Substring(index + 1), CultureInfo.InvariantCulture));
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
