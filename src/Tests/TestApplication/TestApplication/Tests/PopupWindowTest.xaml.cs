using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;

namespace TestApplication.OpenSilver.Tests
{
    public partial class PopupWindowTest : Page
    {
        public PopupWindowTest()
        {
            this.InitializeComponent();
        }

        private void OpenSilver_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("https://opensilver.net"), "_blank", null);
        }

        private void OpenSilver1_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("https://opensilver.net"), "_blank", new HtmlPopupWindowOptions
            {
                Height = 200,
                Width = 200,
            });
        }
    }
}
