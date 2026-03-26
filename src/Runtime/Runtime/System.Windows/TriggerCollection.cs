
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

using OpenSilver.Internal;
using System.Diagnostics;

namespace System.Windows;

/// <summary>
/// Represents a collection of <see cref="TriggerBase"/> objects.
/// </summary>
public sealed class TriggerCollection : PresentationFrameworkCollection<TriggerBase>
{
    private readonly IInternalFrameworkElement _owner;
    private bool _sealed;

    internal TriggerCollection() { }

    internal TriggerCollection(IInternalFrameworkElement owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    /// <summary>
    /// Gets a value that indicates whether this collection is read-only and cannot be changed.
    /// </summary>
    /// <returns>
    /// true if this collection is read-only; otherwise, false.
    /// </returns>
    public new bool IsSealed => _sealed;

    private bool IsInitialized => _owner is not null && _owner.IsInitialized;

    internal override void AddOverride(TriggerBase value)
    {
        CheckSealed();

        AddDependencyObjectInternal(value);
        
        if (IsInitialized)
        {
            EventTrigger.ProcessOneTrigger(_owner, value);
        }
    }

    internal override void ClearOverride()
    {
        CheckSealed();

        if (IsInitialized)
        {
            EventTrigger.DisconnectAllTriggers(_owner);
        }
        
        ClearDependencyObjectInternal();
    }

    internal override void InsertOverride(int index, TriggerBase value)
    {
        CheckSealed();

        InsertDependencyObjectInternal(index, value);

        if (IsInitialized)
        {
            EventTrigger.ProcessOneTrigger(_owner, value);
        }
    }

    internal override void RemoveAtOverride(int index)
    {
        CheckSealed();

        TriggerBase trigger = GetItemInternal(index);
        RemoveAtDependencyObjectInternal(index);

        if (IsInitialized)
        {
            EventTrigger.DisconnectOneTrigger(_owner, trigger);
        }
    }

    internal override TriggerBase GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, TriggerBase value)
    {
        CheckSealed();

        TriggerBase oldTrigger = GetItemInternal(index);
        SetItemDependencyObjectInternal(index, value);
        
        if (IsInitialized)
        {
            EventTrigger.DisconnectOneTrigger(_owner, oldTrigger);
            EventTrigger.ProcessOneTrigger(_owner, value);
        }
    }

    internal new void Seal()
    {
        Debug.Assert(_owner is null);

        _sealed = true;

        // Seal all the setters
        foreach (TriggerBase trigger in InternalItems)
        {
            trigger.Seal();
        }
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(TriggerCollection)));
        }
    }
}
