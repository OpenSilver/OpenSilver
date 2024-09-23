
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

using System.ComponentModel;
using System.Collections.ObjectModel;

namespace System.Windows.Data;

/// <summary>
/// Represents a group created by a <see cref="CollectionView"/> object based on the 
/// <see cref="CollectionView.GroupDescriptions"/>.
/// </summary>
public abstract class CollectionViewGroup : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionViewGroup"/> class with 
    /// the specified group name.
    /// </summary>
    /// <param name="name">
    /// The name of the group.
    /// </param>
    protected CollectionViewGroup(object name)
    {
        Name = name;
        ProtectedItems = new ObservableCollection<object>();
        Items = new ReadOnlyObservableCollection<object>(ProtectedItems);
    }

    /// <summary>
    /// Gets the name of this group.
    /// </summary>
    /// <returns>
    /// The name of this group.
    /// </returns>
    public object Name { get; }

    /// <summary>
    /// Gets the items that are immediate children of the group.
    /// </summary>
    /// <returns>
    /// A read-only collection of the immediate items in this group. This is either a
    /// collection of subgroups or a collection of data items if this group does not
    /// have any subgroups.
    /// </returns>
    public ReadOnlyObservableCollection<object> Items { get; }

    /// <summary>
    /// Gets the number of data items in the subtree under this group.
    /// </summary>
    /// <returns>
    /// The number of data items in the subtree under this group.
    /// </returns>
    public int ItemCount { get; private set; }

    /// <summary>
    /// Gets a value that indicates whether this group has any subgroups.
    /// </summary>
    /// <returns>
    /// true if this group is at the bottom level and does not have any subgroups; otherwise, false.
    /// </returns>
    public abstract bool IsBottomLevel { get; }

    /// <summary>
    /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
    /// </summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    protected virtual event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with the provided arguments.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    /// <summary>
    /// Gets the immediate items that are contained in this group.
    /// </summary>
    /// <returns>
    /// The immediate items that are contained in this group.
    /// </returns>
    protected ObservableCollection<object> ProtectedItems { get; }

    /// <summary>
    /// Gets or sets the number of data items in the subtree under this group.
    /// </summary>
    /// <returns>
    /// The number of data items in the subtree under this group.
    /// </returns>
    protected int ProtectedItemCount
    {
        get { return ItemCount; }
        set
        {
            ItemCount = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemCount)));
        }
    }
}
