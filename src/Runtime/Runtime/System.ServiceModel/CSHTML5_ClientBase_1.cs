

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


#if (!FOR_DESIGN_TIME) && CORE
extern alias ToBeReplacedAtRuntime;
#endif

#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ComponentModel;
using System.Threading;
using System.ServiceModel.Channels;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Linq;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace System.ServiceModel
{
    /// <summary>
    /// Provides the base implementation used to create Windows Communication Foundation
    /// (WCF) client objects that can call services.
    /// </summary>
    /// <typeparam name="TChannel">The channel to be used to connect to the service.</typeparam>
    /// <example>
    /// Here is an example on how you can use a WebService to receive data:
    /// <code lang="C#">
    /// //We create a new instance of the ServiceClient
    /// MyServiceClient soapClient =
    ///     new MyServiceClient(
    ///         new BasicHttpBinding(),
    ///         new EndpointAddress(
    ///             new Uri("http://MyServiceAddress.com/MyService.svc")));
    ///
    /// //We call the method that will give us the data we want:
    /// var result = await soapClient.GetToDosAsync(_ownerId);
    /// //We get the data from the response:
    /// ToDoItem[] todos = result.Body.GetToDosResult;
    /// </code>
    /// Here is another example that shows how you can send data to a WebService:
    /// <code lang="C#">
    /// //We create an item to send to the WebService:
    /// ToDoItem todo = new ToDoItem()
    /// {
    ///     Description = MyTextBox.Text,
    ///     Id = Guid.NewGuid(),
    /// };
    ///
    /// //We create a new instance of the ServiceClient
    /// MyServiceClient soapClient =
    ///     new MyServiceClient(
    ///         new BasicHttpBinding(),
    ///         new EndpointAddress(
    ///             new Uri("http://MyServiceAddress.com/MyService.svc")));
    ///
    /// //We send the data by calling a method implemented for that purpose in the WebService:
    /// await soapClient.AddOrUpdateToDoAsync(todo);
    /// </code>
    /// </example>
#if WORKINPROGRESS && !CSHTML5BLAZOR
    public abstract partial class CSHTML5_ClientBase<TChannel> : ICommunicationObject where TChannel : class
#else
    public abstract partial class CSHTML5_ClientBase<TChannel> /*: ICommunicationObject, IDisposable*/ where TChannel : class
#endif
    {
#if OPENSILVER
        //Note: Adding this because they are in the file generated when adding a Service Reference through the "Add Connected Service" for OpenSilver.
        public System.ServiceModel.Description.ServiceEndpoint Endpoint { get; } = new Description.ServiceEndpoint(new Description.ContractDescription("none"));
        public System.ServiceModel.Description.ClientCredentials ClientCredentials { get; } = new Description.ClientCredentials();
#endif
        string _remoteAddressAsString;

        private TChannel channel;
        public TChannel Channel
        {
            get
            {
                return channel;
            }
        }

        /// <summary>
        /// Provides support for implementing the event-based asynchronous pattern.
        /// </summary>
        /// <param name="beginOperationDelegate">A delegate that is used for calling the asynchronous operation.</param>
        /// <param name="inValues">The input values to the asynchronous call.</param>
        /// <param name="endOperationDelegate">A delegate that is used to end the asynchronous call after it has completed.</param>
        /// <param name="operationCompletedCallback">
        /// A client-supplied callback that is invoked when the asynchronous method is
        /// complete. The callback is passed to the BeginOperationDelegate.
        /// </param>
        /// <param name="userState">The userState object to associate with the asynchronous call.</param>
        protected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues,
          EndOperationDelegate endOperationDelegate, SendOrPostCallback operationCompletedCallback, object userState)
        {
#if USE_ASYNCOPERATION_CLASS
            AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userState);
#endif
            AsyncOperationContext context = new AsyncOperationContext(
#if USE_ASYNCOPERATION_CLASS
                asyncOperation,
#endif
endOperationDelegate, operationCompletedCallback);
            IAsyncResult result = beginOperationDelegate(inValues, OnAsyncCallCompleted, context);
        }

        static void OnAsyncCallCompleted(IAsyncResult result)
        {
            AsyncOperationContext context = (AsyncOperationContext)((System.ServiceModel.INTERNAL_WebMethodsCaller.WebMethodAsyncResult)result).AsyncState;
            Exception error = null;
            object[] results = null; //todo: fix type?
            try
            {
                results = context.EndDelegate(result);
            }
            catch (Exception e)
            {
                error = e;
            }
            CompleteAsyncCall(context, results, error);
        }

        static void CompleteAsyncCall(AsyncOperationContext context, object[] results, Exception error)
        {
            if (context.CompletionCallback != null)
            {
                InvokeAsyncCompletedEventArgs e = new InvokeAsyncCompletedEventArgs(results, error, false, null/*context.AsyncOperation.UserSuppliedState*/);
#if USE_ASYNCOPERATION_CLASS
                context.AsyncOperation.PostOperationCompleted(context.CompletionCallback, e);
#else
                context.CompletionCallback(e);
#endif
            }
#if USE_ASYNCOPERATION_CLASS
            else
            {
                context.AsyncOperation.OperationCompleted();
            }
#endif
        }

        class AsyncOperationContext
        {
#if USE_ASYNCOPERATION_CLASS
            AsyncOperation asyncOperation;
#endif
            EndOperationDelegate endDelegate;
            SendOrPostCallback completionCallback;

            internal AsyncOperationContext(
#if USE_ASYNCOPERATION_CLASS
                AsyncOperation asyncOperation,
#endif
EndOperationDelegate endDelegate, SendOrPostCallback completionCallback)
            {
#if USE_ASYNCOPERATION_CLASS
                this.asyncOperation = asyncOperation;
#endif
                this.endDelegate = endDelegate;
                this.completionCallback = completionCallback;
            }

#if USE_ASYNCOPERATION_CLASS
            internal AsyncOperation AsyncOperation
            {
                get
                {
                    return this.asyncOperation;
                }
            }
#endif

            internal EndOperationDelegate EndDelegate
            {
                get
                {
                    return this.endDelegate;
                }
            }

            internal SendOrPostCallback CompletionCallback
            {
                get
                {
                    return this.completionCallback;
                }
            }

        }

        /// <summary>
        /// A delegate that is used by ClientBase&lt;TChannel&gt;.InvokeAsync(...)
        /// for calling asynchronous operations on the client.
        /// </summary>
        /// <param name="inValues"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, object state);

        /// <summary>
        /// A delegate that is invoked by ClientBase&lt;TChannel&gt;.InvokeAsync(...)
        /// on successful completion of the call made by ClientBase&lt;TChannel&gt;.InvokeAsync(...)
        /// to ClientBase&lt;TChannel&gt;.BeginOperationDelegate.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected delegate object[] EndOperationDelegate(IAsyncResult result);

        /// <summary>
        /// Stores the results from an asynchronous call made by the client.
        /// </summary>
        protected class InvokeAsyncCompletedEventArgs : AsyncCompletedEventArgs
        {
            object[] results;

            internal InvokeAsyncCompletedEventArgs(object[] results, Exception error, bool cancelled, object userState)
                : base(error, cancelled, userState)
            {
                this.results = results;
            }

            public object[] Results
            {
                get
                {
                    return this.results;
                }
            }
        }

        public string INTERNAL_RemoteAddressAsString
        {
            get
            {
                return _remoteAddressAsString;
            }
        }

        //#if !FOR_DESIGN_TIME && CORE
        //        [JSIgnore]
        //        INTERNAL_RealClientBaseImplementation<TChannel> _realClientBase;
        //#endif

        // Exceptions:
        //   System.InvalidOperationException:
        //     Either there is no default endpoint information in the configuration file,
        //     more than one endpoint in the file, or no configuration file.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the default target endpoint from the application configuration
        /// file.
        /// </summary>
        protected CSHTML5_ClientBase()
        {
            //CreateRealClientBaseImplementationOfNotJavaScript();
            //we get the path to the correct .config file:

            //we get the name of the contract we are looking for:
            string contractConfigurationName = null;
            //BRIDGETODO
            //Compare result of [...].GenericTypeArguments[0] and [...].GetGenericArguments()[0]
#if !BRIDGE
            Type interfacetype = this.GetType().BaseType.GenericTypeArguments[0];
#else
            Type interfacetype = this.GetType().BaseType.GetGenericArguments()[0];
#endif
            foreach (Attribute attribute in interfacetype.GetCustomAttributes(false))
            {
                if (attribute is ServiceContract2Attribute)
                {
                    contractConfigurationName = ((ServiceContract2Attribute)attribute).ConfigurationName;
                    if (contractConfigurationName != null)
                    {
                        break;
                    }
                }
#if BRIDGE || CSHTML5BLAZOR
                if (attribute is ServiceContractAttribute)
                {
                    contractConfigurationName = ((ServiceContractAttribute)attribute).ConfigurationName;
                    if (contractConfigurationName != null)
                    {
                        break;
                    }
                }
#endif
            }
            if (contractConfigurationName == null)
            {
                throw new Exception("Could not find a suitable ServiceContractAttribute to get the default endpoint in the type: \"" + interfacetype.FullName + "\".");
            }

            // Attempt to read the WCF endpoint address by first looking into the "ServiceReferences.ClientConfig" file, and then the "App.Config" file:
            string endpointAddress;
            object serviceReferencesClientConfig = CSHTML5.Interop.ExecuteJavaScript("window.ServiceReferencesClientConfig");
            if (TryReadEndpoint(serviceReferencesClientConfig, "ServiceReferences.ClientConfig", contractConfigurationName, throwIfFileNotFound: false, endpointAddress: out endpointAddress))
            {
                _remoteAddressAsString = endpointAddress;
            }
            else
            {
                object appConfig = CSHTML5.Interop.ExecuteJavaScript("window.AppConfig");
                if (TryReadEndpoint(appConfig, "App.Config", contractConfigurationName, throwIfFileNotFound: true, endpointAddress: out endpointAddress))
                {
                    _remoteAddressAsString = endpointAddress;
                }
                else
                {
                    throw new Exception("Could not find the default WCF endpoint element that references the contract \"" + contractConfigurationName + "\" in the ServiceModel client configuration section.");
                }
            }
        }

        static bool TryReadEndpoint(object configFileContent, string fileName, string contractConfigurationName, bool throwIfFileNotFound, out string endpointAddress)
        {
            bool isNullOrUndefined = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 == undefined || $0 == null", configFileContent));
            if (!isNullOrUndefined)
            {
                string fileContentAsString = Convert.ToString(configFileContent);
                //string fileContent = await Application.GetResourceString(new Uri(filePath));
                //convert the file to a XDocument
                XDocument doc = XDocument.Parse(fileContentAsString);

                // Attempt to find the elements that contain the correct Binding and endpointAddress:
                foreach (XElement endpointXElement in doc.Descendants("endpoint"))
                {
                    XAttribute contractXAttribute = endpointXElement.Attributes("contract").FirstOrDefault();
                    if (contractXAttribute != null
                        && contractXAttribute.Value == contractConfigurationName)
                    {
                        XAttribute addressXAttribute = endpointXElement.Attributes("address").FirstOrDefault();
                        if (addressXAttribute != null)
                        {
                            endpointAddress = addressXAttribute.Value;
                            return true;
                        }
                    }
                }

                endpointAddress = null;
                return false;
            }
            else
            {
                //--------------------------
                // FILE NOT FOUND
                //--------------------------

                if (throwIfFileNotFound)
                {
                    throw new FileNotFoundException("Could not find the \"" + fileName + "\" file to get the default endpoint. Please make sure that you have added your service reference through \"Add service reference\" or use the constructor that allows you to set the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
                }
                else
                {
                    endpointAddress = null;
                    return false;
                }
            }
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The specified endpoint information is null.
        //
        //   System.InvalidOperationException:
        //     The endpoint cannot be found or the endpoint contract is not valid.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the configuration information specified in the application configuration
        /// file by endpointConfigurationName.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        protected CSHTML5_ClientBase(string endpointConfigurationName)
        {
            //CreateRealClientBaseImplementationOfNotJavaScript(endpointConfigurationName);

            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The binding is null.
        //
        //   System.ArgumentNullException:
        //     The remote address is null.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the specified binding and target address.
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        protected CSHTML5_ClientBase(Binding binding, EndpointAddress remoteAddress)
        {
            //CreateRealClientBaseImplementationOfNotJavaScript(binding, remoteAddress);

            _remoteAddressAsString = remoteAddress.Uri.OriginalString;

            //todo: finish the implementation.
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The endpoint is null.
        //
        //   System.ArgumentNullException:
        //     The remote address is null.
        //
        //   System.InvalidOperationException:
        //     The endpoint cannot be found or the endpoint contract is not valid.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the specified target address and endpoint information.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        protected CSHTML5_ClientBase(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            //CreateRealClientBaseImplementationOfNotJavaScript(endpointConfigurationName, remoteAddress);

            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The endpoint is null.
        //
        //   System.ArgumentNullException:
        //     The remote address is null.
        //
        //   System.InvalidOperationException:
        //     The endpoint cannot be found or the endpoint contract is not valid.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        protected CSHTML5_ClientBase(string endpointConfigurationName, string remoteAddress)
        {
            //CreateRealClientBaseImplementationOfNotJavaScript(endpointConfigurationName, remoteAddress);

            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

#if !FOR_DESIGN_TIME
        /// <summary>
        /// Provides an API to call web methods defined in a WebService
        /// </summary>
        public partial class WebMethodsCaller
        {
            string _addressOfService;

#if !CSHTML5BLAZOR
            INTERNAL_WebRequestHelper_SimulatorOnly _webRequestHelper = new INTERNAL_WebRequestHelper_SimulatorOnly();
#endif
            INTERNAL_WebRequestHelper_JSOnly _webRequestHelper_JSVersion = new INTERNAL_WebRequestHelper_JSOnly();

            /// <summary>
            /// Constructor for the WebMethodsCaller's class
            /// </summary>
            /// <param name="addressOfService">The address of the WebService</param>
            public WebMethodsCaller(string addressOfService)
            {
                _addressOfService = addressOfService;
            }

            public void BeginCallWebMethod(string methodName, Type interfaceType, Type methodReturnType, Dictionary<string, Tuple<Type, object>> originalRequestObject, Action<string> callback, string soapVersion)
            {
                bool isXmlSerializerRatherThanDataContractSerializer = IsXmlSerializerRatherThanDataContractSerializer(methodName, methodReturnType, originalRequestObject);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(methodName, interfaceType, originalRequestObject, isXmlSerializerRatherThanDataContractSerializer, out headers, soapVersion, out request);

                // Make the actual web service call:
                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
#if BRIDGE || CSHTML5BLAZOR
                    _webRequestHelper_JSVersion.MakeRequest(
#if WORKINPROGRESS
                        INTERNAL_UriHelper.EnsureAbsoluteUri(_addressOfService)
#else
                        new Uri(_addressOfService)
#endif
                        , "POST", this, headers, request,
                        (INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler)((sender, e) =>
                        {
                            string xmlReturnedFromTheServer = e.Result;
                            callback(xmlReturnedFromTheServer);
                        })
                        , true, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
#else
                    _webRequestHelper.MakeRequestAsync_CSharpVersion(new Uri(_addressOfService), headers, request,
                        (UploadStringCompletedEventHandler)((sender, e) =>
                        {
                            string xmlReturnedFromTheServer = e.Result;
                            callback(xmlReturnedFromTheServer);
                        }));
#endif
                }
                else
                {
                    _webRequestHelper_JSVersion.MakeRequest(
#if WORKINPROGRESS
                        INTERNAL_UriHelper.EnsureAbsoluteUri(_addressOfService)
#else
                        new Uri(_addressOfService)
#endif
                        , "POST", this, headers, request,
                        (INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler)((sender, e) =>
                        {
                            string xmlReturnedFromTheServer = e.Result;
                            callback(xmlReturnedFromTheServer);
                        })
                        , true, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
                }
            }

            public RETURN_TYPE EndCallWebMethod<RETURN_TYPE>(string methodName, Type interfaceType, string xmlReturnedFromTheServer, string soapVersion)
            {
                bool isXmlSerializerRatherThanDataContractSerializer = IsXmlSerializerRatherThanDataContractSerializer(methodName, typeof(RETURN_TYPE), null);

                RETURN_TYPE requestResponse = (RETURN_TYPE)ReadAndPrepareResponse(xmlReturnedFromTheServer, interfaceType, typeof(RETURN_TYPE), faultException => { throw faultException; }, isXmlSerializerRatherThanDataContractSerializer, soapVersion);

                return requestResponse;
            }

            //the following comments seem to be useless since apparently, this method is not called using c#
            /// <summary>
            /// Asynchronously calls a WebMethod.
            /// </summary>
            /// <typeparam name="T">The return type of the WebMethod</typeparam>
            /// <param name="methodName">The name of the WebMethod</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType">The return Type of the method</param>
            /// <param name="originalRequestObject">The additional arguments of the method</param>
            /// <returns>The result of the call of the method.</returns>
            public Task<T> CallWebMethodAsync<T>(string methodName, Type interfaceType, Type methodReturnType, Dictionary<string, Tuple<Type, object>> originalRequestObject, string soapVersion) // Note: we don't arrive here using c#
            {
                //todo: find out what happens with methods that take multiple arguments (if possible) and change the parameterName to a string[].
                Dictionary<string, string> headers;
                string request;
                bool isXmlSerializerRatherThanDataContractSerializer = IsXmlSerializerRatherThanDataContractSerializer(methodName, methodReturnType, originalRequestObject);

                PrepareRequest(methodName, interfaceType, originalRequestObject, isXmlSerializerRatherThanDataContractSerializer, out headers, soapVersion, out request);
                string response = null;

                var taskCompletionSource = new TaskCompletionSource<T>(); //todo: here we need to change object to the return type
                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
#if BRIDGE || CSHTML5BLAZOR
                    response = _webRequestHelper_JSVersion.MakeRequest(new Uri(_addressOfService), "POST", this, headers, request, (sender, args2) => ReadAndPrepareResponseGeneric_JSVersion(taskCompletionSource, args2, interfaceType, methodReturnType, isXmlSerializerRatherThanDataContractSerializer, soapVersion), true, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
#else
                    _webRequestHelper.MakeRequestAsync_CSharpVersion(new Uri(_addressOfService), headers, request, (sender, args2) => ReadAndPrepareResponseGeneric(taskCompletionSource, args2, interfaceType, methodReturnType, isXmlSerializerRatherThanDataContractSerializer));
#endif
                }
                else
                {
                    response = _webRequestHelper_JSVersion.MakeRequest(new Uri(_addressOfService), "POST", this, headers, request, (sender, args2) => ReadAndPrepareResponseGeneric_JSVersion(taskCompletionSource, args2, interfaceType, methodReturnType, isXmlSerializerRatherThanDataContractSerializer, soapVersion), true, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
                }

                return taskCompletionSource.Task;
            }

            /// <summary>
            /// Calls a WebMethod
            /// </summary>
            /// <param name="methodName">The name of the Method</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType"></param>
            /// <param name="originalRequestObject"></param>
            /// <returns>The result of the call of the method.</returns>
            public object CallWebMethod(string methodName, Type interfaceType, Type methodReturnType, Dictionary<string, Tuple<Type, object>> originalRequestObject, string soapVersion) // Note: we don't arrive here using c#.
            {
                //**************************************
                // What the request should look like in case of classes or strings:
                //**************************************
                //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                //  <s:Body>
                //    <GetCurrentTime xmlns="http://tempuri.org/"/>
                //  </s:Body>
                //</s:Envelope>
                //**************************************
                // What the request should look like with parameters:
                //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                //  <s:Body>
                //    <AddTodo xmlns="http://tempuri.org/">
                //    <id>1</id>
                //    <todo>zsfzef</todo>
                //    <priority>1</priority>
                //    <dueDate i:nil="true" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"/>
                //    </AddTodo>
                //  </s:Body>
                //</s:Envelope>
                //**************************************

                //**************************************
                // What the request should look like in case of value types (eg. int MethodName(int)):
                //**************************************

                //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                //    <s:Body>
                //        <MethodName xmlns="http://tempuri.org/">
                //            <value>10</value>
                //        </MethodName>
                //    </s:Body>
                //</s:Envelope>
                //**************************************

                Dictionary<string, string> headers;
                string request;
                bool isXmlSerializerRatherThanDataContractSerializer = IsXmlSerializerRatherThanDataContractSerializer(methodName, methodReturnType, originalRequestObject);
                PrepareRequest(methodName, interfaceType, originalRequestObject, isXmlSerializerRatherThanDataContractSerializer, out headers, soapVersion, out request);
                string response = null;

                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
#if BRIDGE || CSHTML5BLAZOR
                    response = _webRequestHelper_JSVersion.MakeRequest(new Uri(_addressOfService), "POST", this, headers, request, null, false, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
#else

                    response = _webRequestHelper.MakeRequest_CSharpVersion(new Uri(_addressOfService), headers, request);
#endif
                }
                else
                {
                    response = _webRequestHelper_JSVersion.MakeRequest(new Uri(_addressOfService), "POST", this, headers, request, null, false, Application.Current.Host.Settings.DefaultSoapCredentialsMode);
                }

                Type requestResponseType = methodReturnType; //GetReturnType(); //return type is of type XXXResponse
                return ReadAndPrepareResponse(response, interfaceType, requestResponseType, faultException => { throw faultException; }, isXmlSerializerRatherThanDataContractSerializer, soapVersion);
            }

            private static bool IsXmlSerializerRatherThanDataContractSerializer(string methodName, Type methodReturnType, object originalRequestObject)
            {
                bool doesRequestContainNonEmptyDictionnary = originalRequestObject != null ? (((Dictionary<string, Tuple<Type, object>>)originalRequestObject).Count > 0 ? true : false) : false;

                if ((methodReturnType.Name == methodName + "Response" && methodReturnType.GetField("Body") != null)
                    || (methodReturnType.Name == "IAsyncResult" && doesRequestContainNonEmptyDictionnary ? ((Dictionary<string, Tuple<Type, object>>)originalRequestObject).First().Value.Item1.Name == methodName + "Request" : false))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Note: JSReplacement does not work here (for some unknown reason)
#if BRIDGE
            [Template("(typeof {obj} !== 'undefined' && {obj} !== null)")]
#endif
            private static bool IsNotUndefinedOrNull(dynamic obj)
            {
#if !BRIDGE
                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
                    return obj != null;
                }
                else
                {
                    return JSIL.Verbatim.Expression("(typeof $0 !== 'undefined' && $0 !== null)", obj);
                }
#else
                return false;
#endif
            }

            private void PrepareRequest(string methodName, Type interfaceType, Dictionary<string, Tuple<Type, object>> requestParameters, bool isXmlSerializerRatherThanDataContractSerializer, out Dictionary<string, string> headers, string soapVersion, out string request)
            {
                //todo: This was added for Client_GD (because of the "Begin/End async pattern") but it may cause issues on other projects if there are web methods which name starts with "Begin".
                if (methodName.StartsWith("Begin"))
                    methodName = methodName.Substring(5);
                string interfaceTypeName = interfaceType.Name;
                string interfaceTypeNamespace = "http://tempuri.org/";
                string soapActionPrefix = string.Empty; // initial value // This is used to determine the SOAPAction header to send to the server, by concatenating the soapActionPrefix and the method name.

#if BRIDGE || CSHTML5BLAZOR
                foreach (Attribute attribute in interfaceType.GetCustomAttributes(typeof(ServiceContract2Attribute), true))
#else
                foreach (Attribute attribute in interfaceType.GetCustomAttributes(true))
                {
                    if (attribute is ServiceContract2Attribute)

#endif
                    {
                        ServiceContract2Attribute attributeAsDataContractAttribute = (ServiceContract2Attribute)attribute;
                        soapActionPrefix = attributeAsDataContractAttribute.SOAPActionPrefix;
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Namespace))
                        {
                            interfaceTypeNamespace = attributeAsDataContractAttribute.Namespace;
                            break;
                        }
#if !BRIDGE && !CSHTML5BLAZOR
                    }
#endif
                }
#if BRIDGE || CSHTML5BLAZOR
                if (string.IsNullOrEmpty(soapActionPrefix))
                {
                    foreach (Attribute attribute in interfaceType.GetCustomAttributes(typeof(ServiceContractAttribute), true))
                    {
                        ServiceContractAttribute attributeAsDataContractAttribute = (ServiceContractAttribute)attribute;
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Namespace))
                        {
                            interfaceTypeNamespace = attributeAsDataContractAttribute.Namespace;
                            soapActionPrefix = attributeAsDataContractAttribute.Namespace;
                        }
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Name))
                        {
                            soapActionPrefix += attributeAsDataContractAttribute.Name + "/";
                        }
                        break;
                    }
                }
#endif

                headers = new Dictionary<string, string>();
                
                switch (soapVersion)
                {
                    case "1.1":
                        headers.Add("Content-Type", @"text/xml; charset=utf-8");
                        headers.Add("SOAPAction", @"""" + soapActionPrefix + methodName + "\"");
                        request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body>";//<" + methodName + @" xmlns=""" + interfaceTypeNamespace + "\">";
                        break;
                    case "1.2":
                        headers.Add("Content-Type", @"application/soap+xml; charset=utf-8");
                        request = $@"<s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:s=""http://www.w3.org/2003/05/soap-envelope""><s:Header><a:Action>http://tempuri.org/ServiceHost/{methodName}</a:Action></s:Header><s:Body>";//<" + methodName + @" xmlns=""" + interfaceTypeNamespace + "\">";
                        break;
                    default:
                        throw new InvalidOperationException($"SOAP version not supported: {soapVersion}");
                }
                
                XElement methodNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(methodName)); //in every case, we want the name of the method as a XElement

                //Note: now we want to add the parameters of the method:
                // to do that, we basically get the serialized version of the objects, and replace their tag that should have the type with the parameter name.

                if (requestParameters != null)
                {
                    foreach (string parameterName in requestParameters.Keys)
                    {
                        object requestBody = requestParameters[parameterName].Item2;
                        if (requestBody != null)
                        {
                            //we serialize the body of the request:
                            //get the known types from the interface type:
                            List<Type> knownTypes = new List<Type>();
                            MethodInfo methodInfo = interfaceType.GetMethod(methodName);
                            foreach (Attribute attribute in interfaceType.GetCustomAttributes(true))
                            {
                                if (attribute is ServiceKnownTypeAttribute)
                                {
                                    knownTypes.Add(((ServiceKnownTypeAttribute)attribute).Type);
                                }
                            }

                            //we find the type expected by the method for this parameter:
                            Type requestBodyType = requestParameters[parameterName].Item1;
                            //foreach(ParameterInfo parameterInfo in methodInfo.GetParameters())
                            //{
                            //    if(parameterInfo.Name == parameterName)
                            //    {
                            //        requestBodyType = parameterInfo.ParameterType;
                            //        break;
                            //    }
                            //}

#if CSHTML5BLAZOR
                            DataContractSerializer_CSHTML5Ver dataContractSerializer = new DataContractSerializer_CSHTML5Ver(requestBodyType, knownTypes, isXmlSerializerRatherThanDataContractSerializer);
                            XDocument xdoc = dataContractSerializer.SerializeToXDocument(requestBody);
#else
                            DataContractSerializer dataContractSerializer = new DataContractSerializer(requestBodyType, knownTypes, isXmlSerializerRatherThanDataContractSerializer);
                            XDocument xdoc = dataContractSerializer.SerializeToXDocument(requestBody);
#endif

                            XElement paramNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(parameterName));
                            if (!isXmlSerializerRatherThanDataContractSerializer)
                            {
                                methodNameElement.Add(paramNameElement); //we don't want to add this in the case of an XmlSerializer because it would be <request> which is not what we want. The correct parameter name is alread in the Request body.
                                foreach (XNode currentNode in xdoc.Root.Nodes())
                                {
                                    paramNameElement.Add(currentNode);
                                }
                                foreach (XAttribute currentAttribute in xdoc.Root.Attributes())
                                {
                                    if (currentAttribute.Name.LocalName != "xmlns") //we don't want to keep the "xmlns="http://schemas.microsoft.com/2003/10/Serialization/" because it breaks the request.
                                    {
                                        paramNameElement.Add(currentAttribute);
                                    }
                                }
                            }
                            else //we are using the XmlSerializer style:
                            {
                                //we assume that it always has the same structure: <root><AddOrUpdateToDoRequest xmlns="http://schemas.datacontract.org/2004/07/"><Body><toDoItem
                                //so we want to go to xdoc.Root.Nodes()[0].Nodes()
                                foreach (XNode currentNode in xdoc.Root.Nodes())
                                {
                                    //there should be only one here
                                    if (currentNode is XElement) //this should be true.
                                    {
                                        XElement xNodeAsXElement = (XElement)currentNode;
                                        foreach (XNode node in xNodeAsXElement.Nodes())
                                        {
                                            methodNameElement.Add(node);
                                        }
                                    }
                                }
                            }
                        }
                        else // the value is null so we simply need to put the parameter name with i:nil="true" and we're good
                        {
                            XElement paramNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(parameterName));
                            XAttribute attribute = new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("nil"), "true");
                            paramNameElement.Add(attribute);
                            methodNameElement.Add(paramNameElement);
                        }
                    }
                }

