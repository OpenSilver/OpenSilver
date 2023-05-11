

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public static class INTERNAL_WebMethodsCaller
    {
        /// <summary>
        /// Key for the AsyncCallback paramater that must be passed as
        /// part of the request parameters IDictionary for asynchronous 
        /// request with Begin/End pattern.
        /// </summary>
        public const string CallbackParameterName = "callback";

        /// <summary>
        /// Key for the asyncState paramater that must be passed as
        /// part of the request parameters IDictionary for asynchronous 
        /// request with Begin/End pattern.
        /// </summary>
        public const string AsyncStateParameterName = "asyncState";

        /// <summary>
        /// Key for the IAsyncResult paramater that must be passed as
        /// part of the request parameters IDictionary for asynchronous 
        /// request with Begin/End pattern.
        /// </summary>
        public const string ResultParameterName = "result";

        //------------------------------------------
        // WCF METHODS WHICH RETURN A VALUE
        //------------------------------------------

        public static Task<RETURN_TYPE> CallWebMethodAsync<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // Call the web method
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);

            Task<RETURN_TYPE> task;
            try
            {
                task = webMethodsCaller.CallWebMethodAsync<RETURN_TYPE>(
                    webMethodName,
                    typeof(INTERFACE_TYPE),
                    typeof(RETURN_TYPE),
                    requestParameters,
                    soapVersion);
            }
            catch (MissingMethodException)
            {
                task = webMethodsCaller.CallWebMethodAsyncBeginEnd<RETURN_TYPE>(
                    webMethodName,
                    typeof(INTERFACE_TYPE),
                    typeof(RETURN_TYPE),
                    requestParameters,
                    soapVersion);
            }

            return task;
        }

#if OPENSILVER
        public static (RETURN_TYPE, Channels.MessageHeaders) CallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IEnumerable<Channels.MessageHeader> outgoingMessageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);

            var (typedResponseBody, incommingMessageHeaders) = webMethodsCaller.CallWebMethod(
                webMethodName,
                typeof(INTERFACE_TYPE),
                typeof(RETURN_TYPE),
                outgoingMessageHeaders,
                requestParameters,
                soapVersion);

            return ((RETURN_TYPE)typedResponseBody, incommingMessageHeaders);
        }

        public static Task<(RETURN_TYPE, Channels.MessageHeaders)> CallWebMethodAsync<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IEnumerable<Channels.MessageHeader> outgoingMessageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // Call the web method
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);

            Task<(RETURN_TYPE, Channels.MessageHeaders)> task;
            try
            {
                task = webMethodsCaller.CallWebMethodAsyncBeginEnd<RETURN_TYPE>(
                    webMethodName,
                    typeof(INTERFACE_TYPE),
                    typeof(RETURN_TYPE),
                    outgoingMessageHeaders,
                    requestParameters,
                    soapVersion);
            }
            catch (MissingMethodException)
            {
                task = webMethodsCaller.CallWebMethodAsync<RETURN_TYPE>(
                    webMethodName,
                    typeof(INTERFACE_TYPE),
                    typeof(RETURN_TYPE),
                    outgoingMessageHeaders,
                    requestParameters,
                    soapVersion);
            }

            return task;
        }
