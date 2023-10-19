
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
    [OpenSilver.NotImplemented]
    public static class TreeViewConnectingLines
    {
        [OpenSilver.NotImplemented]
        public static TreeViewItem GetIsHeaderOf(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsHeaderOfProperty) as TreeViewItem;
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsHeaderOfProperty =
            DependencyProperty.RegisterAttached(
                "IsHeaderOf",
                typeof(TreeViewItem),
                typeof(TreeViewConnectingLines),
                null);
    }
}
