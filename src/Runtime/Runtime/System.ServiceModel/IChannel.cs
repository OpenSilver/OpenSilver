
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



using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    // Summary:
    //     Defines the basic interface that all channel objects must implement. It requires
    //     that they implement the state machine interface shared by all communication
    //     objects and that they implement a method to retrieve objects from the channel
    //     stack.
    public partial interface IChannel : ICommunicationObject
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
}