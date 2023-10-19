
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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using CSHTML5;
using CSHTML5.Internal;

namespace System
{
    internal sealed class INTERNAL_WebRequestHelper_JSOnly
    {
        object _xmlHttpRequest;

        /// <summary>
        /// Occurs when the string download is completed.
        /// </summary>
        public event INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler DownloadStringCompleted;

        /// <summary>
        /// Initializes a new instance of the INTERNAL_WebRequestHelper class.
        /// </summary>
        public INTERNAL_WebRequestHelper_JSOnly() { }

        static Dictionary<string, bool> WebServiceUrlToJsCredentialsSupported = new Dictionary<string, bool>();

        // copy the parameters for resending method in case of fail
        Uri _address;
        string _Method;
        static object _sender;
        Dictionary<string, string> _headers;
        string _body;
        bool _isAsync;
        INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler _callback;
        static INTERNAL_WebRequestHelper_JSOnly _requester;

        /// <summary>
        /// Makes a synchronous or asynchronous request at the specified uri, using the specified method, with the specified headers and body, and calls the callbackMethod.
        /// </summary>
        /// <param name="address">the uri that identifies the Internet resource.</param>
        /// <param name="Method">The method to be called after making the request.</param>
        /// <param name="sender"></param>
        /// <param name="headers">
        /// A dictionary containing the headers to put in the method.
        /// The pairs key/values in the dictionary correspond to the pairs key/value in the headers.
        /// </param>
        /// <param name="body">The body of the request.</param>
        /// <param name="callbackMethod">The method to be called after the request has been made.</param>
        /// <param name="isAsync">A boolean that determines whether the request must be made synchronously or asynchronously.</param>
        /// <param name="mode"></param>
        /// <returns>The result of the request as a string.</returns>
        public string MakeRequest(
            Uri address, 
            string Method, 
            object sender, 
            Dictionary<string, string> headers, 
            string body, 
            INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler callbackMethod, 
            bool isAsync, 
            CredentialsMode mode = CredentialsMode.Disabled)
        {
            if (Application.Current.Host.Settings.EnableWebRequestsLogging)
            {
                string headersCode = headers != null && headers.Count > 0 ? "new global::System.Collections.Generic.Dictionary<string, string> { " + string.Join(", ", headers.Select(keyValuePair => "{ " + EscapeStringAndSurroundWithQuotes(keyValuePair.Key) + ", " + EscapeStringAndSurroundWithQuotes(keyValuePair.Value) + " }")) + " }" : "null";

                Debug.WriteLine(string.Format("CSHTML5.Internal.WebRequestsHelper.MakeRequest({0}, {1}, {2}, {3}, {4});",
                    EscapeStringAndSurroundWithQuotes(address.ToString()),
                    EscapeStringAndSurroundWithQuotes(Method),
                    EscapeStringAndSurroundWithQuotes(body),
                    headersCode,
                    "false"
                    ));
            }

            bool askForUnsafeRequest = false; // This is true if we are doing the initial request to determine whether the credentials are supported or not.

            _xmlHttpRequest = GetWebRequest();

            //define the action to do when the xmlhttp has finished the request:
            if (callbackMethod != null)
            {
                DownloadStringCompleted -= callbackMethod;
                DownloadStringCompleted += callbackMethod;
            }
            SetCallbackMethod((object)_xmlHttpRequest, OnDownloadStringCompleted);

            //create the request:
            CreateRequest((object)_xmlHttpRequest, address.OriginalString, Method, isAsync);

            if (!WebServiceUrlToJsCredentialsSupported.ContainsKey(address.OriginalString))
            {
                if (mode == CredentialsMode.Auto)
                {
                    WebServiceUrlToJsCredentialsSupported.Add(address.OriginalString, true); // if not supported, this value will be changed soon
                    askForUnsafeRequest = true;
                }
                else
                {
                    WebServiceUrlToJsCredentialsSupported.Add(address.OriginalString, mode == CredentialsMode.Disabled ? false : true);
                }
            }

            // XHR does not allow to read the 'Set-Cookie' header and to write the 
            // 'cookie' header. The command withCredentials = true must be use to
            // automatically handle cookies. But from server side Access-control-orginal must not be '*'
            // so some modifications must be done from server side too.
            //https://developer.mozilla.org/fr/docs/Web/HTTP/CORS
            // With credentials, the 'Origin' header must not be null, but XHR does not allow direct modifications
            // on "Origin" and running the app from the file always set the Origin to null:
            // to test credential, make sure to run it in local server
            // headers.Add("Origin", "http://Something.com/");
            EnableCookies((object)_xmlHttpRequest, WebServiceUrlToJsCredentialsSupported[address.OriginalString]);

            if (headers != null && headers.Count > 0)
            {
                foreach (string key in headers.Keys)
                {
                    SetRequestHeader((object)_xmlHttpRequest, key, headers[key]);
                }
            }

            if (askForUnsafeRequest) // if the settings of the request are still unsafe
            {
                // handle special errors especially crash in pre flight, that GetHasError doesn't catch
                SetErrorCallback((object)_xmlHttpRequest, OnError);

                // save the inputs to resend the request in case of error
                SaveParameters(address, Method, sender, headers, callbackMethod, body, isAsync);

                // safe request, will resend the request with different settings if it crashes.
                return SendUnsafeRequest((object)_xmlHttpRequest, address.OriginalString, Method, isAsync, body);
            }
            else
            {
                SendRequest((object)_xmlHttpRequest, address.OriginalString, Method, isAsync, body);
            }

            string result = GetResult((object)_xmlHttpRequest);

            if (GetHasError((object)_xmlHttpRequest))
            {
                if (result.IndexOf(":Fault>") == -1) // We make a special case to not consider FaultExceptions as a server Internal error. The error will be handled later in CSHTML5_ClientBase.WebMethodsCaller.ReadAndPrepareResponse
                {
	                throw new Exception("The remote server has returned an error: (" + GetCurrentStatus((object)_xmlHttpRequest) + ") " + GetCurrentStatusText((object)_xmlHttpRequest) + ".");
                }
            }
            else
            {
                //we check whether the server could be found at all. It is not definite that readyState = 4 and status = 0 means that the server could not be found but that's the only example I have met so far and we're a bit poor on informations anyway.
                int currentReadyState = GetCurrentReadyState((object)_xmlHttpRequest);
                int currentStatus = GetCurrentStatus((object)_xmlHttpRequest);
                if ((currentStatus == 0 && !GetIsFileProtocol()) && (currentReadyState == 4 || currentReadyState == 1)) //we could replace that "if" with a method since it is used in SetEventArgs. //Note: see note on the same test in SetEventArgs
                {
                    if (!isAsync) // Note: we only throw the exception when the call is not asynchronous because it is dealt with in the callback (defined by SetCallbackMethod and automatically called by SendRequest).
                    {
                        throw new Exception("An error occured. Please make sure that the target URL is available: " + address.ToString());
                    }
                }
            }

            //get the response:
            return result;
        }

