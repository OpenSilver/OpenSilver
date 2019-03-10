
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================




#if DISABLED_BECAUSE_REPLACED_WITH_XMLHTTPREQUEST_CLASS

#if UNIMPLEMENTED_MEMBERS
using System.Collections.Generic;
using System.Threading.Tasks;

using JSIL.Meta;

namespace System.ServiceModel
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

    public class HttpRequest
    {
        private const int _defaultTimeOut = 2000;

        private TaskCompletionSource<string> _tcs;
        private object _xmlHttpRequest;

        public Uri Uri { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }

        public HttpRequest()
        {
            this.Method = HttpMethod.GET.ToString();
            this.Headers = new Dictionary<string, string>();
        }

        public HttpRequest(Uri uri)
            : this()
        {
            this.Uri = uri;
        }

        public HttpRequest(string address)
            : this()
        {
            this.Uri = new Uri(address);
        }

        public string Execute()
        {
            if (!IsRunningInJavaScript())
                return null;

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
            if (!IsRunningInJavaScript())
                return null;

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

        [JSReplacement("new XMLHttpRequest()")]
        private object NewHttpRequest()
        {
            return null;
        }

        [JSReplacement("$xmlHttpRequest.timeout = $timeOut")]
        private void SetTimout(object xmlHttpRequest, int timeOut)
        {
        }

        [JSReplacement("$xmlHttpRequest.setRequestHeader($key, $header)")]
        private void SetRequestHeader(object xmlHttpRequest, string key, string header)
        {
        }

        [JSReplacement("$xmlHttpRequest.onload = $onLoad")]
        private void OnLoad(object xmlHttpRequest, Action<object> onLoad)
        {
        }

        [JSReplacement("$xmlHttpRequest.onprogress = $onProgress")]
        private void OnProgress(object xmlHttpRequest, Action<object> onProgress)
        {
        }

        [JSReplacement("$xmlHttpRequest.onerror = $onError")]
        private void OnError(object xmlHttpRequest, Action<object> onError)
        {
        }

        [JSReplacement("$xmlHttpRequest.onabort = $onAbort")]
        private void OnAbort(object xmlHttpRequest, Action<object> onAbort)
        {
        }        
        
        [JSReplacement("$xmlHttpRequest.ontimeout = $onTimeOut")]
        private void OnTimeOut(object xmlHttpRequest, Action<object> onTimeOut)
        {
        }

        [JSReplacement("$xmlHttpRequest.open($method, $address, $isAsync)")]
        private void Open(object xmlHttpRequest, string method, string address, bool isAsync)
        {
        }

        [JSReplacement("$xmlHttpRequest.send($body)")]
        private static void Send(object xmlHttpRequest, string body)
        {
        }

        [JSReplacement("$xmlHttpRequest.readyState")]
        private static XhrReadyState GetReadyState(object xmlHttpRequest)
        {
            return 0;
        }

        [JSReplacement("$xmlHttpRequest.status")]
        private static int GetStatus(object xmlHttpRequest)
        {
            return 0;
        }

        [JSReplacement("$xmlHttpRequest.responseText")]
        private static string GetResponse(object xmlHttpRequest)
        {
            return null;
        }

        [JSReplacement("true")]
        private static bool IsRunningInJavaScript()
        {
            return false;
        }
    }
}
#endif

#endif