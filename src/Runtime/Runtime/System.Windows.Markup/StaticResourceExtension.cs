

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class used to access elements inside the XAML code
    /// </summary>
    [System.Windows.Markup.ContentProperty("ResourceKey")]
    public partial class StaticResourceExtension : MarkupExtension
    {
        /// <summary>
        /// The key of the StaticResource.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Initializes a new instance of StaticResource.
        /// </summary>
        public StaticResourceExtension() { }

        /// <summary>
        /// Initializes a new instance of StaticResource with the given ResourceKey.
        /// </summary>
        /// <param name="resourceKey">The ResourceKey for the StaticResource</param>
        public StaticResourceExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// returns an object that is provided as the value of the target property for this StaticResource.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the StaticResource.</param>
        /// <returns>An object that is provided as the value of the target property for this StaticResource.</returns>
#if BRIDGE
        public override object ProvideValue(ServiceProvider serviceProvider)
#else
        public override object ProvideValue(IServiceProvider serviceProvider)
#endif
        {
            ServiceProvider serviceProviderAsServiceProvider = (ServiceProvider)serviceProvider;
            Type targetType;
            if (serviceProviderAsServiceProvider.TargetProperty as DependencyProperty != null)
            {
                targetType = ((DependencyProperty)serviceProviderAsServiceProvider.TargetProperty).PropertyType;
            }
            else
            {
                targetType = null;
            }
            object elementItself = null;
            foreach (object parentElement in serviceProviderAsServiceProvider.Parents)
            {
                ResourceDictionary resourceDictionary = null;
                if ((resourceDictionary = parentElement as ResourceDictionary) == null)
                {
                    if (parentElement is FrameworkElement parentFE)
                    {
                        resourceDictionary = parentFE.HasResources ? parentFE.Resources
                                                                   : null;
                    }
                }
                if (resourceDictionary != null && resourceDictionary.Contains(ResourceKey))
                {
                    object returnElement = resourceDictionary[ResourceKey];
                    if (!object.Equals(returnElement, elementItself))
                    {
                        return this.EnsurePropertyType(returnElement, targetType);
                    }
                }
            }
            if (Application.Current.Resources.Contains(ResourceKey))
            {
                return this.EnsurePropertyType(Application.Current.Resources[ResourceKey], targetType);
            }
            else
            {
                // Look in the built-in resources (eg. "SystemAccentColor"):
                object result = Application.Current.TryFindResource(ResourceKey);
                if (result == null)
                {
                    throw new XamlParseException(string.Format("StaticResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
                }
                return this.EnsurePropertyType(result, targetType);
            }
            throw new XamlParseException(string.Format("StaticResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
        }

        private object EnsurePropertyType(object item, Type targetType)
        {
            if (targetType == null || item == null)
            {
                return item;
            }

            Type itemType = item.GetType();
            if (targetType.IsAssignableFrom(itemType))
            {
                return item;
            }
            else if (itemType == typeof(string) && TypeFromStringConverters.CanTypeBeConverted(targetType))
            {
                return TypeFromStringConverters.ConvertFromInvariantString(targetType, (string)item);
            }
            else
            {
                //note: can crash
                return Convert.ChangeType(item, targetType);
            }
        }
    }
}
