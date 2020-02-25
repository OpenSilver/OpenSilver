﻿using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    internal sealed partial class INTERNAL_TextContainerSpan : INTERNAL_TextContainer
    {
        #region Constructor
        internal INTERNAL_TextContainerSpan(Span span) : base(span)
        {

        }
        #endregion

        #region Public Properties
        public Span Span
        {
            get
            {
                return (Span)this.Parent;
            }
        }

        public override string Text
        {
            get
            {
                string text = string.Empty;
                foreach(Inline inline in this.Span.Inlines)
                {
                    if(inline is Run)
                    {
                        text += ((Run)inline).Text;
                    }
                    else if(inline is Span)
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
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.Span))
            {
                INTERNAL_TextContainerHelper.FromOwner(this.Span.Parent).EndChange();
            }
        }

        protected override void OnTextAddedOverride(TextElement textElement)
        {
            if(INTERNAL_VisualTreeManager.IsElementInVisualTree(this.Span))
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(textElement, this.Span);
            }
        }

        protected override void OnTextRemovedOverride(TextElement textElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this.Span))
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(textElement, this.Span);
            }
        }
        #endregion
    }
}