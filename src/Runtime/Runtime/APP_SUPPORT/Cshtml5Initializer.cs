
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
using DotNetForHtml5.Core;

namespace DotNetForHtml5
{
    public static class Cshtml5Initializer
    {
        public static void Initialize(IWebAssemblyExecutionHandler executionHandler)
        {
            Initialize((IJavaScriptExecutionHandler)executionHandler);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Initialize(IJavaScriptExecutionHandler executionHandler)
        {
            INTERNAL_Simulator.JavaScriptExecutionHandler = executionHandler;
        }
    }
}
