using System;

namespace DotNetBrowser
{
    public abstract class JSPrimitive<T> : JSValue
    {
        protected JSPrimitive() { throw new NotImplementedException(); }

        public abstract T Value { get; }

        public override string ToString() { throw new NotImplementedException(); }
    }
}
