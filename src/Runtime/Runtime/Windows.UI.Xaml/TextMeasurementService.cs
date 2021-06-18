using CSHTML5.Internal;
using System;
using System.Globalization;

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


        public TextMeasurementService()
        {
            measureTextBoxElementID = "";
            measureTextBlockElementID = "";
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
            measureTextBlockElementID = ((INTERNAL_HtmlDomElementReference)textBlockReference).UniqueIdentifier;
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
                textBoxDivStyle.width = String.Format(CultureInfo.InvariantCulture, "{0}px", maxWidth);
                textBoxDivStyle.maxWidth = String.Format(CultureInfo.InvariantCulture, "{0}px", maxWidth);
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

            associatedTextBlock.Text = String.IsNullOrEmpty(text) ? "A" : text;
            associatedTextBlock.FontFamily = fontFamily;
            associatedTextBlock.FontStyle = style;
            associatedTextBlock.FontWeight = weight;
            //associatedTextBlock.FontStretch = stretch;
            associatedTextBlock.Padding = padding;
            associatedTextBlock.FontSize = fontSize;

            associatedTextBlock.TextWrapping = wrapping;

            if (double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
            {
                textBlockDivStyle.width = "";
                textBlockDivStyle.maxWidth = "";
            }
            else
            {
                textBlockDivStyle.width = String.Format(CultureInfo.InvariantCulture, "{0}px", maxWidth);
                textBlockDivStyle.maxWidth = String.Format(CultureInfo.InvariantCulture, "{0}px", maxWidth);
            }

            // On Simulator, it needs time to get actualwidth and actualheight
#if OPENSILVER
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                global::System.Threading.Thread.Sleep(20);
            }

            return new Size(associatedTextBlock.ActualWidth + 1, associatedTextBlock.ActualHeight);
        }
    }
}