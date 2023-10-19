
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

namespace System.Windows.Input
{
    [OpenSilver.NotImplemented]
    public sealed partial class TouchPointCollection : PresentationFrameworkCollection<TouchPoint>
    {
        internal TouchPointCollection() : base(false)
        {
        }

        internal override void AddOverride(TouchPoint value)
        {
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
        }

        internal override void InsertOverride(int index, TouchPoint value)
        {
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
        }

        internal override TouchPoint GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, TouchPoint value)
        {
            this.SetItemInternal(index, value);
        }
    }
}
