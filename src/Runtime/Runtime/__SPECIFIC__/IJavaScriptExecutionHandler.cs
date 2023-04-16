
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
using System.ComponentModel;
using OpenSilver.Internal;

namespace DotNetForHtml5
{
    public interface IJavaScriptExecutionHandler
    {
        void ExecuteJavaScript(string javaScriptToExecute);

        object ExecuteJavaScriptWithResult(string javaScriptToExecute);
    }

    public interface IWebAssemblyExecutionHandler : IJavaScriptExecutionHandler
    {
        TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0);
        TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1);
        TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use DotNetForHtml5.IWebAssemblyExecutionHandler instead.", true)]
    public interface IJavaScriptExecutionHandler2 : IJavaScriptExecutionHandler
    {
        TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0);
    }

    internal sealed class JSRuntimeWrapper : IWebAssemblyExecutionHandler
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

        public TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1)
            => throw new NotSupportedException();

        public TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2)
            => throw new NotSupportedException();
    }

    internal sealed class SimulatorDynamicJSRuntime : IWebAssemblyExecutionHandler
    {
        private readonly dynamic _dynamicRuntime;

        public SimulatorDynamicJSRuntime(dynamic dynamicRuntime)
        {
            _dynamicRuntime = dynamicRuntime ?? throw new ArgumentNullException(nameof(dynamicRuntime));
        }

        public void ExecuteJavaScript(string javaScriptToExecute)
            => _dynamicRuntime.ExecuteJavaScript(javaScriptToExecute);

        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
            => _dynamicRuntime.ExecuteJavaScriptWithResult(javaScriptToExecute);

        public TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0)
            => throw new NotImplementedException();

        public TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1)
            => throw new NotImplementedException();

        public TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2)
            => throw new NotImplementedException();
    }
}

#endif