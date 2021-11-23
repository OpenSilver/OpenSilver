

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
    /// Represents a <see cref="DataGrid"/> column that hosts <see cref="CheckBox"/>
    /// controls in its cells.
    /// </summary>
    public partial class DataGridCheckBoxColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            //return GenerateElement(childData, true);
            return null;
        }
        private FrameworkElement GenerateElement(object childData, bool enable)
        {
            CheckBox checkBox = new CheckBox();
            //if (enable)
            //{
                checkBox.DataContext = childData;
            //}
            checkBox.IsEnabled = enable; //note: this is to avoid having the possibility to check/uncheck a checkbox when the datagrid/column is readonly
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
                checkBox.SetBinding(CheckBox.IsCheckedProperty, b);
            }
            else if (childData is bool)
                checkBox.IsChecked = (bool)childData;
            return checkBox;
        }

        internal override void EnterEditionMode(DataGridCell dataGridCell)
        {
            //base.EnterEditionMode(dataGridCell); //this does nothing but might do something in the future
            ((FrameworkElement)dataGridCell.Content).IsEnabled = true;
        }

        internal override void LeaveEditionMode(DataGridCell dataGridCell)
        {
            //base.LeaveEditionMode(dataGridCell); //this does nothing but might do something in the future
            ((FrameworkElement)dataGridCell.Content).IsEnabled = false;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(dataItem, false);
        }
    }
}
