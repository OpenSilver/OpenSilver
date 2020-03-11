

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
using System.Collections;
using TypeScriptDefinitionsSupport;

namespace ToJavaScriptObjectExtender
{
    public static class ToJavaScriptObjectExtender
    {
        public static object ToJavaScriptObject(this object o)
        {
            return o;
        }

        public static object ToJavaScriptObject(this IJSObject o)
        {
            return o.UnderlyingJSInstance;
        }

        public static object ToJavaScriptObject(this string S)
        {
            return (object)S;
        }

        public static object ToJavaScriptObject(this double D)
        {
            return (object)D;
        }

        public static object ToJavaScriptObject(this bool B)
        {
            return (object)B;
        }

        public static Action ToJavaScriptObject(this Action a)
        {
            return a;
        }

        public static object ToJavaScriptObject(this IJSObject[] a)
        {
            return (object)a;
        }
    }
}
