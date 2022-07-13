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

using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    internal class INTERNAL_TextContainerParagraph : INTERNAL_TextContainer
    {
        public INTERNAL_TextContainerParagraph(Paragraph parent)
            :base(parent)
        {

        }

        public Paragraph Paragraph => (Paragraph)this.Parent;

        public override string Text
        {
            get
            {
                string text = "";
                foreach(var inline in ((Paragraph)Parent).Inlines)
                {
                    if(inline is Run run)
                    {
                        text += run.Text;
                    }
                    else if(inline is LineBreak)
                    {
                        text += "\\n";
                    }
                    else if(inline is Span span)
                    {
                        var textContainer = new INTERNAL_TextContainerSpan(span);
                        text += textContainer.Text;
                    }
                }

                return text;
            }
        }

        protected override void OnTextAddedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.Paragraph))
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, this.Paragraph);
            }
        }

        protected override void OnTextRemovedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.Paragraph))
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, this.Paragraph);
            }
        }
    }
}
