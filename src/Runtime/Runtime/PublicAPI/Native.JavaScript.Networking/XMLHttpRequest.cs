

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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSHTML5.Native.JavaScript.Networking
{
    //todo: implement cors (see jsil doxhr)
    //todo: binary response

    internal enum HttpMethod
    {
        GET, POST, PUT, DELETE
    }

    internal enum XhrReadyState
    {
        UNSENT,             //0: Client has been created. open() not called yet.
        OPENED,             //1: open() has been called.
        HEADERS_RECEIVED,   //2: send() has been called, and headers and status are available.
        LOADING,	        //3: Downloading; responseText holds partial data.
        DONE,               //4: The operation is complete.
    }

    public class XMLHttpRequest
    {
        private const int _defaultTimeOut = 2000;

        private TaskCompletionSource<string> _tcs;
        private object _xmlHttpRequest;

        public Uri Uri { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }

        public XMLHttpRequest()
        {
            this.Method = HttpMethod.GET.ToString();
            this.Headers = new Dictionary<string, string>();
        }

        public XMLHttpRequest(Uri uri)
            : this()
        {
            this.Uri = uri;
        }

        public XMLHttpRequest(string address)
            : this()
        {
            this.Uri = new Uri(address);
        }

        public string Execute()
        {
            object xhr = NewHttpRequest();

            Open(xhr, this.Method, this.Uri.ToString(), isAsync: false);
            foreach (var kv in this.Headers)
                SetRequestHeader(xhr, kv.Key, kv.Value);

            Send(xhr, this.Body);

            CheckStatusAndThrow(xhr);

            object responseObject = GetResponse(xhr);
            return responseObject != null ? responseObject.ToString() : null;
        }

        public Task<string> ExecuteAsync(int timeOut = _defaultTimeOut)
        {
            _tcs = new TaskCompletionSource<string>();

            _xmlHttpRequest = NewHttpRequest();

            OnLoad(_xmlHttpRequest, InternalOnLoad);
            OnProgress(_xmlHttpRequest, InternalOnProgress);
            OnError(_xmlHttpRequest, InternalOnError);
            OnAbort(_xmlHttpRequest, InternalOnAbort);
            OnTimeOut(_xmlHttpRequest, InternalOnTimeOut);

            Open(_xmlHttpRequest, this.Method, this.Uri.ToString(), isAsync: true);
            foreach (var kv in this.Headers)
                SetRequestHeader(_xmlHttpRequest, kv.Key, kv.Value);

            SetTimout(_xmlHttpRequest, timeOut);
            Send(_xmlHttpRequest, this.Body);

            return _tcs.Task;
        }

        private void InternalOnProgress(object e)
        {
            //TODO
        }

        private void InternalOnLoad(object e)
        {
            var exception = CheckStatus(_xmlHttpRequest);
            if (exception == null)
            {
                object responseObject = GetResponse(_xmlHttpRequest);
                var response = responseObject != null ? responseObject.ToString() : null;
                _tcs.SetResult(response);
            }
            else
            {
                _tcs.SetException(exception);
            }
        }

        private void InternalOnError(object e)
        {
            var exception = CheckStatus(_xmlHttpRequest);
            if (exception == null)
                exception = new NotSupportedException("This error type is not yet supported!");
            _tcs.SetException(exception);
        }

        private void InternalOnAbort(object e)
        {
            var exception = CheckStatus(_xmlHttpRequest);
            if (exception == null)
                exception = new NotSupportedException("This abort type is not yet supported!");
            _tcs.SetException(exception);
        }

        private void InternalOnTimeOut(object e)
        {
            var exception = new TimeoutException();
            _tcs.SetException(exception);
        }

        private void CheckStatusAndThrow(object xhr)
        {
            var exception = CheckStatus(xhr);
            if (exception != null)
                throw exception;
        }

        private Exception CheckStatus(object xhr)
        {
            //TODO: test and implement all cases
            var readState = GetReadyState(xhr);
            var status = GetStatus(xhr);
            if (status == 0)
            {
                return new Exception("no response from server or error");
            }
            if (status == 404)
            {
                return new Exception("there was a response from the server; it was a 404 meaning resource not found");
            }
            if (status >= 500)
            {
                return new Exception("there was a response from the server; it was a " + status + " meaning an error occured on the server");
            }
            if (status != 200)
            {
                return new Exception("there was a response from the server; it was a " + status);
            }
            switch (readState)
            {
                case XhrReadyState.UNSENT:
                    return new Exception("The request is not initialized!");
                case XhrReadyState.OPENED:
                    return new Exception("An error occured. Cross-Site Http Request might not be allowed at the target Url. If you own the domain of the Url, consider adding the header \"Access-Control-Allow-Origin\" to enable requests to be done at this Url.");
                case XhrReadyState.HEADERS_RECEIVED:
                case XhrReadyState.LOADING:
                    return new Exception("An error occured while submitting your request.");
                case XhrReadyState.DONE:
                    //OK
                    break;
            }
            return null;
        }

        private object NewHttpRequest() => OpenSilver.Interop.ExecuteJavaScript("new XMLHttpRequest()");

        private void SetTimout(object xmlHttpRequest, int timeOut)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sTimeout = OpenSilver.Interop.GetVariableStringForJS(timeOut);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.timeout = {sTimeout}");
        }

        private void SetRequestHeader(object xmlHttpRequest, string key, string header)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sKey = OpenSilver.Interop.GetVariableStringForJS(key);
            string sHeader = OpenSilver.Interop.GetVariableStringForJS(header);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.setRequestHeader({sKey}, {sHeader})");
        }

        private void OnLoad(object xmlHttpRequest, Action<object> onLoad)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = OpenSilver.Interop.GetVariableStringForJS(onLoad);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onload = {sCallback}");
        }

        private void OnProgress(object xmlHttpRequest, Action<object> onProgress)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = OpenSilver.Interop.GetVariableStringForJS(onProgress);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onprogress = {sCallback}");
        }

        private void OnError(object xmlHttpRequest, Action<object> onError)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = OpenSilver.Interop.GetVariableStringForJS(onError);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onerror = {sCallback}");
        }

        private void OnAbort(object xmlHttpRequest, Action<object> onAbort)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = OpenSilver.Interop.GetVariableStringForJS(onAbort);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onabort = {sCallback}");
        }

        private void OnTimeOut(object xmlHttpRequest, Action<object> onTimeOut)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = OpenSilver.Interop.GetVariableStringForJS(onTimeOut);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.ontimeout = {sCallback}");
        }

        private void Open(object xmlHttpRequest, string method, string address, bool isAsync)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sMethod = OpenSilver.Interop.GetVariableStringForJS(method);
            string sAddress = OpenSilver.Interop.GetVariableStringForJS(address);
            string sAsync = OpenSilver.Interop.GetVariableStringForJS(isAsync);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.open({sMethod}, {sAddress}, {sAsync})");
        }

        private static void Send(object xmlHttpRequest, string body)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            string sBody = OpenSilver.Interop.GetVariableStringForJS(body ?? "");
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.send({sBody})");
        }

        private static XhrReadyState GetReadyState(object xmlHttpRequest)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            return (XhrReadyState)OpenSilver.Interop.ExecuteJavaScriptInt32($"{sRequest}.readyState");
        }

        private static int GetStatus(object xmlHttpRequest)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sRequest}.status");
        }

        private static string GetResponse(object xmlHttpRequest)
        {
            string sRequest = OpenSilver.Interop.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptString($"{sRequest}.responseText");
        }
    }
}
