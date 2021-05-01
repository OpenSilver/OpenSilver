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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CSHTML5.Native.Html.Controls
{
    public class NativeComboBox : FrameworkElement
    {
        #region Data

        private dynamic _nativeComboBoxDomElement;
        private HtmlEventProxy _changeEventProxy;
        private bool _changingSelectionProgrammatically;
        private bool _changingSelectionInHtml;

        private bool _selectionChangeIsOnIndex = false;
        private bool _selectionChangeIsOnItem = false;
        private bool _selectionChangeIsOnValue = false;

        private bool SkipCoerceSelectedItemCheck;
        private SelectionInfo _selectionInfo;

        private ItemCollection _items;

        #endregion Data

        #region Constructor

        /// <summary>
        /// Instantiate a new Native ComboBox.
        /// </summary>
        public NativeComboBox()
        {
            this.INTERNAL_DoNotApplyStyle = true;
        }

        #endregion Constructor

        #region Public Events

        /// <summary>
        /// Occurs when the selection is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Items is the collection of data that is used to generate the content
        /// of this control.
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new ItemCollection(null);
                    this._items.CollectionChanged += this.OnItemCollectionChanged;
                }
                return this._items;
            }
        }

        #endregion Public Properties

        #region Dependency Properties

        /// <summary>
        /// Gets or sets an object source used to generate the content of the NativeComboBox.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(NativeComboBox.ItemsSourceProperty); }
            set { this.SetValue(NativeComboBox.ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(NativeComboBox),
                new PropertyMetadata(null, NativeComboBox.OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;
            IEnumerable newValue = (IEnumerable)e.NewValue;

            if (e.NewValue != null)
            {
                // ItemsSource is non-null.  Go to ItemsSource mode
                cb.Items.SetItemsSource(newValue);
            }
            else
            {
                // ItemsSource is explicitly null.  Return to normal mode.
                cb.Items.ClearItemsSource();
            }

            cb.SetCurrentValue(NativeComboBox.SelectedIndexProperty, -1);
        }

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the visual
        /// representation of the object.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)this.GetValue(NativeComboBox.DisplayMemberPathProperty); }
            set { this.SetValue(NativeComboBox.DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(NativeComboBox),
                new PropertyMetadata(string.Empty, NativeComboBox.OnDisplayMemberPathChanged));

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;

            // Refresh the combobox
            cb.Refresh();
        }

        /// <summary>
        /// The index of the first item in the current selection or -1 if the selection is empty.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(NativeComboBox.SelectedIndexProperty); }
            set { SetValue(NativeComboBox.SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(NativeComboBox),
                new PropertyMetadata(-1, NativeComboBox.OnSelectedIndexChanged, NativeComboBox.CoerceSelectedIndex));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;

            int newValue = (int)e.NewValue;

            if (!cb._changingSelectionInHtml)
            {
                cb._changingSelectionProgrammatically = true;

                try
                {
                    cb.SetSelectedIndexInNativeHtmlDom(newValue);
                }
                finally
                {
                    cb._changingSelectionProgrammatically = false;
                }
            }

            // we only want to change the other ones if the change comes from 
            // SelectedIndex (otherwise it's already done by the one that was 
            // originally changed (SelectedItem or SelectedValue)
            if (!cb._selectionChangeIsOnValue && !cb._selectionChangeIsOnItem)
            {
                cb._selectionChangeIsOnIndex = true;

                try
                {
                    if (newValue == -1)
                    {
                        // Note : we use SetCurrentValue to preserve any potential bindings.
                        cb.SetCurrentValue(NativeComboBox.SelectedValueProperty, null);

                        // Skip the call to Items.IndexOf()
                        cb.SkipCoerceSelectedItemCheck = true;

                        try
                        {
                            // Note: always update the value of SelectedItem last when
                            // synchronizing selection properties, so that all the properties
                            // are up to date when the selection changed event is fired.
                            cb.SetCurrentValue(NativeComboBox.SelectedItemProperty, null);
                        }
                        finally
                        {
                            cb.SkipCoerceSelectedItemCheck = false;
                        }
                    }
                    else
                    {
                        object item = cb.Items[newValue];

                        // Note : we use SetCurrentValue to preserve any potential bindings.
                        cb.SetCurrentValue(NativeComboBox.SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, cb.SelectedValuePath));
                        
                        // Skip the call to Items.IndexOf()
                        cb.SkipCoerceSelectedItemCheck = true;

                        try
                        {
                            // Note: always update the value of SelectedItem last when
                            // synchronizing selection properties, so that all the properties
                            // are up to date when the selection changed event is fired.
                            cb.SetCurrentValue(NativeComboBox.SelectedItemProperty, item);
                        }
                        finally
                        {
                            cb.SkipCoerceSelectedItemCheck = false;
                        }
                    }
                }
                finally
                {
                    cb._selectionChangeIsOnIndex = false;
                }
            }
        }

        private static object CoerceSelectedIndex(DependencyObject d, object baseValue)
        {
            NativeComboBox cb = (NativeComboBox)d;

            int index = (int)baseValue;
            if (index < 0)
            {
                return -1;
            }
            else if (index >= cb.Items.Count)
            {
                return cb.Items.Count - 1;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(NativeComboBox.SelectedItemProperty); }
            set { this.SetValue(NativeComboBox.SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(NativeComboBox),
                new PropertyMetadata(null, NativeComboBox.OnSelectedItemChanged, NativeComboBox.CoerceSelectedItem));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;
            object newValue = e.NewValue;

            // we only want to change the other ones if the change comes from 
            // SelectedItem (otherwise it's already done by the one that was 
            // originally changed (SelectedIndex or SelectedValue)
            if (!cb._selectionChangeIsOnValue && !cb._selectionChangeIsOnIndex)
            {
                cb._selectionChangeIsOnItem = true;

                try
                {
                    if (newValue == null)
                    {
                        // we use SetCurrentValue to preserve any potential bindings.
                        cb.SetCurrentValue(NativeComboBox.SelectedValueProperty, null);
                        cb.SetCurrentValue(NativeComboBox.SelectedIndexProperty, -1);
                    }
                    else
                    {
                        // we use SetCurrentValue to preserve any potential bindings.
                        cb.SetCurrentValue(NativeComboBox.SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(newValue, cb.SelectedValuePath));
                        cb.SetCurrentValue(NativeComboBox.SelectedIndexProperty, cb.Items.IndexOf(newValue));
                    }
                }
                finally
                {
                    cb._selectionChangeIsOnItem = false;
                }
            }

            // Raise the selection changed event
            cb.OnSelectionChanged(e.OldValue, e.NewValue);
        }

        private static object CoerceSelectedItem(DependencyObject d, object baseValue)
        {
            NativeComboBox cb = (NativeComboBox)d;

            if (baseValue == null || cb.SkipCoerceSelectedItemCheck)
            {
                return baseValue;
            }

            int selectedIndex = cb.SelectedIndex;

            if ((selectedIndex > -1 && selectedIndex < cb.Items.Count && cb.Items[selectedIndex] == baseValue)
                || cb.Items.Contains(baseValue))
            {
                return baseValue;
            }

            return DependencyProperty.UnsetValue; // reset baseValue to old value.
        }

        /// <summary>
        /// Gets or sets the value of the selected item, obtained by using the SelectedValuePath.
        /// </summary>
        public object SelectedValue
        {
            get { return this.GetValue(NativeComboBox.SelectedValueProperty); }
            set { this.SetValue(NativeComboBox.SelectedValueProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                "SelectedValue",
                typeof(object),
                typeof(NativeComboBox),
                new PropertyMetadata(null, NativeComboBox.OnSelectedValueChanged, NativeComboBox.CoerceSelectedValue));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;

            // we only want to change the other ones if the change comes from 
            // SelectedItem (otherwise it's already done by the one that was 
            // originally changed (SelectedIndex or SelectedValue)
            if (!cb._selectionChangeIsOnItem && !cb._selectionChangeIsOnIndex)
            {
                cb._selectionChangeIsOnValue = true;

                try
                {
                    cb.SetCurrentValue(NativeComboBox.SelectedIndexProperty, cb._selectionInfo.Index);

                    // Note: always update the value of SelectedItem last when
                    // synchronizing selection properties, so that all the properties
                    // are up to date when the selection changed event is fired.
                    cb.SetCurrentValue(NativeComboBox.SelectedItemProperty, cb._selectionInfo.Item);
                }
                finally
                {
                    cb._selectionChangeIsOnValue = false;
                    cb._selectionInfo = null;
                }
            }
        }

        private static object CoerceSelectedValue(DependencyObject d, object baseValue)
        {
            NativeComboBox cb = (NativeComboBox)d;

            if (cb._selectionChangeIsOnIndex || cb._selectionChangeIsOnItem)
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
                object item = cb.FindItemWithValue(baseValue, out index);

                // if the search fails, coerce the value to null.  Unless there
                // are no items at all, in which case wait for the items to appear
                // and search again.
                if (item == DependencyProperty.UnsetValue && cb.Items.Count > 0)
                {
                    baseValue = null;
                }

                // Store the new selected item so we don't have to look for it again
                // in OnSelectedValueChanged.
                cb._selectionInfo = new SelectionInfo(item == DependencyProperty.UnsetValue ? null : item, index);
            }

            return baseValue;
        }

        /// <summary>
        /// Gets or sets the property path that is used to get the SelectedValue property
        /// of the SelectedItem property.
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)this.GetValue(NativeComboBox.SelectedValuePathProperty); }
            set { this.SetValue(NativeComboBox.SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                "SelectedValuePath",
                typeof(string),
                typeof(NativeComboBox),
                new PropertyMetadata(string.Empty, NativeComboBox.OnSelectedValuePathChanged));

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeComboBox cb = (NativeComboBox)d;

            cb._selectionChangeIsOnItem = true;
            cb._selectionChangeIsOnIndex = true;

            try
            {
                object item = null;
                if (cb.SelectedItem != null)
                {
                    item = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(cb.SelectedItem, (string)e.NewValue);
                }
                cb.SetCurrentValue(NativeComboBox.SelectedValueProperty, item);
            }
            finally
            {
                cb._selectionChangeIsOnItem = false;
                cb._selectionChangeIsOnIndex = false;
            }
        }

        #endregion Dependency Properties

        #region Public Methods

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var select = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("select", parentRef, this);
            domElementWhereToPlaceChildren = select;
            this._nativeComboBoxDomElement = select;

            INTERNAL_HtmlDomManager.SetDomElementStyleProperty(select, new List<string>() { "fontSize" }, "inherit");

            // Set the mark saying that the pointer events must be "absorbed" by the ComboBox:
            INTERNAL_HtmlDomManager.SetDomElementProperty(select, "data-absorb-events", true);

            // Fill the ComboBox and synchronize selection properties
            this.Refresh();

            // Listen to native selection change event
            this.SubscribeToHtmlChangeEvent();

            return select;
        }

        private void SubscribeToHtmlChangeEvent()
        {
            this._changeEventProxy = INTERNAL_EventsHelper.AttachToDomEvents(
                "change",
                this._nativeComboBoxDomElement,
                (Action<object>)this.DomSelectionChanged);
        }

        private void UnsubscribeFromHtmlChangeEvent()
        {
            if (this._changeEventProxy == null)
            {
                return;
            }

            INTERNAL_EventsHelper.DetachEvent(
                "change",
                this._nativeComboBoxDomElement,
                this._changeEventProxy,
                (Action<object>)this.DomSelectionChanged);
        }

        #endregion Public Methods

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

        private void Refresh()
        {
            this.ResetOptions();

            this.OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update displayed options
            this.ManageCollectionChanged(e);

            // Update selection properties
            this.OnItemsChanged(e);
        }

        private void ManageCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Note : the '+1' is here to compensate the empty option.

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.ResetOptions();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.AddOption(e.NewItems[0], e.NewStartingIndex + 1);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.RemoveOption(e.OldItems[0], e.OldStartingIndex + 1);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                this.RemoveOption(e.OldItems[0], e.OldStartingIndex + 1);
                this.AddOption(e.NewItems[0], e.OldStartingIndex + 1);
            }
            else
            {
                throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }
        }

        private void ResetOptions()
        {
            if (this._nativeComboBoxDomElement == null)
            {
                return;
            }

            for (int i = 0; i < this.Items.Count + 1; ++i) // +1 to remove the 'empty option'
            {
                // empty the combobox
                INTERNAL_HtmlDomManager.RemoveOptionFromNativeComboBox(this._nativeComboBoxDomElement, 0);
            }

            this.AddEmptyOption(); // 'empty option'

            for (int i = 0; i < this.Items.Count; ++i)
            {
                this.AddOption(this.Items[i], i + 1);
            }

            // Set selectedindex
            this.SetSelectedIndexInNativeHtmlDom(this.SelectedIndex);
        }

        private void AddOption(object option, int index)
        {
            if (this._nativeComboBoxDomElement == null)
            {
                return;
            }

            object value = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(option, this.DisplayMemberPath);
            if (value != null)
            {
                INTERNAL_HtmlDomManager.AddOptionToNativeComboBox(
                    this._nativeComboBoxDomElement,
                    value.ToString(),
                    index);
            }
        }

        private void RemoveOption(object option, int index)
        {
            if (this._nativeComboBoxDomElement == null)
            {
                return;
            }

            INTERNAL_HtmlDomManager.RemoveOptionFromNativeComboBox(this._nativeComboBoxDomElement, index);
        }

        // Add an empty option at the beginning of the combobox.
        private void AddEmptyOption()
        {
            // Add an empty element that will make it easier to have 
            // nothing selected when items are added to the ComboBox: 
            // See: http://stackoverflow.com/questions/8605516/default-select-option-as-blank
            var emptyOption = INTERNAL_HtmlDomManager.AddOptionToNativeComboBox(this._nativeComboBoxDomElement, "", 0);
            CSHTML5.Interop.ExecuteJavaScriptAsync("$0.disabled = true", emptyOption);
            CSHTML5.Interop.ExecuteJavaScriptAsync("$0.selected = true", emptyOption);
            CSHTML5.Interop.ExecuteJavaScriptAsync("$0.style.display = 'hidden'", emptyOption);
        }

        private void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
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
                this.SetCurrentValue(NativeComboBox.SelectedIndexProperty, selectedIndex);
            }
        }

        private void OnSelectionChanged(object oldItem, object newItem)
        {
            if (this.SelectionChanged != null)
            {
                List<object> oldItems = new List<object>();
                oldItems.Add(oldItem);
                List<object> newItems = new List<object>();
                newItems.Add(newItem);

                SelectionChangedEventArgs args = new SelectionChangedEventArgs(oldItems, newItems);
                args.OriginalSource = this;

                this.SelectionChanged(this, args);
            }
        }

        private void DomSelectionChanged(dynamic element)
        {
            if (this._changingSelectionProgrammatically)
            {
                return;
            }

            int selectedIndex = this.GetSelectedIndexInNativeHtmlDom();

            // Update selection properties
            this._changingSelectionInHtml = true;

            try
            {
                // we use SetCurrentValue to preserve any potential bindings.
                this.SetCurrentValue(NativeComboBox.SelectedIndexProperty, selectedIndex);
            }
            finally
            {
                this._changingSelectionInHtml = false;
            }
        }

        private void SetSelectedIndexInNativeHtmlDom(int value)
        {
            if (this._nativeComboBoxDomElement != null)
            {
                // Compensate for the fact that the ComboBox contains an empty
                // element at the beginning (see note in 'AddEmptyOption' method)
                INTERNAL_HtmlDomManager.SetDomElementProperty(
                    _nativeComboBoxDomElement,
                    "selectedIndex",
                    value >= 0 ? value + 1 : value,
                    true);
            }
        }

        private int GetSelectedIndexInNativeHtmlDom()
        {
            if (this._nativeComboBoxDomElement != null)
            {
                int selectedIndexInHtmlDom = Convert.ToInt32(INTERNAL_HtmlDomManager.GetDomElementAttribute(this._nativeComboBoxDomElement, "selectedIndex"));

                // Compensate for the fact that the ComboBox contains an empty 
                // element at the beginning (see note in 'AddEmptyOption' method)
                if (selectedIndexInHtmlDom >= 0)
                {
                    selectedIndexInHtmlDom -= 1;
                }

                return selectedIndexInHtmlDom;
            }

            return -1;
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
    }
}
