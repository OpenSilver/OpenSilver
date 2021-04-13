using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if SLMIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif


namespace TestApplication
{
    public class MyCustomImageControl : FrameworkElement
    {
        public MyCustomImageControl()
        {
#if CSHTML5
            // Specify the HTML representation of the control
            CSharpXamlForHtml5.DomManagement.SetHtmlRepresentation(this, "<img/>");
#endif
        }

        // Dependency Property to store the Image Source URL
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(MyCustomImageControl), new PropertyMetadata("", Source_Changed));

        // Called when the "Source" property changes
        static void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (MyCustomImageControl)sender;
            var newValue = (string)e.NewValue;

            // Always check that the control is in the Visual Tree before modifying its HTML representation
#if CSHTML5
            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                // Update the "src" property of the <img> tag
                CSHTML5.Interop.ExecuteJavaScript("$0.src = $1", CSHTML5.Interop.GetDiv(control), newValue);
            }
#endif
        }
    }
}
