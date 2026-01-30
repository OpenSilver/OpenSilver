
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

using System.ComponentModel;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Provides access to an ordered, strongly typed collection of <see cref="RowDefinition"/> objects.
/// </summary>
[TypeConverter(typeof(RowDefinitionCollectionConverter))]
public sealed class RowDefinitionCollection : PresentationFrameworkCollection<RowDefinition>
{
    private Grid _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowDefinitionCollection"/> class.
    /// </summary>
    public RowDefinitionCollection() { }

    internal RowDefinitionCollection(Grid owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
        PrivateOnModified();
    }

    internal Grid Owner
    {
        get => _owner;
        set
        {
            if (_owner == value)
            {
                return;
            }

            if (_owner is null)
            {
                if (value.RowDefinitions.Count > 0)
                {
                    throw new ArgumentException(
                        string.Format(Strings.GridCollection_InOtherCollection, nameof(Grid), nameof(RowDefinitionCollection)));
                }

                _owner = value;
                PrivateOnModified();
                for (int i = 0; i < InternalItems.Count; i++)
                {
                    DefinitionBase item = InternalItems[i];
                    SetParent(item);
                    item.OnEnterParentTree();
                }
            }
            else if (value is null)
            {
                PrivateOnModified();
                for (int i = 0; i < InternalItems.Count; i++)
                {
                    DefinitionBase item = InternalItems[i];
                    item.OnExitParentTree();
                    ClearParent(item);
                }
                _owner = null;
            }
            else
            {
                throw new ArgumentException(
                    string.Format(Strings.GridCollection_InOtherCollection, nameof(RowDefinitionCollection), nameof(Grid)));
            }
        }
    }

    internal override bool IsReadOnlyImpl => AreDefinitionsLocked();

    internal override void AddOverride(RowDefinition value)
    {
        VerifyWriteAccess();

        PrivateConnectChild(InternalCount, value);
        AddInternal(value);

        PrivateOnModified();
    }

    internal override void ClearOverride()
    {
        VerifyWriteAccess();

        foreach (RowDefinition row in InternalItems)
        {
            PrivateDisconnectChild(row);
        }

        ClearInternal();

        PrivateOnModified();
    }

    internal override void InsertOverride(int index, RowDefinition value)
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

    internal override RowDefinition GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, RowDefinition value)
    {
        VerifyWriteAccess();

        PrivateDisconnectChild(GetItemInternal(index));
        PrivateConnectChild(index, value);
        SetItemInternal(index, value);

        PrivateOnModified();
    }

    internal override int IndexOfImpl(RowDefinition value)
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
    ///     Updates version of the RowDefinitionCollection.
    ///     Notifies owner grid about the change.
    /// </summary>
    private void PrivateOnModified()
    {
        if (_owner is Grid owner)
        {
            owner.RowDefinitionCollectionDirty = true;
            owner.Invalidate();
        }
    }

    private void SetParent(DefinitionBase value)
    {
        value.Parent = _owner;
        _owner?.ProvideSelfAsInheritanceContext(value, null);
    }

    private void ClearParent(DefinitionBase value)
    {
        value.Parent = null;
        _owner?.RemoveSelfAsInheritanceContext(value, null);
    }

    private void VerifyWriteAccess()
    {
        if (AreDefinitionsLocked())
        {
            throw new InvalidOperationException(string.Format(Strings.GridCollection_CannotModifyReadOnly, nameof(RowDefinitionCollection)));
        }
    }

    private bool AreDefinitionsLocked() => _owner is Grid owner && (owner.MeasureOverrideInProgress || owner.ArrangeOverrideInProgress);
}
