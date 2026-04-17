
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
                [("System", "Double")] = "0R",
                [("System", "Single")] = "0F",
                [("System", "TimeSpan")] = "New Global.System.TimeSpan()",
                [("System", "String")] = "",
                [("System", "Boolean")] = "false",
                [("System", "Byte")] = "CByte(0)",
                [("System", "Int16")] = "0S",
                [("System", "Int32")] = "0I",
                [("System", "Int64")] = "0L",
                [("System", "UInt16")] = "0US",
                [("System", "UInt32")] = "0UI",
                [("System", "UInt64")] = "0UL",
                [("System", "SByte")] = "CSByte(0)",
                [("System", "Char")] = "Chr(0)",
                [("System", "Decimal")] = "0D",
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
            ReadOnlySpan<char> value = source.AsSpan().Trim();

            // special cases

            if (value.Equals("auto", StringComparison.OrdinalIgnoreCase) || value.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return "Double.NaN";
            }

            if (value.Equals("infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("+infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "Double.PositiveInfinity";
            }

            if (value.Equals("-infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "Double.NegativeInfinity";
            }

            if (TryParseLengthWithUnit(value, out string length, out double unitFactor) &&
                double.TryParse(length, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double l))
            {
                l *= unitFactor;
                return $"{l.ToString("R", CultureInfo.InvariantCulture)}R";
            }

            if (value.EndsWith("r", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Slice(0, value.Length - 1);
            }

            if (value.EndsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Slice(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                value = "0";
            }

            return $"{value}R";
        }

        public override string ConvertToSingle(string source)
        {
            ReadOnlySpan<char> value = source.AsSpan().Trim();

            // special cases

            if (value.Equals("auto", StringComparison.OrdinalIgnoreCase) || value.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return "Single.NaN";
            }

            if (value.Equals("infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("+infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "Single.PositiveInfinity";
            }

            if (value.Equals("-infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "Single.NegativeInfinity";
            }

            if (TryParseLengthWithUnit(value, out string length, out double unitFactor) &&
                float.TryParse(length, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float l))
            {
                l = (float)(l * unitFactor);
                return $"{l.ToString("R", CultureInfo.InvariantCulture)}F";
            }

            if (value.EndsWith("f", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Slice(0, value.Length - 1);
            }

            if (value.EndsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Slice(0, value.Length - 1);
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

            if (value.EndsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 1);
            }

            return $"{value}S";
        }

        public override string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int32")];
            }

            if (value.EndsWith("I", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 1);
            }

            return $"{value}I";
        }

        public override string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "int64")];
            }

            if (value.EndsWith("L", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 1);
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

            if (source.EndsWith("US", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 2);
            }

            return $"{value}US";
        }

        public override string ConvertToUInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint32")];
            }

            if (source.EndsWith("UI", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 2);
            }

            return $"{value}UI";
        }

        public override string ConvertToUInt64(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues[("system", "uint64")];
            }

            if (source.EndsWith("UL", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 2);
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
            string value = source.Trim();

            if (value.EndsWith("D", StringComparison.OrdinalIgnoreCase))
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

            return $"{value}D";
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
