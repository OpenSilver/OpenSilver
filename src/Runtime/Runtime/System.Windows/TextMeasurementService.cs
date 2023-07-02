
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
using System.Globalization;
using OpenSilver.Internal;
using CSHTML5;
using CSHTML5.Internal;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal interface ITextMeasurementService
    {
        Size MeasureText(string uid,
                         string whiteSpace,
                         string overflowWrap,
                         Thickness padding,
                         double maxWidth,
                         string emptyVal);
    }

    /// <summary>
    /// Measure Text Block width and height from html element.
    /// </summary>
    internal sealed class TextMeasurementService : ITextMeasurementService
    {
        private INTERNAL_HtmlDomStyleReference textBlockDivStyle;
        private object textBlockReference;
        private TextBlock associatedTextBlock;

        private string measureTextBlockElementID;

        private string savedWhiteSpace;
        private Thickness savedTextBlockPadding;

        public TextMeasurementService(UIElement parent)
        {
            Debug.Assert(parent is not null);

            measureTextBlockElementID = "";
            savedWhiteSpace = "pre";
            savedTextBlockPadding = new Thickness(double.NegativeInfinity);

            CreateMeasurementText(parent);
        }

        private void CreateMeasurementText(UIElement parent)
        {
            associatedTextBlock = new TextBlock
            {
                // Prevent the TextBlock from using an implicit style that could mess up the layout
                Style = null
            };
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(associatedTextBlock, parent);

            bool hasMarginDiv = false;
            if (associatedTextBlock.INTERNAL_AdditionalOutsideDivForMargins != null)
            {
                hasMarginDiv = true;

                var wrapperDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(associatedTextBlock.INTERNAL_AdditionalOutsideDivForMargins);
                wrapperDivStyle.position = "absolute";
                wrapperDivStyle.visibility = "hidden";
                wrapperDivStyle.left = "-100000px";
                wrapperDivStyle.top = "-100000px";
            }

            textBlockReference = associatedTextBlock.INTERNAL_OuterDomElement;
            textBlockDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBlockReference);
            textBlockDivStyle.position = "absolute";
            textBlockDivStyle.visibility = "hidden";
            textBlockDivStyle.height = "";
            textBlockDivStyle.width = "";
            textBlockDivStyle.borderWidth = "1";
            textBlockDivStyle.whiteSpace = "pre";
            textBlockDivStyle.left = hasMarginDiv ? "0px" : "-100000px";
            textBlockDivStyle.top = hasMarginDiv ? "0px" : "-100000px";

            associatedTextBlock.Text = "A";

            measureTextBlockElementID = ((INTERNAL_HtmlDomElementReference)textBlockReference).UniqueIdentifier;
        }

        public Size MeasureText(string uid,
                                string whiteSpace,
                                string overflowWrap,
                                Thickness padding,
                                double maxWidth,
                                string emptyVal)
        {
            if (textBlockReference == null)
            {
                return new Size();
            }

            string strPadding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
            string strMaxWidth = double.IsNaN(maxWidth) || double.IsInfinity(maxWidth) ? string.Empty : $"{maxWidth.ToInvariantString()}px";

            if (savedWhiteSpace == whiteSpace)
                whiteSpace = string.Empty;
            else
                savedWhiteSpace = whiteSpace;

            if (savedTextBlockPadding == padding)
                strPadding = "";
            else
                savedTextBlockPadding = padding;

            string javaScriptCodeToExecute = $@"document.measureTextBlock(""{measureTextBlockElementID}"",""{uid}"",""{whiteSpace}"",""{overflowWrap}"",""{strPadding}"",""{strMaxWidth}"",""{emptyVal}"")";
            string strTextSize = OpenSilver.Interop.ExecuteJavaScriptString(javaScriptCodeToExecute);
            Size measuredSize;
            int sepIndex = strTextSize != null ? strTextSize.IndexOf('|') : -1;
            if (sepIndex > -1)
            {
                double actualWidth = double.Parse(strTextSize.Substring(0, sepIndex), CultureInfo.InvariantCulture);
                double actualHeight = double.Parse(strTextSize.Substring(sepIndex + 1), CultureInfo.InvariantCulture);
                measuredSize = new Size(actualWidth + 1, actualHeight);
            }
            else
            {
                measuredSize = new Size(0, 0);
            }

            return measuredSize;
        }
    }
}