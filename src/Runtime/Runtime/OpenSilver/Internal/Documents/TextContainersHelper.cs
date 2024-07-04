
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace OpenSilver.Internal.Documents;

internal static class TextContainersHelper
{
    public static ITextContainer Create(DependencyObject d) =>
        d switch
        {
            TextBlock textBlock => new TextContainerTextBlock(textBlock),
            Run run => new TextContainerRun(run),
            Span span => new TextContainerSpan(span),
            LineBreak => TextContainerLineBreak.Instance,
            RichTextBlock richTextBlock => new TextContainerRichTextBlock(richTextBlock),
            Paragraph paragraph => new TextContainerParagraph(paragraph),
            Section section => new TextContainerSection(section),
            RichTextBox richTextBox => new TextContainerRichTextBox(richTextBox),
            _ => null,
        };

    public static ITextContainer Get(DependencyObject d) =>
        d switch
        {
            TextElement textElement => textElement.TextContainer,
            TextBlock textBlock => textBlock.Inlines.TextContainer,
            RichTextBlock richTextBlock => richTextBlock.Blocks.TextContainer,
            RichTextBox richTextBox => richTextBox.Blocks.TextContainer,
            _ => null,
        };
}