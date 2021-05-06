

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
    public abstract partial class DataGridBoundColumn : DataGridColumn
    {
        private BindingBase _binding;

        public BindingBase Binding
        {
            get
            {
                return this._binding;
            }
            set
            {
                if (this._binding != value)
                {
                    BindingBase oldBinding = this._binding;
                    this._binding = value;
                    this.OnBindingChanged(oldBinding, this._binding);
                }
            }
        }

        private void OnBindingChanged(BindingBase oldBinding, BindingBase newBinding)
        {
            //todo: tell the DataGrid that the Binding has changed so it has to refresh the elements (only i it is already in the visual tree).
        }
    }
}
