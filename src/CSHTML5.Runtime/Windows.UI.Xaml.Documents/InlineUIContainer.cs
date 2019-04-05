
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

#if NOT_YET_IMPLEMENTED // This class is not finished because there is an issue displaying the element "inline" instead of on a new line.
    /// <summary>
    /// Provides an inline content element that enables UIElement types to be embedded in the content of a (Rich)TextBlock.
    /// </summary>
    [ContentProperty("Child")]
    public sealed class InlineUIContainer : Inline
    {
        private UIElement _child;

        /// <summary>
        /// Gets or sets the UIElement hosted by the InlineUIContainer.
        /// </summary>
        public UIElement Child
        {
            get
            {
                return _child;
            }
            set
            {
                if (this._isLoaded)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_child, this);
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(value, this);
                }
                _child = value;
            }
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);
        }
    }
#endif
}
