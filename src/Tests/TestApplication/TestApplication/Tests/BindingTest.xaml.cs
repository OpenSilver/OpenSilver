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
using System.ComponentModel;

namespace TestApplication.Tests
{
    public partial class BindingTest : Page
    {
        public BindingTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        class BindingTestClass : INotifyPropertyChanged
        {
            private string _text = "before";
            public string Text
            {
                get { return _text; }
                set { _text = value; OnPropertyChanged("Text"); }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged(string propertyName)
            {
                if ((PropertyChanged != null))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private void ButtonSetDataContext_Click(object sender, RoutedEventArgs e)
        {
            StackPanelForBinding.DataContext = new BindingTestClass();
        }

        bool isTextBlockInVisualTree = true;
        TextBlock testBindingTextBlock;

        private void ButtonTestBinding_Click(object sender, RoutedEventArgs e)
        {
            if (isTextBlockInVisualTree)
            {
                testBindingTextBlock = TestBindingTextblock;
                StackPanelForBinding.Children.Remove(TestBindingTextblock);
                isTextBlockInVisualTree = false;
            }
            else
            {
                StackPanelForBinding.Children.Add(testBindingTextBlock);
                isTextBlockInVisualTree = true;
            }
        }

        private void ButtonTestBinding2_Click(object sender, RoutedEventArgs e)
        {
            ((BindingTestClass)StackPanelForBinding.DataContext).Text = new Random().Next().ToString();
        }
    }

    public class TemplateBindingExample
    {
        public static DependencyProperty TemplateBindingTextProperty =
            DependencyProperty.RegisterAttached
            (
                "TemplateBindingText",
                typeof(string),
                typeof(TextBox), // different source type (TextBox) than dependency property owner type (TemplateBindingExample)
                new PropertyMetadata(string.Empty)
            );

        public static void SetTemplateBindingText(TextBox element, string value)
        {
            element.SetValue(TemplateBindingTextProperty, value);
        }

        public static string GetTemplateBindingText(TextBox element)
        {
            return element.GetValue(TemplateBindingTextProperty)?.ToString();
        }
    }
}
