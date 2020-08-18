

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
using System.Reflection;

namespace CSHTML5.Internal
{
    public static class WebRequestsHelper
    {
        public static string MakeRequest(string address, string method, string body, Dictionary<string, string> headers = null, bool isAsync = false)
        {
            return new INTERNAL_WebRequestHelper_JSOnly().MakeRequest(new Uri(address), method,  null, headers, body, null, isAsync);
        }
    }
}
