
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if WCF_STACK || BRIDGE

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Defines the behavior of outbound request and request/reply channels used
    /// by client applications.
    /// </summary>
    public interface IClientChannel : IDisposable
#if WORKINPROGRESS
        , IChannel
#endif
#if UNIMPLEMENTED_MEMBERS
        , IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>
#endif
        //, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>
    {
        // Returns:
        //     true if Windows Communication Foundation (WCF) is permitted to invoke interactive
        //     channel initializers; otherwise, false.
        /// <summary>
        /// Gets or sets a value indicating whether System.ServiceModel.IClientChannel.DisplayInitializationUI()
        /// attempts to call the System.ServiceModel.Dispatcher.IInteractiveChannelInitializer
        /// objects in the System.ServiceModel.Dispatcher.ClientRuntime.InteractiveChannelInitializers
        /// property or throws if that collection is not empty.
        /// </summary>
        bool AllowInitializationUI { get; set; }

        // Returns:
        //     true if the System.ServiceModel.IClientChannel.DisplayInitializationUI()
        //     method was called (or the System.ServiceModel.IClientChannel.BeginDisplayInitializationUI(System.AsyncCallback,System.Object)
        //     and System.ServiceModel.IClientChannel.EndDisplayInitializationUI(System.IAsyncResult)
        //     methods; otherwise, false.
        /// <summary>
        /// Gets a value indicating whether a call was done to a user interface to obtain
        /// credential information.
        /// </summary>
        bool DidInteractiveInitialization { get; }

        /// <summary>
        /// Gets the URI that contains the transport address to which messages are sent
        /// on the client channel.
        /// </summary>
        Uri Via { get; }

#if UNIMPLEMENTED_MEMBERS
        /// <summary>
        /// This is a reserved event.
        /// </summary>
        event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived;
#endif

        /// <summary>
        /// An asynchronous call to begin using a user interface to obtain credential
        /// information.
        /// </summary>
        /// <param name="callback">The method that is called when this method completes.</param>
        /// <param name="state">Information about the state of the channel.</param>
        /// <returns>The System.IAsyncResult to use to call back when processing has completed.</returns>
        IAsyncResult BeginDisplayInitializationUI(AsyncCallback callback, object state);

        /// <summary>
        /// A call to a user interface to obtain credential information.
        /// </summary>
        void DisplayInitializationUI();

        /// <summary>
        /// Called when the call to System.ServiceModel.IClientChannel.BeginDisplayInitializationUI(System.AsyncCallback,System.Object)
        /// has finished.
        /// </summary>
        /// <param name="result">The System.IAsyncResult.</param>
        void EndDisplayInitializationUI(IAsyncResult result);
    }
}

#endif