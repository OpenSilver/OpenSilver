using System;

namespace DotNetBrowser
{
    public class JSFunction : JSObject
    {
        public override JSFunction AsFunction() { throw new NotImplementedException(); }
        public void Invoke(JSObject instance, params object[] args) { throw new NotImplementedException(); }
        public JSValue InvokeAndReturnValue(JSObject instance, params object[] args) { throw new NotImplementedException(); }
        public override bool IsFunction() { throw new NotImplementedException(); }
    }
}
