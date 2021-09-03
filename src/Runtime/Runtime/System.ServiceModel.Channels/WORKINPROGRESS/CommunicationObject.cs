namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Provides a common base implementation for the basic state machine common to all
    //     communication-oriented objects in the system, including channels and the channel
    //     factories.
	[OpenSilver.NotImplemented]
    public abstract partial class CommunicationObject : ICommunicationObject
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.CommunicationObject
        //     class.
		[OpenSilver.NotImplemented]
        protected CommunicationObject()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.CommunicationObject
        //     class with the mutually exclusive lock to protect the state transitions specified.
        //
        // Parameters:
        //   mutex:
        //     The mutually exclusive lock that protects the class instance during a state transition.
		[OpenSilver.NotImplemented]
        protected CommunicationObject(object mutex)
        {

        }

        //
        // Summary:
        //     Gets a value that indicates the current state of the communication object.
        //
        // Returns:
        //     A value from the System.ServiceModel.CommunicationState enumeration that indicates
        //     the current state of the object.
		[OpenSilver.NotImplemented]
        public CommunicationState State { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets the default interval of time provided
        //     for a close operation to complete.
        //
        // Returns:
        //     The default System.Timespan that specifies how long the close operation has to
        //     complete before timing out.
		[OpenSilver.NotImplemented]
        protected abstract TimeSpan DefaultCloseTimeout { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets the default interval of time provided
        //     for an open operation to complete.
        //
        // Returns:
        //     The default System.Timespan that specifies how long the open operation has to
        //     complete before timing out.
		[OpenSilver.NotImplemented]
        protected abstract TimeSpan DefaultOpenTimeout { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the communication object has been disposed.
        //
        // Returns:
        //     true if the communication object is in a closed state; otherwise, false.
		[OpenSilver.NotImplemented]
        protected bool IsDisposed { get; }
        //
        // Summary:
        //     Gets the mutually exclusive lock that protects the class instance during a state
        //     transition.
        //
        // Returns:
        //     The mutually exclusive lock that protects the class instance during a state transition.
		[OpenSilver.NotImplemented]
        protected object ThisLock { get; }

        //
        // Summary:
        //     Occurs when a communication object transitions into the closed state.
		[OpenSilver.NotImplemented]
        public event EventHandler Closed;
        //
        // Summary:
        //     Occurs when a communication object transitions into the closing state.
		[OpenSilver.NotImplemented]
        public event EventHandler Closing;
        //
        // Summary:
        //     Occurs when a communication object transitions into the faulted state.
		[OpenSilver.NotImplemented]
        public event EventHandler Faulted;
        //
        // Summary:
        //     Occurs when a communication object transitions into the opened state.
		[OpenSilver.NotImplemented]
        public event EventHandler Opened;
        //
        // Summary:
        //     Occurs when a communication object transitions into the opening state.
		[OpenSilver.NotImplemented]
        public event EventHandler Opening;

        //
        // Summary:
        //     Causes a communication object to transition immediately from its current state
        //     into the closing state.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     Either the base class System.ServiceModel.Channels.CommunicationObject.OnClosing
        //     method or the base class System.ServiceModel.Channels.CommunicationObject.OnClosed
        //     method is not called.
		[OpenSilver.NotImplemented]
        public void Abort()
        {

        }
        //
        // Summary:
        //     Begins an asynchronous operation to close a communication object.
        //
        // Parameters:
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous close operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous close operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous close operation.
        //
        // Exceptions:
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     Overload:System.ServiceModel.Channels.CommunicationObject.BeginClose was called
        //     on an object in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   T:System.TimeoutException:
        //     The default interval of time that was allotted for the operation was exceeded
        //     before the operation was completed.
		[OpenSilver.NotImplemented]
        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return null;
        }
        //
        // Summary:
        //     Begins an asynchronous operation to close a communication object with a specified
        //     timeout.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the close operation has to complete
        //     before timing out.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous close operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous close operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous close operation.
        //
        // Exceptions:
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     Overload:System.ServiceModel.Channels.CommunicationObject.BeginClose was called
        //     on an object in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
        //
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }
        //
        // Summary:
        //     Begins an asynchronous operation to close a communication object.
        //
        // Parameters:
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous open operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous open operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous open operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The communication object is not in a System.ServiceModel.CommunicationState.Opened
        //     or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state and cannot be modified.
        //
        //   T:System.TimeoutException:
        //     The default interval of time that was allotted for the operation was exceeded
        //     before the operation was completed.
		[OpenSilver.NotImplemented]
        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return null;
        }
        //
        // Summary:
        //     Begins an asynchronous operation to close a communication object within a specified
        //     interval of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the send operation has to complete
        //     before timing out.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous open operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous open operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous open operation.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
        //
        //   T:System.InvalidOperationException:
        //     The communication object is not in a System.ServiceModel.CommunicationState.Opened
        //     or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state and cannot be modified.
        //
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }
        //
        // Summary:
        //     Causes a communication object to transition from its current state into the closed
        //     state.
        //
        // Exceptions:
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     Overload:System.ServiceModel.Channels.CommunicationObject.Close was called on
        //     an object in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   T:System.TimeoutException:
        //     The default interval of time that was allotted for the operation was exceeded
        //     before the operation was completed.
		[OpenSilver.NotImplemented]
        public void Close()
        {

        }
        //
        // Summary:
        //     Causes a communication object to transition from its current state into the closed
        //     state within a specified interval of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the close operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     Overload:System.ServiceModel.Channels.CommunicationObject.Close was called on
        //     an object in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
        //
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        public void Close(TimeSpan timeout)
        {

        }
        //
        // Summary:
        //     Completes an asynchronous operation to close a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the Overload:System.ServiceModel.Channels.CommunicationObject.BeginClose
        //     method.
		[OpenSilver.NotImplemented]
        public void EndClose(IAsyncResult result)
        {

        }
        //
        // Summary:
        //     Completes an asynchronous operation to open a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the Overload:System.ServiceModel.Channels.CommunicationObject.BeginClose
        //     method.
		[OpenSilver.NotImplemented]
        public void EndOpen(IAsyncResult result)
        {

        }
        //
        // Summary:
        //     Causes a communication object to transition from the created state into the opened
        //     state.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The communication object is not in a System.ServiceModel.CommunicationState.Opened
        //     or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state and cannot be modified.
        //
        //   T:System.TimeoutException:
        //     The default interval of time that was allotted for the operation was exceeded
        //     before the operation was completed.
		[OpenSilver.NotImplemented]
        public void Open()
        {

        }
        //
        // Summary:
        //     Causes a communication object to transition from the created state into the opened
        //     state within a specified interval of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the open operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
        //
        //   T:System.InvalidOperationException:
        //     The communication object is not in a System.ServiceModel.CommunicationState.Opened
        //     or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state and cannot be modified.
        //
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        public void Open(TimeSpan timeout)
        {

        }
        //
        // Summary:
        //     Causes a communication object to transition from its current state into the faulted
        //     state.
		[OpenSilver.NotImplemented]
        protected void Fault()
        {

        }
        //
        // Summary:
        //     Gets the type of communication object.
        //
        // Returns:
        //     The type of communication object.
		[OpenSilver.NotImplemented]
        protected virtual Type GetCommunicationObjectType()
        {
            return null;
        }
        //
        // Summary:
        //     When implemented in a derived class, inserts processing on a communication object
        //     after it transitions to the closing state due to the invocation of a synchronous
        //     abort operation.
		[OpenSilver.NotImplemented]
        protected abstract void OnAbort();
        //
        // Summary:
        //     When implemented in a derived class, inserts processing after a communication
        //     object transitions to the closing state due to the invocation of an asynchronous
        //     close operation.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the on close operation has to complete
        //     before timing out.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous on close operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous on close operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous on close operation.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
		[OpenSilver.NotImplemented]
        protected abstract IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);
        //
        // Summary:
        //     When implemented in a derived class, inserts processing on a communication object
        //     after it transitions to the opening state due to the invocation of an asynchronous
        //     open operation.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the on open operation has to complete
        //     before timing out.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous on open operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information associated
        //     with the asynchronous on open operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous on open operation.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
		[OpenSilver.NotImplemented]
        protected abstract IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
        //
        // Summary:
        //     When implemented in a derived class, inserts processing on a communication object
        //     after it transitions to the closing state due to the invocation of a synchronous
        //     close operation.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the on close operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
		[OpenSilver.NotImplemented]
        protected abstract void OnClose(TimeSpan timeout);
        //
        // Summary:
        //     Invoked during the transition of a communication object into the closing state.
		[OpenSilver.NotImplemented]
        protected virtual void OnClosed()
        {

        }
        //
        // Summary:
        //     Invoked during the transition of a communication object into the closing state.
		[OpenSilver.NotImplemented]
        protected virtual void OnClosing()
        {

        }

        //
        // Summary:
        //     When implemented in a derived class, completes an asynchronous operation on the
        //     close of a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the System.ServiceModel.Channels.CommunicationObject.OnEndClose(System.IAsyncResult)
        //     method.
        //
        // Exceptions:
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        protected abstract void OnEndClose(IAsyncResult result);
        //
        // Summary:
        //     When implemented in a derived class, completes an asynchronous operation on the
        //     open of a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the System.ServiceModel.Channels.CommunicationObject.OnEndOpen(System.IAsyncResult)
        //     method.
        //
        // Exceptions:
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        protected abstract void OnEndOpen(IAsyncResult result);
        //
        // Summary:
        //     Inserts processing on a communication object after it transitions to the faulted
        //     state due to the invocation of a synchronous fault operation.
		[OpenSilver.NotImplemented]
        protected virtual void OnFaulted()
        {

        }
        //
        // Summary:
        //     When implemented in a derived class, inserts processing on a communication object
        //     after it transitions into the opening state which must complete within a specified
        //     interval of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the on open operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     timeout is less than zero.
        //
        //   T:System.TimeoutException:
        //     The interval of time specified by timeout that was allotted for the operation
        //     was exceeded before the operation was completed.
		[OpenSilver.NotImplemented]
        protected abstract void OnOpen(TimeSpan timeout);
        //
        // Summary:
        //     Invoked during the transition of a communication object into the opened state.
		[OpenSilver.NotImplemented]
        protected virtual void OnOpened()
        {

        }
        //
        // Summary:
        //     Invoked during the transition of a communication object into the opening state.
		[OpenSilver.NotImplemented]
        protected virtual void OnOpening()
        {

        }
        //
        // Summary:
        //     Throws an exception if the communication object is disposed.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state.
		[OpenSilver.NotImplemented]
        protected internal void ThrowIfDisposed()
        {

        }
        //
        // Summary:
        //     Throws an exception if the communication object the System.ServiceModel.Channels.CommunicationObject.State
        //     property is not set to the System.ServiceModel.CommunicationState.Created state.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Opening
        //     or System.ServiceModel.CommunicationState.Opened state.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state.
		[OpenSilver.NotImplemented]
        protected internal void ThrowIfDisposedOrImmutable()
        {

        }
        //
        // Summary:
        //     Throws an exception if the communication object is not in the System.ServiceModel.CommunicationState.Opened
        //     state.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The communication object is not in a System.ServiceModel.CommunicationState.Created
        //     or System.ServiceModel.CommunicationState.Opening state and cannot be used.
        //
        //   T:System.ObjectDisposedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Closing
        //     or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
        //
        //   T:System.ServiceModel.CommunicationObjectFaultedException:
        //     The communication object is in a System.ServiceModel.CommunicationState.Faulted
        //     state and cannot be modified.
		[OpenSilver.NotImplemented]
        protected internal void ThrowIfDisposedOrNotOpen()
        {

        }
    }
}
