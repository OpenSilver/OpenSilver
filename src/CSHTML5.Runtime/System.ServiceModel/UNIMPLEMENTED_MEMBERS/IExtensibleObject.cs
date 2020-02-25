﻿
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

namespace System.ServiceModel
{
    // Summary:
    //     Enable an object to participate in custom behavior, such as registering for
    //     events, or watching state transitions.
    //
    // Type parameters:
    //   T:
    //     The type of the extension class.
    public partial interface IExtensibleObject<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        // Summary:
        //     Gets a collection of extension objects for this extensible object.
        //
        // Returns:
        //     A System.ServiceModel.IExtensionCollection<T> of extension objects.
        IExtensionCollection<T> Extensions { get; }
    }
}


#endif

#endif