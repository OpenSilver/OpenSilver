
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
using System.Linq;
using System.Windows.Documents;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerSection : ITextContainer
{
    private readonly Section _section;

    public TextContainerSection(Section section)
    {
        _section = section ?? throw new ArgumentNullException(nameof(section));
    }

    public string Text => string.Join("\n", _section.Blocks.InternalItems.Select(b => b.TextContainer.Text));

    public void OnTextAdded(TextElement textElement, int index)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_section))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, _section, index);
        }
    }

    public void OnTextRemoved(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_section))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, _section);
        }
    }

    public void OnTextContentChanged() { }
}
