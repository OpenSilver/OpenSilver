

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


using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScriptDefinitionsSupport
{
    public interface IJSObject
    {
        object UnderlyingJSInstance { get; set; }
    }

    public class JSObject : IJSObject
    {
        public static readonly JSObject Undefined = new JSObject(OpenSilver.Interop.ExecuteJavaScript("undefined"));
        public static readonly JSObject Null = new JSObject(OpenSilver.Interop.ExecuteJavaScript("null"));

        public object UnderlyingJSInstance { get; set; }

        public JSObject()
        {
            // Use new Object() instead of {} syntax, because the latter
            // is interpreted as undefined in the Simulator
            this.UnderlyingJSInstance = OpenSilver.Interop.ExecuteJavaScript("new Object()");
        }

        public JSObject(object jsObj)
        {
            this.UnderlyingJSInstance = jsObj;
        }

        static public T FromJavaScriptInstance<T>(object jsInstance) where T : IJSObject, new()
        {
            var t = new T();
            t.UnderlyingJSInstance = jsInstance;
            return t;
        }

        // ----------------------
        // Cast operator from standard C# types
        // ----------------------

        static public implicit operator JSObject(Boolean b)
        {
            return new JSObject(b);
        }

        static public implicit operator JSObject(Byte b)
        {
            return new JSObject(b);
        }

        static public implicit operator JSObject(SByte s)
        {
            return new JSObject(s);
        }

        static public implicit operator JSObject(Char c)
        {
            return new JSObject(c);
        }

        static public implicit operator JSObject(Decimal d)
        {
            return new JSObject(d);
        }

        static public implicit operator JSObject(Double d)
        {
            return new JSObject(d);
        }

        static public implicit operator JSObject(Single f)
        {
            return new JSObject(f);
        }

        static public implicit operator JSObject(Int32 i)
        {
            return new JSObject(i);
        }

        static public implicit operator JSObject(UInt32 u)
        {
            return new JSObject(u);
        }

        static public implicit operator JSObject(Int64 l)
        {
            return new JSObject(l);
        }

        static public implicit operator JSObject(UInt64 u)
        {
            return new JSObject(u);
        }

        static public implicit operator JSObject(Int16 s)
        {
            return new JSObject(s);
        }

        static public implicit operator JSObject(UInt16 u)
        {
            return new JSObject(u);
        }

        static public implicit operator JSObject(String s)
        {
            return new JSObject(s);
        }

        static public JSObject CreateFrom(object o)
        {
            if (o is JSObject)
                return (JSObject)o;
            else if (o is IJSObject)
                return new JSObject(((IJSObject)o).UnderlyingJSInstance);
            else
                return new JSObject(o);
        }

        public override bool Equals(object obj)
        {
            if (obj is IJSObject)
                return ((IJSObject)obj).UnderlyingJSInstance == this.UnderlyingJSInstance;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool Helper_IsBuiltInType<T>()
        {
            return typeof(T) == typeof(Int32) ||
                   typeof(T) == typeof(Single) ||
                   typeof(T) == typeof(Double) ||
                   typeof(T) == typeof(String) ||
                   typeof(T) == typeof(Boolean) ||
                   typeof(T) == typeof(UInt32) ||
                   typeof(T) == typeof(Int16) ||
                   typeof(T) == typeof(UInt16) ||
                   typeof(T) == typeof(Int64) ||
                   typeof(T) == typeof(UInt64) ||
                   typeof(T) == typeof(Decimal) ||
                   typeof(T) == typeof(Byte) ||
                   typeof(T) == typeof(SByte);
        }

        public static T Helper_ConvertTo<T>(object jsObj)
        {
            if (typeof(T) == typeof(Int32))
                return (T)(object)Convert.ToInt32(jsObj);
            else if (typeof(T) == typeof(Single))
                return (T)(object)Convert.ToSingle(jsObj);
            else if (typeof(T) == typeof(Double))
                return (T)(object)Convert.ToDouble(jsObj);
            else if (typeof(T) == typeof(String))
                return (T)(object)Convert.ToString(jsObj);
            else if (typeof(T) == typeof(Boolean))
                return (T)(object)Convert.ToBoolean(jsObj);
            else if (typeof(T) == typeof(UInt32))
                return (T)(object)Convert.ToUInt32(jsObj);
            else if (typeof(T) == typeof(Int16))
                return (T)(object)Convert.ToInt16(jsObj);
            else if (typeof(T) == typeof(UInt16))
                return (T)(object)Convert.ToUInt16(jsObj);
            else if (typeof(T) == typeof(Int64))
                return (T)(object)Convert.ToInt64(jsObj);
            else if (typeof(T) == typeof(UInt64))
                return (T)(object)Convert.ToUInt64(jsObj);
            else if (typeof(T) == typeof(Decimal))
                return (T)(object)Convert.ToDecimal(jsObj);
            else if (typeof(T) == typeof(Byte))
                return (T)(object)Convert.ToByte(jsObj);
            else if (typeof(T) == typeof(SByte))
                return (T)(object)Convert.ToSByte(jsObj);
            else
                throw new Exception("The type '" + typeof(T).Name + "' is not a built-in type");
        }
    }
}
