

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
namespace System.Windows.Input
#else
namespace Windows.Devices.Input
#endif
{
    /// <summary>
    /// Enumerates pointer device types.
    /// </summary>
    public enum PointerDeviceType
    {
        /// <summary>
        /// A touch-enabled device
        /// </summary>
        Touch = 0,
             
        /// <summary>
        /// Pen
        /// </summary>
        Pen = 1,
        
        /// <summary>
        /// Mouse
        /// </summary>
        Mouse = 2,
    }
}