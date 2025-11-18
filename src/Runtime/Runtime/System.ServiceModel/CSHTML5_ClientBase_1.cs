

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
using System.ServiceModel.Description;
using System.Collections.Concurrent;
using System.Text;

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
        public ServiceEndpoint Endpoint { get; } = new ServiceEndpoint(new ContractDescription("none"));
        public ClientCredentials ClientCredentials { get; } = new ClientCredentials();

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
            var oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(
                Windows.Threading.Dispatcher.CurrentDispatcher.DefaultSynchronizationContext);

            var asyncOperation = AsyncOperationManager.CreateOperation(userState);

            SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);

            var context = new AsyncOperationContext(asyncOperation, endOperationDelegate, operationCompletedCallback);

            Exception error = null;
            object[] results = null;
            IAsyncResult result = null;

            try
            {
                result = beginOperationDelegate(inValues, OnAsyncCallCompleted, context);
                if (result.CompletedSynchronously)
                {
                    results = endOperationDelegate(result);
                }
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error != null || result.CompletedSynchronously) /* result cannot be null if error == null */
            {
                CompleteAsyncCall(context, results, error);
            }
        }

        private static void OnAsyncCallCompleted(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            var context = (AsyncOperationContext)result.AsyncState;
            Exception error = null;
            object[] results = null;

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

        private static void CompleteAsyncCall(AsyncOperationContext context, object[] results, Exception error)
        {
            if (context.CompletionCallback != null)
            {
                var e = new InvokeAsyncCompletedEventArgs(results, error, false, context.AsyncOperation.UserSuppliedState);
                context.AsyncOperation.PostOperationCompleted(context.CompletionCallback, e);
            }
            else
            {
                context.AsyncOperation.OperationCompleted();
            }
        }

        private sealed class AsyncOperationContext
        {
            internal AsyncOperationContext(AsyncOperation asyncOperation, EndOperationDelegate endDelegate, SendOrPostCallback completionCallback)
            {
                AsyncOperation = asyncOperation;
                EndDelegate = endDelegate;
                CompletionCallback = completionCallback;
            }

            internal AsyncOperation AsyncOperation { get; }

            internal EndOperationDelegate EndDelegate { get; }

            internal SendOrPostCallback CompletionCallback { get; }
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
            internal InvokeAsyncCompletedEventArgs(object[] results, Exception error, bool cancelled, object userState)
                : base(error, cancelled, userState)
            {
                Results = results;
            }

            public object[] Results { get; }
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
                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                PrepareRequest(
                    operation,
                    knownTypes,
                    messageHeaders,
                    originalRequestObject,
                    soapVersion,
                    out Dictionary<string, string> headers,
                    out string request);

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
                object[] args,
                Type interfaceType,
                Type methodReturnType,
                IReadOnlyList<Type> knownTypes,
                string xmlReturnedFromTheServer,
                string soapVersion)
            {
                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                (object result, Exception error) = ReadAndPrepareResponse(
                    operation,
                    args,
                    xmlReturnedFromTheServer,
                    methodReturnType,
                    knownTypes,
                    soapVersion);

                if (error is not null)
                {
                    throw error;
                }

                return result;
            }

            public object EndCallWebMethod(
                string webMethodName,
                Type interfaceType,
                Type methodReturnType,
                IReadOnlyList<Type> knownTypes,
                string xmlReturnedFromTheServer,
                string soapVersion)
            {
                return EndCallWebMethod(webMethodName,
                    [],
                    interfaceType,
                    methodReturnType,
                    knownTypes,
                    xmlReturnedFromTheServer,
                    soapVersion);
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
                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                string outgoingMessageHeadersString = GetEnvelopeHeaders(outgoingMessageHeaders?.ToList(), soapVersion);

                PrepareRequest(
                    operation,
                    null,
                    outgoingMessageHeadersString,
                    originalRequestObject,
                    soapVersion,
                    out Dictionary<string, string> headers,
                    out string request);

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
                            operation,
                            methodReturnType,
                            null,
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
                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                PrepareRequest(
                    operation,
                    null,
                    "",
                    originalRequestObject,
                    soapVersion,
                    out Dictionary<string, string> headers,
                    out string request);

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
                            operation,
                            methodReturnType,
                            null,
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

                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                PrepareRequest(
                    operation,
                    null,
                    "",
                    originalRequestObject,
                    soapVersion,
                    out Dictionary<string, string> headers,
                    out string request);

                string response = _webRequestHelper_JSVersion.MakeRequest(
                        new Uri(_addressOfService),
                        "POST",
                        this,
                        headers,
                        request,
                        null,
                        false,
                        Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                (object result, Exception error) = ReadAndPrepareResponse(
                    operation,
                    [],
                    response,
                    methodReturnType,
                    null,
                    soapVersion);

                if (error is not null)
                {
                    throw error;
                }

                return result;
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

                ContractDescription contract = ContractDescriptionProvider.GetContract(interfaceType);
                OperationDescription operation = contract.Operations.Find(webMethodName);

                var outgoingMessageHeadersString = GetEnvelopeHeaders(outgoingMessageHeaders?.ToList(), soapVersion);

                PrepareRequest(
                    operation,
                    null,
                    outgoingMessageHeadersString,
                    originalRequestObject,
                    soapVersion,
                    out Dictionary<string, string> headers,
                    out string request);

                string response = _webRequestHelper_JSVersion.MakeRequest(
                        new Uri(_addressOfService),
                        "POST",
                        this,
                        headers,
                        request,
                        null,
                        false,
                        Application.Current.Host.Settings.DefaultSoapCredentialsMode);

                (object result, Exception error) = ReadAndPrepareResponse(
                    operation,
                    [],
                    response,
                    methodReturnType,
                    null,
                    soapVersion);

                if (error is not null)
                {
                    throw error;
                }

                var incomingMessageHeaders = GetEnvelopeHeaders(response, soapVersion);

                return (result, incomingMessageHeaders);
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
                OperationDescription operation,
                IReadOnlyList<Type> knownTypes,
                string envelopeHeaders,
                IDictionary<string, object> requestParameters,
                string soapVersion,
                out Dictionary<string, string> headers,
                out string request)
            {
                headers = [];

                var bodyBuilder = new StringBuilder();
                using (var xmlWriter = XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(bodyBuilder, new XmlWriterSettings { OmitXmlDeclaration = true })))
                {
                    MessageDescription messageDescription = operation.Messages[0];

                    if (messageDescription.Body.WrapperName is not null)
                    {
                        xmlWriter.WriteStartElement(messageDescription.Body.WrapperName, messageDescription.Body.WrapperNamespace);
                    }

                    if (requestParameters is not null)
                    {
                        var types = new List<Type>(knownTypes ?? Enumerable.Empty<Type>());
                        types.AddRange(operation.KnownTypes);
                        types.AddRange(KnownTypesHelper.KnownTypes);

                        if (messageDescription.MessageType is null)
                        {
                            foreach (MessagePartDescription part in messageDescription.Body.Parts)
                            {
                                var requestBody = requestParameters[part.Name];
                                var serializer = new DataContractSerializer(part.Type, part.Name, part.Namespace, types);
                                serializer.WriteObject(xmlWriter, requestBody);
                            }
                        }
                        else
                        {
                            var body = requestParameters.Values.First();
                            foreach (MessagePartDescription part in messageDescription.Body.Parts)
                            {
                                var serializer = new DataContractSerializer(part.Type, part.Name, part.Namespace, types);
                                var bodyMember = part.MemberInfo.MemberType switch
                                {
                                    MemberTypes.Property => ((PropertyInfo)part.MemberInfo).GetValue(body),
                                    _ => ((FieldInfo)part.MemberInfo).GetValue(body),
                                };
                                serializer.WriteObject(xmlWriter, bodyMember);
                            }
                        }
                    }

                    if (messageDescription.Body.WrapperName is not null)
                    {
                        xmlWriter.WriteEndElement();
                    }
                }

                string elementAsString = bodyBuilder.ToString();

                // Look for the soapAction.
                string soapAction = operation.Messages[0].Action;

                switch (soapVersion)
                {
                    case "1.1":
                        headers.Add("Content-Type", "text/xml; charset=utf-8");
                        headers.Add("SOAPAction", soapAction);

                        if (!string.IsNullOrEmpty(envelopeHeaders))
                        {
                            envelopeHeaders = "<s:Header>" + envelopeHeaders + "</s:Header>";
                        }

                        request = $"<s:Envelope xmlns:s=\"{MessageStrings.SOAP11.Namespace}\">{(envelopeHeaders ?? string.Empty)}<s:Body>{elementAsString}</s:Body></s:Envelope>";
                        break;

                    case "1.2":
                        headers.Add("Content-Type", "application/soap+xml; charset=utf-8");

                        request = $"<s:Envelope xmlns:a=\"{MessageStrings.NamespaceAddressing10}\" xmlns:s=\"{MessageStrings.SOAP12.Namespace}\"><s:Header><a:Action>{soapAction}</a:Action>{envelopeHeaders ?? string.Empty}<a:To>{_addressOfService}</a:To></s:Header><s:Body>{elementAsString}</s:Body></s:Envelope>";
                        break;

                    default:
                        throw new InvalidOperationException($"SOAP version not supported: {soapVersion}");
                }
            }

            private void ReadAndPrepareResponseGeneric_JSVersion<T>(
                TaskCompletionSource<T> tcs,
                INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e,
                OperationDescription operation,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                string soapVersion)
            {
                if (e.Error is not null && string.IsNullOrEmpty(e.Result))
                {
                    tcs.TrySetException(e.Error);
                    return;
                }

                (object result, Exception error) = ReadAndPrepareResponse(
                    operation,
                    [],
                    e.Result,
                    requestResponseType,
                    knownTypes,
                    soapVersion);

                if (error is not null)
                {
                    tcs.TrySetException(error);
                }
                else
                {
                    tcs.TrySetResult((T)result);
                }
            }

            private void ReadAndPrepareResponseGeneric_JSVersion<T>(
                TaskCompletionSource<(T, MessageHeaders)> tcs,
                INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e,
                OperationDescription operation,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
                string soapVersion)
            {
                if (e.Error is not null)
                {
                    tcs.TrySetException(e.Error);
                    return;
                }

                (object result, Exception error) = ReadAndPrepareResponse(
                    operation,
                    [],
                    e.Result,
                    requestResponseType,
                    knownTypes,
                    soapVersion);

                if (error is not null)
                {
                    tcs.SetException(error);
                }
                else
                {
                    tcs.SetResult(((T)result, GetEnvelopeHeaders(e.Result, soapVersion)));
                }
            }

            private static (object Result, Exception Error) ReadAndPrepareResponse(
                OperationDescription operation,
                object[] args,
                string responseAsString,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes,
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

                // Check that the response is not empty:
                if (string.IsNullOrEmpty(responseAsString))
                {
                    throw new CommunicationException("The remote server returned an error. To debug, look at the browser Console output, or use a tool such as Fiddler.");
                }

                string ns;
                if (soapVersion == "1.1")
                {
                    ns = MessageStrings.SOAP11.Namespace;
                }
                else
                {
                    Debug.Assert(soapVersion == "1.2", $"Unexpected soap version ({soapVersion}) !");
                    ns = MessageStrings.SOAP12.Namespace;
                }

                var envelopeElement = DataContractSerializerCustom.ParseToXDocument(responseAsString).Root;
                var bodyElement = envelopeElement.Element(XName.Get(MessageStrings.Body, ns));

                if (bodyElement.Element(XName.Get(MessageStrings.Fault, ns)) is XElement faultElement)
                {
                    if (soapVersion == "1.1")
                    {
                        FaultException fe = GetFaultException11(operation, faultElement);
                        return (null, fe);
                    }
                    else
                    {
                        Debug.Assert(soapVersion == "1.2");

                        var headerElement = envelopeElement.Element(XName.Get(MessageStrings.Action, MessageStrings.SOAP12.Namespace));
                        string action = headerElement.Element(XName.Get(MessageStrings.Action, MessageStrings.NamespaceAddressing10)).Value;
                        FaultException fe = GetFaultException12(operation, faultElement, action);
                        return (null, fe);
                    }
                }

                if (operation.IsOneWay)
                {
                    return (null, null);
                }

                object result = ReadResponseReferenceType(
                    bodyElement,
                    operation,
                    args,
                    requestResponseType,
                    knownTypes);

                return (result, null);
            }

            private static object ReadResponseReferenceType(
                XElement bodyElement,
                OperationDescription operation,
                object[] args,
                Type requestResponseType,
                IReadOnlyList<Type> knownTypes)
            {
                Debug.Assert(!operation.IsOneWay);

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

                // get the known types from the interface type
                var types = new List<Type>(knownTypes ?? Enumerable.Empty<Type>());
                types.AddRange(operation.KnownTypes);
                types.AddRange(KnownTypesHelper.KnownTypes);

                //exclude the parts that are <Enveloppe><Body>... since they are useless 
                // and would keep the deserialization from working properly
                // they should always be the two outermost elements
                XElement xElement = bodyElement;

                object requestResponse;

                var replyDescription = operation.Messages[1]; // out message are always at index 1

                if (replyDescription.MessageType is not null)
                {
                    requestResponse = Activator.CreateInstance(
                        replyDescription.MessageType,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic,
                        null,
                        [],
                        null);

                    if (replyDescription.Body.WrapperName is not null)
                    {
                        xElement = xElement.Elements().First();
                        Debug.Assert(xElement.Name.LocalName == replyDescription.Body.WrapperName &&
                            xElement.Name.NamespaceName == replyDescription.Body.WrapperNamespace);
                    }

                    foreach (var part in replyDescription.Body.Parts)
                    {
                        XElement element = xElement.Element(XName.Get(part.Name, part.Namespace));
                        if (element is null)
                        {
                            continue;
                        }

                        var serializer = new DataContractSerializer(part.Type,
                            part.Name,
                            part.Namespace,
                            types);

                        object o = DeserializeXElement(serializer, element);

                        if (part.MemberInfo.MemberType == MemberTypes.Property)
                        {
                            ((PropertyInfo)part.MemberInfo).SetValue(requestResponse, o);
                        }
                        else
                        {
                            ((FieldInfo)part.MemberInfo).SetValue(requestResponse, o);
                        }
                    }
                }
                else
                {
                    if (replyDescription.Body.WrapperName is not null)
                    {
                        xElement = xElement.Elements().First();
                        Debug.Assert(xElement.Name.LocalName == replyDescription.Body.WrapperName &&
                            xElement.Name.NamespaceName == replyDescription.Body.WrapperNamespace);
                    }

                    requestResponse = null;

                    var returnPart = replyDescription.Body.ReturnValue;

                    if (returnPart is not null && returnPart.Type != typeof(void))
                    {
                        var element = xElement.Element(XName.Get(returnPart.Name, returnPart.Namespace));

                        if (element is not null)
                        {
                            var serializer = new DataContractSerializer(
                                returnPart.Type,
                                returnPart.Name,
                                returnPart.Namespace,
                                types);

                            requestResponse = DeserializeXElement(serializer, element);
                        }
                    }

                    var outParts = replyDescription.Body.Parts;
                    int outPartsCount = Math.Min(outParts.Count, args?.Length ?? 0);

                    for (int i = 0; i < outPartsCount; i++)
                    {
                        var part = outParts[i];

                        var element = xElement.Element(XName.Get(part.Name, part.Namespace));
                        if (element is not null)
                        {
                            var serializer = new DataContractSerializer(
                                part.Type,
                                part.Name,
                                part.Namespace,
                                types);

                            args[i] = DeserializeXElement(serializer, element);
                        }
                        else
                        {
                            // Element not found, add default value
                            args[i] = part.Type.IsValueType ? Activator.CreateInstance(part.Type) : null;
                        }
                    }
                }

                return requestResponse;
            }

            private static object DeserializeXElement(DataContractSerializer serializer, XElement xElement)
            {
                using (var reader = xElement.CreateReader())
                {
                    return serializer.ReadObject(reader);
                }
            }

            private static FaultException GetFaultException11(OperationDescription operation, XElement faultElement)
            {
                var faultCodeElement = faultElement.Element(XName.Get(MessageStrings.SOAP11.FaultCode));
                (string name, string ns) = ReadContentAsQName(faultCodeElement);
                var code = new FaultCode(name, ns);

                var faultStringElement = faultElement.Element(XName.Get(MessageStrings.SOAP11.FaultString));
                string text = faultStringElement.Value;
                string xmlLang = faultStringElement.Attribute(XName.Get("lang", XNamespace.Xml.NamespaceName))?.Value ?? string.Empty;
                var translation = new FaultReasonText(text, xmlLang);
                var reason = new FaultReason(translation);

                if (faultElement.Element(XName.Get(MessageStrings.SOAP11.FaultDetail)) is XElement detailElement)
                {
                    (Type detailType, object detail) = GetFaultDetail(operation, detailElement);

                    if (detailType is not null)
                    {
                        return (FaultException)Activator.CreateInstance(
                            typeof(FaultException<>).MakeGenericType(detailType),
                            detail,
                            reason,
                            code);
                    }
                }

                return new FaultException(reason, code, null);
            }

            private static FaultException GetFaultException12(OperationDescription operation, XElement faultElement, string action)
            {
                var code = ReadFaultCode12(faultElement.Element(XName.Get(MessageStrings.SOAP12.FaultCode, MessageStrings.SOAP12.Namespace)));

                var translations = new List<FaultReasonText>();
                var reasonElement = faultElement.Element(XName.Get(MessageStrings.SOAP12.FaultReason, MessageStrings.SOAP12.Namespace));
                foreach (var textElement in reasonElement.Elements(XName.Get(MessageStrings.SOAP12.FaultText, MessageStrings.SOAP12.Namespace)))
                {
                    translations.Add(ReadTranslation12(textElement));
                }
                var reason = new FaultReason(translations);

                if (faultElement.Element(XName.Get(MessageStrings.SOAP12.FaultDetail, MessageStrings.SOAP12.Namespace)) is XElement detailElement)
                {
                    (Type detailType, object detail) = GetFaultDetail(operation, detailElement);

                    if (detailType is not null)
                    {
                        return (FaultException)Activator.CreateInstance(
                            typeof(FaultException<>).MakeGenericType(detailType),
                            detail,
                            reason,
                            code,
                            action);
                    }
                }

                return new FaultException(reason, code, action);
            }

            private static FaultCode ReadFaultCode12(XElement codeElement)
            {
                (string localName, string ns) = ReadContentAsQName(codeElement);
                if (codeElement.Element(XName.Get(MessageStrings.SOAP12.FaultSubcode, MessageStrings.SOAP12.Namespace)) is XElement subCodeElement)
                {
                    var subCode = ReadFaultCode12(subCodeElement);
                    return new FaultCode(localName, ns, subCode);
                }
                return new FaultCode(localName, ns);
            }

            private static FaultReasonText ReadTranslation12(XElement textElement)
            {
                string xmlLang = null;
                if (textElement.Attribute(XName.Get("lang", XNamespace.Xml.NamespaceName)) is XAttribute xmlLangAttribute)
                {
                    xmlLang = xmlLangAttribute.Value;
                }

                if (xmlLang is null)
                {
                    throw new XmlException("Required xml:lang attribute value is missing.");
                }

                string text = textElement.Value;
                return new FaultReasonText(text, xmlLang);
            }

            private static (Type DetailType, object Detail) GetFaultDetail(OperationDescription operation, XElement detailElement)
            {
                if (detailElement is not null && detailElement.Elements().FirstOrDefault() is XElement detailContentElement)
                {
                    foreach (FaultDescription fault in operation.Faults)
                    {
                        if (fault.Name == detailContentElement.Name.LocalName && fault.Namespace == detailContentElement.Name.NamespaceName)
                        {
                            var serializer = new DataContractSerializer(fault.DetailType, fault.Name, fault.Namespace, operation.KnownTypes);
                            var detail = DeserializeXElement(serializer, detailContentElement);

                            return (fault.DetailType, detail);
                        }
                    }
                }

                return (null, null);
            }

            private static (string localName, string ns) ReadContentAsQName(XElement element)
            {
                string prefix, localName;

                string qname = element.Value;
                int index = qname.IndexOf(':');

                if (index < 0)
                {
                    prefix = string.Empty;
                    localName = qname.Trim();
                }
                else
                {
                    if (index == qname.Length - 1)
                    {
                        throw new XmlException($"Expected XML qualified name, found '{qname}'.");
                    }
                    prefix = qname.AsSpan(0, index).TrimStart().ToString();
                    localName = qname.AsSpan(index + 1).TrimEnd().ToString();
                }

                XNamespace ns = string.IsNullOrEmpty(prefix) ?
                    element.GetDefaultNamespace() :
                    element.GetNamespaceOfPrefix(prefix);

                if (ns is null)
                {
                    throw new XmlException($"Unbound prefix used in qualified name '{qname}'.");
                }

                return (localName, ns.NamespaceName);
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

        /// <summary>
        /// Replicates the behavior of the default keyword in C#.
        /// </summary>
        /// <typeparam name="T">
        /// The type that is identified as reference or numeric by the keyword.
        /// </typeparam>
        /// <returns>
        /// Returns null if <typeparamref name="T"/> is a reference type and zero if <typeparamref name="T"/> is a numeric value type.
        /// </returns>
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

    internal static class ContractDescriptionProvider
    {
        private static readonly ConcurrentDictionary<Type, ContractDescription> _cache = [];

        public static ContractDescription GetContract(Type type) => _cache.GetOrAdd(type, ContractDescription.GetContract);
    }

    internal static class MessageStrings
    {
        public const string NamespaceAddressing10 = "http://www.w3.org/2005/08/addressing";
        public const string Action = "Action";
        public const string Header = "Header";
        public const string Fault = "Fault";
        public const string Body = "Body";

        internal static class SOAP11
        {
            public const string Namespace = "http://schemas.xmlsoap.org/soap/envelope/";
            public const string FaultCode = "faultcode";
            public const string FaultString = "faultstring";
            public const string FaultDetail = "detail";
        }

        internal static class SOAP12
        {
            public const string Namespace = "http://www.w3.org/2003/05/soap-envelope";
            public const string FaultCode = "Code";
            public const string FaultReason = "Reason";
            public const string FaultText = "Text";
            public const string FaultDetail = "Detail";
            public const string FaultSubcode = "Subcode";
        }
    }
}