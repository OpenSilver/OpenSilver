using DotNetForHtml5;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;

namespace TestApplication.OpenSilver.Browser.Interop
{
    public class UnmarshalledJavaScriptExecutionHandler : IWebAssemblyExecutionHandler
    {
        private const string MethodName = "callJSUnmarshalled";
        private readonly WebAssemblyJSRuntime _runtime;

        public UnmarshalledJavaScriptExecutionHandler(IJSRuntime runtime)
        {
            _runtime = runtime as WebAssemblyJSRuntime;
        }

        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            _runtime.InvokeUnmarshalled<string, object>(MethodName, javaScriptToExecute);
        }

        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            return _runtime.InvokeUnmarshalled<string, object>(MethodName, javaScriptToExecute);
        }

        public TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0)
        {
            return _runtime.InvokeUnmarshalled<T0, TResult>(identifier, arg0);
        }

        public TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1)
        {
            return _runtime.InvokeUnmarshalled<T0, T1, TResult>(identifier, arg0, arg1);
        }

        public TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2)
        {
            return _runtime.InvokeUnmarshalled<T0, T1, T2, TResult>(identifier, arg0, arg1, arg2);
        }
    }
}