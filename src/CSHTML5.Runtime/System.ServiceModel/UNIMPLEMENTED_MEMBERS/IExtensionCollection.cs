
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    // Summary:
    //     A collection of the System.ServiceModel.IExtension<T> objects that allow
    //     for retrieving the System.ServiceModel.IExtension<T> by its type.
    //
    // Type parameters:
    //   T:
    //     The type of the extension objects.
    public interface IExtensionCollection<T> : ICollection<IExtension<T>>, IEnumerable<IExtension<T>>, IEnumerable where T : System.ServiceModel.IExtensibleObject<T>
    {
        // Summary:
        //     Finds the specified extension object in the collection.
        //
        // Type parameters:
        //   E:
        //     The type of extension object.
        //
        // Returns:
        //     The extension object that was found.
        E Find<E>();
        //
        // Summary:
        //     Finds all extension object in the collection specified by E.
        //
        // Type parameters:
        //   E:
        //     The type of extension object.
        //
        // Returns:
        //     A collection of all extension objects in the collection that implement the
        //     specified type.
        Collection<E> FindAll<E>();
    }
}

#endif

#endif