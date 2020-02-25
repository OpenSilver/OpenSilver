#if WORKINPROGRESS

namespace System.ComponentModel
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    //
    // Summary:
    //     Enables collections to have the functionalities of current record management,
    //     custom sorting, filtering, and grouping.
    public interface ICollectionView : IEnumerable, INotifyCollectionChanged
    {
        //
        // Summary:
        //     Gets a value that indicates whether this view supports filtering by way of the
        //     System.ComponentModel.ICollectionView.Filter property.
        //
        // Returns:
        //     true if this view supports filtering; otherwise, false.
        bool CanFilter { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this view supports grouping by way of the
        //     System.ComponentModel.ICollectionView.GroupDescriptions property.
        //
        // Returns:
        //     true if this view supports grouping; otherwise, false.
        bool CanGroup { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this view supports sorting by way of the
        //     System.ComponentModel.ICollectionView.SortDescriptions property.
        //
        // Returns:
        //     true if this view supports sorting; otherwise, false.
        bool CanSort { get; }
        //
        // Summary:
        //     Gets or sets the cultural information for any operations of the view that may
        //     differ by culture, such as sorting.
        //
        // Returns:
        //     The culture information to use during culture-sensitive operations.
        CultureInfo Culture { get; set; }
        //
        // Summary:
        //     Gets the current item in the view.
        //
        // Returns:
        //     The current item in the view or null if there is no current item.
        object CurrentItem { get; }
        //
        // Summary:
        //     Gets the ordinal position of the System.ComponentModel.ICollectionView.CurrentItem
        //     in the view.
        //
        // Returns:
        //     The ordinal position of the System.ComponentModel.ICollectionView.CurrentItem
        //     in the view.
        int CurrentPosition { get; }
        //
        // Summary:
        //     Gets or sets a callback that is used to determine whether an item is appropriate
        //     for inclusion in the view.
        //
        // Returns:
        //     A method that is used to determine whether an item is appropriate for inclusion
        //     in the view.
        Predicate<object> Filter { get; set; }
        //
        // Summary:
        //     Gets a collection of System.ComponentModel.GroupDescription objects that describe
        //     how the items in the collection are grouped in the view.
        //
        // Returns:
        //     A collection of objects that describe how the items in the collection are grouped
        //     in the view.
        ObservableCollection<GroupDescription> GroupDescriptions { get; }
        //
        // Summary:
        //     Gets the top-level groups.
        //
        // Returns:
        //     A read-only collection of the top-level groups or null if there are no groups.
        ReadOnlyObservableCollection<object> Groups { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the System.ComponentModel.ICollectionView.CurrentItem
        //     of the view is beyond the end of the collection.
        //
        // Returns:
        //     true if the System.ComponentModel.ICollectionView.CurrentItem of the view is
        //     beyond the end of the collection; otherwise, false.
        bool IsCurrentAfterLast { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the System.ComponentModel.ICollectionView.CurrentItem
        //     of the view is beyond the start of the collection.
        //
        // Returns:
        //     true if the System.ComponentModel.ICollectionView.CurrentItem of the view is
        //     beyond the start of the collection; otherwise, false.
        bool IsCurrentBeforeFirst { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the view is empty.
        //
        // Returns:
        //     true if the view is empty; otherwise, false.
        bool IsEmpty { get; }
        //
        // Summary:
        //     Gets a collection of System.ComponentModel.SortDescription instances that describe
        //     how the items in the collection are sorted in the view.
        //
        // Returns:
        //     A collection of values that describe how the items in the collection are sorted
        //     in the view.
        SortDescriptionCollection SortDescriptions { get; }
        //
        // Summary:
        //     Gets the underlying collection.
        //
        // Returns:
        //     The underlying collection.
        IEnumerable SourceCollection { get; }

        //
        // Summary:
        //     Occurs after the current item has been changed.
        event EventHandler CurrentChanged;
        //
        // Summary:
        //     Occurs before the current item changes.
        event CurrentChangingEventHandler CurrentChanging;

        //
        // Summary:
        //     Indicates whether the specified item belongs to this collection view.
        //
        // Parameters:
        //   item:
        //     The object to check.
        //
        // Returns:
        //     true if the item belongs to this collection view; otherwise, false.
        bool Contains(object item);
        //
        // Summary:
        //     Enters a defer cycle that you can use to merge changes to the view and delay
        //     automatic refresh.
        //
        // Returns:
        //     The typical usage is to create a using scope with an implementation of this method
        //     and then include multiple view-changing calls within the scope. The implementation
        //     should delay automatic refresh until after the using scope exits.
        IDisposable DeferRefresh();
        //
        // Summary:
        //     Sets the specified item in the view as the System.ComponentModel.ICollectionView.CurrentItem.
        //
        // Parameters:
        //   item:
        //     The item to set as the current item.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentTo(object item);
        //
        // Summary:
        //     Sets the first item in the view as the System.ComponentModel.ICollectionView.CurrentItem.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentToFirst();
        //
        // Summary:
        //     Sets the last item in the view as the System.ComponentModel.ICollectionView.CurrentItem.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentToLast();
        //
        // Summary:
        //     Sets the item after the System.ComponentModel.ICollectionView.CurrentItem in
        //     the view as the System.ComponentModel.ICollectionView.CurrentItem.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentToNext();
        //
        // Summary:
        //     Sets the item at the specified index to be the System.ComponentModel.ICollectionView.CurrentItem
        //     in the view.
        //
        // Parameters:
        //   position:
        //     The index to set the System.ComponentModel.ICollectionView.CurrentItem to.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentToPosition(int position);
        //
        // Summary:
        //     Sets the item before the System.ComponentModel.ICollectionView.CurrentItem in
        //     the view to the System.ComponentModel.ICollectionView.CurrentItem.
        //
        // Returns:
        //     true if the resulting System.ComponentModel.ICollectionView.CurrentItem is an
        //     item in the view; otherwise, false.
        bool MoveCurrentToPrevious();
        //
        // Summary:
        //     Recreates the view.
        void Refresh();
    }
}

#endif