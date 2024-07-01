using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TestApplication.Tests.ResourceDictionary.CachedResourceDictionary;

namespace TestApplication.Tests
{
    public partial class ResourceDictionaryTest : Page
    {
        public ResourceDictionaryTest()
        {
            InitializeComponent();
        }

        private void CachedResourceDictionaryLoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CachedResourceDictionary cachedResourceDictionary = new CachedResourceDictionary
            {
                Source = new Uri(
#if OPENSILVER
                    "/TestApplication.OpenSilver;component/Tests/ResourceDictionary/CachedResourceDictionary/SomeResourceDictionary.xaml",
#else
                    "/TestApplication.Silverlight;component/Tests/ResourceDictionary/CachedResourceDictionary/SomeResourceDictionary.xaml",
#endif
                    UriKind.Relative)
            };
            CachedResourceDictionaryTextBox.Text += $"There are multiple cached dict (3 in XAML and one per button click), but the content has been loaded {CountableInstance.Count} times.\r\n";
        }
    }
}
