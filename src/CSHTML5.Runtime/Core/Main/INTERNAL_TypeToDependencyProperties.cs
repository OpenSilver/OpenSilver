

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
    internal static class INTERNAL_TypeToDependencyProperties
    {
        public static Dictionary<Type, List<DependencyProperty>> TypeToDependencyProperties = new Dictionary<Type, List<DependencyProperty>>(); //todo: use a HashSet instead of a List?

        public static void Add(Type ownerType, DependencyProperty dependencyProperty)
        {
            GetCollectionForType(ownerType).Add(dependencyProperty);
        }

        static List<DependencyProperty> GetCollectionForType(Type ownerType)
        {
            List<DependencyProperty> dependencyProperties;
            if (TypeToDependencyProperties.ContainsKey(ownerType))
            {
                dependencyProperties = TypeToDependencyProperties[ownerType];
                if (dependencyProperties == null)
                {
                    dependencyProperties = new List<DependencyProperty>();
                    TypeToDependencyProperties[ownerType] = dependencyProperties;
                }
            }
            else
            {
                dependencyProperties = new List<DependencyProperty>();
                TypeToDependencyProperties.Add(ownerType, dependencyProperties);
            }
            return dependencyProperties;
        }
    }
}
