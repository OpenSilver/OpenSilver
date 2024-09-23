
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

using System.Collections;               // IComparer
using System.Collections.ObjectModel;   // ObservableCollection
using System.Collections.Specialized;   // NotifyCollectionChangedEvent
using System.Diagnostics;               // Debug
using System.Globalization;             // CultureInfo

namespace System.ComponentModel;

/// <summary>
/// Provides a base class for defining how to divide the items in a collection into groups.
/// </summary>
public abstract class GroupDescription : INotifyPropertyChanged
{
    private SortDescriptionCollection _sort;
    private IComparer _customSort;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupDescription"/> class.
    /// </summary>
    protected GroupDescription()
    {
        GroupNames = new ObservableCollection<object>();
        GroupNames.CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupNamesChanged);
    }

    /// <summary>
    /// This event is raised when a property of the group description has changed.
    /// </summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    /// <summary>
    /// Occurs when a property value has changed.
    /// </summary>
    protected virtual event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    /// <summary>
    /// Gets the collection of group names.
    /// </summary>
    /// <returns>
    /// The collection of group names.
    /// </returns>
    public ObservableCollection<object> GroupNames { get; }

    /// <summary>
    /// Gets the collection of sort criteria in which to sort the groups.
    /// </summary>
    /// <returns>
    /// The collection of sort criteria in which to sort the groups.
    /// </returns>
    public SortDescriptionCollection SortDescriptions
    {
        get
        {
            if (_sort is null)
            {
                SetSortDescriptions(new SortDescriptionCollection());
            }
            return _sort;
        }
    }

    /// <summary>
    /// Gets or sets a custom comparer that sorts groups using an object that implements <see cref="IComparer"/>.
    /// </summary>
    /// <returns>
    /// A custom comparer that sorts groups using an object that implements <see cref="IComparer"/>.
    /// </returns>
    public IComparer CustomSort
    {
        get { return _customSort; }
        set
        {
            _customSort = value;
            SetSortDescriptions(null);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CustomSort)));
        }
    }

    /// <summary>
    /// Indicates whether the group names should be serialized.
    /// </summary>
    /// <returns>
    /// true if the group names should be serialized; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeGroupNames() => GroupNames.Count > 0;

    /// <summary>
    /// Returns whether serialization processes should serialize the effective value of the 
    /// <see cref="SortDescriptions"/> property on instances of this class.
    /// </summary>
    /// <returns>
    /// true if the <see cref="SortDescriptions"/> property value should be serialized; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeSortDescriptions() => _sort is not null && _sort.Count > 0;

    /// <summary>
    /// Returns the group name or names for the specified item.
    /// </summary>
    /// <param name="item">
    /// The item to return the group name for.
    /// </param>
    /// <param name="level">
    /// The level of the group within the grouping hierarchy.
    /// </param>
    /// <param name="culture">
    /// The culture information that affects grouping.
    /// </param>
    /// <returns>
    /// An object that represents the group name or names.
    /// </returns>
    public abstract object GroupNameFromItem(object item, int level, CultureInfo culture);

    /// <summary>
    /// Indicates whether the specified item belongs in the specified group.
    /// </summary>
    /// <param name="groupName">
    /// The name of the group to check.
    /// </param>
    /// <param name="itemName">
    /// The name of the item to check.
    /// </param>
    /// <returns>
    /// true if the item belongs in the group; otherwise, false.
    /// </returns>
    public virtual bool NamesMatch(object groupName, object itemName) => Equals(groupName, itemName);

    /// <summary>
    /// Collection of Sort criteria to sort the groups.  Does not do lazy initialization.
    /// </summary>
    internal SortDescriptionCollection SortDescriptionsInternal => _sort;

    private void OnGroupNamesChanged(object sender, NotifyCollectionChangedEventArgs e)
        => OnPropertyChanged(new PropertyChangedEventArgs(nameof(GroupNames)));

    // set new SortDescription collection; rehook collection change notification handler
    private void SetSortDescriptions(SortDescriptionCollection descriptions)
    {
        if (_sort is not null)
        {
            ((INotifyCollectionChanged)_sort).CollectionChanged -= new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
        }

        bool raiseChangeEvent = _sort != descriptions;

        _sort = descriptions;

        if (_sort is not null)
        {
            Debug.Assert(_sort.Count == 0, "must be empty SortDescription collection");
            ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
        }

        if (raiseChangeEvent)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SortDescriptions)));
        }
    }

    // SortDescription was added/removed, notify listeners
    private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // adding to SortDescriptions overrides custom sort
        if (_sort.Count > 0)
        {
            if (_customSort is not null)
            {
                _customSort = null;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CustomSort)));
            }
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(SortDescriptions)));
    }
}