#if CSHTML5NETSTANDARD
                request += methodNameElement.ToString(SaveOptions.DisableFormatting); //this adds everything we want. "false" means that we do not want the result to be indented.
#else
                request += methodNameElement.ToString(false); //this adds everything we want. "false" means that we do not want the result to be indented.
#endif

                //we add the parameters to the method:
                request += "</s:Body></s:Envelope>";
                //request += "</" + methodName + "></s:Body></s:Envelope>";
            }

            private void ReplaceObjectTypeNameWithParameterName(ref string request, string objectTypeName, string parameterName)
            {
                //we get the first occurence of the type name (which is the first tag of the type):
                int objectFirstTagNameIndex = request.IndexOf(objectTypeName);
                //we look for the last occurence of the type name (which is the closing tag of the type):
                int objectLastTagNameIndex = -1;
                int i = request.IndexOf(objectTypeName, objectFirstTagNameIndex + 1);

                while (i > -1)
                {
                    objectLastTagNameIndex = i;
                    i = request.IndexOf(objectTypeName, i + 1);
                }

                if (objectLastTagNameIndex == -1)
                {
                    //as far as I know, we should never arrive here no matter what the user does, so it is a mistake on our part.
                    throw new Exception("Could not prepare request.");
                }
                else
                {
                    //we replace the tag names:
                    //Note: we have to replace the part in the last tag first because parameterName is likely to be of a different length than objectTypeName,
                    //  which would cause an offset on anything that comes after the replacement (in this case, changing the first tag would make objectLastTagNameIndex incorrect).
                    request = request.Remove(objectLastTagNameIndex, objectTypeName.Length);
                    request = request.Insert(objectLastTagNameIndex, parameterName);

                    request = request.Remove(objectFirstTagNameIndex, objectTypeName.Length);
                    request = request.Insert(objectFirstTagNameIndex, parameterName);
                }
            }

            private void ReadAndPrepareResponseGeneric<T>(TaskCompletionSource<T> taskCompletionSource, UploadStringCompletedEventArgs e, Type interfaceType, Type requestResponseType, bool isXmlSerializerRatherThanDataContractSerializer, string soapVersion)
            {
                if (e.Error == null)
                {
                    T requestResponse = (T)ReadAndPrepareResponse(e.Result, interfaceType, requestResponseType, faultException => taskCompletionSource.TrySetException(faultException), isXmlSerializerRatherThanDataContractSerializer, soapVersion);
                    if (!taskCompletionSource.Task.IsCompleted) //Note: this Task.IsCompleted can be true if we met an exception which triggered a call to TrySetException (above).
                        taskCompletionSource.SetResult(requestResponse);
                }
                else
                {
                    taskCompletionSource.TrySetException(e.Error);
                }
            }

            private void ReadAndPrepareResponseGeneric_JSVersion<T>(TaskCompletionSource<T> taskCompletionSource, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e, Type interfaceType, Type requestResponseType, bool isXmlSerializerRatherThanDataContractSerializer, string soapVersion)
            {
                if (e.Error == null)
                {
                    T requestResponse = (T)ReadAndPrepareResponse(e.Result, interfaceType, requestResponseType, faultException => taskCompletionSource.TrySetException(faultException), isXmlSerializerRatherThanDataContractSerializer, soapVersion);
                    if (!taskCompletionSource.Task.IsCompleted) //Note: this Task.IsCompleted can be true if we met an exception which triggered a call to TrySetException (above).
                        taskCompletionSource.SetResult(requestResponse);
                }
                else
                {
                    taskCompletionSource.TrySetException(e.Error);
                }
            }

            private object ReadAndPrepareResponse(string responseAsString, Type interfaceType, Type requestResponseType, Action<FaultException> raiseFaultException, bool isXmlSerializerRatherThanDataContractSerializer, string soapVersion)
            {
                //**************************************
                // What the response should look like in case of classes or strings:
                //**************************************
                //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                //  <s:Body xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                //    <METHODNAMEResponse xmlns="http://tempuri.org/">
                //      <METHODNAMEResult>
                //        <BoolValue>true</BoolValue>
                //        <StringValue>44</StringValue>
                //      </METHODNAMEResult>
                //    </METHODNAMEResponse>
                //  </s:Body>
                //</s:Envelope>
                //**************************************
                // What the response should look like in case of value types (eg. int MethodName(int)):
                //**************************************
                //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                //    <s:Body xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                //        <MethodNameResponse xmlns="http://tempuri.org/">
                //            <MethodNameResult>10</MethodNameResult>
                //        </MethodNameResponse>
                //    </s:Body>
                //</s:Envelope>

                //-----------------------------------------------------------
                //handle the case where the server has sent a FaultException: (this is what it looks like when the exception is of type FaultException, we need to try with a custom FaultException)
                //-----------------------------------------------------------
                //<s:Fault>
                //    <faultcode>s:Client</faultcode>
                //    <faultstring xml:lang="fr-FR">id already exists</faultstring>
                //</s:Fault>
                //we make our own exception. Because it is easier that way (it will probably require an actual deserialization of the users' FaultException later on, to be able to support their custom ones).
#if OPENSILVER
                // Error parsing, if applicable
                if (soapVersion == "1.2")
                {
                    const string NS = "http://www.w3.org/2003/05/soap-envelope";
                    
                    XElement envelopeElement = XDocument.Parse(responseAsString).Root;
                    XElement headerElement = envelopeElement.Element(XName.Get("Header", NS));
                    XElement bodyElement = envelopeElement.Element(XName.Get("Body", NS));

                    XElement faultElement = bodyElement.Element(XName.Get("Fault", NS));
                    
                    if (faultElement != null)
                    {
                        XElement codeElement = faultElement.Element(XName.Get("Code", NS));
                        XElement reasonElement = faultElement.Element(XName.Get("Reason", NS));
                        XElement detailElement = faultElement.Element(XName.Get("Detail", NS));

                        FaultCode faultCode = new FaultCode(codeElement.Elements().First().Value);
                        FaultReason faultReason = new FaultReason(reasonElement.Elements().First().Value);
                        string action = headerElement.Element(XName.Get("Action", "http://www.w3.org/2005/08/addressing")).Value;
                        
                        FaultException faultException;
                        
                        if (detailElement != null)
                        {
                            XElement innerExceptionElement = detailElement.Elements().First();

                            object innerException = ParseException(innerExceptionElement, innerExceptionElement.Name.LocalName);
                            
                            Type faultExceptionType = typeof(FaultException<>).MakeGenericType(innerException.GetType());

                            faultException = (FaultException)Activator.CreateInstance(faultExceptionType, innerException, faultReason, faultCode, action);
                        }
                        else
                        {
                            faultException = new FaultException(faultReason, faultCode, action);
                        }
                        
                        
                        raiseFaultException(faultException);
                        return null;
                    }

                    object ParseException(XElement exceptionElement, string exceptionTypeName)
                    {
                        Type exceptionType = ResolveType(exceptionTypeName);
                        
                        object exception = Activator.CreateInstance(exceptionType);
                        
                        foreach (XElement element in exceptionElement.Elements())
                        {
                            PropertyInfo property = exceptionType.GetProperty(element.Name.LocalName);
                            
                            XAttribute isNullAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName == "nil");
                            if (isNullAttribute != null && isNullAttribute.Value == "true")
                            {
                                property.SetValue(exception, null);
                            }
                            else
                            {
                                if (property.Name == "InnerException")
                                    property.SetValue(exception, ParseException(element, "SoaUnknownException"));
                                else
                                    property.SetValue(exception, element.Value);
                            }
                        }

                        return exception;
                    }

                    Type ResolveType(string name)
                    {
                        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        int asemblyCount = assemblies.Length;
                        for (int i = 0; i < asemblyCount; i++)
                        {
                            Type[] types = assemblies[i].GetTypes();
                            int typeCount = types.Length;
                            for (int j = 0; j < typeCount; j++)
                            {
                                if (types[j].Name == name) return types[j];
                            }
                        }

                        throw new InvalidOperationException($"Could not resolve type {name}");
                    }
                }
                else
                {
                    int m = responseAsString.IndexOf(":Fault>");
                    if (m == -1)
                    {
                        m = responseAsString.IndexOf("<Fault>");
                    }
                    if (m != -1)
                    {
                        m = responseAsString.IndexOf("<faultstring", m);
                        if (m != -1) //this might seem redundant but with that we have more chances to have an actual FaultException
                        {
                            m = responseAsString.IndexOf('>', m);
                            responseAsString = responseAsString.Remove(0, m + 1);
                            m = responseAsString.IndexOf("</faultstring");
                            responseAsString = responseAsString.Remove(m);
#if OPENSILVER
                            FaultException fe = new FaultException(); //todo: Add the message that is in responseAsString.
#else
                        FaultException fe = new FaultException(responseAsString);
#endif
                            raiseFaultException(fe);
                            return null;
                        }
                    }
                }
