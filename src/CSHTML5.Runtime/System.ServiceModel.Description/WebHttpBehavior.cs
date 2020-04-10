using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace System.ServiceModel.Description
{
    //
    // Summary:
    //     Enables the Web programming model for a Silverlight client.
    public class WebHttpBehavior : IEndpointBehavior
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Description.WebHttpBehavior
        //     class.
        public WebHttpBehavior()
        {

        }

        //
        // Summary:
        //     Gets and sets the default message body style.
        //
        // Returns:
        //     One of the values defined in the System.ServiceModel.Web.WebMessageBodyStyle
        //     enumeration.
        public virtual WebMessageBodyStyle DefaultBodyStyle { get; set; }
        //
        // Summary:
        //     Gets and sets the default outgoing request format.
        //
        // Returns:
        //     One of the values defined in the System.ServiceModel.Web.WebMessageFormat enumeration.
        public virtual WebMessageFormat DefaultOutgoingRequestFormat { get; set; }
        //
        // Summary:
        //     Gets and sets the default outgoing response format.
        //
        // Returns:
        //     One of the values defined in the System.ServiceModel.Web.WebMessageFormat enumeration.
        public virtual WebMessageFormat DefaultOutgoingResponseFormat { get; set; }
        //
        // Summary:
        //     Gets or sets the flag that specifies whether a FaultException is generated when
        //     an internal server error (HTTP status code: 500) occurs.
        //
        // Returns:
        //     Returns true if the flag is enabled; otherwise returns false.
        public virtual bool FaultExceptionEnabled { get; set; }

        //
        // Summary:
        //     This method is not used in Silverlight 5.
        //
        // Parameters:
        //   endpoint:
        //     The service endpoint to be accessed.
        //
        //   bindingParameters:
        //     The binding parameters that support modifying the bindings.
        public virtual void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }
        //
        // Summary:
        //     Implements the System.ServiceModel.Description.IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint,System.ServiceModel.Dispatcher.ClientRuntime)
        //     method to support modification or extension of the client across an endpoint.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint that exposes the contract the client is to access.
        //
        //   clientRuntime:
        //     The client to which the custom behavior is applied.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     Either the endpoint of the clientRuntime is null.
        public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }
        //
        // Summary:
        //     This method is not used in Silverlight 5.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint that exposes the contract.
        //
        //   endpointDispatcher:
        //     The endpoint dispatcher to which the behavior is applied.
        public virtual void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }
        //
        // Summary:
        //     Confirms that the endpoint meets the requirements for the Web programming model.
        //
        // Parameters:
        //   endpoint:
        //     The service endpoint.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     Endpoint is null.
        //
        //   T:System.InvalidOperationException:
        //     Message headers are included or the a scheme other than HTTP(S) is used or manual
        //     addressing is configured incorrectly.
        public virtual void Validate(ServiceEndpoint endpoint)
        {

        }
        //
        // Summary:
        //     Adds a client error inspector to the specified client runtime.
        //
        // Parameters:
        //   endpoint:
        //     A service endpoint.
        //
        //   clientRuntime:
        //     The client runtime.
        protected virtual void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }
        //
        // Summary:
        //     Gets the query string converter.
        //
        // Parameters:
        //   operationDescription:
        //     A contract that characterizes an operation in terms of the messages it exchanges.
        //
        // Returns:
        //     A System.ServiceModel.Dispatcher.QueryStringConverter instance.
        protected virtual QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return null;
        }
        //
        // Summary:
        //     Gets the reply formatter on the client for the specified endpoint and service
        //     operation.
        //
        // Parameters:
        //   operationDescription:
        //     A contract that characterizes an operation in terms of the messages it exchanges.
        //
        //   endpoint:
        //     The service endpoint.
        //
        // Returns:
        //     An System.ServiceModel.Dispatcher.IClientMessageFormatter reference to the reply
        //     formatter on the client for the specified operation and endpoint.
        protected virtual IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return null;
        }
        //
        // Summary:
        //     Gets the request formatter on the client for the specified service operation
        //     and endpoint.
        //
        // Parameters:
        //   operationDescription:
        //     A contract that characterizes an operation in terms of the messages it exchanges.
        //
        //   endpoint:
        //     The service endpoint to be accessed by the client.
        //
        // Returns:
        //     An System.ServiceModel.Dispatcher.IClientMessageFormatter reference to the request
        //     formatter on the client for the specified operation and endpoint.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The JSON format is not currently supported in Silverlight.
        protected virtual IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return null;
        }
        //
        // Summary:
        //     Ensures the binding is valid for use with the Silverlight Web Programming Model.
        //
        // Parameters:
        //   endpoint:
        //     The service endpoint.
        protected virtual void ValidateBinding(ServiceEndpoint endpoint)
        {

        }
    }
}

