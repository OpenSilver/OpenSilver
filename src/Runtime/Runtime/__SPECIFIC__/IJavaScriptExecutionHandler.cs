

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



#if CSHTML5NETSTANDARD

using System;

namespace DotNetForHtml5
{
    public interface IJavaScriptExecutionHandler
    {
        void ExecuteJavaScript(string javaScriptToExecute);

        object ExecuteJavaScriptWithResult(string javaScriptToExecute);        
    }

    public interface IJavaScriptExecutionHandler2 : IJavaScriptExecutionHandler
    {
        TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0);
    }

    internal sealed class JSRuntimeWrapper : IJavaScriptExecutionHandler2
    {
        private readonly IJavaScriptExecutionHandler _jsRuntime;

        public JSRuntimeWrapper(IJavaScriptExecutionHandler jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public void ExecuteJavaScript(string javaScriptToExecute)
            => _jsRuntime.ExecuteJavaScript(javaScriptToExecute);

        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
            => _jsRuntime.ExecuteJavaScriptWithResult(javaScriptToExecute);

        public TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0)
            => throw new NotSupportedException();
    }
}

#endif