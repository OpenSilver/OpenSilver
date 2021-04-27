using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System;

namespace TestApplication.Tests
{
    public partial class ComboBoxTest : Page
    {
        public ComboBoxTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ComboBox1.Items.Add("Initial item 1");
            ComboBox1.Items.Add("Initial item 2");
            ComboBox1.SelectedIndex = 1;
        }

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        private void ButtonTestComboBox_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestComboBox_ItemsAddString_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Add("Test String");
        }

        private void ButtonTestComboBox_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Clear();
        }

        private void ButtonTestComboBox_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Remove(ComboBox1.Items[0]);
        }

        private void ButtonTestComboBox_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestComboBox_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestComboBox_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Clear();
        }

        private void ButtonTestComboBox_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Remove(((ObservableCollection<string>)ComboBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestComboBox_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.ItemsSource = null;
        }

        private void ButtonTestComboBox_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = ComboBox1.Items[1];
        }

        private void ButtonTestComboBox_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedIndex = 1;
        }

        private void ButtonTestComboBox_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = null;
        }

        private void ButtonTestComboBox_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedIndex = -1;
        }
    }
}
