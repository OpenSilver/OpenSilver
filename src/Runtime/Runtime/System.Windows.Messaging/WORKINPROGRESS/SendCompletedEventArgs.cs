using System.ComponentModel;

namespace System.Windows.Messaging
{
    /// <summary>
    /// Provides data for the <see cref="LocalMessageSender.SendCompleted"/>
    /// event.
    /// </summary>
	[OpenSilver.NotImplemented]
    public sealed class SendCompletedEventArgs : AsyncCompletedEventArgs
    {
        internal SendCompletedEventArgs(Exception error, bool cancelled, object userState) 
            : base(error, cancelled, userState)
        {
        }

        /// <summary>
        /// Gets the message sent from a <see cref="LocalMessageSender"/>
        /// to a <see cref="LocalMessageReceiver"/>.
        /// </summary>
        /// <returns>
        /// The message that was sent.
        /// </returns>
		[OpenSilver.NotImplemented]
        public string Message { get; }

        /// <summary>
        /// Gets the domain of the <see cref="LocalMessageReceiver"/> that
        /// received the message.
        /// </summary>
        /// <returns>
        /// The domain of the <see cref="LocalMessageReceiver"/> that received
        /// the message.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string ReceiverDomain { get; }

        /// <summary>
        /// Gets the name of the <see cref="LocalMessageReceiver"/> that received
        /// the message.
        /// </summary>
        /// <returns>
        /// The name of the message receiver.
        /// </returns>
		[OpenSilver.NotImplemented]
        public string ReceiverName { get; }

        /// <summary>
        /// Gets the response message sent to the <see cref="LocalMessageSender"/>
        /// from the <see cref="LocalMessageReceiver"/> that received the
        /// original message.
        /// </summary>
        /// <returns>
        /// The response message sent to the sender of the original message.
        /// </returns>
		[OpenSilver.NotImplemented]
        public string Response { get; }
    }
}
