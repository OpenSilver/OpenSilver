

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

namespace TypeScriptDefinitionsSupport
{
    public abstract class UnionType : JSObject
    {
        protected Type _Type { get; set; }

        public bool instanceof(string type)
        {
            return Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0 instanceof $1", this.UnderlyingJSInstance, type));
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