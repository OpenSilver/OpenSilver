
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
using OpenSilver.Internal.Xaml;

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class used to access elements inside the XAML code
    /// </summary>
    [ContentProperty("ResourceKey")]
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
            ResourceDictionary dictionaryWithKey = FindTheResourceDictionary(serviceProvider);
            if (dictionaryWithKey != null)
            {
                return dictionaryWithKey[ResourceKey];
            }

            object value = FindResourceInAppOrSystem();
            if (value == null)
            {
                throw new XamlParseException($"StaticResource resolve failed: cannot find resource named '{ResourceKey}' (Note: resource names are case sensitive)");
            }
            return value;
        }

        private ResourceDictionary FindTheResourceDictionary(IServiceProvider serviceProvider)
        {
            IAmbientResourcesProvider ambientProvider = serviceProvider.GetService(typeof(IAmbientResourcesProvider)) as IAmbientResourcesProvider;
            if (ambientProvider == null)
            {
                throw new InvalidOperationException(
                    string.Format("Markup extension '{0}' requires '{1}' be implemented in the IServiceProvider for ProvideValue.",
                        GetType().Name,
                        nameof(IAmbientResourcesProvider)));
            }

            IEnumerable<object> ambientValues = ambientProvider.GetAllAmbientValues();
            foreach (object ambientValue in ambientValues)
            {
                if (ambientValue is ResourceDictionary rd)
                {
                    if (rd.Contains(ResourceKey))
                    {
                        return rd;
                    }
                }
            }

            return null;
        }

        private object FindResourceInAppOrSystem()
        {
            Application app = Application.Current;
            if (app != null)
            {
                if (app.HasResources && app.Resources.Contains(ResourceKey))
                {
                    return app.Resources[ResourceKey];
                }
                else
                {
                    // Look in the built-in resources (eg. "SystemAccentColor")
                    return app.TryFindResource(ResourceKey);
                }
            }

            return null;
        }
    }
}
