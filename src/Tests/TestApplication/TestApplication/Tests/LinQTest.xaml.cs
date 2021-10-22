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
    public partial class LinQTest : Page
    {
        public LinQTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TestGroupBy();
        }

        private void TestButtonLinq_Click(object sender, RoutedEventArgs e)
        {
            // Testing Linq:
            LinqSamples samples = new LinqSamples();

            samples.Linq1(); // This sample  uses the where clause  to find all elements  of an array with a value 
            // less than 5

            samples.Linq2(); // This sample uses the where clause to find all products that are out of stock

            samples.Linq3(); // This sample uses the where clause to find all products that are in  stock and cost 
            // more than 3.00 per unit

            //samples.Linq4(); // This sample uses the where  clause to find all customers in Washington and then it 
            // uses a foreach loop to iterate over the orders collection that belongs to each 
            // customer

            samples.Linq5(); // This sample demonstrates an indexed where clause that returns digits whose name is 
            // shorter than their value

            samples.Linq6();
            samples.Linq7();
            samples.Linq8();
            samples.Linq9();
            samples.Linq10();
            samples.Linq11();
            samples.Linq12();
            samples.Linq13();
            samples.Linq14();

            samples.Linq54();
            samples.Linq55();
            samples.Linq56();
            samples.Linq57();

            samples.Linq29();
            samples.Linq30();

            samples.Linq31();

            samples.Linq78();
            samples.Linq79();

            samples.LinqCustom1();
        }

        private void TestGroupBy()
        {
            List<double> list = new List<double>();
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            list.Add(6);
            //var v = System.Linq.Enumerable.GroupBy(list, o => o > 3); //works too
            var v = list.GroupBy(o => o > 3);
            foreach (var element in v)
            {
                var w = element.Key;

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6));
                stackPanel.Margin = new Thickness(2);
                TextBlock t = new TextBlock();
                t.Text = w.ToString();
                t.Margin = new Thickness(2);
                stackPanel.Children.Add(t);
                GroupByStackPanel.Children.Add(stackPanel);

                foreach (var element2 in element)
                {
                    var x = element2;
                    TextBlock t2 = new TextBlock();
                    t2.Text = x.ToString();
                    t2.Margin = new Thickness(2);
                    stackPanel.Children.Add(t2);
                    x -= 1;
                }
            }
        }
    }
}
