

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
    public partial class TreeViewDragDropTarget : ItemsControlDragDropTarget<ItemsControl, TreeViewItem>
    {
        [OpenSilver.NotImplemented]
        protected override void AddItem(ItemsControl control, object data)
        {
        }

        [OpenSilver.NotImplemented]
        protected override UIElement ContainerFromIndex(ItemsControl itemsControl, int index)
        {
            return default(UIElement);
        }

        [OpenSilver.NotImplemented]
        protected override int? IndexFromContainer(ItemsControl itemsControl, UIElement itemContainer)
        {
            return default(int?);
        }

        [OpenSilver.NotImplemented]
        protected override void InsertItem(ItemsControl itemsControl, int index, object data)
        {
        }

        [OpenSilver.NotImplemented]
        protected override ItemsControl INTERNAL_ReturnNewTItemsControl()
        {
            return default(ItemsControl);
        }

        [OpenSilver.NotImplemented]
        protected override void RemoveItem(ItemsControl itemsControl, object data)
        {
        }

        [OpenSilver.NotImplemented]
        protected override void RemoveItemAtIndex(ItemsControl itemsControl, int index)
        {
        }

        [OpenSilver.NotImplemented]
        internal override int INTERNAL_GetNumberOfElementsBetweenItemsRootAndDragDropTarget()
        {
            return default(int);
        }
    }
}
