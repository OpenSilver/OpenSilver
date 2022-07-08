

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


#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    internal partial class INTERNAL_TextContainerHelper
    {
        public static INTERNAL_ITextContainer FromOwner(DependencyObject parent)
        {
            INTERNAL_TextContainer container = null;
            if (parent is TextBlock)
            {
                container = new INTERNAL_TextContainerTextBlock((TextBlock)parent);
            }
            else if (parent is Span)
            {
                container = new INTERNAL_TextContainerSpan((Span)parent);
            }
            else if (parent is RichTextBlock)
            {
                container = new INTERNAL_TextContainerRichTextBlock((RichTextBlock)parent);
            }
            else if (parent is Paragraph)
            {
                container = new INTERNAL_TextContainerParagraph((Paragraph)parent);
            }
            else if (parent is Section)
            {
                container = new INTERNAL_TextContainerSection((Section)parent);
            }
            else if(parent is RichTextBox)
            {
                container = new INTERNAL_TextContainerRichTextBox((RichTextBox)parent);
            }

            return container;
        }
    }
}