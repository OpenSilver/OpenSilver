
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
