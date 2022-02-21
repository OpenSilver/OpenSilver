using CSHTML5.Internal;
using System;
using System.Globalization;
using OpenSilver.Internal;
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
        
        Size MeasureTextBlock(string text, 
                              double fontSize, 
                              FontFamily fontFamily, 
                              FontStyle style, 
                              FontWeight weight, 
                              /*FontStretch stretch,*/ 
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

        private double savedTextBlockFontSize;
        private string savedTextBlockFontFamily;
        private FontStyle savedTextBlockFontStyle;
        private FontWeight savedTextBlockFontWeight;
        private TextWrapping savedTextBlockTextWrapping;
        private Thickness savedTextBlockPadding;

        public TextMeasurementService()
        {
            measureTextBoxElementID = "";
            measureTextBlockElementID = "";

            savedTextBlockFontSize = 0;
            savedTextBlockFontFamily = "";
#if MIGRATION
            savedTextBlockFontStyle = FontStyles.Normal;
#else
            savedTextBlockFontStyle = FontStyle.Normal;
#endif
            savedTextBlockFontWeight.Weight = 0;
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

            associatedTextBox = new TextBox();
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(associatedTextBox, parent);
            textBoxReference = associatedTextBox.INTERNAL_OuterDomElement;
            textBoxDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBoxReference);
            textBoxDivStyle.position = "absolute";
            textBoxDivStyle.visibility = "hidden";
            textBoxDivStyle.height = "";
            textBoxDivStyle.width = "";
            textBoxDivStyle.top = "0px";
            textBoxDivStyle.borderWidth = "1";

            measureTextBoxElementID = ((INTERNAL_HtmlDomElementReference)textBoxReference).UniqueIdentifier;

            // For TextBlock
            if (associatedTextBlock != null)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(associatedTextBlock, parent);
            }

            associatedTextBlock = new TextBlock();
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(associatedTextBlock, parent);
            textBlockReference = associatedTextBlock.INTERNAL_OuterDomElement;
            textBlockDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBlockReference);
            textBlockDivStyle.position = "absolute";
            textBlockDivStyle.visibility = "hidden";
            textBlockDivStyle.height = "";
            textBlockDivStyle.width = "";
            textBlockDivStyle.top = "100px";
            textBlockDivStyle.borderWidth = "1";
            textBlockDivStyle.whiteSpace = "pre";
            associatedTextBlock.Text = "A";
            measureTextBlockElementID = ((INTERNAL_HtmlDomElementReference)textBlockReference).UniqueIdentifier;
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"document.measureTextBlockElement=$0", textBlockReference);
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

        public Size MeasureTextBlock(string text, 
                                     double fontSize, 
                                     FontFamily fontFamily, 
                                     FontStyle style, 
                                     FontWeight weight, 
                                     /*FontStretch stretch,*/ 
                                     TextWrapping wrapping, 
                                     Thickness padding, 
                                     double maxWidth)
        {
            if (textBlockReference == null)
            {
                return new Size();
            }
#if OPENSILVER
            string strText = String.IsNullOrEmpty(text) ? "A" : INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text);
#elif BRIDGE
            string strText = String.IsNullOrEmpty(text) ? "A" : text;
#endif
            string strFontSize = (Math.Floor(fontSize * 1000) / 1000).ToString(CultureInfo.InvariantCulture) + "px";
            string strFontFamily = fontFamily != null ? INTERNAL_FontsHelper.LoadFont(fontFamily.Source, (UIElement)associatedTextBlock) : "-";
            string strFontStyle = style.ToString().ToLower();
            string strFontWeight = weight.Weight.ToInvariantString();
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

            if (savedTextBlockFontSize == fontSize)
                strFontSize = "";
            else
                savedTextBlockFontSize = fontSize;

            if (savedTextBlockFontFamily == strFontFamily)
                strFontFamily = "";
            else
                savedTextBlockFontFamily = strFontFamily;

            if (savedTextBlockFontStyle == style)
                strFontStyle = "";
            else
                savedTextBlockFontStyle = style;

            if (savedTextBlockFontWeight == weight)
                strFontWeight = "";
            else
                savedTextBlockFontWeight = weight;

            if (savedTextBlockTextWrapping == wrapping)
                strTextWrapping = "";
            else
                savedTextBlockTextWrapping = wrapping;

            if (savedTextBlockPadding == padding)
                strPadding = "";
            else
                savedTextBlockPadding = padding;

            // UGiacobbi 18022022 If we pass a quoted font family we need to escape it
            var escapedstrFontFamily = strFontFamily.Replace("\"", "\\\"");

            string javaScriptCodeToExecute = $@"document.measureTextBlock(""{strText}"",""{strFontSize}"",""{escapedstrFontFamily}"",""{strFontStyle}"",""{strFontWeight}"",""{strTextWrapping}"",""{strPadding}"",""{strWidth}"",""{strMaxWidth}"")";
#if OPENSILVER
            string strTextSize = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript(javaScriptCodeToExecute));
#elif BRIDGE
            string strTextSize = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("eval($0)", javaScriptCodeToExecute));
#endif
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