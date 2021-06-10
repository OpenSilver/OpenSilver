#if WORKINPROGRESS

namespace System.Windows.Messaging
{
    // Summary:
    //     Provides data for the System.Windows.Messaging.LocalMessageSender.SendCompleted
    //     event.
	[OpenSilver.NotImplemented]
    public sealed class SendCompletedEventArgs
    {
        // Summary:
        //     Gets the message sent from a System.Windows.Messaging.LocalMessageSender
        //     to a System.Windows.Messaging.LocalMessageReceiver.
        //
        // Returns:
        //     The message that was sent.
		[OpenSilver.NotImplemented]
        public string Message { get; }
        //
        // Summary:
        //     Gets the domain of the System.Windows.Messaging.LocalMessageReceiver that
        //     received the message.
        //
        // Returns:
        //     The domain of the System.Windows.Messaging.LocalMessageReceiver that received
        //     the message.
		[OpenSilver.NotImplemented]
        public string ReceiverDomain { get; }
        //
        // Summary:
        //     Gets the name of the System.Windows.Messaging.LocalMessageReceiver that received
        //     the message.
        //
        // Returns:
        //     The name of the message receiver.
		[OpenSilver.NotImplemented]
        public string ReceiverName { get; }
        //
        // Summary:
        //     Gets the response message sent to the System.Windows.Messaging.LocalMessageSender
        //     from the System.Windows.Messaging.LocalMessageReceiver that received the
        //     original message.
        //
        // Returns:
        //     The response message sent to the sender of the original message.
		[OpenSilver.NotImplemented]
        public string Response { get; }

		[OpenSilver.NotImplemented]
        public Exception Error { get; }

        [OpenSilver.NotImplemented]
        public object UserState { get; }
    }
}
#endif