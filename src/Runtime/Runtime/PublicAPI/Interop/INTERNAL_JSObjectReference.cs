

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


#if BRIDGE
using Bridge;
using DotNetBrowser;
#else
using JSIL.Meta;
#endif

using System;


namespace CSHTML5.Types
{
#if BRIDGE
    [External] //we exclude this class
#else
    [JSIgnore]
#endif
    internal class INTERNAL_JSObjectReference : IConvertible
    {
        public object Value { get; set; }
        public string ReferenceId { get; set; }
        public bool IsArray { get; set; }

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

#if BRIDGE
        [External] //we exclude this method
#else
        [JSReplacement("null")]
#endif
        public object GetActualValue()
        {
#if !BUILDINGDOCUMENTATION // We don't have the references to the "DotNetBrowser" web browser control when building the documentation.
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
                else if(Value != null && (fullName == "DotNetBrowser.JSObject" || fullName == "System.Text.Json.JsonElement"))
                {
                    result = ((dynamic)Value).GetProperty(ArrayIndex.ToString());
                }
                else
                {
                    throw new InvalidOperationException("Value is marked as array but is neither an object[], nor a JSArray, nor a JSObject, nor a JsonElement. ReferenceId: " + (this.ReferenceId ?? "n/a"));
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
            else
            {
                return result;
            }
#endif
        }

        public bool IsUndefined()
        {
            var actualValue = GetActualValue();
#if CSHTML5NETSTANDARD
            if (Interop.IsRunningInTheSimulator_WorkAround)
                return (actualValue == null || actualValue.GetType().FullName == "DotNetBrowser.JSUndefined");
            else
                return (actualValue == null);
#else
            if (actualValue ==  null || !(actualValue is JSValue))
                return false;
            return ((JSValue)actualValue).IsUndefined();
#endif
        }

        public bool IsNull()
        {
            var actualValue = GetActualValue();
#if CSHTML5NETSTANDARD
            return actualValue == null;
#else
            if (actualValue == null)
                return true;
            if (!(actualValue is JSValue))
                return false;
            return ((JSValue)actualValue).IsNull();
#endif
        }


        //  Note: in the methods below, we use "Convert.*" rather than  casting, in order to prevent issues related to unboxing values. cf. http://stackoverflow.com/questions/4113056/whats-wrong-with-casting-0-0-to-double

        public static explicit operator string(INTERNAL_JSObjectReference input)
        {
            return input.GetActualValue().ToString();
        }

        public static explicit operator bool(INTERNAL_JSObjectReference input)
        {
            return Convert.ToBoolean(input.GetActualValue());
        }

        public static explicit operator byte(INTERNAL_JSObjectReference input)
        {
            return Convert.ToByte(input.GetActualValue());
        }

        public static explicit operator DateTime(INTERNAL_JSObjectReference input)
        {
            return Convert.ToDateTime(input.GetActualValue());
        }

        public static explicit operator decimal(INTERNAL_JSObjectReference input)
        {
            return Convert.ToDecimal(input.GetActualValue());
        }


        public static explicit operator double(INTERNAL_JSObjectReference input)
        {
            return Convert.ToDouble(input.GetActualValue());
        }

        public static explicit operator short(INTERNAL_JSObjectReference input)
        {
            return Convert.ToInt16(input.GetActualValue());
        }

        public static explicit operator int(INTERNAL_JSObjectReference input)
        {
            return Convert.ToInt32(input.GetActualValue());
        }

        public static explicit operator long(INTERNAL_JSObjectReference input)
        {
            return Convert.ToInt64(input.GetActualValue());
        }

        public static explicit operator sbyte(INTERNAL_JSObjectReference input)
        {
            return Convert.ToSByte(input.GetActualValue());
        }

        public static explicit operator float(INTERNAL_JSObjectReference input)
        {
            return Convert.ToSingle(input.GetActualValue());
        }

        public static explicit operator ushort(INTERNAL_JSObjectReference input)
        {
            return Convert.ToUInt16(input.GetActualValue());
        }

        public static explicit operator uint(INTERNAL_JSObjectReference input)
        {
            return Convert.ToUInt32(input.GetActualValue());
        }

        public static explicit operator ulong(INTERNAL_JSObjectReference input)
        {
            return Convert.ToUInt64(input.GetActualValue());
        }


        public override string ToString()
        {
            object actualValue = this.GetActualValue();
            return (actualValue != null ? actualValue.ToString() : null);
        }

#region IConvertible implementation

        //  Note: in the methods below, we use "Convert.*" rather than  casting, in order to prevent issues related to unboxing values. cf. http://stackoverflow.com/questions/4113056/whats-wrong-with-casting-0-0-to-double

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.GetActualValue());
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.GetActualValue());
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.GetActualValue());
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.GetActualValue());
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.GetActualValue());
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.GetActualValue());
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.GetActualValue());
        }

        public int ToInt32(IFormatProvider provider)
        {
            int.TryParse(GetActualValue().ToString(), out var result);

            return result;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.GetActualValue());
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.GetActualValue());
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.GetActualValue());
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.GetActualValue());
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.GetActualValue());
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.GetActualValue());
        }

        public string ToString(IFormatProvider provider)
        {
            object actualValue = this.GetActualValue();
            return (actualValue != null ? actualValue.ToString() : null);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
#endregion
    }
}
