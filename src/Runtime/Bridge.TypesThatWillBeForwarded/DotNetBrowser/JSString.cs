using System;

namespace DotNetBrowser
{
    public class JSString : JSPrimitive<string>
    {
        public override string Value { get { throw new NotImplementedException(); } }

        public override JSString AsString() { throw new NotImplementedException(); }
        public override string GetString() { throw new NotImplementedException(); }
        public override bool IsString() { throw new NotImplementedException(); }
    }
}
