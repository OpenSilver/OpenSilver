
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

using DotNetForHtml5;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CSHTML5.Internal
{
    internal interface IPendingJavascript
    {
        void AddJavaScript(string javascript);
        
        void Flush();

        object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult);
    }

    internal sealed class PendingJavascript : IPendingJavascript
    {
        private const string CallJSMethodNameAsync = "callJSUnmarshalledHeap";
        private const string CallJSMethodNameSync = "callJSUnmarshalled";

        private static readonly Encoding DefaultEncoding = Encoding.Unicode;
        private static readonly byte[] Delimiter = DefaultEncoding.GetBytes(";\n");
        private readonly object _syncObj = new();
        private readonly IWebAssemblyExecutionHandler _webAssemblyExecutionHandler;
        private byte[] _buffer;
        private int _currentLength;

        public PendingJavascript(int bufferSize, IWebAssemblyExecutionHandler webAssemblyExecutionHandler)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentException("Buffer size can not be less or equal to 0");
            }

            _webAssemblyExecutionHandler = webAssemblyExecutionHandler ?? throw new ArgumentNullException(nameof(webAssemblyExecutionHandler));
            _buffer = new byte[bufferSize];
        }

        public void AddJavaScript(string javascript)
        {
            if (javascript == null) return;

            lock (_syncObj)
            {
                var maxByteCount = DefaultEncoding.GetMaxByteCount(javascript.Length);
                while (maxByteCount + _currentLength > _buffer.Length)
                {
                    IncreaseBuffer();
                }

                _currentLength += DefaultEncoding.GetBytes(javascript, 0, javascript.Length, _buffer, _currentLength);

                Buffer.BlockCopy(Delimiter, 0, _buffer, _currentLength, Delimiter.Length);
                _currentLength += Delimiter.Length;
            }
        }

        public void Flush() {
            if (_currentLength == 0)
                return;

            var curLength = _currentLength;
            _currentLength = 0;

            //Here we pass a reference to _buffer object and current length
            //Js will read data from the heap

            // everything that was appended with AddJavascript, nothing to return
            _webAssemblyExecutionHandler.InvokeUnmarshalled<byte[], int, object>(CallJSMethodNameAsync, _buffer, curLength);
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult) {
            // IMPORTANT: wantsResult is passed on to JS, so that it will know if it needs to pass anything back to us
            // (optimization, when we don't care for the result)
            var result = _webAssemblyExecutionHandler.InvokeUnmarshalled<string, int, bool, object>(CallJSMethodNameSync, javascript, referenceId, wantsResult);
            return wantsResult ? result : null;
        }

        private void IncreaseBuffer()
        {
            var currentBuffer = _buffer;
            _buffer = new byte[currentBuffer.Length * 2];
            currentBuffer.CopyTo(_buffer, 0);
        }
    }

    internal sealed class PendingJavascriptSimulator : IPendingJavascript
    {
        private readonly List<string> _pending = new();
        private readonly IJavaScriptExecutionHandler _jsExecutionHandler;

        public PendingJavascriptSimulator(IJavaScriptExecutionHandler jsExecutionHandler)
        {
            _jsExecutionHandler = jsExecutionHandler ?? throw new ArgumentNullException(nameof(jsExecutionHandler));
        }
        
        public void AddJavaScript(string javascript)
        {
            lock (_pending)
            {
                _pending.Add(javascript);
            }
        }

        public void Flush() {
            string javascript = ReadAndClearAggregatedPendingJavaScriptCode();
            if (!string.IsNullOrWhiteSpace(javascript)) 
                PerformActualInteropCall(javascript);
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult) {
            // FIXME see how to perform this -- just wrap everything in callScriptSafe
            return PerformActualInteropCall(javascript);
        }


        private object PerformActualInteropCall(string javaScriptToExecute)
        {
            if (INTERNAL_ExecuteJavaScript.EnableInteropLogging)
            {
                javaScriptToExecute = "//---- START INTEROP ----"
                    + Environment.NewLine
                    + javaScriptToExecute
                    + Environment.NewLine
                    + "//---- END INTEROP ----";
            }

            try
            {
                if (INTERNAL_ExecuteJavaScript.EnableInteropLogging)
                {
                    Debug.WriteLine(javaScriptToExecute);
                }

                return _jsExecutionHandler.ExecuteJavaScriptWithResult(javaScriptToExecute);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Unable to execute the following JavaScript code: " + Environment.NewLine + javaScriptToExecute, ex);
            }
        }

        internal string ReadAndClearAggregatedPendingJavaScriptCode()
        {
            lock (_pending)
            {
                if (_pending.Count == 0)
                    return null;

                _pending.Add(string.Empty);
                string aggregatedPendingJavaScriptCode = string.Join(";\n", _pending);
                _pending.Clear();
                return aggregatedPendingJavaScriptCode;
            }
        }
    }
}
