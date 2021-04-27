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
    public partial class Event1Test : Page
    {
        public Event1Test()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void TestButtonParent1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Parent 1");
        }

        void TestButtonParent2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Parent 2");
        }
    }
}
