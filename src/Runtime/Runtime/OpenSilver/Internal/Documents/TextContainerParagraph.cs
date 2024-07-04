
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
using System.Windows.Documents;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerParagraph : ITextContainer
{
    private readonly Paragraph _paragraph;

    public TextContainerParagraph(Paragraph paragraph)
    {
        Debug.Assert(paragraph is not null);
        _paragraph = paragraph;
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

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_paragraph)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }
}
