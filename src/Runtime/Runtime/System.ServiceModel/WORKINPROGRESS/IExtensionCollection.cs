
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



#if WCF_STACK || BRIDGE

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    /// <summary>
    /// A collection of the <see cref="IExtension{T}"/> objects that allow
    /// for retrieving the <see cref="IExtension{T}"/> by its type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the extension objects.
    /// </typeparam>
    public partial interface IExtensionCollection<T> : ICollection<IExtension<T>>, IEnumerable<IExtension<T>>, IEnumerable where T : IExtensibleObject<T>
    {
        /// <summary>
        /// Finds the specified extension object in the collection.
        /// </summary>
        /// <typeparam name="E">
        ///  The type of extension object.
        /// </typeparam>
        /// <returns>
        /// The extension object that was found.
        /// </returns>
        E Find<E>();
        
        /// <summary>
        /// Finds all extension object in the collection specified by E.
        /// </summary>
        /// <typeparam name="E">
        /// The type of extension object.
        /// </typeparam>
        /// <returns>
        /// A collection of all extension objects in the collection that implement the
        /// specified type.
        /// </returns>
        Collection<E> FindAll<E>();
    }
}

#endif
