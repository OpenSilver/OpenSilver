
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

namespace System.Windows;

/// <summary>
/// Represents a collection of <see cref="TriggerAction"/> objects.
/// </summary>
public sealed class TriggerActionCollection : PresentationFrameworkCollection<TriggerAction>
{
    private bool _sealed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TriggerActionCollection"/> class.
    /// </summary>
    public TriggerActionCollection() { }

    internal TriggerActionCollection(DependencyObject owner)
    {
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(TriggerAction value)
    {
        CheckSealed();
        AddDependencyObjectInternal(value);
    }

    internal override void ClearOverride()
    {
        CheckSealed();
        ClearDependencyObjectInternal();
    }

    internal override void InsertOverride(int index, TriggerAction value)
    {
        CheckSealed();
        InsertDependencyObjectInternal(index, value);
    }

    internal override void RemoveAtOverride(int index)
    {
        CheckSealed();
        RemoveAtDependencyObjectInternal(index);
    }

    internal override TriggerAction GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, TriggerAction value)
    {
        CheckSealed();
        SetItemDependencyObjectInternal(index, value);
    }

    internal new void Seal()
    {
        _sealed = true;

        foreach (TriggerAction action in InternalItems)
        {
            action.Seal();
        }
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(TriggerActionCollection)));
        }
    }
}
