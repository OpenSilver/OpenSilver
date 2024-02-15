
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

using System.Linq;
using System;
using System.Windows.Documents;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerParagraph : ITextContainer
{
    private readonly Paragraph _paragraph;

    public TextContainerParagraph(Paragraph paragraph)
    {
        _paragraph = paragraph ?? throw new ArgumentNullException(nameof(paragraph));
    }

    public string Text => string.Join(string.Empty, _paragraph.Inlines.InternalItems.Select(i => i.TextContainer.Text));

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_paragraph))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, _paragraph, index);
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_paragraph))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, _paragraph);
        }
    }

    public void OnTextContentChanged() { }
}
