
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
using System.Reflection;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
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
                        //in the case of JS, members with reserved names(?) (such as "name" without uppercase) do not work properly
                        //in regards to getting the member since the name of the member was changed to "$name" AND the GetValue method does not work either even if we manage to get the $name member.
                        // for that, we try to directly get the value with currentItem["someVariationOfThePropName"];
                        if (!CSHTML5.Interop.IsRunningInTheSimulator)
                        {
                            //Note: there are a lot of tests below because depending on how the property/field was declared, it is not in the same place in JS:
                            //      - in $propName
                            //      - in TypeName$$propName
                            //      - in TypeName$$propName$value
                            // are the possible names observed.
                            object obj = CSHTML5.Interop.ExecuteJavaScript(@"
                            (function(){
    var propname = ""$$"" + $1;
    var temp = $0[propname];
    if(!temp){
        propname = $0.GetType().Name + ""$$"" + propname;
        temp = $0[propname];
        if(!temp){
            propname = propname + ""$$value"";
            temp = $0[propname];
            if(!temp) temp = null;
        }
    }
    return temp;
}())", currentItem, splittedPath[i]);
                            if (obj == null)
                            {
                                return "";
                            }
                            currentItem = obj;
                        }
                        else
                        {
                            return ""; //the path cannot be followed until the end
                        }
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
