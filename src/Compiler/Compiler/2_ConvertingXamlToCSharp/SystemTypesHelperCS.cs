
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
    internal sealed class SystemTypesHelperCS : SystemTypesHelper
    {
        private const string InvariantCulture = "global::System.Globalization.CultureInfo.InvariantCulture";

        private static readonly Dictionary<(string Namespace, string Type), string> _supportIntrinsicTypesDefaultValues =
            new(16, StringTupleComparer.Instance)
            {
                [("system", "double")] = "0D",
                [("system", "single")] = "0F",
                [("system", "timespan")] = "new global::System.TimeSpan()",
                [("system", "string")] = "",
                [("system", "boolean")] = "false",
                [("system", "byte")] = "(global::System.Byte)0",
                [("system", "int16")] = "(global::System.Int16)0",
                [("system", "int32")] = "0",
                [("system", "int64")] = "0L",
                [("system", "uint16")] = "(global::System.UInt16)0",
                [("system", "uint32")] = "0U",
                [("system", "uint64")] = "0UL",
                [("system", "sbyte")] = "(global::System.SByte)0",
                [("system", "char")] = "(global::System.Char)0",
                [("system", "decimal")] = "0M",
                [("system", "object")] = "\"\"",
            };

        public override bool IsNullableType(string fullTypeName, string assembly, out string underlyingType)
        {
            const string Nullable = "System.Nullable<";

            if (fullTypeName.StartsWith(Nullable) && IsCoreLibraryOrNull(assembly))
            {
                underlyingType = fullTypeName.Substring(Nullable.Length, fullTypeName.Length - Nullable.Length - 1);
                return true;
            }

            underlyingType = null;
            return false;
        }

        public override string GetDefaultValue(string namespaceName, string typeName, string assemblyIfAny)
        {
            if (IsCoreLibraryOrNull(assemblyIfAny))
            {
                if (_supportIntrinsicTypesDefaultValues.TryGetValue((namespaceName, typeName), out string value))
                {
                    return value;
                }
            }

            return null;
        }

        public override string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny)
        {
            Debug.Assert(IsCoreLibraryOrNull(assemblyIfAny));
            Debug.Assert(namespaceName == "System");

            return $"global::{namespaceName}.{typeName}";
        }

        protected override string ConvertToDouble(string source)
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

        protected override string ConvertToSingle(string source)
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

        protected override string ConvertToTimeSpan(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "timespan")];
            }

            // Optimization to avoid parsing at runtime
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return $"new global::System.TimeSpan({timeSpan.Ticks}L)";
            }

            return $"global::System.TimeSpan.Parse({Escape(value)}, {InvariantCulture})";
        }

        protected override string ConvertToString(string source)
        {
            return Escape(source);
        }

        protected override string ConvertToBoolean(string source)
        {
            string value = source.Trim();
            
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "boolean")];
            }

            return value.ToLower();
        }

        protected override string ConvertToByte(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "byte")];
            }

            return $"(global::System.Byte){value}";
        }

        protected override string ConvertToInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int16")];
            }

            return $"(global::System.Int16){value}";
        }

        protected override string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int32")];
            }

            return value;
        }

        protected override string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int64")];
            }

            return $"{value}L";
        }

        protected override string ConvertToUInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint16")];
            }

            return $"(global::System.UInt16){value}";
        }

        protected override string ConvertToUInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint32")];
            }

            return $"{value}U";
        }

        protected override string ConvertToUInt64(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint64")];
            }

            return $"{value}UL";
        }

        protected override string ConvertToSByte(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "sbyte")];
            }

            return $"(global::System.SByte){value}";
        }

        protected override string ConvertToChar(string source)
        {
            if (source.Length == 1)
            {
                return $"'{source}'";
            }

            return _supportIntrinsicTypesDefaultValues[("system", "char")];
        }

        protected override string ConvertToDecimal(string source)
        {
            string value = source.ToLower();

            if (value.EndsWith("m"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.EndsWith("."))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "decimal")];
            }

            return $"{value}M";
        }

        protected override string ConvertToObject(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return _supportIntrinsicTypesDefaultValues[("system", "object")];
            }

            return Escape(source);
        }

        private static string Escape(string s) => string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
    }
}
