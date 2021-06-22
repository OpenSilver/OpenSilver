#if WORKINPROGRESS
#if MIGRATION
using CSHTML5.Internal;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public interface ITextMeasurementService
    {
        Size Measure(string text, double fontSize, FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, TextWrapping wrapping, Thickness padding, double maxWidth);
        Size MeasureTextBlock(string text, double fontSize, FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, TextWrapping wrapping, Thickness padding, double maxWidth);
        void CreateMeasurementText(UIElement parent);
        bool IsTextMeasureDivID(string id);
    }

    //
    // Summary:
    //     Measure Text Block width and height from html element.
    public class TextMeasurementService : ITextMeasurementService
    {
#if CSHTML5NETSTANDARD
        private INTERNAL_HtmlDomStyleReference
#else
        private dynamic
#endif
            textBoxDivStyle;
        private object textBoxReference;
        private TextBox associatedTextBox;

#if CSHTML5NETSTANDARD
        private INTERNAL_HtmlDomStyleReference
#else
        private dynamic
#endif
            textBlockDivStyle;
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
            textBoxReference = (INTERNAL_HtmlDomElementReference)associatedTextBox.INTERNAL_OuterDomElement;
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
            textBlockReference = (INTERNAL_HtmlDomElementReference)associatedTextBlock.INTERNAL_OuterDomElement;
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

        public Size Measure(string text, double fontSize, FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, TextWrapping wrapping, Thickness padding, double maxWidth)
        {
            //Console.WriteLine($"MeasureTextBox maxWidth {maxWidth}");
            if (textBoxReference == null)
            {
                return Size.Zero;
            }

            associatedTextBox.Text = String.IsNullOrEmpty(text) ? "A" : text;
            associatedTextBox.FontFamily = fontFamily;
            associatedTextBox.FontStyle = style;
            associatedTextBox.FontWeight = weight;
            associatedTextBox.FontStretch = stretch;
            associatedTextBox.Padding = padding;
            associatedTextBox.FontSize = fontSize;

            associatedTextBox.TextWrapping = wrapping;

            if (maxWidth.IsNaN() || double.IsInfinity(maxWidth))
            {
                textBoxDivStyle.width = "";
                textBoxDivStyle.maxWidth = "";
            }
            else
            {
                textBoxDivStyle.width = String.Format("{0}px", maxWidth);
                textBoxDivStyle.maxWidth = String.Format("{0}px", maxWidth);
            }

            // On Simulator, it needs time to get actualwidth and actualheight
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
            {
                System.Threading.Thread.Sleep(20);
            }

            return new Size(associatedTextBox.ActualWidth + 2, associatedTextBox.ActualHeight);
        }

        public Size MeasureTextBlock(string text, double fontSize, FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, TextWrapping wrapping, Thickness padding, double maxWidth)
        {
            if (textBlockReference == null)
            {
                return Size.Zero;
            }

            associatedTextBlock.Text = String.IsNullOrEmpty(text) ? "A" : text;
            associatedTextBlock.FontFamily = fontFamily;
            associatedTextBlock.FontStyle = style;
            associatedTextBlock.FontWeight = weight;
            associatedTextBlock.FontStretch = stretch;
            associatedTextBlock.Padding = padding;
            associatedTextBlock.FontSize = fontSize;

            associatedTextBlock.TextWrapping = wrapping;

            if (maxWidth.IsNaN() || double.IsInfinity(maxWidth))
            {
                textBlockDivStyle.width = "";
                textBlockDivStyle.maxWidth = "";
            }
            else
            {
                textBlockDivStyle.width = String.Format("{0}px", maxWidth);
                textBlockDivStyle.maxWidth = String.Format("{0}px", maxWidth);
            }

            // On Simulator, it needs time to get actualwidth and actualheight
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
            {
                System.Threading.Thread.Sleep(20);
            }

            return new Size(associatedTextBlock.ActualWidth + 1, associatedTextBlock.ActualHeight);
        }
    }
}
#endif