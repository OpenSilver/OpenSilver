namespace System.Windows.Messaging
{
    /// <summary>
    /// Represents the sending end of a local messaging channel between two Silverlight-based
    /// applications.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class LocalMessageSender
    {
        /// <summary>
        /// A value that represents the global domain.
        /// </summary>
        public const string Global = "*";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMessageSender"/>
        /// class and configures it to send messages to the receiver with the specified name.
        /// </summary>
        /// <param name="receiverName">
        /// The <see cref="LocalMessageReceiver.ReceiverName"/> property value
        /// of the <see cref="LocalMessageReceiver"/> that this sender will send
        /// messages to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// receiverName is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// receiverName is longer than 256 characters.
        /// </exception>
		[OpenSilver.NotImplemented]
        public LocalMessageSender(string receiverName) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMessageSender"/>
        /// class and configures it to send messages to the receiver with the specified name
        /// and domain.
        /// </summary>
        /// <param name="receiverName">
        /// The <see cref="LocalMessageReceiver.ReceiverName"/> property value
        /// of the <see cref="LocalMessageReceiver"/> that this sender will send
        /// messages to.
        /// </param>
        /// <param name="receiverDomain">
        /// The domain of the <see cref="LocalMessageReceiver"/> that this sender
        /// will send messages to, or <see cref="LocalMessageSender.Global"/>
        /// if the receiver is not scoped to a domain.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// receiverName is null.-or-receiverDomain is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// receiverName is longer than 256 characters.-or-receiverDomain is longer than
        /// 256 characters.-or-receiverDomain contains one or more invalid characters (","
        /// and ":").
        /// </exception>
		[OpenSilver.NotImplemented]
        public LocalMessageSender(string receiverName, string receiverDomain)
        {
        }

        /// <summary>
        /// Occurs when the message has been successfully sent.
        /// </summary>
		[OpenSilver.NotImplemented]
        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        /// <summary>
        /// Gets the domain of the <see cref="LocalMessageReceiver"/> that this
        /// sender will send messages to.
        /// </summary>
        /// <returns>
        /// The domain of the <see cref="LocalMessageReceiver"/> that this sender
        /// will send messages to, depending on the receiver's <see cref="LocalMessageReceiver.NameScope"/>
        /// property value.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string ReceiverDomain { get; }

        /// <summary>
        /// Gets the name of the <see cref="LocalMessageReceiver"/> that this
        /// sender will send messages to.
        /// </summary>
        /// <returns>
        ///  The name of the <see cref="LocalMessageReceiver"/> that this sender
        ///  will send messages to.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string ReceiverName { get; }

        /// <summary>
        /// Sends the specified message to the configured receiver asynchronously.
        /// </summary>
        /// <param name="message">
        /// The message to send.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// message is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// message is longer than 40,960 characters.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void SendAsync(string message)
        {
        }

        /// <summary>
        /// Sends the specified messages to the configured receiver asynchronously.
        /// </summary>
        /// <param name="message">
        /// The message to send.
        /// </param>
        /// <param name="userState">
        /// A unique user-state object that functions as a task ID for the message transfer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// message is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// message is longer than 40,960 characters.
        /// </exception>
        [OpenSilver.NotImplemented]
        public void SendAsync(string message, object userState)
        {
        }
    }
}
