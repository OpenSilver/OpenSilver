
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ? This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ? You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ? Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
