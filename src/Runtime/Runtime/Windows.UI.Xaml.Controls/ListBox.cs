

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.System;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Contains a list of selectable items.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <StackPanel Orientation="Horizontal" Margin="0,10,0,0">
    ///     <ListBox x:Name="ListBox1" DisplayMemberPath="Name" SelectedValuePath="ImagePath" VerticalAlignment="Top" SelectionMode="Single" BorderThickness="1" BorderBrush="LightGray"/>
    ///     <Image Source="{Binding ElementName=ListBox1, Path=SelectedValue}" Width="60" Height="60" Margin="20,0,0,0" VerticalAlignment="Top"/>
    /// </StackPanel>
    /// </code>
    /// <code lang="C#">
    /// ListBox1.ItemsSource = Planet.GetListOfPlanets();
    /// public partial class Planet
    /// {
    ///    public string Name { get; set; }
    ///    public string ImagePath { get; set; }
    ///
    ///
    ///    public static ObservableCollection&lt;Planet&gt; GetListOfPlanets()
    ///    {
    ///        return new ObservableCollection&lt;Planet&gt;()
    ///        {
    ///            new Planet() { Name = "Mercury", ImagePath = "ms-appx:/Planets/Mercury.png" },
    ///            new Planet() { Name = "Venus", ImagePath = "ms-appx:/Planets/Venus.png" },
    ///            new Planet() { Name = "Earth", ImagePath = "ms-appx:/Planets/Earth.png" },
    ///            new Planet() { Name = "Mars", ImagePath = "ms-appx:/Planets/Mars.png" },
    ///            new Planet() { Name = "Jupiter", ImagePath = "ms-appx:/Planets/Jupiter.png" },
    ///            new Planet() { Name = "Saturn", ImagePath = "ms-appx:/Planets/Saturn.png" },
    ///            new Planet() { Name = "Uranus", ImagePath = "ms-appx:/Planets/Uranus.png" },
    ///            new Planet() { Name = "Neptune", ImagePath = "ms-appx:/Planets/Neptune.png" }
    ///            new Planet() { Name = "Pluto", ImagePath = "ms-appx:/Planets/Pluto.png" }
    ///        };
    ///    }
    ///} 
    /// </code>
    /// </example>
    public partial class ListBox : MultiSelector //normally it should be Selector but since MultiSelector already adds SelectedItems, we inherit from MultiSelector.
    {
        //local variable used in SelectionMode.Extended 
        int _indexOfLastClickedItemWithoutShiftKey = 0;
        /// <summary>
        /// Initializes a new instance of the ListBox class.
        /// </summary>
        public ListBox()
        {
            this.DefaultStyleKey = typeof(ListBox);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ListBoxItem container = element as ListBoxItem;
            if (container != null)
            {
                container.INTERNAL_CorrespondingItem = item;
                container.INTERNAL_ParentSelectorControl = this;
                if (SelectedItems.Contains(item))
                {
                    // todo if possible: reuse the former items if they were already 
                    // created rather than recreating them. It would actually be 
                    // necessary to avoir errors like item is a string and there are 
                    // two items with the same string, but only one of them is 
                    // selected. Both will be considered (un)selected.

                    // this can be needed when the SelectedItems collection was 
                    // modified before the ListBox.Loaded event happened (which 
                    // leads to an ItemsControl.UpdateItemsPanel, which then leads 
                    // here).
                    container.IsSelected = true;
                }
                container.Click += listBoxItem_Click;
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            ListBoxItem container = element as ListBoxItem;
            if (container != null)
            {
                container.INTERNAL_CorrespondingItem = null;
                container.INTERNAL_ParentSelectorControl = null;
                container.Click -= listBoxItem_Click;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ListBoxItem);
        }

        protected override SelectorItem INTERNAL_GenerateContainer(object item)
        {
            return (SelectorItem)this.GetContainerFromItem(item);
        }

        protected override DependencyObject GetContainerFromItem(object item)
        {
            ListBoxItem listBoxItem = item as ListBoxItem ?? new ListBoxItem();
            listBoxItem.INTERNAL_CorrespondingItem = item;
            listBoxItem.INTERNAL_ParentSelectorControl = this;
            if (SelectedItems.Contains(item))
            {
                // todo if possible: reuse the former items if they were already 
                // created rather than recreating them. It would actually be 
                // necessary to avoir errors like item is a string and there are 
                // two items with the same string, but only one of them is 
                // selected. Both will be considered (un)selected.

                // this can be needed when the SelectedItems collection was 
                // modified before the ListBox.Loaded event happened (which 
                // leads to an ItemsControl.UpdateItemsPanel, which then leads 
                // here).
                listBoxItem.IsSelected = true;
            }
            listBoxItem.Click += listBoxItem_Click;
            return listBoxItem;
        }


        #region selection related elements

        /// <summary>
        /// Gets or sets the selection behavior for the ListBox control.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ListBox), new PropertyMetadata(SelectionMode.Single));
        //we didn't implement a SelectionMode_Changed method since I don't think it would be of any use.


        protected override void ManageSelectedIndex_Changed(DependencyPropertyChangedEventArgs e)
        {
            if (!_skipSelectionChangedEvent) //this test because if it is true (meaning the SelectionChanged event is dealt with somewhere else), it means that SelectedItems has already been updated.
            {
                bool skipSelectionEventHere = _skipSelectionChangedEvent;
                _skipSelectionChangedEvent = true; // Note: we keep this to true for the whole method since this method will handle the SelectionChanged event itself.

                base.ManageSelectedIndex_Changed(e);

                bool hasSelectionChanged = false;

                List<object> addedItems = new List<object>();
                List<object> removedItems = new List<object>();
                HashSet<object> selectedItemsHashSet = null;

                // Note: in this method, we use "Convert.ToInt32()" intead of casting to "(int)" because otherwise the JS code is not compatible with IE 9 (or Windows Phone 8.0).
                bool isNewValueSet = e.NewValue != null && e.NewValue is int && Convert.ToInt32(e.NewValue) >= 0;
                bool isOldValueSet = e.OldValue != null && e.OldValue is int && Convert.ToInt32(e.OldValue) >= 0;
                bool isNewValueDifferentFromOldValue = e.NewValue != e.OldValue;
                if (isOldValueSet && isNewValueDifferentFromOldValue)
                {
                    //We clear the selection and remember the old items if needed for the SelectionChanged event:
                    if (!skipSelectionEventHere)
                    {
                        selectedItemsHashSet = CopyToHashSet(SelectedItems);
                        hasSelectionChanged = true;
                    }
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                }
                if (isNewValueSet)
                {
                    int newValue = Convert.ToInt32(e.NewValue);
                    object item = Items[newValue];
                    if (SelectedItems.Count == 0 || SelectedItems[0] != item) //If SelectedItem stays the same (the first item in SelectedItems), we do nothing
                    {
                        if (SelectedItems.Count != 0) //this test is to avoid calling clear (and entering MultiSelector.RefreshSelectedItems) for no reason.
                        {
                            if (!skipSelectionEventHere)
                            {
                                selectedItemsHashSet = CopyToHashSet(SelectedItems);
                            }
                            INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                        }
                        SelectedItems.Add(item);
                        if (!skipSelectionEventHere)
                        {
                            if (selectedItemsHashSet != null && selectedItemsHashSet.Contains(item))
                            {
                                selectedItemsHashSet.Remove(item);
                            }
                            addedItems.Add(item);
                            hasSelectionChanged = true;
                        }
                    }
                }
                if (hasSelectionChanged && !skipSelectionEventHere)
                {
                    if (selectedItemsHashSet != null)
                    {
                        foreach (object item in selectedItemsHashSet)
                        {
                            removedItems.Add(item);
                        }
                    }
                    OnSelectionChanged(new SelectionChangedEventArgs(removedItems, addedItems));
                }
                _skipSelectionChangedEvent = skipSelectionEventHere;
            }
        }

        void listBoxItem_Click(object sender, RoutedEventArgs e)
        {
            _skipSelectionChangedEvent = true; // Note: we keep this to true for the whole method since this method will handle the SelectionChanged event itself (this way we don't risk having multiple calls to SelectionChanged when adding multiple elements for example).

            //object[] oldItems = null;
            bool hasSelectionChanged = false;

            List<object> addedItems = new List<object>();
            List<object> removedItems = new List<object>();

#if MIGRATION
            ModifierKeys modifiersKey = new ModifierKeys();
            modifiersKey = Keyboard.Modifiers;
            bool isControl = modifiersKey.HasFlag(ModifierKeys.Control);
            bool isShift = modifiersKey.HasFlag(ModifierKeys.Shift);
#else
            VirtualKeyModifiers modifiersKey = new VirtualKeyModifiers();
            modifiersKey = Keyboard.Modifiers;
            bool isControl = modifiersKey.HasFlag(VirtualKeyModifiers.Control);
            bool isShift = modifiersKey.HasFlag(VirtualKeyModifiers.Shift);

#endif
            SelectorItem selectorItem = (SelectorItem)sender;
            //---------------------------------------------------
            //Single
            //---------------------------------------------------
            if (SelectionMode == SelectionMode.Single)
            {
                if (SelectedItem != selectorItem.INTERNAL_CorrespondingItem) //todo: SelectedItems contains more than one element (I'm pretty sure we ignore is as long as the clicked element is the SelectedItem).
                {
                    HashSet<object> selectedItemsHashSet = CopyToHashSet(SelectedItems);
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                    SelectedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                    addedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                    if (selectedItemsHashSet.Contains(selectorItem.INTERNAL_CorrespondingItem))
                    {
                        selectedItemsHashSet.Remove(selectorItem.INTERNAL_CorrespondingItem);
                    }
                    foreach (object item in selectedItemsHashSet)
                    {
                        removedItems.Add(item);
                    }
                    hasSelectionChanged = true;
                }
            }
            //---------------------------------------------------
            //Multiple
            //---------------------------------------------------
            else if (SelectionMode == SelectionMode.Multiple)
            {
                //If click on an already selected element
                if (INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, selectorItem.INTERNAL_CorrespondingItem))//if we are in a multiple mode and the currently selected element is the one we clicked, we want to unselect it.
                {
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Remove(SelectedItems, selectorItem.INTERNAL_CorrespondingItem);
                    removedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                }
                //else  click on a new element
                else
                {
                    SelectedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                    addedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                }
                hasSelectionChanged = true;
            }
            //---------------------------------------------------
            //Extended
            //---------------------------------------------------
            else if (SelectionMode == SelectionMode.Extended)
            {

                int indexOfClickedItem = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(Items).IndexOf(selectorItem.INTERNAL_CorrespondingItem); //todo-perfs: this is O(N).

                //if Shift is pressed
                if (isShift)
                {


                    int indexStart = _indexOfLastClickedItemWithoutShiftKey;
                    int indexEnd = indexOfClickedItem;
                    int change;
                    if (indexStart > indexEnd)
                    {
                        change = indexEnd;
                        indexEnd = indexStart;
                        indexStart = change;
                    }
                    int index = 0;
                    //copy the SelectedItems in a hashset so we can easily know which items have been removed:
                    HashSet<object> selectedItemsHashSet = CopyToHashSet(SelectedItems);
                    //clear before adding
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                    foreach (object item in Items)
                    {
                        if (indexStart <= index && index <= indexEnd)
                        {
                            if (!INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, item)) //todo-perfs: use a dictionary
                            {
                                SelectedItems.Add(item);
                                addedItems.Add(item);
                                if (selectedItemsHashSet.Contains(item))
                                {
                                    selectedItemsHashSet.Remove(item);
                                }
                            }
                        }
                        index++;
                    }
                    foreach (object item in selectedItemsHashSet)
                    {
                        removedItems.Add(item);
                    }

                    hasSelectionChanged = removedItems.Count > 0 || addedItems.Count > 0;
                }
                //if Control is pressed
                else if (isControl)
                {
                    //If click on an already selected element
                    if (INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, selectorItem.INTERNAL_CorrespondingItem))//if we are in a multiple mode and the currently selected element is the one we clicked, we want to unselect it.
                    {
                        INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Remove(SelectedItems, selectorItem.INTERNAL_CorrespondingItem);
                        removedItems.Add(selectorItem.INTERNAL_CorrespondingItem);

                    }
                    //else click on a new element
                    else
                    {
                        SelectedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                        addedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                    }
                    hasSelectionChanged = true;
                }
                //Nothing is pressed
                else
                {
                    if (SelectedItem != selectorItem.INTERNAL_CorrespondingItem)
                    {
                        HashSet<object> selectedItemsHashSet = CopyToHashSet(SelectedItems);
                        INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                        SelectedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                        addedItems.Add(selectorItem.INTERNAL_CorrespondingItem);
                        if (selectedItemsHashSet.Contains(selectorItem.INTERNAL_CorrespondingItem))
                        {
                            selectedItemsHashSet.Remove(selectorItem.INTERNAL_CorrespondingItem);
                        }
                        foreach (object item in selectedItemsHashSet)
                        {
                            removedItems.Add(item);
                        }
                        hasSelectionChanged = true;
                    }
                }

                if (!isShift)
                    _indexOfLastClickedItemWithoutShiftKey = indexOfClickedItem;
            }
            else
            {
                throw new NotSupportedException("SelectionMode is not Single, Multiple or Extended.");
            }

            OnSelectionChanged(new SelectionChangedEventArgs(removedItems, addedItems));
            _skipSelectionChangedEvent = false;
        }

        HashSet<object> CopyToHashSet(IList iListToCopy)
        {
            HashSet<object> hashSet = new HashSet<object>();
            if (iListToCopy != null)
            {
                foreach (object item in iListToCopy)
                {
                    hashSet.Add(item);
                }
            }
            return hashSet;
        }


        protected override void UnselectAllItems()
        {
            foreach (SelectorItem selectorItem in ItemContainerGenerator.INTERNAL_AllContainers)
            {
                selectorItem.IsSelected = false;
            }
        }



        protected override void SetItemVisualSelectionState(object item, bool newState)
        {
            ListBoxItem listBoxItem = (ListBoxItem)ItemContainerGenerator.ContainerFromItem(item);
            if (listBoxItem != null)
            {
                listBoxItem.IsSelected = newState;
                //if (newState)
                //{
                //    SelectedItem = item;
                //}
                //else
                //{
                //    //trigger the OnSelectionChanged event:
                //    List<object> oldItems = new List<object>();
                //    oldItems.Add(item);
                //    List<object> newItems = new List<object>();
                //    //newItems.Add(null);
                //    OnSelectionChanged(new SelectionChangedEventArgs(oldItems, newItems));
                //}
                ////else
                ////{
                ////    SelectedItem = null;
                ////}
            }
        }

        ////
        //// Summary:
        ////     Selects all the items in the ListBox class.
        public void SelectAll()
        {
            if (this.SelectionMode == SelectionMode.Single)
            {
                throw new NotSupportedException();
            }
            else
            {
                foreach (SelectorItem selectorItem in ItemContainerGenerator.INTERNAL_AllContainers)
                {
                    selectorItem.IsSelected = true;
                }
            }
        }
        #endregion

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            List<object> l = new List<object>();
            foreach (object item in SelectedItems)
            {
                l.Add(item);
            }
            foreach (object item in l)
            {
                SetItemVisualSelectionState(item, true);
            }
        }

        //// Summary:
        ////     Causes the object to scroll into view.
        ////
        //// Parameters:
        ////   item:
        ////     The object to scroll to.
        //public void ScrollIntoView(object item);

        [Obsolete("Use SelectedItemBackground instead.")]
        public Brush SelectedItemBackgroundBrush
        {
            get { return SelectedItemBackground; }
            set { SelectedItemBackground = value; }
        }

        [Obsolete("Use SelectedItemForeground instead.")]
        public Brush SelectedItemForegroundBrush
        {
            get { return SelectedItemForeground; }
            set { SelectedItemForeground = value; }
        }

        [Obsolete("Use RowBackground instead.")]
        public Brush UnselectedItemBackgroundBrush
        {
            get { return RowBackground; }
            set { RowBackground = value; }
        }

        [Obsolete("Use UnselectedItemForeground instead.")]
        public Brush UnselectedItemForegroundBrush
        {
            get { return UnselectedItemForeground; }
            set { UnselectedItemForeground = value; }
        }

#if WORKINPROGRESS
        [OpenSilver.NotImplemented]
        public void ScrollIntoView(object item)
        {

        }
#endif
    }
}