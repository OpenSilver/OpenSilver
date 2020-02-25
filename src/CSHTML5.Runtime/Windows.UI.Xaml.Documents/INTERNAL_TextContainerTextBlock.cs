﻿using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    internal sealed partial class INTERNAL_TextContainerTextBlock : INTERNAL_TextContainer
    {
        #region Constructor
        internal INTERNAL_TextContainerTextBlock(TextBlock parent) : base(parent)
        {

        }
        #endregion

        #region Public Properties
        public TextBlock TextBlock
        {
            get
            {
                return (TextBlock)this.Parent;
            }
        }

        public override string Text
        {
            get
            {
                string text = string.Empty;
                foreach (Inline inline in this.TextBlock.Inlines)
                {
                    if (inline is Run)
                    {
                        text += ((Run)inline).Text;
                    }
                    else if (inline is Span)
                    {
                        text += new INTERNAL_TextContainerSpan(((Span)inline)).Text;
                    }
                    else
                    {
                        //do nothing
                        //note: should we throw an exception ?
                    }
                }
                return text;
            }
        }
        #endregion

        #region Public Methods
        public override void EndChange()
        {
            this.TextBlock.SetTextPropertyNoCallBack(this.Text);
        }

        protected override void OnTextAddedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.TextBlock))
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, this.TextBlock);
            }
        }

        protected override void OnTextRemovedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.TextBlock))
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, this.TextBlock);
            }
        }
        #endregion
    }
}
