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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
    public class DataGridRowGroupHeader : Control
    {
        #region PropertyNameVisibility
        /// <summary>
        /// Gets or sets a value that indicates whether the property name is visible.
        /// </summary>
        public Visibility PropertyNameVisibility
        {
            get { return (Visibility)GetValue(PropertyNameVisibilityProperty); }
            set { SetValue(PropertyNameVisibilityProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for PropertyNameVisibility
        /// </summary>
        public static readonly DependencyProperty PropertyNameVisibilityProperty =
            DependencyProperty.Register(
                "PropertyNameVisibility",
                typeof(Visibility),
                typeof(DataGridRowGroupHeader),
                null);
        #endregion PropertyNameVisibility

        #region SublevelIndent
        /// <summary>
        /// Gets or sets a value that indicates the amount that the 
        /// children of the <see cref="T:System.Windows.Controls.RowGroupHeader" /> are indented. 
        /// </summary>
        public double SublevelIndent
        {
            get { return (double)GetValue(SublevelIndentProperty); }
            set { SetValue(SublevelIndentProperty, value); }
        }

        /// <summary>
        /// SublevelIndent Dependency property
        /// </summary>
        public static readonly DependencyProperty SublevelIndentProperty =
            DependencyProperty.Register(
                "SublevelIndent",
                typeof(double),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(20d/*DataGrid.DATAGRID_defaultRowGroupSublevelIndent*/, OnSublevelIndentPropertyChanged));

        private static void OnSublevelIndentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion SublevelIndent
    }
}
