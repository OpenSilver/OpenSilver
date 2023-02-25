
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
using System.Diagnostics;
using System.Globalization;

namespace OpenSilver.Compiler
{
    internal static class SystemTypesHelper
    {
        private const string InvariantCulture = "global::System.Globalization.CultureInfo.InvariantCulture";
        private const string mscorlib = "mscorlib";

        private static Dictionary<string, Func<string, string>> SupportedIntrinsicTypes { get; } =
            new Dictionary<string, Func<string, string>>(9)
            {
                ["system.double"] = (s => ConvertToDouble(s)),
                ["system.single"] = (s => ConvertToSingle(s)),
                ["system.timespan"] = (s => ConvertToTimeSpan(s)),
                ["system.string"] = (s => ConvertToString(s)),
                ["system.boolean"] = (s => ConvertToBoolean(s)),
                ["system.byte"] = (s => ConvertToByte(s)),
                ["system.int16"] = (s => ConvertToInt16(s)),
                ["system.int32"] = (s => ConvertToInt32(s)),
                ["system.int64"] = (s => ConvertToInt64(s)),
            };

        private static Dictionary<string, string> SupportIntrinsicTypesDefaultValues { get; } =
            new Dictionary<string, string>(9)
            {
                ["system.double"] = "0D",
                ["system.single"] = "0F",
                ["system.timespan"] = "new global::System.TimeSpan()",
                ["system.string"] = "",
                ["system.boolean"] = "false",
                ["system.byte"] = "(global::System.Byte)0",
                ["system.int16"] = "(global::System.Int16)0",
                ["system.int32"] = "0",
                ["system.int64"] = "0L",
            };

        public static bool IsSupportedSystemType(string typeFullName, string assemblyIfAny)
        {
            if (IsMscorlibOrNull(assemblyIfAny))
            {
                return SupportedIntrinsicTypes.ContainsKey(typeFullName.ToLower());
            }
            
            return false;
        }

        public static string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny)
        {
            Debug.Assert(IsMscorlibOrNull(assemblyIfAny));
            Debug.Assert(namespaceName == "System");

            return $"global::{namespaceName}.{typeName}";
        }

        public static string ConvertFromInvariantString(string source, string typeFullName)
        {
            if (SupportedIntrinsicTypes.TryGetValue(typeFullName.ToLower(), out var converter))
            {
                Debug.Assert(converter != null);
                return converter(source);
            }

            throw new InvalidOperationException(
                $"'{typeFullName}' is not a supported system type."
            );
        }

        public static string GetDefaultValue(string namespaceName, string typeName, string assemblyIfAny)
        {
            if (IsMscorlibOrNull(assemblyIfAny))
            {
                string key = GetKey(namespaceName, typeName);

                if (SupportIntrinsicTypesDefaultValues.TryGetValue(key, out string value))
                {
                    return value;
                }
            }

            return null;
        }

        private static string ConvertToDouble(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "global::System.Double.NaN";

                case "infinity":
                    return "global::System.Double.PositiveInfinity";

                case "-infinity":
                    return "global::System.Double.NegativeInfinity";
            }

            if (value.EndsWith("d"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.EndsWith("."))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                value = "0";
            }

            return $"{value}D";
        }

        private static string ConvertToSingle(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "global::System.Single.NaN";

                case "infinity":
                    return "global::System.Single.PositiveInfinity";

                case "-infinity":
                    return "global::System.Single.NegativeInfinity";                    
            }

            if (value.EndsWith("f"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.EndsWith("."))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                value = "0";
            }

            return $"{value}F";
        }

        private static string ConvertToTimeSpan(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.timespan"];
            }

            // Optimization to avoid parsing at runtime
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return $"new global::System.TimeSpan({timeSpan.Ticks}L)";
            }

            return $"global::System.TimeSpan.Parse({Escape(value)}, {InvariantCulture})";
        }

        private static string ConvertToString(string source)
        {
            return Escape(source);
        }

        private static string ConvertToBoolean(string source)
        {
            string value = source.Trim();
            
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.boolean"];
            }

            return value.ToLower();
        }

        private static string ConvertToByte(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.byte"];
            }

            return $"(global::System.Byte){value}";
        }

        private static string ConvertToInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int16"];
            }

            return $"(global::System.Int16){value}";
        }

        private static string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int32"];
            }

            return value;
        }

        private static string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int64"];
            }

            return $"{value}L";
        }

        private static bool IsMscorlibOrNull(string assemblyName)
        {
            if (assemblyName == null || 
                assemblyName.Equals(mscorlib, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static string GetKey(string namespaceName, string typeName)
        {
            return $"{namespaceName}.{typeName}".ToLower();
        }

        private static string Escape(string s)
        {
            return string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
        }
    }
}
