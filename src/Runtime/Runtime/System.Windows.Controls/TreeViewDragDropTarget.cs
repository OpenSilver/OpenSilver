

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
using System.Windows.Controls.Primitives;

#if !MIGRATION
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class TreeViewDragDropTarget : ItemsControlDragDropTarget<ItemsControl, TreeViewItem>
    {
        /// <inheritdoc/>
        protected override ItemsControl INTERNAL_ReturnNewTItemsControl()
        {
            return new TreeViewItem();
        }

        /// <inheritdoc/>
        internal override TreeViewItem INTERNAL_GetDeepestItemContainer(ItemsControl itemsControl,
            List<object> elementsFromDeepestToRoot)
        {
            TreeViewItem deepestItemContainer = null;
            foreach (object element in elementsFromDeepestToRoot)
            {
                if (element is ToggleButton)
                {
                    // Do not start drag & drop when trying to expand a TreeViewItem with the ToggleButton
                    return null;
                }

                if (deepestItemContainer == null && element is TreeViewItem deepestTreeViewItem)
                {
                    deepestItemContainer = deepestTreeViewItem;
                }
            }
            // Not possible to use the base method because items deeper than on the first level
            // fail the test of whether they have an index on the TreeView (similar to a contains test).
            // Instead, these deeper items belong to their TreeViewItem parents, instead to the TreeView.
            // So the contains test is skipped and the deepest item is just returned.
            return deepestItemContainer;
        }
    }
}
