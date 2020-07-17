

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


using Microsoft.JSInterop;

namespace DotNetForHtml5
{
    public class JavaScriptExecutionHandler : IJavaScriptExecutionHandler
    {
        public IJSRuntime JSRuntime { get; set; }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            JSRuntime.InvokeVoidAsync("callJS", javaScriptToExecute);
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            object result = ((JSInProcessRuntime)JSRuntime).Invoke<object>("callJS", javaScriptToExecute);
            return result;
        }
    }
}
