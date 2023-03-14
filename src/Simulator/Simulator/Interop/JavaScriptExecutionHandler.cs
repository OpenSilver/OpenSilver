

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using DotNetBrowser.Browser;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class JavaScriptExecutionHandler
    {
        private bool _webControlDisposed = false;
        private IBrowser _browser;
        private List<string> _fullLogOfExecutedJavaScriptCode = new List<string>();

        public JavaScriptExecutionHandler(IBrowser browser)
        {
            _browser = browser;
            browser.Disposed += WebControl_DisposeEvent;
        }

        private void WebControl_DisposeEvent(object sender, EventArgs e)
        {
            _webControlDisposed = true;
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed || javaScriptToExecute == null)
                return;
            if (OpenSilver.Simulator.SimulatorLauncher.Parameters.LogExecutedJavaScriptCode)
                _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            _ = _browser.MainFrame?.ExecuteJavaScript(javaScriptToExecute).Result;
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed || javaScriptToExecute == null)
                return null;
            if (OpenSilver.Simulator.SimulatorLauncher.Parameters.LogExecutedJavaScriptCode)
                _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            return _browser.MainFrame?.ExecuteJavaScript(javaScriptToExecute).Result;
        }


        public string FullLogOfExecutedJavaScriptCode
        {
            get
            {
                return
@"window.onCallBack = {
    OnCallbackFromJavaScript: function(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject)
    {
        // dummy function
    },
    OnCallbackFromJavaScriptError: function(idWhereCallbackArgsAreStored)
    {
        // dummy function
    }
};
"
                + string.Join("\n\n", _fullLogOfExecutedJavaScriptCode);
            }
        }
    }
}
