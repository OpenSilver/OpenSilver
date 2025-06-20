
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

internal sealed class VisualTransitionsCollection : Collection<VisualTransition>
{
    private readonly VisualStateGroup _owner;

    public VisualTransitionsCollection(VisualStateGroup owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, VisualTransition item)
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
            VisualTransition[] transitions = [.. this];

            base.ClearItems();

            foreach (VisualTransition transition in transitions)
            {
                _owner.RemoveSelfAsInheritanceContext(transition, null);
            }
        }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        VisualTransition oldTransition = this[index];

        base.RemoveItem(index);

        _owner.RemoveSelfAsInheritanceContext(oldTransition, null);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, VisualTransition item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        VisualTransition oldTransition = this[index];

        base.SetItem(index, item);

        _owner.RemoveSelfAsInheritanceContext(oldTransition, null);
        _owner.ProvideSelfAsInheritanceContext(item, null);
    }
}