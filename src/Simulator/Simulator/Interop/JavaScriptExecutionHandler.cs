

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
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            _webControl.Browser.ExecuteJavaScript(javaScriptToExecute);
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);
            return _webControl.Browser.ExecuteJavaScriptAndReturnValue(javaScriptToExecute);
        }


        internal string GetLastExecutedJavaScriptCode()
        {
            return _lastExecutedJavaScriptCode;
        }

        internal string GetFullLogOfExecutedJavaScriptCode(bool removeCallbacksArgumentsCode = false)
        {
            // Note: the option to remove the callsbacks arguments code is useful when exporting the JS code via the "Debug JS code" feature of the Simulator, because such code does not exist when running outside the Simulator, due to the fact that they are created in the event handlers, and those are not executed outside the Simulator. If we did not remove this code, we would get errors saying that objects like document.jsSimulatorObjectReferences["args481089"] are not defined.

            StringBuilder stringBuilder = new StringBuilder();
            foreach (string jsCode in _fullLogOfExecutedJavaScriptCode)
            {
                if (removeCallbacksArgumentsCode)
                {
                    using (StringReader strReader = new StringReader(jsCode))
                    {
                        string line;
                        while ((line = strReader.ReadLine()) != null)
                        {
                            if (!line.Contains(@"callback_args_"))
                                stringBuilder.AppendLine(line);
                            else
                                stringBuilder.AppendLine("var result = ''; // Callback removed");
                        }
                    }
                }
                else
                {
                    stringBuilder.AppendLine(jsCode);
                }
            }
            return stringBuilder.ToString();
        }

        public string FullLogOfExecutedJavaScriptCode
        {
            get
            {
                return GetFullLogOfExecutedJavaScriptCode(true);
            }
        }
    }
}
