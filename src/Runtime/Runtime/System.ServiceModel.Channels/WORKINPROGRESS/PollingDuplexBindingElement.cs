#if WORKINPROGRESS
#if MIGRATION

namespace System.ServiceModel.Channels
{
    public class PollingDuplexBindingElement :
      BindingElement,
      IPollingDuplexListenerSettings,
      IPollingDuplexSettings
    {
        public PollingDuplexBindingElement()
        {
        }

        protected PollingDuplexBindingElement(PollingDuplexBindingElement elementToBeCloned)
        {
        }

        public TimeSpan InactivityTimeout { get; set; }

        public TimeSpan ServerPollTimeout { get; set; }

        public int MaxPendingSessions { get; set; }

        public int MaxPendingMessagesPerSession { get; set; }

        public override BindingElement Clone()
        {
            throw new NotImplementedException();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            throw new NotImplementedException();
        }
    }
}

#endif
#endif


