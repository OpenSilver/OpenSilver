
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

namespace System.ServiceModel
{
    // Summary:
    //     Defines the states in which an System.ServiceModel.ICommunicationObject can
    //     exist.
    public enum CommunicationState
    {
        // Summary:
        //     Indicates that the communication object has been instantiated and is configurable,
        //     but not yet open or ready for use.
        Created = 0,
        //
        // Summary:
        //     Indicates that the communication object is being transitioned from the System.ServiceModel.CommunicationState.Created
        //     state to the System.ServiceModel.CommunicationState.Opened state.
        Opening = 1,
        //
        // Summary:
        //     Indicates that the communication object is now open and ready to be used.
        Opened = 2,
        //
        // Summary:
        //     Indicates that the communication object is transitioning to the System.ServiceModel.CommunicationState.Closed
        //     state.
        Closing = 3,
        //
        // Summary:
        //     Indicates that the communication object has been closed and is no longer
        //     usable.
        Closed = 4,
        //
        // Summary:
        //     Indicates that the communication object has encountered an error or fault
        //     from which it cannot recover and from which it is no longer usable.
        Faulted = 5,
    }
}

#endif

#endif