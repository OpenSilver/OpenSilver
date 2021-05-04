using DotNetForHtml5;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;

namespace $safeprojectname$.Interop
{
    public class UnmarshalledJavaScriptExecutionHandler : IJavaScriptExecutionHandler
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
    }
}