
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
    internal sealed class JSObjectRef : IConvertible, IJavaScriptConvertible, IDisposable
    {
        private static bool IsTrackingAllJavascriptObjects => OpenSilver.Interop.DumpAllJavascriptObjectsEveryMs > 0;

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

        private JSObjectRef(object value)
        {
            Value = value;
            GC.SuppressFinalize(this);
        }

        public JSObjectRef(object value, string referenceId, string javascript)
        {
            Value = value;
            ReferenceId = referenceId;

            if (IsTrackingAllJavascriptObjects)
            {
                JSObjectReferenceHolder.Instance.Add(this, javascript);
            }
        }

        public JSObjectRef(object value, string referenceId, int arrayIndex)
        {
            Value = value;
            ReferenceId = referenceId;
            IsArray = true;
            ArrayIndex = arrayIndex;

            if (IsTrackingAllJavascriptObjects)
            {
                JSObjectReferenceHolder.Instance.Add(this, $"callback {referenceId}");
            }
        }

        ~JSObjectRef()
        {
            // Removing itself from JS dict used for C# to JS Interops, otherwise dict keeps growing. Needs more testing
            RemoveFromJS();
        }

        private void RemoveFromJS()
        {
            if (ReferenceId is not null)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"delete document.jsObjRef['{ReferenceId}']");

                if (IsTrackingAllJavascriptObjects)
                {
                    JSObjectReferenceHolder.Instance.Remove(this);
                }

                ReferenceId = null;
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
                if (Value is string s)
                {
                    result = s;
                }
                else if (Value is object[] array)
                {
                    result = array[ArrayIndex];
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

            return result;
        }

        public bool IsUndefined()
        {
            var actualValue = GetActualValue();
            return actualValue == null || actualValue.ToString() == "[UNDEFINED]";
        }

        public bool IsNull() => GetActualValue() == null;

        public override string ToString() => ToString(null);

        public void Dispose()
        {
            RemoveFromJS();
            GC.SuppressFinalize(this);
        }

        public static explicit operator string(JSObjectRef input) => input.ToString(null);
        public static explicit operator bool(JSObjectRef input) => input.ToBoolean(null);
        public static explicit operator byte(JSObjectRef input) => input.ToByte(null);
        public static explicit operator DateTime(JSObjectRef input) => input.ToDateTime(null);
        public static explicit operator decimal(JSObjectRef input) => input.ToDecimal(null);
        public static explicit operator double(JSObjectRef input) => input.ToDouble(null);
        public static explicit operator short(JSObjectRef input) => input.ToInt16(null);
        public static explicit operator int(JSObjectRef input) => input.ToInt32(null);
        public static explicit operator long(JSObjectRef input) => input.ToInt64(null);
        public static explicit operator sbyte(JSObjectRef input) => input.ToSByte(null);
        public static explicit operator float(JSObjectRef input) => input.ToSingle(null);
        public static explicit operator ushort(JSObjectRef input) => input.ToUInt16(null);
        public static explicit operator uint(JSObjectRef input) => input.ToUInt32(null);
        public static explicit operator ulong(JSObjectRef input) => input.ToUInt64(null);

        public static bool ToBoolean(object value) => Convert.ToBoolean(new JSObjectRef(value).GetActualValue());
        public static byte ToByte(object value) => Convert.ToByte(new JSObjectRef(value).GetActualValue());
        public static char ToChar(object value) => Convert.ToChar(new JSObjectRef(value).GetActualValue());
        public static DateTime ToDateTime(object value) => Convert.ToDateTime(new JSObjectRef(value).GetActualValue());
        public static decimal ToDecimal(object value) => Convert.ToDecimal(new JSObjectRef(value).GetActualValue());
        public static double ToDouble(object value) => Convert.ToDouble(new JSObjectRef(value).GetActualValue());
        public static short ToInt16(object value) => Convert.ToInt16(new JSObjectRef(value).GetActualValue());
        public static int ToInt32(object value) => Convert.ToInt32(new JSObjectRef(value).GetActualValue());
        public static long ToInt64(object value) => Convert.ToInt64(new JSObjectRef(value).GetActualValue());
        public static sbyte ToSByte(object value) => Convert.ToSByte(new JSObjectRef(value).GetActualValue());
        public static float ToSingle(object value) => Convert.ToSingle(new JSObjectRef(value).GetActualValue());
        public static ushort ToUInt16(object value) => Convert.ToUInt16(new JSObjectRef(value).GetActualValue());
        public static uint ToUInt32(object value) => Convert.ToUInt32(new JSObjectRef(value).GetActualValue());
        public static ulong ToUInt64(object value) => Convert.ToUInt64(new JSObjectRef(value).GetActualValue());
        public static string ToString(object value) => new JSObjectRef(value).GetActualValue()?.ToString();
        public static object ToType(Type conversionType, object value) => new JSObjectRef(value).ToType(conversionType, null);
        public static bool IsUndefined(object value) => new JSObjectRef(value).IsUndefined();
        public static bool IsNull(object value) => new JSObjectRef(value).IsNull();

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
