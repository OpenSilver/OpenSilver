
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
using System.Linq;
using System.Text;

namespace OpenSilver.Compiler;

internal abstract partial class TypeReferenceHelper
{
    private sealed class TypeReferenceHelperFS : TypeReferenceHelper
    {
        public override string Global => "global.";

        public override string Null => "null";

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

            if (type.GetElementType() is TypeReference elementType &&
                elementType.GetAssemblyName() == "FSharp.Core" &&
                elementType.FullName == "Microsoft.FSharp.Control.FSharpHandler`1")
            {
                // Because of the CompiledNameAttribute, we need to replace this type, because FSharpHandler is not known at compile time.
                result.Append("Handler`1");
            }
            else
            {
                result.Append(type.Name);
            }

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
