
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
using Mono.Cecil;

namespace OpenSilver.Compiler;

internal static class TypeDefinitionExtensions
{
    /// <summary>
    /// Returns true if type is a subclass of targetType.
    /// Does not test interface inheritance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static bool IsSubclassOf(this TypeDefinition type, TypeDefinition targetType)
    {
        foreach (TypeDefinition baseType in type.EnumerateBaseClasses(skipSelf: true))
        {
            if (Equals(baseType, targetType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true if type directly or indirectly implements interfaceType.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool DoesAnySubTypeImplementInterface(this TypeDefinition type, TypeDefinition interfaceType)
    {
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException("Parent type must be an interface.", nameof(interfaceType));
        }

        foreach (TypeDefinition baseType in type.EnumerateBaseClasses())
        {
            if (baseType.DoesSpecificTypeImplementInterface(interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    public static bool Equals(TypeDefinition a, TypeDefinition b) =>
        a.MetadataToken == b.MetadataToken && a.FullName == b.FullName;

    /// <summary>
    /// Returns true if type directly implements interfaceType.
    /// Does not test parent classes of type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    private static bool DoesSpecificTypeImplementInterface(this TypeDefinition type, TypeDefinition interfaceType)
    {
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException("Parent type must be an interface.", nameof(interfaceType));
        }

        foreach (InterfaceImplementation impl in type.Interfaces)
        {
            if (DoesSpecificInterfaceImplementInterface(impl.InterfaceType.Resolve(), interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true if type is equal to or implements the interface.
    /// Both type and interfaceType must be interfaces.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    private static bool DoesSpecificInterfaceImplementInterface(TypeDefinition type, TypeDefinition interfaceType)
    {
        if (!type.IsInterface)
        {
            throw new ArgumentException("Child type must be an interface.", nameof(interfaceType));
        }
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException("Parent type must be an interface.", nameof(interfaceType));
        }

        return Equals(type, interfaceType) || type.DoesAnySubTypeImplementInterface(interfaceType);
    }

    /// <summary>
    /// Enumerate the current type, it's parent and all the way to the top type
    /// </summary>
    /// <param name="classType"></param>
    /// <returns></returns>
    private static IEnumerable<TypeDefinition> EnumerateBaseClasses(this TypeDefinition classType, bool skipSelf = false)
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
}