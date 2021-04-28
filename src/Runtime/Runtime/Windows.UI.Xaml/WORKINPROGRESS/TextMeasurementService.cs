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


        private string measureTextElementID;

        public TextMeasurementService()
        {
            measureTextElementID = "";
        }

        public void CreateMeasurementText(UIElement parent)
        {
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

            measureTextElementID = ((INTERNAL_HtmlDomElementReference)textBoxReference).UniqueIdentifier;
        }

        public bool IsTextMeasureDivID(string id)
        {
            if (measureTextElementID == id)
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

            return new Size(associatedTextBox.ActualWidth + 1, associatedTextBox.ActualHeight);
        }

    }
}
#endif