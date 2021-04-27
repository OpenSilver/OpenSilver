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
    public partial class Double_ClickTest : Page
    {
        public Double_ClickTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        
        private void TestDoubleClick_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                MessageBox.Show("You double-clicked!");
        }
    }
}
