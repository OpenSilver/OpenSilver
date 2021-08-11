

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
#if !MIGRATION
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies the panel that the ItemsPresenter creates for the layout of the
    /// items of an ItemsControl.
    /// </summary>
    public partial class ItemsPanelTemplate : FrameworkTemplate
    {
        /// <summary>
        /// Initializes a new instance of the ItemsPanelTemplate class.
        /// </summary>
        public ItemsPanelTemplate() : base() { }
    }
}
