using System;

namespace DotNetBrowser
{
    public class JSNumber : JSPrimitive<double>
    {
        public override double Value { get { throw new NotImplementedException(); } }

        public override JSNumber AsNumber() { throw new NotImplementedException(); }
        public override double GetNumber() { throw new NotImplementedException(); }
        public override bool IsNumber() { throw new NotImplementedException(); }
    }
}
