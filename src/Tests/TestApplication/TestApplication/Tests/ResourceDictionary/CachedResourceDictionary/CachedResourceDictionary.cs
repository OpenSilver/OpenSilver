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

                System.Windows.ResourceDictionary resourceDictionary = new System.Windows.ResourceDictionary
                {
                    Source = _source
                };
#if OPENSILVER
                Application.LoadComponent(resourceDictionary, _source);
#endif
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}
