using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace TestApplication
{
    public class MyCustomImageControl : FrameworkElement
    {
        public MyCustomImageControl()
        {
        }

        // Dependency Property to store the Image Source URL
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(MyCustomImageControl), new PropertyMetadata(""));
    }
}
