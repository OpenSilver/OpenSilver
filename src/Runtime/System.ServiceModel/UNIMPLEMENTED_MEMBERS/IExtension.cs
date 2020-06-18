
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

using System;

namespace System.ServiceModel
{
    // Summary:
    //     Enables an object to extend another object through aggregation.
    //
    // Type parameters:
    //   T:
    //     The object that participates in the custom behavior.
    public partial interface IExtension<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        // Summary:
        //     Enables an extension object to find out when it has been aggregated. Called
        //     when the extension is added to the System.ServiceModel.IExtensibleObject<T>.Extensions
        //     property.
        //
        // Parameters:
        //   owner:
        //     The extensible object that aggregates this extension.
        void Attach(T owner);
        //
        // Summary:
        //     Enables an object to find out when it is no longer aggregated. Called when
        //     an extension is removed from the System.ServiceModel.IExtensibleObject<T>.Extensions
        //     property.
        //
        // Parameters:
        //   owner:
        //     The extensible object that aggregates this extension.
        void Detach(T owner);
    }
}

#endif

#endif