

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a <see cref="DataGrid"/> column that hosts textual content
    /// in its cells.
    /// </summary>
    public partial class DataGridTextColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            TextBox textBox = new TextBox();
            textBox.DataContext = childData;
            Binding b = GetBinding();
            if (b != null)
            {
                if (b.Mode == BindingMode.OneWay)
                {
                    if (!b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                    {
                        b.Mode = BindingMode.TwoWay;
                    }
                }
                textBox.SetBinding(TextBox.TextProperty, b);
                //we make sure we will be able to get the value once we leave the Edition mode:
                //cell.EditingElementContainingValue = textBox;
                //cell.PropertyContainingValue = TextBox.TextProperty;
                //textBox.TextChanged += cell.TextChanged;
                //we make the same Binding on the cell's property in TwoWay mode so we are able to change its value when leaving the edition mode:
                //Binding b2 = b.Clone();
                //b2.Mode = BindingMode.TwoWay;
                //cell.SetBinding(DataGridCell.MyPropertyProperty, b2);
            }
            else if (childData is string)
                textBox.Text = (string)childData;

            // Focus the TextBox when loaded, and select all text:
            textBox.Loaded += (s, e) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            return textBox;
        }

        private FrameworkElement GenerateElement(object childData)
        {
            TextBlock textBlock = new TextBlock();
            //textBlock.DataContext = childData;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            Binding b = GetBinding();
            if (b != null)
                textBlock.SetBinding(TextBlock.TextProperty, b);
            else if (childData is string)
                textBlock.Text = (string)childData;
            return textBlock;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(dataItem);
        }

    }
}
