
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
using System.Diagnostics;
using System.Text;

namespace OpenSilver.Compiler;

internal abstract partial class TypeReferenceHelper
{
    public static TypeReferenceHelper CSharp { get; } = new TypeReferenceHelperCS();

    public static TypeReferenceHelper VisualBasic { get; } = new TypeReferenceHelperVB();

    public static TypeReferenceHelper FSharp { get; } = new TypeReferenceHelperFS();

    public string ConvertToString(TypeReference type)
    {
        var fullNamespace = BuildFullPath(type);
        var typeName = GetTypeNameIncludingGenericArguments(type, false);

        return string.IsNullOrEmpty(fullNamespace) ? typeName : $"{fullNamespace}.{typeName}";
    }

    public string BuildFullPath(TypeReference type)
    {
        var fullPath = new StringBuilder();
        var parentType = type;
        var rootType = type;

        while ((parentType = parentType.DeclaringType) != null)
        {
            if (fullPath.Length > 0)
            {
                fullPath.Insert(0, '.');
            }

            fullPath.Insert(0, parentType.Name);

            rootType = parentType;
        }

        if (!string.IsNullOrEmpty(rootType.Namespace))
        {
            if (fullPath.Length > 0)
            {
                fullPath.Insert(0, '.');
            }

            fullPath.Insert(0, rootType.Namespace);
        }

        return fullPath.ToString();
    }

    public abstract string Global { get; }

    public abstract string Null { get; }

    public abstract string GetTypeNameIncludingGenericArguments(TypeReference type, bool appendNamespace);

    public abstract string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue);

    public virtual bool IsEnum(TypeDefinition type)
    {
        Debug.Assert(type is not null);
        return type.IsEnum;
    }
}
