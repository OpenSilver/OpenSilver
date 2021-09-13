using System.Collections.Generic;
using System.Security;

namespace System.Windows.Messaging
{
	//
	// Summary:
	//     Represents the receiving end of a local messaging channel between two Silverlight-based
	//     applications.
	[OpenSilver.NotImplemented]
	public sealed class LocalMessageReceiver : IDisposable
	{
		//
		// Summary:
		//     A value that represents any domain.
		[OpenSilver.NotImplemented]
		public static readonly IEnumerable<string> AnyDomain;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Messaging.LocalMessageReceiver
		//     class and configures it with the specified name.
		//
		// Parameters:
		//   receiverName:
		//     The name of the receiver.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     receiverName is null.
		//
		//   T:System.ArgumentException:
		//     receiverName is longer than 256 characters.
		[OpenSilver.NotImplemented]
		public LocalMessageReceiver(string receiverName)
		{
			
		}
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Messaging.LocalMessageReceiver
		//     class and configures it with the specified name, namescope requirement, and allowed
		//     sender domains.
		//
		// Parameters:
		//   receiverName:
		//     The name of the receiver, which must be unique either within the global namescope
		//     or the receiver's domain, depending on the value of the nameScope parameter.
		//
		//   nameScope:
		//     A value that indicates whether the receiverName is scoped to the global namescope
		//     or to the receiver's specific domain.
		//
		//   allowedSenderDomains:
		//     The domains that the receiver can receive messages from, or System.Windows.Messaging.LocalMessageReceiver.AnyDomain
		//     to receive from any domain.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     receiverName is null.-or-allowedSenderDomains is null.-or-allowedSenderDomains
		//     contains one or more null entries.
		//
		//   T:System.ArgumentException:
		//     receiverName is longer than 256 characters.-or-allowedSenderDomains contains
		//     one or more entries longer than 256 characters.-or-allowedSenderDomains contains
		//     one or more entries with invalid characters ("," and ":").
		[OpenSilver.NotImplemented]
		public LocalMessageReceiver(string receiverName, ReceiverNameScope nameScope, IEnumerable<string> allowedSenderDomains)
		{
			
		}

		//
		// Summary:
		//     Gets the domains that the receiver can receive messages from.
		//
		// Returns:
		//     The domains that the receiver can receive messages from, or System.Windows.Messaging.LocalMessageReceiver.AnyDomain
		//     if the receiver can receive messages from any domain.
		[OpenSilver.NotImplemented]
		public IEnumerable<string> AllowedSenderDomains { get; }
		//
		// Summary:
		//     Gets or sets a value that indicates whether the receiver can receive messages
		//     from a sender with a different Protected Mode setting.
		//
		// Returns:
		//     true if the receiver can receive messages regardless of the sender's Protected
		//     Mode setting; otherwise, false.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     When setting this property, the System.Windows.Messaging.LocalMessageReceiver.Listen
		//     method has already been called.
		//
		//   T:System.ObjectDisposedException:
		//     When setting this property, the System.Windows.Messaging.LocalMessageReceiver.Dispose
		//     method has already been called.
		[OpenSilver.NotImplemented]
		public bool DisableSenderTrustCheck { get; set; }
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Messaging.LocalMessageReceiver.ReceiverName
		//     is scoped to the global namescope or to the receiver's specific domain.
		//
		// Returns:
		//     A value that indicates whether the System.Windows.Messaging.LocalMessageReceiver.ReceiverName
		//     is scoped to the global namescope or to the receiver's specific domain.
		[OpenSilver.NotImplemented]
		public ReceiverNameScope NameScope { get; }
		//
		// Summary:
		//     Gets the name of the receiver.
		//
		// Returns:
		//     The name of the receiver.
		[OpenSilver.NotImplemented]
		public string ReceiverName { get; }

		//
		// Summary:
		//     Occurs when a message is received from a System.Windows.Messaging.LocalMessageSender.
		[OpenSilver.NotImplemented]
		public event EventHandler<MessageReceivedEventArgs> MessageReceived;

		//
		// Summary:
		//     Stops the receiver from receiving messages and releases all resources used by
		//     the receiver.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public void Dispose()
		{
			
		}
		//
		// Summary:
		//     Starts listening for messages from a System.Windows.Messaging.LocalMessageSender.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The System.Windows.Messaging.LocalMessageReceiver.Listen method has already been
		//     called.
		//
		//   T:System.ObjectDisposedException:
		//     The System.Windows.Messaging.LocalMessageReceiver.Dispose method has already
		//     been called.
		//
		//   T:System.Windows.Messaging.ListenFailedException:
		//     There is already a receiver registered with the same name and name scope.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public void Listen()
		{
			
		}
	}
}