        private static string EscapeStringAndSurroundWithQuotes(string str)
        {
            return "@\"" + (str ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }

        // special version of sendRequest, it handles some errors and modifies the credentials mode if needed
        // return directly the result of the right response
        private string SendUnsafeRequest(object xmlHttpRequest, string address, string method, bool isAsync, string body)
        {
            ConsoleLog_JSOnly("CredentialsMode is set to Auto: if a preflight error appears below, please ignore it.");

            if (!isAsync) // if synchronous mode, we must handle any problem in the request just after it happens
            {
                try
                {
                    SendRequest((object)_xmlHttpRequest, address, method, isAsync, body);

                    ResendRequestInCaseOfPreflightError(false);
                    return GetResult((object)_xmlHttpRequest);
                }
                catch
                {
                    // try new credentials settings and resend this request
                    if (IsCrashInPreflight)
                    {
                        return ResendRequestInCaseOfPreflightError(true);
                    }
                    else
                    {
                        ResendRequestInCaseOfPreflightError(false);
                        return GetResult((object)_xmlHttpRequest); // normally, in this method, crash are due to preflight errors, we are not suppose to arrive here
                    }
                }
            }
            else
            {
                // in asynchronous mode, the error callback will directly arrive in OnError, and it will resend this request
                _isFirstTryAtSendingUnsafeRequest = true;
                SendRequest((object)_xmlHttpRequest, address, method, isAsync, body);
                return GetResult((object)_xmlHttpRequest);
            }
        }

        bool _isFirstTryAtSendingUnsafeRequest = false;

        // if the last request was asynchronous, any error will be cought here.
        private void OnError()
        {
            if (IsCrashInPreflight)
                ResendRequestInCaseOfPreflightError(true);
        }

        // if the last request has crashed, we modify the settings and we resend the last request
        // the return result is only useful in synchronous mode
        private string ResendRequestInCaseOfPreflightError(bool error)
        {
            string newResult = string.Empty;

            if (error)
            {
                WebServiceUrlToJsCredentialsSupported[_address.OriginalString] = false; // not supported

                // resend the request with the new setting
                newResult = ResendLastUnsafeRequest();

                ConsoleLog_JSOnly("The requested server does not seem to accept credentials. To stop getting the error above, make sure to set CredentialsMode to Disabled. To do so with REST calls, please set the property WebClientWithCredentials.CredentialsMode to Disabled. For SOAP calls, please place the following code in your application constructor: Application.Current.Host.Settings.DefaultSoapCredentialsMode = System.Net.CredentialsMode.Disabled;");
            }

            ConsoleLog_JSOnly("Credentials status is now confirmed: no other preflight errors are expected for this request.");

            return newResult;
        }

        private bool IsCrashInPreflight
        {
            get
            {
                //CurrentReadyState = 4 and CurrentStatus = 0 (unsend) seem to mean error in preflight, but no more informations
                return (GetCurrentReadyState((object)_xmlHttpRequest) == 4 && GetCurrentStatus((object)_xmlHttpRequest) == 0);
            }
        }

        private static string ResendLastUnsafeRequest()
        {
            // we need to recreate a webRequestHelper, beacause we can't modify settings after the request was send
            return new INTERNAL_WebRequestHelper_JSOnly().MakeRequest(
                _requester._address, 
                _requester._Method, 
                _sender, 
                _requester._headers, 
                _requester._body, 
                _requester._callback, 
                _requester._isAsync, 
                CredentialsMode.Disabled);
        }

        private void SaveParameters(
            Uri address, 
            string Method, 
            object sender, 
            Dictionary<string, string> headers, 
            INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler callback, 
            string body, 
            bool isAsync)
        {
            _address = address;
            _Method = Method;
            _sender = sender;
            _headers = headers;
            _body = body;
            _isAsync = isAsync;
            _callback = callback;
            _requester = this;
        }

        private static bool GetIsFileProtocol()
            => OpenSilver.Interop.ExecuteJavaScriptBoolean(@"(document.location.protocol === 'file:')");

        private static void SetRequestHeader(object xmlHttpRequest, string key, string header)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string sKey = INTERNAL_InteropImplementation.GetVariableStringForJS(key);
            string sHeader = INTERNAL_InteropImplementation.GetVariableStringForJS(header);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.setRequestHeader({sKey}, {sHeader})");
        }

