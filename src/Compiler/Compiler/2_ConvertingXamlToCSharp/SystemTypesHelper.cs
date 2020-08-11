

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
        static Dictionary<string, Func<string, string>> LowercaseSupportedTypesAndHowToConvertFromXamlValueToCSharp
            = new Dictionary<string, Func<string, string>>()
        {
            { "double", x => "global::System.Double.Parse(" + "\"" + x + "\"" + ")" }, //todo: parse using "InvariantCulture" when supported! (so that it uses a dot for the decimal separator)
            { "string", x => "@\"" + x.Replace("\"", "\"\"") + "\"" },
            { "int16", x => x },
            { "int32", x => x },
            { "int64", x => x },
            { "boolean", x => x.ToLower() },
            { "single", x => "global::System.Single.Parse(" + "\"" + x + "\"" + ")" }, //todo: parse using "InvariantCulture" when supported! (so that it uses a dot for the decimal separator)
            { "byte", x => "(global::System.Byte)" + x },
            { "timespan", x => "global::System.TimeSpan.Parse(" + "\"" + x + "\"" + ")" }
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
