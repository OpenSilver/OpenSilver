
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

#if MIGRATION
namespace System.Windows.Ink
#else
namespace Windows.UI.Xaml.Ink
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="Stroke"/> objects.
    /// </summary>
    public sealed class StrokeCollection : PresentationFrameworkCollection<Stroke>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeCollection"/> class.
        /// </summary>
        public StrokeCollection()
            : base(true)
        {
        }

        internal override void AddOverride(Stroke point)
        {
            AddInternal(point);
        }

        internal override void ClearOverride()
        {
            ClearInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            RemoveAtInternal(index);
        }

        internal override void InsertOverride(int index, Stroke point)
        {
            InsertInternal(index, point);
        }

        internal override Stroke GetItemOverride(int index)
        {
            return GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, Stroke point)
        {
            SetItemInternal(index, point);
        }
    }
}
