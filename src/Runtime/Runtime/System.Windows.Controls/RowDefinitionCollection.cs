
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
    /// <summary>
    /// Provides access to an ordered, strongly typed collection of <see cref="RowDefinition"/> objects.
    /// </summary>
    public sealed class RowDefinitionCollection : PresentationFrameworkCollection<RowDefinition>
    {
        private readonly Grid _grid;

        internal RowDefinitionCollection(Grid parent)
        {
            _grid = parent;
            parent.ProvideSelfAsInheritanceContext(this, null);
        }

        internal override bool IsReadOnlyImpl => AreDefinitionsLocked();

        internal override void AddOverride(RowDefinition value)
        {
            VerifyWriteAccess();

            AddDependencyObjectInternal(value);
            value.SetParent(_grid);

            _grid.InvalidateDefinitions();
        }

        internal override void ClearOverride()
        {
            VerifyWriteAccess();

            if (_grid != null)
            {
                foreach (RowDefinition column in InternalItems)
                {
                    column.SetParent(null);
                }
            }

            ClearDependencyObjectInternal();

            _grid.InvalidateDefinitions();
        }

        internal override void InsertOverride(int index, RowDefinition value)
        {
            VerifyWriteAccess();

            value.SetParent(_grid);
            InsertDependencyObjectInternal(index, value);

            _grid.InvalidateDefinitions();
        }

        internal override void RemoveAtOverride(int index)
        {
            VerifyWriteAccess();

            RowDefinition removedRow = GetItemInternal(index);
            removedRow.SetParent(null);
            RemoveAtDependencyObjectInternal(index);

            _grid.InvalidateDefinitions();
        }

        internal override RowDefinition GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, RowDefinition value)
        {
            VerifyWriteAccess();

            RowDefinition originalItem = GetItemInternal(index);
            originalItem.SetParent(null);
            SetItemDependencyObjectInternal(index, value);

            _grid.InvalidateDefinitions();
        }

        private void VerifyWriteAccess()
        {
            if (AreDefinitionsLocked())
            {
                throw new InvalidOperationException("Cannot modify 'RowDefinitionCollection' in read-only state.");
            }
        }

        private bool AreDefinitionsLocked() =>
            _grid is not null &&
            (_grid.MeasureOverrideInProgress || _grid.ArrangeOverrideInProgress);
    }
}
