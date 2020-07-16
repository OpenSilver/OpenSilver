using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Web
{
    //
    // Summary:
    //     Indicates that an operation is logically a retrieval operation and that it can
    //     be called by the Web HTTP programming model.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class WebGetAttribute : Attribute, IOperationBehavior
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Web.WebGetAttribute class.
        public WebGetAttribute()
        {

        }

        //
        // Summary:
        //     Gets and sets the body style of the messages that are sent to and from an operation.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageBodyStyle enumeration values.
        public WebMessageBodyStyle BodyStyle { get; set; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebGetAttribute.IsBodyStyleSetExplicitly property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.BodyStyle
        //     property is set.
        public bool IsBodyStyleSetExplicitly { get; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebGetAttribute.IsRequestFormatSetExplicitly
        //     property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.RequestFormat
        //     property was set.
        public bool IsRequestFormatSetExplicitly { get; }
        //
        // Summary:
        //     Gets the System.ServiceModel.Web.WebGetAttribute.IsResponseFormatSetExplicitly
        //     property.
        //
        // Returns:
        //     A value that specifies whether the System.ServiceModel.Web.WebGetAttribute.ResponseFormat
        //     property was set.
        public bool IsResponseFormatSetExplicitly { get; }
        //
        // Summary:
        //     Gets and sets the System.ServiceModel.Web.WebGetAttribute.RequestFormat property.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageFormat enumeration values.
        public WebMessageFormat RequestFormat { get; set; }
        //
        // Summary:
        //     Gets and sets the System.ServiceModel.Web.WebGetAttribute.ResponseFormat property.
        //
        // Returns:
        //     One of the System.ServiceModel.Web.WebMessageFormat enumeration values.
        public WebMessageFormat ResponseFormat { get; set; }
        //
        // Summary:
        //     Gets and sets the Uniform Resource Identifier (URI) template for the operation.
        //
        // Returns:
        //     The URI template for an operation.
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