

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif 
{
    /// <summary>
    /// Defines the selection behavior for a ListBox.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// The user can select only one item at a time.
        /// </summary>
        Single = 0,

        /// <summary>
        /// The user can select multiple items without entering a special mode.
        /// </summary>
        Multiple = 1,
       
        ///// <summary>
        ///// The user can select multiple items by entering a special mode, for example
        ///// when pressing a modifier key.
        ///// </summary>
        Extended = 2,
    }
}
