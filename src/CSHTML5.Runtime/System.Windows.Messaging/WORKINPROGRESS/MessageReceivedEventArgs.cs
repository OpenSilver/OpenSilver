#if WORKINPROGRESS

namespace System.Windows.Messaging
{
	//
	// Summary:
	//     Provides data for the System.Windows.Messaging.LocalMessageReceiver.MessageReceived
	//     event.
	public sealed class MessageReceivedEventArgs : EventArgs
	{
		//
		// Summary:
		//     Gets the message sent from a System.Windows.Messaging.LocalMessageSender to a
		//     System.Windows.Messaging.LocalMessageReceiver.
		//
		// Returns:
		//     The message that was sent.
		public string Message { get; }
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Messaging.LocalMessageReceiver
		//     is configured to receive messages from the global namescope or from a specific
		//     domain.
		//
		// Returns:
		//     A value that indicates whether the receiver is configured for a global or domain
		//     namescope.
		public ReceiverNameScope NameScope { get; }
		//
		// Summary:
		//     Gets the name of the System.Windows.Messaging.LocalMessageReceiver that received
		//     the message.
		//
		// Returns:
		//     The name of the message receiver.
		public string ReceiverName { get; }
		//
		// Summary:
		//     Gets or sets a response message to be sent to the System.Windows.Messaging.LocalMessageSender
		//     that sent the original message.
		//
		// Returns:
		//     The response message to be sent to the sender of the original message.
		public string Response { get; set; }
		//
		// Summary:
		//     Gets the domain of the System.Windows.Messaging.LocalMessageSender that sent
		//     the message.
		//
		// Returns:
		//     The domain of the System.Windows.Messaging.LocalMessageSender that sent the message.
		public string SenderDomain { get; }
	}
}

#endif