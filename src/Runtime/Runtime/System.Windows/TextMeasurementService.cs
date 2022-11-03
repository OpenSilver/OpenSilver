using CSHTML5.Internal;
using System;
using System.Globalization;
using OpenSilver.Internal;
using CSHTML5;
#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal interface ITextMeasurementService
    {
        Size Measure(string text, 
                     double fontSize, 
                     FontFamily fontFamily, 
                     FontStyle style, 
                     FontWeight weight, 
                     /*FontStretch stretch,*/ 
                     TextWrapping wrapping, 
                     Thickness padding, 
                     double maxWidth);
        
        Size MeasureTextBlock(string uid,
                              TextWrapping wrapping, 
                              Thickness padding, 
                              double maxWidth);
        
        void CreateMeasurementText(UIElement parent);
        
        bool IsTextMeasureDivID(string id);
    }

    /// <summary>
    /// Measure Text Block width and height from html element.
    /// </summary>
    internal class TextMeasurementService : ITextMeasurementService
    {
#if OPENSILVER
        private INTERNAL_HtmlDomStyleReference textBoxDivStyle;
#elif BRIDGE
        private dynamic textBoxDivStyle;
#endif

        private object textBoxReference;
        private TextBox associatedTextBox;

#if OPENSILVER
        private INTERNAL_HtmlDomStyleReference textBlockDivStyle;
#elif BRIDGE
        private dynamic textBlockDivStyle;
#endif

        private object textBlockReference;
        private TextBlock associatedTextBlock;


        private string measureTextBoxElementID;
        private string measureTextBlockElementID;

        private TextWrapping savedTextBlockTextWrapping;
        private Thickness savedTextBlockPadding;

        public TextMeasurementService()
        {
            measureTextBoxElementID = "";
            measureTextBlockElementID = "";

            savedTextBlockTextWrapping = TextWrapping.NoWrap;
            savedTextBlockPadding = new Thickness(double.NegativeInfinity);
        }

        public void CreateMeasurementText(UIElement parent)
        {
            // For TextBox
            if (associatedTextBox != null)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(associatedTextBox, parent);
            }

            associatedTextBox = new TextBox
            {
                // Prevent the TextBox from using an implicit style that could mess up the layout
                Style = null
            };
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(associatedTextBox, parent);

            bool hasMarginDiv = false;
            if (associatedTextBox.INTERNAL_AdditionalOutsideDivForMargins != null)
            {
                hasMarginDiv = true;

                var wrapperDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(associatedTextBox.INTERNAL_AdditionalOutsideDivForMargins);
                wrapperDivStyle.position = "absolute";
                wrapperDivStyle.visibility = "hidden";
                wrapperDivStyle.left = "-100000px";
                wrapperDivStyle.top = "-100000px";
            }

            textBoxReference = associatedTextBox.INTERNAL_OuterDomElement;
            textBoxDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBoxReference);
            textBoxDivStyle.position = "absolute";
            textBoxDivStyle.visibility = "hidden";
            textBoxDivStyle.height = "";
            textBoxDivStyle.width = "";
            textBoxDivStyle.borderWidth = "1";
            textBoxDivStyle.left = hasMarginDiv ? "0px" : "-100000px";
            textBoxDivStyle.top = hasMarginDiv ? "0px" : "-100000px";

            measureTextBoxElementID = ((INTERNAL_HtmlDomElementReference)textBoxReference).UniqueIdentifier;

            // For TextBlock
            if (associatedTextBlock != null)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(associatedTextBlock, parent);
            }

            associatedTextBlock = new TextBlock
            {
                // Prevent the TextBlock from using an implicit style that could mess up the layout
                Style = null
            };
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(associatedTextBlock, parent);

            hasMarginDiv = false;
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
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"document.measureTextBlockElement={INTERNAL_InteropImplementation.GetVariableStringForJS(textBlockReference)}");
        }

        public bool IsTextMeasureDivID(string id)
        {
            if (measureTextBoxElementID == id || measureTextBlockElementID == id)
                return true;

            return false;
        }

        public Size Measure(string text, 
                            double fontSize, 
                            FontFamily fontFamily, 
                            FontStyle style, 
                            FontWeight weight, 
                            /*FontStretch stretch,*/ 
                            TextWrapping wrapping, 
                            Thickness padding, 
                            double maxWidth)
        {
            //Console.WriteLine($"MeasureTextBox maxWidth {maxWidth}");
            if (textBoxReference == null)
            {
                return new Size();
            }

            associatedTextBox.Text = String.IsNullOrEmpty(text) ? "A" : text;
            associatedTextBox.FontFamily = fontFamily;
            associatedTextBox.FontStyle = style;
            associatedTextBox.FontWeight = weight;
            //associatedTextBox.FontStretch = stretch;
            associatedTextBox.Padding = padding;
            associatedTextBox.FontSize = fontSize;

            associatedTextBox.TextWrapping = wrapping;

            if (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
            {
                textBoxDivStyle.width = "";
                textBoxDivStyle.maxWidth = "";
            }
            else
            {
                textBoxDivStyle.width = maxWidth.ToString(CultureInfo.InvariantCulture) +"px";
                textBoxDivStyle.maxWidth = maxWidth.ToString(CultureInfo.InvariantCulture) + "px";
            }

            // On Simulator, it needs time to get actualwidth and actualheight
#if OPENSILVER
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
#elif BRIDGE
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                global::System.Threading.Thread.Sleep(20);
            }

            return new Size(associatedTextBox.ActualWidth + 2, associatedTextBox.ActualHeight);
        }

        public Size MeasureTextBlock(string uid,
                                     TextWrapping wrapping, 
                                     Thickness padding, 
                                     double maxWidth)
        {
            if (textBlockReference == null)
            {
                return new Size();
            }

            string strTextWrapping = wrapping == TextWrapping.Wrap ? "pre-wrap" : "pre";
            string strPadding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
            string strWidth = "";
            string strMaxWidth = "";
            if (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
            {
                strWidth = "";
                strMaxWidth = "";
            }
            else
            {
                strWidth = maxWidth.ToInvariantString() + "px";
                strMaxWidth = maxWidth.ToInvariantString() + "px";
            }

            if (savedTextBlockTextWrapping == wrapping)
                strTextWrapping = "";
            else
                savedTextBlockTextWrapping = wrapping;

            if (savedTextBlockPadding == padding)
                strPadding = "";
            else
                savedTextBlockPadding = padding;

            string javaScriptCodeToExecute = $@"document.measureTextBlock(""{uid}"",""{strTextWrapping}"",""{strPadding}"",""{strWidth}"",""{strMaxWidth}"")";
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