
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