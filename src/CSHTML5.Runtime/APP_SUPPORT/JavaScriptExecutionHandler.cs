using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Web;

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
            object result = null;
            result = ((JSInProcessRuntime)JSRuntime).Invoke<object>("callJS", javaScriptToExecute);
            return result;
        }
    }
}
