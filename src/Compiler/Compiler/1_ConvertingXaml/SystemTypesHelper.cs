
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

namespace OpenSilver.Compiler;

internal abstract class SystemTypesHelper
{
    private const string mscorlib = "mscorlib";
    private const string system_runtime = "System.Runtime";
    private const string system_private_corelib = "System.Private.CoreLib";
    private const string netstandard = "netstandard";

    private readonly Dictionary<string, Func<string, string>> _knownIntrinsicTypes;

    public static SystemTypesHelper CSharp { get; } = new SystemTypesHelperCS();
    public static SystemTypesHelper VisualBasic { get; } = new SystemTypesHelperVB();
    public static SystemTypesHelper FSharp { get; } = new SystemTypesHelperFS();

    protected SystemTypesHelper()
    {
        _knownIntrinsicTypes = new(16, StringComparer.OrdinalIgnoreCase)
        {
            ["System.Double"] = ConvertToDouble,
            ["System.Single"] = ConvertToSingle,
            ["System.TimeSpan"] = ConvertToTimeSpan,
            ["System.String"] = ConvertToString,
            ["System.Boolean"] = ConvertToBoolean,
            ["System.Byte"] = ConvertToByte,
            ["System.Int16"] = ConvertToInt16,
            ["System.Int32"] = ConvertToInt32,
            ["System.Int64"] = ConvertToInt64,
            ["System.UInt16"] = ConvertToUInt16,
            ["System.UInt32"] = ConvertToUInt32,
            ["System.UInt64"] = ConvertToUInt64,
            ["System.SByte"] = ConvertToSByte,
            ["System.Char"] = ConvertToChar,
            ["System.Decimal"] = ConvertToDecimal,
            ["System.Object"] = ConvertToObject,
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

    public bool IsKnownType(string typeFullName, string assemblyIfAny)
    {
        if (IsCoreLibraryOrNull(assemblyIfAny))
        {
            return _knownIntrinsicTypes.ContainsKey(typeFullName);
        }

        return false;
    }

    public string ConvertKnownType(string source, string typeFullName)
    {
        if (_knownIntrinsicTypes.TryGetValue(typeFullName, out var converter))
        {
            Debug.Assert(converter != null);
            return converter(source);
        }

        throw new InvalidOperationException($"'{typeFullName}' is not a supported system type.");
    }

    public abstract bool IsNullableType(string fullTypeName, string assembly, out string underlyingType);

    public abstract string GetDefaultValue(string fullTypeName);

    public abstract string ConvertToDouble(string source);

    public abstract string ConvertToSingle(string source);

    public abstract string ConvertToTimeSpan(string source);

    public abstract string ConvertToString(string source);

    public abstract string ConvertToBoolean(string source);

    public abstract string ConvertToByte(string source);

    public abstract string ConvertToInt16(string source);

    public abstract string ConvertToInt32(string source);

    public abstract string ConvertToInt64(string source);

    public abstract string ConvertToUInt16(string source);

    public abstract string ConvertToUInt32(string source);

    public abstract string ConvertToUInt64(string source);

    public abstract string ConvertToSByte(string source);

    public abstract string ConvertToChar(string source);

    public abstract string ConvertToDecimal(string source);

    public abstract string ConvertToObject(string source);

    internal static bool TryParseLengthWithUnit(ReadOnlySpan<char> value, out string length, out double unitFactor)
    {
        if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            length = value.Slice(0, value.Length - 2).ToString();
            unitFactor = 1.0;
            return true;
        }
        else if (value.EndsWith("in", StringComparison.OrdinalIgnoreCase))
        {
            length = value.Slice(0, value.Length - 2).ToString();
            unitFactor = 96.0;
            return true;
        }
        else if (value.EndsWith("cm", StringComparison.OrdinalIgnoreCase))
        {
            length = value.Slice(0, value.Length - 2).ToString();
            unitFactor = 96.0 / 2.54;
            return true;
        }
        else if (value.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
        {
            length = value.Slice(0, value.Length - 2).ToString();
            unitFactor = 96.0 / 72.0;
            return true;
        }
        else
        {
            length = default;
            unitFactor = 0;
            return false;
        }
    }

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