#else
                int m = responseAsString.IndexOf(":Fault>");
                if (m == -1)
                {
                    m = responseAsString.IndexOf("<Fault>");
                }
                if (m != -1)
                {
                    m = responseAsString.IndexOf("<faultstring", m);
                    if (m != -1) //this might seem redundant but with that we have more chances to have an actual FaultException
                    {
                        m = responseAsString.IndexOf('>', m);
                        responseAsString = responseAsString.Remove(0, m + 1);
                        m = responseAsString.IndexOf("</faultstring");
                        responseAsString = responseAsString.Remove(m);
#if OPENSILVER
                        FaultException fe = new FaultException(); //todo: Add the message that is in responseAsString.
#else
                        FaultException fe = new FaultException(responseAsString);
#endif
                        raiseFaultException(fe);
                        return null;
                    }
                }
#endif

                object requestResponse = null;

                //BRIDGE TODO
                //Implemente Type.isValueType (ticket is opened here :https://github.com/bridgedotnet/Bridge/issues/3329 )
#if !BRIDGE
                bool isResponseValueType = requestResponseType.IsValueType;

                if (!isResponseValueType)
#else
                if (true)
#endif
                {
                    //we deserialize the response
                    //in the case of an XmlSerializer:
                    //  - change the xsi:nil="true" or add the xsi namespace (probably better to add the namespace)
                    //  - directly use xDoc.Root instead of its Nodes (I think)

                    //  - change the type to deserialize to the type with the name : requestResponseType.Name + "Body"
                    FieldInfo bodyFieldInfo = null;

                    //we make sure this is not a method with no return type:
                    if (requestResponseType == typeof(object)) //Note: we test for the "Object" type since it is what we put instead of "void" to allow passing it as Generic type argument when calling CallWebMethod.
                    {
                        XDocument doc = XDocument.Parse(responseAsString);
                        XElement currentElement = doc.Root; //Current element is Envelope
                        currentElement = (XElement)currentElement.FirstNode; //Current element is now Body
                        currentElement = (XElement)currentElement.FirstNode; //Current element is now METHODNAMEResponse
                        if (currentElement.Nodes().Count() == 0)
                        {
                            //Note: there might be a more efficient way of checking if the method has a return type (possibly through a smart use of responseAsString.IndexOf but it seems complicated and not necessarily more efficient).
                            //this is a method with no return type, there is no need to read the response after checking that there was no FaultException.
                            return null;
                        }
                    }

                    Type typeToDeserialize = requestResponseType;
                    if (isXmlSerializerRatherThanDataContractSerializer)
                    {
                        bodyFieldInfo = requestResponseType.GetField("Body");
                        typeToDeserialize = bodyFieldInfo.FieldType;
                    }

                    //get the known types from the interface type:
                    List<Type> knownTypes = new List<Type>();
#if BRIDGE || CSHTML5BLAZOR
                    foreach (Attribute attribute in interfaceType.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), true))