#endif

        public static RETURN_TYPE CallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);

            return (RETURN_TYPE)webMethodsCaller.CallWebMethod(
                webMethodName,
                typeof(INTERFACE_TYPE),
                typeof(RETURN_TYPE),
                requestParameters,
                soapVersion);
        }

        public static IAsyncResult BeginCallWebMethod<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            Type methodReturnType,
            IReadOnlyList<Type> knownTypes,
            string messageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // Read the parameters
            AsyncCallback callback = (AsyncCallback)requestParameters[CallbackParameterName];
            object asyncState = requestParameters[AsyncStateParameterName];

            // Note on the commentary below: we changed the Dictionary but it is still relevant.
            // Remove the "callback" and "asyncState" items from the "requestParameters" dictionary
            // so as to leave only the real parameters of the web method. For example, in case of 
            // the web method "string GetData(int value1, int value2)", the "requestParameters" is 
            // equal to: 
            // new Dictionary<string, object>() 
            // {
            //     {"value1", value1}, 
            //     {"value2", value2}, 
            //     {"callback", callback}, 
            //     {"asyncState", asyncState} 
            // }
            // We need to remove the "callback" and "asyncState" because otherwise 
            // they risk being serialized with the request.
            requestParameters.Remove(CallbackParameterName);
            requestParameters.Remove(AsyncStateParameterName);

            // Call the server
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);
            var webMethodAsyncResult = new WebMethodAsyncResult(callback, asyncState);

            webMethodsCaller.BeginCallWebMethod(
                webMethodName,
                typeof(INTERFACE_TYPE),
                methodReturnType,
                knownTypes,
                messageHeaders,
                requestParameters,
                (xmlReturnedFromTheServer) =>
                {
                    // After server call has finished (not deserialized yet)
                    webMethodAsyncResult.XmlReturnedFromTheServer = xmlReturnedFromTheServer;

                    // This causes a call to "EndCallWebMethod" which will deserialize the response.
                    webMethodAsyncResult.Completed();
                },
                soapVersion);

            return webMethodAsyncResult;
        }

        public static IAsyncResult BeginCallWebMethod<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            Type methodReturnType,
            string messageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            return BeginCallWebMethod<INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                methodReturnType,
                null,
                messageHeaders,
                requestParameters,
                soapVersion);
        }

        public static IAsyncResult BeginCallWebMethod<INTERFACE_TYPE>(
           string endpointAddress,
           string webMethodName,
           Type methodReturnType,
           IDictionary<string, object> requestParameters,
           string soapVersion) where INTERFACE_TYPE : class
        {
            return BeginCallWebMethod<INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                methodReturnType,
                null,
                "",
                requestParameters,
                soapVersion);
        }

        public static IAsyncResult BeginCallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            return BeginCallWebMethod<INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                typeof(RETURN_TYPE),
                null,
                "",
                requestParameters,
                soapVersion);
        }

        public static object EndCallWebMethod<INTERFACE_TYPE>(string endpointAddress,
            string webMethodName,
            Type methodReturnType,
            IReadOnlyList<Type> knownTypes,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // Read the XML result from the parameters
            IAsyncResult asyncResult = (IAsyncResult)requestParameters[ResultParameterName];
            WebMethodAsyncResult webMethodAsyncResult = (WebMethodAsyncResult)asyncResult;
            string xmlReturnedFromTheServer = webMethodAsyncResult.XmlReturnedFromTheServer;

            // Call "EndCallWebMethod" to deserialize the result
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(endpointAddress);

            object result = webMethodsCaller.EndCallWebMethod(
                webMethodName,
                typeof(INTERFACE_TYPE),
                methodReturnType,
                knownTypes,
                xmlReturnedFromTheServer,
                soapVersion);

            // Return the deserialized result
            return result;
        }

        public static object EndCallWebMethod<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            Type methodReturnType,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            return EndCallWebMethod<INTERFACE_TYPE>(endpointAddress,
               webMethodName,
               methodReturnType,
               null,
               requestParameters,
               soapVersion);
        }

        public static RETURN_TYPE EndCallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            return (RETURN_TYPE)EndCallWebMethod<INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                typeof(RETURN_TYPE),
                requestParameters,
                soapVersion);
        }

        //------------------------------------------
        // WCF METHODS WHICH DO NOT RETURN ANY VALUE
        //------------------------------------------

        public static Task CallWebMethodAsync_WithoutReturnValue<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // The following call works fine because "Task<object>" inherits from "Task".
            return CallWebMethodAsync<object, INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                requestParameters,
                soapVersion);
        }

#if OPENSILVER
        public static (object, Channels.MessageHeaders) CallWebMethod_WithoutReturnValue<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IEnumerable<Channels.MessageHeader> outgoingMessageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            return CallWebMethod<object, INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                outgoingMessageHeaders,
                requestParameters,
                soapVersion);
        }

        public static Task<(object, Channels.MessageHeaders)> CallWebMethodAsync_WithoutReturnValue<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IEnumerable<Channels.MessageHeader> outgoingMessageHeaders,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            // The following call works fine because "Task<object>" inherits from "Task".
            return CallWebMethodAsync<object, INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                outgoingMessageHeaders,
                requestParameters,
                soapVersion);
        }
#endif

        public static void CallWebMethod_WithoutReturnValue<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            CallWebMethod<object, INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                requestParameters,
                soapVersion);
        }

        public static void EndCallWebMethod_WithoutReturnValue<INTERFACE_TYPE>(
            string endpointAddress,
            string webMethodName,
            IDictionary<string, object> requestParameters,
            string soapVersion) where INTERFACE_TYPE : class
        {
            EndCallWebMethod<object, INTERFACE_TYPE>(
                endpointAddress,
                webMethodName,
                requestParameters,
                soapVersion);
        }

        internal partial class WebMethodAsyncResult : INTERNAL_AsyncResult
        {
            public string XmlReturnedFromTheServer { get; set; }

            public WebMethodAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }
    }
}