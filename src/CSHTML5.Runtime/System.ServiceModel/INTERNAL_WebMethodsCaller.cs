

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
        //------------------------------------------
        // WCF METHODS WHICH RETURN A VALUE
        //------------------------------------------

        public static Task<RETURN_TYPE> CallWebMethodAsync<RETURN_TYPE, INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
#if !FOR_DESIGN_TIME
            // Remove the word "Async" from the method name (this is because the method called on the server's side is the one without "Async" at the end)
            if (methodName.EndsWith("Async")) //should always be the case
                methodName = methodName.Remove(methodName.Length - 5);

            // Call the web method:
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(clientBase.INTERNAL_RemoteAddressAsString);
            return webMethodsCaller.CallWebMethodAsync<RETURN_TYPE>(methodName, typeof(INTERFACE_TYPE), typeof(RETURN_TYPE), requestParameters);
#else
            return null;
#endif
        }

        public static RETURN_TYPE CallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
#if !FOR_DESIGN_TIME
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(clientBase.INTERNAL_RemoteAddressAsString);
            return (RETURN_TYPE)webMethodsCaller.CallWebMethod(methodName, typeof(INTERFACE_TYPE), typeof(RETURN_TYPE), requestParameters);
#else
            return default(RETURN_TYPE);
#endif
        }


        internal partial class WebMethodAsyncResult : INTERNAL_AsyncResult
        {
            public string XmlReturnedFromTheServer { get; set; }

            public WebMethodAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        public static System.IAsyncResult BeginCallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
#if !FOR_DESIGN_TIME
            // Read the parameters:
            System.AsyncCallback callback = (System.AsyncCallback)requestParameters["callback"].Item2;
            object asyncState = requestParameters["asyncState"].Item2;

            //Note on the commentary below: we changed the Dictionary but it is still relevant.
            // Remove the "callback" and "asyncState" items from the "requestParameters" dictionary so as to leave only the real parameters of the web method. For example, in case of the web method "string GetData(int value1, int value2)", the "requestParameters" is equal to: new Dictionary<string, object>() {{"value1", value1} , {"value2", value2} , {"callback", callback} , {"asyncState", asyncState} }). We need to remove the "callback" and "asyncState" because otherwise they risk being serialized with the request.
            requestParameters.Remove("callback");
            requestParameters.Remove("asyncState");

            // Remove the word "Begin" from the method name (this is because the method called on the server's side is the one without "Begin")
            if (methodName.StartsWith("Begin")) //should always be the case
                methodName = methodName.Substring(5);

            // Call the server:
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(clientBase.INTERNAL_RemoteAddressAsString);
            var webMethodAsyncResult = new WebMethodAsyncResult(callback, asyncState);
            webMethodsCaller.BeginCallWebMethod(methodName, typeof(INTERFACE_TYPE), typeof(RETURN_TYPE), requestParameters,
                (xmlReturnedFromTheServer) =>
                {
                    //-------------------------------
                    // After server call has finished (not deserialized yet)
                    //-------------------------------
                    webMethodAsyncResult.XmlReturnedFromTheServer = xmlReturnedFromTheServer;
                    webMethodAsyncResult.Completed(); // This causes a call to "EndCallWebMethod" which will deserialize the response.
                });

            return webMethodAsyncResult;
#else
            return null;
#endif
        }

        public static RETURN_TYPE EndCallWebMethod<RETURN_TYPE, INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
#if !FOR_DESIGN_TIME
            // Remove the word "End" from the method name (this is because the method called on the server's side is the one without "End")
            if (methodName.StartsWith("End")) //should always be the case
                methodName = methodName.Substring(3);

            // Read the XML result from the parameters:
            System.IAsyncResult asyncResult = (System.IAsyncResult)requestParameters["result"].Item2;
            WebMethodAsyncResult webMethodAsyncResult = (WebMethodAsyncResult)asyncResult;
            string xmlReturnedFromTheServer = webMethodAsyncResult.XmlReturnedFromTheServer;

            // Call "EndCallWebMethod" to deserialize the result: 
            var webMethodsCaller = new CSHTML5_ClientBase<INTERFACE_TYPE>.WebMethodsCaller(clientBase.INTERNAL_RemoteAddressAsString);
            RETURN_TYPE result = webMethodsCaller.EndCallWebMethod<RETURN_TYPE>(methodName, typeof(INTERFACE_TYPE), xmlReturnedFromTheServer);

            // Return the deserialized result:
            return result;
#else
            return default(RETURN_TYPE);
#endif
        }

        //------------------------------------------
        // WCF METHODS WHICH DO NOT RETURN ANY VALUE
        //------------------------------------------

        public static Task CallWebMethodAsync_WithoutReturnValue<INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
            // The following call works fine because "Task<object>" inherits from "Task".

            return CallWebMethodAsync<object, INTERFACE_TYPE>(clientBase, methodName, requestParameters);
        }

        public static void CallWebMethod_WithoutReturnValue<INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
            CallWebMethod<object, INTERFACE_TYPE>(clientBase, methodName, requestParameters);
        }

        public static void EndCallWebMethod_WithoutReturnValue<INTERFACE_TYPE>(CSHTML5_ClientBase<INTERFACE_TYPE> clientBase, string methodName, Dictionary<string, Tuple<Type, object>> requestParameters) where INTERFACE_TYPE : class
        {
            EndCallWebMethod<object, INTERFACE_TYPE>(clientBase, methodName, requestParameters);
        }
    }
}