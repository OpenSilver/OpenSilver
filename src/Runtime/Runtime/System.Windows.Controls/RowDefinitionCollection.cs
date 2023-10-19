
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
    /// <exclude/>
    public sealed class RowDefinitionCollection : PresentationFrameworkCollection<RowDefinition>
    {
        private readonly Grid _parentGrid;

        internal RowDefinitionCollection() : base(true)
        {
        }

        internal RowDefinitionCollection(Grid parent) : base(true)
        {
            this._parentGrid = parent;
            parent.ProvideSelfAsInheritanceContext(this, null);
        }

        internal override void AddOverride(RowDefinition value)
        {
            this.AddDependencyObjectInternal(value);
            value.Parent = this._parentGrid;
        }

        internal override void ClearOverride()
        {
            if (this._parentGrid != null)
            {
                foreach (RowDefinition column in this)
                {
                    column.Parent = null;
                }
            }

            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, RowDefinition value)
        {
            value.Parent = this._parentGrid;
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            RowDefinition removedRow = this.GetItemInternal(index);
            removedRow.Parent = null;
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override RowDefinition GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, RowDefinition value)
        {
            RowDefinition originalItem = this.GetItemInternal(index);
            originalItem.Parent = null;
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
