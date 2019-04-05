
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    /// Represents a System.Windows.Controls.DataGrid column that hosts textual content
    /// in its cells.
    /// </summary>
    public class DataGridTextColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            TextBox textBox = new TextBox();
            textBox.DataContext = childData;
            Binding b =  this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
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
            return textBox;
        }



        internal override FrameworkElement GenerateElement(object childData)
        {
            TextBlock textBlock = new TextBlock();
            //textBlock.DataContext = childData;
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
            if (b != null)
                textBlock.SetBinding(TextBlock.TextProperty, b);
            else if (childData is string)
                textBlock.Text = (string)childData;
            return textBlock;
        }
    }
}
