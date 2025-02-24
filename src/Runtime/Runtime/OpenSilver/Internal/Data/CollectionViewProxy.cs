// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Proxy that adds context affinity to an ICollectionView that
//              doesn't already have it.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

///<summary>
/// Proxy view, used to interpose between the UI and a view that doesn't
/// support context affinity.
///</summary>
internal sealed class CollectionViewProxy : CollectionView, IEditableCollectionViewAddNewItem, IItemProperties
{
    internal CollectionViewProxy(ICollectionView view)
        : base(view.SourceCollection, false)
    {
        _view = view;

        view.CollectionChanged += new NotifyCollectionChangedEventHandler(OnViewChanged);

        view.CurrentChanging += new CurrentChangingEventHandler(OnCurrentChanging);
        view.CurrentChanged += new EventHandler(OnCurrentChanged);

        INotifyPropertyChanged ipc = view as INotifyPropertyChanged;
        if (ipc != null)
            ipc.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
    }

    /// <summary>
    /// Culture to use during sorting.
    /// </summary>
    public override CultureInfo Culture
    {
        get { return ProxiedView.Culture; }
        set { ProxiedView.Culture = value; }
    }

    /// <summary>
    /// Return true if the item belongs to this view.  No assumptions are
    /// made about the item. This method will behave similarly to IList.Contains().
    /// If the caller knows that the item belongs to the
    /// underlying collection, it is more efficient to call PassesFilter.
    /// </summary>
    public override bool Contains(object item)
    {
        return ProxiedView.Contains(item);
    }

    /// <summary>
    /// SourceCollection is the original un-filtered collection of which
    /// this ICollectionView is a view.
    /// </summary>
    public override IEnumerable SourceCollection
    {
        get { return base.SourceCollection; }
    }

    /// <summary>
    /// Set/get a filter callback to filter out items in collection.
    /// This property will always accept a filter, but the collection view for the
    /// underlying InnerList or ItemsSource may not actually support filtering.
    /// Please check <seealso cref="CanFilter"/>
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Collections assigned to ItemsSource may not support filtering and could throw a NotSupportedException.
    /// Use <seealso cref="CanSort"/> property to test if sorting is supported before adding
    /// to SortDescriptions.
    /// </exception>
    public override Predicate<object> Filter
    {
        get { return ProxiedView.Filter; }
        set { ProxiedView.Filter = value; }
    }

    /// <summary>
    /// Test if this ICollectionView supports filtering before assigning
    /// a filter callback to <seealso cref="Filter"/>.
    /// </summary>
    public override bool CanFilter
    {
        get { return ProxiedView.CanFilter; }
    }

    /// <summary>
    /// Set/get Sort criteria to sort items in collection.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Clear a sort criteria by assigning SortDescription.Empty to this property.
    /// One or more sort criteria in form of <seealso cref="SortDescription"/>
    /// can be used, each specifying a property and direction to sort by.
    /// </p>
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Simpler implementations do not support sorting and will throw a NotSupportedException.
    /// Use <seealso cref="CanSort"/> property to test if sorting is supported before adding
    /// to SortDescriptions.
    /// </exception>
    public override SortDescriptionCollection SortDescriptions
    {
        get { return ProxiedView.SortDescriptions; }
    }

    /// <summary>
    /// Test if this ICollectionView supports sorting before adding
    /// to <seealso cref="SortDescriptions"/>.
    /// </summary>
    public override bool CanSort
    {
        get { return ProxiedView.CanSort; }
    }

    /// <summary>
    /// Returns true if this view really supports grouping.
    /// When this returns false, the rest of the interface is ignored.
    /// </summary>
    public override bool CanGroup
    {
        get { return ProxiedView.CanGroup; }
    }

    /// <summary>
    /// The description of grouping, indexed by level.
    /// </summary>
    public override ObservableCollection<GroupDescription> GroupDescriptions
    {
        get { return ProxiedView.GroupDescriptions; }
    }

    /// <summary>
    /// The top-level groups, constructed according to the descriptions
    /// given in GroupDescriptions.
    /// </summary>
    public override ReadOnlyObservableCollection<object> Groups
    {
        get { return ProxiedView.Groups; }
    }

    /// Re-create the view, using any <seealso cref="SortDescriptions"/>.
    public override void Refresh()
    {
        IndexedEnumerable indexer = Interlocked.Exchange(ref _indexer, null);
        indexer?.Invalidate();

        ProxiedView.Refresh();
    }

