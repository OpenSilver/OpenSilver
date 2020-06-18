

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
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_ObjectTreeHelper
    {
        public static List<DependencyObject> GetObjectTreeChildren(DependencyObject dependencyObject)
        {
            bool isFound = false;
            bool isSingleChild = false;
            UIElement singleChild = null;
            List<DependencyObject> multipleChildren = new List<DependencyObject>();
            if (dependencyObject is Border)
            {
                singleChild = ((Border)dependencyObject).Child;
                isSingleChild = true;
                isFound = true;
            }
            if (dependencyObject is Canvas)
            {
                foreach (var child in ((Canvas)dependencyObject).Children)
                    multipleChildren.Add(child);
                isSingleChild = false;
                isFound = true;
            }
            if (dependencyObject is UserControl)
            {
                singleChild = ((UserControl)dependencyObject).Content;
                isSingleChild = true;
                isFound = true;
            }
            if (dependencyObject is Window)
            {
                singleChild = (UIElement)((Window)dependencyObject).Content;
                isSingleChild = true;
                isFound = true;
            }
            if (isFound)
            {
                if (isSingleChild)
                {
                    if (singleChild != null)
                        return new List<DependencyObject>() { singleChild };
                    else
                        return new List<DependencyObject>() { };
                }
                else
                    return multipleChildren;
            }

            throw new Exception("Could not determine the children of: " + dependencyObject.GetType().ToString());
        }
    }
}
