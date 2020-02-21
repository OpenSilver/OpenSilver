
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



using CSHTML5;
using System;

namespace TypeScriptDefinitionsSupport
{
    public class UnionType<T0, T1, T2, T3, T4> : UnionType
    {
        private UnionType(object jsObj)
        {
            this.UnderlyingJSInstance = jsObj;
        }

        public static UnionType<T0, T1, T2, T3, T4> FromJavaScriptInstance(object jsObj)
        {
            return new UnionType<T0, T1, T2, T3, T4>(jsObj);
        }
        private T0 _t0 { get; set; }

        public UnionType(T0 t)
        {
            this._t0 = t;
            this._Type = typeof(T0);
            JSObject o = JSObject.CreateFrom(t);;
            this.UnderlyingJSInstance = o.UnderlyingJSInstance;
        }

        static public implicit operator UnionType<T0, T1, T2, T3, T4>(T0 t)
        {
            return new UnionType<T0, T1, T2, T3, T4>(t);
        }

        static public implicit operator T0(UnionType<T0, T1, T2, T3, T4> value)
        {
            if (value._Type == null)
            {
                value._t0 = value.CreateInstance<T0>();
                value._Type = typeof(T0);
                return value._t0;
            }
            if (value._Type == typeof(T0))
                return value._t0;
            else
                throw new Exception("Unable to cast this UnionType to " + typeof(T0).Name + " because it contains a value that is of another type.");
        }
        private T1 _t1 { get; set; }

        public UnionType(T1 t)
        {
            this._t1 = t;
            this._Type = typeof(T1);
            JSObject o = JSObject.CreateFrom(t);;
            this.UnderlyingJSInstance = o.UnderlyingJSInstance;
        }

        static public implicit operator UnionType<T0, T1, T2, T3, T4>(T1 t)
        {
            return new UnionType<T0, T1, T2, T3, T4>(t);
        }

        static public implicit operator T1(UnionType<T0, T1, T2, T3, T4> value)
        {
            if (value._Type == null)
            {
                value._t1 = value.CreateInstance<T1>();
                value._Type = typeof(T1);
                return value._t1;
            }
            if (value._Type == typeof(T1))
                return value._t1;
            else
                throw new Exception("Unable to cast this UnionType to " + typeof(T1).Name + " because it contains a value that is of another type.");
        }
        private T2 _t2 { get; set; }

        public UnionType(T2 t)
        {
            this._t2 = t;
            this._Type = typeof(T2);
            JSObject o = JSObject.CreateFrom(t);;
            this.UnderlyingJSInstance = o.UnderlyingJSInstance;
        }

        static public implicit operator UnionType<T0, T1, T2, T3, T4>(T2 t)
        {
            return new UnionType<T0, T1, T2, T3, T4>(t);
        }

        static public implicit operator T2(UnionType<T0, T1, T2, T3, T4> value)
        {
            if (value._Type == null)
            {
                value._t2 = value.CreateInstance<T2>();
                value._Type = typeof(T2);
                return value._t2;
            }
            if (value._Type == typeof(T2))
                return value._t2;
            else
                throw new Exception("Unable to cast this UnionType to " + typeof(T2).Name + " because it contains a value that is of another type.");
        }
        private T3 _t3 { get; set; }

        public UnionType(T3 t)
        {
            this._t3 = t;
            this._Type = typeof(T3);
            JSObject o = JSObject.CreateFrom(t);;
            this.UnderlyingJSInstance = o.UnderlyingJSInstance;
        }

        static public implicit operator UnionType<T0, T1, T2, T3, T4>(T3 t)
        {
            return new UnionType<T0, T1, T2, T3, T4>(t);
        }

        static public implicit operator T3(UnionType<T0, T1, T2, T3, T4> value)
        {
            if (value._Type == null)
            {
                value._t3 = value.CreateInstance<T3>();
                value._Type = typeof(T3);
                return value._t3;
            }
            if (value._Type == typeof(T3))
                return value._t3;
            else
                throw new Exception("Unable to cast this UnionType to " + typeof(T3).Name + " because it contains a value that is of another type.");
        }
        private T4 _t4 { get; set; }

        public UnionType(T4 t)
        {
            this._t4 = t;
            this._Type = typeof(T4);
            JSObject o = JSObject.CreateFrom(t);;
            this.UnderlyingJSInstance = o.UnderlyingJSInstance;
        }

        static public implicit operator UnionType<T0, T1, T2, T3, T4>(T4 t)
        {
            return new UnionType<T0, T1, T2, T3, T4>(t);
        }

        static public implicit operator T4(UnionType<T0, T1, T2, T3, T4> value)
        {
            if (value._Type == null)
            {
                value._t4 = value.CreateInstance<T4>();
                value._Type = typeof(T4);
                return value._t4;
            }
            if (value._Type == typeof(T4))
                return value._t4;
            else
                throw new Exception("Unable to cast this UnionType to " + typeof(T4).Name + " because it contains a value that is of another type.");
        }
    }
}