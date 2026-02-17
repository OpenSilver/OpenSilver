using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class MenuTest : Page
    {
        public MenuTest()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Generic click handler for menu items
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string header = menuItem.Header?.ToString() ?? "Unknown";
                UpdateOutput($"Clicked: {header}");
            }
        }

        /// <summary>
        /// Updates the output text block with the specified message
        /// </summary>
        private void UpdateOutput(string message)
        {
            OutputTextBlock.Text = message;
            OutputTextBlock.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
