
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

internal sealed class StateTriggerCollection : Collection<StateTriggerBase>
{
    private readonly VisualState _owner;

    public StateTriggerCollection(VisualState owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, StateTriggerBase item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        base.InsertItem(index, item);

        SetParent(item);
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
        if (Count > 0)
        {
            StateTriggerBase[] stateTriggers = [.. this];

            base.ClearItems();

            foreach (StateTriggerBase stateTrigger in stateTriggers)
            {
                ClearParent(stateTrigger);
            }
        }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        StateTriggerBase oldStateTrigger = this[index];

        base.RemoveItem(index);

        ClearParent(oldStateTrigger);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, StateTriggerBase item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        StateTriggerBase oldStateTrigger = this[index];

        base.SetItem(index, item);

        ClearParent(oldStateTrigger);
        SetParent(item);
    }

    private void SetParent(StateTriggerBase stateTrigger)
    {
        stateTrigger.VisualState = _owner;
        _owner.ProvideSelfAsInheritanceContext(stateTrigger, null);
    }

    private void ClearParent(StateTriggerBase stateTrigger)
    {
        stateTrigger.VisualState = null;
        _owner.RemoveSelfAsInheritanceContext(stateTrigger, null);
    }
}
