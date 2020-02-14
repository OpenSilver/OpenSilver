
#if CSHTML5NETSTANDARD

using System;

namespace DotNetForHtml5
{
    public interface IJavaScriptExecutionHandler
    {
        void ExecuteJavaScript(string javaScriptToExecute);

        object ExecuteJavaScriptWithResult(string javaScriptToExecute);
    }
}

#endif