#if WORKINPROGRESS

namespace System.Windows.Messaging
{
	[OpenSilver.NotImplemented]
    public sealed class LocalMessageSender
    {
        private string indexedName;

		[OpenSilver.NotImplemented]
        public LocalMessageSender(string indexedName)
        {
            this.indexedName = indexedName;
        }

        // Summary:
        //     Occurs when the message has been successfully sent.
		[OpenSilver.NotImplemented]
        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        // Summary:
        //     Sends the specified message to the configured receiver asynchronously.
        //
        // Parameters:
        //   message:
        //     The message to send.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     message is null.
        //
        //   System.ArgumentException:
        //     message is longer than 40,960 characters.
		[OpenSilver.NotImplemented]
        public void SendAsync(string message)
        {

        }

        [OpenSilver.NotImplemented]
        public void SendAsync(string message, object userState)
        {

        }
    }
}
#endif