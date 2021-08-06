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
    public partial class TextChangedTest : Page
    {
        public TextChangedTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void TestTextChanged_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TestTextChanged_Count.Text = (int.Parse(TestTextChanged_Count.Text) + 1).ToString();
        }
    }
}
