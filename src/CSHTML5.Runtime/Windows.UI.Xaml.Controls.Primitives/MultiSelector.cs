

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
        }

        protected abstract void UnselectAllItems();

        protected abstract void SetItemVisualSelectionState(object item, bool newState);


        protected override void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged_BeforeVisualUpdate(oldValue, newValue);
            if (SelectedItems != null && SelectedItems.Count != 0)
            {
                INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
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
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelector), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



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
