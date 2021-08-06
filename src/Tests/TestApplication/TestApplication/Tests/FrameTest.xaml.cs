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
    public partial class FrameTest : Page
    {
        public FrameTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Frame1_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame1_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void Frame1_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void Frame1_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoForward)
            {
                MyFrame.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

        private void Frame2_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame2.CanGoBack)
            {
                MyFrame2.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame2_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame2.Source = uri;
        }

        private void Frame2_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame2.Source = uri;
        }

        private void Frame2_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame2.CanGoForward)
            {
                MyFrame2.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

        private void Frame3_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame3.CanGoBack)
            {
                MyFrame3.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame3_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame3.Source = uri;
        }

        private void Frame3_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame3.Source = uri;
        }

        private void Frame3_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame3.CanGoForward)
            {
                MyFrame3.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

        private void InnerFrame_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void InnerFrame_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;

            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            frame.Source = uri;
        }

        private void InnerFrame_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            frame.Source = uri;
        }

        private void InnerFrame_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            if (frame.CanGoForward)
            {
                frame.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }
    }
}