#else
                    foreach (Attribute attribute in interfaceType.GetCustomAttributes(true))
#endif
                    {
#if !BRIDGE && !CSHTML5BLAZOR
                        if (attribute is ServiceKnownTypeAttribute)
#endif
                            knownTypes.Add(((ServiceKnownTypeAttribute)attribute).Type);
                    }

#if CSHTML5BLAZOR
                    DataContractSerializer_CSHTML5Ver deSerializer = new DataContractSerializer_CSHTML5Ver(typeToDeserialize, knownTypes);
                    XDocument xDoc = XDocument.Parse(responseAsString);
#else
                    DataContractSerializer deSerializer = new DataContractSerializer(typeToDeserialize, knownTypes);
                    XDocument xDoc = XDocument.Parse(responseAsString);
#endif
                    responseAsString = RemoveUnparsableStrings(responseAsString);
                    XElement xElement = xDoc.Root;

                    //exclude the parts that are <Enveloppe><Body>... since they are useless and would keep the deserialization from working properly:
                    //they should always be the two outermost elements:
                    
                    if (soapVersion == "1.2")
                    {
                        xElement = xElement.Element(XName.Get("Body", "http://www.w3.org/2003/05/soap-envelope"));
                    }
                    else
                    {
                        xElement = xElement.Elements().FirstOrDefault() ?? xElement; //move inside of the <Enveloppe> tag
                    }

                    //we check if the type is defined in the next XElement because it changes the XElement we want to use in that case.
                    // The reason is that the response uses one less XElement in the case where we use XmlSerializer and the method has the return Type object.
                    bool isTypeSpecified = false;
                    foreach (XAttribute attribute in xElement.Attributes(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance").GetName("type")))
                    {
                        isTypeSpecified = true;
                    }
                    if (!isXmlSerializerRatherThanDataContractSerializer || !isTypeSpecified)
                    {
                        //we are either not in the XmlSerializer version or we have the "extra" XElement so we move in once.
                        xElement = xElement.Elements().FirstOrDefault() ?? xElement; //move inside of the <Body> tag
                    }

                    if (!isXmlSerializerRatherThanDataContractSerializer)
                    {
                        xElement = xElement.Elements().FirstOrDefault() ?? xElement;

                        requestResponse = deSerializer.DeserializeFromXElement(xElement);
                    }
                    else
                    {
                        requestResponse = Activator.CreateInstance(requestResponseType);
                        object requestResponseBody = deSerializer.DeserializeFromXElement(xElement);
                        bodyFieldInfo.SetValue(requestResponse, requestResponseBody);
                    }
                }
                else
                {
                    //we remove the parts of the response string that are not the response itself. That is, we remove the "<s:Body>" so as to keep only its content:
                    string keyWord = ":Body";
                    int i = responseAsString.IndexOf(keyWord);
                    int k = responseAsString.IndexOf('>', i);
                    responseAsString = responseAsString.Remove(0, k + 1);
                    i = responseAsString.LastIndexOf(keyWord);
                    k = responseAsString.Substring(0, i).LastIndexOf('<');
                    responseAsString = responseAsString.Remove(k);

                    // Here we are dealing with a value type.
                    // Example:
                    //     <METHODNAMEResponse xmlns="http://tempuri.org/">
                    //         <METHODNAMEResult>10</METHODNAMEResult>
                    //     </METHODNAMEResponse>


                    //we look for :nil="true". Since we have a value type, it will mean that the value type is nullable and the value is null:
                    int indexOfNil = responseAsString.IndexOf(":nil=\"true\"");
                    if (indexOfNil == -1)
                    {
                        ////we remove the <MethodNameResponse> tag:
                        int ii = responseAsString.IndexOf('>', 0);
                        int jj = responseAsString.IndexOf('>', ii + 1);
                        int kk = responseAsString.IndexOf('<', jj + 1);
                        responseAsString = responseAsString.Substring(jj + 1, (kk - jj - 1));

                        //todo: support more value types?
                        //todo: handle nullables that are null
                        if (requestResponseType == typeof(int) || requestResponseType == typeof(int?))
                            requestResponse = int.Parse(responseAsString);
                        else if (requestResponseType == typeof(long) || requestResponseType == typeof(long?))
                            requestResponse = long.Parse(responseAsString);
                        else if (requestResponseType == typeof(bool) || requestResponseType == typeof(bool?))
                            requestResponse = bool.Parse(responseAsString);
                        else if (requestResponseType == typeof(float) || requestResponseType == typeof(float?))
                            requestResponse = float.Parse(responseAsString); //todo: ensure this is the culture-invariant parsing!
                        else if (requestResponseType == typeof(double) || requestResponseType == typeof(double?))
                            requestResponse = double.Parse(responseAsString); //todo: ensure this is the culture-invariant parsing!
                        else if (requestResponseType == typeof(char) || requestResponseType == typeof(char?))
                            requestResponse = (char)(int.Parse(responseAsString)); //todo: support encodings
                        else if (requestResponseType == typeof(DateTime) || requestResponseType == typeof(DateTime?))
                            requestResponse = INTERNAL_DateTimeHelpers.ToDateTime(responseAsString); //todo: ensure this is the culture-invariant parsing!
                        else
                            throw new NotSupportedException("The following type is not supported in the current WCF implementation: " + requestResponseType.ToString() + ". Please report this issue to support@cshtml5.com");
                    }
                    else
                    {
                        if (requestResponseType.FullName.StartsWith("System.Nullable`1"))
                        {
                            return null;
                        }
                        else
                        {
                            return Activator.CreateInstance(requestResponseType);
                        }
                    }
                }

                return requestResponse;
            }

            // Now that we use the javascript function ParseFromString we need to remove unsparable strings from the response
            // To Do : replace the encoding of the soap response so there are no unparsable strings
            private string RemoveUnparsableStrings(string str)
            {
                string[] unparsableStrings = new string[]
                {
                    "&#x1A;"
                };

                foreach (string unparsableString in unparsableStrings)
                {
                    str = str.Replace(unparsableString, "");
                }
                return str;
            }

        }
