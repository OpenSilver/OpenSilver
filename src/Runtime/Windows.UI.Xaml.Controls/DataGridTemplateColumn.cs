

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
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a System.Windows.Controls.DataGrid column that hosts template-specified
    /// content in its cells.
    /// </summary>
    public partial class DataGridTemplateColumn : DataGridBoundColumn
    {
        /// <summary>
        /// Gets or sets the template to use to display the contents of a cell that is
        /// not in editing mode.
        /// </summary>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridTemplateColumn.CellTemplate
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridTemplateColumn), new PropertyMetadata(null, CellTemplateProperty_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void CellTemplateProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: tell the DataGrid to redraw this column for all the children.
        }

        /// <summary>
        /// Gets or sets the template to use to display the contents of a cell that is
        /// in editing mode.</summary>
        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridTemplateColumn.CellEditingTemplate
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridTemplateColumn), new PropertyMetadata(null, CellEditingTemplateProperty_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void CellEditingTemplateProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //probably nothing to do here
        }

        internal override FrameworkElement GenerateElement(object childData)
        {
            if (CellTemplate != null)
            {
                FrameworkElement element = CellTemplate.INTERNAL_InstantiateFrameworkTemplate();
                element.DataContext = childData;
                return element;
            }
            else //we are going to make a ToString() of the element the user tries to show:
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = childData.ToString();
                return textBlock;
            }
        }

        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            if (CellEditingTemplate != null)
            {
                FrameworkElement element = CellEditingTemplate.INTERNAL_InstantiateFrameworkTemplate();
                element.DataContext = childData;
                return element;
            }
            else
            {
                return null;
                //return GenerateElement(childData);
            }
        }
    }
}
