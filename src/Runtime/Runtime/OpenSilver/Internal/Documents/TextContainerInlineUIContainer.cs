
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

using System.Windows.Documents;
using System.Windows.Media;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerInlineUIContainer : ITextContainer
{
    private readonly InlineUIContainer _uiContainer;

    internal TextContainerInlineUIContainer(InlineUIContainer uiContainer)
    {
        _uiContainer = uiContainer;
    }

    public string Text => string.Empty;

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_uiContainer)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }
}
