
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

namespace System.ServiceModel
{
    // Summary:
    //     Enable an object to participate in custom behavior, such as registering for
    //     events, or watching state transitions.
    //
    // Type parameters:
    //   T:
    //     The type of the extension class.
    public interface IExtensibleObject<T> where T : System.ServiceModel.IExtensibleObject<T>
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