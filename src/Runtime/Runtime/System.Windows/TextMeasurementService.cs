
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
using CSHTML5;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows
{
    internal interface ITextMeasurementService
    {
        Size MeasureText(string uid,
                         string whiteSpace,
                         string overflowWrap,
                         double maxWidth,
                         string emptyVal);
    }

    /// <summary>
    /// Measure Text Block width and height from html element.
    /// </summary>
    internal sealed class TextMeasurementService : ITextMeasurementService
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
            Debug.Assert(parent.INTERNAL_OuterDomElement is INTERNAL_HtmlDomElementReference);

            string sParent = INTERNAL_InteropImplementation.GetVariableStringForJS(parent.INTERNAL_OuterDomElement);
            return OpenSilver.Interop.ExecuteJavaScriptString($"document.createMeasurementService({sParent});");
        }

        public Size MeasureText(string uid,
                                string whiteSpace,
                                string overflowWrap,
                                double maxWidth,
                                string emptyVal)
        {
            string strMaxWidth = (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
                ? string.Empty : $"{maxWidth.ToInvariantString()}px";

            string strTextSize = OpenSilver.Interop.ExecuteJavaScriptString(
                $"document.measureTextBlock('{_measurerId}','{uid}','{whiteSpace}','{overflowWrap}','{strMaxWidth}','{emptyVal}')");

            int index = strTextSize?.IndexOf('|') ?? -1;
            if (index > -1)
            {
                return new Size(
                    double.Parse(strTextSize.Substring(0, index), CultureInfo.InvariantCulture),
                    double.Parse(strTextSize.Substring(index + 1), CultureInfo.InvariantCulture));
            }

            return new Size(0, 0);
        }
    }
}