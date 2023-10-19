
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
    public sealed partial class ColumnDefinitionCollection : PresentationFrameworkCollection<ColumnDefinition>
    {
        private readonly Grid _parentGrid;

        internal ColumnDefinitionCollection() : base(true)
        {
        }

        internal ColumnDefinitionCollection(Grid parent) : base(true)
        {
            this._parentGrid = parent;
            parent.ProvideSelfAsInheritanceContext(this, null);
        }

        internal override void AddOverride(ColumnDefinition value)
        {
            this.AddDependencyObjectInternal(value);
            value.Parent = this._parentGrid;
        }

        internal override void ClearOverride()
        {
            if (this._parentGrid != null)
            {
                foreach (ColumnDefinition column in this)
                {
                    column.Parent = null;
                }
            }

            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, ColumnDefinition value)
        {
            value.Parent = this._parentGrid;
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            ColumnDefinition removedColumn = this.GetItemInternal(index);
            removedColumn.Parent = null;
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override ColumnDefinition GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, ColumnDefinition value)
        {
            ColumnDefinition originalItem = this.GetItemInternal(index);
            originalItem.Parent = null;
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