    /// <summary>
    /// Enter a Defer Cycle.
    /// Defer cycles are used to coalesce changes to the ICollectionView.
    /// </summary>
    public override IDisposable DeferRefresh()
    {
        return ProxiedView.DeferRefresh();
    }

    /// <summary> Return current item. </summary>
    public override object CurrentItem
    {
        get { return ProxiedView.CurrentItem; }
    }

    /// <summary>
    /// The ordinal position of the <seealso cref="CurrentItem"/> within the (optionally
    /// sorted and filtered) view.
    /// </summary>
    public override int CurrentPosition
    {
        get { return ProxiedView.CurrentPosition; }
    }

    /// <summary> Return true if currency is beyond the end (End-Of-File). </summary>
    public override bool IsCurrentAfterLast
    {
        get { return ProxiedView.IsCurrentAfterLast; }
    }

    /// <summary> Return true if currency is before the beginning (Beginning-Of-File). </summary>
    public override bool IsCurrentBeforeFirst
    {
        get { return ProxiedView.IsCurrentBeforeFirst; }
    }

    /// <summary> Move to the first item. </summary>
    public override bool MoveCurrentToFirst()
    {
        return ProxiedView.MoveCurrentToFirst();
    }

    /// <summary> Move to the previous item. </summary>
    public override bool MoveCurrentToPrevious()
    {
        return ProxiedView.MoveCurrentToPrevious();
    }

    /// <summary> Move to the next item. </summary>
    public override bool MoveCurrentToNext()
    {
        return ProxiedView.MoveCurrentToNext();
    }

    /// <summary> Move to the last item. </summary>
    public override bool MoveCurrentToLast()
    {
        return ProxiedView.MoveCurrentToLast();
    }

    /// <summary> Move to the given item. </summary>
    public override bool MoveCurrentTo(object item)
    {
        return ProxiedView.MoveCurrentTo(item);
    }

    /// <summary>Move CurrentItem to this index</summary>
    public override bool MoveCurrentToPosition(int position)
    {
        //
        // If the index is out of range here, I'll let the
        // ProxiedView be the one to make that determination.
        //
        return ProxiedView.MoveCurrentToPosition(position);
    }

    public override event CurrentChangingEventHandler CurrentChanging
    {
        add { PrivateCurrentChanging += value; }
        remove { PrivateCurrentChanging -= value; }
    }

    public override event EventHandler CurrentChanged
    {
        add { PrivateCurrentChanged += value; }
        remove { PrivateCurrentChanged -= value; }
    }

    /// <summary>
    /// Return the number of records (or -1, meaning "don't know").
    /// A virtualizing view should return the best estimate it can
    /// without de-virtualizing all the data.  A non-virtualizing view
    /// should return the exact count of its (filtered) data.
    /// </summary>
    public override int Count
    {
        get { return EnumerableWrapper.Count; }
    }

    public override bool IsEmpty
    {
        get { return ProxiedView.IsEmpty; }
    }


    public ICollectionView ProxiedView
    {
        get
        {
            //                 VerifyAccess();
            return _view;
        }
    }

    /// <summary> Return the index where the given de belongs, or -1 if this index is unknown.
    /// More precisely, if this returns an index other than -1, it must always be true that
    /// view[index-1] &lt; de &lt;= view[index], where the comparisons are done via
    /// the view's IComparer.Compare method (if any).
    /// </summary>
    /// <param name="item">data item</param>
    public override int IndexOf(object item)
    {
        return EnumerableWrapper.IndexOf(item);
    }

    /// <summary>
    /// Return true if the item belongs to this view.  The item is assumed to belong to the
    /// underlying DataCollection;  this method merely takes filters into account.
    /// It is commonly used during collection-changed notifications to determine if the added/removed
    /// item requires processing.
    /// Returns true if no filter is set on collection view.
    /// </summary>
    public override bool PassesFilter(object item)
    {
        if (ProxiedView.CanFilter && ProxiedView.Filter != null &&
                item != NewItemPlaceholder && item != ((IEditableCollectionView)this).CurrentAddItem)
            return ProxiedView.Filter(item);

        return true;
    }

    /// <summary>
    /// Retrieve item at the given zero-based index in this CollectionView.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if index is out of range
    /// </exception>
    public override object GetItemAt(int index)
    {
        // only check lower bound because Count could be expensive
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        return EnumerableWrapper[index];
    }

