
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
using System.Runtime.CompilerServices;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_TypeToStringsToDependencyProperties
    {
        private static readonly Dictionary<Type, Dictionary<string, DependencyProperty>> TypeToStringsToDependencyProperties = 
            new Dictionary<Type, Dictionary<string, DependencyProperty>>();

        internal static DependencyProperty GetPropertyInTypeOrItsBaseTypes(Type ownerType, string propertyName)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            DependencyProperty dp = null;

            do
            {
                RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);

                if (!TypeToStringsToDependencyProperties.TryGetValue(ownerType, out var properties)
                    || !properties.TryGetValue(propertyName, out dp))
                {
                    ownerType = ownerType.BaseType;
                }
            }
            while (dp == null && ownerType != null) ;

            return dp;
        }

        internal static void Register(DependencyProperty dp)
        {
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            Dictionary<string, DependencyProperty> properties = GetDictionaryForType(dp.OwnerType);

#if !MIGRATION
            if (properties.ContainsKey(dp.Name))
            {
                // THE FOLLOWING CHECK IS DISABLED IN THE SILVERLIGHT COMPATIBLE VERSION
                // BECAUSE IT APPEARS THAT SILVERLIGHT IS TOLERANT TO DECLARING TWICE
                // THE SAME DEPENDENCY PROPERTY OR ATTACHED PROPERTY.
                throw new ArgumentException($"'{dp.Name}' property was already registered by '{dp.OwnerType.Name}'.");
            }
#endif

            properties[dp.Name] = dp;
        }

        private static Dictionary<string, DependencyProperty> GetDictionaryForType(Type ownerType)
        {
            if (!TypeToStringsToDependencyProperties.TryGetValue(ownerType, out var properties))
            {
                properties = new Dictionary<string, DependencyProperty>();
                TypeToStringsToDependencyProperties.Add(ownerType, properties);
            }

            return properties;
        }
    }
}
