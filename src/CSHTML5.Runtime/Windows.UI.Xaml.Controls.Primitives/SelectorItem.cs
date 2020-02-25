﻿
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
    DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectorItem), new PropertyMetadata(false, IsSelected_Changed));

        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectorItem selectorItem = (SelectorItem)d;
            
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(selectorItem))
            {
                //todo: remove what's inside the "if" above once we will have defined a default style for the ItemsControl, that does about the same thing.
                if (selectorItem.INTERNAL_ParentSelectorControl != null)
                {
                    Selector selector = selectorItem.INTERNAL_ParentSelectorControl;

                    if (selector.ItemContainerStyle == null)
                    {
                        bool newValue = (bool)e.NewValue;
                        if (newValue != (bool)e.OldValue)
                        {
                            if (newValue)
                            {
                                // Remember the values of the Background and Foreground before selection, in order to be able to revert them back when needed:
                                selectorItem._backgroundBeforeBeingSelected = selectorItem.Background;
                                selectorItem._foregroundBeforeBeingSelected = selectorItem.Foreground;
                                selectorItem._hasEverBeenSelected = true;

                                // Set the Background and Foreground:
                                selectorItem.Background = selector.SelectedItemBackground;
                                selectorItem.Foreground = selector.SelectedItemForeground;
                            }
                            else
                            {
                                // Restore the Background and Foreground that the item had before being selected:
                                if (selectorItem._hasEverBeenSelected)
                                {
                                    selectorItem.Background = selectorItem._backgroundBeforeBeingSelected;
                                    selectorItem.Foreground = selectorItem._foregroundBeforeBeingSelected;
                                }
                                //selectorItem.Background = selector.UnselectedItemBackground ?? selector.Background;
                                //selectorItem.Foreground = selector.UnselectedItemForeground ?? selector.Foreground;
                            }
                        }
                    }
                }
            }
            if ((bool)e.NewValue)
            {
                VisualStateManager.GoToState(selectorItem, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(selectorItem, "Unselected", false);
            }
        }
    }
}
