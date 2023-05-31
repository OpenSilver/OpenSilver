﻿
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

using System;
using System.Windows.Markup;
using CSHTML5.Internal;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Groups other Inline flow content elements.
    /// </summary>
    [ContentProperty(nameof(Inlines))]
    public class Span : Inline
    {
        internal override int VisualChildrenCount
        {
            get { return this.Inlines.Count; }
        }

        internal override UIElement GetVisualChild(int index)
        {
            if (index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (Inline)this.Inlines[index];
        }

        /// <summary>
        /// Initializes a new instance of the Span class.
        /// </summary>
        public Span()
        {
            Inlines = new InlineCollection(this);
        }

        /// <summary>
        ///Gets an InlineCollection containing the top-level inline elements that include the contents of Span.
        /// </summary>
        public InlineCollection Inlines { get; }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            foreach (Inline child in this.Inlines)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
            }
        }
    }
}
