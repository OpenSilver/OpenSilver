namespace System.ServiceModel.Channels
{
	[OpenSilver.NotImplemented]
    public class PollingDuplexBindingElement :
      BindingElement,
      IPollingDuplexListenerSettings,
      IPollingDuplexSettings
    {
		[OpenSilver.NotImplemented]
        public PollingDuplexBindingElement()
        {
        }

		[OpenSilver.NotImplemented]
        protected PollingDuplexBindingElement(PollingDuplexBindingElement elementToBeCloned)
        {
        }

		[OpenSilver.NotImplemented]
        public TimeSpan InactivityTimeout { get; set; }

		[OpenSilver.NotImplemented]
        public TimeSpan ServerPollTimeout { get; set; }

		[OpenSilver.NotImplemented]
        public int MaxPendingSessions { get; set; }

		[OpenSilver.NotImplemented]
        public int MaxPendingMessagesPerSession { get; set; }

		[OpenSilver.NotImplemented]
        public override BindingElement Clone()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override T GetProperty<T>(BindingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
