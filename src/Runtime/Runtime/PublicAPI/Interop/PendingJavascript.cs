
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

        object ExecuteJavaScript(string javascript, bool flush);
    }

    internal sealed class PendingJavascript : IPendingJavascript
    {
        private const string CallJSMethodName = "callJSUnmarshalledHeap";

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

        public object ExecuteJavaScript(string javascript, bool flush)
        {
            if (!flush)
            {
                return _webAssemblyExecutionHandler.ExecuteJavaScriptWithResult(javascript);
            }

            AddJavaScript(javascript);

            if (_currentLength == 0)
            {
                return null;
            }

            var curLength = _currentLength;
            _currentLength = 0;
            //Here we pass a reference to _buffer object and current length
            //Js will read data from the heap
            return _webAssemblyExecutionHandler.InvokeUnmarshalled<byte[], int, object>(
                CallJSMethodName, _buffer, curLength);
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

        public object ExecuteJavaScript(string javascript, bool flush)
        {
            if (flush)
            {
                string aggregatedPendingJavaScriptCode = ReadAndClearAggregatedPendingJavaScriptCode();

                if (!string.IsNullOrWhiteSpace(aggregatedPendingJavaScriptCode))
                {
                    javascript = string.Join(Environment.NewLine, new List<string>
                    {
                        "// [START OF PENDING JAVASCRIPT]",
                        aggregatedPendingJavaScriptCode,
                        "// [END OF PENDING JAVASCRIPT]" + Environment.NewLine,
                        javascript
                    });
                }
            }

            return PerformActualInteropCall(javascript);
        }

        private object PerformActualInteropCall(string javaScriptToExecute)
        {
            if (INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging)
            {
                javaScriptToExecute = "//---- START INTEROP ----"
                    + Environment.NewLine
                    + javaScriptToExecute
                    + Environment.NewLine
                    + "//---- END INTEROP ----";
            }

            try
            {
                if (INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging)
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
