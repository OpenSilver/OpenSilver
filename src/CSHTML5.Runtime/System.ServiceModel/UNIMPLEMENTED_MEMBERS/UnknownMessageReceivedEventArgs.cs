
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


#if WCF_STACK

#if UNIMPLEMENTED_MEMBERS

using System;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Contains the message received by a channel and cannot be associated with
    /// any callback operation or pending request.
    /// </summary>
    public sealed class UnknownMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the unknown message that caused the event.
        /// </summary>
        public Message Message
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

#endif

#endif