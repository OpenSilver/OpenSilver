using System;

namespace DotNetBrowser
{
    public class JSBooleanObject : JSObject
    {
        public bool ValueOf { get { throw new NotImplementedException(); } }

        public override JSBooleanObject AsBooleanObject() { throw new NotImplementedException(); }
        public override bool GetBool() { throw new NotImplementedException(); }
        public override bool IsBooleanObject() { throw new NotImplementedException(); }
    }
}
