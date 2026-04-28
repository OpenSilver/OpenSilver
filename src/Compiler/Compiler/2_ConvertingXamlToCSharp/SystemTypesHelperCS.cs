
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

        private static readonly Dictionary<string, string> _supportIntrinsicTypesDefaultValues =
            new(16, StringComparer.OrdinalIgnoreCase)
            {
                ["System.Double"] = "0D",
                ["System.Single"] = "0F",
                ["System.TimeSpan"] = "new global::System.TimeSpan()",
                ["System.String"] = "\"\"",
                ["System.Boolean"] = "false",
                ["System.Byte"] = "(byte)0",
                ["System.Int16"] = "(short)0",
                ["System.Int32"] = "0",
                ["System.Int64"] = "0L",
                ["System.UInt16"] = "(ushort)0",
                ["System.UInt32"] = "0U",
                ["System.UInt64"] = "0UL",
                ["System.SByte"] = "(sbyte)0",
                ["System.Char"] = "(char)0",
                ["System.Decimal"] = "0M",
                ["System.Object"] = "\"\"",
            };

        public override bool IsNullableType(string fullTypeName, string assembly, out string underlyingType)
        {
            const string Nullable = "System.Nullable<";

            if (fullTypeName.StartsWith(Nullable) && IsCoreLibraryOrNull(assembly))
            {
                int startIndex = Nullable.Length + "global::".Length;

                underlyingType = fullTypeName.Substring(startIndex, fullTypeName.Length - startIndex - 1);
                return true;
            }

            underlyingType = null;
            return false;
        }

        public override string GetDefaultValue(string fullTypeName)
        {
            if (_supportIntrinsicTypesDefaultValues.TryGetValue(fullTypeName, out string value))
            {
                return value;
            }

            return null;
        }

        public override string ConvertToDouble(string source)
        {
            ReadOnlySpan<char> value = source.AsSpan().Trim();

            // special cases

            if (value.Equals("auto", StringComparison.OrdinalIgnoreCase) || value.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return "double.NaN";
            }

            if (value.Equals("infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("+infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "double.PositiveInfinity";
            }

            if (value.Equals("-infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "double.NegativeInfinity";
            }

            if (TryParseLengthWithUnit(value, out string length, out double unitFactor) &&
                double.TryParse(length, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double l))
            {
                l *= unitFactor;
                return $"{l.ToString("R", CultureInfo.InvariantCulture)}D";
            }

            if (value.EndsWith("d", StringComparison.OrdinalIgnoreCase))
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

            return $"{value}D";
        }

        public override string ConvertToSingle(string source)
        {
            ReadOnlySpan<char> value = source.AsSpan().Trim();

            // special cases

            if (value.Equals("auto", StringComparison.OrdinalIgnoreCase) || value.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return "float.NaN";
            }

            if (value.Equals("infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("+infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "float.PositiveInfinity";
            }

            if (value.Equals("-infinity", StringComparison.OrdinalIgnoreCase))
            {
                return "float.NegativeInfinity";
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
                return _supportIntrinsicTypesDefaultValues["System.TimeSpan"];
            }

            // Optimization to avoid parsing at runtime
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return $"new global::System.TimeSpan({timeSpan.Ticks}L)";
            }

            return $"global::System.TimeSpan.Parse({Escape(value)}, {InvariantCulture})";
        }

        public override string ConvertToString(string source)
        {
            return Escape(source);
        }

        public override string ConvertToBoolean(string source)
        {
            string value = source.Trim();
            
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Boolean"];
            }

            return value.ToLower();
        }

        public override string ConvertToByte(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Byte"];
            }

            return $"(byte){value}";
        }

        public override string ConvertToInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Int16"];
            }

            return $"(short){value}";
        }

        public override string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Int32"];
            }

            return value;
        }

        public override string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Int64"];
            }

            return $"{value}L";
        }

        public override string ConvertToUInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.UInt16"];
            }

            return $"(ushort){value}";
        }

        public override string ConvertToUInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.UInt32"];
            }

            return $"{value}U";
        }

        public override string ConvertToUInt64(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.UInt64"];
            }

            return $"{value}UL";
        }

        public override string ConvertToSByte(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.SByte"];
            }

            return $"(sbyte){value}";
        }

        public override string ConvertToChar(string source)
        {
            if (source.Length == 1)
            {
                return $"'{source}'";
            }

            return _supportIntrinsicTypesDefaultValues["System.Char"];
        }

        public override string ConvertToDecimal(string source)
        {
            string value = source.Trim();

            if (value.EndsWith("M", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.EndsWith("."))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                return _supportIntrinsicTypesDefaultValues["System.Decimal"];
            }

            return $"{value}M";
        }

        public override string ConvertToObject(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return _supportIntrinsicTypesDefaultValues["System.Object"];
            }

            return Escape(source);
        }

        private static string Escape(string s) => string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
    }
}
