
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

internal sealed class TextContainerRichTextBlock : ITextContainer
{
    private readonly RichTextBlock _rtb;

    public TextContainerRichTextBlock(RichTextBlock rtb)
    {
        Debug.Assert(rtb is not null);
        _rtb = rtb;
    }

    public string Text => string.Join("\n", _rtb.Blocks.InternalItems.Select(b => b.TextContainer.Text));

    public void OnTextContentChanged() => _rtb.InvalidateMeasure();
}
