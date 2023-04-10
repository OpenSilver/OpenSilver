
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
using OpenSilver;

namespace CSHTML5.Internal
{
    internal interface IPendingJavascript
    {
        void AddJavaScript(string javascript);

        void Flush();

        object ExecuteJavaScriptFunc<T>(string funcName, T arg0);
        object ExecuteJavaScriptFunc<T0, T1>(string funcName, T0 arg0, T1 arg1);
        object ExecuteJavaScriptFunc<T0, T1, T2>(string funcName, T0 arg0, T1 arg1, T2 arg2);

        object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult);
    }

    internal sealed class PendingJavascript : IPendingJavascript
    {
        private const string CallJSMethodNameAsync = "callJSUnmarshalledHeap";
        private const string CallJSMethodNameSync = "callJSUnmarshalled";

        private static readonly Encoding DefaultEncoding = Encoding.Unicode;
        private static readonly byte[] Delimiter = DefaultEncoding.GetBytes(";\n");
        private readonly IWebAssemblyExecutionHandler _webAssemblyExecutionHandler;
        private readonly byte[] _buffer;
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

            // the idea:
            // when we have async calls, we need to have them executed sequentially
            // however, this gets complicated when we're trying to do this with both Interop. and Interop2. calls,
            // since Interop2. calls are executed one by one, while Interop. calls are appended to a byte-array and executed all at once
            //
            // so in order to maintain this sequential behavior, when we execute an Interop2 Async call, we flush the Interop. queue,
            // and vice versa
            Interop2.Flush();

            // the idea -- the current buffer is already huge - in the very rare scenario it would get full, just flush it and restart
            var maxByteCount = DefaultEncoding.GetMaxByteCount(javascript.Length) + Delimiter.Length;
            if (maxByteCount + _currentLength > _buffer.Length)
                Flush();

            if (maxByteCount > _buffer.Length)
                throw new Exception($"javascript too big! {maxByteCount}");

            _currentLength += DefaultEncoding.GetBytes(javascript, 0, javascript.Length, _buffer, _currentLength);

            Buffer.BlockCopy(Delimiter, 0, _buffer, _currentLength, Delimiter.Length);
            _currentLength += Delimiter.Length;
        }

        public void Flush()
        {
            if (_currentLength == 0)
                return;

            var curLength = _currentLength;
            _currentLength = 0;

            //Here we pass a reference to _buffer object and current length
            //Js will read data from the heap

            // everything that was appended with AddJavascript, nothing to return
            _webAssemblyExecutionHandler.InvokeUnmarshalled<byte[], int, object>(CallJSMethodNameAsync, _buffer, curLength);
        }

        public object ExecuteJavaScriptFunc<T>(string funcName, T arg0) {
            var result = _webAssemblyExecutionHandler.InvokeUnmarshalled<T, object>(funcName, arg0);
            return result;
        }
        public object ExecuteJavaScriptFunc<T0,T1>(string funcName, T0 arg0, T1 arg1) {
            var result = _webAssemblyExecutionHandler.InvokeUnmarshalled<T0, T1, object>(funcName, arg0, arg1);
            return result;
        }
        public object ExecuteJavaScriptFunc<T0,T1,T2>(string funcName, T0 arg0, T1 arg1, T2 arg2) {
            var result = _webAssemblyExecutionHandler.InvokeUnmarshalled<T0, T1, T2, object>(funcName, arg0, arg1, arg2);
            return result;
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult)
        {
            // IMPORTANT: wantsResult is passed on to JS, so that it will know if it needs to pass anything back to us
            // (optimization, when we don't care for the result)
            var result = _webAssemblyExecutionHandler.InvokeUnmarshalled<string, int, bool, object>(CallJSMethodNameSync, javascript, referenceId, wantsResult);
            return wantsResult ? result : null;
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
            if (javascript == null)
                return;

            // the idea:
            // when we have async calls, we need to have them executed sequentially
            // however, this gets complicated when we're trying to do this with both Interop. and Interop2. calls,
            // since Interop2. calls are executed one by one, while Interop. calls are appended to a byte-array and executed all at once
            //
            // so in order to maintain this sequential behavior, when we execute an Interop2 Async call, we flush the Interop. queue,
            // and vice versa
            Interop2.Flush();

            lock (_pending)
            {
                _pending.Add(javascript);
            }
        }

        public void Flush()
        {
            string javascript = ReadAndClearAggregatedPendingJavaScriptCode();
            if (!string.IsNullOrWhiteSpace(javascript))
                PerformActualInteropCallVoid(javascript);
        }

        public object ExecuteJavaScriptFunc<T>(string funcName, T arg0) {
            // FIXME
            return null;
        }
        public object ExecuteJavaScriptFunc<T0,T1>(string funcName, T0 arg0, T1 arg1) {
            // FIXME
            return null;
        }
        public object ExecuteJavaScriptFunc<T0,T1,T2>(string funcName, T0 arg0, T1 arg1, T2 arg2) {
            // FIXME
            return null;
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult)
        {
            if (referenceId > 0 && !javascript.StartsWith("document.callScriptSafe"))
                javascript = INTERNAL_ExecuteJavaScript.WrapReferenceIdInJavascriptCall(javascript, referenceId);

            if (wantsResult)
                return PerformActualInteropCall(javascript);
            else
            {
                PerformActualInteropCallVoid(javascript);
                return null;
            }
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

        private void PerformActualInteropCallVoid(string javaScriptToExecute)
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

                _jsExecutionHandler.ExecuteJavaScript(javaScriptToExecute);
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
