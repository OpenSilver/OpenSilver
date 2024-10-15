
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

/// <summary>
/// A list of bindings, used by MultiBinding classes.
/// </summary>
internal sealed class BindingCollection : Collection<BindingBase>
{
    internal BindingCollection(BindingBase owner, Action callback)
    {
        Debug.Assert(owner is not null && callback is not null);
        _owner = owner;
        _collectionChangedCallback = callback;
    }

    /// <summary>
    /// called by base class Collection&lt;T&gt; when the list is being cleared;
    /// raises a CollectionChanged event to any listeners
    /// </summary>
    protected override void ClearItems()
    {
        _owner.CheckSealed();
        base.ClearItems();
        OnBindingCollectionChanged();
    }

    /// <summary>
    /// called by base class Collection&lt;T&gt; when an item is removed from list;
    /// raises a CollectionChanged event to any listeners
    /// </summary>
    protected override void RemoveItem(int index)
    {
        _owner.CheckSealed();
        base.RemoveItem(index);
        OnBindingCollectionChanged();
    }

    /// <summary>
    /// called by base class Collection&lt;T&gt; when an item is added to list;
    /// raises a CollectionChanged event to any listeners
    /// </summary>
    protected override void InsertItem(int index, BindingBase item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidateItem(item);
        _owner.CheckSealed();

        base.InsertItem(index, item);
        OnBindingCollectionChanged();
    }

    /// <summary>
    /// called by base class Collection&lt;T&gt; when an item is added to list;
    /// raises a CollectionChanged event to any listeners
    /// </summary>
    protected override void SetItem(int index, BindingBase item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidateItem(item);
        _owner.CheckSealed();

        base.SetItem(index, item);
        OnBindingCollectionChanged();
    }

    private void ValidateItem(BindingBase binding)
    {
        // for V1, we only allow Binding as an item of BindingCollection.
        if (binding is not Binding)
        {
            throw new NotSupportedException(string.Format(Strings.BindingCollectionContainsNonBinding, binding.GetType().Name));
        }
    }

    private void OnBindingCollectionChanged() => _collectionChangedCallback?.Invoke();

    private readonly BindingBase _owner;
    private readonly Action _collectionChangedCallback;
}
