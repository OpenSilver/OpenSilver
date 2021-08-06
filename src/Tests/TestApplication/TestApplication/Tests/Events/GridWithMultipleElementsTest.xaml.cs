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

namespace TestApplication.Tests.Events
{
    public partial class GridWithMultipleElementsTest : Page
    {
        public GridWithMultipleElementsTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void TestLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Left button clicked.");
        }

        void TestRightButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Right button clicked.");
        }
    }
}
