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

namespace TestApplication.Tests
{
    public partial class AutoCompleteBoxTest : Page
    {
        public AutoCompleteBoxTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //AutoCompleteBox1.Items.Add("Initial item 1");
            //AutoCompleteBox1.Items.Add("Initial item 2");
            //AutoCompleteBox1.SelectedIndex = 1;
        }

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        //private void ButtonTestAutoCompleteBox1_ItemsAddUIElement_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.Items.Add(new TextBlock() { Text = "This is a UI element", Foreground = new SolidColorBrush(Colors.Red) });
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsAddString_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.Items.Add("Test String");
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.Items.Add("Item #" + RandomId());
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsClear_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.Items.Clear();
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.Items.Remove(AutoCompleteBox1.Items[0]);
        //}

        //private void ButtonTestAutoCompleteBox1_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.ItemsSource = new ObservableCollection<string>()
        //    {
        //        "One", "Two", "Three"
        //    };
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Add("Item #" + RandomId());
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Clear();
        //}

        //private void ButtonTestAutoCompleteBox1_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Remove(((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).FirstOrDefault());
        //}

        //private void ButtonTestAutoCompleteBox1_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.ItemsSource = null;
        //}

        //private void ButtonTestAutoCompleteBox1_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.SelectedItem = AutoCompleteBox1.Items[1];
        //}

        //private void ButtonTestAutoCompleteBox1_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.SelectedIndex = 1;
        //}

        //private void ButtonTestAutoCompleteBox1_SelectItemNull_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.SelectedItem = null;
        //}

        //private void ButtonTestAutoCompleteBox1_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        //{
        //    AutoCompleteBox1.SelectedIndex = -1;
        //}
    }
}
