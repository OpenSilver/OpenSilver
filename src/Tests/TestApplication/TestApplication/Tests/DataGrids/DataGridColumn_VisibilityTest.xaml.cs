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

namespace TestApplication.Tests.DataGrids
{
    public partial class DataGridColumn_VisibilityTest : Page
    {
        public DataGridColumn_VisibilityTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        //int visibilityState = 0;
        //private void ButtonTestDataGridColumnVisibility_Click(object sender, RoutedEventArgs e)
        //{
        //    if (visibilityState == 0)
        //    {
        //        NameDataGridColumn.Visibility = Visibility.Collapsed;
        //        visibilityState = 1;
        //    }
        //    else if (visibilityState == 1)
        //    {
        //        AgeDataGridColumn.Visibility = Visibility.Collapsed;
        //        visibilityState = 2;
        //    }
        //    else if (visibilityState == 2)
        //    {
        //        NameDataGridColumn.Visibility = Visibility.Visible;
        //        visibilityState = 3;
        //    }
        //    else if (visibilityState == 3)
        //    {
        //        AgeDataGridColumn.Visibility = Visibility.Visible;
        //        visibilityState = 0;
        //    }
        //}
    }
}
