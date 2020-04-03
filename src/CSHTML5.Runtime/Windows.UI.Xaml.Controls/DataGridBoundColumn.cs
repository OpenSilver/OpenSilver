

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
    /// Serves as the base class for columns that can bind to a property in the data
    /// source of a System.Windows.Controls.DataGrid.
    /// </summary>
    public abstract partial class DataGridBoundColumn : DataGridColumn
    {
        /// <summary>
        /// Gets or sets the binding that associates the column with a property in the
        /// data source.
        /// </summary>
        public BindingBase Binding
        {
            get { return (BindingBase)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridBoundColumn.Binding dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(BindingBase), typeof(DataGridBoundColumn), new PropertyMetadata(null, Binding_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        static void Binding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: tell the DataGrid that the Binding has changed so it has to refresh the elements (only i it is already in the visual tree).
        }
    }
}
