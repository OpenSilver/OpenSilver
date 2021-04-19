

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal static class SystemTypesHelper
    {
        private const string InvariantCulture = "global::System.Globalization.CultureInfo.InvariantCulture";

        static Dictionary<string, Func<string, string>> LowercaseSupportedTypesAndHowToConvertFromXamlValueToCSharp
            = new Dictionary<string, Func<string, string>>()
        {
#if BRIDGE || CSHTML5BLAZOR
            { "double", x => $"global::System.Double.Parse(\"{x}\", {InvariantCulture})" },
            { "single", x => $"global::System.Single.Parse(\"{x}\", {InvariantCulture})" },
            { "timespan", x => $"global::System.TimeSpan.Parse(\"{x}\", {InvariantCulture})" },
#else // Obsolete, JSIL
            { "double", x => "global::System.Double.Parse(" + "\"" + x + "\"" + ")" },
            { "single", x => "global::System.Single.Parse(" + "\"" + x + "\"" + ")" },
            { "timespan", x => "global::System.TimeSpan.Parse(" + "\"" + x + "\"" + ")" }
#endif
            { "string", x => $"@\"{x.Replace("\"", "\"\"")}\"" },
            { "boolean", x => x.ToLower() },
            { "byte", x => $"(global::System.Byte){x}" },
            { "int16", x => x },
            { "int32", x => x },
            { "int64", x => x },
        };

        static Dictionary<string, string> DefaultValueOfSystemTypes =
            new Dictionary<string, string>()
            {
                {"double", "0"},
                {"string", ""},
                {"int16", "0"},
                {"int32", "0"},
                {"int64", "0"},
                {"boolean", "false"},
                {"single", "0"},
                {"byte", "0"},
                {"timespan", "new global::System.TimeSpan()" }
            };

        internal static bool IsSupportedSystemType(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            if (namespaceName.ToLower() == "system"
                && LowercaseSupportedTypesAndHowToConvertFromXamlValueToCSharp.ContainsKey(localTypeName.ToLower())
                && (assemblyNameIfAny == null || assemblyNameIfAny.ToLower() == "mscorlib"))
                return true;
            else
                return false;
        }

        internal static string GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return "global::System." + localTypeName;
        }

        internal static string ConvertSytemTypeFromXamlValueToCSharp(string xamlValueToConvert, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return LowercaseSupportedTypesAndHowToConvertFromXamlValueToCSharp[localTypeName.ToLower()](xamlValueToConvert);
        }

        internal static string GetDefaultValueOfSystemTypeAsString(string localTypeName)
        {
            if (DefaultValueOfSystemTypes.ContainsKey(localTypeName.ToLower()))
            {
                return DefaultValueOfSystemTypes[localTypeName.ToLower()];
            }
            return null;
        }
    }
}
