

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


using System.Collections.Generic;
using OpenSilver.Simulator;

namespace OpenSilver.Simulator
{
    public class JavaScriptExecutionHandler
    {
        private bool _webControlDisposed = false;
        private string _lastExecutedJavaScriptCode;
        private List<string> _InteropsPartLog = new List<string>();
        private List<string> _InteropsContinuousLog = new List<string>();
        private bool _IsInteropsPartLogEnabled;
        public bool IsInteropsContinuousLogEnabled { get; set; }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed)
                return;
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            if (_IsInteropsPartLogEnabled)
                _InteropsPartLog.Add(javaScriptToExecute + ";");
            if (IsInteropsContinuousLogEnabled)
                _InteropsContinuousLog.Add(javaScriptToExecute + ";");

            SimBrowser.Instance.ExecuteScriptAsync(javaScriptToExecute);
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed)
                return null;
            _lastExecutedJavaScriptCode = javaScriptToExecute;
            if (_IsInteropsPartLogEnabled)
                _InteropsPartLog.Add(javaScriptToExecute + ";");

            if (IsInteropsContinuousLogEnabled)
                _InteropsContinuousLog.Add(javaScriptToExecute + ";");

            return SimBrowser.Instance.ExecuteScriptWithResult(javaScriptToExecute);
        }

        internal string GetLastExecutedJavaScriptCode()
        {
            return _lastExecutedJavaScriptCode;
        }

        public string InteropsPartLog
        {
            get
            {
                return string.Join("\n\n", _InteropsPartLog);
            }
        }
        public string InteropsContinuousLog
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
                    };"
                    + string.Join("\n\n", _InteropsContinuousLog);
            }
        }

        public void ClearInteropLog()
        {
            _InteropsPartLog.Clear();
        }

        public void StartInteropLogging()
        {
            _IsInteropsPartLogEnabled = true;
        }

        public void StopInteropLoggin()
        {
            _IsInteropsPartLogEnabled = false;
        }
    }
}