namespace System.ServiceModel.Dispatcher
{
    //
    // Summary:
    //     This class converts a parameter from an object to its query string representation.
    public class QueryStringConverter
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Dispatcher.QueryStringConverter
        //     class.
        public QueryStringConverter()
        {

        }

        //
        // Summary:
        //     Determines whether the specified type can be converted to and from a string representation.
        //
        // Parameters:
        //   type:
        //     The System.Type to convert.
        //
        // Returns:
        //     A value that specifies whether the type can be converted.
        public virtual bool CanConvert(Type type)
        {
            return false;
        }
        //
        // Summary:
        //     Converts a parameter to a query string representation.
        //
        // Parameters:
        //   parameter:
        //     The parameter to convert.
        //
        //   parameterType:
        //     The System.Type of the parameter to convert.
        //
        // Returns:
        //     The parameter name and value.
        public virtual string ConvertValueToString(object parameter, Type parameterType)
        {
            return null;
        }
    }
}

namespace System.ServiceModel.Web
{
    //
    // Summary:
    //     An enumeration that specifies whether to wrap parameter and return values within
    //     XML elements.
    public enum WebMessageBodyStyle
    {
        //
        // Summary:
        //     Both requests and responses are not wrapped.
        Bare = 0,
        //
        // Summary:
        //     Both requests and responses are wrapped.
        Wrapped = 1,
        //
        // Summary:
        //     Requests are wrapped, responses are not wrapped.
        WrappedRequest = 2,
        //
        // Summary:
        //     Responses are wrapped, requests are not wrapped.
        WrappedResponse = 3
    }
}

namespace System.ServiceModel.Web
{
    //
    // Summary:
    //     An enumeration that specifies the format of Web messages.
    public enum WebMessageFormat
    {
        //
        // Summary:
        //     The XML format.
        Xml = 0,
        //
        // Summary:
        //     The JavaScript Object Notation (JSON) format.
        Json = 1
    }
}

namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Enables management of how HTTP cookies are handled in HTTP requests and responses.
    public class HttpCookieContainerBindingElement : BindingElement
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.HttpCookieContainerBindingElement
        //     class.
        public HttpCookieContainerBindingElement()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.HttpCookieContainerBindingElement
        //     class from a specified binding element.
        //
        // Parameters:
        //   elementToBeCloned:
        //     The System.ServiceModel.Channels.BindingElement to be cloned.
        protected HttpCookieContainerBindingElement(HttpCookieContainerBindingElement elementToBeCloned)
        {

        }

        //
        // Summary:
        //     Initializes a channel factory for producing channels of a specified type from
        //     the binding context.
        //
        // Parameters:
        //   context:
        //     The System.ServiceModel.Channels.BindingContext that provides context for the
        //     binding element.
        //
        // Type parameters:
        //   TChannel:
        //     The type of channel that the factory builds.
        //
        // Returns:
        //     The System.ServiceModel.Channels.IChannelFactory`1 of type TChannel initialized
        //     from the context.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     context is null.
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return null;
        }
        //
        // Summary:
        //     Returns a copy of the current binding element object.
        //
        // Returns:
        //     The System.ServiceModel.Channels.BindingElement object that is a deep clone of
        //     the original.
        public override BindingElement Clone()
        {
            return null;
        }
        //
        // Summary:
        //     Returns a typed object requested, if present, from the appropriate layer in the
        //     binding stack.
        //
        // Parameters:
        //   context:
        //     The System.ServiceModel.Channels.BindingContext for the binding element.
        //
        // Type parameters:
        //   T:
        //     The typed object for which the method is querying.
        //
        // Returns:
        //     The typed object T requested if it is present, or null if it is not present.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     context is null.
        public override T GetProperty<T>(BindingContext context)
        {
            return default(T);
        }
    }
}