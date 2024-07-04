
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

internal sealed class TextContainerRichTextBlock : ITextContainer
{
    private readonly RichTextBlock _rtb;

    public TextContainerRichTextBlock(RichTextBlock rtb)
    {
        Debug.Assert(rtb is not null);
        _rtb = rtb;
    }

    public string Text => string.Join("\n", _rtb.Blocks.InternalItems.Select(b => b.TextContainer.Text));

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_rtb))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, _rtb, index);
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_rtb))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, _rtb);
        }
    }

    public void OnTextContentChanged() => _rtb.InvalidateMeasure();
}
