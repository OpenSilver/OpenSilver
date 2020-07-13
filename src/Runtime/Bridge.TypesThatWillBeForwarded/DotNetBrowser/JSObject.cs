using System;
using System.Collections.Generic;

namespace DotNetBrowser
{
    public class JSObject : JSValue
    {
        protected JSContext context;

        public JSContext Context { get { throw new NotImplementedException(); } }
        public bool IsDisposed { get { throw new NotImplementedException(); } }

        public override object AsDotNetObject() { throw new NotImplementedException(); }
        public override JSObject AsObject() { throw new NotImplementedException(); }
        public List<string> GetOwnPropertyNames() { throw new NotImplementedException(); }
        public JSValue GetProperty(string name) { throw new NotImplementedException(); }
        public List<string> GetPropertyNames() { throw new NotImplementedException(); }
        public bool HasProperty(string name) { throw new NotImplementedException(); }
        public override bool IsDotNetObject() { throw new NotImplementedException(); }
        public override bool IsObject() { throw new NotImplementedException(); }
        public bool RemoveProperty(string name) { throw new NotImplementedException(); }
        public bool SetProperty(int index, object value) { throw new NotImplementedException(); }
        public bool SetProperty(string name, object value) { throw new NotImplementedException(); }
        public string ToJSONString() { throw new NotImplementedException(); }
    }
}
