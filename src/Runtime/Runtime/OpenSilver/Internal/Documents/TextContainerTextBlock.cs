
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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerTextBlock : ITextContainer
{
    private readonly TextBlock _textblock;

    internal TextContainerTextBlock(TextBlock tb)
    {
        Debug.Assert(tb is not null);
        _textblock = tb;
    }

    public string Text => string.Join(string.Empty, _textblock.Inlines.InternalItems.Select(i => i.TextContainer.Text));

    public void OnTextContentChanged() => _textblock.OnTextContentChanged();

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_textblock))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, _textblock, index);
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_textblock))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, _textblock);
        }
    }
}
