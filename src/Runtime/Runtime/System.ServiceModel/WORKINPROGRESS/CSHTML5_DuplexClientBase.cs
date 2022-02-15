using System.ServiceModel.Channels;
using System.ComponentModel;

namespace System.ServiceModel
{
    public abstract class CSHTML5_DuplexClientBase<TChannel> : CSHTML5_ClientBase<TChannel> where TChannel : class
    {
        [OpenSilver.NotImplemented]
        protected CSHTML5_DuplexClientBase(InstanceContext callbackInstance)
        {

        }

        [OpenSilver.NotImplemented]
        protected CSHTML5_DuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
        {

        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected CSHTML5_DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName)
        {

        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected CSHTML5_DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
        {

        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected CSHTML5_DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
        {

        }
    }
}

