

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



using DotNetBrowser.WPF;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class JavaScriptExecutionHandler
    {
        WPFBrowserView _webControl;
        string _lastExecutedJavaScriptCode;
        List<string> _fullLogOfExecutedJavaScriptCode = new List<string>();

        public JavaScriptExecutionHandler(WPFBrowserView webControl)
        {
            _webControl = webControl;
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControl.IsDisposed || _webControl.Browser.IsDisposed())
                return;
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            _webControl.Browser.ExecuteJavaScript(javaScriptToExecute);
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControl.IsDisposed || _webControl.Browser.IsDisposed())
                return null;
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            return _webControl.Browser.ExecuteJavaScriptAndReturnValue(javaScriptToExecute);
        }


        internal string GetLastExecutedJavaScriptCode()
        {
            return _lastExecutedJavaScriptCode;
        }

        public string FullLogOfExecutedJavaScriptCode
        {
            get
            {
#if OPENSILVER
                // Note: we use a better solution with OpenSilver: as with ES6 and template literals we can make multiline comments, we can better format interops (particularly stuff in eval() statements).
                // We cannot use this solution with CSHTML5 as IE11 does not supports ES6
                
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(@"
window.onCallBack = {
    OnCallbackFromJavaScript: function(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject)
    {
        // dummy function
    },
    OnCallbackFromJavaScriptError: function(idWhereCallbackArgsAreStored)
    {
        // dummy function
    }
};");
                
                foreach (string jsCode in _fullLogOfExecutedJavaScriptCode)
                {
                    using (StringReader strReader = new StringReader(jsCode))
                    {
                        string line;
                        while ((line = strReader.ReadLine()) != null)
                        {
                            line = line.Replace("\\r", "\r");
                            line = line.Replace("\\n", "\n");
                            line = line.Replace("\\t", "\t");
                            stringBuilder.AppendLine(line);
                        }
                    }
                    
                    stringBuilder.AppendLine();
                }
                return stringBuilder.ToString();
#else
                // Note: the option to remove the callsbacks arguments code is useful when exporting the JS code via the "Debug JS code" feature of the Simulator, because such code does not exist when running outside the Simulator, due to the fact that they are created in the event handlers, and those are not executed outside the Simulator. If we did not remove this code, we would get errors saying that objects like document.jsSimulatorObjectReferences["args481089"] are not defined.
                
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
#endif
            }
        }
    }
}
