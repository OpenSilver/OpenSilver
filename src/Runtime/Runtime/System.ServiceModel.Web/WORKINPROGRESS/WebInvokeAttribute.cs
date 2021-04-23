using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Web
{
    //
    // Summary:
    //     Indicates an operation is logically an invoke operation and that it can be called
    //     by the Web HTTP programming.
    [AttributeUsage(AttributeTargets.Method)]
	[OpenSilver.NotImplemented]
    public sealed class WebInvokeAttribute : Attribute, IOperationBehavior
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Web.WebGetAttribute class.
		[OpenSilver.NotImplemented]
        public WebInvokeAttribute()
        {

        }

        //
        // Summary:
        //     Gets and sets the body style of the messages that are sent to and from an operation.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageBodyStyle enumeration values.
		[OpenSilver.NotImplemented]
        public WebMessageBodyStyle BodyStyle { get; set; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebInvokeAttribute.IsBodyStyleSetExplicitly
        //     property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.BodyStyle
        //     property was set explicitly.
		[OpenSilver.NotImplemented]
        public bool IsBodyStyleSetExplicitly { get; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebInvokeAttribute.IsRequestFormatSetExplicitly
        //     property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.RequestFormat
        //     property was set.
		[OpenSilver.NotImplemented]
        public bool IsRequestFormatSetExplicitly { get; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebInvokeAttribute.IsResponseFormatSetExplicitly
        //     property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.ResponseFormat
        //     property was set.
		[OpenSilver.NotImplemented]
        public bool IsResponseFormatSetExplicitly { get; }
        //
        // Summary:
        //     Gets and sets the protocol (for example HTTP) method an operation responds to.
        //
        // Returns:
        //     The protocol method associated with the operation.
		[OpenSilver.NotImplemented]
        public string Method { get; set; }
        //
        // Summary:
        //     Gets and sets the System.ServiceModel.Web.WebInvokeAttribute.RequestFormat property.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageFormat enumeration values.
		[OpenSilver.NotImplemented]
        public WebMessageFormat RequestFormat { get; set; }
        //
        // Summary:
        //     Gets and sets the System.ServiceModel.Web.WebInvokeAttribute.ResponseFormat property.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageFormat enumeration values.
		[OpenSilver.NotImplemented]
        public WebMessageFormat ResponseFormat { get; set; }
        //
        // Summary:
        //     The Uniform Resource Identifier (URI) template for an operation.
        //
        // Returns:
        //     The URI template for t operation.
		[OpenSilver.NotImplemented]
        public string UriTemplate { get; set; }

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            throw new NotImplementedException();
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            throw new NotImplementedException();
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            throw new NotImplementedException();
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
            throw new NotImplementedException();
        }
    }
}