    /// <summary>
    /// Detach from the source collection.  (I.e. stop listening to the collection's
    /// events, or anything else that makes the CollectionView ineligible for
    /// garbage collection.)
    /// </summary>
    public override void DetachFromSourceCollection()
    {
        if (_view != null)
        {
            _view.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnViewChanged);

            _view.CurrentChanging -= new CurrentChangingEventHandler(OnCurrentChanging);
            _view.CurrentChanged -= new EventHandler(OnCurrentChanged);

            INotifyPropertyChanged ipc = _view as INotifyPropertyChanged;
            if (ipc != null)
                ipc.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);

            _view = null;
        }

        base.DetachFromSourceCollection();
    }

    /// <summary>
    /// Indicates whether to include a placeholder for a new item, and if so,
    /// where to put it.
    /// </summary>
    NewItemPlaceholderPosition IEditableCollectionView.NewItemPlaceholderPosition
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.NewItemPlaceholderPosition;
            }
            else
            {
                return NewItemPlaceholderPosition.None;
            }
        }
        set
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                ecv.NewItemPlaceholderPosition = value;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.NewItemPlaceholderPosition)));
            }
        }
    }

    /// <summary>
    /// Return true if the view supports <seealso cref="IEditableCollectionView.AddNew"/>.
    /// </summary>
    bool IEditableCollectionView.CanAddNew
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.CanAddNew;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Add a new item to the underlying collection.  Returns the new item.
    /// After calling AddNew and changing the new item as desired, either
    /// <seealso cref="IEditableCollectionView.CommitNew"/> or <seealso cref="IEditableCollectionView.CancelNew"/> should be
    /// called to complete the transaction.
    /// </summary>
    object IEditableCollectionView.AddNew()
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            return ecv.AddNew();
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.AddNew)));
        }
    }


    /// <summary>
    /// Complete the transaction started by <seealso cref="IEditableCollectionView.AddNew"/>.  The new
    /// item remains in the collection, and the view's sort, filter, and grouping
    /// specifications (if any) are applied to the new item.
    /// </summary>
    void IEditableCollectionView.CommitNew()
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.CommitNew();
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.CommitNew)));
        }
    }

    /// <summary>
    /// Complete the transaction started by <seealso cref="IEditableCollectionView.AddNew"/>.  The new
    /// item is removed from the collection.
    /// </summary>
    void IEditableCollectionView.CancelNew()
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.CancelNew();
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.CancelNew)));
        }
    }

    /// <summary>
    /// Returns true if an <seealso cref="IEditableCollectionView.AddNew"/> transaction is in progress.
    /// </summary>
    bool IEditableCollectionView.IsAddingNew
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.IsAddingNew;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// When an <seealso cref="IEditableCollectionView.AddNew"/> transaction is in progress, this property
    /// returns the new item.  Otherwise it returns null.
    /// </summary>
    object IEditableCollectionView.CurrentAddItem
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.CurrentAddItem;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Return true if the view supports <seealso cref="IEditableCollectionView.Remove"/> and
    /// <seealso cref="IEditableCollectionView.RemoveAt"/>.
    /// </summary>
    bool IEditableCollectionView.CanRemove
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.CanRemove;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Remove the item at the given index from the underlying collection.
    /// The index is interpreted with respect to the view (not with respect to
    /// the underlying collection).
    /// </summary>
    void IEditableCollectionView.RemoveAt(int index)
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.RemoveAt(index);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.RemoveAt)));
        }
    }

    /// <summary>
    /// Remove the given item from the underlying collection.
    /// </summary>
    void IEditableCollectionView.Remove(object item)
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.Remove(item);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.Remove)));
        }
    }

    /// <summary>
    /// Begins an editing transaction on the given item.  The transaction is
    /// completed by calling either <seealso cref="IEditableCollectionView.CommitEdit"/> or
    /// <seealso cref="IEditableCollectionView.CancelEdit"/>.  Any changes made to the item during
    /// the transaction are considered "pending", provided that the view supports
    /// the notion of "pending changes" for the given item.
    /// </summary>
    void IEditableCollectionView.EditItem(object item)
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.EditItem(item);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.EditItem)));
        }
    }

    /// <summary>
    /// Complete the transaction started by <seealso cref="IEditableCollectionView.EditItem"/>.
    /// The pending changes (if any) to the item are committed.
    /// </summary>
    void IEditableCollectionView.CommitEdit()
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.CommitEdit();
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.CommitEdit)));
        }
    }

    /// <summary>
    /// Complete the transaction started by <seealso cref="IEditableCollectionView.EditItem"/>.
    /// The pending changes (if any) to the item are discarded.
    /// </summary>
    void IEditableCollectionView.CancelEdit()
    {
        IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
        if (ecv != null)
        {
            ecv.CancelEdit();
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionView.CancelEdit)));
        }
    }

    /// <summary>
    /// Returns true if the view supports the notion of "pending changes" on the
    /// current edit item.  This may vary, depending on the view and the particular
    /// item.  For example, a view might return true if the current edit item
    /// implements <seealso cref="IEditableObject"/>, or if the view has special
    /// knowledge about the item that it can use to support rollback of pending
    /// changes.
    /// </summary>
    bool IEditableCollectionView.CanCancelEdit
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.CanCancelEdit;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Returns true if an <seealso cref="IEditableCollectionView.EditItem"/> transaction is in progress.
    /// </summary>
    bool IEditableCollectionView.IsEditingItem
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.IsEditingItem;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// When an <seealso cref="IEditableCollectionView.EditItem"/> transaction is in progress, this property
    /// returns the affected item.  Otherwise it returns null.
    /// </summary>
    object IEditableCollectionView.CurrentEditItem
    {
        get
        {
            IEditableCollectionView ecv = ProxiedView as IEditableCollectionView;
            if (ecv != null)
            {
                return ecv.CurrentEditItem;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Return true if the view supports <seealso cref="IEditableCollectionViewAddNewItem.AddNewItem"/>.
    /// </summary>
    bool IEditableCollectionViewAddNewItem.CanAddNewItem
    {
        get
        {
            IEditableCollectionViewAddNewItem ani = ProxiedView as IEditableCollectionViewAddNewItem;
            if (ani != null)
            {
                return ani.CanAddNewItem;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Add a new item to the underlying collection.  Returns the new item.
    /// After calling AddNewItem and changing the new item as desired, either
    /// <seealso cref="IEditableCollectionView.CommitNew"/> or <seealso cref="IEditableCollectionView.CancelNew"/> should be
    /// called to complete the transaction.
    /// </summary>
    object IEditableCollectionViewAddNewItem.AddNewItem(object newItem)
    {
        IEditableCollectionViewAddNewItem ani = ProxiedView as IEditableCollectionViewAddNewItem;
        if (ani != null)
        {
            return ani.AddNewItem(newItem);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(IEditableCollectionViewAddNewItem.AddNewItem)));
        }
    }

    /// <summary>
    /// Returns information about the properties available on items in the
    /// underlying collection.  This information may come from a schema, from
    /// a type descriptor, from a representative item, or from some other source
    /// known to the view.
    /// </summary>
    ReadOnlyCollection<ItemPropertyInfo> IItemProperties.ItemProperties
    {
        get
        {
            IItemProperties iip = ProxiedView as IItemProperties;
            if (iip != null)
            {
                return iip.ItemProperties;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary> Implementation of IEnumerable.GetEnumerator().
    /// This provides a way to enumerate the members of the collection
    /// without changing the currency.
    /// </summary>
    protected override IEnumerator GetEnumerator() { return ((IEnumerable)ProxiedView).GetEnumerator(); }

    internal override void GetCollectionChangedSources(int level, Action<int, object, bool?, List<string>> format, List<string> sources)
    {
        format(level, this, false, sources);
        if (_view != null)
        {
            format(level + 1, _view, true, sources);

            object collection = _view.SourceCollection;
            if (collection != null)
            {
                format(level + 2, collection, null, sources);
            }
        }
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        OnPropertyChanged(args);
    }

    void OnViewChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        //             VerifyAccess();    // will throw an exception if caller is not in correct UiContext

        OnCollectionChanged(args);
    }

    void OnCurrentChanging(object sender, CurrentChangingEventArgs args)
    {
        //             VerifyAccess();    // will throw an exception if caller is not in correct UiContext

        PrivateCurrentChanging?.Invoke(this, args);
    }

    void OnCurrentChanged(object sender, EventArgs args)
    {
        //             VerifyAccess();    // will throw an exception if caller is not in correct UiContext

        PrivateCurrentChanged?.Invoke(this, args);
    }

    private IndexedEnumerable EnumerableWrapper
    {
        get
        {
            if (_indexer == null)
            {
                IndexedEnumerable newIndexer = new IndexedEnumerable(ProxiedView, new Predicate<object>(this.PassesFilter));
                Interlocked.CompareExchange(ref _indexer, newIndexer, null);
            }

            return _indexer;
        }
    }

    private ICollectionView _view;

    private IndexedEnumerable _indexer;

    private event CurrentChangingEventHandler PrivateCurrentChanging;
    private event EventHandler PrivateCurrentChanged;
}