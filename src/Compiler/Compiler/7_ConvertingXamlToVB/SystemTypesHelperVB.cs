
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
    internal sealed class SystemTypesHelperVB : SystemTypesHelper
    {
        private const string InvariantCulture = "Global.System.Globalization.CultureInfo.InvariantCulture";

        private static readonly Dictionary<(string Namespace, string Type), string> _supportIntrinsicTypesDefaultValues =
            new(16, StringTupleComparer.Instance)
            {
                [("System", "Double")] = "0D",
                [("System", "Single")] = "0F",
                [("System", "TimeSpan")] = "New Global.System.TimeSpan()",
                [("System", "String")] = "",
                [("System", "Boolean")] = "false",
                [("System", "Byte")] = "CByte(0)",
                [("System", "Int16")] = "CShort(0)",
                [("System", "Int32")] = "0",
                [("System", "Int64")] = "0L",
                [("System", "UInt16")] = "CUShort(0)",
                [("System", "UInt32")] = "CUInt(0)",
                [("System", "UInt64")] = "0UL",
                [("System", "SByte")] = "CSByte(0)",
                [("System", "Char")] = "Chr(0)",
                [("System", "Decimal")] = "CDec(0)",
                [("System", "Object")] = "\"\"",
            };

        public override bool IsNullableType(string fullTypeName, string assembly, out string underlyingType)
        {
            const string Nullable = "System.Nullable(Of ";

            if (fullTypeName.StartsWith(Nullable) && IsCoreLibraryOrNull(assembly))
            {
                int startIndex = Nullable.Length + "Global.".Length;

                underlyingType = fullTypeName.Substring(startIndex, fullTypeName.Length - startIndex - 1);
                return true;
            }

            underlyingType = null;
            return false;
        }

        public override string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny)
        {
            Debug.Assert(IsCoreLibraryOrNull(assemblyIfAny));
            Debug.Assert(namespaceName == "System");

            return $"Global.{namespaceName}.{typeName}";
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

        public override string ConvertToDouble(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "Global.System.Double.NaN";

                case "infinity":
                case "+infinity":
                    return "Global.System.Double.PositiveInfinity";

                case "-infinity":
                    return "Global.System.Double.NegativeInfinity";
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

        public override string ConvertToSingle(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "Global.System.Single.NaN";

                case "infinity":
                case "+infinity":
                    return "Global.System.Single.PositiveInfinity";

                case "-infinity":
                    return "Global.System.Single.NegativeInfinity";                    
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

        public override string ConvertToTimeSpan(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "timespan")];
            }

            // Optimization to avoid parsing at runtime
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return $"New Global.System.TimeSpan({timeSpan.Ticks}L)";
            }

            return $"Global.System.TimeSpan.Parse({Escape(value)}, {InvariantCulture})";
        }

        public override string ConvertToString(string source) => Escape(source);

        public override string ConvertToBoolean(string source)
        {
            string value = source.Trim();
            
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "boolean")];
            }

            return value.ToLower();
        }

        public override string ConvertToByte(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "byte")];
            }

            return $"CByte({value})";
        }

        public override string ConvertToInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int16")];
            }

            return $"CShort({value})";
        }

        public override string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int32")];
            }

            return value;
        }

        public override string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int64")];
            }

            return $"{value}L";
        }

        public override string ConvertToUInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint16")];
            }

            return $"CUShort({value})";
        }

        public override string ConvertToUInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint32")];
            }

            return $"CUInt({value})";
        }

        public override string ConvertToUInt64(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint64")];
            }

            return $"{value}UL";
        }

        public override string ConvertToSByte(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "sbyte")];
            }

            return $"CSByte({value})";
        }

        public override string ConvertToChar(string source)
        {
            if (source.Length == 1)
            {
                return $"\"{source}\"c";
            }

            return _supportIntrinsicTypesDefaultValues[("system", "char")];
        }

        public override string ConvertToDecimal(string source)
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

            return $"CDec({value})";
        }

        public override string ConvertToObject(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return _supportIntrinsicTypesDefaultValues[("system", "object")];
            }

            return Escape(source);
        }

        private static string Escape(string s) => string.Concat("\"", s.Replace("\"", "\"\""), "\"");
    }
}
