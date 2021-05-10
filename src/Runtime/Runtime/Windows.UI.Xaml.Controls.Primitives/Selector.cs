

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


#if !BRIDGE
using JSIL.Meta;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;


#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that allows a user to select an item from a collection
    /// of items.
    /// </summary>
    public partial class Selector : ItemsControl
    {
        #region Data

        private bool _selectionChangeIsOnIndex = false;
        private bool _selectionChangeIsOnItem = false;
        private bool _selectionChangeIsOnValue = false;

        private bool SkipCoerceSelectedItemCheck;
        private SelectionInfo _selectionInfo;

        private DependencyPropertyChangedEventArgs _indexChangeEventArgs = null; //this one is so we can call ManageSelectedIndex_Changed from OnSelectedItemChanged (we cannot call it from OnSelectedIndexChanged because SelectedItem is not up to date at that moment if the selection change comes from SelectedValue).

        #endregion Data

        #region Constructor

        public Selector()
        {

        }

        #endregion Constructor

        #region Public Events

        /// <summary>
        /// Occurs when the selection is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion Public Events

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(Selector.SelectedItemProperty); }
            set { this.SetValue(Selector.SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(Selector),
                new PropertyMetadata(null, Selector.OnSelectedItemChanged, Selector.CoerceSelectedItem));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;
            object newValue = e.NewValue;

            // we only want to change the other ones if the change comes from 
            // SelectedItem (otherwise it's already done by the one that was 
            // originally changed (SelectedIndex or SelectedValue)
            if (!s._selectionChangeIsOnValue && !s._selectionChangeIsOnIndex)
            {
                s._selectionChangeIsOnItem = true;

                try
                {
                    if (newValue == null)
                    {
                        // we use SetCurrentValue to preserve any potential bindings.
                        s.SetCurrentValue(Selector.SelectedValueProperty, null);
                        s.SetCurrentValue(Selector.SelectedIndexProperty, -1);
                    }
                    else
                    {
                        // we use SetCurrentValue to preserve any potential bindings.
                        s.SetCurrentValue(Selector.SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(newValue, s.SelectedValuePath));
                        s.SetCurrentValue(Selector.SelectedIndexProperty, s.Items.IndexOf(newValue));
                        s.OnSelectedItemChanged(newValue);
                    }
                }
                finally
                {
                    s._selectionChangeIsOnItem = false;
                }
            }

            //calling the methods to update the Visual Tree/Selection:
            s.ApplySelectedIndex(s.SelectedIndex);
            s.ManageSelectedIndex_Changed(s._indexChangeEventArgs);

            //We do not want to raise the event here when we have a MultiSelector (or when SelectionMode is not Single ?)
            if (!(s is MultiSelector))
            {
                // Raise the selection changed event
                List<object> removedItems = new List<object>();
                removedItems.Add(e.OldValue);
                List<object> addedItems = new List<object>();
                addedItems.Add(e.NewValue);
                SelectionChangedEventArgs args = new SelectionChangedEventArgs(removedItems, addedItems);

                s.OnSelectionChanged(args);
            }
        }

        private static object CoerceSelectedItem(DependencyObject d, object baseValue)
        {
            Selector s = (Selector)d;

            if (baseValue == null || s.SkipCoerceSelectedItemCheck)
            {
                return baseValue;
            }

            int selectedIndex = s.SelectedIndex;

            if ((selectedIndex > -1 && selectedIndex < s.Items.Count && s.Items[selectedIndex] == baseValue)
                || s.Items.Contains(baseValue))
            {
                return baseValue;
            }

            return DependencyProperty.UnsetValue; // reset baseValue to old value.
        }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)this.GetValue(Selector.SelectedIndexProperty); }
            set { this.SetValue(Selector.SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(Selector),
                new PropertyMetadata(-1, Selector.OnSelectedIndexChanged, Selector.CoerceSelectedIndex));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;

            s._indexChangeEventArgs = e; //keeping the event args for the call of ManageSelectedIndex_Changed in OnSelectedItemChanged

            int newValue = (int)e.NewValue;
            // we only want to change the other ones if the change comes from 
            // SelectedIndex (otherwise it's already done by the one that was 
            // originally changed (SelectedItem or SelectedValue)
            if (!s._selectionChangeIsOnValue && !s._selectionChangeIsOnItem)
            {
                s._selectionChangeIsOnIndex = true;

                try
                {
                    if (newValue == -1)
                    {
                        // Note : we use SetCurrentValue to preserve any potential bindings.
                        s.SetCurrentValue(Selector.SelectedValueProperty, null);

                        // Skip the call to Items.IndexOf()
                        s.SkipCoerceSelectedItemCheck = true;

                        try
                        {
                            // Note: always update the value of SelectedItem last when
                            // synchronizing selection properties, so that all the properties
                            // are up to date when the selection changed event is fired.
                            s.SetCurrentValue(Selector.SelectedItemProperty, null);
                        }
                        finally
                        {
                            s.SkipCoerceSelectedItemCheck = false;
                        }
                    }
                    else
                    {
                        object item = s.Items[newValue];

                        // Note : we use SetCurrentValue to preserve any potential bindings.
                        s.SetCurrentValue(Selector.SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, s.SelectedValuePath));

                        // Skip the call to Items.IndexOf()
                        s.SkipCoerceSelectedItemCheck = true;

                        try
                        {
                            // Note: always update the value of SelectedItem last when
                            // synchronizing selection properties, so that all the properties
                            // are up to date when the selection changed event is fired.
                            s.SetCurrentValue(Selector.SelectedItemProperty, item);
                        }
                        finally
                        {
                            s.SkipCoerceSelectedItemCheck = false;
                        }
                    }
                }
                finally
                {
                    s._selectionChangeIsOnIndex = false;
                }
            }
        }

        private static object CoerceSelectedIndex(DependencyObject d, object baseValue)
        {
            Selector s = (Selector)d;

            int index = (int)baseValue;
            if (index < 0)
            {
                return -1;
            }
            else if (index >= s.Items.Count)
            {
                return s.Items.Count - 1;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// Gets or sets the value of the selected item, obtained by using the SelectedValuePath.
        /// </summary>
        public object SelectedValue
        {
            get { return this.GetValue(Selector.SelectedValueProperty); }
            set { this.SetValue(Selector.SelectedValueProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                "SelectedValue",
                typeof(object),
                typeof(Selector),
                new PropertyMetadata(null, Selector.OnSelectedValueChanged, Selector.CoerceSelectedValue));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;

            // we only want to change the other ones if the change comes from 
            // SelectedItem (otherwise it's already done by the one that was 
            // originally changed (SelectedIndex or SelectedValue)
            if (!s._selectionChangeIsOnItem && !s._selectionChangeIsOnIndex)
            {
                s._selectionChangeIsOnValue = true;

                try
                {
                    s.SetCurrentValue(Selector.SelectedIndexProperty, s._selectionInfo.Index);

                    // Note: always update the value of SelectedItem last when
                    // synchronizing selection properties, so that all the properties
                    // are up to date when the selection changed event is fired.
                    //cb.SetCurrentValue(NativeComboBox.SelectedItemProperty, item);
                    s.SetCurrentValue(Selector.SelectedItemProperty, s._selectionInfo.Item);
                }
                finally
                {
                    s._selectionChangeIsOnValue = false;
                    s._selectionInfo = null;
                }
            }
        }

        private static object CoerceSelectedValue(DependencyObject d, object baseValue)
        {
            Selector s = (Selector)d;

            if (s._selectionChangeIsOnIndex || s._selectionChangeIsOnItem)
            {
                // If we're in the middle of a selection change (SelectedIndex
                // or SelectedItem), accept the value.
                return baseValue;
            }
            else
            {
                // Otherwise, this is a user-initiated change to SelectedValue.
                // Find the corresponding item.
                int index;
                object item = s.FindItemWithValue(baseValue, out index);

                // if the search fails, coerce the value to null:
                if (item == DependencyProperty.UnsetValue)
                {
                    baseValue = null;
                }

                // Store the new selected item so we don't have to look for it again
                // in OnSelectedValueChanged.
                s._selectionInfo = new SelectionInfo(item == DependencyProperty.UnsetValue ? null : item, index);
            }

            return baseValue;
        }

        /// <summary>
        /// Gets or sets the property path that is used to get the SelectedValue property
        /// of the SelectedItem property.
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)this.GetValue(Selector.SelectedValuePathProperty); }
            set { this.SetValue(Selector.SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                "SelectedValuePath",
                typeof(string),
                typeof(Selector),
                new PropertyMetadata(string.Empty, Selector.OnSelectedValuePathChanged));

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;

            s._selectionChangeIsOnItem = true;
            s._selectionChangeIsOnIndex = true;

            try
            {
                object item = null;
                if (s.SelectedItem != null)
                {
                    item = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(s.SelectedItem, (string)e.NewValue);
                }
                s.SetCurrentValue(Selector.SelectedValueProperty, item);
            }
            finally
            {
                s._selectionChangeIsOnItem = false;
                s._selectionChangeIsOnIndex = false;
            }
        }

        #region things to replace with selectors Controltemplates

        /// <summary>
        /// Gets or sets the bakground color of the selected Items.
        /// </summary>
        public Brush SelectedItemBackground
        {
            get { return (Brush)this.GetValue(Selector.SelectedItemBackgroundProperty); }
            set { this.SetValue(Selector.SelectedItemBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItemBackground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register(
                "SelectedItemBackground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString("#FFBADDE9"))));

        /// <summary>
        /// Gets or sets the foreground color of the selected Items.
        /// </summary>
        public Brush SelectedItemForeground
        {
            get { return (Brush)this.GetValue(Selector.SelectedItemForegroundProperty); }
            set { this.SetValue(Selector.SelectedItemForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItemForeground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemForegroundProperty =
            DependencyProperty.Register(
                "SelectedItemForeground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        public Brush RowBackground
        {
            get { return (Brush)this.GetValue(Selector.RowBackgroundProperty); }
            set { this.SetValue(Selector.RowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the RowBackground dependency property
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register(
                "RowBackground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));


        /// <summary>
        /// Gets or sets the foreground color of the Items that are not selected.
        /// </summary>
        public Brush UnselectedItemForeground
        {
            get { return (Brush)this.GetValue(Selector.UnselectedItemForegroundProperty); }
            set { this.SetValue(Selector.UnselectedItemForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the UnselectedItemForeground dependency property
        /// </summary>
        public static readonly DependencyProperty UnselectedItemForegroundProperty =
            DependencyProperty.Register(
                "UnselectedItemForeground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata((Brush)null));

        #endregion things to replace with selectors Controltemplates

        #endregion Dependency Properties        

        #region Public/Protected Methods

        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            int selectedIndex = this.SelectedIndex;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (!this.Items.Contains(this.SelectedItem))
                    {
                        selectedIndex = -1; // unselect.
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex <= this.SelectedIndex)
                    {
                        ++selectedIndex;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems[0] == this.SelectedItem)
                    {
                        selectedIndex = -1; // unselect
                    }
                    else if (e.OldStartingIndex <= this.SelectedIndex)
                    {
                        --selectedIndex;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems[0] == this.SelectedItem)
                    {
                        selectedIndex = -1;
                    }
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }

            if (selectedIndex != this.SelectedIndex)
            {
                // Note : we use SetCurrentValue to preserve bindings if any.
                this.SetCurrentValue(Selector.SelectedIndexProperty, selectedIndex);
            }
        }

        protected override void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            //Update the selection so th eselected item can be displayed as selected:
            //Note: We call CoerceValue on all 3 properties because we don't know which property was set. Based on a test in wpf, SelectedIndex takes precedence over SelectedItem, which takes precedence over SelectedValue.
            //      Precedence is applied below since when calling coerce: - if the value was set, it will go to the callback which changes the values of the other two
            //                                                             - if the value was not set, it will not call the callback so no impact on the other ones.
            this.CoerceValue(SelectedIndexProperty);
            this.CoerceValue(SelectedItemProperty);
            this.CoerceValue(SelectedValueProperty);
            //this.SetCurrentValue(Selector.SelectedIndexProperty, -1); // Note: I don't know why this was there but it seemed incorrect so I commented it.
        }

        protected virtual void ApplySelectedIndex(int index)
        {

        }

        protected virtual void ManageSelectedIndex_Changed(DependencyPropertyChangedEventArgs e)
        {

        }

        protected virtual void OnSelectedItemChanged(object selectedItem)
        {

        }

        public virtual void NotifyItemMouseEnter(SelectorItem item)
        {

        }
        #endregion Public/Protected Methods

        #region Private Methods

        private object FindItemWithValue(object value, out int index)
        {
            index = -1;

            if (this.Items.Count == 0)
            {
                return DependencyProperty.UnsetValue;
            }

            string selectedValuePath = this.SelectedValuePath;

            // optimize for case where there is no SelectedValuePath
            if (string.IsNullOrEmpty(selectedValuePath))
            {
                index = this.Items.IndexOf(value);
                if (index >= 0)
                {
                    return value;
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            index = 0;
            foreach (object item in this.Items)
            {
                object displayedItem = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, selectedValuePath);
                if (ItemsControl.EqualsEx(displayedItem, value))
                {
                    return item;
                }
                ++index;
            }

            index = -1;
            return DependencyProperty.UnsetValue;
        }

        #endregion Private Methods

        #region Private classes

        private class SelectionInfo
        {
            public int Index;
            public object Item;

            public SelectionInfo(object item, int index)
            {
                this.Item = item;
                this.Index = index;
            }
        }

        #endregion Private classes

        #region Obsolete

        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        [Obsolete("Use RowBackground instead.")]
        public Brush UnselectedItemBackground
        {
            get { return this.RowBackground; }
            set { this.RowBackground = value; }
        }

        [Obsolete("Unused.")]
        protected bool ChangingSelectionProgrammatically { get; set; }

        #endregion Obsolete
    }
}