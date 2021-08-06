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
    public partial class FocusTest : Page
    {
        public FocusTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void FocusTextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusTextBlockLog.Text = FocusTextBlockLog.Text + Environment.NewLine + "Lost focus on text box 1";
        }

        private void FocusTextBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusTextBlockLog.Text = FocusTextBlockLog.Text + Environment.NewLine + "Got focus on text box 2";
        }
    }
}
