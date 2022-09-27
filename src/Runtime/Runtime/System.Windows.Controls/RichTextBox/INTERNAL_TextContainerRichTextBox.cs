
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
using System.Windows.Documents;
#else
using Windows.UI.Xaml.Documents;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class INTERNAL_TextContainerRichTextBox : INTERNAL_TextContainer
    {
        private readonly RichTextBox _parent;

        public INTERNAL_TextContainerRichTextBox(RichTextBox parent)
            : base(parent)
        {
            _parent = parent;
        }

        public override string Text => _parent.GetRawText();

        protected override void OnTextAddedOverride(TextElement textElement)
        {
            if (textElement is Paragraph paragraph)
            {
                string text = "";
                foreach (var inline in paragraph.Inlines)
                {
                    if (inline is Run run)
                    {
                        text += run.Text;
                    }
                    //TODO: support other Inlines
                }

                _parent.InsertText(text);
            }
            else if (textElement is Section)
            {
                //Does not support now
            }
        }

        protected override void OnTextRemovedOverride(TextElement textElement)
        {
            //TODO: implement
        }
    }
}
