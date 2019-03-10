
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
