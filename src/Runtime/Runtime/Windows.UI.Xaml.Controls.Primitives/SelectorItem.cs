

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
using System;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Provides a base class for ListBoxItem, ComboBoxItem, or potentially for other item types.
    /// </summary>
    public partial class SelectorItem : ButtonBase //todo: it originally inherited from ContentControl so maybe it would be better to inherit from it. NOTE: we inherited from ButtonBase so that we could use the Click event
    {
        /// <summary>
        /// The business object that the SelectorItem (ListBoxItem, ComboBoxItem, etc.) represents. For example, if the ItemsSource is a collection of business objects, each of them will be stored in this variable.
        /// </summary>
        internal object INTERNAL_CorrespondingItem = null;
        internal Selector INTERNAL_ParentSelectorControl = null;
        Brush _backgroundBeforeBeingSelected = null;
        Brush _foregroundBeforeBeingSelected = null;
        bool _hasEverBeenSelected = false;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return (this.Content ?? "").ToString(); // This is important for example for displaying values defined as <ComboBoxItem>This is the value</ComboBoxItem> in a native ComboBox.
        }

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
        /// Identifies the IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectorItem), new PropertyMetadata(false, IsSelected_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectorItem selectorItem = (SelectorItem)d;
            selectorItem.HandleIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        virtual internal protected void HandleIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //todo: remove what's inside the "if" above once we will have defined a default style for the ItemsControl, that does about the same thing.
                if (INTERNAL_ParentSelectorControl != null)
                {
                    Selector selector = INTERNAL_ParentSelectorControl;

                    if (selector.ItemContainerStyle == null)
                    {
                        //bool newValue = (bool)e.NewValue;
                        if (newValue != oldValue)
                        {
                            if (newValue)
                            {
                                // Remember the values of the Background and Foreground before selection, in order to be able to revert them back when needed:
                                _backgroundBeforeBeingSelected = this.Background;
                                _foregroundBeforeBeingSelected = this.Foreground;
                                _hasEverBeenSelected = true;

                                // Set the Background and Foreground:
                                this.Background = selector.SelectedItemBackground;
                                this.Foreground = selector.SelectedItemForeground;
                            }
                            else
                            {
                                // Restore the Background and Foreground that the item had before being selected:
                                if (_hasEverBeenSelected)
                                {
                                    this.Background = _backgroundBeforeBeingSelected;
                                    this.Foreground = _foregroundBeforeBeingSelected;
                                }
                                //selectorItem.Background = selector.UnselectedItemBackground ?? selector.Background;
                                //selectorItem.Foreground = selector.UnselectedItemForeground ?? selector.Foreground;
                            }
                        }
                    }
                }
            }
            if (newValue)
            {
                VisualStateManager.GoToState(this, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", false);
            }
        }
    }
}
