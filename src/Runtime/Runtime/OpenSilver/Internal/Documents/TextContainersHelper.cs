
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
using System.Windows.Documents;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
#endif

namespace OpenSilver.Internal.Documents;

internal static class TextContainersHelper
{
    public static ITextContainer FromOwner(DependencyObject parent)
    {
        return parent switch
        {
            TextBlock textBlock => new TextContainerTextBlock(textBlock),
            Span span => new TextContainerSpan(span),
            RichTextBlock richTextBlock => new TextContainerRichTextBlock(richTextBlock),
            Paragraph paragraph => new TextContainerParagraph(paragraph),
            Section section => new TextContainerSection(section),
            RichTextBox richTextBox => new TextContainerRichTextBox(richTextBox),
            _ => null,
        };
    }
}