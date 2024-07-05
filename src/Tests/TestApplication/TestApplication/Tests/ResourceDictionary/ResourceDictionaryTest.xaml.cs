using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using TestApplication.Tests.ResourceDictionary;
using TestApplication.Tests.ResourceDictionary.CachedResourceDictionary;

namespace TestApplication.Tests
{
    public partial class ResourceDictionaryTest : Page
    {
        public ResourceDictionaryTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var customPropertyResourceDictionary = Resources.MergedDictionaries.OfType<CustomPropertyResourceDictionary>().FirstOrDefault();
            if (customPropertyResourceDictionary != null)
            {
                CustomPropertyValue.Text = customPropertyResourceDictionary.CustomProperty;
            }
        }

        private void CachedResourceDictionaryLoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CachedResourceDictionary cachedResourceDictionary = new CachedResourceDictionary
            {
                Source = new Uri(
#if OPENSILVER
                    "/TestApplication.OpenSilver;component/Tests/ResourceDictionary/CachedResourceDictionary/CountableResourceDictionary.xaml",
#else
                    "/TestApplication.Silverlight;component/Tests/ResourceDictionary/CachedResourceDictionary/CountableResourceDictionary.xaml",
#endif
                    UriKind.Relative)
            };
            CachedResourceDictionaryTextBox.Text += $"There are multiple cached dict (3 in XAML and one per button click), but the content has been loaded {CountableInstance.Count} times.\r\n";
        }
    }
}
