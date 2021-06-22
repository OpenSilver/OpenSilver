

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
#if WORKINPROGRESS

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

        [OpenSilver.NotImplemented]
        internal override FrameworkElement GenerateElement(object childData)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

        [OpenSilver.NotImplemented]
        protected abstract object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs);
        
        [OpenSilver.NotImplemented]
        protected abstract FrameworkElement GenerateElement(DataGridCell cell, object dataItem);
        
        [OpenSilver.NotImplemented]
        public virtual Binding ClipboardContentBinding { get; set; }
    }
}
#endif