
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
using System.Windows;

namespace OpenSilver.Internal;

internal sealed class VisualStateGroupCollection : Collection<VisualStateGroup>
{
    private readonly WeakReference<DependencyObject> _owner;

    public VisualStateGroupCollection(DependencyObject owner)
    {
        Debug.Assert(owner is not null);
        _owner = new(owner);
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
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        base.InsertItem(index, item);

        Owner?.ProvideSelfAsInheritanceContext(item, null);
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
                owner.RemoveSelfAsInheritanceContext(group, null);
            }
        }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        VisualStateGroup oldGroup = this[index];

        base.RemoveItem(index);

        Owner?.RemoveSelfAsInheritanceContext(oldGroup, null);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, VisualStateGroup item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        VisualStateGroup oldGroup = this[index];

        base.SetItem(index, item);

        if (Owner is DependencyObject owner)
        {
            owner.RemoveSelfAsInheritanceContext(oldGroup, null);
            owner.ProvideSelfAsInheritanceContext(item, null);
        }
    }
}