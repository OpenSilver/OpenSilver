namespace System.ServiceModel
{
    /// <summary>
    /// Represents a communication error in either the service or client application.
    /// </summary>
    public partial class CommunicationException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.CommunicationException
        /// class.
        /// </summary>
        public CommunicationException()
        { }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.CommunicationException
        /// class, using the specified message.
        /// </summary>
        /// <param name="message">The description of the error condition.</param>
        public CommunicationException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.CommunicationException
        /// class, using the specified message and the inner exception.
        /// </summary>
        /// <param name="message">The description of the error condition.</param>
        /// <param name="innerException">The inner exception to be used.</param>
        public CommunicationException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}