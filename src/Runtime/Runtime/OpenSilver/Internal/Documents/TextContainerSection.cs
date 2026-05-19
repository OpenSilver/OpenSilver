
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

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerSection : ITextContainer
{
    private readonly Section _section;

    public TextContainerSection(Section section)
    {
        Debug.Assert(section is not null);
        _section = section;
    }

    public string Text => string.Join("\n", _section.Blocks.InternalItems.Select(b => b.TextContainer.Text));

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_section)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }
}
