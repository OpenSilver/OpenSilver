
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

using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Documents;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
#endif

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerRichTextBlock : TextContainer<RichTextBlock>
{
    public TextContainerRichTextBlock(RichTextBlock parent)
        : base(parent)
    {
    }

    public override string Text
    {
        get
        {
            string text = "";
            foreach (var block in Parent.Blocks)
            {
                text += block.GetContainerText();
            }

            return text;
        }
    }

    protected override void OnTextAddedOverride(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(Parent))
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, Parent);
        }
    }

    protected override void OnTextRemovedOverride(TextElement textElement)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(Parent))
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, Parent);
        }
    }
}
