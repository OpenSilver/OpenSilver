
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

namespace System.Windows.Controls
{
    public sealed class ColumnDefinitionCollection : PresentationFrameworkCollection<ColumnDefinition>
    {
        private readonly Grid _parentGrid;

        internal ColumnDefinitionCollection(Grid parent)
            : base(true)
        {
            _parentGrid = parent;
            parent.ProvideSelfAsInheritanceContext(this, null);
        }

        internal override bool IsReadOnlyImpl => AreDefinitionsLocked();

        internal override void AddOverride(ColumnDefinition value)
        {
            VerifyWriteAccess();
            AddDependencyObjectInternal(value);
            value.SetParent(_parentGrid);
        }

        internal override void ClearOverride()
        {
            VerifyWriteAccess();

            if (_parentGrid != null)
            {
                foreach (ColumnDefinition column in InternalItems)
                {
                    column.SetParent(null);
                }
            }

            ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, ColumnDefinition value)
        {
            VerifyWriteAccess();
            value.SetParent(_parentGrid);
            InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            VerifyWriteAccess();
            ColumnDefinition removedColumn = GetItemInternal(index);
            removedColumn.SetParent(null);
            RemoveAtDependencyObjectInternal(index);
        }

        internal override ColumnDefinition GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, ColumnDefinition value)
        {
            VerifyWriteAccess();
            ColumnDefinition originalItem = GetItemInternal(index);
            originalItem.SetParent(null);
            SetItemDependencyObjectInternal(index, value);
        }

        private void VerifyWriteAccess()
        {
            if (AreDefinitionsLocked())
            {
                throw new InvalidOperationException("Cannot modify 'ColumnDefinitionCollection' in read-only state.");
            }
        }

        private bool AreDefinitionsLocked() =>
            _parentGrid is not null &&
            (_parentGrid.MeasureOverrideInProgress || _parentGrid.ArrangeOverrideInProgress);
    }
}
