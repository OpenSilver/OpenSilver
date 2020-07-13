using System;

namespace DotNetBrowser
{
    public class JSValue
    {
        public virtual JSArray AsArray() { throw new NotImplementedException(); }
        public virtual JSBoolean AsBoolean() { throw new NotImplementedException(); }
        public virtual JSBooleanObject AsBooleanObject() { throw new NotImplementedException(); }
        public virtual object AsDotNetObject() { throw new NotImplementedException(); }
        public virtual JSFunction AsFunction() { throw new NotImplementedException(); }
        public virtual JSNumber AsNumber() { throw new NotImplementedException(); }
        public virtual JSNumberObject AsNumberObject() { throw new NotImplementedException(); }
        public virtual JSObject AsObject() { throw new NotImplementedException(); }
        public virtual JSString AsString() { throw new NotImplementedException(); }
        public virtual JSStringObject AsStringObject() { throw new NotImplementedException(); }
        public static JSValue Create(bool value) { throw new NotImplementedException(); }
        public static JSValue Create(double value) { throw new NotImplementedException(); }
        public static JSValue Create(string value) { throw new NotImplementedException(); }
        public static JSONString CreateJSON(string jsonString) { throw new NotImplementedException(); }
        public static JSValue CreateNull() { throw new NotImplementedException(); }
        public static JSValue CreateUndefined() { throw new NotImplementedException(); }
        public virtual bool GetBool() { throw new NotImplementedException(); }
        public virtual double GetNumber() { throw new NotImplementedException(); }
        public virtual string GetString() { throw new NotImplementedException(); }
        public virtual bool IsArray() { throw new NotImplementedException(); }
        public virtual bool IsBool() { throw new NotImplementedException(); }
        public virtual bool IsBooleanObject() { throw new NotImplementedException(); }
        public virtual bool IsDotNetObject() { throw new NotImplementedException(); }
        public bool IsFalse() { throw new NotImplementedException(); }
        public virtual bool IsFunction() { throw new NotImplementedException(); }
        public virtual bool IsJSON() { throw new NotImplementedException(); }
        public virtual bool IsNull() { throw new NotImplementedException(); }
        public virtual bool IsNumber() { throw new NotImplementedException(); }
        public virtual bool IsNumberObject() { throw new NotImplementedException(); }
        public virtual bool IsObject() { throw new NotImplementedException(); }
        public virtual bool IsString() { throw new NotImplementedException(); }
        public virtual bool IsStringObject() { throw new NotImplementedException(); }
        public bool IsTrue() { throw new NotImplementedException(); }
        public virtual bool IsUndefined() { throw new NotImplementedException(); }
    }
}
