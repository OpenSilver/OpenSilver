
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;

namespace System.ServiceModel
{
    // Summary:
    //     Defines the contract for the basic state machine for all communication-oriented
    //     objects in the system, including channels, the channel managers, factories,
    //     listeners, and dispatchers, and service hosts.
    public partial interface ICommunicationObject
    {
        // Summary:
        //     Gets the current state of the communication-oriented object.
        //
        // Returns:
        //     The value of the System.ServiceModel.CommunicationState of the object.
        CommunicationState State { get; }

        // Summary:
        //     Occurs when the communication object completes its transition from the closing
        //     state into the closed state.
        event EventHandler Closed;
        //
        // Summary:
        //     Occurs when the communication object first enters the closing state.
        event EventHandler Closing;
        //
        // Summary:
        //     Occurs when the communication object first enters the faulted state.
        event EventHandler Faulted;
        //
        // Summary:
        //     Occurs when the communication object completes its transition from the opening
        //     state into the opened state.
        event EventHandler Opened;
        //
        // Summary:
        //     Occurs when the communication object first enters the opening state.
        event EventHandler Opening;

        // Summary:
        //     Causes a communication object to transition immediately from its current
        //     state into the closed state.
        void Abort();
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
        //     An object, specified by the application, that contains state information
        //     associated with the asynchronous close operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous close operation.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationObjectFaultedException:
        //     System.ServiceModel.ICommunicationObject.BeginClose() was called on an object
        //     in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The default time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to close gracefully.
        IAsyncResult BeginClose(AsyncCallback callback, object state);
        //
        // Summary:
        //     Begins an asynchronous operation to close a communication object with a specified
        //     time-out.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the send operation has to complete
        //     before timing out.
        //
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous close operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information
        //     associated with the asynchronous close operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous close operation.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationObjectFaultedException:
        //     System.ServiceModel.ICommunicationObject.BeginClose() was called on an object
        //     in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The specified time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to close gracefully.
        IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state);
        //
        // Summary:
        //     Begins an asynchronous operation to open a communication object.
        //
        // Parameters:
        //   callback:
        //     The System.AsyncCallback delegate that receives notification of the completion
        //     of the asynchronous open operation.
        //
        //   state:
        //     An object, specified by the application, that contains state information
        //     associated with the asynchronous open operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous open operation.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationException:
        //     The System.ServiceModel.ICommunicationObject was unable to be opened and
        //     has entered the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The default open time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to enter the System.ServiceModel.CommunicationState.Opened state
        //     and has entered the System.ServiceModel.CommunicationState.Faulted state.
        IAsyncResult BeginOpen(AsyncCallback callback, object state);
        //
        // Summary:
        //     Begins an asynchronous operation to open a communication object within a
        //     specified interval of time.
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
        //     An object, specified by the application, that contains state information
        //     associated with the asynchronous open operation.
        //
        // Returns:
        //     The System.IAsyncResult that references the asynchronous open operation.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationException:
        //     The System.ServiceModel.ICommunicationObject was unable to be opened and
        //     has entered the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The specified time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to enter the System.ServiceModel.CommunicationState.Opened state
        //     and has entered the System.ServiceModel.CommunicationState.Faulted state.
        IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
        //
        // Summary:
        //     Causes a communication object to transition from its current state into the
        //     closed state.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationObjectFaultedException:
        //     System.ServiceModel.ICommunicationObject.Close() was called on an object
        //     in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The default close time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to close gracefully.
        void Close();
        //
        // Summary:
        //     Causes a communication object to transition from its current state into the
        //     closed state.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the send operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationObjectFaultedException:
        //     System.ServiceModel.ICommunicationObject.Close() was called on an object
        //     in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to close gracefully.
        void Close(TimeSpan timeout);
        //
        // Summary:
        //     Completes an asynchronous operation to close a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the System.ServiceModel.ICommunicationObject.BeginClose()
        //     method.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationObjectFaultedException:
        //     System.ServiceModel.ICommunicationObject.BeginClose() was called on an object
        //     in the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to close gracefully.
        void EndClose(IAsyncResult result);
        //
        // Summary:
        //     Completes an asynchronous operation to open a communication object.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult that is returned by a call to the System.ServiceModel.ICommunicationObject.BeginOpen()
        //     method.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationException:
        //     The System.ServiceModel.ICommunicationObject was unable to be opened and
        //     has entered the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to enter the System.ServiceModel.CommunicationState.Opened state
        //     and has entered the System.ServiceModel.CommunicationState.Faulted state.
        void EndOpen(IAsyncResult result);
        //
        // Summary:
        //     Causes a communication object to transition from the created state into the
        //     opened state.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationException:
        //     The System.ServiceModel.ICommunicationObject was unable to be opened and
        //     has entered the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The default open time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to enter the System.ServiceModel.CommunicationState.Opened state
        //     and has entered the System.ServiceModel.CommunicationState.Faulted state.
        void Open();
        //
        // Summary:
        //     Causes a communication object to transition from the created state into the
        //     opened state within a specified interval of time.
        //
        // Parameters:
        //   timeout:
        //     The System.Timespan that specifies how long the send operation has to complete
        //     before timing out.
        //
        // Exceptions:
        //   System.ServiceModel.CommunicationException:
        //     The System.ServiceModel.ICommunicationObject was unable to be opened and
        //     has entered the System.ServiceModel.CommunicationState.Faulted state.
        //
        //   System.TimeoutException:
        //     The specified time-out elapsed before the System.ServiceModel.ICommunicationObject
        //     was able to enter the System.ServiceModel.CommunicationState.Opened state
        //     and has entered the System.ServiceModel.CommunicationState.Faulted state.
        void Open(TimeSpan timeout);
    }
}