using System;

namespace DotNetBrowser
{
    public class JSNumberObject : JSObject
    {
        public double ValueOf { get { throw new NotImplementedException(); } }

        public override JSNumberObject AsNumberObject() { throw new NotImplementedException(); }
        public override double GetNumber() { throw new NotImplementedException(); }
        public override bool IsNumberObject() { throw new NotImplementedException(); }
    }
}