#endif

#if WORKINPROGRESS && (!CSHTML5BLAZOR || WORKINPROGRESS)
#region Not Supported Stuff

        //    /// <summary>
        //    /// Gets the underlying System.ServiceModel.ChannelFactory<TChannel> object.
        //    /// </summary>
        //    public ChannelFactory<TChannel> ChannelFactory { get; }

        //    /// <summary>
        //    /// Gets the client credentials used to call an operation.
        //    /// </summary>
        //    public ClientCredentials ClientCredentials { get; }

        //    /// <summary>
        //    /// Gets the target endpoint for the service to which the WCF client can connect.
        //    /// </summary>
        //    public ServiceEndpoint Endpoint { get; }

        /// <summary>
        /// Gets the underlying System.ServiceModel.IClientChannel implementation.
        /// </summary>
        public IClientChannel InnerChannel
        {
            get { return null; }
        }

        public void Abort()
        {

        }

        public CommunicationState State
        {
            get { return CommunicationState.Created; }
        }


        //    /// <summary>
        //    /// Returns a new channel to the service.
        //    /// </summary>
        //    /// <returns>A channel of the type of the service contract.</returns>
        protected virtual TChannel CreateChannel()
        {
            return null;
        }

        //    /// <summary>
        //    /// Replicates the behavior of the default keyword in C#.
        //    /// </summary>
        //    /// <typeparam name="T">The type that is identified as reference or numeric by the keyword.</typeparam>
        //    /// <returns>Returns null if T is a reference type and zero if T is a numeric value type.</returns>
        //    protected T GetDefaultValueForInitialization<T>();

        /// <summary>
        /// Generic ChannelBase class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        //protected class ChannelBase<T> : IOutputChannel, IRequestChannel, IClientChannel, IDisposable, IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel> where T : class
        protected class ChannelBase<T> where T : class
        {
            /// <summary>
            /// Initializes a new instance of the System.ServiceModel.ClientBase<TChannel>.ChannelBase<T>
            /// class from an existing instance of the class.
            /// </summary>
            /// <param name="client">The object used to initialize the new instance of the class.</param>
            protected ChannelBase(CSHTML5_ClientBase<T> client)
            {

            }

            /// <summary>
            /// Starts an asynchronous call of a specified method by name.
            /// </summary>
            /// <param name="methodName">The name of the method to be called asynchronously.</param>
            /// <param name="args">An array of arguments for the method invoked.</param>
            /// <param name="callback">The System.AsyncCallback delegate.</param>
            /// <param name="state">The state object.</param>
            /// <returns>The System.IAsyncResult that references the asynchronous method invoked.</returns>
            //[SecuritySafeCritical]
            protected IAsyncResult BeginInvoke(string methodName, object[] args, AsyncCallback callback, object state)
            {
                return null;
            }

            /// <summary>
            /// Completes an asynchronous invocation by name of a specified method.
            /// </summary>
            /// <param name="methodName">The name of the method called asynchronously.</param>
            /// <param name="args">An array of arguments for the method invoked.</param>
            /// <param name="result">The result returned by a call.</param>
            /// <returns>The System.Object output by the method invoked.</returns>
            //[SecuritySafeCritical]
            protected object EndInvoke(string methodName, object[] args, IAsyncResult result)
            {
                return null;
            }
        }


#endregion
#endif

#if WORKINPROGRESS && !CSHTML5BLAZOR
#region ICommunicationObject methods

        CommunicationState ICommunicationObject.State
        {
            get { return CommunicationState.Created; }
        }

        event EventHandler ICommunicationObject.Closed
        {
            add { }
            remove { }
        }

        event EventHandler ICommunicationObject.Closing
        {
            add { }
            remove { }
        }

        event EventHandler ICommunicationObject.Faulted
        {
            add { }
            remove { }
        }

        event EventHandler ICommunicationObject.Opened
        {
            add { }
            remove { }
        }

        event EventHandler ICommunicationObject.Opening
        {
            add { }
            remove { }
        }

        void ICommunicationObject.Abort()
        {

        }

        void ICommunicationObject.Close()
        {

        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {

        }

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            return null;
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {

        }

        void ICommunicationObject.Open()
        {

        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {

        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return null;
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {

        }

        //void ICommunicationObject.Dispose()
        //{

        //}
#endregion
#endif
    }
}