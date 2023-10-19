
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

using System.Windows.Documents;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerParagraph : TextContainer<Paragraph>
{
    public TextContainerParagraph(Paragraph parent)
        : base(parent)
    {
    }

    public override string Text
    {
        get
        {
            string text = "";
            foreach (var inline in Parent.Inlines)
            {
                if (inline is Run run)
                {
                    text += run.Text;
                }
                else if (inline is LineBreak)
                {
                    text += "\\n";
                }
                else if (inline is Span span)
                {
                    var textContainer = new TextContainerSpan(span);
                    text += textContainer.Text;
                }
            }

            return text;
        }
    }

    protected override void OnTextAddedOverride(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(Parent))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, Parent);
        }
    }

    protected override void OnTextRemovedOverride(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(Parent))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, Parent);
        }
    }
}
