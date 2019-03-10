
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
using System.Windows.Markup;


#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Groups other Inline flow content elements.
    /// </summary>
    [ContentProperty("Inlines")]
    public class Span : Inline
    {
        InlineCollection _inlines;

        /// <summary>
        /// Initializes a new instance of the Span class.
        /// </summary>
        public Span()
        {
        }

        /// <summary>
        ///Gets an InlineCollection containing the top-level inline elements that include the contents of Span.
        /// </summary>
        public InlineCollection Inlines
        {
            get
            {
                if (_inlines == null)
                    _inlines = new InlineCollection();
                return _inlines;
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic span = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("span", parentRef, this);
            domElementWhereToPlaceChildren = span;
            return span;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            if (_inlines != null)
            {
                foreach (Inline child in _inlines)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
                }
            }
        }
    }
}
