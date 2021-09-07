
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

#if !OPENSILVER

#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

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
    public partial interface IClientChannel : IDisposable
#if BRIDGE
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
#endif