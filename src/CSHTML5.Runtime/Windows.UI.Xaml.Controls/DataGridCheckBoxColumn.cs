
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    /// Represents a System.Windows.Controls.DataGrid column that hosts System.Windows.Controls.CheckBox
    /// controls in its cells.
    /// </summary>
    public class DataGridCheckBoxColumn : DataGridBoundColumn
    {
        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            //return GenerateElement(childData, true);
            return null;
        }

        internal override FrameworkElement GenerateElement(object childData)
        {
            return GenerateElement(childData, false);
        }

        private FrameworkElement GenerateElement(object childData, bool enable)
        {
            CheckBox checkBox = new CheckBox();
            //if (enable)
            //{
                checkBox.DataContext = childData;
            //}
            checkBox.IsEnabled = enable; //note: this is to avoid having the possibility to check/uncheck a checkbox when the datagrid/column is readonly
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.

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
    }
}
