

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

#if WORKINPROGRESS

using CSHTML5.Internal;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    // This class is not finished because there is an issue displaying the element "inline" instead of on a new line.
    /// <summary>
    /// Provides an inline content element that enables UIElement types to be embedded in the content of a (Rich)TextBlock.
    /// </summary>
    [ContentProperty("Child")]
    public sealed partial class InlineUIContainer : Inline
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
            base.INTERNAL_OnAttachedToVisualTree();

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);
        }
    }
}

#endif
