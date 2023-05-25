
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal static class TypeDefinitionExtensions
    {
        /// <summary>
        /// Returns true if childType is a subclass of parentType.
        /// Does not test interface inheritance.
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static bool IsSubclassOf(this TypeDefinition childType, TypeDefinition parentType) =>
           childType.EnumerateBaseClasses(skipSelf: true).Any(b => Equals(b, parentType));

        /// <summary>
        /// Returns true if childType directly or indirectly implements parentInterface.
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="parentInterface"></param>
        /// <returns></returns>
        public static bool DoesAnySubTypeImplementInterface(this TypeDefinition childType, TypeDefinition parentInterface)
        {
            if (!parentInterface.IsInterface)
            {
                throw new ArgumentException("Parent type must be an interface", nameof(parentInterface));
            }

            return
                childType
                .EnumerateBaseClasses()
                .Any(typeDefinition => typeDefinition.DoesSpecificTypeImplementInterface(parentInterface));
        }

        /// <summary>
        /// Returns true if childType directly implements parentInterface.
        /// Does not test parent classes of childType.
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="parentInterface"></param>
        /// <returns></returns>
        public static bool DoesSpecificTypeImplementInterface(this TypeDefinition childType, TypeDefinition parentInterface)
        {
            if (!parentInterface.IsInterface)
            {
                throw new ArgumentException("Parent type must be an interface", nameof(parentInterface));
            }

            return childType
               .Interfaces
               .Any(impl => DoesSpecificInterfaceImplementInterface(impl.InterfaceType.ResolveOrThrow(), parentInterface));
        }

        /// <summary>
        /// Returns true if child is equal to or implements parent.
        /// Both child and parent must be interfaces.
        /// </summary>
        /// <param name="childInterface"></param>
        /// <param name="parentInterface"></param>
        /// <returns></returns>
        public static bool DoesSpecificInterfaceImplementInterface(TypeDefinition childInterface, TypeDefinition parentInterface)
        {
            if (!childInterface.IsInterface)
            {
                throw new ArgumentException("Child type must be an interface", nameof(parentInterface));
            }
            if (!parentInterface.IsInterface)
            {
                throw new ArgumentException("Parent type must be an interface", nameof(parentInterface));
            }
            return Equals(childInterface, parentInterface) || childInterface.DoesAnySubTypeImplementInterface(parentInterface);
        }

        /// <summary>
        /// Is source type assignable to target type
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this TypeDefinition target, TypeDefinition source)
            => target == source
              || Equals(target, source)
              || source.IsSubclassOf(target)
              || target.IsInterface && source.DoesAnySubTypeImplementInterface(target);

        /// <summary>
        /// Enumerate the current type, it's parent and all the way to the top type
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static IEnumerable<TypeDefinition> EnumerateBaseClasses(this TypeDefinition classType, bool skipSelf = false)
        {
            if (classType == null)
            {
                yield break;
            }

            TypeDefinition td = classType;
            if (skipSelf)
            {
                if (classType.BaseType == null)
                {
                    yield break;
                }

                td = classType.BaseType.ResolveOrThrow();
            }

            for (; td != null; td = td.BaseType?.ResolveOrThrow())
            {
                yield return td;
            }
        }

        public static bool Equals(TypeDefinition a, TypeDefinition b)
        {
            return
                a.MetadataToken == b.MetadataToken
                && a.FullName == b.FullName;
        }
    }
}