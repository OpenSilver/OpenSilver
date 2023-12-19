
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

using System.Windows.Documents;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerLineBreak : ITextContainer
{
    private TextContainerLineBreak() { }

    public static TextContainerLineBreak Instance { get; } = new();

    public string Text => "\n";

    public void OnTextContentChanged() => throw new NotSupportedException();

    public void OnTextAdded(TextElement textElement, int index) => throw new NotSupportedException();

    public void OnTextRemoved(TextElement textElement) => throw new NotSupportedException();
}
