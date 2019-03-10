
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
    public abstract class DataGridBoundColumn : DataGridColumn
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
            DependencyProperty.Register("Binding", typeof(BindingBase), typeof(DataGridBoundColumn), new PropertyMetadata(null, Binding_Changed));


        static void Binding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: tell the DataGrid that the Binding has changed so it has to refresh the elements (only i it is already in the visual tree).
        }
    }
}
