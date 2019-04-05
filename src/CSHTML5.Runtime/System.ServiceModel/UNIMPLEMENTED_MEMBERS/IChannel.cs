
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



#if WCF_STACK

#if UNIMPLEMENTED_MEMBERS

using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Defines the basic interface that all channel objects must implement. It requires
    /// that they implement the state machine interface shared by all communication
    /// objects and that they implement a method to retrieve objects from the channel
    /// stack.
    /// </summary>
    public interface IChannel : ICommunicationObject
    {
        /// <summary>
        /// Returns a typed object requested, if present, from the appropriate layer
        /// in the channel stack.
        /// </summary>
        /// <typeparam name="T">The typed object for which the method is querying.</typeparam>
        /// <returns>The typed object T requested if it is present or null if it is not.</returns>
        T GetProperty<T>() where T : class;
    }
}
#endif

#endif