﻿
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
    [OpenSilver.NotImplemented]
    public static class TreeViewExtensions
    {
        [OpenSilver.NotImplemented]
        public static bool SelectItem(this TreeView view, object item)
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public static TreeViewItem GetSelectedContainer(this TreeView view)
        {
            return null;
        }
    }
}
