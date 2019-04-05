
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