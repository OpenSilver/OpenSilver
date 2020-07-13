using System;

namespace DotNetBrowser
{
    public class JSStringObject : JSObject
    {
        public string ValueOf { get { throw new NotImplementedException(); } }

        public override JSStringObject AsStringObject() { throw new NotImplementedException(); }
        public override string GetString() { throw new NotImplementedException(); }
        public override bool IsStringObject() { throw new NotImplementedException(); }
    }
}
