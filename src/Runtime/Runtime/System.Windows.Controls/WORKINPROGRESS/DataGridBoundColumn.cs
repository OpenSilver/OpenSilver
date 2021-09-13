

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

#if MIGRATION
using System.Windows.Data;
#else
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
    /// source of a <see cref="DataGrid"/>.
    /// </summary>
    public partial class DataGridBoundColumn
    {
        private Binding _clipboardContentBinding;

        protected DataGridBoundColumn()
        {

        }


        [OpenSilver.NotImplemented]

        public Style EditingElementStyle { get; set; }

        [OpenSilver.NotImplemented]

        public Style ElementStyle { get; set; }

        [OpenSilver.NotImplemented]

        internal override FrameworkElement GenerateEditingElement(object childData)
        {
            //return GenerateElement(childData, true);
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, gets an editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">
        /// The cell that will contain the generated element.
        /// </param>
        /// <param name="dataItem">
        /// The data item represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </returns>
        protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

        [OpenSilver.NotImplemented]
        protected abstract object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs);

                /// <summary>
        /// The binding that will be used to get or set cell content for the clipboard.
        /// </summary>
        public virtual Binding ClipboardContentBinding
        {
            get
            {
                return this._clipboardContentBinding;
            }
            set
            {
                this._clipboardContentBinding = value;
            }
        }

    }
}
