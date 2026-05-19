
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
}
