
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

using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Provides access to an ordered, strongly typed collection of <see cref="ColumnDefinition"/> objects.
/// </summary>
public sealed class ColumnDefinitionCollection : PresentationFrameworkCollection<ColumnDefinition>
{
    private readonly Grid _owner;

    internal ColumnDefinitionCollection(Grid owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
        PrivateOnModified();
    }

    internal override bool IsReadOnlyImpl => AreDefinitionsLocked();

    internal override void AddOverride(ColumnDefinition value)
    {
        VerifyWriteAccess();

        PrivateConnectChild(InternalCount, value);
        AddInternal(value);

        PrivateOnModified();
    }

    internal override void ClearOverride()
    {
        VerifyWriteAccess();

        foreach (ColumnDefinition column in InternalItems)
        {
            PrivateDisconnectChild(column);
        }

        ClearInternal();

        PrivateOnModified();
    }

    internal override void InsertOverride(int index, ColumnDefinition value)
    {
        VerifyWriteAccess();

        for (int i = InternalItems.Count - 1; i >= index; --i)
        {
            Debug.Assert(GetItemInternal(i).Parent == _owner);
            GetItemInternal(i).Index = i + 1;
        }

        PrivateConnectChild(index, value);
        InsertInternal(index, value);

        PrivateOnModified();
    }

    internal override void RemoveAtOverride(int index)
    {
        VerifyWriteAccess();

        PrivateDisconnectChild(GetItemInternal(index));
        for (int i = index + 1; i < InternalItems.Count; ++i)
        {
            Debug.Assert(GetItemInternal(i + 1).Parent == _owner);
            GetItemInternal(i).Index = i;
        }
        RemoveAtInternal(index);

        PrivateOnModified();
    }

    internal override ColumnDefinition GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ColumnDefinition value)
    {
        VerifyWriteAccess();

        PrivateDisconnectChild(GetItemInternal(index));
        PrivateConnectChild(index, value);
        SetItemInternal(index, value);

        PrivateOnModified();
    }

    internal override int IndexOfImpl(ColumnDefinition value)
    {
        if (value is null || value.Parent != _owner)
        {
            return -1;
        }
        else
        {
            return value.Index;
        }
    }

    private void PrivateConnectChild(int index, DefinitionBase value)
    {
        Debug.Assert(value is not null && value.Index == -1);

        // add the value into collection's array
        value.Index = index;

        SetParent(value);
        value.OnEnterParentTree();
    }

    private void PrivateDisconnectChild(DefinitionBase value)
    {
        Debug.Assert(value is not null);

        value.OnExitParentTree();

        // remove the value from collection's array
        value.Index = -1;

        ClearParent(value);
    }

    /// <summary>
    ///     Updates version of the ColumnDefinitionCollection.
    ///     Norifies owner grid about the change.
    /// </summary>
    private void PrivateOnModified()
    {
        _owner.ColumnDefinitionCollectionDirty = true;
        _owner.Invalidate();
    }

    private void SetParent(DefinitionBase value)
    {
        value.Parent = _owner;
        _owner.ProvideSelfAsInheritanceContext(value, null);
    }

    private void ClearParent(DefinitionBase value)
    {
        value.Parent = null;
        _owner.RemoveSelfAsInheritanceContext(value, null);
    }

    private void VerifyWriteAccess()
    {
        if (AreDefinitionsLocked())
        {
            throw new InvalidOperationException(string.Format(Strings.GridCollection_CannotModifyReadOnly, nameof(ColumnDefinitionCollection)));
        }
    }

    private bool AreDefinitionsLocked() => _owner.MeasureOverrideInProgress || _owner.ArrangeOverrideInProgress;
}
