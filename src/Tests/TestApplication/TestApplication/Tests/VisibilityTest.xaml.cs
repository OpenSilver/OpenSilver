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

namespace TestApplication.Tests
{
    public partial class VisibilityTest : Page
    {
        public VisibilityTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ButtonVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (VisibilityBorder.Visibility == Visibility.Collapsed)
                VisibilityBorder.Visibility = Visibility.Visible;
            else
                VisibilityBorder.Visibility = Visibility.Collapsed;
        }
    }
}