        internal static object GetWebRequest()
            => OpenSilver.Interop.ExecuteJavaScript("new XMLHttpRequest()");

        internal static void SetCallbackMethod(object xmlHttpRequest, Action OnDownloadStatusCompleted)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(OnDownloadStatusCompleted));
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onloadend = {sCallback}");
        }

        private static void CreateRequest(object xmlHttpRequest, string address, string method, bool isAsync)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string sAddress = INTERNAL_InteropImplementation.GetVariableStringForJS(address);
            string sMethod = INTERNAL_InteropImplementation.GetVariableStringForJS(method);
            string sAsync = INTERNAL_InteropImplementation.GetVariableStringForJS(isAsync);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.open({sMethod}, {sAddress}, {sAsync})");
        }

        private static void EnableCookies(object xmlHttpRequest, bool value)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string enableCookies = INTERNAL_InteropImplementation.GetVariableStringForJS(value);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.withCredentials = {enableCookies}");
        }

        internal static void SetErrorCallback(object xmlHttpRequest, Action onError)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string sCallback = INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(onError));
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.onerror = {sCallback}");
        }


        internal static void ConsoleLog_JSOnly(string message)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid($"console.log({INTERNAL_InteropImplementation.GetVariableStringForJS(message)});");
        }

        internal static void SendRequest(object xmlHttpRequest, string address, string method, bool isAsync, string body)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            string sBody = INTERNAL_InteropImplementation.GetVariableStringForJS(body);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sRequest}.send({sBody})");
        }

        private void OnDownloadStringCompleted()
        {
            var e = new INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs();
            SetEventArgs(e);
            if (!_isFirstTryAtSendingUnsafeRequest || !IsCrashInPreflight) // if NOT(first unsafe try AND preflight error). The only case we do not want to enter this if is when the request will be resent without credentials.
            {
                if (DownloadStringCompleted != null)
                {
                    DownloadStringCompleted(_sender, e);
                }
            }
            _isFirstTryAtSendingUnsafeRequest = false;
        }

        private void SetEventArgs(INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            int currentReadyState = GetCurrentReadyState((object)_xmlHttpRequest);
            int currentStatus = GetCurrentStatus((object)_xmlHttpRequest);
            string errorMessage = null;
            if (GetHasError((object)_xmlHttpRequest))
            {
                //cases where current status represents an error (like 404 for page not found)
                errorMessage = string.Format("{0} - {1}",currentStatus, GetCurrentStatusText((object)_xmlHttpRequest));
            }
            else if (currentReadyState == 0 && !e.Cancelled)
            {
                errorMessage ="Request not initialized";
            }
            else if ((currentStatus == 0 && !GetIsFileProtocol()) && (currentReadyState == 4 || currentReadyState == 1)) //Note: we check whether the file protocol is file: because apparently, browsers return 0 as the status on a successful call.
            {
                errorMessage = "An error occurred. Please make sure that the target Url is available.";
            }
            else if (currentReadyState == 1 && !e.Cancelled)
            {
                errorMessage = "An Error occurred. Cross-Site Http Request might not be allowed at the target Url. If you own the domain of the Url, consider adding the header \"Access-Control-Allow-Origin\" to enable requests to be done at this Url.";
            }
            else if (currentReadyState != 4)
            {
                errorMessage = "An Error has occurred while submitting your request.";
            }
            if(errorMessage != null)
            {
                e.Error = new Exception(errorMessage);
            }
            e.Result = GetResult((object)_xmlHttpRequest);
        }

        private static int GetCurrentReadyState(object xmlHttpRequest)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sRequest}.readyState");
        }

        private static int GetCurrentStatus(object xmlHttpRequest)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sRequest}.status");
        }

        private static string GetCurrentStatusText(object xmlHttpRequest)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptString($"{sRequest}.statusText");
        }

        private static string GetResult(object xmlHttpRequest)
        {
            string sRequest = INTERNAL_InteropImplementation.GetVariableStringForJS(xmlHttpRequest);
            return OpenSilver.Interop.ExecuteJavaScriptString($"{sRequest}.responseText");
        }

        private static bool GetHasError(object xmlHttpRequest)
        {
            int currentStatus = GetCurrentStatus(xmlHttpRequest);
            //note: 4XX status corresponds to a client error (basically the  request is badly formatted or unauthorized... so it is not accepted by the server)
            //      5XX status corresponds to a server error (it happened to us with ClientBase when the serialization of the objects passed as parameters was not properly done and couldn't be deserialized)
            if (currentStatus >= 400 && currentStatus < 600)
            {
                return true;
            }
            return false;
        }


        //todo: see if this should be removed or not (I think it should)
        internal object GetXmlHttpRequest()
        {
            return _xmlHttpRequest;
        }
    }
}
