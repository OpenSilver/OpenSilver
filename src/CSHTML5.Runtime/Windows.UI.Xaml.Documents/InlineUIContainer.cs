
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
