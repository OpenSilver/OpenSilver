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

namespace TestApplication.Tests.DataGrids
{
    public class Cat
    {
        public Cat(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }
        public int Age { get; set; }

    }

    public partial class DataGrid1Test : Page
    {
        public DataGrid1Test()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //DataGrid1.Items.Add(new Cat("Tom", 10));
            //DataGrid1.Items.Add(new Cat("Blacky", 20));
        }

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        //private void ButtonTestDataGrid_ItemsAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Add("Item #" + RandomId());
        //}

        //private void ButtonTestDataGrid_ItemsAddString_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Add("Test String");
        //}

        //private void ButtonTestDataGrid_ItemsClear_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Clear();
        //}

        //private void ButtonTestDataGrid_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Remove(DataGrid1.Items[0]);
        //}

        //private void ButtonTestDataGrid_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.ItemsSource = new ObservableCollection<string>()
        //    {
        //        "One", "Two", "Three"
        //    };
        //}

        //private void ButtonTestDataGrid_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Add("Item #" + RandomId());
        //}

        //private void ButtonTestDataGrid_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Clear();
        //}

        //private void ButtonTestDataGrid_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Remove(((ObservableCollection<string>)DataGrid1.ItemsSource).FirstOrDefault());
        //}

        //private void ButtonTestDataGrid_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.ItemsSource = null;
        //}

        //private void ButtonTestDataGrid_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedItem = DataGrid1.Items[1];
        //}

        //private void ButtonTestDataGrid_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedIndex = 1;
        //}

        //private void ButtonTestDataGrid_SelectItemNull_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedItem = null;
        //}

        //private void ButtonTestDataGrid_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedIndex = -1;
        //}
    }
}
