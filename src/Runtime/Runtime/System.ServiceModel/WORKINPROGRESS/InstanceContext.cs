using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class InstanceContext : CommunicationObject, IExtensibleObject<InstanceContext>
    {
        [OpenSilver.NotImplemented]
        public InstanceContext(object implementation)
        {
          
        }

        [OpenSilver.NotImplemented]
        public IExtensionCollection<InstanceContext> Extensions => throw new NotImplementedException();

        [OpenSilver.NotImplemented]
        protected override TimeSpan DefaultCloseTimeout => throw new NotImplementedException();

        [OpenSilver.NotImplemented]
        protected override TimeSpan DefaultOpenTimeout => throw new NotImplementedException();
        
        [OpenSilver.NotImplemented]
        protected override void OnAbort()
        {

        }

        [OpenSilver.NotImplemented]
        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override void OnClose(TimeSpan timeout)
        {

        }

        [OpenSilver.NotImplemented]
        protected override void OnEndClose(IAsyncResult result)
        {

        }

        [OpenSilver.NotImplemented]
        protected override void OnEndOpen(IAsyncResult result)
        {

        }

        [OpenSilver.NotImplemented]
        protected override void OnOpen(TimeSpan timeout)
        {

        }
    }
}
