

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

using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.ServiceModel.Channels;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using CSHTML5.Internal;
using DataContractSerializerCustom = System.Runtime.Serialization.DataContractSerializer_CSHTML5Ver;
using System.Globalization;

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
    public abstract partial class CSHTML5_ClientBase<TChannel> /*: ICommunicationObject, IDisposable*/ where TChannel : class
    {
        //Note: Adding this because they are in the file generated when adding a Service Reference through the "Add Connected Service" for OpenSilver.
        public Description.ServiceEndpoint Endpoint { get; } = new Description.ServiceEndpoint(new Description.ContractDescription("none"));
        public Description.ClientCredentials ClientCredentials { get; } = new Description.ClientCredentials();

        public TChannel Channel { get; }

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
            AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userState);
            AsyncOperationContext context = new AsyncOperationContext(asyncOperation, endOperationDelegate, operationCompletedCallback);
            IAsyncResult result = beginOperationDelegate(inValues, OnAsyncCallCompleted, context);
        }

        static void OnAsyncCallCompleted(IAsyncResult result)
        {
            AsyncOperationContext context = (AsyncOperationContext)result.AsyncState;
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
                InvokeAsyncCompletedEventArgs e = new InvokeAsyncCompletedEventArgs(results, error, false, context.AsyncOperation.UserSuppliedState);
                context.AsyncOperation.PostOperationCompleted(context.CompletionCallback, e);
            }
            else
            {
                context.AsyncOperation.OperationCompleted();
            }
        }

        class AsyncOperationContext
        {
            AsyncOperation asyncOperation;
            EndOperationDelegate endDelegate;
            SendOrPostCallback completionCallback;

            internal AsyncOperationContext(AsyncOperation asyncOperation, EndOperationDelegate endDelegate, SendOrPostCallback completionCallback)
            {
                this.asyncOperation = asyncOperation;
                this.endDelegate = endDelegate;
                this.completionCallback = completionCallback;
            }

            internal AsyncOperation AsyncOperation
            {
                get
                {
                    return this.asyncOperation;
                }
            }

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

        public string INTERNAL_RemoteAddressAsString { get; }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the default target endpoint from the application configuration
        /// file.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Either there is no default endpoint information in the configuration file,
        /// more than one endpoint in the file, or no configuration file.
        /// </exception>
        protected CSHTML5_ClientBase()
        {
            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
        }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the configuration information specified in the application configuration
        /// file by endpointConfigurationName.
        /// </summary>
        /// <param name="endpointConfigurationName">
        /// The name of the endpoint in the application configuration file.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified endpoint information is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The endpoint cannot be found or the endpoint contract is not valid.
        /// </exception>
        protected CSHTML5_ClientBase(string endpointConfigurationName)
        {
            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the specified binding and target address.
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        /// <exception cref="ArgumentNullException">
        /// The binding is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The remote address is null.
        /// </exception>
        protected CSHTML5_ClientBase(Binding binding, EndpointAddress remoteAddress)
        {
            if (remoteAddress == null)
            {
                throw new ArgumentNullException("remoteAddress");
            }

            INTERNAL_RemoteAddressAsString = remoteAddress.Uri.OriginalString;

            //todo: finish the implementation.
        }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class using the specified target address and endpoint information.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="ArgumentNullException">
        /// The endpoint is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The remote address is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The endpoint cannot be found or the endpoint contract is not valid.
        /// </exception>
        protected CSHTML5_ClientBase(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ClientBase`1
        /// class.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="ArgumentNullException">
        /// The endpoint is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The remote address is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The endpoint cannot be found or the endpoint contract is not valid.
        /// </exception>
        protected CSHTML5_ClientBase(string endpointConfigurationName, string remoteAddress)
        {
            throw new NotSupportedException("Please specify the Binding and Endpoint programmatically. See http://www.cshtml5.com/links/wcf-limitations-and-tutorials.aspx for details.");
            //todo
        }

        /// <summary>
        /// Provides an API to call web methods defined in a WebService
        /// </summary>
        public partial class WebMethodsCaller
        {
            private const string XMLSCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema-instance"; // Usually associated to the "xsi:" prefix.
            private const string DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE = "http://schemas.datacontract.org/2004/07/";

            string _addressOfService;

            INTERNAL_WebRequestHelper_JSOnly _webRequestHelper_JSVersion = new INTERNAL_WebRequestHelper_JSOnly();

            /// <summary>
            /// Constructor for the WebMethodsCaller's class
            /// </summary>
            /// <param name="addressOfService">The address of the WebService</param>
            public WebMethodsCaller(string addressOfService)
            {
                _addressOfService = addressOfService;
            }

            public void BeginCallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IDictionary<string, object> originalRequestObject,
                Action<string> callback,
                string soapVersion)
            {
                BeginCallWebMethod(webMethodName, interfaceType, methodReturnType, null, "", originalRequestObject,
                    callback, soapVersion);
            }

            public void BeginCallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IEnumerable<MessageHeader> outgoingMessageHeaders,
                IDictionary<string, object> originalRequestObject,
                Action<string> callback,
                string soapVersion)
            {
                BeginCallWebMethod(webMethodName, interfaceType, methodReturnType, null,
                    GetEnvelopeHeaders(outgoingMessageHeaders?.ToList(), soapVersion), originalRequestObject,
                    callback, soapVersion);
            }

            public void BeginCallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                string messageHeaders,
                IDictionary<string, object> originalRequestObject,
                Action<string> callback,
                string soapVersion)
            {
                BeginCallWebMethod(webMethodName, interfaceType, methodReturnType, null,
                    messageHeaders, originalRequestObject, callback, soapVersion);
            }

            public void BeginCallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IReadOnlyList<Type> knownTypes,
                string messageHeaders,
                IDictionary<string, object> originalRequestObject,
                Action<string> callback,
                string soapVersion)
            {
                MethodInfo method = ResolveMethod(interfaceType, webMethodName, "Begin" + webMethodName);
                bool isXmlSerializer = IsXmlSerializer(webMethodName, methodReturnType, method);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(
                    webMethodName,
                    method,
                    interfaceType,
                    methodReturnType,
                    knownTypes,
                    messageHeaders,
                    originalRequestObject,
                    soapVersion,
                    isXmlSerializer,
                    out headers,
                    out request);

                Uri address = INTERNAL_UriHelper.EnsureAbsoluteUri(_addressOfService);

                // Make the actual web service call
                _webRequestHelper_JSVersion.MakeRequest(
                    address,
                    "POST",
                    this,
                    headers,
                    request,
                    (sender, e) =>
                    {
                        string xmlReturnedFromTheServer = e.Result;
                        callback(xmlReturnedFromTheServer);
                    },
                    true,
                    Application.Current.Host.Settings.DefaultSoapCredentialsMode);
            }

            public object EndCallWebMethod(
               string webMethodName,
               Type interfaceType,
               Type methodReturnType,
               string xmlReturnedFromTheServer,
               string soapVersion)
            {
                return EndCallWebMethod(webMethodName,
                     interfaceType,
                     methodReturnType,
                     null,
                     xmlReturnedFromTheServer,
                     soapVersion);
            }

            public object EndCallWebMethod(
                     string webMethodName,
                     Type interfaceType,
                     Type methodReturnType,
                     IReadOnlyList<Type> knownTypes,
                     string xmlReturnedFromTheServer,
                     string soapVersion)
            {
                MethodInfo beginMethod = ResolveMethod(interfaceType, webMethodName, "Begin" + webMethodName);
                bool isXmlSerializer = IsXmlSerializer(webMethodName,
                                                       methodReturnType,
                                                       beginMethod);

                object requestResponse = ReadAndPrepareResponse(
                    xmlReturnedFromTheServer,
                    interfaceType,
                    methodReturnType,
                    knownTypes,
                    static faultException => throw faultException,
                    isXmlSerializer,
                    soapVersion);

                return requestResponse;
            }

            public RETURN_TYPE EndCallWebMethod<RETURN_TYPE>(
                string webMethodName,
                Type interfaceType,
                string xmlReturnedFromTheServer,
                string soapVersion)
            {
                return (RETURN_TYPE)EndCallWebMethod(
                    webMethodName,
                    interfaceType,
                    typeof(RETURN_TYPE),
                    xmlReturnedFromTheServer,
                    soapVersion);
            }

            internal Task<T> CallWebMethodAsyncBeginEnd<T>(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IDictionary<string, object> originalRequestObject,
                string soapVersion)
            {
                TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

                AsyncCallback callback = new AsyncCallback(delegate (IAsyncResult asyncResponseResult)
                {
                    try
                    {
                        T result = EndCallWebMethod<T>(
                            webMethodName,
                            interfaceType,
                            ((WebMethodAsyncResult)asyncResponseResult).XmlReturnedFromTheServer,
                            soapVersion);
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });
                object asyncState = null;

                WebMethodAsyncResult webMethodAsyncResult = new WebMethodAsyncResult(callback, asyncState);

                BeginCallWebMethod(
                    webMethodName,
                    interfaceType,
                    methodReturnType,
                    originalRequestObject,
                    (xmlReturnedFromTheServer) =>
                    {
                        // After server call has finished (not deserialized yet)
                        webMethodAsyncResult.XmlReturnedFromTheServer = xmlReturnedFromTheServer;

                        // This causes a call to "EndCallWebMethod" which will deserialize the response.
                        webMethodAsyncResult.Completed();
                    },
                    soapVersion);

                return tcs.Task;
            }

            internal Task<(T, MessageHeaders)> CallWebMethodAsyncBeginEnd<T>(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IEnumerable<MessageHeader> outgoingMessageHeaders,
                IDictionary<string, object> originalRequestObject,
                string soapVersion)
            {
                TaskCompletionSource<(T, MessageHeaders)> tcs = new TaskCompletionSource<(T, MessageHeaders)>();

                AsyncCallback callback = new AsyncCallback(delegate (IAsyncResult asyncResponseResult)
                {
                    try
                    {
                        T result = EndCallWebMethod<T>(
                            webMethodName,
                            interfaceType,
                            ((WebMethodAsyncResult)asyncResponseResult).XmlReturnedFromTheServer,
                            soapVersion);

                        var messageHeaders = GetEnvelopeHeaders(((WebMethodAsyncResult)asyncResponseResult).XmlReturnedFromTheServer, soapVersion);

                        tcs.SetResult((result, messageHeaders));
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });
                object asyncState = null;

                WebMethodAsyncResult webMethodAsyncResult = new WebMethodAsyncResult(callback, asyncState);

                BeginCallWebMethod(
                    webMethodName,
                    interfaceType,
                    methodReturnType,
                    outgoingMessageHeaders,
                    originalRequestObject,
                    (xmlReturnedFromTheServer) =>
                    {
                        // After server call has finished (not deserialized yet)
                        webMethodAsyncResult.XmlReturnedFromTheServer = xmlReturnedFromTheServer;

                        // This causes a call to "EndCallWebMethod" which will deserialize the response.
                        webMethodAsyncResult.Completed();
                    },
                    soapVersion);

                return tcs.Task;
            }

            /// <summary>
            /// Asynchronously calls a WebMethod.
            /// </summary>
            /// <typeparam name="T">The return type of the WebMethod</typeparam>
            /// <param name="webMethodName">The name of the WebMethod</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType">The return Type of the method</param>
            /// <param name="outgoingMessageHeaders">The outgoing message headers</param>
            /// <param name="originalRequestObject">The additional arguments of the method</param>
            /// <param name="soapVersion">The SOAP Version of the request</param>
            /// <returns>The result of the call of the method and the incoming message headers.</returns>
            public Task<(T, MessageHeaders)> CallWebMethodAsync<T>(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IEnumerable<MessageHeader> outgoingMessageHeaders,
                IDictionary<string, object> originalRequestObject,
                string soapVersion) // Note: we don't arrive here using c#
            {
                // todo: find out what happens with methods that take multiple arguments 
                // (if possible) and change the parameterName to a string[].
                MethodInfo method = ResolveMethod(interfaceType, webMethodName, webMethodName + "Async");
                bool isXmlSerializer = IsXmlSerializer(webMethodName, methodReturnType, method);
                string outgoingMessageHeadersString = GetEnvelopeHeaders(outgoingMessageHeaders?.ToList(), soapVersion);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(
                    webMethodName,
                    method,
                    interfaceType,
                    methodReturnType,
                    null,
                    outgoingMessageHeadersString,
                    originalRequestObject,
                    soapVersion,
                    isXmlSerializer,
                    out headers,
                    out request);

                var tcs = new TaskCompletionSource<(T, MessageHeaders)>(); //todo: here we need to change object to the return type

                string response = _webRequestHelper_JSVersion.MakeRequest(
                    new Uri(_addressOfService),
                    "POST",
                    this,
                    headers,
                    request,
                    (sender, args2) =>
                    {
                        ReadAndPrepareResponseGeneric_JSVersion(
                            tcs,
                            args2,
                            interfaceType,
                            methodReturnType,
                            null,
                            isXmlSerializer,
                            soapVersion);
                    },
                    true,
                    Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                return tcs.Task;
            }

            /// <summary>
            /// Asynchronously calls a WebMethod.
            /// </summary>
            /// <typeparam name="T">The return type of the WebMethod</typeparam>
            /// <param name="webMethodName">The name of the WebMethod</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType">The return Type of the method</param>
            /// <param name="originalRequestObject">The additional arguments of the method</param>
            /// <param name="soapVersion">The SOAP Version of the request</param>
            /// <returns>The result of the call of the method.</returns>
            public Task<T> CallWebMethodAsync<T>(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IDictionary<string, object> originalRequestObject,
                string soapVersion) // Note: we don't arrive here using c#
            {
                // todo: find out what happens with methods that take multiple arguments 
                // (if possible) and change the parameterName to a string[].
                MethodInfo method = ResolveMethod(interfaceType, webMethodName, webMethodName + "Async");
                bool isXmlSerializer = IsXmlSerializer(webMethodName, methodReturnType, method);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(
                    webMethodName,
                    method,
                    interfaceType,
                    methodReturnType,
                    null,
                    "",
                    originalRequestObject,
                    soapVersion,
                    isXmlSerializer,
                    out headers,
                    out request);

                var tcs = new TaskCompletionSource<T>(); //todo: here we need to change object to the return type

                string response = _webRequestHelper_JSVersion.MakeRequest(
                    new Uri(_addressOfService),
                    "POST",
                    this,
                    headers,
                    request,
                    (sender, args2) =>
                    {
                        ReadAndPrepareResponseGeneric_JSVersion(
                            tcs,
                            args2,
                            interfaceType,
                            methodReturnType,
                            null,
                            isXmlSerializer,
                            soapVersion);
                    },
                    true,
                    Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                return tcs.Task;
            }

            /// <summary>
            /// Calls a WebMethod
            /// </summary>
            /// <param name="webMethodName">The name of the Method</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType">The return Type of the method</param>
            /// <param name="originalRequestObject">The additional arguments of the method</param>
            /// <param name="soapVersion">The SOAP Version of the request</param>
            /// <returns>The result of the call of the method.</returns>
            public object CallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IDictionary<string, object> originalRequestObject,
                string soapVersion) // Note: we don't arrive here using c#.
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

                MethodInfo method = ResolveMethod(interfaceType, webMethodName, webMethodName, "Begin" + webMethodName);
                bool isXmlSerializer = IsXmlSerializer(webMethodName, methodReturnType, method);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(
                    webMethodName,
                    method,
                    interfaceType,
                    methodReturnType,
                    null,
                    "",
                    originalRequestObject,
                    soapVersion,
                    isXmlSerializer,
                    out headers,
                    out request);

                string response = _webRequestHelper_JSVersion.MakeRequest(
                        new Uri(_addressOfService),
                        "POST",
                        this,
                        headers,
                        request,
                        null,
                        false,
                        Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                return ReadAndPrepareResponse(
                    response,
                    interfaceType,
                    methodReturnType,
                    null,
                    faultException => throw faultException,
                    isXmlSerializer,
                    soapVersion);
            }

            /// <summary>
            /// Calls a WebMethod
            /// </summary>
            /// <param name="webMethodName">The name of the Method</param>
            /// <param name="interfaceType">The Type of the interface</param>
            /// <param name="methodReturnType">The return Type of the method</param>
            /// <param name="outgoingMessageHeaders">The outgoing message headers</param>
            /// <param name="originalRequestObject">The additional arguments of the method</param>
            /// <param name="soapVersion">The SOAP Version of the request</param>
            /// <returns>The result of the call of the method and the incoming message headers.</returns>
            public (object, MessageHeaders) CallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IEnumerable<MessageHeader> outgoingMessageHeaders,
                IDictionary<string, object> originalRequestObject,
                string soapVersion) // Note: we don't arrive here using c#.
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

                MethodInfo method = ResolveMethod(interfaceType, webMethodName, webMethodName, "Begin" + webMethodName);
                bool isXmlSerializer = IsXmlSerializer(webMethodName, methodReturnType, method);
                var outgoingMessageHeadersString = GetEnvelopeHeaders(outgoingMessageHeaders?.ToList(), soapVersion);

                Dictionary<string, string> headers;
                string request;
                PrepareRequest(
                    webMethodName,
                    method,
                    interfaceType,
                    methodReturnType,
                    null,
                    outgoingMessageHeadersString,
                    originalRequestObject,
                    soapVersion,
                    isXmlSerializer,
                    out headers,
                    out request);

                string response = _webRequestHelper_JSVersion.MakeRequest(
                        new Uri(_addressOfService),
                        "POST",
                        this,
                        headers,
                        request,
                        null,
                        false,
                        Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                var typedResponseBody = ReadAndPrepareResponse(
                    response,
                    interfaceType,
                    methodReturnType,
                    null,
                    faultException => throw faultException,
                    isXmlSerializer,
                    soapVersion);

                var incomingMessageHeaders = GetEnvelopeHeaders(response, soapVersion);

                return (typedResponseBody, incomingMessageHeaders);
            }

            private static MethodInfo ResolveMethod(Type interfaceType, string webMethodName, string methodName1, string methodName2 = null)
            {
                if (interfaceType.GetMethod(methodName1) is MethodInfo method1)
                {
                    return method1;
                }

                if (methodName2 is not null && interfaceType.GetMethod(methodName2) is MethodInfo method2)
                {
                    return method2;
                }

                throw new MissingMethodException($"Cannot find an operation named '{webMethodName}'.");
            }

            private static bool IsXmlSerializer(
                string webMethodName,
                Type methodReturnType,
                MethodInfo method)
            {
                if (methodReturnType != null)
                {
                    if (methodReturnType.Name == webMethodName + "Response" &&
                        methodReturnType.GetField("Body") != null)
                    {
                        return true;
                    }
                }

                if (method != null)
                {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    if (methodReturnType == typeof(IAsyncResult) &&
                       (parameterInfos != null && parameterInfos.Length > 0) &&
                        parameterInfos[0].ParameterType.Name == webMethodName + "Request")
                    {
                        return true;
                    }
                }

                return false;
            }

            private static string GetEnvelopeHeaders(ICollection<MessageHeader> messageHeaders, string soapVersion)
            {
                if (messageHeaders == null || !messageHeaders.Any())
                {
                    return "";
                }

                var settings = new XmlWriterSettings { OmitXmlDeclaration = true };

                return string.Join("", messageHeaders.Select(mh =>
                {
                    using (var sw = new StringWriter())
                    using (var xw = XmlWriter.Create(sw, settings))
                    {
                        mh.WriteHeader(xw, soapVersion == "1.1" ? MessageVersion.Soap11 : MessageVersion.Soap12WSAddressing10);

                        xw.Flush();
                        return sw.ToString();
                    }
                }));
            }

            private static MessageHeaders GetEnvelopeHeaders(string incomingMessageString, string soapVersion)
            {
                using (var reader = XmlReader.Create(new StringReader(incomingMessageString)))
                {
                    var incomingMessage = Message.CreateMessage(reader, int.MaxValue, soapVersion == "1.1" ? MessageVersion.Soap11 : MessageVersion.Soap12WSAddressing10);
                    return incomingMessage.Headers;
                }
            }

            private void ProcessNode(XElement node, Action<XElement> action)
            {
                action(node);
                foreach (XElement child in node.Elements())
                {
                    ProcessNode(child, action);
                }
            }

            private void PrepareRequest(
                string webMethodName, // webMethod
                MethodInfo method, // method to look for in 'interfaceType'
                Type interfaceType,
                Type methodReturnType,
                IReadOnlyList<Type> knownTypes,
                string envelopeHeaders,
                IDictionary<string, object> requestParameters,
                string soapVersion,
                bool isXmlSerializer,
                out Dictionary<string, string> headers,
                out string request)
            {
                headers = new Dictionary<string, string>();

                string interfaceTypeName = interfaceType.Name; // default value
                string interfaceTypeNamespace = "http://tempuri.org/"; // default value

                if (interfaceType.GetCustomAttributes<ServiceContractAttribute>(false).FirstOrDefault() is ServiceContractAttribute serviceContractAttr)
                {
                    if (serviceContractAttr.Namespace is not null)
                    {
                        interfaceTypeNamespace = serviceContractAttr.Namespace;
                    }
                    if (!string.IsNullOrEmpty(serviceContractAttr.Name))
                    {
                        interfaceTypeName = serviceContractAttr.Name;
                    }
                }

                // in every case, we want the name of the method as a XElement
                var methodNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(webMethodName));

                // Note: now we want to add the parameters of the method
                // to do that, we basically get the serialized version of the objects, 
                // and replace their tag that should have the type with the parameter name.
                if (requestParameters != null)
                {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    int parametersCount = requestParameters != null ?
                                          requestParameters.Count :
                                          0;

                    for (int i = 0; i < parametersCount; ++i)
                    {
                        object requestBody = requestParameters[parameterInfos[i].Name];
                        if (requestBody != null)
                        {
                            var types = new List<Type>(knownTypes ?? Enumerable.Empty<Type>());
                            types.AddRange(interfaceType.GetCustomAttributes<ServiceKnownTypeAttribute>(true).Select(o => o.Type));

                            var dataContractSerializer = new DataContractSerializerCustom(
                                parameterInfos[i].ParameterType.IsByRef ? parameterInfos[i].ParameterType.GetElementType(): parameterInfos[i].ParameterType,
                                types,
                                isXmlSerializer);

                            XDocument xdoc = dataContractSerializer.SerializeToXDocument(requestBody);

                            if (!isXmlSerializer)
                            {
                                var paramNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(parameterInfos[i].Name));

                                // we don't want to add this in the case of an XmlSerializer 
                                // because it would be <request> which is not what we want. 
                                // The correct parameter name is alread in the Request body.
                                methodNameElement.Add(paramNameElement);
                                foreach (XNode currentNode in xdoc.Root.Nodes())
                                {
                                    paramNameElement.Add(currentNode);
                                }
                                foreach (XAttribute currentAttribute in xdoc.Root.Attributes())
                                {
                                    // we don't want to keep the "xmlns="http://schemas.microsoft.com/2003/10/Serialization/" 
                                    // because it breaks the request.
                                    if (!currentAttribute.IsNamespaceDeclaration)
                                    {
                                        paramNameElement.Add(currentAttribute);
                                    }
                                }
                            }
                            else
                            {
                                //we assume that it always has the same structure 
                                // <root>
                                //   <AddOrUpdateToDoRequest xmlns="http://schemas.datacontract.org/2004/07/">
                                //      <Body>
                                //         <toDoItem
                                // so we want to go to xdoc.Root.Nodes()[0].Nodes()
                                foreach (XElement xElement in xdoc.Root.Nodes().OfType<XElement>())
                                {
                                    foreach (XElement node in xElement.Elements())
                                    {
                                        ProcessNode(node, x => x.Name = XNamespace.Get(string.IsNullOrEmpty(x.Name.NamespaceName) ?
                                                                                       interfaceTypeNamespace :
                                                                                       x.Name.NamespaceName)
                                                                                  .GetName(x.Name.LocalName));
                                        methodNameElement.Add(node);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // the value is null so we simply need to put the parameter name with i:nil="true" and we're good
                            var paramNameElement = new XElement(XNamespace.Get(interfaceTypeNamespace).GetName(parameterInfos[i].Name));
                            var attribute = new XAttribute(XNamespace.Get(XMLSCHEMA_NAMESPACE).GetName("nil"), "true");
                            paramNameElement.Add(attribute);
                            methodNameElement.Add(paramNameElement);
                        }
                    }
                }

                string elementAsString = DataContractSerializerCustom.XElementToString(methodNameElement);

                // Look for the soapAction.
                string soapAction = string.Empty;

                if (method.GetCustomAttributes<OperationContractAttribute>(false).FirstOrDefault() is OperationContractAttribute operationContractAttr)
                {
                    soapAction = operationContractAttr.Action;
                }

                if (string.IsNullOrEmpty(soapAction))
                {
                    soapAction = $"{interfaceTypeNamespace.Trim('/')}/{interfaceTypeName.Trim('/')}/{webMethodName}";
                }

                switch (soapVersion)
                {
                    case "1.1":
                        headers.Add("Content-Type", "text/xml; charset=utf-8");
                        headers.Add("SOAPAction", soapAction);

                        if (!string.IsNullOrEmpty(envelopeHeaders))
                        {
                            envelopeHeaders = "<s:Header>" + envelopeHeaders + "</s:Header>";
                        }

                        request = $"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">{(envelopeHeaders ?? string.Empty)}<s:Body>{elementAsString}</s:Body></s:Envelope>";
                        break;

                    case "1.2":
                        headers.Add("Content-Type", "application/soap+xml; charset=utf-8");

                        request = $"<s:Envelope xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\"><s:Header><a:Action>{soapAction}</a:Action>{(envelopeHeaders ?? string.Empty)}<a:To>{_addressOfService}</a:To></s:Header><s:Body>{elementAsString}</s:Body></s:Envelope>";
                        break;

                    default:
                        throw new InvalidOperationException($"SOAP version not supported: {soapVersion}");
                }
            }

            private void ReadAndPrepareResponseGeneric_JSVersion<T>(
                TaskCompletionSource<T> taskCompletionSource,
                INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e,
                Type interfaceType,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                bool isXmlSerializer,
                string soapVersion)
            {
                if (e.Error != null && string.IsNullOrEmpty(e.Result))
                {
                    taskCompletionSource.TrySetException(e.Error);
                }
                else
                {
                    T requestResponse = (T)ReadAndPrepareResponse(
                        e.Result,
                        interfaceType,
                        requestResponseType,
                        knownTypes,
                        faultException => taskCompletionSource.TrySetException(faultException),
                        isXmlSerializer,
                        soapVersion);

                    // Note: this Task.IsCompleted can be true if we met an exception 
                    // which triggered a call to TrySetException (above).
                    if (!taskCompletionSource.Task.IsCompleted)
                    {
                        taskCompletionSource.SetResult(requestResponse);
                    }
                }
            }

            private void ReadAndPrepareResponseGeneric_JSVersion<T>(
                TaskCompletionSource<(T, MessageHeaders)> taskCompletionSource,
                INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e,
                Type interfaceType,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                bool isXmlSerializer,
                string soapVersion)
            {
                if (e.Error == null)
                {
                    T requestResponse = (T)ReadAndPrepareResponse(
                        e.Result,
                        interfaceType,
                        requestResponseType,
                        knownTypes,
                        faultException => taskCompletionSource.TrySetException(faultException),
                        isXmlSerializer,
                        soapVersion);

                    // Note: this Task.IsCompleted can be true if we met an exception 
                    // which triggered a call to TrySetException (above).
                    if (!taskCompletionSource.Task.IsCompleted)
                    {
                        taskCompletionSource.SetResult((requestResponse, GetEnvelopeHeaders(e.Result, soapVersion)));
                    }
                }
                else
                {
                    taskCompletionSource.TrySetException(e.Error);
                }
            }

            private static FaultException GetFaultException(string response, bool useXmlSerializerFormat)
            {
                const string ns = "http://schemas.xmlsoap.org/soap/envelope/";

                VerifyThatResponseIsNotNullOrEmpty(response);
                var faultElement = DataContractSerializerCustom.ParseToXDocument(response).Root
                                                 .Element(XName.Get("Body", ns))
                                                 .Element(XName.Get("Fault", ns));

                if (faultElement == null)
                {
                    return new FaultException();
                }

                var faultStringElement = faultElement.Element(XName.Get("faultstring"));
                var faultReasonValue = faultStringElement?.Value;
                var lang = faultStringElement?.Attribute(XName.Get("lang", XNamespace.Xml.NamespaceName))?.Value;
                var faultReasonText = string.IsNullOrEmpty(lang)
                    ? new FaultReasonText(faultReasonValue)
                    : new FaultReasonText(faultReasonValue, lang);
                var reason = new FaultReason(faultReasonText);

                var faultCodeElement = faultElement.Element(XName.Get("faultcode"));
                var code = new FaultCode(faultCodeElement?.Value);

                var detailElement = faultElement.Element(XName.Get("detail"));
                if (detailElement == null)
                {
                    return new FaultException(reason, code, null);
                }

                detailElement = detailElement.Elements().First();
                var detailType = ResolveType(detailElement.Name, useXmlSerializerFormat);

                var serializer = new DataContractSerializerCustom(detailType);

                var detail = serializer.DeserializeFromXElement(detailElement);

                var type = typeof(FaultException<>).MakeGenericType(detailType);

                return (FaultException)Activator.CreateInstance(type, detail, reason, code, null);

                static Type ResolveType(XName name, bool useXmlSerializerFormat)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; ++i)
                    {
                        Type[] types = assemblies[i].GetTypes();
                        for (int j = 0; j < types.Length; ++j)
                        {
                            Type type = types[j];
                            object[] attrs = type.GetCustomAttributes(typeof(DataContractAttribute), false);
                            DataContractAttribute attr = attrs != null && attrs.Length > 0 ?
                                                         (DataContractAttribute)attrs[0] :
                                                         null;

                            if (attr != null)
                            {
                                bool nameMatch = attr.IsNameSetExplicitly ?
                                    attr.Name == name.LocalName :
                                    type.Name == name.LocalName;

                                if (nameMatch)
                                {
                                    bool namespaceMatch = attr.IsNamespaceSetExplicitly ?
                                        attr.Namespace == name.NamespaceName :
                                        GetDefaultNamespace(type.Namespace, useXmlSerializerFormat) == name.NamespaceName;

                                    if (namespaceMatch)
                                        return type;
                                }
                            }
                        }
                    }

                    throw new InvalidOperationException($"Could not resolve type {name}");
                }
            }

            private static object ReadAndPrepareResponse(
                string responseAsString,
                Type interfaceType,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                Action<FaultException> raiseFaultException,
                bool isXmlSerializer,
                string soapVersion)
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
                // handle the case where the server has sent a FaultException: 
                // (this is what it looks like when the exception is of type FaultException, 
                // we need to try with a custom FaultException)
                //-----------------------------------------------------------
                //<s:Fault>
                //    <faultcode>s:Client</faultcode>
                //    <faultstring xml:lang="fr-FR">id already exists</faultstring>
                //</s:Fault>
                // we make our own exception. Because it is easier that way (it will probably
                // require an actual deserialization of the users' FaultException later on, to 
                // be able to support their custom ones).

                VerifyThatResponseIsNotNullOrEmpty(responseAsString);
                string NS;
                if (soapVersion == "1.1")
                {
                    NS = "http://schemas.xmlsoap.org/soap/envelope/";
                }
                else
                {
                    Debug.Assert(soapVersion == "1.2", $"Unexpected soap version ({soapVersion}) !");
                    NS = "http://www.w3.org/2003/05/soap-envelope";
                }

                XElement envelopeElement = null;
                XElement headerElement = null;
                XElement bodyElement = null;

                // Error parsing, if applicable
                if (soapVersion == "1.2")
                {
                    envelopeElement = DataContractSerializerCustom.ParseToXDocument(responseAsString).Root;
                    headerElement = envelopeElement.Element(XName.Get("Header", NS));
                    bodyElement = envelopeElement.Element(XName.Get("Body", NS));

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
                        FaultException fe = GetFaultException(responseAsString, isXmlSerializer);
                        raiseFaultException(fe);
                        return null;
                    }
                }

                if (requestResponseType.IsValueType)
                {
                    return ReadResponseValueType(responseAsString, requestResponseType);
                }
                else
                {
                    if (envelopeElement is null)
                    {
                        envelopeElement = DataContractSerializerCustom.ParseToXDocument(responseAsString).Root;
                        headerElement = envelopeElement.Element(XName.Get("Header", NS));
                        bodyElement = envelopeElement.Element(XName.Get("Body", NS));
                    }

                    return ReadResponseReferenceType(
                        envelopeElement,
                        bodyElement,
                        interfaceType,
                        requestResponseType,
                        knownTypes,
                        isXmlSerializer,
                        soapVersion);
                }

                static object ParseException(XElement exceptionElement, string exceptionTypeName)
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

                static Type ResolveType(string name)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    int asemblyCount = assemblies.Length;
                    for (int i = 0; i < asemblyCount; i++)
                    {
                        Type[] types = assemblies[i].GetTypes();
                        int typeCount = types.Length;
                        for (int j = 0; j < typeCount; j++)
                        {
                            if (types[j].Name == name)
                                return types[j];
                        }
                    }

                    throw new InvalidOperationException($"Could not resolve type {name}");
                }
            }

            private static object ReadResponseValueType(string responseAsString, Type requestResponseType)
            {
                Debug.Assert(requestResponseType.IsValueType);

                object requestResponse = null;

                // we remove the parts of the response string that are not the 
                // response itself. That is, we remove the "<s:Body>" so as to 
                // keep only its content
                ReadOnlySpan<char> response = responseAsString.AsSpan();
                ReadOnlySpan<char> keyWord = ":Body".AsSpan();
                int i = response.IndexOf(keyWord);
                int k = i + response.Slice(i).IndexOf('>');
                response = response.Slice(k + 1);
                i = response.LastIndexOf(keyWord);
                k = response.Slice(0, i).LastIndexOf('<');
                response = response.Slice(0, k);

                // Here we are dealing with a value type.
                // Example:
                //     <METHODNAMEResponse xmlns="http://tempuri.org/">
                //         <METHODNAMEResult>10</METHODNAMEResult>
                //     </METHODNAMEResponse>


                // we look for :nil="true". Since we have a value type, it will
                // mean that the value type is nullable and the value is null
                if (response.IndexOf(":nil=\"true\"".AsSpan()) == -1)
                {
                    // we remove the <MethodNameResponse> tag
                    int ii = response.IndexOf('>');
                    int jj = ii + 1 + response.Slice(ii + 1).IndexOf('>');
                    int kk = jj + 1 + response.Slice(jj + 1).IndexOf('<');
                    response = response.Slice(jj + 1, kk - jj - 1);

                    //todo: support more value types?
                    //todo: handle nullables that are null
                    if (requestResponseType == typeof(int) || requestResponseType == typeof(int?))
                    {
                        requestResponse = int.Parse(response.ToString());
                    }
                    else if (requestResponseType == typeof(long) || requestResponseType == typeof(long?))
                    {
                        requestResponse = long.Parse(response.ToString());
                    }
                    else if (requestResponseType == typeof(bool) || requestResponseType == typeof(bool?))
                    {
                        requestResponse = bool.Parse(response.ToString());
                    }
                    else if (requestResponseType == typeof(float) || requestResponseType == typeof(float?))
                    {
                        requestResponse = float.Parse(response.ToString()); //todo: ensure this is the culture-invariant parsing!
                    }
                    else if (requestResponseType == typeof(double) || requestResponseType == typeof(double?))
                    {
                        requestResponse = double.Parse(response.ToString()); //todo: ensure this is the culture-invariant parsing!
                    }
                    else if (requestResponseType == typeof(decimal) || requestResponseType == typeof(decimal?))
                    {
                        requestResponse = decimal.Parse(response.ToString()); //todo: ensure this is the culture-invariant parsing!
                    }
                    else if (requestResponseType == typeof(char) || requestResponseType == typeof(char?))
                    {
                        requestResponse = (char)int.Parse(response.ToString()); //todo: support encodings
                    }
                    else if (requestResponseType == typeof(DateTime) || requestResponseType == typeof(DateTime?))
                    {
                        requestResponse = DateTime.Parse(response.ToString(), CultureInfo.InvariantCulture);
                    }
                    else if (requestResponseType == typeof(TimeSpan) || requestResponseType == typeof(TimeSpan?))
                    {
                        requestResponse = XmlConvert.ToTimeSpan(response.ToString());
                    }
                    else if (requestResponseType.IsEnum)
                    {
                        requestResponse = Enum.Parse(requestResponseType, response.ToString());
                    }
                    else if (requestResponseType == typeof(void))
                    {
                        // Do nothing so null object will be returned
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"The type '{requestResponseType}' is not supported in the current WCF implementation, string value is {response.ToString()}.");
                    }
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

                return requestResponse;
            }

            private static object ReadResponseReferenceType(XElement envelopeElement,
                XElement bodyElement,
                Type interfaceType,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                bool isXmlSerializer,
                string soapVersion)
            {
                Debug.Assert(!requestResponseType.IsValueType);

                object requestResponse = null;

                // we deserialize the response
                // in the case of an XmlSerializer:
                // - change the xsi:nil="true" or add the xsi namespace (probably better to add the namespace)
                // - directly use xDoc.Root instead of its Nodes (I think)
                // - change the type to deserialize to the type with the name : requestResponseType.Name + "Body"
                FieldInfo bodyFieldInfo = null;

                // we make sure this is not a method with no return type
                // Note: we test for the "Object" type since it is what we put instead of "void"
                // to allow passing it as Generic type argument when calling CallWebMethod.
                if (requestResponseType == typeof(object))
                {
                    if (bodyElement != null && !bodyElement.Nodes().Any())
                    {
                        // Note: there might be a more efficient way of checking if the method has a return 
                        // type (possibly through a smart use of responseAsString.IndexOf but it seems 
                        // complicated and not necessarily more efficient).
                        // this is a method with no return type, there is no need to read the response 
                        // after checking that there was no FaultException.
                        return null;
                    }
                }

                Type typeToDeserialize = requestResponseType;
                if (isXmlSerializer)
                {
                    bodyFieldInfo = requestResponseType.GetField("Body");
                    typeToDeserialize = bodyFieldInfo.FieldType;
                }

                // get the known types from the interface type
                var types = new List<Type>(knownTypes ?? Enumerable.Empty<Type>());
                types.AddRange(interfaceType.GetCustomAttributes<ServiceKnownTypeAttribute>(true).Select(o => o.Type));

                var deSerializer = new DataContractSerializerCustom(typeToDeserialize, types);
                XElement xElement = envelopeElement;

                //exclude the parts that are <Enveloppe><Body>... since they are useless 
                // and would keep the deserialization from working properly
                // they should always be the two outermost elements
                if (soapVersion == "1.1")
                {
                    xElement = bodyElement ?? xElement;
                }
                else
                {
                    xElement = bodyElement;
                }

                // we check if the type is defined in the next XElement because 
                // it changes the XElement we want to use in that case.
                // The reason is that the response uses one less XElement in the 
                // case where we use XmlSerializer and the method has the return 
                // Type object.
                bool isTypeSpecified =
                    xElement.Attributes(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance").GetName("type"))
                            .Any();
                if (!isXmlSerializer || !isTypeSpecified)
                {
                    // we are either not in the XmlSerializer version or we have 
                    // the "extra" XElement so we move in once.
                    xElement = xElement.Elements().FirstOrDefault() ?? xElement; //move inside of the <Body> tag
                }

                if (!isXmlSerializer)
                {
                    if (typeToDeserialize.GetCustomAttribute<MessageContractAttribute>() != null)
                    {
                        // DataContractSerializer needs correct namespace instead of http://tempuri.org/
                        XNamespace ns = GetDefaultNamespace(typeToDeserialize.Namespace, false);
                        xElement.Name = ns + xElement.Name.LocalName;
                        xElement.Attributes("xmlns").Remove();
                        foreach (var childElement in xElement.Elements())
                        {
                            childElement.Name = ns + childElement.Name.LocalName;
                        }
                    }
                    else
                    {
                        xElement = xElement.Elements().FirstOrDefault() ?? xElement;
                    }
                    requestResponse = deSerializer.DeserializeFromXElement(xElement);
                }
                else
                {
                    requestResponse = Activator.CreateInstance(requestResponseType);
                    object requestResponseBody = deSerializer.DeserializeFromXElement(xElement);
                    bodyFieldInfo.SetValue(requestResponse, requestResponseBody);
                }

                return requestResponse;
            }

            static void VerifyThatResponseIsNotNullOrEmpty(string responseAsString)
            {
                // Check that the response is not empty:
                if (string.IsNullOrEmpty(responseAsString))
                {
                    throw new CommunicationException("The remote server returned an error. To debug, look at the browser Console output, or use a tool such as Fiddler.");
                }
            }

            private static string GetDefaultNamespace(string typeNamespace, bool useXmlSerializerFormat)
            {
                if (useXmlSerializerFormat)
                    return null;
                else
                    return DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE + typeNamespace;
            }
        }

        #region work in progress

        #region Not Supported Stuff

        /// <summary>
        /// Gets the underlying System.ServiceModel.IClientChannel implementation.
        /// </summary>
		[OpenSilver.NotImplemented]
        public IClientChannel InnerChannel
        {
            get { return null; }
        }

        [OpenSilver.NotImplemented]
        public void Abort()
        {

        }

        [OpenSilver.NotImplemented]
        public CommunicationState State
        {
            get { return CommunicationState.Created; }
        }


        //    /// <summary>
        //    /// Returns a new channel to the service.
        //    /// </summary>
        //    /// <returns>A channel of the type of the service contract.</returns>
        [OpenSilver.NotImplemented]
        protected virtual TChannel CreateChannel()
        {
            return null;
        }

        //    /// <summary>
        //    /// Replicates the behavior of the default keyword in C#.
        //    /// </summary>
        //    /// <typeparam name="T">The type that is identified as reference or numeric by the keyword.</typeparam>
        //    /// <returns>Returns null if T is a reference type and zero if T is a numeric value type.</returns>
        [OpenSilver.NotImplemented]
        protected T GetDefaultValueForInitialization<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Generic ChannelBase class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        //protected class ChannelBase<T> : IOutputChannel, IRequestChannel, IClientChannel, IDisposable, IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel> where T : class
        [OpenSilver.NotImplemented]
        protected class ChannelBase<T> where T : class
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="System.ServiceModel.ClientBase{TChannel}.ChannelBase{T}"/>
            /// class from an existing instance of the class.
            /// </summary>
            /// <param name="client">The object used to initialize the new instance of the class.</param>
		    [OpenSilver.NotImplemented]
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
            [OpenSilver.NotImplemented]
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
            [OpenSilver.NotImplemented]
            protected object EndInvoke(string methodName, object[] args, IAsyncResult result)
            {
                return null;
            }
        }


        #endregion

        #endregion work in progress
    }
}