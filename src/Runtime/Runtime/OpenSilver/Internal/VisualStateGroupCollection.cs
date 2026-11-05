
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
using System.Windows;

namespace OpenSilver.Internal;

internal sealed class VisualStateGroupCollection : Collection<VisualStateGroup>
{
    private readonly WeakReference<DependencyObject> _owner;
    private bool _isDefaultValue;

    internal VisualStateGroupCollection(DependencyObject owner)
    {
        _owner = new(owner);
        _isDefaultValue = true;
    }

    private DependencyObject Owner
    {
        get
        {
            _owner.TryGetTarget(out DependencyObject owner);
            return owner;
        }
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, VisualStateGroup item)
    {
        ArgumentNullException.ThrowIfNull(item);

        base.InsertItem(index, item);

        DependencyObject owner = Owner;

        SetParent(item, owner);

        if (_isDefaultValue)
        {
            _isDefaultValue = false;
            owner?.SetValueInternal(VisualStateManager.VisualStateGroupsPropertyKey, this);
        }
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
        if (Count > 0)
        {
            DependencyObject owner = Owner;

            VisualStateGroup[] groups = owner is not null ? [.. this] : [];

            base.ClearItems();

            foreach (VisualStateGroup group in groups)
            {
                ClearParent(group, owner);
            }
        }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        VisualStateGroup oldGroup = this[index];

        base.RemoveItem(index);

        ClearParent(oldGroup, Owner);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, VisualStateGroup item)
    {
        ArgumentNullException.ThrowIfNull(item);

        VisualStateGroup oldGroup = this[index];

        base.SetItem(index, item);

        if (Owner is DependencyObject owner)
        {
            ClearParent(oldGroup, owner);
            SetParent(item, owner);
        }
    }

    private void SetParent(VisualStateGroup group, DependencyObject owner)
    {
        group.SetVisualElement(_owner);
        owner?.ProvideSelfAsInheritanceContext(group, null);
    }

    private void ClearParent(VisualStateGroup group, DependencyObject owner)
    {
        group.SetVisualElement(null);
        owner?.RemoveSelfAsInheritanceContext(group, null);
    }
}