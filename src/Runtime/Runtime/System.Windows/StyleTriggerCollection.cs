
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

using System.Collections.ObjectModel;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a collection of <see cref="TriggerBase"/> objects used by a <see cref="Style"/>.
/// </summary>
public sealed class StyleTriggerCollection : Collection<TriggerBase>
{
    private bool _sealed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleTriggerCollection"/> class.
    /// </summary>
    public StyleTriggerCollection() { }

    /// <summary>
    /// Gets a value that indicates whether this collection is in an immutable state.
    /// </summary>
    /// <returns>
    /// true if this collection is in an immutable state; otherwise, false.
    /// </returns>
    public bool IsSealed => _sealed;

    /// <inheritdoc/>
    protected override void ClearItems()
    {
        CheckSealed();
        base.ClearItems();
    }

    /// <inheritdoc/>
    protected override void InsertItem(int index, TriggerBase item)
    {
        CheckSealed();
        ValidateTrigger(item);
        base.InsertItem(index, item);
    }

    /// <inheritdoc/>
    protected override void RemoveItem(int index)
    {
        CheckSealed();
        base.RemoveItem(index);
    }

    /// <inheritdoc/>
    protected override void SetItem(int index, TriggerBase item)
    {
        CheckSealed();
        ValidateTrigger(item);
        base.SetItem(index, item);
    }

    internal void Seal()
    {
        _sealed = true;

        // Seal all the triggers
        foreach (TriggerBase trigger in this)
        {
            trigger.Seal();
        }
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(StyleTriggerCollection)));
        }
    }

    private static void ValidateTrigger(TriggerBase item)
    {
        ArgumentNullException.ThrowIfNull(item);
    }
}
