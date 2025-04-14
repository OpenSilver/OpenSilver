
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

internal sealed class VisualStatesCollection : Collection<VisualState>
{
    private readonly VisualStateGroup _owner;

    public VisualStatesCollection(VisualStateGroup owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, VisualState item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        base.InsertItem(index, item);

        _owner.ProvideSelfAsInheritanceContext(item, null);
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
        if (Count > 0)
        {
            VisualState[] states = [.. this];

            base.ClearItems();

            foreach (VisualState state in states)
            {
                _owner.RemoveSelfAsInheritanceContext(state, null);
            }
        }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        VisualState oldState = this[index];

        base.RemoveItem(index);

        _owner.RemoveSelfAsInheritanceContext(oldState, null);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, VisualState item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        VisualState oldState = this[index];

        base.SetItem(index, item);

        _owner.RemoveSelfAsInheritanceContext(oldState, null);
        _owner.ProvideSelfAsInheritanceContext(item, null);
    }
}