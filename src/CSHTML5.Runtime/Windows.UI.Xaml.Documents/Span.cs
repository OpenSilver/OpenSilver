

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
    public partial class Span : Inline
    {
        private readonly InlineCollection _inlines;

        /// <summary>
        /// Initializes a new instance of the Span class.
        /// </summary>
        public Span()
        {
            this._inlines = new InlineCollection(this);
        }

        /// <summary>
        ///Gets an InlineCollection containing the top-level inline elements that include the contents of Span.
        /// </summary>
        public InlineCollection Inlines
        {
            get
            {
                return this._inlines;
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
            foreach (Inline child in _inlines)
            {
#if REWORKLOADED
                this.AddVisualChild(child);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
#endif
            }
        }
    }
}
