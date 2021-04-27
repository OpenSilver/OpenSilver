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

namespace TestApplication.Tests.Grids
{
    public partial class Grid_canvas_overlapping_bugTest : Page
    {
        public Grid_canvas_overlapping_bugTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void TestBugCanvasInGrid_AddCanvas_Click(object sender, RoutedEventArgs e)
        {
            Random rd = new Random();
            TestBugCanvasInGrid.Children.Add(new Rectangle() { Width = 100, Height = 30, Fill = new SolidColorBrush(new Color() { A = (byte)255, B = (byte)rd.Next(256), G = (byte)rd.Next(256), R = (byte)rd.Next(256) }) });
        }
        int i = 0;
        private void TestBugCanvasInGrid_AddText_Click(object sender, RoutedEventArgs e)
        {
            TestBugCanvasInGrid.Children.Add(new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Text = "Test " + i });
            ++i;
        }
        private void TestBugCanvasInGrid_Reset_Click(object sender, RoutedEventArgs e)
        {
            TestBugCanvasInGrid.Children.Clear();
        }
    }
}
