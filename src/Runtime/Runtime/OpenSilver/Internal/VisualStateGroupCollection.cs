
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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
    internal class VisualStateGroupCollection : PresentationFrameworkCollection<VisualStateGroup>
    {
        public VisualStateGroupCollection(FrameworkElement owner)
            : base(false)
        {
            Debug.Assert(owner != null);
            owner.ProvideSelfAsInheritanceContext(this, null);
        }

        internal override void AddOverride(VisualStateGroup value)
            => AddDependencyObjectInternal(value);

        internal override void ClearOverride()
            => ClearDependencyObjectInternal();

        internal override VisualStateGroup GetItemOverride(int index)
            => GetItemInternal(index);

        internal override void InsertOverride(int index, VisualStateGroup value)
            => InsertDependencyObjectInternal(index, value);

        internal override void RemoveAtOverride(int index)
            => RemoveAtDependencyObjectInternal(index);

        internal override void SetItemOverride(int index, VisualStateGroup value)
            => SetItemDependencyObjectInternal(index, value);
    }
}