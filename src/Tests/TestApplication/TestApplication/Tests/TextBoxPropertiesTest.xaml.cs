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
    public partial class TextBoxPropertiesTest : Page
    {
        public TextBoxPropertiesTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void CheckBoxForWrapping_Checked(object sender, RoutedEventArgs e)
        {
            if (TextBoxForWrapping != null)
                TextBoxForWrapping.TextWrapping = TextWrapping.Wrap;
        }

        private void CheckBoxForWrapping_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TextBoxForWrapping != null)
                TextBoxForWrapping.TextWrapping = TextWrapping.NoWrap;
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionTextBlock.Text = SelectionTextBox.SelectedText;
            SelectionStartTextBlock.Text = SelectionTextBox.SelectionStart.ToString();
            SelectionLengthTextBlock.Text = SelectionTextBox.SelectionLength.ToString();
        }
    }
}
