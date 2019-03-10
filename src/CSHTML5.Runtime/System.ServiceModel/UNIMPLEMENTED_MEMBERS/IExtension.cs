
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
    //     Enables an object to extend another object through aggregation.
    //
    // Type parameters:
    //   T:
    //     The object that participates in the custom behavior.
    public interface IExtension<T> where T : System.ServiceModel.IExtensibleObject<T>
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