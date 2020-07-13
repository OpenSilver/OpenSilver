using System;

namespace DotNetBrowser
{
    public class JSBoolean : JSPrimitive<bool>
    {
        public override bool Value { get { throw new NotImplementedException(); } }

        public override JSBoolean AsBoolean() { throw new NotImplementedException(); }
        public override bool GetBool() { throw new NotImplementedException(); }
        public override bool IsBool() { throw new NotImplementedException(); }
    }
}
