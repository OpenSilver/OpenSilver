
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
    internal class SystemTypesHelperVB : SystemTypesHelper
    {
        private const string InvariantCulture = "Global.System.Globalization.CultureInfo.InvariantCulture";
        private const string mscorlib = "mscorlib";

        private static SystemTypesHelperVB systemTypesHelper = new SystemTypesHelperVB();

        private static Dictionary<string, Func<string, string>> SupportedIntrinsicTypes { get; } =
            new Dictionary<string, Func<string, string>>(9)
            {
                ["system.double"] = (s => systemTypesHelper.ConvertToDouble(s)),
                ["system.single"] = (s => systemTypesHelper.ConvertToSingle(s)),
                ["system.timespan"] = (s => systemTypesHelper.ConvertToTimeSpan(s)),
                ["system.string"] = (s => systemTypesHelper.ConvertToString(s)),
                ["system.boolean"] = (s => systemTypesHelper.ConvertToBoolean(s)),
                ["system.byte"] = (s => systemTypesHelper.ConvertToByte(s)),
                ["system.int16"] = (s => systemTypesHelper.ConvertToInt16(s)),
                ["system.int32"] = (s => systemTypesHelper.ConvertToInt32(s)),
                ["system.int64"] = (s => systemTypesHelper.ConvertToInt64(s)),
            };

        private static Dictionary<string, string> SupportIntrinsicTypesDefaultValues { get; } =
            new Dictionary<string, string>(9)
            {
                ["system.double"] = "0D",
                ["system.single"] = "0F",
                ["system.timespan"] = "New Global.System.TimeSpan()",
                ["system.string"] = "",
                ["system.boolean"] = "false",
                ["system.byte"] = "CByte(0)",
                ["system.int16"] = "CShort(0)",
                ["system.int32"] = "0",
                ["system.int64"] = "0L",
            };

        public override bool IsSupportedSystemType(string typeFullName, string assemblyIfAny)
        {
            if (IsMscorlibOrNull(assemblyIfAny))
            {
                return SupportedIntrinsicTypes.ContainsKey(typeFullName.ToLower());
            }
            
            return false;
        }

        public override string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny)
        {
            Debug.Assert(IsMscorlibOrNull(assemblyIfAny));
            Debug.Assert(namespaceName == "System");

            return $"Global.{namespaceName}.{typeName}";
        }

        public override string ConvertFromInvariantString(string source, string typeFullName)
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

        public override string GetDefaultValue(string namespaceName, string typeName, string assemblyIfAny)
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

        internal override string ConvertToDouble(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "Global.System.Double.NaN";

                case "infinity":
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

        internal override string ConvertToSingle(string source)
        {
            string value = source.Trim().ToLower();

            // special cases
            switch (value)
            {
                case "auto":
                case "nan":
                    return "Global.System.Single.NaN";

                case "infinity":
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

        internal override string ConvertToTimeSpan(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.timespan"];
            }

            // Optimization to avoid parsing at runtime
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return $"New Global.System.TimeSpan({timeSpan.Ticks}L)";
            }

            return $"Global.System.TimeSpan.Parse({Escape(value)}, {InvariantCulture})";
        }

        internal override string ConvertToString(string source)
        {
            return Escape(source);
        }

        internal override string ConvertToBoolean(string source)
        {
            string value = source.Trim();
            
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.boolean"];
            }

            return value.ToLower();
        }

        internal override string ConvertToByte(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.byte"];
            }

            return $"CByte({value})";
        }

        internal override string ConvertToInt16(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int16"];
            }

            return $"CShort({value})";
        }

        internal override string ConvertToInt32(string source)
        {
            string value = source.Trim();

            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int32"];
            }

            return value;
        }

        internal override string ConvertToInt64(string source)
        {
            string value = source.Trim();
        
            if (value.Length == 0)
            {
                return SupportIntrinsicTypesDefaultValues["system.int64"];
            }

            return $"{value}L";
        }

        internal override bool IsMscorlibOrNull(string assemblyName)
        {
            if (assemblyName == null || 
                assemblyName.Equals(mscorlib, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        internal override string GetKey(string namespaceName, string typeName)
        {
            return $"{namespaceName}.{typeName}".ToLower();
        }

        internal override string Escape(string s)
        {
            return string.Concat("\"", s.Replace("\"", "\"\""), "\"");
        }
    }
}
