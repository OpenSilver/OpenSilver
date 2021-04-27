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
    public partial class PrintingTest : Page
    {
        public PrintingTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        // SLDISABLED
        /*
        void ButtonPrint_SpecificElement_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.Print(ElementToPrint);
        }

        void ButtonPrint_ShowDialog_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.Print();
        }

        void ButtonPrint_SetPrintArea_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.SetPrintArea(ElementToPrint);
        }

        void ButtonPrint_ResetPrintArea_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.ResetPrintArea();
        }

        void ButtonPrint_InMemoryElement_Click(object sender, RoutedEventArgs e)
        {
            // Create the element to print (some black text on a yellow page):
            var pageBackground = new StackPanel() { Background = new SolidColorBrush(Colors.Yellow) };
            pageBackground.Children.Add(new TextBlock() { Text = "This is some text to print.", TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Colors.Black) }); ;

            // Print it:
            CSHTML5.Native.Html.Printing.PrintManager.Print(pageBackground);
        }*/
    }
}
