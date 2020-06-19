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
            MainContainer.Children.Clear();

            if (targetElement != null)
            {
                // Get the element type:
                Type targetElementType = targetElement.GetType();

                // Get the list of properties that have a "set" accessor, sorted alphabetically:
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                var properties =
                    from property
                    in targetElementType.GetProperties(flags)
                    where property.CanWrite
                    orderby property.Name
                    select property;

                // Display the properties and their values:
                foreach (PropertyInfo propertyInfo in properties)
                {
                    var editor = new XamlSinglePropertyEditor(propertyInfo, targetElement);
                    MainContainer.Children.Add(editor);
                }
            }

            // Set the appropriate size:
            if (MainContainer.Children.Count > 0)
                this.Width = 250;
            else
                this.Width = 0;
        }
    }
}
