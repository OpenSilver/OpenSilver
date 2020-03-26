

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

namespace System.Net
{
    
    public enum CredentialsMode
    {
        /// <summary>
        /// Cookies are not passed
        /// </summary>
        Disabled,
        /// <summary>
        /// Cookies are passed
        /// </summary>
        Enabled,
        /// <summary>
        /// The first request will attempt to use cookies. If it fails, the first request and all the subsequent ones are done without cookies.
        /// </summary>
        Auto
    }
}
