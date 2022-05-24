using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls.Primitives;

#if OPENSILVER
using OpenSilver.Internal.Xaml.Context;
#endif

namespace TestApplication.Tests
{
    public partial class AutoCompleteBoxTest : Page
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>();

        public AutoCompleteBoxTest()
        {
            InitializeComponent();

#if OPENSILVER
            // Virtualizing the ListBox of the ACB dropdown by giving it a custom template
            AutoCompleteVirtualized.Loaded += (s, e) =>
            {
                IList<DependencyObject> acbDescendants = AutoCompleteVirtualized.GetVisualDescendants().ToList();
                Popup popup = acbDescendants.OfType<Popup>().First();

                IList<DependencyObject> popupDescendants = popup.Child.GetVisualDescendants().ToList();
                Grid popupGrid = popupDescendants.OfType<Grid>().First();
                popupGrid.CustomLayout = true;
                popupGrid.IsAutoHeightOnCustomLayout = true;
                popupGrid.IsAutoWidthOnCustomLayout = true;

                ListBox listBox = popupDescendants.OfType<ListBox>().First();

                var xamlContext = global::OpenSilver.Internal.Xaml.RuntimeHelpers.Create_XamlContext();
                var itemsPanelTemplate = new ItemsPanelTemplate();
                global::OpenSilver.Internal.Xaml.RuntimeHelpers.SetTemplateContent(itemsPanelTemplate, xamlContext, CreateItemsPanelTemplate);
                listBox.ItemsPanel = itemsPanelTemplate;
            };
        }

        private FrameworkElement CreateItemsPanelTemplate(FrameworkElement templateOwner_ItemsPanelTemplate,
            XamlContext xamlContext)
        {
            var virtualizingStackPanel = new VirtualizingStackPanel();
            global::OpenSilver.Internal.Xaml.RuntimeHelpers.SetTemplatedParent(virtualizingStackPanel, templateOwner_ItemsPanelTemplate);
            return virtualizingStackPanel;
        }
#else
        }
#endif

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CreateNewItemsSource();
        }

        private void CreateNewItemsSource()
        {
            _items = new ObservableCollection<string>(Enumerable.Range(1, 50).Select(i => $"Initial item {i}").ToList());
            AutoCompleteBox1.ItemsSource = _items;
            AutoCompleteBox1.SelectedItem = _items[0];

            AutoCompleteVirtualized.ItemsSource = _items;
            AutoCompleteVirtualized.SelectedItem = _items[0];
        }

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        private void ButtonTestAutoCompleteBox1_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            _items.Add("Item #" + RandomId());
        }

        private void ButtonTestAutoCompleteBox1_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            _items.Clear();
        }

        private void ButtonTestAutoCompleteBox1_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            _items.Remove(_items[0]);
        }

        private void ButtonTestAutoCompleteBox1_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            CreateNewItemsSource();
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Clear();
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Remove(((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestAutoCompleteBox1_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.ItemsSource = null;
        }

        private void ButtonTestAutoCompleteBox1_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedItem = _items[1];
        }

        private void ButtonTestAutoCompleteBox1_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedItem = null;
        }
    }
}
