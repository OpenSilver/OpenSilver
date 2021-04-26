

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

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a hierarchical selectable item for the <see cref="T:System.Windows.Controls.TreeView" /> control.
    /// </summary>
    public class TreeViewItem : HeaderedItemsControl, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The DependencyProperty for the <see cref="IsSelected"/> property.
        ///     Default Value: false
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                    "IsSelected",
                    typeof(bool),
                    typeof(TreeViewItem),
                    new PropertyMetadata(false));

        /// <summary>
        ///     Specifies whether this item is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the <see cref="IsExpanded"/> property.
        ///     Default Value: false
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                    "IsExpanded",
                    typeof(bool),
                    typeof(TreeViewItem),
                    new PropertyMetadata(false));

        /// <summary>
        ///     Specifies whether this item has expanded its children or not.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
    }
}
#endif