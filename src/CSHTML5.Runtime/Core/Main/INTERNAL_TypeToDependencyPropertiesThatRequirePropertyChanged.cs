
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
    internal static class INTERNAL_TypeToDependencyPropertiesThatRequirePropertyChanged
    {
        //---------------------------------------------------------------------------
        // When a UI element is added to the Visual Tree, we call "PropertyChanged"
        // on all its set properties so that the control can refresh itself.
        // However, when a property is not set, we don't call PropertyChanged.
        // Unless the property is in the list here (below).
        // To be listed here, just set "CallPropertyChangedWhenLoadedIntoVisualTree"
        // to "Always" in the "TypeMetadata" of the DependencyProperty.
        //---------------------------------------------------------------------------
        public static Dictionary<Type, List<DependencyProperty>> TypeToDependencyPropertiesThatRequirePropertyChanged = new Dictionary<Type, List<DependencyProperty>>(); //todo: use a HashSet instead of a List?

        public static void Add(Type ownerType, DependencyProperty dependencyProperty)
        {
            GetCollectionForType(ownerType).Add(dependencyProperty);
        }

        static List<DependencyProperty> GetCollectionForType(Type ownerType)
        {
            List<DependencyProperty> dependencyProperties;
            if (TypeToDependencyPropertiesThatRequirePropertyChanged.ContainsKey(ownerType))
            {
                dependencyProperties = TypeToDependencyPropertiesThatRequirePropertyChanged[ownerType];
                if (dependencyProperties == null)
                {
                    dependencyProperties = new List<DependencyProperty>();
                    TypeToDependencyPropertiesThatRequirePropertyChanged[ownerType] = dependencyProperties;
                }
            }
            else
            {
                dependencyProperties = new List<DependencyProperty>();
                TypeToDependencyPropertiesThatRequirePropertyChanged.Add(ownerType, dependencyProperties);
            }
            return dependencyProperties;
        }
    }
}
