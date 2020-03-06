#if WORKINPROGRESS

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	//
	// Summary:
	//     Represents a view for grouping, sorting, filtering, and navigating a paged data
	//     collection.
	public sealed class PagedCollectionView : ICollectionView, IEnumerable, INotifyCollectionChanged, IPagedCollectionView, IEditableCollectionView, INotifyPropertyChanged
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Data.PagedCollectionView class.
		//
		// Parameters:
		//   source:
		//     The source for the collection.
		public PagedCollectionView(IEnumerable source)
		{
			
		}
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Data.PagedCollectionView class
		//     and specifies whether the data is sorted and in group order.
		//
		// Parameters:
		//   source:
		//     The source for the collection.
		//
		//   isDataSorted:
		//     Specifies whether the source is already sorted.
		//
		//   isDataInGroupOrder:
		//     Specifies whether the source is already in the correct order for grouping.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     source is null.
		public PagedCollectionView(IEnumerable source, bool isDataSorted, bool isDataInGroupOrder)
		{
			
		}

		//
		// Summary:
		//     Gets the item at the specified index.
		//
		// Parameters:
		//   index:
		//     The index of the item to be retrieved.
		//
		// Returns:
		//     The item at the specified index.
		public object this[int index] => default(object);

		//
		// Summary:
		//     Gets a value that indicates whether a new item can be added to the collection.
		//
		// Returns:
		//     true if a new item can be added to the collection; otherwise, false.
		public bool CanAddNew { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the collection view can discard pending changes
		//     and restore the original values of an edited object.
		//
		// Returns:
		//     true if the collection view can discard pending changes and restore the original
		//     values of an edited object; otherwise, false.
		public bool CanCancelEdit { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Data.PagedCollectionView.PageIndex
		//     value can change.
		//
		// Returns:
		//     true in all cases.
		public bool CanChangePage { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this view supports filtering by way of the
		//     System.Windows.Data.PagedCollectionView.Filter property.
		//
		// Returns:
		//     true in all cases.
		public bool CanFilter { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this view supports grouping.
		//
		// Returns:
		//     true in all cases.
		public bool CanGroup { get; }
		//
		// Summary:
		//     Gets a value that indicates whether an item can be removed from the collection.
		//
		// Returns:
		//     true if an item can be removed from the collection; otherwise, false.
		public bool CanRemove { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this view supports sorting.
		//
		// Returns:
		//     true in all cases.
		public bool CanSort { get; }
		//
		// Summary:
		//     Gets the number of records in the view after filtering, sorting, and paging.
		//
		// Returns:
		//     The number of records in the view after filtering, sorting, and paging.
		public int Count { get; }
		//
		// Summary:
		//     Gets or sets the cultural information for any operations of the view that might
		//     differ by culture, such as sorting.
		//
		// Returns:
		//     The culture to use during view operations.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     The culture information is null.
		public CultureInfo Culture { get; set; }
		//
		// Summary:
		//     Gets the item that is being added when an System.Windows.Data.PagedCollectionView.AddNew
		//     transaction is in progress.
		//
		// Returns:
		//     The item that is being added if System.Windows.Data.PagedCollectionView.IsAddingNew
		//     is true; otherwise, null.
		public object CurrentAddItem { get; }
		//
		// Summary:
		//     Gets the item in the collection that is being edited when an System.Windows.Data.PagedCollectionView.EditItem(System.Object)
		//     transaction is in progress.
		//
		// Returns:
		//     The item that is being edited if System.Windows.Data.PagedCollectionView.IsEditingItem
		//     is true; otherwise, null.
		public object CurrentEditItem { get; }
		//
		// Summary:
		//     Gets the current item in the view.
		//
		// Returns:
		//     The current item in the view or null if there is no current item.
		public object CurrentItem { get; }
		//
		// Summary:
		//     Gets the ordinal position of the System.Windows.Data.PagedCollectionView.CurrentItem
		//     in the view, which might be sorted and filtered.
		//
		// Returns:
		//     The ordinal position of the System.Windows.Data.PagedCollectionView.CurrentItem
		//     in the view.
		public int CurrentPosition { get; }
		//
		// Summary:
		//     Gets or sets a callback that is used to determine whether an item is suited for
		//     inclusion in the view.
		//
		// Returns:
		//     A method that is used to determine whether an item is suited for inclusion in
		//     the view.
		//
		// Exceptions:
		//   T:System.NotSupportedException:
		//     The implementation does not support filtering.Simpler implementations do not
		//     support filtering and will throw a System.NotSupportedException. Check the System.Windows.Data.PagedCollectionView.CanFilter
		//     property to test whether filtering is supported before assigning a non-null value.
		//
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsAddingNew is true.-or-System.Windows.Data.PagedCollectionView.IsEditingItem
		//     is true.
		public Predicate<object> Filter { get; set; }
		//
		// Summary:
		//     Gets a collection of System.ComponentModel.GroupDescription objects that describe
		//     how the items in the collection are grouped in the view.
		//
		// Returns:
		//     A collection of System.ComponentModel.GroupDescription objects that describe
		//     how the items in the collection are grouped in the view.
		public ObservableCollection<GroupDescription> GroupDescriptions { get; }
		//
		// Summary:
		//     Gets the top-level groups, constructed according to the descriptions specified
		//     in the System.Windows.Data.PagedCollectionView.GroupDescriptions property.
		//
		// Returns:
		//     A read-only collection of the top-level groups or null if there are no groups.
		public ReadOnlyObservableCollection<object> Groups { get; }
		//
		// Summary:
		//     Gets a value that indicates whether an System.Windows.Data.PagedCollectionView.AddNew
		//     transaction is in progress.
		//
		// Returns:
		//     true if an System.Windows.Data.PagedCollectionView.AddNew transaction is in progress;
		//     otherwise, false.
		public bool IsAddingNew { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Data.PagedCollectionView.CurrentItem
		//     of the view is beyond the end of the collection.
		//
		// Returns:
		//     true if the System.Windows.Data.PagedCollectionView.CurrentItem of the view is
		//     beyond the end of the collection; otherwise, false.
		public bool IsCurrentAfterLast { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Data.PagedCollectionView.CurrentItem
		//     of the view is before the start of the collection.
		//
		// Returns:
		//     true if the System.Windows.Data.PagedCollectionView.CurrentItem of the view is
		//     before the start of the collection; otherwise, false.
		public bool IsCurrentBeforeFirst { get; }
		//
		// Summary:
		//     Gets a value that indicates whether an System.Windows.Data.PagedCollectionView.EditItem(System.Object)
		//     transaction is in progress.
		//
		// Returns:
		//     true if an System.Windows.Data.PagedCollectionView.EditItem(System.Object) transaction
		//     is in progress; otherwise, false.
		public bool IsEditingItem { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the view is empty.
		//
		// Returns:
		//     true if the view is empty; otherwise, false.
		public bool IsEmpty { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the page index is changing.
		//
		// Returns:
		//     true if the page index is changing; otherwise, false.
		public bool IsPageChanging { get; }
		//
		// Summary:
		//     Gets the minimum number of items known to be in the source collection that satisfy
		//     the current filter.
		//
		// Returns:
		//     The minimum number of items known to be in the collection that satisfy the current
		//     filter.
		public int ItemCount { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this view needs to be refreshed.
		//
		// Returns:
		//     true if the view needs to be refreshed; otherwise, false.
		public bool NeedsRefresh { get; }
		//
		// Summary:
		//     Gets or sets a value that indicates whether to include a new item placeholder
		//     in the collection view, and where to include it.
		//
		// Returns:
		//     This implementation always returns System.ComponentModel.NewItemPlaceholderPosition.None.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The value is not System.ComponentModel.NewItemPlaceholderPosition.None.
		public NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }
		//
		// Summary:
		//     Gets the zero-based index of the current page.
		//
		// Returns:
		//     The zero-based index of the current page.
		public int PageIndex { get; }
		//
		// Summary:
		//     Gets or sets the number of items to display on a page.
		//
		// Returns:
		//     The number of items to display on a page.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     The page size is less than 0.
		//
		//   T:System.InvalidOperationException:
		//     The page size could not be changed because there is a new or edited item that
		//     could not be committed.
		public int PageSize { get; set; }
		//
		// Summary:
		//     Gets a collection of System.ComponentModel.SortDescription objects that describe
		//     how the items in the collection are sorted in the view.
		//
		// Returns:
		//     A collection of System.ComponentModel.SortDescription objects that describe how
		//     the items in the collection are sorted in the view.
		public SortDescriptionCollection SortDescriptions { get; }
		//
		// Summary:
		//     Gets the System.Collections.IEnumerable collection underlying this view.
		//
		// Returns:
		//     The System.Collections.IEnumerable collection underlying this view.
		public IEnumerable SourceCollection { get; }
		//
		// Summary:
		//     Gets the total number of items in the view before paging is applied.
		//
		// Returns:
		//     The total number of items in the view before paging is applied.
		public int TotalItemCount { get; }

		//
		// Summary:
		//     Occurs when the view has changed.
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		//
		// Summary:
		//     Occurs after the System.Windows.Data.PagedCollectionView.CurrentItem has changed.
		public event EventHandler CurrentChanged;
		//
		// Summary:
		//     Occurs when the System.Windows.Data.PagedCollectionView.CurrentItem is changing.
		public event CurrentChangingEventHandler CurrentChanging;
		//
		// Summary:
		//     Occurs after the System.Windows.Data.PagedCollectionView.PageIndex has changed.
		public event EventHandler<EventArgs> PageChanged;
		//
		// Summary:
		//     Occurs when the System.Windows.Data.PagedCollectionView.PageIndex is changing.
		public event EventHandler<PageChangingEventArgs> PageChanging;
		//
		// Summary:
		//     Occurs after a property value has changed.
		public event PropertyChangedEventHandler PropertyChanged;

		//
		// Summary:
		//     Adds a new item to the underlying collection.
		//
		// Returns:
		//     The new item that is being added.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.CanAddNew is false.
		public object AddNew()
		{
			return default(object);
		}
		//
		// Summary:
		//     Ends the edit transaction and, if it is possible, restores the original value
		//     of the item.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.CanCancelEdit is false.-or-System.Windows.Data.PagedCollectionView.IsAddingNew
		//     is true.
		public void CancelEdit()
		{
			
		}
		//
		// Summary:
		//     Ends the add transaction and discards the pending new item.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsEditingItem is true.
		public void CancelNew()
		{
			
		}
		//
		// Summary:
		//     Ends the edit transaction and saves the pending changes.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsAddingNew is true.
		public void CommitEdit()
		{
			
		}
		//
		// Summary:
		//     Ends the add transaction and saves the pending new item.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsEditingItem is true.
		public void CommitNew()
		{
			
		}
		//
		// Summary:
		//     Returns a value that indicates whether the specified item belongs to this collection
		//     view.
		//
		// Parameters:
		//   item:
		//     The object to check.
		//
		// Returns:
		//     true if the specified item belongs to this collection view; otherwise, false.
		public bool Contains(object item)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Enters a defer cycle that you can use to merge changes to the view and delay
		//     automatic refresh.
		//
		// Returns:
		//     An System.IDisposable object that you can use to dispose of the calling object.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsAddingNew is true.-or-System.Windows.Data.PagedCollectionView.IsEditingItem
		//     is true.
		public IDisposable DeferRefresh()
		{
			return default(IDisposable);
		}
		//
		// Summary:
		//     Begins an edit transaction on the specified item.
		//
		// Parameters:
		//   item:
		//     The item to edit.
		public void EditItem(object item)
		{
			
		}
		//
		// Summary:
		//     Returns an System.Collections.IEnumerator object that you can use to enumerate
		//     the items in the view.
		//
		// Returns:
		//     An System.Collections.IEnumerator object that you can use to enumerate the items
		//     in the view.
		public IEnumerator GetEnumerator()
		{
			return default(IEnumerator);
		}
		//
		// Summary:
		//     Gets the item at the specified zero-based index in this System.Windows.Data.PagedCollectionView,
		//     after the source collection is filtered, sorted, and paged.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to retrieve.
		//
		// Returns:
		//     The item at the specified zero-based index.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is less than 0, or greater than or equal to the System.Windows.Data.PagedCollectionView.Count
		//     of items in the view.
		public object GetItemAt(int index)
		{
			return default(object);
		}
		//
		// Summary:
		//     Returns the zero-based index at which the specified item is located.
		//
		// Parameters:
		//   item:
		//     The item to locate.
		//
		// Returns:
		//     The index at which the specified item is located, or –1 if the item is unknown.
		public int IndexOf(object item)
		{
			return default(int);
		}
		//
		// Summary:
		//     Sets the specified item to be the System.Windows.Data.PagedCollectionView.CurrentItem
		//     in the view.
		//
		// Parameters:
		//   item:
		//     The item to set as the System.Windows.Data.PagedCollectionView.CurrentItem.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		public bool MoveCurrentTo(object item)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the first item in the view as the System.Windows.Data.PagedCollectionView.CurrentItem.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		public bool MoveCurrentToFirst()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the last item in the view as the System.Windows.Data.PagedCollectionView.CurrentItem.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		public bool MoveCurrentToLast()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Set the item after the System.Windows.Data.PagedCollectionView.CurrentItem in
		//     the view as the System.Windows.Data.PagedCollectionView.CurrentItem.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		public bool MoveCurrentToNext()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the item at the specified index to be the System.Windows.Data.PagedCollectionView.CurrentItem
		//     in the view.
		//
		// Parameters:
		//   position:
		//     The index to set the System.Windows.Data.PagedCollectionView.CurrentItem to.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     position is less than -1 or greater than the System.Windows.Data.PagedCollectionView.Count
		//     of items in the view.
		public bool MoveCurrentToPosition(int position)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the item before the System.Windows.Data.PagedCollectionView.CurrentItem
		//     in the view as the System.Windows.Data.PagedCollectionView.CurrentItem.
		//
		// Returns:
		//     true if the resulting System.Windows.Data.PagedCollectionView.CurrentItem is
		//     an item in the view; otherwise, false.
		public bool MoveCurrentToPrevious()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the first page as the current page.
		//
		// Returns:
		//     true if the operation was successful; otherwise, false.
		public bool MoveToFirstPage()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Sets the last page as the current page.
		//
		// Returns:
		//     true if the operation was successful; otherwise, false.
		public bool MoveToLastPage()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Moves to the page after the current page.
		//
		// Returns:
		//     true if the operation was successful; otherwise, false.
		public bool MoveToNextPage()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Moves to the page at the specified index.
		//
		// Parameters:
		//   pageIndex:
		//     The index of the page to move to.
		//
		// Returns:
		//     true if the operation was successful; otherwise, false.
		public bool MoveToPage(int pageIndex)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Moves to the page before the current page.
		//
		// Returns:
		//     true if the operation was successful; otherwise, false.
		public bool MoveToPreviousPage()
		{
			return default(bool);
		}
		//
		// Summary:
		//     Returns a value that indicates whether the specified item in the underlying collection
		//     belongs to the view after filters are applied.
		//
		// Parameters:
		//   item:
		//     The item to check.
		//
		// Returns:
		//     true if the specified item belongs to the filtered view or if no filter is set
		//     on the collection view; otherwise, false.
		public bool PassesFilter(object item)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Re-creates the view.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsAddingNew is true.-or- System.Windows.Data.PagedCollectionView.IsEditingItem
		//     is true.
		public void Refresh()
		{
			
		}
		//
		// Summary:
		//     Removes the specified item from the collection.
		//
		// Parameters:
		//   item:
		//     The item to remove.
		public void Remove(object item)
		{
			
		}
		//
		// Summary:
		//     Removes the item at the specified position from the collection.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to remove.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is less than 0, or greater than or equal to the System.Windows.Data.PagedCollectionView.Count
		//     of items in the view.
		//
		//   T:System.InvalidOperationException:
		//     System.Windows.Data.PagedCollectionView.IsAddingNew is true.-or- System.Windows.Data.PagedCollectionView.IsEditingItem
		//     is true.-or-System.Windows.Data.PagedCollectionView.CanRemove is false.
		public void RemoveAt(int index)
		{
			
		}
	}
}

#else

using System.Collections;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	//
	// Summary:
	//     Represents a view for grouping, sorting, filtering, and navigating a paged data
	//     collection.
	public sealed class PagedCollectionView : INTERNAL_PagedCollectionView
	{
		public PagedCollectionView(IEnumerable source) : base(source)
		{
		}
	}
}

#endif