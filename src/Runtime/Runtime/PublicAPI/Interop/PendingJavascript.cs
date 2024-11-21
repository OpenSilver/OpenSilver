
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

using System;
using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using DotNetForHtml5;
using OpenSilver.Buffers;

namespace CSHTML5.Internal
{
    internal interface IPendingJavascript
    {
        void AcquireLock();

        void ReleaseLock();

        void Flush();

        object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult);
    }

    internal sealed class PendingJavascript : IPendingJavascript
    {
        private readonly INativeMethods _nativeMethods;
        private readonly CharArrayBuilder _charArrayBuilder;

        private byte[] _buffer = Array.Empty<byte>();

        public PendingJavascript(INativeMethods nativeMethods, CharArrayBuilder buffer)
        {
            _nativeMethods = nativeMethods ?? throw new ArgumentNullException(nameof(nativeMethods));
            _charArrayBuilder = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        public void AcquireLock() { }

        public void ReleaseLock() { }

        public void Flush()
        {
            int minLength = Encoding.Unicode.GetMaxByteCount(_charArrayBuilder.Length);

            if (_buffer.Length < minLength)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = ArrayPool<byte>.Shared.Rent(minLength);
            }

            int bytesLength = Encoding.Unicode.GetBytes(_charArrayBuilder.Buffer, 0, _charArrayBuilder.Length, _buffer, 0);
            _charArrayBuilder.Reset();

            _nativeMethods.InvokePendingJS(_buffer, bytesLength);
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult)
        {
            // IMPORTANT: wantsResult is passed on to JS, so that it will know if it needs to pass anything back to us
            // (optimization, when we don't care for the result)
            var result = _nativeMethods.InvokeJS(javascript, referenceId, wantsResult);
            return wantsResult ? result : null;
        }
    }

    internal sealed class PendingJavascriptSimulator : IPendingJavascript
    {
        private readonly IJavaScriptExecutionHandler _jsExecutionHandler;
        private readonly object _sync = new();
        private readonly CharArrayBuilder _charArrayBuilder;

        public PendingJavascriptSimulator(IJavaScriptExecutionHandler jsExecutionHandler, CharArrayBuilder buffer)
        {
            _jsExecutionHandler = jsExecutionHandler ?? throw new ArgumentNullException(nameof(jsExecutionHandler));
            _charArrayBuilder = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        public void AcquireLock() => Monitor.Enter(_sync);

        public void ReleaseLock() => Monitor.Exit(_sync);

        public void Flush()
        {
            string javascript = ReadAndClearAggregatedPendingJavaScriptCode();
            if (!string.IsNullOrWhiteSpace(javascript))
            {
                PerformActualInteropCallVoid(javascript);
            }
        }

        public object ExecuteJavaScript(string javascript, int referenceId, bool wantsResult)
        {
            if (referenceId > 0 && !javascript.StartsWith("document.callScriptSafe"))
            {
                javascript = OpenSilver.Interop.WrapReferenceIdInJavascriptCall(javascript, referenceId);
            }

            if (wantsResult)
            {
                return PerformActualInteropCall(javascript);
            }
            else
            {
                PerformActualInteropCallVoid(javascript);
                return null;
            }
        }

        private object PerformActualInteropCall(string javaScriptToExecute)
        {
            if (OpenSilver.Interop.EnableInteropLogging)
            {
                javaScriptToExecute = "//---- START INTEROP ----"
                    + Environment.NewLine
                    + javaScriptToExecute
                    + Environment.NewLine
                    + "//---- END INTEROP ----";
            }

            try
            {
                if (OpenSilver.Interop.EnableInteropLogging)
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
            if (OpenSilver.Interop.EnableInteropLogging)
            {
                javaScriptToExecute = "//---- START INTEROP ----"
                                      + Environment.NewLine
                                      + javaScriptToExecute
                                      + Environment.NewLine
                                      + "//---- END INTEROP ----";
            }

            try
            {
                if (OpenSilver.Interop.EnableInteropLogging)
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
            try
            {
                AcquireLock();
                return _charArrayBuilder.ToStringAndClear();
            }
            finally
            {
                ReleaseLock();
            }
        }
    }
}
