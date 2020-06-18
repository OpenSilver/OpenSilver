#if WORKINPROGRESS
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    //
    // Summary:
    //     A collection of the System.ServiceModel.IExtension`1 objects that allow for retrieving
    //     the System.ServiceModel.IExtension`1 objects by their type.
    //
    // Type parameters:
    //   T:
    //     The type of the extension objects.
    public partial interface IExtensionCollection<T> : ICollection<IExtension<T>>, IEnumerable<IExtension<T>>, IEnumerable where T : IExtensibleObject<T>
    {
        //
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
        //     Finds all extension objects in the collection specified by E.
        //
        // Type parameters:
        //   E:
        //     The type of extension object.
        //
        // Returns:
        //     A collection of all extension objects in the collection that implements the specified
        //     type.
        Collection<E> FindAll<E>();
    }
}

#endif