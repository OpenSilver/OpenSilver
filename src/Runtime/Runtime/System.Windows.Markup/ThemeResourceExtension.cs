

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
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class used to access elements inside the XAML code
    /// </summary>
    [System.Windows.Markup.ContentProperty("ResourceKey")]
    public partial class ThemeResourceExtension : MarkupExtension
    {
        /// <summary>
        /// The key of the StaticResource.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Initializes a new instance of StaticResource.
        /// </summary>
        public ThemeResourceExtension() { }

        /// <summary>
        /// Initializes a new instance of StaticResource with the given ResourceKey.
        /// </summary>
        /// <param name="resourceKey">The ResourceKey for the StaticResource</param>
        public ThemeResourceExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    serviceProvider.GetService();
        //    var staticResourceExtension = new StaticResourceExtension(ResourceKey);

        //    var resource = staticResourceExtension.ProvideValue(serviceProvider) as INTERNAL_CorrespondingItem;

        //    return resource == null ? "Invalid INTERNAL_CorrespondingItem" : String.Format("My {0} {1}", resource.Value, resource.Title);
        //}



        /// <summary>
        /// returns an object that is provided as the value of the target property for this StaticResource.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the StaticResource.</param>
        /// <returns>An object that is provided as the value of the target property for this StaticResource.</returns>

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            //BRIDGETODO :
            //implemente code below
#if BRIDGE
            if (serviceProvider is ServiceProvider)
            {
                ServiceProvider serviceProviderAsServiceProvider = (ServiceProvider)serviceProvider;
                bool isFirst = true;
                object elementItself = null;
                foreach (Object parentElement in serviceProviderAsServiceProvider.Parents)
                {
                    if (isFirst)
                    {
                        elementItself = parentElement;
                    }
                    else
                    {
                        ResourceDictionary resourceDictionary = null;
                        if (parentElement is ResourceDictionary)
                        {
                            resourceDictionary = (ResourceDictionary)parentElement;
                        }
                        else if (parentElement is FrameworkElement parentFE)
                        {
                            resourceDictionary = parentFE.HasResources ? parentFE.Resources
                                                                       : null;
                        }
                        if (resourceDictionary != null && resourceDictionary.Contains(ResourceKey))
                        {
                            object returnElement = resourceDictionary[ResourceKey];
                            if (returnElement != elementItself)
                            {
                                return returnElement;
                            }
                        }
                    }
                    isFirst = false;
                }
                if (Application.Current.Resources != null && Application.Current.Resources.Contains(ResourceKey))
                {
                    return Application.Current.Resources[ResourceKey];
                }
                throw new XamlParseException(string.Format("StaticResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
            }
            else
            {
                throw new SystemException("StaticResourceExtension.ProvideValue failed: the service provider is not of the expected type. Please contact support.");
            }
#else

            if (serviceProvider is ServiceProvider)
            {
                ServiceProvider serviceProviderAsServiceProvider = (ServiceProvider)serviceProvider;
                bool isFirst = true;
                object elementItself = null;
                foreach (Object parentElement in serviceProviderAsServiceProvider.Parents)
                {
                    if (isFirst)
                    {
                        elementItself = parentElement;
                    }
                    else
                    {
                        ResourceDictionary resourceDictionary = null;
                        if (parentElement is ResourceDictionary)
                        {
                            resourceDictionary = (ResourceDictionary)parentElement;
                        }
                        else if (parentElement is FrameworkElement)
                        {
                            resourceDictionary = ((FrameworkElement)parentElement).Resources;
                        }
                        //todo: add something to get the name of the current Theme (Dark, Light, HighContrast, Default, maybe some others?) and find in ResourceDictionary.ThemeDictionaries the ResourceDictionary that has the current Theme as its key.
                        //      if that Dictionary does not contain the ResourceKey, try in the one that has the "Default" key
                        //      if the ResourceKey was still not found, check the ResourceDictionary.MergedDictionary ?
                        if(resourceDictionary != null && resourceDictionary.ThemeDictionaries.ContainsKey("Default"))
                        {
                            ResourceDictionary themeDictionary = (ResourceDictionary)resourceDictionary.ThemeDictionaries["Default"];
                            if(themeDictionary.Contains(ResourceKey))
                            {
                                object returnElement = themeDictionary[ResourceKey];
                                if (returnElement != elementItself)
                                {
                                    return returnElement;
                                }
                            }
                        }
                        else
                        if(resourceDictionary != null && resourceDictionary.Contains(ResourceKey))
                        {
                            object result = resourceDictionary[ResourceKey];
                            return resourceDictionary[ResourceKey];
                        }
                    }
                    isFirst = false;
                }
                if (Application.Current.Resources != null && Application.Current.Resources.Contains(ResourceKey))
                {
                    return Application.Current.Resources[ResourceKey];
                }
                else
                {
                    object result = Application.Current.TryFindResource(ResourceKey);
                    if(result == null)
                    {
                        throw new XamlParseException(string.Format("ThemeResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
                    }
                    return result;
                }
                throw new XamlParseException(string.Format("ThemeResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
            }
            else
            {
                throw new SystemException("ThemeResourceExtension.ProvideValue failed: the service provider is not of the expected type. Please contact support.");
            }
#endif
        }
    }
}