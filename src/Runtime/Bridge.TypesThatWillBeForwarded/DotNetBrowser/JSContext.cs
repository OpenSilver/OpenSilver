using System;

namespace DotNetBrowser
{
    public class JSContext : IDisposable
    {
        public long FrameId { get { throw new NotImplementedException(); } }
        public bool IsDisposed { get { throw new NotImplementedException(); } }
        public long WorldId { get { throw new NotImplementedException(); } }

        public event EventHandler DisposeEvent;

        public JSObject CreateObject() { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        protected void Dispose(bool disposing) { throw new NotImplementedException(); }
        public override bool Equals(object obj) { throw new NotImplementedException(); }
        public override int GetHashCode() { throw new NotImplementedException(); }
    }
}
