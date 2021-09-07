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
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    internal sealed class INTERNAL_TextContainerRichTextBlock : INTERNAL_TextContainer
    {
        public INTERNAL_TextContainerRichTextBlock(RichTextBlock parent)
            :base(parent)
        {

        }

        public RichTextBlock RichTextBlock => (RichTextBlock)Parent;

        public override string Text
        {
            get
            {
                string text = "";
                foreach(var block in RichTextBlock.Blocks)
                {
                    text += block.GetContainerText();
                }

                return text;
            }
        }

        protected override void OnTextAddedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.RichTextBlock))
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, this.RichTextBlock);
            }
        }

        protected override void OnTextRemovedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.RichTextBlock))
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, this.RichTextBlock);
            }
        }
    }
}
