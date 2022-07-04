
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


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed partial class ListBoxDragDropTarget : ItemsControlDragDropTarget<ListBox, ListBoxItem>
    {
        /// <summary>
        /// Gets a new instance of the item control.
        /// </summary>
        /// <returns>The item control.</returns>
        protected override ListBox INTERNAL_ReturnNewTItemsControl()
        {
            return new ListBox();
        }
    }
}
