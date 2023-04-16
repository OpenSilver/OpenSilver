
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

using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;
using System.Text;

namespace OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal static class TypeReferenceExtensions
    {
        private const string GlobalPrefix = "global::";

        public static bool IsString(this TypeReference typeRef) => typeRef == typeRef.Module.TypeSystem.String;

        public static TypeReference ResolveGenericParameter(this TypeReference typeRef, TypeReference elementType)
        {
            var genericParameter = typeRef as GenericParameter;
            if (genericParameter == null)
            {
                throw new ArgumentException("Type Reference must be a GenericParameter");
            }

            var genericType = genericParameter.DeclaringType.GetGenericInstanceType(elementType);

            return genericType.GenericArguments[genericParameter.Position];
        }

        public static GenericInstanceType GetGenericInstanceType(this TypeReference typeRef, TypeReference elementType)
        {
            elementType = elementType.ResolveOrThrow().BaseType;
            var typeDef = typeRef.ResolveOrThrow();
            while (elementType != null && typeDef != elementType.ResolveOrThrow())
            {
                elementType = elementType.ResolveOrThrow().BaseType;
            }

            return elementType as GenericInstanceType;
        }

        public static TypeReference PopulateGeneric(this TypeReference typeRef, TypeReference rootElementType, TypeReference ownerElementType)
        {
            if (typeRef.IsGenericParameter)
            {
                return typeRef.ResolveGenericParameter(rootElementType);
            }

            if (typeRef is GenericInstanceType instance)
            {
                var cc = ownerElementType.GetGenericInstanceType(rootElementType);
                var arg = instance.GenericArguments.Select(ga =>
                {
                    if (ga is GenericParameter genericParameter)
                    {
                        return cc.GenericArguments[genericParameter.Position];
                    }

                    return ga.PopulateGeneric(rootElementType, ownerElementType);
                }).ToArray();
                return instance.ElementType.MakeGenericInstanceType(arg);
            }

            return typeRef;
        }

        public static TypeDefinition ResolveOrThrow(this TypeReference typeRef)
        {
            var res = typeRef.Resolve();
            if (res == null)
            {
                throw new ApplicationException($"'{typeRef.FullName}' can not be resolved. Module file name is '{typeRef.Module.FileName}'. Scope name is '{typeRef.Scope.Name}'.");
            }

            return res;
        }

        public static string ConvertToString(this TypeReference typeRef)
        {
            var fullNamespace = typeRef.BuildFullPath();
            var typeName = typeRef.GetTypeNameIncludingGenericArguments(false);

            return string.IsNullOrEmpty(fullNamespace) ? typeName : $"{fullNamespace}.{typeName}";
        }

        public static string BuildFullPath(this TypeReference type)
        {
            var fullPath = string.Empty;
            var parentType = type;
            var rootType = type;
            while ((parentType = parentType.DeclaringType) != null)
            {
                if (!string.IsNullOrEmpty(fullPath)) fullPath = "." + fullPath;

                fullPath = parentType.Name + fullPath;
                rootType = parentType;
            }

            fullPath = rootType.Namespace +
                       (!string.IsNullOrEmpty(rootType.Namespace) && !string.IsNullOrEmpty(fullPath) ? "." : string.Empty) +
                       fullPath;
            return fullPath;
        }

        public static string GetTypeNameIncludingGenericArguments(this TypeReference type, bool appendNamespace)
        {
            var result = new StringBuilder();
            if (appendNamespace)
            {
                result.Append(GlobalPrefix);
                if (!string.IsNullOrEmpty(type.Namespace)) result.Append(type.Namespace + ".");
            }

            result.Append(type.Name);

            if (type is not GenericInstanceType genericInstanceType)
            {
                return result.ToString();
            }

            result = new StringBuilder(result.ToString().Split('`')[0]);
            result.Append(
                $"<{string.Join(", ", genericInstanceType.GenericArguments.Select(x => GetTypeNameIncludingGenericArguments(x, true)))}>");

            return result.ToString();
        }
    }
}
