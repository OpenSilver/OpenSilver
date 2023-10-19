
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

namespace System.Windows
{
	[OpenSilver.NotImplemented]
    public sealed partial class AssemblyPartCollection : PresentationFrameworkCollection<AssemblyPart>
    {
        public AssemblyPartCollection() : base(false)
        {
        }

        internal override void AddOverride(AssemblyPart value)
        {
            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, AssemblyPart value)
        {
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override AssemblyPart GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, AssemblyPart value)
        {
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
