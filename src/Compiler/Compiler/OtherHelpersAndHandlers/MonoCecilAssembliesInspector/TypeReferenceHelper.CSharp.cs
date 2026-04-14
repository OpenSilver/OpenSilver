
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
using System.Linq;
using System.Text;

namespace OpenSilver.Compiler;

internal abstract partial class TypeReferenceHelper
{
    private sealed class TypeReferenceHelperCS : TypeReferenceHelper
    {
        public override string Global => "global::";

        public override string Null => "null";

        public override string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
        {
            Debug.Assert(enumType is not null && IsEnum(enumType));

            name = name.Trim();

            MemberFlags flags = ignoreCase ?
                MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Static :
                MemberFlags.Public | MemberFlags.Static;

            var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
                enumType,
                name,
                flags,
                out _);

            if (field is not null)
            {
                return $"{Global}{ConvertToString(enumType)}.{field.Name}";
            }
            if (allowIntegerValue)
            {
                if (long.TryParse(name, out long l))
                {
                    return $"({Global}{ConvertToString(enumType)}){l}";
                }
                if (ulong.TryParse(name, out ulong ul))
                {
                    return $"({Global}{ConvertToString(enumType)}){ul}";
                }
            }
            return null;
        }

        public override string GetTypeNameIncludingGenericArguments(TypeReference type, bool appendNamespace)
        {
            var result = new StringBuilder();

            if (appendNamespace)
            {
                result.Append(Global);
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    result.Append(type.Namespace).Append('.');
                }
            }

            if (type is not GenericInstanceType genericInstanceType)
            {
                result.Append(type.Name);
                return result.ToString();
            }

            int index = type.Name.IndexOf('`');
            result.Append(type.Name, 0, index == -1 ? type.Name.Length : index);

            result.Append(
                $"<{string.Join(", ", genericInstanceType.GenericArguments.Select(x => GetTypeNameIncludingGenericArguments(x, true)))}>");

            return result.ToString();
        }
    }
}
