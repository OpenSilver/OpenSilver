using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Printing;

namespace TestApplication.OpenSilver
{
    public partial class PrintDocumentTest : Page
    {
        public PrintDocumentTest()
        {
            this.InitializeComponent();
        }

        void ButtonPrint_DockPanel(object sender, RoutedEventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += Print_DockPanel;
            doc.Print("DockPanel");
        }

        private void Print_DockPanel(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = DockPanel;
            e.HasMorePages = false;
        }

        private void ButtonPrint_Invoice(object sender, RoutedEventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += Print_Invoice;
            doc.Print("Invoice");
        }

        private void Print_Invoice(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = Invoice;
            e.HasMorePages = false;
        }

        Stack<UIElement> elements = new Stack<UIElement>();
        private void ButtonPrint_Both(object sender, RoutedEventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += Doc_PrintBoth;

            elements.Push(DockPanel);
            elements.Push(Invoice);

            doc.Print("DockPanel and Invoice");
        }

        private void Doc_PrintBoth(object sender, PrintPageEventArgs e)
        {
            if (elements.Count == 0)
            {
                e.PageVisual = null;
                e.HasMorePages = false;
            }
            else
            {
                e.PageVisual = elements.Pop();
                e.HasMorePages = true;
            }
        }
    }
}
