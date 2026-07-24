
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

using OpenSilver.Internal.Documents;
using System.Text;

namespace System.Windows.Documents;

/// <summary>
/// Represents an inline element that causes a new line to begin in 
/// content when rendered in a text container.
/// </summary>
public sealed class LineBreak : Inline
{
    internal override string TagName => "br";

    internal override ITextContainer OnCreateTextContainer() => TextContainerLineBreak.Instance;

    internal override void AppendHtml(StringBuilder builder) => builder.Append("<br />");

    private sealed class TextContainerLineBreak : ITextContainer
    {
        private TextContainerLineBreak() { }

        public static TextContainerLineBreak Instance { get; } = new();

        public string Text => "\n";

        public void OnTextContentChanged() => throw new NotSupportedException();
    }
}
