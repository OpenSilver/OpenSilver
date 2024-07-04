
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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerRichTextBox : ITextContainer
{
    private readonly RichTextBox _richTextBox;

    public TextContainerRichTextBox(RichTextBox richTextBox)
    {
        Debug.Assert(richTextBox is not null);
        _richTextBox = richTextBox;
    }

    public string Text => string.Empty;

    public void OnTextContentChanged() => _richTextBox.InvalidateUI();

    public void OnTextAdded(TextElement textElement, int index) { }

    public void OnTextRemoved(TextElement textElement) { }
}
