
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
