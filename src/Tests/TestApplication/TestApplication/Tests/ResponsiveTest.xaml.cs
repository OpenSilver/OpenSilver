using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class ResponsiveTest : Page
    {
        public ResponsiveTest()
        {
            InitializeComponent();

            sp6.SetValue(StackPanel.OrientationProperty, new ResponsiveExtension
            {
                Mobile = Orientation.Vertical,
                Desktop = Orientation.Horizontal,
                Threshold = new ResponsiveThreshold(300),
                Source = container6,
            }.ProvideValue(new ServiceProvider(sp6, StackPanel.OrientationProperty)));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
