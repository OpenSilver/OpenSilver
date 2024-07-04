
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

internal sealed class TextContainerSpan : ITextContainer
{
    private readonly Span _span;

    internal TextContainerSpan(Span span)
    {
        Debug.Assert(span is not null);
        _span = span;
    }

    public string Text => string.Join(string.Empty, _span.Inlines.InternalItems.Select(i => i.TextContainer.Text));

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_span)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_span))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, _span, index);
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_span))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, _span);
        }
    }
}
