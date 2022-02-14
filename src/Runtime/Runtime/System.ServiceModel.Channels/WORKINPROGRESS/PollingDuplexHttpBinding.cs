#if MIGRATION

namespace System.ServiceModel.Channels
{
    [OpenSilver.NotImplemented]
    public class PollingDuplexHttpBinding : Binding
    {
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpBinding()
        {
        }

        [OpenSilver.NotImplemented]
        public PollingDuplexMode DuplexMode { get; set; }

        [OpenSilver.NotImplemented]
        public EnvelopeVersion EnvelopeVersion { get; }

        [OpenSilver.NotImplemented]
        public TimeSpan InactivityTimeout { get; set; }

        [OpenSilver.NotImplemented]
        public int MaxBufferSize { get; set; }

        [OpenSilver.NotImplemented]
        public long MaxReceivedMessageSize { get; set; }

        [OpenSilver.NotImplemented]
        public override string Scheme { get; }

        [OpenSilver.NotImplemented]
        public PollingDuplexHttpSecurity Security { get; }

        [OpenSilver.NotImplemented]
        public override BindingElementCollection CreateBindingElements()
        {
            throw new NotImplementedException();
        }
    }
}

#endif
