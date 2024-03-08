
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
using System.Web;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal;

/// <summary>
/// Measure Text Block width and height from html element.
/// </summary>
internal sealed class TextMeasurementService
{
    private readonly string _measurerId;

    public TextMeasurementService(Window parent)
    {
        Debug.Assert(parent is not null);

        string id = CreateMeasurementText(parent);
        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidOperationException();
        }

        _measurerId = id;
    }

    private string CreateMeasurementText(Window parent)
    {
        Debug.Assert(parent.OuterDiv is not null);

        string sParent = Interop.GetVariableStringForJS(parent.OuterDiv);
        return Interop.ExecuteJavaScriptString($"document.createMeasurementService({sParent});");
    }

    public Size MeasureView(string uid,
                            string whiteSpace,
                            string overflowWrap,
                            double maxWidth,
                            string emptyVal)
    {
        string strMaxWidth = (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
            ? string.Empty : $"{maxWidth.ToInvariantString()}px";

        string strTextSize = Interop.ExecuteJavaScriptString(
            $"document.measureTextBlock('{_measurerId}','{uid}','{whiteSpace}','{overflowWrap}','{strMaxWidth}','{emptyVal}')");

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
        string sFontSize = UIElementHelpers.ToCssPxFontSize(fontSize);
        string sFontFamily = fontFamily.ToCssString(null);
        string sFontStyle = fontStyle.ToCssString();
        string sFontWeight = fontWeight.ToCssString();
        string sLineHeight = UIElementHelpers.ToCssLineHeight(lineHeight);
        string sSpacing = UIElementHelpers.ToCssLetterSpacing(characterSpacing);
        (string sWhiteSpace, string sOverflowWrap) = UIElementHelpers.ToCssTextWrapping(textWrapping);

        string size = Interop.ExecuteJavaScriptString(
            $"document.measureText('{_measurerId}',{escapedText},'{sMaxWidth}','{sFontSize}','{sFontFamily}','{sFontStyle}','{sFontWeight}','{sLineHeight}','{sSpacing}','{sWhiteSpace}','{sOverflowWrap}')");

        int index = size.IndexOf('|');
        return new Size(
            double.Parse(size.Substring(0, index), CultureInfo.InvariantCulture),
            double.Parse(size.Substring(index + 1), CultureInfo.InvariantCulture));
    }
}
