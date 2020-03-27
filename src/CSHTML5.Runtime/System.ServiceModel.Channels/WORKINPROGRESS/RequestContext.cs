#if WORKINPROGRESS
namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Provides a reply that is correlated to an incoming request.
    public abstract partial class RequestContext : IDisposable
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.RequestContext
        //     class.
        protected RequestContext()
        {

        }

        //
        // Summary:
        //     When overridden in a derived class, gets the message that contains the request.
        //
        // Returns:
        //     The System.ServiceModel.Channels.Message that contains the request.
        public abstract Message RequestMessage { get; }

        //
        // Summary:
        //     When overridden in a derived class, aborts processing the request associated
        //     with the context.
        public abstract void Abort();
        //
        // Summary:
        //     When overridden in a derived class, begins an asynchronous operation to reply
        //     to the request associated with the current context.
        //
        // Parameters:
        //   message:
        //     The incoming System.ServiceModel.Channels.Message that contains the request.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives the notification of the asynchronous
        //     reply operation completion.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous reply operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous reply operation.
        public abstract IAsyncResult BeginReply(Message message, AsyncCallback callback, object state);
        //
        // Summary:
        //     When overridden in a derived class, begins an asynchronous operation to reply
        //     to the request associated with the current context within a specified interval
        //     of time.
        //
        // Parameters:
        //   message:
        //     The incoming System.ServiceModel.Channels.Message that contains the request.
        //
        //   timeout:
        //     The System.Timespan that specifies the interval of time to wait for the reply
        //     to an available request.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives the notification of the asynchronous
        //     reply operation completion.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous reply operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous reply operation.
        public abstract IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        //
        // Summary:
        //     When overridden in a derived class, closes the operation that is replying to
        //     the request context associated with the current context.
        public abstract void Close();
        //
        // Summary:
        //     When overridden in a derived class, closes the operation that is replying to
        //     the request context associated with the current context within a specified interval
        //     of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies the interval of time within which the reply
        //     operation associated with the current context must close.
        public abstract void Close(TimeSpan timeout);
        //
        // Summary:
        //     When overridden in a derived class, completes an asynchronous operation to reply
        //     to a request message.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult returned by a call to one of the Overload:System.ServiceModel.Channels.RequestContext.BeginReply
        //     methods.
        public abstract void EndReply(IAsyncResult result);
        //
        // Summary:
        //     When overridden in a derived class, replies to a request message.
        //
        // Parameters:
        //   message:
        //     The incoming System.ServiceModel.Channels.Message that contains the request.
        public abstract void Reply(Message message);
        //
        // Summary:
        //     When overridden in a derived class, replies to a request message within a specified
        //     interval of time.
        //
        // Parameters:
        //   message:
        //     The incoming System.ServiceModel.Channels.Message that contains the request.
        //
        //   timeout:
        //     The System.Timespan that specifies the interval of time to wait for the reply
        //     to a request.
        public abstract void Reply(Message message, TimeSpan timeout);
        //
        // Summary:
        //     Releases resources associated with the context.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only unmanaged
        //     resources.
        protected virtual void Dispose(bool disposing)
        {

        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
#endif