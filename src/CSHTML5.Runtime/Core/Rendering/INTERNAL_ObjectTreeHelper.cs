
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
