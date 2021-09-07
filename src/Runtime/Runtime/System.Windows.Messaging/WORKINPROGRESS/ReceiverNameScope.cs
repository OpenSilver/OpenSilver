namespace System.Windows.Messaging
{
	//
	// Summary:
	//     Defines values that indicate whether a System.Windows.Messaging.LocalMessageReceiver
	//     name is scoped to the global domain or to a specific domain.
	public enum ReceiverNameScope
	{
		//
		// Summary:
		//     The receiver name is scoped to the domain of the receiver.
		Domain = 0,
		//
		// Summary:
		//     The receiver name is scoped to the global domain.
		Global = 1
	}
}
