
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
    public abstract class UnionType : JSObject
    {
        protected Type _Type { get; set; }

        public bool instanceof(string type)
        {
            return Convert.ToBoolean(Interop.ExecuteJavaScript("$0 instanceof $1", this.UnderlyingJSInstance, type));
        }

        public T CreateInstance<T>()
        {
            if (typeof(T) == typeof(double))
                return (T)(object)Convert.ToDouble(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(string))
                return (T)(object)Convert.ToString(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(bool))
                return (T)(object)Convert.ToBoolean(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(object))
                return (T)this.UnderlyingJSInstance;
            else
                return (T)Activator.CreateInstance(typeof(T), this.UnderlyingJSInstance);
        }
    }
}