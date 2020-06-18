

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
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_TypeToStringsToDependencyProperties
    {
        public static Dictionary<Type, Dictionary<string, DependencyProperty>> TypeToStringsToDependencyProperties = new Dictionary<Type, Dictionary<string, DependencyProperty>>();

        internal static DependencyProperty GetPropertyInTypeOrItsBaseTypes(Type type, string propertyName)
        {
            Type currentType = type;
            while(currentType != null &&
                (!INTERNAL_TypeToStringsToDependencyProperties.TypeToStringsToDependencyProperties.ContainsKey(currentType)
                || (INTERNAL_TypeToStringsToDependencyProperties.TypeToStringsToDependencyProperties[currentType] == null
                    || !INTERNAL_TypeToStringsToDependencyProperties.TypeToStringsToDependencyProperties[currentType].ContainsKey(propertyName))))
            {
                currentType = currentType.BaseType;
            }
            if (currentType != null)
            {
                return INTERNAL_TypeToStringsToDependencyProperties.TypeToStringsToDependencyProperties[currentType][propertyName];
            }
            return null;
        }

        public static Dictionary<string, DependencyProperty> GetDictionaryForType(Type ownerType)
        {
            Dictionary<string, DependencyProperty> stringsToDependencyProperties;
            if (TypeToStringsToDependencyProperties.ContainsKey(ownerType))
            {
                stringsToDependencyProperties = TypeToStringsToDependencyProperties[ownerType];
                if (stringsToDependencyProperties == null)
                {
                    stringsToDependencyProperties = new Dictionary<string, DependencyProperty>();
                    TypeToStringsToDependencyProperties[ownerType] = stringsToDependencyProperties;
                }
            }
            else
            {
                stringsToDependencyProperties = new Dictionary<string, DependencyProperty>();
                TypeToStringsToDependencyProperties.Add(ownerType, stringsToDependencyProperties);
            }
            return stringsToDependencyProperties;
        }
    }
}
