using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System;

namespace TestApplication.Tests.ListBoxes
{
    public partial class ListBox1Test : Page
    {
        public ListBox1Test()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListBox1.Items.Add("Initial item 1");
            ListBox1.Items.Add("Initial item 2");
            ListBox1.SelectedIndex = 1;
        }

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        public void ButtonTestListBox_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestListBox_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Clear();
        }

        private void ButtonTestListBox_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Remove(ListBox1.Items[0]);
        }

        private void ButtonTestListBox_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestListBox_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestListBox_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Clear();
        }

        private void ButtonTestListBox_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Remove(((ObservableCollection<string>)ListBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestListBox_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = null;
        }

        private void ButtonTestListBox_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedItem = ListBox1.Items[1];
        }

        private void ButtonTestListBox_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedIndex = 1;
        }

        private void ButtonTestListBox_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedItem = null;
        }

        private void ButtonTestListBox_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedIndex = -1;
        }
    }
}
