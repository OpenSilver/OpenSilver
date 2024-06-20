using System;
using System.Linq;
using System.Windows;

#if OPENSILVER
using OpenSilver.Internal.Xaml;
#endif

namespace TestApplication.Tests.ResourceDictionary.CachedResourceDictionary
{
    public class CachedResourceDictionary : System.Windows.ResourceDictionary
    {
        private Uri _source;
        public new Uri Source
        {
            get
            {
                return _source;
            }
            set
            {
                string source = value.OriginalString;
                if (!source.Contains(";"))
                {
#if OPENSILVER
                    source = "/TestApplication.OpenSilver;component" + source;
#else
                    source = "/TestApplication.Silverlight;component" + source;
#endif
                }
                _source = new Uri(source, UriKind.Relative);
                if (Application.Current.Resources.MergedDictionaries.Any(d => d.Source == _source))
                {
                    return;
                }

#if OPENSILVER
                // OpenSilver does not load the ResourceDictionary content when setting Source
                // (it calls LoadComponent in the compiled XAML code), so it is done manually here.
                string uriString = value.ToString();
                string typeName = System.Text.RegularExpressions.Regex.Replace(uriString,
                    @"\W", // Not word
                    // \u01C0 is an Unicode character that is not exactly pipe
                    System.Text.RegularExpressions.Regex.Unescape(@"\u01C0\u01C0")) +
                    System.Text.RegularExpressions.Regex.Unescape(@"\u01C0\u01C0") + "Factory";

                string assemblyName = null;
                if (uriString.Contains(";"))
                {
                    // Removing leading forward slash
                    assemblyName = uriString.Split(';')[0].Replace("/", "");
                }

                Type type = Type.GetType(!string.IsNullOrEmpty(assemblyName) ? typeName + "," + assemblyName : typeName,
                    // Should do case-insensitive search because typeName is not capitalized here
                    null, null, false, true);
                if (type != null)
                {
                    IXamlComponentFactory factory = Activator.CreateInstance(type) as
                        IXamlComponentFactory;
                    if (factory?.CreateComponent() is System.Windows.ResourceDictionary resourceDictionary)
                    {
                        resourceDictionary.Source = value;
                        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                    }
                }
#else
                System.Windows.ResourceDictionary resourceDictionary = new System.Windows.ResourceDictionary
                {
                    Source = _source
                };
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
#endif
            }
        }
    }
}
