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
    public partial class NestedElementsTest : Page
    {
        public NestedElementsTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void TextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            Control s = (Control)sender;
            s.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xFF, 0xFF));
        }

        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            Control s = (Control)sender;
            s.Background = new SolidColorBrush(Colors.Black);
        }

        void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //((Button)sender).Content = TextBox1.Text;
            //TextBox1.Text = "test";
            //Uri uri = new Uri("C:\\Users\\Sylvain\\Documents\\Adventure Maker v4.7\\Projects\\ASA_game\\Icons\\settings.ico");
            //BitmapImage bmpImage = new BitmapImage(uri);
            //Image1.Source = bmpImage;
            //Image1.Stretch = Stretch.Fill;
        }
    }
}
