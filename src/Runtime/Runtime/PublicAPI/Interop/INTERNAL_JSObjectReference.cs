
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/

using System;
using System.Text.Json;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace CSHTML5.Types
{
    internal sealed class INTERNAL_JSObjectReference : IConvertible, IJavaScriptConvertible, IDisposable
    {
        private string _jsCache;

        string IJavaScriptConvertible.ToJavaScriptString()
        {

            if (IsArray)
            {
                return $"document.jsObjRef[\"{ReferenceId}\"][{_arrayIndex}]";
            }
            else
            {
                return _jsCache ??= $"document.jsObjRef[\"{ReferenceId}\"]";
            }
        }

        private object Value { get; }

        public string ReferenceId { get; private set; }
        public bool IsArray { get; }

        private int _arrayIndex;
        public int ArrayIndex // Note: this property applies only if "IsArray" is true.
        {
            get
            {
                if (!IsArray)
                    System.Diagnostics.Debug.WriteLine("INTERNAL_JSObjectReference error: cannot get index of non-array item");
                return _arrayIndex;
            }
            set
            {
                if (!IsArray)
                    System.Diagnostics.Debug.WriteLine("INTERNAL_JSObjectReference error: cannot set index of non-array item");
                _arrayIndex = value;
            }
        }

        private INTERNAL_JSObjectReference(object value)
        {
            Value = value;
        }

        public INTERNAL_JSObjectReference(object value, string referenceId, string javascript)
        {
            Value = value;
            ReferenceId = referenceId;

            if (OpenSilver.Interop.IsTrackingAllJavascriptObjects)
                JSObjectReferenceHolder.Instance.Add(this, javascript);
        }

        public INTERNAL_JSObjectReference(object value, string referenceId, int arrayIndex, string javascript)
        {
            Value = value;
            ReferenceId = referenceId;
            IsArray = true;
            ArrayIndex = arrayIndex;

            if (OpenSilver.Interop.IsTrackingAllJavascriptObjects)
                JSObjectReferenceHolder.Instance.Add(this, javascript);
        }

        ~INTERNAL_JSObjectReference()
        {
            // Removing itself from JS dict used for C# to JS Interops, otherwise dict keeps growing. Needs more testing
            RemoveFromJS();
        }

        private void RemoveFromJS()
        {
            if (ReferenceId != string.Empty)
            {
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript($"delete document.jsObjRef['{ReferenceId}']");

                if (OpenSilver.Interop.IsTrackingAllJavascriptObjects)
                    JSObjectReferenceHolder.Instance.Remove(this);

                ReferenceId = string.Empty;
            }

        }

        public object GetActualValue()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return GetActualValueSimulator();
            }
            else
            {
                return GetActualValueWasm();
            }
        }

        private object GetActualValueWasm()
        {
            static object GetValueFromJsonElement(JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.Object or JsonValueKind.Array => jsonElement,
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.GetSingle(),
                    JsonValueKind.True or JsonValueKind.False => jsonElement.GetBoolean(),
                    JsonValueKind.Undefined or JsonValueKind.Null or _ => null,
                };
            }

            static object GetValueFromObject(object o)
            {
                if (o is null)
                {
                    return null;
                }

                if (o is string || o.GetType().IsPrimitive)
                {
                    return o;
                }

                return GetValueFromJsonElement((JsonElement)o);
            }

            if (IsArray)
            {
                return Value switch
                {
                    object[] array => GetValueFromObject(array[ArrayIndex]),
                    JsonElement jsonElement => GetValueFromJsonElement(jsonElement.GetProperty(_arrayIndex.ToString())),
                    _ => throw new InvalidOperationException("Value is marked as array but is neither an object[], nor a JsonElement. ReferenceId: " + (ReferenceId ?? "n/a")),
                };
            }
            else
            {
                return GetValueFromObject(Value);
            }
        }

        private object GetActualValueSimulator()
        {
            object result;

            if (IsArray)
            {
                var fullName = Value.GetType().FullName;
                if (Value != null && fullName == "DotNetBrowser.JSArray")
                {
                    result = ((dynamic)Value)[ArrayIndex];
                }
                else if (Value is object[] array)
                {
                    result = array[ArrayIndex];
                }
                else if (Value != null && (fullName == "DotNetBrowser.JSObject"))
                {
                    result = ((dynamic)Value).GetProperty(ArrayIndex.ToString());
                }
                else
                {
                    throw new InvalidOperationException("Value is marked as array but is neither an object[], nor a JSArray, nor a JSObject. ReferenceId: " + (this.ReferenceId ?? "n/a"));
                }
            }
            else
            {
                result = Value;
            }

            if (result == null)
            {
                return null;
            }
            else if (result.GetType().FullName == "DotNetBrowser.JSNumber")
            {
                return ((dynamic)result).GetNumber();
            }
            else if (result.GetType().FullName == "DotNetBrowser.JSBoolean")
            {
                return ((dynamic)result).GetBool();
            }
            else if (result.GetType().FullName == "DotNetBrowser.JSString")
            {
                return ((dynamic)result).GetString();
            }

            return DotNetForHtml5.Core.INTERNAL_Simulator.ConvertBrowserResult(result);
        }

        public bool IsUndefined()
        {
            var actualValue = GetActualValue();
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return actualValue == null || actualValue.GetType().FullName == "DotNetBrowser.JSUndefined";
            }
            else
            {
                return actualValue == null || actualValue.ToString() == "[UNDEFINED]";
            }
        }

        public bool IsNull() => GetActualValue() == null;

        public override string ToString() => ToString(null);

        public void Dispose()
        {
            RemoveFromJS();
            GC.SuppressFinalize(this);
        }

        public static explicit operator string(INTERNAL_JSObjectReference input) => input.ToString(null);
        public static explicit operator bool(INTERNAL_JSObjectReference input) => input.ToBoolean(null);
        public static explicit operator byte(INTERNAL_JSObjectReference input) => input.ToByte(null);
        public static explicit operator DateTime(INTERNAL_JSObjectReference input) => input.ToDateTime(null);
        public static explicit operator decimal(INTERNAL_JSObjectReference input) => input.ToDecimal(null);
        public static explicit operator double(INTERNAL_JSObjectReference input) => input.ToDouble(null);
        public static explicit operator short(INTERNAL_JSObjectReference input) => input.ToInt16(null);
        public static explicit operator int(INTERNAL_JSObjectReference input) => input.ToInt32(null);
        public static explicit operator long(INTERNAL_JSObjectReference input) => input.ToInt64(null);
        public static explicit operator sbyte(INTERNAL_JSObjectReference input) => input.ToSByte(null);
        public static explicit operator float(INTERNAL_JSObjectReference input) => input.ToSingle(null);
        public static explicit operator ushort(INTERNAL_JSObjectReference input) => input.ToUInt16(null);
        public static explicit operator uint(INTERNAL_JSObjectReference input) => input.ToUInt32(null);
        public static explicit operator ulong(INTERNAL_JSObjectReference input) => input.ToUInt64(null);

        public static bool ToBoolean(object value) => Convert.ToBoolean(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static byte ToByte(object value) => Convert.ToByte(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static char ToChar(object value) => Convert.ToChar(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static DateTime ToDateTime(object value) => Convert.ToDateTime(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static decimal ToDecimal(object value) => Convert.ToDecimal(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static double ToDouble(object value) => Convert.ToDouble(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static short ToInt16(object value) => Convert.ToInt16(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static int ToInt32(object value) => Convert.ToInt32(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static long ToInt64(object value) => Convert.ToInt64(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static sbyte ToSByte(object value) => Convert.ToSByte(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static float ToSingle(object value) => Convert.ToSingle(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static ushort ToUInt16(object value) => Convert.ToUInt16(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static uint ToUInt32(object value) => Convert.ToUInt32(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static ulong ToUInt64(object value) => Convert.ToUInt64(new INTERNAL_JSObjectReference(value).GetActualValue());
        public static string ToString(object value) => new INTERNAL_JSObjectReference(value).GetActualValue()?.ToString();
        public static object ToType(Type conversionType, object value) => new INTERNAL_JSObjectReference(value).ToType(conversionType, null);
        public static bool IsUndefined(object value) => new INTERNAL_JSObjectReference(value).IsUndefined();
        public static bool IsNull(object value) => new INTERNAL_JSObjectReference(value).IsNull();

        public TypeCode GetTypeCode() => throw new NotImplementedException();
        public bool ToBoolean(IFormatProvider provider) => Convert.ToBoolean(GetActualValue());
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(GetActualValue());
        public char ToChar(IFormatProvider provider) => Convert.ToChar(GetActualValue());
        public DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(GetActualValue());
        public decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(GetActualValue());
        public double ToDouble(IFormatProvider provider) => Convert.ToDouble(GetActualValue());
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(GetActualValue());
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(GetActualValue());
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(GetActualValue());
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(GetActualValue());
        public float ToSingle(IFormatProvider provider) => Convert.ToSingle(GetActualValue());
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(GetActualValue());
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(GetActualValue());
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(GetActualValue());
        public string ToString(IFormatProvider provider) => GetActualValue()?.ToString();
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(Guid))
            {
                return Guid.Parse(ToString(null));
            }

            throw new NotImplementedException();
        }
    }
}
