

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public sealed partial class AssemblyPartCollection : PresentationFrameworkCollection<AssemblyPart>
    {
        internal override void AddOverride(AssemblyPart value)
        {
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
        }

        internal override void InsertOverride(int index, AssemblyPart value)
        {
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
        }

        internal override bool RemoveOverride(AssemblyPart value)
        {
            return this.RemoveInternal(value);
        }

        internal override void SetItemOverride(int index, AssemblyPart value)
        {
            this.SetItemInternal(index, value);
        }
    }
}
#endif