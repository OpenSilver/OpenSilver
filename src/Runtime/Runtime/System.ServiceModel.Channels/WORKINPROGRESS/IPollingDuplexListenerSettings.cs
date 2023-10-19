namespace System.ServiceModel.Channels
{
    internal interface IPollingDuplexListenerSettings : IPollingDuplexSettings
    {
        TimeSpan ServerPollTimeout { get; set; }

        int MaxPendingSessions { get; set; }

        int MaxPendingMessagesPerSession { get; set; }
    }
}
