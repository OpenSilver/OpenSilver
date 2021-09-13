
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

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public partial class Binding
	{
        [OpenSilver.NotImplemented]
		public Binding(Binding original)
		{
			original?.CopyTo(this);
		}

        /// <summary>
        /// Gets or sets a value that indicates whether the binding ignores any <see cref="ICollectionView"/>
        /// settings on the data source.
        /// </summary>
        /// <returns>
        /// true if the binding binds directly to the data source; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool BindsDirectlyToSource { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report validation
        /// errors from an System.ComponentModel.INotifyDataErrorInfo implementation on the
        /// bound data entity.
        /// </summary>
        /// <returns>
        /// true if the binding engine will report System.ComponentModel.INotifyDataErrorInfo
        /// validation errors; otherwise, false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool ValidatesOnNotifyDataErrors { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the binding engine will report validation
        /// errors from an System.ComponentModel.IDataErrorInfo implementation on the bound
        /// data entity.
        /// </summary>
        /// <returns>
        /// true if the binding engine will report System.ComponentModel.IDataErrorInfo validation
        /// errors; otherwise, false. The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool ValidatesOnDataErrors { get; set; }
    }
}
