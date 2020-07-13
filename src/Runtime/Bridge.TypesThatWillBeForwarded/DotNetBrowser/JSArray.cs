using System;
using System.Reflection;

namespace DotNetBrowser
{
    public class JSArray : JSObject
    {
        public int Count { get { throw new NotImplementedException(); } }

        public JSValue this[int index] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override JSArray AsArray() { throw new NotImplementedException(); }
        public override bool IsArray() { throw new NotImplementedException(); }
    }
}
