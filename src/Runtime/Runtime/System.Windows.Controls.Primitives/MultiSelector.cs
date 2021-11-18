

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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Provides an abstract class for controls that allow multiple items to be selected.
    /// </summary>
    public abstract partial class MultiSelector : Selector
    {
        /// <summary>
        /// This value is used to state that we do not want SelectedItems_Changed to fire the SelectionChanged event.
        /// It should be set to true when the event is already fired by something else
        /// (for example by Selector.OnSelectedItemChanged or by a subsequent modification on SelectedItems)
        /// IMPORTANT: it needs to be set back to false once we want to allow SelectionChanged to be fired again.
        /// </summary>
        internal bool _skipSelectionChangedEvent = false;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.Primitives.MultiSelector
        /// class.
        /// </summary>
        protected MultiSelector() //todo: see if it should be internal
        {
            SelectedItems = new ObservableCollection<object>();
            ((ObservableCollection<object>)SelectedItems).CollectionChanged += SelectedItems_CollectionChanged;
        }

        void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshSelectedItems(e);
        }

        void RefreshSelectedItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnselectAllItems();
            }
            if (e.OldItems != null)
            {
                List<object> oldItems = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(e.OldItems);
                foreach (object item in oldItems)
                {
                    SetItemVisualSelectionState(item, false);
                }
            }
            if (e.NewItems != null)
            {
                List<object> newItems = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(e.NewItems);
                foreach (object item in newItems)
                {
                    SetItemVisualSelectionState(item, true);
                }
            }
            // Update the SelectedItem property:
            // based on WPF's behaviour, SelectedItem is the item that was first selected among the currently selected items:
            IList items = SelectedItems; //Note: items cannot be null as we came here as a result of it firing the CollectionChanged event.
            if (items.Count > 0)
            {
                bool tempSkipSelectionChangedEvent = _skipSelectionChangedEvent;
                _skipSelectionChangedEvent = true; //setting this to true so setting SelectedItem will not remove the items from the list.

                foreach (object item in items) //Note: items being an IList, we do not have access to ElementAt ot First so we do this.
                {
                    if (SelectedItem != item)
                    {
                        SelectedItem = item;
                    }
                    break;
                }
                _skipSelectionChangedEvent = tempSkipSelectionChangedEvent;
            } //todo: should we unset SelectedItem when the user clears SelectedItems? (in that case, do not do it if _isIntermediarySelectedItemChange is true)

            if (!_skipSelectionChangedEvent)
            {
                //Fire the SelectionChanged event:
                var removedItems = e.OldItems ?? new Collection<object>();
                var addedItems = e.NewItems ?? new Collection<object>();
                Dispatcher.BeginInvoke(() => //Note: We need to delay firing the event so we're done with everything we need to do in case the user tries to modify the selection.
                {
                    OnSelectionChanged(new SelectionChangedEventArgs(removedItems, addedItems));
                });
            }

        }

        protected abstract void UnselectAllItems();

        protected abstract void SetItemVisualSelectionState(object item, bool newState);


        protected override void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged_BeforeVisualUpdate(oldValue, newValue);
            if (SelectedItems != null && SelectedItems.Count != 0)
            {
                SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Gets the items in the System.Windows.Controls.Primitives.MultiSelector that
        /// are selected.
        /// </summary>
        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            private set { SetValue(SelectedItemsProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelector), new PropertyMetadata(null));



        //// Summary:
        ////     Gets or sets a value that indicates whether the multiple items in the System.Windows.Controls.Primitives.MultiSelector
        ////     can be selected at a time.
        ////
        //// Returns:
        ////     true if multiple items in the System.Windows.Controls.Primitives.MultiSelector
        ////     can be selected at a time; otherwise, false.
        //protected bool CanSelectMultipleItems { get; set; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether the System.Windows.Controls.Primitives.MultiSelector
        ////     is currently performing a bulk update to the System.Windows.Controls.Primitives.MultiSelector.SelectedItems
        ////     collection.
        ////
        //// Returns:
        ////     true if the System.Windows.Controls.Primitives.MultiSelector is currently
        ////     performing a bulk update to the System.Windows.Controls.Primitives.MultiSelector.SelectedItems
        ////     collection; otherwise, false.
        //protected bool IsUpdatingSelectedItems { get; }


        //// Summary:
        ////     Starts a new selection transaction.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     System.Windows.Controls.Primitives.MultiSelector.IsUpdatingSelectedItems
        ////     is true when this method is called.
        //protected void BeginUpdateSelectedItems();
        ////
        //// Summary:
        ////     Commits the selected items to the System.Windows.Controls.Primitives.MultiSelector.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     System.Windows.Controls.Primitives.MultiSelector.IsUpdatingSelectedItems
        ////     is false when this method is called.
        //protected void EndUpdateSelectedItems();
        ////
        //// Summary:
        ////     Selects all of the items in the System.Windows.Controls.Primitives.MultiSelector.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     System.Windows.Controls.Primitives.MultiSelector.CanSelectMultipleItems is
        ////     false.
        //public void SelectAll();
        ////
        //// Summary:
        ////     Unselects all of the items in the System.Windows.Controls.Primitives.MultiSelector.
        //public void UnselectAll();
    }
}
