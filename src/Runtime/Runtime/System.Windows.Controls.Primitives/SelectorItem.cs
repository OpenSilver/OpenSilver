
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

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Provides a base class for ListBoxItem, ComboBoxItem, or potentially for other item types.
    /// </summary>
    public partial class SelectorItem : ContentControl
    {
        /// <summary>
        /// Provides base class initialization behavior for SelectorItem-derived classes.
        /// </summary>
        protected SelectorItem()
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the item is selected in a selector.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected), 
                typeof(bool), 
                typeof(SelectorItem), 
                new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectorItem container = (SelectorItem)d;
            if (container.ParentSelector != null)
            {
                container.ParentSelector.NotifyIsSelectedChanged(container, (bool)e.NewValue);
                if (container.ParentSelector is ListBox && (bool)e.NewValue == true)
                {
                    ((ListBox)container.ParentSelector).ScrollIntoViewInternal(container as ListBoxItem);
                }
            }

            container.UpdateVisualStates();
        }

        internal Selector ParentSelector { get; set; }
    }
}
