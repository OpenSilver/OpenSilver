
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
using System.Reflection;

namespace System.Windows
{
    internal static class PropertyPathHelper
    {
        public static object AccessValueByApplyingPropertyPathIfAny(object item, string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                //we need to get through the path and find the property that contains the value.
                string[] splittedPath = path.Split('.');
                object currentItem = item;
                for (int i = 0; i < splittedPath.Length; ++i)
                {
                    if (currentItem == null)
                    {
                        return DependencyProperty.UnsetValue;
                    }

                    Type type = currentItem.GetType();
                    var memberArray = type.GetMember(splittedPath[i]);
                    if (memberArray != null && memberArray.Length > 0)
                    {
                        if (memberArray[0] is FieldInfo)
                        {
                            FieldInfo info = (FieldInfo)memberArray[0];
                            currentItem = info.GetValue(currentItem);
                        }
                        else if (memberArray[0] is PropertyInfo)
                        {
                            PropertyInfo info = (PropertyInfo)memberArray[0];
                            currentItem = info.GetValue(currentItem);
                        }
                        else
                        {
                            return ""; //we are not supposed to go through another type of member
                        }
                    }
                    else
                    {
                        return ""; //the path cannot be followed until the end
                    }
                }
                return currentItem;
            }

            else
            {
                return item;
            }
        }
    }
}
