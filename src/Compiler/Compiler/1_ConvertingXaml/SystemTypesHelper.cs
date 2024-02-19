
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

namespace OpenSilver.Compiler
{
    internal abstract class SystemTypesHelper
    {
        private const string mscorlib = "mscorlib";
        private const string system_runtime = "System.Runtime";
        private const string system_private_corelib = "System.Private.CoreLib";
        private const string netstandard = "netstandard";

        private readonly Dictionary<string, Func<string, string>> _supportedIntrinsicTypes;

        public static SystemTypesHelper CSharp { get; } = new SystemTypesHelperCS();
        public static SystemTypesHelper VisualBasic { get; } = new SystemTypesHelperVB();
        public static SystemTypesHelper FSharp { get; } = new SystemTypesHelperFS();

        protected SystemTypesHelper()
        {
            _supportedIntrinsicTypes = new(16, StringComparer.OrdinalIgnoreCase)
            {
                ["system.double"] = s => ConvertToDouble(s),
                ["system.single"] = s => ConvertToSingle(s),
                ["system.timespan"] = s => ConvertToTimeSpan(s),
                ["system.string"] = s => ConvertToString(s),
                ["system.boolean"] = s => ConvertToBoolean(s),
                ["system.byte"] = s => ConvertToByte(s),
                ["system.int16"] = s => ConvertToInt16(s),
                ["system.int32"] = s => ConvertToInt32(s),
                ["system.int64"] = s => ConvertToInt64(s),
                ["system.uint16"] = s => ConvertToUInt16(s),
                ["system.uint32"] = s => ConvertToUInt32(s),
                ["system.uint64"] = s => ConvertToUInt64(s),
                ["system.sbyte"] = s => ConvertToSByte(s),
                ["system.char"] = s => ConvertToChar(s),
                ["system.decimal"] = s => ConvertToDecimal(s),
                ["system.object"] = s => ConvertToObject(s),
            };
        }

        public static bool IsCoreLibraryOrNull(string assemblyName)
        {
            return assemblyName == null ||
                   assemblyName.Equals(mscorlib, StringComparison.OrdinalIgnoreCase) ||
                   assemblyName.Equals(system_runtime, StringComparison.OrdinalIgnoreCase) ||
                   assemblyName.Equals(system_private_corelib, StringComparison.OrdinalIgnoreCase) ||
                   assemblyName.Equals(netstandard, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSupportedSystemType(string typeFullName, string assemblyIfAny)
        {
            if (IsCoreLibraryOrNull(assemblyIfAny))
            {
                return _supportedIntrinsicTypes.ContainsKey(typeFullName);
            }

            return false;
        }

        public string ConvertFromInvariantString(string source, string typeFullName)
        {
            if (_supportedIntrinsicTypes.TryGetValue(typeFullName, out var converter))
            {
                Debug.Assert(converter != null);
                return converter(source);
            }

            throw new InvalidOperationException(
                $"'{typeFullName}' is not a supported system type."
            );
        }

        public abstract bool IsNullableType(string fullTypeName, string assembly, out string underlyingType);

        public abstract string GetDefaultValue(string namespaceName, string typeName, string assemblyIfAny);

        public abstract string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny);

        protected abstract string ConvertToDouble(string source);

        protected abstract string ConvertToSingle(string source);

        protected abstract string ConvertToTimeSpan(string source);

        protected abstract string ConvertToString(string source);

        protected abstract string ConvertToBoolean(string source);

        protected abstract string ConvertToByte(string source);

        protected abstract string ConvertToInt16(string source);

        protected abstract string ConvertToInt32(string source);

        protected abstract string ConvertToInt64(string source);

        protected abstract string ConvertToUInt16(string source);

        protected abstract string ConvertToUInt32(string source);

        protected abstract string ConvertToUInt64(string source);

        protected abstract string ConvertToSByte(string source);

        protected abstract string ConvertToChar(string source);

        protected abstract string ConvertToDecimal(string source);

        protected abstract string ConvertToObject(string source);

        internal sealed class StringTupleComparer : IEqualityComparer<(string Namespace, string Type)>
        {
            public static StringTupleComparer Instance { get; } = new();

            public bool Equals((string Namespace, string Type) x, (string Namespace, string Type) y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x.Namespace, y.Namespace) &&
                       StringComparer.OrdinalIgnoreCase.Equals(x.Type, y.Type);
            }

            public int GetHashCode((string Namespace, string Type) obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Namespace) ^
                       StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Type);
            }
        }
    }
}
