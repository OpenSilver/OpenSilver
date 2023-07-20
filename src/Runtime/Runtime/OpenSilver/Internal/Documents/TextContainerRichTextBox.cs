
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

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Documents;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
#endif

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerRichTextBox : TextContainer<RichTextBox>
{
    public TextContainerRichTextBox(RichTextBox parent)
        : base(parent)
    {
    }

    public override string Text => Parent.GetRawText();

    protected override void OnTextAddedOverride(TextElement textElement)
    {
        if (textElement is Paragraph paragraph)
        {
            string text = "";
            foreach (var inline in paragraph.Inlines)
            {
                if (inline is Run run)
                {
                    text += run.Text;
                }
                //TODO: support other Inlines
            }

            Parent.InsertText(text);
        }
        else if (textElement is Section)
        {
            //Does not support now
        }
    }

    protected override void OnTextRemovedOverride(TextElement textElement)
    {
        //TODO: implement
    }
}
