
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
using System.Windows.Controls;
using System.Windows.Documents;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerRichTextBox : ITextContainer
{
    private readonly RichTextBox _rtb;

    public TextContainerRichTextBox(RichTextBox rtb)
    {
        _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
    }

    public string Text => _rtb.GetRawText();

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (textElement is Paragraph paragraph)
        {
            string text = "";
            foreach (var inline in paragraph.Inlines.InternalItems)
            {
                if (inline is Run run)
                {
                    text += run.Text;
                }
                //TODO: support other Inlines
            }

            _rtb.InsertText(text);
        }
        else if (textElement is Section)
        {
            //Does not support now
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        //TODO: implement
    }

    public void OnTextContentChanged() { }
}
