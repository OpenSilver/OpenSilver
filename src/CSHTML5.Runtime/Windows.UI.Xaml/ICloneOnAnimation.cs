

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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal partial interface ICloneOnAnimation //todo: make it public?
    {
        /// <summary>
        /// Returns a clone of this object. Note: make sure to add a boolean to say that the clone is already a clone (for its future uses).
        /// </summary>
        /// <returns>A clone of this object.</returns>
        object Clone();
        bool IsAlreadyAClone();
    }
}
