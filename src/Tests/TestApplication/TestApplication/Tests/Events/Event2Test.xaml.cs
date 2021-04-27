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
    public partial class Event2Test : Page
    {
        public Event2Test()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void TestButtonParent_Click(object sender, MouseEventArgs e)
        {
            TestButtonParentCount.Text = (int.Parse(TestButtonParentCount.Text) + 1).ToString();
        }
        
        void TestButton1_Click(object sender, MouseEventArgs e)
        {
            TestButton1Count.Text = (int.Parse(TestButton1Count.Text) + 1).ToString();
            //e.Handled = true;
        }
        
        void TestButton2_Click(object sender, MouseButtonEventArgs e)
        {
            TestButton2Count.Text = (int.Parse(TestButton2Count.Text) + 1).ToString();
            e.Handled = true;
        }
    }
}
