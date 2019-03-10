
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


using System.ServiceModel;

namespace System.ServiceModel.Channels
{
#if WORKINPROGRESS
    // Summary:
    //     Defines the basic interface that all channel objects must implement. It requires
    //     that they implement the state machine interface shared by all communication
    //     objects and that they implement a method to retrieve objects from the channel
    //     stack.
    public interface IChannel : ICommunicationObject
    {
        // Summary:
        //     Returns a typed object requested, if present, from the appropriate layer
        //     in the channel stack.
        //
        // Type parameters:
        //   T:
        //     The typed object for which the method is querying.
        //
        // Returns:
        //     The typed object T requested if it is present or null if it is not.
        T GetProperty<T>() where T : class;
    }
#endif
}