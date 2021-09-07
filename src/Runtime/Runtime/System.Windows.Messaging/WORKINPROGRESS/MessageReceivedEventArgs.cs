namespace System.Windows.Messaging
{
	//
	// Summary:
	//     Provides data for the System.Windows.Messaging.LocalMessageReceiver.MessageReceived
	//     event.
	[OpenSilver.NotImplemented]
	public sealed class MessageReceivedEventArgs : EventArgs
	{
		//
		// Summary:
		//     Gets the message sent from a System.Windows.Messaging.LocalMessageSender to a
		//     System.Windows.Messaging.LocalMessageReceiver.
		//
		// Returns:
		//     The message that was sent.
		[OpenSilver.NotImplemented]
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
		[OpenSilver.NotImplemented]
		public ReceiverNameScope NameScope { get; }
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
		//     Gets or sets a response message to be sent to the System.Windows.Messaging.LocalMessageSender
		//     that sent the original message.
		//
		// Returns:
		//     The response message to be sent to the sender of the original message.
		[OpenSilver.NotImplemented]
		public string Response { get; set; }
		//
		// Summary:
		//     Gets the domain of the System.Windows.Messaging.LocalMessageSender that sent
		//     the message.
		//
		// Returns:
		//     The domain of the System.Windows.Messaging.LocalMessageSender that sent the message.
		[OpenSilver.NotImplemented]
		public string SenderDomain { get; }
	}
}
