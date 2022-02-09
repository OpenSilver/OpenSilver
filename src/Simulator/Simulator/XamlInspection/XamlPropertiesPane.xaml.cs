

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetForHtml5.EmulatorWithoutJavascript.XamlInspection
{
    /// <summary>
    /// Interaction logic for XamlPropertiesPane.xaml
    /// </summary>
    public partial class XamlPropertiesPane : UserControl
    {
        public XamlPropertiesPane()
        {
            InitializeComponent();
        }

        public void Refresh(object targetElement)
        {
            // Clear the display:
            WritablesPanel.Children.Clear();
            ReadOnlyPanel.Children.Clear();

            if (targetElement != null)
            {
                // Get the element type:
                Type targetElementType = targetElement.GetType();

                // Get the list of properties that have a "set" accessor, sorted alphabetically:
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                var properties =
                    from property
                    in targetElementType.GetProperties(flags)
                    orderby property.Name
                    select property;

                // Display the writable properties and their values:
                foreach (PropertyInfo propertyInfo in properties.Where(p => p.CanWrite))
                {
                    var editor = new XamlSinglePropertyEditor(propertyInfo, targetElement, false);
                    WritablesPanel.Children.Add(editor);
                }

                // Display the read only properties and their values:
                foreach (PropertyInfo propertyInfo in properties.Where(p => !p.CanWrite))
                {
                    var editor = new XamlSinglePropertyEditor(propertyInfo, targetElement, true);
                    ReadOnlyPanel.Children.Add(editor);
                }
            }

            // Set the appropriate size:
            if (WritablesPanel.Children.Count > 0)
                this.Width = 250;
            else
                this.Width = 0;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var readonlyPropertyElements = ReadOnlyPanel.Children.Cast<XamlSinglePropertyEditor>();
            foreach (var element in readonlyPropertyElements)
            {
                element.Refresh();
            }

            var writablePropertyElements = WritablesPanel.Children.Cast<XamlSinglePropertyEditor>();
            foreach (var element in readonlyPropertyElements)
            {
                element.Refresh();
            }
        }
    }
}
