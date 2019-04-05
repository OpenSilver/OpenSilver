
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
