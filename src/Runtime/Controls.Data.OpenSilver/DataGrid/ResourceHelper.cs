// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;


#if MIGRATION
using System.Windows.Controls;
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal sealed class ResourceHelper
    {
        static Dictionary<string, string> _cache = new Dictionary<string, string>();

        private ResourceHelper()
        {
        }

        static public ControlTemplate GetControlTemplate<T>()
        {
            return GetControlTemplate(typeof(T));
        }

        static public ControlTemplate GetControlTemplate(Type type)
        {
            return GetControlTemplate(type, type.FullName);
        }

        static public ControlTemplate GetControlTemplate(Type type, string resourceName)
        {
            string xaml = ResourceHelper.GetTemplateXaml(type, resourceName);

            if (String.IsNullOrEmpty(xaml))
            {
                throw new Exception(type.Name + " xaml could not be loaded");
            }
            else
            {
#if DEBUG
                try
                {
#endif
                    ControlTemplate result = (ControlTemplate)XamlReader.Load(xaml);

                    return result;
#if DEBUG
                }
                catch 
                {
                    throw;
                }
#endif
            }
        }

        static public string GetTemplateXaml(Type type, string resourceName)
        {
            string template;

            if (!_cache.TryGetValue(resourceName, out template))
            {
                System.IO.Stream s = type.Assembly.GetManifestResourceStream(resourceName + ".xaml");
                if (s != null)
                {
                    template = new System.IO.StreamReader(s).ReadToEnd();
                    _cache[resourceName] = template;
                }
            }

            return template;
        }
    }
}
