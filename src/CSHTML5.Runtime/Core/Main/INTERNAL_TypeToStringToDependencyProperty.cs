
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
