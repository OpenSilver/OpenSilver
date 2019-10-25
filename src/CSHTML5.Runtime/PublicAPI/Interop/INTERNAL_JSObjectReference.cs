
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if BRIDGE
using Bridge;
#else
using JSIL.Meta;
#endif

#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD// We don't have the references to the "DotNetBrowser" web browser control when building the documentation.
//using DotNetBrowser;
#endif
using DotNetBrowser;

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
        public int ArrayIndex { get; set; } // Note: this property applies only if "IsArray" is true.

#if BRIDGE
        [External] //we exclude this method
#else
        [JSReplacement("null")]
#endif
        public object GetActualValue()
        {
            object result = null;

#if !BUILDINGDOCUMENTATION // We don't have the references to the "DotNetBrowser" web browser control when building the documentation.
            if (this.IsArray)
            {
#if CSHTML5NETSTANDARD
                result = ((object[])this.Value)[this.ArrayIndex];
#else
                result = ((JSArray)this.Value)[this.ArrayIndex];
#endif
            }
            else
            {
                result = this.Value;
            }

#if !CSHTML5NETSTANDARD
            if (result is JSNumber)
            {
                result = ((JSNumber)result).GetNumber(); // This prevents the "InvalidCastException" with message "Unable to cast object of type 'DotNetBrowser.JSNumber' to type 'System.IConvertible'" that happened in the method "ToDouble" below.
            }
            else if (result is JSBoolean)
            {
                result = ((JSBoolean)result).GetBool();
            }
            else if (result is JSString)
            {
                result = ((JSString)result).GetString();
            }
#endif
#endif

            return result;
        }

        public bool IsUndefined()
        {
#if CSHTML5NETSTANDARD
           return Value == null;
#else
            if (Value ==  null || !(Value is JSValue))
                return false;
            return ((JSValue)Value).IsUndefined();
#endif
        }

        public bool IsNull()
        {

#if CSHTML5NETSTANDARD
           return Value == null;
#else
            if (Value == null)
                return true;
            if (!(Value is JSValue))
                return false;
            return ((JSValue)Value).IsNull();
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
            return Convert.ToInt32(this.GetActualValue());
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
