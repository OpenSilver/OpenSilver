
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Xaml;
using OpenSilver.Internal.Xaml;

namespace System.Windows.Markup
{
    /// <summary>
    /// Class used to access elements inside the XAML code
    /// </summary>
    [ContentProperty(nameof(ResourceKey))]
    public class StaticResourceExtension : MarkupExtension
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
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (TryFindTheResource(serviceProvider, out object resource))
            {
                return resource;
            }

            object value = FindResourceInAppOrSystem();
            if (value == null)
            {
                throw new XamlParseException($"StaticResource resolve failed: cannot find resource named '{ResourceKey}' (Note: resource names are case sensitive)");
            }
            return value;
        }

        private bool TryFindTheResource(IServiceProvider serviceProvider, out object resource)
        {
            if (serviceProvider.GetService(typeof(IAmbientResourcesProvider)) is IAmbientResourcesProvider ambientResourcesProvider)
            {
                return TryFindResourceFromCompiler(ambientResourcesProvider, out resource);
            }
            else if (serviceProvider.GetService(typeof(IAmbientProvider)) is IAmbientProvider ambientProvider)
            {
                if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider schemaContextProvider)
                {
                    throw new InvalidOperationException(
                        string.Format("Markup extension '{0}' requires '{1}' be implemented in the IServiceProvider for ProvideValue.",
                            GetType().Name,
                            nameof(IXamlSchemaContextProvider)));
                }

                return TryFindResourceFromParser(ambientProvider, schemaContextProvider, out resource);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Markup extension '{0}' requires '{1}' or '{2}' be implemented in the IServiceProvider for ProvideValue.",
                        GetType().Name,
                        nameof(IAmbientResourcesProvider),
                        nameof(IAmbientProvider)));
            }
        }

        private bool TryFindResourceFromCompiler(IAmbientResourcesProvider ambientProvider, out object resource)
        {
            Debug.Assert(ambientProvider != null);

            IEnumerable<object> ambientValues = ambientProvider.GetAllAmbientValues();
            foreach (object ambientValue in ambientValues)
            {
                if (ambientValue is ResourceDictionary rd)
                {
                    if (rd.TryGetResource(ResourceKey, out resource))
                    {
                        return true;
                    }
                }
            }

            resource = null;
            return false;
        }

        private bool TryFindResourceFromParser(IAmbientProvider ambientProvider, IXamlSchemaContextProvider schemaContextProvider, out object resource)
        {
            Debug.Assert(ambientProvider != null);
            Debug.Assert(schemaContextProvider != null);

            XamlSchemaContext schemaContext = schemaContextProvider.SchemaContext;

            XamlType feXType = schemaContext.GetXamlType(typeof(FrameworkElement));
            XamlType appXType = schemaContext.GetXamlType(typeof(Application));

            XamlMember feResourcesProperty = feXType.GetMember("Resources");
            XamlMember appResourcesProperty = appXType.GetMember("Resources");

            XamlType[] types = new XamlType[1] { schemaContext.GetXamlType(typeof(ResourceDictionary)) };

            var ambientValues = ambientProvider.GetAllAmbientValues(null,
                                                                    false,
                                                                    types,
                                                                    feResourcesProperty,
                                                                    appResourcesProperty);

            foreach (var ambientValue in ambientValues)
            {
                if (ambientValue.Value is ResourceDictionary rd)
                {
                    if (rd.TryGetResource(ResourceKey, out resource))
                    {
                        return true;
                    }
                }
            }

            resource = null;
            return false;
        }

        private object FindResourceInAppOrSystem()
        {
            Application app = Application.Current;
            if (app != null)
            {
                if (app.HasResources && app.Resources.TryGetResource(ResourceKey, out object resource))
                {
                    return resource;
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
